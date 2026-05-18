using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using LotTraceApp.Models;
using LotTraceApp.Repositories;

using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace LotTraceApp.Services
{
    public sealed class TraceProgressState
    {
        public TraceProgressState(string message, int? percent = null)
        {
            Message = message;
            Percent = percent;
        }

        public string Message { get; private set; }
        public int? Percent { get; private set; }
    }

    public class LotTraceService
    {
        private readonly LotTraceRepository _repo;
        private const int MaxDepth = 255;
        private readonly ICustomerItemMasterRepository _customerItemMasterRepository;

        // 追加：サービス内一時保持
        // BuildDisplayResult では枝構造を直接持てないため、
        // フォワード構造確定直後の occupied 情報を一時保持して
        // UI契約プロパティ(IsFirst/IsLastRowOfStartGroup)の設定にのみ使用する。
        private ForwardDisplayLaneBuildResult _currentForwardLaneBuildResult;
        private Dictionary<string, List<BackwardParentCandidate>> _currentBackwardCandidatesByChildNodeKey;
        private List<BackwardParentSetCandidateGroup> _currentBackwardCandidateGroups;
        private Dictionary<string, List<ChildCandidate>> _currentForwardCandidatesByParentNodeKey;
        public LotTraceService(LotTraceRepository repo, ICustomerItemMasterRepository customerItemMasterRepository)
        {
            if (repo == null) throw new ArgumentNullException("repo");
            _repo = repo;
            _customerItemMasterRepository = customerItemMasterRepository;
        }

        public TraceResult ExecuteTrace(
            TraceSearchParameters p,
            IProgress<TraceProgressState> progress,
            CancellationToken cancellationToken)
        {
            if (p == null) throw new ArgumentNullException("p");
            cancellationToken.ThrowIfCancellationRequested();

            if (p.Direction == TraceDirection.Forward)
            {
                return TraceForward(p, progress, cancellationToken);
            }
            else
            {
                return TraceBackward(p, progress, cancellationToken);
            }
        }
       
        public DataTable BuildDisplayTable(
            TraceDisplayResult displayResult,
            IProgress<TraceProgressState> progress,
            CancellationToken cancellationToken)
        {
            ReportProgress(progress, "グリッド列を準備しています...", 80);
            return BuildDisplayTableFromDisplayResult(displayResult, cancellationToken);
        }

        public TraceDisplayResult BuildDisplayResult(
            TraceResult traceResult,
            TraceDisplayBuildOptions options = null,
            IProgress<TraceProgressState> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var displayResult = new TraceDisplayResult();
            displayResult.Options = options ?? new TraceDisplayBuildOptions();

            if (traceResult == null || traceResult.PathRows == null || traceResult.PathRows.Count == 0)
                return displayResult;

            ReportProgress(progress, "表示行を変換しています...", 68);

            for (int rowIndex = 0; rowIndex < traceResult.PathRows.Count; rowIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var pathRow = traceResult.PathRows[rowIndex];
                if (pathRow == null)
                    continue;



                var row = new TraceDisplayRow
                {
                    DisplayOrder = rowIndex,
                    RouteSystem = ResolveRouteSystem(pathRow),
                    IsDisplayTarget = true,
                    SuppressReason = null
                };

                // Start
                row.Start = CreateDisplayCell(
                    pathRow,
                    pathRow.StartNode,
                    TraceDisplayColumnKind.Start,
                    0,
                    0);

                // Middle
                if (pathRow.MiddleNodes != null)
                {
                    for (int i = 0; i < pathRow.MiddleNodes.Count; i++)
                    {
                        row.Middles.Add(CreateDisplayCell(
                            pathRow,
                            pathRow.MiddleNodes[i],
                            TraceDisplayColumnKind.Middle,
                            i + 1,
                            i + 1));
                    }
                }

                // End
                int endNodeIndex = (pathRow.MiddleNodes == null ? 0 : pathRow.MiddleNodes.Count) + 1;
                row.End = CreateDisplayCell(
                    pathRow,
                    pathRow.EndNode,
                    TraceDisplayColumnKind.End,
                    -1,
                    endNodeIndex);

                // Startグループ情報を引き継ぐ
                row.IsLastRowOfStartGroup = false;

                // Lotグループ情報もそのまま引き継ぐ
                if (pathRow.LevelLotGroups != null)
                {
                    foreach (var g in pathRow.LevelLotGroups)
                    {
                        if (g != null)
                        {
                            row.LevelLotGroups.Add(g);
                        }
                    }
                }

                displayResult.Rows.Add(row);
            }

            // 並びは PathRows 確定順をそのまま使う
            int maxMiddleDepthFromRows = displayResult.Rows.Count == 0
                ? 0
                : displayResult.Rows.Max(r => r == null ? 0 : r.Middles.Count);

            bool hasMiddleLineRange = traceResult != null &&
                traceResult.TraceLineRanges != null &&
                traceResult.TraceLineRanges.Any(x =>
                    x != null &&
                    string.Equals(x.GridKind, "Middle", StringComparison.OrdinalIgnoreCase));

            displayResult.MaxMiddleDepth = hasMiddleLineRange
                ? Math.Max(1, maxMiddleDepthFromRows)
                : maxMiddleDepthFromRows;

            ReportProgress(progress, "グリッド罫線情報を整えています...", 74);

            MarkStartGroupBoundariesFromOccupiedRanges(
                displayResult.Rows,
                _currentForwardLaneBuildResult == null
                    ? null
                    : _currentForwardLaneBuildResult.OccupiedLotGroupRanges);

            PopulateLineRanges(displayResult, traceResult.TraceLineRanges);



            return displayResult;

        }

        private static void ReportProgress(IProgress<TraceProgressState> progress, string message, int? percent = null)
        {
            if (progress == null || string.IsNullOrWhiteSpace(message))
                return;

            progress.Report(new TraceProgressState(message, percent));
        }

        #region トレースフォワード（仕様書 7.1）

        private TraceResult TraceForward(
            TraceSearchParameters p,
            IProgress<TraceProgressState> progress,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new TraceResult();

            // 今回は Lv1 の枝構造確認用。
            // Candidate はここで確定し、この先では変更しない。
            _currentForwardLaneBuildResult = null;
            _currentForwardCandidatesByParentNodeKey =
                new Dictionary<string, List<ChildCandidate>>(StringComparer.OrdinalIgnoreCase);

            ReportProgress(progress, "トレースフォワードの始点を探索しています...", 15);
            var laneBuildResult = BuildForwardDisplayLaneNodes(p);
            cancellationToken.ThrowIfCancellationRequested();
            _currentForwardLaneBuildResult = laneBuildResult;

            if (laneBuildResult == null ||
                laneBuildResult.DisplayNodes == null ||
                laneBuildResult.DisplayNodes.Count == 0)
            {
                return result;
            }
            
           
            // ------------------------------------------------------------
            // 4. 完成した DisplayLaneNode に対して品名解決
            // ------------------------------------------------------------
            ReportProgress(progress, "品目名と開始日時を解決しています...", 48);
            ResolveItemNamesDisplayNodes(laneBuildResult.DisplayNodes);
            cancellationToken.ThrowIfCancellationRequested();
            ResolveStartDateLabelsForDisplayLaneNodes(laneBuildResult.DisplayNodes);

            // ------------------------------------------------------------
            // 5. lane → PathRows
            //    UI契約は変えないので既存橋渡しを使う
            // ------------------------------------------------------------

            result.TraceLineRanges.Clear();

            ReportProgress(progress, "罫線を描画しています...", 58);
            var traceLineRanges = BuildForwardLineRanges(laneBuildResult.DisplayNodes);
            cancellationToken.ThrowIfCancellationRequested();

            AppendForwardOccupiedMiddleLineRanges(traceLineRanges,laneBuildResult);

            AppendSyntheticMiddleLineRangesForDirectStartEndBranches(traceLineRanges,laneBuildResult);

            result.TraceLineRanges.AddRange(traceLineRanges);

            ReportProgress(progress, "検索結果行を作成しています...", 64);
            BuildPathRowsFromDisplayLaneNodes(result, laneBuildResult);

            return result;
        }

        #endregion

        #region トレースバック（仕様書 7.2）
        /// <summary>
        /// 枝構造化に向けて工事中。新中間モデル適用版
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private TraceResult TraceBackward(
            TraceSearchParameters p,
            IProgress<TraceProgressState> progress,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new TraceResult();

            var nodeMap = new Dictionary<string, ProductionResultNode>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<ProductionResultNode>();
            var queuedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var linkMap = new Dictionary<string, ProductionResultLink>(StringComparer.OrdinalIgnoreCase);
            var backwardCandidatesByChildNodeKey =
    new Dictionary<string, List<BackwardParentCandidate>>(StringComparer.OrdinalIgnoreCase);
            var backwardLevel0Candidates = new List<BackwardParentCandidate>();
            _currentBackwardCandidateGroups = new List<BackwardParentSetCandidateGroup>();

            // 枝構造化に渡す始点一覧。
            // 通常始点(B / A Manual)に加えて、StartD の child 側もここへ積む。
            var laneStartNodes = new List<ProductionResultNode>();
            var laneStartNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


            // ------------------------------------------------------------
            // STEP1(START):
            // バック検索の開始ノード取得
            // 通常始点（B / A ManualInput）を取得する。
            // これらは Current として queue に積み、以後の親探索は BFS で広げる。
            // ------------------------------------------------------------
            ReportProgress(progress, "トレースバックの始点を探索しています...", 15);
            var startNodes = BuildBackwardStartNodes(p);
            cancellationToken.ThrowIfCancellationRequested();

            if (startNodes != null)
            {
                foreach (var raw in startNodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (raw == null)
                        continue;

                    EnsureNodeRegistered(result, raw);

                    if (queuedKeys.Add(raw.NodeIdentityKey))
                    {
                        queue.Enqueue(raw);
                    }
                }
            }

            if (queue.Count == 0)
            {
                return result;
            }

            // ------------------------------------------------------------
            // STEP1.5(START-D):
            // Drumcan始点の事前構造化
            // child(StartD始点) -> parent を Candidate として先に反映する
            // ------------------------------------------------------------
            ReportProgress(progress, "始点周辺の親候補を確認しています...", 25);
            var startDrumcanCandidates = _repo.FindBackwardStartDrumcanCandidates(p);
            cancellationToken.ThrowIfCancellationRequested();

            if (startDrumcanCandidates != null)
            {
                backwardLevel0Candidates.AddRange(startDrumcanCandidates.Where(x => x != null));

                foreach (var candidate in startDrumcanCandidates)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (candidate == null || candidate.ChildNode == null || candidate.Node == null)
                        continue;

                    var childNode = candidate.ChildNode;
                    childNode.Depth = 0;
                    childNode.ParentKey = null;
                    childNode.StartDateLabel = ResolveStartDateLabel(childNode, null);

                    EnsureNodeRegistered(result, childNode);

                    var parentNode = candidate.Node;
                    parentNode.Depth = childNode.Depth + 1;
                    parentNode.ParentKey = null;
                    parentNode.StartDateLabel = ResolveBackwardStartDateLabel(parentNode);

                    

                    // ★これが本体（構造はここだけで良い）
                    RegisterBackwardParentCandidateForDisplay(backwardCandidatesByChildNodeKey,candidate);

                    // BFSは parent 側のみ
                    if (queuedKeys.Add(parentNode.NodeIdentityKey))
                    {
                        queue.Enqueue(parentNode);
                    }
                }
            }

            if (queue.Count == 0)
            {
                return result;
            }

            // ------------------------------------------------------------
            // STEP2:
            // BFS（current = child / candidate.Node = parent）
            // ------------------------------------------------------------

            ReportProgress(progress, "親ロットを探索しています...", 35);

            while (queue.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var current = queue.Dequeue();
                if (current == null)
                    continue;

                // ★ これが必要
                var candidates = _repo.FindBackwardParentsByLotStepFlow(current, current.Depth + 1);
                if (candidates == null || candidates.Count == 0)
                    continue;

                if (current.Depth == 0)
                {
                    backwardLevel0Candidates.AddRange(candidates.Where(x => x != null));
                }

                foreach (var candidate in candidates)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (candidate == null || candidate.Node == null)
                        continue;

                    var parentNode = candidate.Node;

                    // ★ 本体（構造登録）
                    RegisterBackwardParentCandidateForDisplay(
                        backwardCandidatesByChildNodeKey, candidate);

                    // BFS展開
                    if (queuedKeys.Add(parentNode.NodeIdentityKey))
                    {
                        queue.Enqueue(parentNode);
                    }
                }
            }

            // ------------------------------------------------------------
            // STEP3:
            // 旧互換ノード一覧の再構築
            // ------------------------------------------------------------
            _currentBackwardCandidatesByChildNodeKey = backwardCandidatesByChildNodeKey;
            _currentBackwardCandidateGroups = BuildBackwardCandidateGroupsByTraversal(
                backwardLevel0Candidates,
                cancellationToken);
            // ------------------------------------------------------------
            // STEP4:
            // バック探索結果 → 枝構造化
            // ------------------------------------------------------------
            ReportProgress(progress, "トレースバックの枝構造を作成しています...", 50);
            var backwardDisplayNodeGroups = BuildBackWardDisplayNodeGroups(_currentBackwardCandidateGroups);
            var backwardLaneBuildResult = BuildBackwardDisplayLaneNodes(backwardDisplayNodeGroups, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "品目名と開始日時を解決しています...", 60);
            ResolveItemNamesDisplayNodes(backwardLaneBuildResult.DisplayNodes);
            cancellationToken.ThrowIfCancellationRequested();
            ResolveStartDateLabelsForDisplayLaneNodes(backwardLaneBuildResult.DisplayNodes);

            // ここで構造ベースの肉付け
            PopulateLineDecisionInfos(backwardLaneBuildResult.DisplayNodes);
            cancellationToken.ThrowIfCancellationRequested();


            // Occupied 確定
            backwardLaneBuildResult.OccupiedLotGroupRanges =
                NormalizeOccupiedRangeByBackwardDisplayTree(backwardLaneBuildResult.DisplayNodes);

            // 追加：BuildDisplayResult から始点境界判定に使用するため一時保持
            _currentForwardLaneBuildResult = backwardLaneBuildResult;

            // ------------------------------------------------------------
            // STEP5:
            // backward 枝剪定・終点共通X正規化
            // ------------------------------------------------------------

            //必要ならここで枝選定メソッドを呼ぶ//
            
            cancellationToken.ThrowIfCancellationRequested();

            //終端処理
            backwardLaneBuildResult.EndInfos =
                FinalizeBackwardEndNodeGroupsAndNormalizeX(
                    backwardLaneBuildResult.DisplayNodes);

            PopulateOccupiedRange(backwardLaneBuildResult.DisplayNodes);
            backwardLaneBuildResult.OccupiedLotGroupRanges =
                NormalizeOccupiedRangeByBackwardDisplayTree(backwardLaneBuildResult.DisplayNodes);

            // ★追加（構造 → 線生成）
            ReportProgress(progress, "罫線情報を作成しています...", 70);
            var traceLineRanges =
            BuildTraceLineRangesFromOccupiedGroups(
                backwardLaneBuildResult.OccupiedLotGroupRanges);
            cancellationToken.ThrowIfCancellationRequested();

            NormalizeBackwardTraceLineRangeGridKinds(traceLineRanges);

            AppendSyntheticMiddleLineRangesForDirectStartEndBranches(
                traceLineRanges,
                backwardLaneBuildResult);

            if (traceLineRanges != null && traceLineRanges.Count > 0)
            {
                result.TraceLineRanges.AddRange(traceLineRanges);
            }

            // ------------------------------------------------------------
            // STEP6:
            // 枝構造 → PathRows
            // ※ route復元ではなく、XLevel / YLane 投影で作る
            // ------------------------------------------------------------
            ReportProgress(progress, "検索結果行を作成しています...", 75);
            BuildBackwardPathRowsFromDisplayLaneNodes(result, backwardLaneBuildResult);

            

            return result;
        }


        /// <summary>
        /// 新中間モデル適用。ただリポジトリとの整合はまだ怪しいかも
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<ProductionResultNode> BuildBackwardStartNodes(TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();
            var seenMergeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var rawStarts = _repo.FindBackwardStartNodes(p);
            if (rawStarts == null || rawStarts.Count == 0)
                return result;

            foreach (var raw in rawStarts)
            {
                if (raw == null)
                    continue;

                raw.Depth = 0;
                raw.ParentKey = null;
                raw.StartDateLabel = ResolveStartDateLabel(raw, null);

                // Backward の入口文脈は repository 側を優先する
                if (!raw.InputSlotNo.HasValue)
                    raw.InputSlotNo = 0;

                if (string.IsNullOrWhiteSpace(raw.InputSourceType))
                    raw.InputSourceType = null;

                if (!raw.IsTraceTerminal)
                    raw.IsTraceTerminal = string.IsNullOrWhiteSpace(raw.LotNumber);

                string mergeKey = raw.NodeIdentityKey;

                if (string.IsNullOrWhiteSpace(mergeKey))
                {
                    result.Add(raw);
                    continue;
                }

                if (!seenMergeKeys.Add(mergeKey))
                    continue;

                result.Add(raw);
            }

            return result;
        }

        
        #endregion

        #region 表示結果変換・グリッド出力

        private sealed class DisplayLaneNode
        {
            public string DisplayNodeKey { get; set; }
            public string MergeKey { get; set; }
            public string ParentDisplayNodeKey { get; set; }

            public ProductionResultNode SourceNode { get; set; }

            // 論理座標
            public int XLevel { get; set; }
            public int YLane { get; set; }
            public int ChildIndex { get; set; }

            public int OccupiedFirstY { get; set; }      // このNode起点の占有開始Y
            public int OccupiedLastY { get; set; }       // このNode起点の占有終了Y

            public bool IsLotGroupRepresentative { get; set; }

            public string LotGroupKey { get; set; }
            public string RepresentativeNodeKey { get; set; }


            public DisplayLaneNode()
            {
                OccupiedFirstY = -1;
                OccupiedLastY = -1;
            }
        }

        private DataTable BuildDisplayTableFromDisplayResult(
            TraceDisplayResult displayResult,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var table = new DataTable();

            if (displayResult == null)
                return table;

            int maxMiddleDepth = displayResult.MaxMiddleDepth < 0
                ? 0
                : displayResult.MaxMiddleDepth;

            AddDisplayTableCommonColumns(table);
            AddDisplayTableStartColumns(table);

            for (int level = 1; level <= maxMiddleDepth; level++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                AddDisplayTableMiddleColumns(table, level);
            }

            AddDisplayTableEndColumns(table);

            if (displayResult.Rows == null || displayResult.Rows.Count == 0)
                return table;

            for (int rowIndex = 0; rowIndex < displayResult.Rows.Count; rowIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var displayRow = displayResult.Rows[rowIndex];
                if (displayRow == null)
                    continue;

                var dr = table.NewRow();

                FillDisplayTableCommonColumns(dr, displayRow, rowIndex);
                FillDisplayTableStartColumns(dr, displayRow.Start);

                for (int level = 1; level <= maxMiddleDepth; level++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var middleCell = GetMiddleCell(displayRow, level);
                    FillDisplayTableMiddleColumns(dr, level, middleCell);
                }

                FillDisplayTableEndColumns(dr, displayRow.End);

                table.Rows.Add(dr);
            }

            return table;
        }

        private void AddDisplayTableCommonColumns(DataTable table)
        {
            if (table == null)
                return;

            table.Columns.Add("RowIndex", typeof(int));
            table.Columns.Add("DisplayOrder", typeof(int));

            table.Columns.Add("RouteSystem", typeof(string));

            table.Columns.Add("IsLastRowOfStartGroup", typeof(bool));

            table.Columns.Add("IsDisplayTarget", typeof(bool));
            table.Columns.Add("SuppressReason", typeof(string));
            table.Columns.Add("IsPruned", typeof(bool));
            table.Columns.Add("PruneReason", typeof(string));
        }

        private void AddDisplayTableStartColumns(DataTable table)
        {
            if (table == null)
                return;

            table.Columns.Add("Start_Exists", typeof(bool));

            table.Columns.Add("Start_NodeKey", typeof(string));
            table.Columns.Add("Start_MasterKey", typeof(string));
            table.Columns.Add("Start_Order", typeof(string));
            table.Columns.Add("Start_Lot", typeof(string));
            table.Columns.Add("Start_ItemCode", typeof(string));
            table.Columns.Add("Start_ItemName", typeof(string));
            table.Columns.Add("Start_StartTime", typeof(string));
            table.Columns.Add("Start_StartDateLabel", typeof(string));
            table.Columns.Add("Start_Weight", typeof(decimal));

            table.Columns.Add("Start_LotGroupKey", typeof(string));
            table.Columns.Add("Start_IsRepresentativeInLotGroup", typeof(bool));
            table.Columns.Add("Start_BranchSignature", typeof(string));
        }


        private void AddDisplayTableMiddleColumns(DataTable table, int level)
        {
            if (table == null || level <= 0)
                return;

            string prefix = "Lv" + level + "_";

            table.Columns.Add(prefix + "Exists", typeof(bool));

            table.Columns.Add(prefix + "NodeKey", typeof(string));
            table.Columns.Add(prefix + "MasterKey", typeof(string));
            table.Columns.Add(prefix + "Order", typeof(string));
            table.Columns.Add(prefix + "Lot", typeof(string));
            table.Columns.Add(prefix + "ItemCode", typeof(string));
            table.Columns.Add(prefix + "ItemName", typeof(string));
            table.Columns.Add(prefix + "StartTime", typeof(string));
            table.Columns.Add(prefix + "StartDateLabel", typeof(string));
            table.Columns.Add(prefix + "Weight", typeof(decimal));

            table.Columns.Add(prefix + "LotGroupKey", typeof(string));
            table.Columns.Add(prefix + "IsRepresentativeInLotGroup", typeof(bool));
            table.Columns.Add(prefix + "BranchSignature", typeof(string));
        }

        private void AddDisplayTableEndColumns(DataTable table)
        {
            if (table == null)
                return;

            table.Columns.Add("End_Exists", typeof(bool));

            table.Columns.Add("End_NodeKey", typeof(string));
            table.Columns.Add("End_MasterKey", typeof(string));
            table.Columns.Add("End_Order", typeof(string));
            table.Columns.Add("End_Lot", typeof(string));
            table.Columns.Add("End_ItemCode", typeof(string));
            table.Columns.Add("End_ItemName", typeof(string));
            table.Columns.Add("End_StartTime", typeof(string));
            table.Columns.Add("End_StartDateLabel", typeof(string));
            table.Columns.Add("End_Weight", typeof(decimal));

            table.Columns.Add("End_LotGroupKey", typeof(string));
            table.Columns.Add("End_IsRepresentativeInLotGroup", typeof(bool));
            table.Columns.Add("End_BranchSignature", typeof(string));

            // ★追加
            table.Columns.Add("End_IsDuplicate", typeof(bool));
            table.Columns.Add("End_DuplicateDisplayGroupIndex", typeof(int));
        }

        private void FillDisplayTableCommonColumns(DataRow row, TraceDisplayRow displayRow, int rowIndex)
        {
            if (row == null || displayRow == null)
                return;

            row["RowIndex"] = rowIndex;
            row["DisplayOrder"] = displayRow.DisplayOrder;

            SetValue(row, "RouteSystem", displayRow.RouteSystem);

            row["IsLastRowOfStartGroup"] = displayRow.IsLastRowOfStartGroup;

            row["IsDisplayTarget"] = displayRow.IsDisplayTarget;
            SetValue(row, "SuppressReason", displayRow.SuppressReason);
            row["IsPruned"] = displayRow.IsPruned;
            SetValue(row, "PruneReason", displayRow.PruneReason);
        }

        private void FillDisplayTableStartColumns(DataRow row, TraceDisplayCell cell)
        {
            FillDisplayTableCell(row, "Start_", cell);
        }

        private void FillDisplayTableMiddleColumns(DataRow row, int level, TraceDisplayCell cell)
        {
            FillDisplayTableCell(row, "Lv" + level + "_", cell);
        }

        private void FillDisplayTableEndColumns(DataRow row, TraceDisplayCell cell)
        {
            FillDisplayTableCell(row, "End_", cell);

            if (row == null || cell == null)
                return;

            row["End_IsDuplicate"] = cell.EndIsDuplicate;
            SetValue(row, "End_DuplicateDisplayGroupIndex", cell.EndDuplicateDisplayGroupIndex);
        }

        private void FillDisplayTableCell(DataRow row, string prefix, TraceDisplayCell cell)
        {
            if (row == null || string.IsNullOrWhiteSpace(prefix))
                return;

            bool exists = cell != null;
            row[prefix + "Exists"] = exists;

            if (!exists)
            {
                return;
            }

            SetValue(row, prefix + "NodeKey", cell.NodeKey);
            SetValue(row, prefix + "MasterKey", cell.MasterKey);
            SetValue(row, prefix + "Order", cell.ProductionOrderNumber);
            SetValue(row, prefix + "Lot", cell.LotNumber);
            SetValue(row, prefix + "ItemCode", cell.ItemCode);
            SetValue(row, prefix + "ItemName", cell.ItemName);

            if (cell.StartDate.HasValue)
            {
                row[prefix + "StartTime"] = cell.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else if(!cell.StartDate.HasValue)
            {
                row[prefix + "StartTime"] = cell.StartDateLabel;
            }

            SetValue(row, prefix + "StartDateLabel", cell.StartDateLabel);

            if (cell.Weight.HasValue)
            {
                row[prefix + "Weight"] = cell.Weight.Value;
            }

            SetValue(row, prefix + "LotGroupKey", cell.LotGroupKey);
            row[prefix + "IsRepresentativeInLotGroup"] = cell.IsRepresentativeInLotGroup;
            SetValue(row, prefix + "BranchSignature", cell.BranchSignature);
        }

        private void SetValue(DataRow row, string columnName, object value)
        {
            if (row == null || row.Table == null)
                return;

            if (string.IsNullOrWhiteSpace(columnName))
                return;

            if (!row.Table.Columns.Contains(columnName))
                return;

            row[columnName] = value ?? DBNull.Value;
        }


        private TraceDisplayCell GetMiddleCell(TraceDisplayRow row, int level)
        {
            if (row == null)
                return null;

            if (level <= 0)
                return null;

            if (row.Middles == null)
                return null;

            int index = level - 1;
            if (index < 0 || index >= row.Middles.Count)
                return null;

            return row.Middles[index];
        }


        private TraceDisplayCell CreateDisplayCell(
    TracePathRow pathRow,
    ProductionResultNode node,
    TraceDisplayColumnKind columnKind,
    int level,
    int nodeIndexInPath)
        {
            if (node == null)
                return null;

            var lotGroupInfo = FindLotGroupInfo(pathRow, columnKind, level);

            return new TraceDisplayCell
            {
                // DAG上の元Node
                Node = node,

                // 表示キー
                MasterKey = node.ControlMasterKey,
                NodeKey = node.NodeIdentityKey,

                ParentMasterKey = node.ParentMasterKey,

                // 座標情報
                ColumnKind = columnKind,
                Level = level,

                // 表示値
                ProductionOrderNumber = node.ProductionOrderNumber,
                LotNumber = node.LotNumber,
                ItemCode = node.ItemCode,
                ItemName = node.ItemName,
                StartDate = node.StartDate,
                StartDateLabel = node.StartDateLabel,
                Weight = node.Weight.HasValue ? (decimal?)Convert.ToDecimal(node.Weight.Value) : null,

                // フォワードLotグループ情報
                LotGroupKey = lotGroupInfo == null ? null : lotGroupInfo.GroupKey,
                IsRepresentativeInLotGroup = lotGroupInfo != null && lotGroupInfo.IsRepresentative,
                BranchSignature = lotGroupInfo == null ? null : lotGroupInfo.DownstreamBranchSignature,

                // End重複表示情報
                EndIsDuplicate =
                    columnKind == TraceDisplayColumnKind.End &&
                    pathRow != null &&
                    pathRow.EndIsDuplicate,

                EndDuplicateDisplayGroupIndex =
                    columnKind == TraceDisplayColumnKind.End && pathRow != null
                        ? pathRow.EndDuplicateDisplayGroupIndex
                        : null,

                // 表示対象
                IsVisible = true,
                SuppressReason = null
            };
        }

        
        private TraceLotGroupInfo FindLotGroupInfo(TracePathRow pathRow, TraceDisplayColumnKind columnKind, int level)
        {
            if (pathRow == null || pathRow.LevelLotGroups == null || pathRow.LevelLotGroups.Count == 0)
                return null;

            if (columnKind == TraceDisplayColumnKind.Start)
            {
                return pathRow.LevelLotGroups
                    .FirstOrDefault(g => g != null && g.Axis == TraceGroupAxis.Start && g.Level == 0);
            }

            if (columnKind == TraceDisplayColumnKind.Middle)
            {
                return pathRow.LevelLotGroups
                    .FirstOrDefault(g => g != null && g.Axis == TraceGroupAxis.Middle && g.Level == level);
            }

            return null;
        }

        private List<TracePathRow> MergeSortByStartExists(List<TracePathRow> source)
        {
            if (source == null)
                return new List<TracePathRow>();

            if (source.Count <= 1)
                return new List<TracePathRow>(source);

            int mid = source.Count / 2;

            var left = MergeSortByStartExists(source.GetRange(0, mid));
            var right = MergeSortByStartExists(source.GetRange(mid, source.Count - mid));

            return MergeByStartExists(left, right);
        }

        private List<TracePathRow> MergeByStartExists(List<TracePathRow> left, List<TracePathRow> right)
        {
            var result = new List<TracePathRow>(left.Count + right.Count);

            int i = 0;
            int j = 0;

            while (i < left.Count && j < right.Count)
            {
                int leftValue = GetStartExistsSortValue(left[i]);
                int rightValue = GetStartExistsSortValue(right[j]);

                if (leftValue <= rightValue)
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }

            while (i < left.Count)
            {
                result.Add(left[i]);
                i++;
            }

            while (j < right.Count)
            {
                result.Add(right[j]);
                j++;
            }

            return result;
        }

        private int GetStartExistsSortValue(TracePathRow row)
        {
            // 実体ありを上にする
            return (row != null && row.StartNode != null) ? 0 : 1;
        }

        
        private void BuildPathRowsRecursiveCore(
    List<TracePathRow> pathRows,
    ProductionResultNode node,
    string rootGroupKey,
    List<ProductionResultNode> currentPathNodes,
    List<ProductionResultLink> currentPathLinks,
    HashSet<string> visitedLinks)
        {
            if (pathRows == null || node == null)
                return;

            currentPathNodes.Add(node);

            var outgoingLinks = node.ChildLinks == null
                ? new List<ProductionResultLink>()
                : node.ChildLinks
                    .Where(l => l != null
                             && l.ParentNode != null
                             && l.ChildNode != null
                             && object.ReferenceEquals(l.ParentNode, node))
                    .ToList();

            if (outgoingLinks.Count == 0)
            {
                var pathRow = new TracePathRow();

                FillPathRowNodesAndLinks(pathRow, currentPathNodes, currentPathLinks);

                pathRows.Add(pathRow); // ← ここ変更

                currentPathNodes.RemoveAt(currentPathNodes.Count - 1);
                return;
            }

            foreach (var link in outgoingLinks)
            {
                string linkKey = GetPathLinkKey(link);

                if (string.IsNullOrWhiteSpace(linkKey))
                    continue;

                if (visitedLinks.Contains(linkKey))
                    continue;

                visitedLinks.Add(linkKey);
                currentPathLinks.Add(link);

                BuildPathRowsRecursiveCore(
                    pathRows,
                    link.ChildNode,
                    rootGroupKey,
                    currentPathNodes,
                    currentPathLinks,
                    visitedLinks);

                currentPathLinks.RemoveAt(currentPathLinks.Count - 1);
                visitedLinks.Remove(linkKey);
            }

            currentPathNodes.RemoveAt(currentPathNodes.Count - 1);
        }

        private void BuildPathRowsRecursive(
    TraceResult result,
    ProductionResultNode node,
    string rootGroupKey,
    List<ProductionResultNode> currentPathNodes,
    List<ProductionResultLink> currentPathLinks,
    HashSet<string> visitedLinks)
        {
            if (result == null || node == null)
                return;

            currentPathNodes.Add(node);

            var outgoingLinks = node.ChildLinks == null
                ? new List<ProductionResultLink>()
                : node.ChildLinks
                    .Where(l => l != null
                             && l.ParentNode != null
                             && l.ChildNode != null
                             && object.ReferenceEquals(l.ParentNode, node))
                    .ToList();

            

            if (outgoingLinks.Count == 0)
            {
                var pathRow = new TracePathRow();

                FillPathRowNodesAndLinks(pathRow, currentPathNodes, currentPathLinks);
                result.PathRows.Add(pathRow);

                currentPathNodes.RemoveAt(currentPathNodes.Count - 1);
                return;
            }

            bool traversedAnyChild = false;

            foreach (var link in outgoingLinks)
            {
                string linkKey = GetPathLinkKey(link);

                if (string.IsNullOrWhiteSpace(linkKey))
                {
                    continue;
                }

                if (visitedLinks.Contains(linkKey))
                {
                   continue;
                }

                visitedLinks.Add(linkKey);
                currentPathLinks.Add(link);          
                traversedAnyChild = true;

                BuildPathRowsRecursive(
                    result,
                    link.ChildNode,
                    rootGroupKey,
                    currentPathNodes,
                    currentPathLinks,
                    visitedLinks);

                currentPathLinks.RemoveAt(currentPathLinks.Count - 1);
                visitedLinks.Remove(linkKey);

            }

            if (!traversedAnyChild)
            {
                var pathRow = new TracePathRow();

                FillPathRowNodesAndLinks(pathRow, currentPathNodes, currentPathLinks);
                result.PathRows.Add(pathRow);
            }

            currentPathNodes.RemoveAt(currentPathNodes.Count - 1);
        }

        /// <summary>
        /// 手投入表示補完メソッド。今はトレースバックだけだけど、後でフォワードにも適用
        /// </summary>
        /// <param name="displayNodes"></param>
        private void ResolveStartDateLabelsForDisplayLaneNodes(List<DisplayLaneNode> displayNodes)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            foreach (var displayNode in displayNodes)
            {
                if (displayNode == null || displayNode.SourceNode == null)
                    continue;

                var node = displayNode.SourceNode;

                // 既に表示ラベルが入っているものは尊重
                if (!string.IsNullOrWhiteSpace(node.StartDateLabel))
                    continue;

                // 開始日時があるなら表示ラベル補完は不要
                if (node.StartDate.HasValue)
                    continue;

                // A系(MaterialTableA相当)の開始日時なしは「補完」
                if (string.Equals(node.RouteSystem, "A", StringComparison.OrdinalIgnoreCase))
                {
                    switch (node.InputSourceType)
                    {
                        case "ManualInput":
                            node.StartDateLabel = "手投入";
                            break;
                        case "Drumcan":
                            node.StartDateLabel = "ドラム缶";
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// TraceFowardで暫定で使う。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="candidate"></param>
        /// <returns></returns>
        private string ResolveStartDateLabel(ProductionResultNode node, ChildCandidate candidate)
        {
            if (node == null)
                return null;

            if (!string.IsNullOrWhiteSpace(node.StartDateLabel))
                return node.StartDateLabel;

            if (node.RouteSystem != "A")
                return null;

            return node.StartDate.HasValue ? null : "手投入";
        }


        
        private void EnsureNodeRegistered(TraceResult result, ProductionResultNode node)
        {
            if (result == null || node == null)
                return;

            if (!result.AllNodes.Contains(node))
            {
                result.AllNodes.Add(node);
            }
        }


        #endregion

        #region 経路行補助

        private void FillPathRowNodesAndLinks(
    TracePathRow pathRow,
    List<ProductionResultNode> currentPathNodes,
    List<ProductionResultLink> currentPathLinks)
        {
            if (pathRow == null)
                return;

            pathRow.StartNode = null;
            pathRow.EndNode = null;
            pathRow.MiddleNodes.Clear();
            pathRow.PathLinks.Clear();

            if (currentPathNodes != null && currentPathNodes.Count > 0)
            {
                pathRow.StartNode = currentPathNodes[0];
            }

            if (currentPathLinks != null)
            {
                foreach (var link in currentPathLinks)
                {
                    if (link != null)
                    {
                        pathRow.PathLinks.Add(link);
                    }
                }
            }

            if (currentPathNodes != null)
            {
                for (int i = 1; i < currentPathNodes.Count - 1; i++)
                {
                    pathRow.MiddleNodes.Add(currentPathNodes[i]);
                }
            }

            if (currentPathNodes == null || currentPathNodes.Count < 2)
            {
                pathRow.EndNode = null;
                return;
            }

            var candidateEndNode = currentPathNodes[currentPathNodes.Count - 1];

            // ------------------------------------------------------------
            // StartOnly 判定
            // 条件：
            //   - 中間ノードなし
            //   - Start と End の MergeKey が一致
            // この場合は Start 側のみに残し、EndNode は立てない
            // ------------------------------------------------------------
            bool hasMiddleNodes = pathRow.MiddleNodes.Count > 0;

            if (!hasMiddleNodes && pathRow.StartNode != null && candidateEndNode != null)
            {
                string startMergeKey = pathRow.StartNode.NodeIdentityKey;
                string endMergeKey = candidateEndNode.NodeIdentityKey;

                if (!string.IsNullOrWhiteSpace(startMergeKey) &&
                    string.Equals(startMergeKey, endMergeKey, StringComparison.OrdinalIgnoreCase))
                {
                    pathRow.EndNode = null;
                    return;
                }
            }

            pathRow.EndNode = candidateEndNode;
        }

        #endregion

        #region 経路メタ情報

        private string GetPathLinkKey(ProductionResultLink link)
        {
            if (link == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(link.LinkIdentityKey))
                return link.LinkIdentityKey;

            string parentKey = link.ParentNode.NodeIdentityKey;
            string childKey = link.ChildNode.NodeIdentityKey;

            return parentKey
                + "->"
                + childKey
                + "|"
                + link.SourceTable.ToString()
                + "|"
                + link.MaterialAInputType.ToString()
                + "|"
                + link.SlotNo.ToString()
                + "|"
                + (string.IsNullOrWhiteSpace(link.ParentLotNumber) ? string.Empty : link.ParentLotNumber.Trim().ToUpperInvariant());
        }

        private string ResolveRouteSystem(TracePathRow pathRow)
        {
            if (pathRow == null)
                return null;

            if (pathRow.PathLinks == null || pathRow.PathLinks.Count == 0)
                return null;

            var firstLink = pathRow.PathLinks.FirstOrDefault(l => l != null);
            if (firstLink == null)
                return null;

            string source = firstLink.SourceTable.ToString();

            if (string.Equals(source, "MaterialTableA", StringComparison.OrdinalIgnoreCase))
                return "A";

            if (string.Equals(source, "MaterialTableB", StringComparison.OrdinalIgnoreCase))
                return "B";

            return source;
        }

        #endregion
              
        #region 交点検出（仕様書 7.3）

        /// <summary>
        /// 複数タブのトレース結果から交点を検出
        /// key: NodeIdentityKey
        /// value: 対象タブごとの存在情報と代表ノード情報
        /// </summary>
        public List<CrossPointRecord> DetectCrossPoints(
            Dictionary<int, TraceResult> tabResults)
        {
            var dict = new Dictionary<string, CrossPointBuildEntry>(StringComparer.OrdinalIgnoreCase);

            if (tabResults == null || tabResults.Count == 0)
                return new List<CrossPointRecord>();

            foreach (var kv in tabResults)
            {
                int tabNo = kv.Key;
                var trace = kv.Value;

                var sourceNodes = EnumerateCrossPointSourceNodes(trace).ToList();
                if (sourceNodes.Count == 0)
                    continue;

                var seenNodeKeysInTab = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var node in sourceNodes)
                {
                    if (node == null)
                        continue;

                    string nodeKey = node.NodeIdentityKey;
                    if (string.IsNullOrWhiteSpace(nodeKey))
                        continue;

                    if (!seenNodeKeysInTab.Add(nodeKey))
                        continue;

                    CrossPointBuildEntry entry;
                    if (!dict.TryGetValue(nodeKey, out entry))
                    {
                        entry = new CrossPointBuildEntry
                        {
                            NodeKey = nodeKey,
                            RepresentativeNode = node
                        };
                        dict[nodeKey] = entry;
                    }

                    entry.Tabs.Add(tabNo);
                }
            }

            var records = new List<CrossPointRecord>();
            foreach (var entry in dict.Values)
            {
                var node = entry.RepresentativeNode;
                if (node == null)
                    continue;

                records.Add(new CrossPointRecord
                {
                    NodeKey = entry.NodeKey,
                    CrossPointFlag = entry.Tabs.Count >= 2 ? 1 : 0,
                    ProductionOrderNumber = node.ProductionOrderNumber,
                    LotNumber = node.LotNumber,
                    ItemName = node.ItemName,
                    StartDateText = ResolveCrossPointStartDateText(node),
                    Weight = node.Weight
                });

                var record = records[records.Count - 1];
                foreach (int tabNo in tabResults.Keys.OrderBy(x => x))
                {
                    record.TabPresence[tabNo] = entry.Tabs.Contains(tabNo) ? 1 : 0;
                }
            }

            return records
                .OrderByDescending(x => x.CrossPointFlag)
                .ThenBy(x => x.ProductionOrderNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LotNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.ItemName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.NodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private IEnumerable<ProductionResultNode> EnumerateCrossPointSourceNodes(TraceResult trace)
        {
            if (trace == null)
                yield break;

            if (trace.PathRows == null || trace.PathRows.Count == 0)
                yield break;

            foreach (var row in trace.PathRows)
            {
                if (row == null)
                    continue;

                if (row.StartNode != null)
                    yield return row.StartNode;

                if (row.MiddleNodes != null)
                {
                    foreach (var node in row.MiddleNodes)
                    {
                        if (node != null)
                            yield return node;
                    }
                }

                if (row.EndNode != null)
                    yield return row.EndNode;
            }
        }

        private sealed class CrossPointBuildEntry
        {
            public string NodeKey { get; set; }
            public ProductionResultNode RepresentativeNode { get; set; }
            public HashSet<int> Tabs { get; private set; }

            public CrossPointBuildEntry()
            {
                Tabs = new HashSet<int>();
            }
        }

        private string ResolveCrossPointStartDateText(ProductionResultNode node)
        {
            if (node == null)
                return null;

            if (node.StartDate.HasValue)
                return node.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss");

            return node.StartDateLabel;
        }

        #endregion
                
        #region 品目名解決


        private void ResolveItemNamesDisplayNodes(List<DisplayLaneNode> displayNodes)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            var targets = displayNodes
                .Where(x => x != null && x.SourceNode != null)
                .Select(x => x.SourceNode)
                .ToList();

            ResolveItemNamesForNodes(targets);
        }
        private void ResolveItemNamesForNodes(IEnumerable<ProductionResultNode> nodes)
        {
            if (nodes == null)
                return;

            var nodeList = nodes
                .Where(n => n != null)
                .ToList();

            if (nodeList.Count == 0)
                return;

            var itemCodes = nodeList
                .Where(n => !string.IsNullOrWhiteSpace(n.ItemCode))
                .Select(n => n.ItemCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (itemCodes.Count == 0)
                return;

            Dictionary<string, string> itemNameMap;

            try
            {
                itemNameMap = _customerItemMasterRepository.GetItemNamesByCodes(itemCodes);
            }
            catch
            {
                // 客先DB失敗でも本体は止めない
                return;
            }

            foreach (var node in nodeList)
            {
                if (string.IsNullOrWhiteSpace(node.ItemCode))
                    continue;

                string itemName;
                if (itemNameMap.TryGetValue(node.ItemCode.Trim(), out itemName))
                {
                    node.ItemName = itemName;
                }
            }
        }
        #endregion

        #region 表示レーンから経路行への変換


        //枝構造をPathRowに流し込む
        private void BuildPathRowsFromDisplayLaneNodes(
    TraceResult result,
    ForwardDisplayLaneBuildResult laneBuildResult)
        {
            result.PathRows.Clear();

            var endInfoByDisplayNodeKey = laneBuildResult.EndInfos
                .ToDictionary(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase);

            var displayNodes = laneBuildResult.DisplayNodes
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.XLevel)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var laneGroups = displayNodes
                .GroupBy(x => x.YLane)
                .OrderBy(g => g.Key)
                .ToList();

            int pathOrder = 0;

            foreach (var laneGroup in laneGroups)
            {
                var row = new TracePathRow();
                row.PathOrder = pathOrder++;

                var laneNodes = laneGroup
                    .OrderBy(x => x.XLevel)
                    .ThenBy(x => x.ChildIndex)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var displayNode in laneNodes)
                {
                    EndDisplayNodeInfo endInfo;

                    if (endInfoByDisplayNodeKey.TryGetValue(displayNode.DisplayNodeKey, out endInfo))
                    {
                        row.EndNode = displayNode.SourceNode;
                        row.EndIsDuplicate = endInfo.IsDuplicate;
                        row.EndDuplicateDisplayGroupIndex = endInfo.DuplicateDisplayIndex;
                        continue;
                    }

                    if (displayNode.XLevel == 0)
                    {
                        row.StartNode = displayNode.SourceNode;
                        continue;
                    }

                    int middleIndex = displayNode.XLevel - 1;

                    EnsureMiddleNodeCapacity(row, middleIndex + 1);
                    row.MiddleNodes[middleIndex] = displayNode.SourceNode;
                }

                result.PathRows.Add(row);
            }
        }


        private void EnsureMiddleNodeCapacity(TracePathRow row, int requiredCount)
        {
            if (row == null)
                return;

            while (row.MiddleNodes.Count < requiredCount)
            {
                row.MiddleNodes.Add(null);
            }
        }

        private void TrimTrailingNullMiddleNodes(TracePathRow row)
        {
            if (row == null)
                return;

            if (row.MiddleNodes == null || row.MiddleNodes.Count == 0)
                return;

            for (int i = row.MiddleNodes.Count - 1; i >= 0; i--)
            {
                if (row.MiddleNodes[i] != null)
                    break;

                row.MiddleNodes.RemoveAt(i);
            }
        }

        private sealed class EndDisplayNodeInfo
        {
            public string DisplayNodeKey { get; set; }

            public bool IsDuplicate { get; set; }

            public int? DuplicateDisplayIndex { get; set; }
        }

        private sealed class ForwardDisplayLaneBuildResult
        {
            public List<DisplayLaneNode> DisplayNodes { get; set; }
            public List<EndDisplayNodeInfo> EndInfos { get; set; }
            // 追加
            public List<OccupiedLotGroupRange> OccupiedLotGroupRanges { get; set; }

            


            public ForwardDisplayLaneBuildResult()
            {
                DisplayNodes = new List<DisplayLaneNode>();
                EndInfos = new List<EndDisplayNodeInfo>();
                OccupiedLotGroupRanges = new List<OccupiedLotGroupRange>();

            }
        }

        private string BuildDisplayLaneNodeKey(ProductionResultNode node, int xLevel, int yLane)
        {
            
            return string.Join("|",
                "DISPLAYNODE",
                xLevel.ToString(),
                yLane.ToString(),
                node.NodeIdentityKey);
        }

        private string NormalizeDisplayLaneLot(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                return null;

            return lotNumber.Trim().ToUpperInvariant();
        }

        #endregion

        #region 罫線・占有範囲


        private void PopulateOccupiedRange(List<DisplayLaneNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            var childrenMap = new Dictionary<string, List<DisplayLaneNode>>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in nodes)
            {
                if (node == null)
                    continue;

                if (string.IsNullOrWhiteSpace(node.ParentDisplayNodeKey))
                    continue;

                List<DisplayLaneNode> list;
                if (!childrenMap.TryGetValue(node.ParentDisplayNodeKey, out list))
                {
                    list = new List<DisplayLaneNode>();
                    childrenMap[node.ParentDisplayNodeKey] = list;
                }

                list.Add(node);
            }

            var ordered = nodes
                .Where(n => n != null)
                .OrderByDescending(n => n.XLevel)
                .ThenByDescending(n => n.YLane)
                .ToList();

            foreach (var node in ordered)
            {
                List<DisplayLaneNode> children;
                if (!childrenMap.TryGetValue(node.DisplayNodeKey, out children) || children.Count == 0)
                {
                    node.OccupiedFirstY = node.YLane;
                    node.OccupiedLastY = node.YLane;
                    continue;
                }

                int first = node.YLane;
                int last = node.YLane;

                foreach (var child in children)
                {
                    if (child == null)
                        continue;

                    if (child.OccupiedFirstY >= 0)
                        first = Math.Min(first, child.OccupiedFirstY);

                    if (child.OccupiedLastY >= 0)
                        last = Math.Max(last, child.OccupiedLastY);
                }

                node.OccupiedFirstY = first;
                node.OccupiedLastY = last;
            }
        }

        private sealed class OccupiedLotGroupRange
        {
            public int Level { get; set; }
            public string ParentDisplayNodeKey { get; set; }
            public string LotGroupKey { get; set; }
            public int OccupiedFirstY { get; set; }
            public int OccupiedLastY { get; set; }
        }

        
        private void PopulateLineRanges(
    TraceDisplayResult displayResult,
    IList<TraceLineRange> sourceLineRanges)
        {
            if (displayResult == null)
                return;

            displayResult.LineRanges.Clear();

            if (sourceLineRanges == null || sourceLineRanges.Count == 0)
                return;

            foreach (var range in sourceLineRanges)
            {
                if (range == null)
                    continue;

                displayResult.LineRanges.Add(range);
            }
        }

        
    private List<TraceLineRange> BuildTraceLineRangesFromOccupiedGroups(
    IList<OccupiedLotGroupRange> occupiedGroups)
{
            var result = new List<TraceLineRange>();

            if (occupiedGroups == null || occupiedGroups.Count == 0)
                return result;

            var validGroups = occupiedGroups
                .Where(x =>
                    x != null &&
                    x.Level >= 0 &&
                    x.OccupiedFirstY >= 0 &&
                    x.OccupiedLastY >= 0)
                .ToList();

            if (validGroups.Count == 0)
                return result;

            var usedLevels = validGroups
                .Select(x => x.Level)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (usedLevels.Count == 0)
                return result;

            int maxLevel = usedLevels[usedLevels.Count - 1];
            int middleMaxX = Math.Max(1, maxLevel);

            foreach (var group in validGroups
                .OrderBy(x => x.Level)
                .ThenBy(x => x.OccupiedLastY)
                .ThenBy(x => x.ParentDisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LotGroupKey, StringComparer.OrdinalIgnoreCase))
            {
                result.Add(new TraceLineRange
                {
                    GridKind = group.Level == 0 ? "Start" : "Middle",
                    LineKind = group.Level == 0 ? "Start" : group.Level == 1 ? "Trunk" : "Branch",
                    Level = group.Level,
                    StartRowIndex = group.OccupiedLastY,
                    EndRowIndex = group.OccupiedLastY,
                    FromXLevel = group.Level,
                    ToXLevel = group.Level == 0 ? middleMaxX : Math.Max(group.Level, middleMaxX)
                });
            }

            DeduplicateTraceLineRanges(result);
            return result;
        }


        private void MarkStartGroupBoundariesFromOccupiedRanges(
    List<TraceDisplayRow> rows,
    IList<OccupiedLotGroupRange> occupiedRanges)
        {
            if (rows == null || rows.Count == 0)
                return;

            // 初期化
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row == null)
                    continue;

                row.IsLastRowOfStartGroup = false;
            }

            if (occupiedRanges == null || occupiedRanges.Count == 0)
                return;

            foreach (var group in occupiedRanges)
            {
                if (group == null)
                    continue;

                if (group.Level != 0)
                    continue;

                int firstY = group.OccupiedFirstY;
                int lastY = group.OccupiedLastY;

                if (firstY >= 0 && firstY < rows.Count)
                {
                    var firstRow = rows[firstY];
                    if (firstRow != null)
                    {
                    }
                }

                if (lastY >= 0 && lastY < rows.Count)
                {
                    var lastRow = rows[lastY];
                    if (lastRow != null)
                    {
                        lastRow.IsLastRowOfStartGroup = true;
                    }
                }
            }
        }

        private void AppendSyntheticMiddleLineRangesForDirectStartEndBranches(
    IList<TraceLineRange> lineRanges,
    ForwardDisplayLaneBuildResult laneBuildResult)
        {
            if (lineRanges == null)
                return;

            if (laneBuildResult == null ||
                laneBuildResult.DisplayNodes == null ||
                laneBuildResult.DisplayNodes.Count == 0)
            {
                return;
            }

            var displayNodes = laneBuildResult.DisplayNodes
                .Where(x => x != null)
                .ToList();

            if (displayNodes.Count == 0)
                return;

            var endInfos = laneBuildResult.EndInfos ?? new List<EndDisplayNodeInfo>();
            if (endInfos.Count == 0)
                return;

            var nodeByKey = displayNodes
                .Where(x => !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            int syntheticMiddleToXLevel =
                ResolveSyntheticMiddleToXLevelFromDisplayNodes(lineRanges, displayNodes);

            if (syntheticMiddleToXLevel < 1)
                return;

            var occupiedRanges = laneBuildResult.OccupiedLotGroupRanges ?? new List<OccupiedLotGroupRange>();

            var candidateRows = new List<int>();

            foreach (var endInfo in endInfos)
            {
                if (endInfo == null)
                    continue;

                if (string.IsNullOrWhiteSpace(endInfo.DisplayNodeKey))
                    continue;

                DisplayLaneNode endNode;
                if (!nodeByKey.TryGetValue(endInfo.DisplayNodeKey, out endNode) || endNode == null)
                    continue;

                // 終端Node自身の occupied は無視し、
                // それ以外に左側 Middle occupied があるかだけを見る
                if (HasEffectiveMiddleOccupiedForTerminalBranchExcludingSelf(endNode, occupiedRanges))
                    continue;

                int rowIndex = endNode.YLane;
                if (rowIndex < 0)
                    continue;

                candidateRows.Add(rowIndex);
            }

            if (candidateRows.Count == 0)
                return;

            foreach (var block in BuildContiguousRowBlocks(candidateRows))
            {
                int boundaryRow = block.Item2;

                lineRanges.Add(new TraceLineRange
                {
                    GridKind = "Middle",
                    LineKind = "Trunk",
                    Level = 1,
                    StartRowIndex = boundaryRow,
                    EndRowIndex = boundaryRow,
                    FromXLevel = 1,
                    ToXLevel = syntheticMiddleToXLevel
                });
            }

            DeduplicateTraceLineRanges(lineRanges);
        }
        private bool HasEffectiveMiddleOccupiedForTerminalBranchExcludingSelf(
    DisplayLaneNode endNode,
    IList<OccupiedLotGroupRange> occupiedRanges)
        {
            if (endNode == null)
                return false;

            if (occupiedRanges == null || occupiedRanges.Count == 0)
                return false;

            // 終端が Lv0/Lv1 相当なら、その左側 Middle は存在しない
            if (endNode.XLevel <= 1)
                return false;

            int rowIndex = endNode.YLane;
            if (rowIndex < 0)
                return false;

            foreach (var range in occupiedRanges)
            {
                if (range == null)
                    continue;

                // Lv0 は始点グループなので除外
                if (range.Level <= 0)
                    continue;

                // 終端Node自身の occupied は除外
                if (range.Level >= endNode.XLevel)
                    continue;

                // より左側の occupied がこの終端rowをまたいでいれば、
                // その経路は Middle を占有している
                if (rowIndex >= range.OccupiedFirstY && rowIndex <= range.OccupiedLastY)
                    return true;
            }

            return false;
        }

        private List<Tuple<int, int>> BuildContiguousRowBlocks(IList<int> rowIndices)
        {
            var result = new List<Tuple<int, int>>();

            if (rowIndices == null || rowIndices.Count == 0)
                return result;

            var ordered = rowIndices
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (ordered.Count == 0)
                return result;

            int blockStart = ordered[0];
            int blockEnd = ordered[0];

            for (int i = 1; i < ordered.Count; i++)
            {
                int row = ordered[i];

                if (row == blockEnd + 1)
                {
                    blockEnd = row;
                    continue;
                }

                result.Add(Tuple.Create(blockStart, blockEnd));
                blockStart = row;
                blockEnd = row;
            }

            result.Add(Tuple.Create(blockStart, blockEnd));

            return result;
        }
        private List<OccupiedLotGroupRange> NormalizeOccupiedRangeByCandidate(List<DisplayLaneNode> nodes)
        {
            var result = new List<OccupiedLotGroupRange>();

            if (nodes == null || nodes.Count == 0)
                return result;

            // ------------------------------------------------------------
            // Backward では
            //   枝の起点 = Candidate
            //   枝の範囲 = DisplayNode.Occupied
            // で扱う。
            //
            // つまり
            //   ChildNodeKey -> Candidate群
            // を起点に、
            //   childDisplayNode の直下にぶら下がる parentDisplayNode
            // を特定し、その parentDisplayNode 自身の Occupied を
            // その枝の occupied range とみなす。
            // ------------------------------------------------------------

            var validNodes = nodes
                .Where(n =>
                    n != null &&
                    !string.IsNullOrWhiteSpace(n.DisplayNodeKey) &&
                    n.SourceNode != null &&
                    n.OccupiedFirstY >= 0 &&
                    n.OccupiedLastY >= 0)
                .ToList();

            if (validNodes.Count == 0)
                return result;

            int endXLevel = nodes
                .Where(x => x != null)
                .Select(x => x.XLevel)
                .DefaultIfEmpty(0)
                .Max();

            
            // DisplayNodeKey -> children
            var childrenMap = BuildBackwardChildrenMap(validNodes);

            // NodeIdentityKey -> DisplayNode群
            var displayNodesByNodeKey = new Dictionary<string, List<DisplayLaneNode>>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var node in validNodes)
            {
                string nodeKey = node.SourceNode.NodeIdentityKey;
                if (string.IsNullOrWhiteSpace(nodeKey))
                    continue;

                List<DisplayLaneNode> list;
                if (!displayNodesByNodeKey.TryGetValue(nodeKey, out list))
                {
                    list = new List<DisplayLaneNode>();
                    displayNodesByNodeKey[nodeKey] = list;
                }

                list.Add(node);
            }

            foreach (var pair in displayNodesByNodeKey)
            {
                pair.Value.Sort((a, b) =>
                {
                    if (ReferenceEquals(a, b))
                        return 0;
                    if (a == null)
                        return 1;
                    if (b == null)
                        return -1;

                    int cmp = a.XLevel.CompareTo(b.XLevel);
                    if (cmp != 0)
                        return cmp;

                    cmp = a.YLane.CompareTo(b.YLane);
                    if (cmp != 0)
                        return cmp;

                    return string.Compare(
                        a.DisplayNodeKey ?? string.Empty,
                        b.DisplayNodeKey ?? string.Empty,
                        StringComparison.OrdinalIgnoreCase);
                });
            }

            var seenBranchStartDisplayNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // ------------------------------------------------------------
            // STEP1:
            // Candidate を起点に「どの枝を罫線対象にするか」を決める
            // ------------------------------------------------------------
            if (_currentBackwardCandidatesByChildNodeKey != null &&
                _currentBackwardCandidatesByChildNodeKey.Count > 0)
            {
                foreach (var entry in _currentBackwardCandidatesByChildNodeKey
                    .OrderBy(x => x.Key ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                {
                    string childNodeKey = entry.Key;
                    var candidates = entry.Value;

                    if (string.IsNullOrWhiteSpace(childNodeKey) ||
                        candidates == null ||
                        candidates.Count == 0)
                    {
                        continue;
                    }

                    List<DisplayLaneNode> childDisplayNodes;
                    if (!displayNodesByNodeKey.TryGetValue(childNodeKey, out childDisplayNodes) ||
                        childDisplayNodes == null ||
                        childDisplayNodes.Count == 0)
                    {
                        continue;
                    }

                    foreach (var childDisplayNode in childDisplayNodes)
                    {
                        if (childDisplayNode == null)
                            continue;

                        List<DisplayLaneNode> directChildren;
                        if (!childrenMap.TryGetValue(childDisplayNode.DisplayNodeKey, out directChildren) ||
                            directChildren == null ||
                            directChildren.Count == 0)
                        {
                            continue;
                        }

                        foreach (var candidate in candidates)
                        {
                            if (candidate == null || candidate.Node == null)
                                continue;

                            string parentNodeKey = candidate.Node.NodeIdentityKey;
                            if (string.IsNullOrWhiteSpace(parentNodeKey))
                                continue;

                            var matchedParentDisplayNodes = directChildren
                                .Where(x =>
                                    x != null &&
                                    x.SourceNode != null &&
                                    string.Equals(
                                        x.SourceNode.NodeIdentityKey,
                                        parentNodeKey,
                                        StringComparison.OrdinalIgnoreCase))
                                .OrderBy(x => x.XLevel)
                                .ThenBy(x => x.YLane)
                                .ThenBy(x => x.ChildIndex)
                                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                                .ToList();

                            foreach (var parentDisplayNode in matchedParentDisplayNodes)
                            {
                                if (parentDisplayNode == null)
                                    continue;

                                if (IsBackwardTerminalDisplayNode(parentDisplayNode))
                                    continue;

                                if (parentDisplayNode.XLevel <= 0)
                                    continue;

                                if (parentDisplayNode.OccupiedFirstY < 0 ||
                                    parentDisplayNode.OccupiedLastY < 0)
                                {
                                    continue;
                                }

                                if (!seenBranchStartDisplayNodeKeys.Add(parentDisplayNode.DisplayNodeKey))
                                    continue;

                                result.Add(new OccupiedLotGroupRange
                                {
                                    Level = parentDisplayNode.XLevel,
                                    ParentDisplayNodeKey = parentDisplayNode.ParentDisplayNodeKey ?? string.Empty,
                                    LotGroupKey = string.Empty,
                                    OccupiedFirstY = parentDisplayNode.OccupiedFirstY,
                                    OccupiedLastY = parentDisplayNode.OccupiedLastY
                                });
                            }

                            // ★追加：Lv0 (始点) 用
                            if (childDisplayNode.XLevel == 0)
                            {
                                if (childDisplayNode.OccupiedFirstY >= 0 &&
                                    childDisplayNode.OccupiedLastY >= 0 &&
                                    seenBranchStartDisplayNodeKeys.Add(childDisplayNode.DisplayNodeKey))
                                {
                                    result.Add(new OccupiedLotGroupRange
                                    {
                                        Level = 0,
                                        ParentDisplayNodeKey = string.Empty,
                                        LotGroupKey = string.Empty,
                                        OccupiedFirstY = childDisplayNode.OccupiedFirstY,
                                        OccupiedLastY = childDisplayNode.OccupiedLastY
                                    });
                                }
                            }
                        }
                    }
                }
            }

           
            return result;
        }

        private List<OccupiedLotGroupRange> NormalizeOccupiedRangeByBackwardDisplayTree(List<DisplayLaneNode> nodes)
        {
            var result = new List<OccupiedLotGroupRange>();

            if (nodes == null || nodes.Count == 0)
                return result;

            var validNodes = nodes
                .Where(n =>
                    n != null &&
                    !string.IsNullOrWhiteSpace(n.DisplayNodeKey) &&
                    n.OccupiedFirstY >= 0 &&
                    n.OccupiedLastY >= 0)
                .ToList();

            if (validNodes.Count == 0)
                return result;

            var childrenMap = BuildBackwardChildrenMap(validNodes);

            foreach (var node in validNodes
                .OrderBy(n => n.XLevel)
                .ThenBy(n => n.OccupiedLastY)
                .ThenBy(n => n.YLane)
                .ThenBy(n => n.DisplayNodeKey, StringComparer.OrdinalIgnoreCase))
            {
                if (node.XLevel <= 0)
                {
                    if (!string.IsNullOrWhiteSpace(node.ParentDisplayNodeKey))
                        continue;

                    if (ShouldSuppressBackwardStartBoundary(node, childrenMap))
                        continue;
                }
                else
                {
                    List<DisplayLaneNode> children;
                    if (!childrenMap.TryGetValue(node.DisplayNodeKey, out children) ||
                        children == null ||
                        children.Count == 0)
                    {
                        continue;
                    }
                }

                result.Add(new OccupiedLotGroupRange
                {
                    Level = node.XLevel,
                    ParentDisplayNodeKey = node.ParentDisplayNodeKey ?? string.Empty,
                    LotGroupKey = node.LotGroupKey ?? node.DisplayNodeKey ?? string.Empty,
                    OccupiedFirstY = node.OccupiedFirstY,
                    OccupiedLastY = node.OccupiedLastY
                });
            }

            return result;
        }

        private bool ShouldSuppressBackwardStartBoundary(
            DisplayLaneNode node,
            IDictionary<string, List<DisplayLaneNode>> childrenMap)
        {
            if (node == null ||
                node.XLevel > 0 ||
                !string.IsNullOrWhiteSpace(node.ParentDisplayNodeKey))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(node.DisplayNodeKey) &&
                childrenMap != null &&
                childrenMap.TryGetValue(node.DisplayNodeKey, out var children) &&
                children != null &&
                children.Count > 0)
            {
                return false;
            }

            string nodeKey = node.SourceNode == null ? null : node.SourceNode.NodeIdentityKey;
            if (string.IsNullOrWhiteSpace(nodeKey) ||
                _currentBackwardCandidateGroups == null ||
                _currentBackwardCandidateGroups.Count == 0)
            {
                return false;
            }

            return _currentBackwardCandidateGroups.Any(group =>
                group != null &&
                group.Level == 0 &&
                group.ParentNodes != null &&
                group.ParentNodes.Count > 0 &&
                group.ChildNodes != null &&
                group.ChildNodes
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.NodeIdentityKey))
                    .Select(x => x.NodeIdentityKey)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count() > 1 &&
                group.ChildNodes.Any(x =>
                    x != null &&
                    string.Equals(x.NodeIdentityKey, nodeKey, StringComparison.OrdinalIgnoreCase)));
        }

        

        private void NormalizeBackwardTraceLineRangeGridKinds(IList<TraceLineRange> lineRanges)
        {
            if (lineRanges == null || lineRanges.Count == 0)
                return;

            foreach (var line in lineRanges)
            {
                if (line == null)
                    continue;

                if (line.Level <= 0)
                    continue;

                line.GridKind = "Middle";
            }
        }

        
        private int ResolveSyntheticMiddleToXLevelFromDisplayNodes(
    IList<TraceLineRange> lineRanges,
    IList<DisplayLaneNode> displayNodes)
        {
            int maxToXFromExistingMiddleLines = -1;

            if (lineRanges != null && lineRanges.Count > 0)
            {
                maxToXFromExistingMiddleLines = lineRanges
                    .Where(x =>
                        x != null &&
                        string.Equals(x.GridKind, "Middle", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.ToXLevel)
                    .DefaultIfEmpty(-1)
                    .Max();
            }

            if (maxToXFromExistingMiddleLines >= 1)
                return maxToXFromExistingMiddleLines;

            int maxXFromDisplayNodes = -1;

            if (displayNodes != null && displayNodes.Count > 0)
            {
                maxXFromDisplayNodes = displayNodes
                    .Where(x => x != null)
                    .Select(x => x.XLevel)
                    .DefaultIfEmpty(-1)
                    .Max();
            }

            if (maxXFromDisplayNodes >= 1)
                return maxXFromDisplayNodes;

            // 全件 Start→End 直結でも Middle Lv1 を最低1列出す
            return 1;
        }
        

        
        private void DeduplicateTraceLineRanges(IList<TraceLineRange> lineRanges)
        {
            if (lineRanges == null || lineRanges.Count <= 1)
                return;

            var unique = lineRanges
                .Where(x => x != null)
                .GroupBy(x => string.Join("|",
                    x.GridKind ?? string.Empty,
                    x.LineKind ?? string.Empty,
                    x.Level.ToString(),
                    x.StartRowIndex.ToString(),
                    x.EndRowIndex.ToString(),
                    x.FromXLevel.ToString(),
                    x.ToXLevel.ToString()),
                    StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(x => x.StartRowIndex)
                .ThenBy(x => x.Level)
                .ThenBy(x => x.FromXLevel)
                .ThenBy(x => x.ToXLevel)
                .ThenBy(x => x.LineKind, StringComparer.OrdinalIgnoreCase)
                .ToList();

            lineRanges.Clear();

            foreach (var item in unique)
            {
                lineRanges.Add(item);
            }
        }

        private void AppendForwardOccupiedMiddleLineRanges(
    IList<TraceLineRange> lineRanges,ForwardDisplayLaneBuildResult laneBuildResult)
        {
            int middleMaxX = laneBuildResult.DisplayNodes
                             .Where(x => x != null && x.XLevel > 0)
                             .Select(x => x.XLevel)
                             .DefaultIfEmpty(0)
                             .Max() - 1;

            if (middleMaxX < 1)
                return;
            foreach (var group in laneBuildResult.OccupiedLotGroupRanges)
            {
                if (group.Level < 0 || group.Level > middleMaxX)
                    continue;

                if (group.OccupiedFirstY <= 0)
                    continue;

                int firstRow = group.OccupiedFirstY - 1;

                lineRanges.Add(new TraceLineRange
                {
                    GridKind = group.Level == 0 ? "Start" : "Middle",
                    LineKind = group.Level == 0
                        ? "Start"
                        : group.Level == 1 ? "Trunk" : "Branch",
                    Level = group.Level,
                    StartRowIndex = firstRow,
                    EndRowIndex = firstRow,
                    FromXLevel = group.Level,
                    ToXLevel = middleMaxX
                });
            }
            DeduplicateTraceLineRanges(lineRanges);
        }

        





        public sealed class TraceLineRange
        {
            public string GridKind { get; set; }   // Start / Middle / End
            public string LineKind { get; set; }   // Trunk / Branch

            public int Level { get; set; }

            public int StartRowIndex { get; set; }
            public int EndRowIndex { get; set; }

            public int FromXLevel { get; set; }
            public int ToXLevel { get; set; }
        }

        

        
        private void PopulateLineDecisionInfos(List<DisplayLaneNode> displayNodes)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            // ------------------------------------------------------------
            // STEP1:
            // 罫線判定用の LotGroupKey を各 DisplayLaneNode に付与
            //
            // 単位:
            //   - 同一Lv
            //   - 同一親DisplayNode
            //   - 同一Lot
            //
            // Lotなしは個別Node扱い
            // ------------------------------------------------------------
            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                string lot = NormalizeDisplayLaneLot(
                    node.SourceNode == null ? null : node.SourceNode.LotNumber);

                if (!string.IsNullOrWhiteSpace(lot))
                {
                    node.LotGroupKey = string.Join("|",
                        "LV", node.XLevel.ToString(),
                        "PARENT", node.ParentDisplayNodeKey ?? string.Empty,
                        "LOT", lot);
                }
                else
                {
                    node.LotGroupKey = string.Join("|",
                        "LV", node.XLevel.ToString(),
                        "PARENT", node.ParentDisplayNodeKey ?? string.Empty,
                        "NODE", node.DisplayNodeKey ?? string.Empty);
                }

                node.RepresentativeNodeKey = null;
            }

            // ------------------------------------------------------------
            // STEP2:
            // LotGroup単位で代表DisplayNodeKeyを決定
            //
            // 優先順位:
            //   1. IsLotGroupRepresentative == true
            //   2. YLane昇順
            //   3. DisplayNodeKey安定順
            // ------------------------------------------------------------
            var groupMap = new Dictionary<string, List<DisplayLaneNode>>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                string groupKey = node.LotGroupKey ?? string.Empty;

                List<DisplayLaneNode> bucket;
                if (!groupMap.TryGetValue(groupKey, out bucket))
                {
                    bucket = new List<DisplayLaneNode>();
                    groupMap[groupKey] = bucket;
                }

                bucket.Add(node);
            }

            foreach (var pair in groupMap)
            {
                var bucket = pair.Value;
                if (bucket == null || bucket.Count == 0)
                    continue;

                var representative = bucket
                    .Where(x => x != null)
                    .OrderByDescending(x => x.IsLotGroupRepresentative ? 1 : 0)
                    .ThenBy(x => x.YLane)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault();

                string repKey = representative == null
                    ? null
                    : representative.DisplayNodeKey;

                foreach (var node in bucket)
                {
                    if (node == null)
                        continue;

                    node.RepresentativeNodeKey = repKey;
                }
            }

            // ------------------------------------------------------------
            // STEP3:
            // OccupiedFirstY / OccupiedLastY は罫線用の占有レンジとして埋める
            //
            // 葉:
            //   自分のYLane
            // 親:
            //   自分 + 子孫のレンジを包含
            // ------------------------------------------------------------
            PopulateOccupiedRange(displayNodes);
        }

        

        #endregion

        #region フォワード表示レーン構築


        private List<ChildCandidate> GetNextLevelCandidates(
    ForwardRelationGroup relationGroup)
        {
            var result = new List<ChildCandidate>();

            if (relationGroup == null ||
                relationGroup.ChildSet == null ||
                relationGroup.ChildSet.Count == 0)
            {
                return result;
            }

            var seenChildNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var childNode in relationGroup.ChildSet)
            {
                if (childNode == null)
                    continue;

                string childNodeKey = childNode.NodeIdentityKey;

                if (!seenChildNodeKeys.Add(childNodeKey))
                    continue;

                var candidates = _repo.FindForwardChildCandidates(
                    childNode,
                    childNode.Depth + 1);

                if (candidates == null || candidates.Count == 0)
                {
                    childNode.IsTraceTerminal = true;
                    result.Add(new ChildCandidate
                    {
                        PearentNode = childNode,
                        ChildNode = null,
                        ChildLotNumber = null,
                        RelationKey = null,
                        DebugSource = "TERMINAL"
                    });

                    continue;
                }

                result.AddRange(candidates);
            }

            return result;
        }

        private ForwardDisplayLaneBuildResult BuildForwardDisplayLaneNodes(
    TraceSearchParameters p)
        {
            var buildResult = new ForwardDisplayLaneBuildResult();

            if (p == null)
                return buildResult;

            var startNodes = BuildForwardStartNodes(p);
            if (startNodes == null || startNodes.Count == 0)
                return buildResult;

            var level0Candidates =_repo.FindForwardInitialCandidates(startNodes, p, 1);

            var displayNodes = new List<DisplayLaneNode>();

            if (level0Candidates == null || level0Candidates.Count == 0)
            {
                buildResult.DisplayNodes = displayNodes;
                return buildResult;
            }

            var rootGroups = BuildForwardRelationGroups(level0Candidates, 0);
            if (rootGroups == null || rootGroups.Count == 0)
            {
                buildResult.DisplayNodes = displayNodes;
                return buildResult;
            }

            int currentY = 0;

            foreach (var rootGroup in rootGroups)
            {
                if (rootGroup == null)
                    continue;

                int groupHeight = PlaceForwardRelationGroupRecursive(
                    displayNodes,
                    rootGroup,
                    currentY);

                currentY += groupHeight;
            }
            buildResult.DisplayNodes = displayNodes;
            buildResult.EndInfos = FinalizeForwardEndNodeGroups(buildResult.DisplayNodes);


   
            buildResult.OccupiedLotGroupRanges =
                NormalizeOccupiedRangeByForwardRelation(displayNodes);

            return buildResult;
        }

        private List<ProductionResultNode> BuildForwardStartNodes(
            TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();

            if (p == null)
                return result;

            var rawStarts = _repo.FindForwardStartNodes(p);
            if (rawStarts == null || rawStarts.Count == 0)
                return result;

            foreach (var node in rawStarts)
            {
                if (node == null)
                    continue;

                node.Depth = 0;
                node.ParentKey = null;
                node.StartDateLabel = ResolveStartDateLabel(node, null);

                result.Add(node);
            }

            return result;
        }

        private List<EndDisplayNodeInfo> FinalizeForwardEndNodeGroups(
    List<DisplayLaneNode> displayNodes)
        {
            var output = new List<EndDisplayNodeInfo>();

            if (displayNodes.Count == 0)
                return output;

            var terminalNodes = GetForwardTerminalDisplayNodes(displayNodes);

            if (terminalNodes.Count == 0)
                return output;

            int endXLevel = ResolveForwardEndXLevel(displayNodes);

            NormalizeForwardTerminalDisplayNode(
                terminalNodes,
                endXLevel);

            return BuildForwardEndDisplayNodeInfos(
                terminalNodes,
                endXLevel);
        }

        private List<DisplayLaneNode> GetForwardTerminalDisplayNodes(
    List<DisplayLaneNode> displayNodes)
        {
            return displayNodes
                .Where(x => x.SourceNode.IsTraceTerminal)
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.XLevel)
                .ToList();
        }

        private int ResolveForwardEndXLevel(
    List<DisplayLaneNode> displayNodes)
        {
            int maxNonTerminalX = displayNodes
                .Where(x => !x.SourceNode.IsTraceTerminal)
                .Select(x => x.XLevel)
                .DefaultIfEmpty(0)
                .Max();

            return maxNonTerminalX + 1;
        }

        private void NormalizeForwardTerminalDisplayNode(
    List<DisplayLaneNode> terminalNodes,
    int endXLevel)
        {
            foreach (var node in terminalNodes)
            {
                node.XLevel = endXLevel;

                node.DisplayNodeKey = BuildDisplayLaneNodeKey(
                    node.SourceNode,
                    node.XLevel,
                    node.YLane);
            }
        }

        private List<EndDisplayNodeInfo> BuildForwardEndDisplayNodeInfos(
    List<DisplayLaneNode> terminalNodes,
    int endXLevel)
        {
            var output = new List<EndDisplayNodeInfo>();

            var grouped = terminalNodes
                .GroupBy(x => x.SourceNode.NodeIdentityKey)
                .OrderBy(g => g.Min(n => n.YLane))
                .ThenBy(g => g.Key ?? string.Empty)
                .ToList();

            int nextDuplicateDisplayIndex = 1;

            foreach (var group in grouped)
            {
                var nodes = group
                    .OrderBy(x => x.YLane)
                    .ThenBy(x => x.XLevel)
                    .ToList();

                int? duplicateDisplayIndex = nodes.Count >= 2
                    ? (int?)nextDuplicateDisplayIndex++
                    : null;

                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    output.Add(new EndDisplayNodeInfo
                    {
                        DisplayNodeKey = node.DisplayNodeKey,
                        IsDuplicate = (i > 0),
                        DuplicateDisplayIndex = (i > 0) ? duplicateDisplayIndex : null
                    });
                }

            }

            return output;
        }



        private List<ForwardRelationGroup> BuildForwardRelationGroups(
    IEnumerable<ChildCandidate> currentLevelCandidates,
    int xLevel)
        {
            var result = new List<ForwardRelationGroup>();

            if (currentLevelCandidates == null)
                return result;

            var lotBuckets = currentLevelCandidates
                .Where(c => c != null && c.PearentNode != null)
                .GroupBy(c => BuildForwardRelationGroupLotKey(c))
                .ToList();

            foreach (var lotBucket in lotBuckets)
            {
                if (lotBucket == null)
                    continue;

                var candidates = lotBucket.ToList();
                if (candidates.Count == 0)
                    continue;

                var group = new ForwardRelationGroup
                {
                    XLevel = xLevel
                };

                foreach (var candidate in candidates)
                {
                    if (candidate == null)
                        continue;

                    group.Candidates.Add(candidate);
                }

                foreach (var candidate in group.Candidates)
                {
                    if (candidate.PearentNode != null &&
                        !group.ParentSet.Any(x =>
                            x != null &&
                            string.Equals(
                                x.NodeIdentityKey,
                                candidate.PearentNode.NodeIdentityKey,
                                StringComparison.OrdinalIgnoreCase)))
                    {
                        group.ParentSet.Add(candidate.PearentNode);
                    }

                    if (candidate.ChildNode != null &&
                        !group.ChildSet.Any(x =>
                            x != null &&
                            string.Equals(
                                x.NodeIdentityKey,
                                candidate.ChildNode.NodeIdentityKey,
                                StringComparison.OrdinalIgnoreCase)))
                    {
                        group.ChildSet.Add(candidate.ChildNode);
                    }
                }

                if (group.ParentSet.Count == 0)
                    continue;

                result.Add(group);
            }

            return result;
        }

        private string BuildForwardRelationGroupLotKey(ChildCandidate candidate)
        {
            if (candidate == null || candidate.PearentNode == null)
                return null;

            if (IsForwardDrumcanRelationCandidate(candidate))
            {
                if (candidate.ChildNode != null &&
                    !string.IsNullOrWhiteSpace(candidate.ChildNode.LotNumber))
                {
                    return candidate.ChildNode.LotNumber;
                }

            }

            return candidate.PearentNode.LotNumber;
        }

        private bool IsForwardDrumcanRelationCandidate(ChildCandidate candidate)
        {
            if (candidate == null || candidate.ChildNode == null)
                return false;

            return string.Equals(
                candidate.PearentNode.InputSourceType,
                "Drumcan",
                StringComparison.OrdinalIgnoreCase);
        }




        private int PlaceForwardRelationGroupRecursive(
    List<DisplayLaneNode> output,
    ForwardRelationGroup relationGroup,
    int baseY)
        {
            if (output == null || relationGroup == null)
                return 0;

            var placedNodes = PlaceForwardRelationGroup(
                                output,
                                relationGroup,
                                baseY);

            int selfHeight = relationGroup.ParentSet == null
                ? 0
                : relationGroup.ParentSet.Count;

            var nextLevelCandidates = GetNextLevelCandidates(relationGroup);
            if (nextLevelCandidates == null || nextLevelCandidates.Count == 0)
            {
                
                return selfHeight;
            }

            var childGroups = BuildForwardRelationGroups(
                nextLevelCandidates,
                relationGroup.XLevel + 1);

            if (childGroups == null || childGroups.Count == 0)
            {
                
                return selfHeight;
            }

            int childHeightSum = 0;
            int childBaseY = baseY;

            foreach (var childGroup in childGroups)
            {
                if (childGroup == null)
                    continue;

                int childHeight = PlaceForwardRelationGroupRecursive(
                    output,
                    childGroup,
                    childBaseY);

                childHeightSum += childHeight;
                childBaseY += childHeight;
            }

            int subtreeHeight = Math.Max(selfHeight, childHeightSum);
            relationGroup.OccupiedFirstY = baseY;
            relationGroup.OccupiedLastY = baseY + subtreeHeight - 1;

            foreach (var node in placedNodes)
            {
                if (node == null)
                    continue;

                node.OccupiedFirstY = baseY;
                node.OccupiedLastY = baseY + subtreeHeight - 1;
            }

            return subtreeHeight;
        }

        private List<DisplayLaneNode> PlaceForwardRelationGroup(List<DisplayLaneNode> output,ForwardRelationGroup relationGroup,int baseY)
        {

            var placedNodes = new List<DisplayLaneNode>();
            if (output == null || relationGroup == null || relationGroup.ParentSet == null)
                return placedNodes;

            var anchorParent = SelectAnchorParent(relationGroup.ParentSet);

            int currentY = baseY;

            foreach (var currentNode in relationGroup.ParentSet)
            {
                if (currentNode == null)
                    continue;

                
                    var displayNode = new DisplayLaneNode
                    {
                        DisplayNodeKey = BuildDisplayLaneNodeKey(currentNode,relationGroup.XLevel,currentY),
                        MergeKey = currentNode.NodeIdentityKey,
                        ParentDisplayNodeKey = null,
                        SourceNode = currentNode,
                        XLevel = relationGroup.XLevel,
                        YLane = currentY,
                        OccupiedFirstY = currentY,
                        OccupiedLastY = currentY,

                        // 現段階では構築には使わず、属性としてだけ保持
                        IsLotGroupRepresentative =
                            anchorParent != null &&
                            string.Equals(
                                currentNode.NodeIdentityKey,
                                anchorParent.NodeIdentityKey,
                                StringComparison.OrdinalIgnoreCase)
                    };

                    output.Add(displayNode);
                    placedNodes.Add(displayNode);
           

                currentY++;
                
            }
            return placedNodes;
        }

        

        private List<TraceLineRange> BuildForwardLineRanges(
    List<DisplayLaneNode> displayNodes)
        {
            var result = new List<TraceLineRange>();

            if (displayNodes == null || displayNodes.Count == 0)
                return result;

            var validNodes = displayNodes;
              

            if (validNodes.Count == 0)
                return result;

            // 最右の実在列を End とみなす
            int endX = validNodes.Select(x => x.XLevel).Max();

            // Middle の最終描画範囲
            // ※トレースバック既存処理に合わせる
            int middleMaxX = validNodes.Select(x => x.XLevel).Max()-1;

            foreach (var node in validNodes)
                //.OrderBy(x => x.XLevel)
                //.ThenBy(x => x.OccupiedLastY)
                //.ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase))
            {
                bool isEndGroup = node.XLevel == endX;

                // Middle 線なのに伸ばす先の Middle 列が存在しない場合は作らない
                if (!isEndGroup && middleMaxX < node.XLevel)
                    continue;

                result.Add(new TraceLineRange
                {
                    GridKind = node.XLevel == 0? "Start": isEndGroup ? "End" : "Middle",
                    LineKind = node.XLevel == 0? "Start": node.XLevel == 1 ? "Trunk" : "Branch",
                    Level = node.XLevel,

                    StartRowIndex = node.OccupiedLastY,
                    EndRowIndex = node.OccupiedLastY,

                    FromXLevel = node.XLevel,
                    ToXLevel = isEndGroup ? node.XLevel : middleMaxX
                });
            }

            return result;
        }

        private List<OccupiedLotGroupRange> NormalizeOccupiedRangeByForwardRelation(
    List<DisplayLaneNode> nodes)
        {
            var result = new List<OccupiedLotGroupRange>();

            if (nodes == null || nodes.Count == 0)
                return result;

            foreach (var node in nodes
                .Where(n =>
                    n != null &&
                    n.OccupiedFirstY >= 0 &&
                    n.OccupiedLastY >= 0)
                .OrderBy(n => n.XLevel)
                .ThenBy(n => n.OccupiedLastY))
            {
                result.Add(new OccupiedLotGroupRange
                {
                    Level = node.XLevel,
                    ParentDisplayNodeKey = node.ParentDisplayNodeKey ?? string.Empty,
                    LotGroupKey = null,
                    OccupiedFirstY = node.OccupiedFirstY,
                    OccupiedLastY = node.OccupiedLastY
                });
            }

            return result;
        }

        
        private ProductionResultNode SelectAnchorParent(List<ProductionResultNode> parentSet)
        {
            if (parentSet == null || parentSet.Count == 0)
                return null;

            return parentSet
                .Where(n => n != null)
                .OrderBy(n => n.StartDate ?? DateTime.MinValue)
                .FirstOrDefault();
        }



        private sealed class ForwardRelationGroup
        {
            public int XLevel { get; set; }
            public string RelationKey { get; set; }

            public int OccupiedFirstY { get; set; }
            public int OccupiedLastY { get; set; }

            // ★集合（構造）
            public List<ProductionResultNode> ParentSet { get; set; }
            public List<ProductionResultNode> ChildSet { get; set; }

            // ★元データ（参照用）
            public List<ChildCandidate> Candidates { get; private set; }

            public ForwardRelationGroup()
            {
                OccupiedFirstY = -1;
                OccupiedLastY = -1;

                ParentSet = new List<ProductionResultNode>();
                ChildSet = new List<ProductionResultNode>();
                Candidates = new List<ChildCandidate>();
            }
        }


        #endregion

        #region バックワード表示レーン構築


        private List<BackwardParentSetCandidateGroup> BuildBackwardCandidateGroupsByTraversal(
            List<BackwardParentCandidate> level0Candidates,
            CancellationToken cancellationToken)
        {
            var result = new List<BackwardParentSetCandidateGroup>();
            var queue = new Queue<BackwardParentSetCandidateGroup>();
            var seenGroupKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Action<List<BackwardParentCandidate>, int> appendGroups = (candidates, level) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var groups = BuildBackwardParentSetCandidateGroups(candidates);
                foreach (var group in groups)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (group == null)
                        continue;

                    group.Level = level;

                    string groupKey = BuildBackwardCandidateGroupIdentityKey(group);
                    if (!seenGroupKeys.Add(groupKey))
                        continue;

                    result.Add(group);
                    queue.Enqueue(group);
                }
            };

            appendGroups(level0Candidates, 0);

            while (queue.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentGroup = queue.Dequeue();
                if (currentGroup == null || currentGroup.Level >= MaxDepth)
                    continue;

                var nextCandidates = GetNextBackwardCandidates(currentGroup);
                appendGroups(nextCandidates, currentGroup.Level + 1);
            }

            return result;
        }

        private string BuildBackwardCandidateGroupIdentityKey(BackwardParentSetCandidateGroup group)
        {
            if (group == null)
                return string.Empty;

            var childKeys = group.ChildNodes == null
                ? new List<string>()
                : group.ChildNodes
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.NodeIdentityKey))
                    .Select(x => x.NodeIdentityKey)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList();

            return string.Join("|",
                group.Level.ToString(),
                group.ParentSetKey ?? string.Empty,
                string.Join(",", childKeys));
        }

        private List<BackwardParentCandidate> GetNextBackwardCandidates(
            BackwardParentSetCandidateGroup candidateGroup)
        {
            var result = new List<BackwardParentCandidate>();

            if (candidateGroup == null ||
                candidateGroup.ParentNodes == null ||
                candidateGroup.ParentNodes.Count == 0)
            {
                return result;
            }

            var seenParentNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var parentNode in candidateGroup.ParentNodes)
            {
                if (parentNode == null)
                    continue;

                string parentNodeKey = parentNode.NodeIdentityKey;
                if (string.IsNullOrWhiteSpace(parentNodeKey))
                    continue;

                if (!seenParentNodeKeys.Add(parentNodeKey))
                    continue;

                var candidates = _repo.FindBackwardParentsByLotStepFlow(
                    parentNode,
                    parentNode.Depth + 1);

                if (candidates == null || candidates.Count == 0)
                {
                    parentNode.IsTraceTerminal = true;
                    continue;
                }

                result.AddRange(candidates.Where(x => x != null));
            }
            return result;
        }

        private ForwardDisplayLaneBuildResult BuildBackwardDisplayLaneNodes(
            List<BackwardDisplayNodeGroup> displayNodeGroups,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var buildResult = new ForwardDisplayLaneBuildResult();

            if (displayNodeGroups == null || displayNodeGroups.Count == 0)
                return buildResult;

            int branchBaseY = 0;
            var PlacedeNodes = new List<DisplayLaneNode>();


            //始点処理。始点Node群抽出
            
            for(int index =0;  index < displayNodeGroups.Count; index++)
            {
                var roots = displayNodeGroups[index];
                if (roots == null || roots.Level != 0)
                {
                    continue;
                }

                var rootNodes = roots.ChildNodes.ToList();
                branchBaseY = PlaceBackwardNextTargetsRecursive(
                    displayNodeGroups,
                    rootNodes,
                    0,
                    branchBaseY,
                    PlacedeNodes,
                    null);

          
            }

            buildResult.DisplayNodes.AddRange(PlacedeNodes);
            return buildResult;

        }

       
        private int PlaceBackwardNextTargetsRecursive(
            List<BackwardDisplayNodeGroup> displayNodeGroups,
            List<DisplayLaneNode> currentNodes,
            int x,
            int y,
            List<DisplayLaneNode> placedeNodes,
            string parentDisplayNodeKey)
        {
            
          
            int currentY =y;
            int nextLevelY = y;

            var workcurrents = currentNodes.ToList();
            var Allready = new List<BackwardDisplayNodeGroup>();
            

            while(workcurrents.Count > 0)
            {
                var Current = workcurrents[0];
                workcurrents.RemoveAt(0);

                var nextGroups = TakeNextTargets(displayNodeGroups, x, Current);
                bool SubTree_FLG = nextGroups.Any(g => !Allready.Contains(g));

                if (SubTree_FLG)
                {
                    currentY = Math.Max(currentY, nextLevelY);
                }

                var placedCurrent = BackwardPlaceNode(Current, x, currentY, parentDisplayNodeKey);
                placedeNodes.Add(placedCurrent);
                currentY++;

                foreach (var nextGroup in nextGroups)
                {
                    if (Allready.Contains(nextGroup))
                    {
                        continue;
                    }

                    Allready.Add(nextGroup);

                    var nextCurrents = nextGroup.ParentNodes?.ToList() ?? new List<DisplayLaneNode>();

                    nextLevelY = PlaceBackwardNextTargetsRecursive(
                        displayNodeGroups,
                        nextCurrents,
                        x + 1,
                        nextLevelY,
                        placedeNodes,
                        placedCurrent == null ? null : placedCurrent.DisplayNodeKey);
                    
                }
                
            }
            return Math.Max(currentY, nextLevelY);

        }

        private List<DisplayLaneNode> GetBackwardNextDisplayNodes(List<BackwardDisplayNodeGroup> displayNodeGroups,int nextLevel,DisplayLaneNode current)
        {
            var result = new List<DisplayLaneNode>();

            foreach (var group in displayNodeGroups)
            {
                if (group == null || group.Level != nextLevel)
                    continue;

                bool matched = group.ChildNodes != null &&
                    group.ChildNodes.Any(c =>
                        c?.SourceNode != null &&
                        string.Equals(
                            c.SourceNode.NodeIdentityKey,
                            current.SourceNode.NodeIdentityKey,
                            StringComparison.OrdinalIgnoreCase));

                if (!matched)
                    continue;

                if (group.ParentNodes != null)
                    result.AddRange(group.ParentNodes);
            }

            return result;
        }


        private List<BackwardDisplayNodeGroup> TakeNextTargets(
    List<BackwardDisplayNodeGroup> displayNodeGroups,
    int nextLevel,
    DisplayLaneNode pearent)
        {
            var result = new List<BackwardDisplayNodeGroup>();

            if (displayNodeGroups == null ||
                pearent == null ||
                pearent.SourceNode == null)
                return result;

            for (int i = displayNodeGroups.Count - 1; i >= 0; i--)
            {
                var g = displayNodeGroups[i];

                if (g == null)
                    continue;

                bool matched =
                    g.Level == nextLevel &&
                    g.ChildNodes.Any(c =>
                        c != null &&
                        c.SourceNode != null &&
                        string.Equals(
                            c.SourceNode.NodeIdentityKey,
                            pearent.SourceNode.NodeIdentityKey,
                            StringComparison.OrdinalIgnoreCase));

                if (!matched)
                    continue;

                result.Add(g);
                //displayNodeGroups.RemoveAt(i);
            }

            return result;
        }

        private DisplayLaneNode BackwardPlaceNode(
            DisplayLaneNode current,
            int x,
            int y,
            string parentDisplayNodeKey)
        {
            if (current == null)
                return null;

            var placedCurrent = new DisplayLaneNode
            {
                SourceNode = current.SourceNode,
                MergeKey = current.MergeKey,
                ParentDisplayNodeKey = parentDisplayNodeKey,

                XLevel = x,
                YLane = y,
                OccupiedFirstY = y,
                OccupiedLastY = y,

                DisplayNodeKey = string.Join(
                    "|",
                    "DISPLAYNODE",
                    x.ToString(),
                    y.ToString(),
                    current.SourceNode?.NodeIdentityKey)
            };

            return placedCurrent;
        }


        private string ResolveBackwardStartDateLabel(ProductionResultNode node)
        {
            if (node == null)
                return null;

            if (node.StartDate.HasValue)
                return node.StartDate.Value.ToString("yyyy/MM/dd HH:mm");

            return null;
        }

        private List<BackwardDisplayNodeGroup> BuildBackWardDisplayNodeGroups(List<BackwardParentSetCandidateGroup> groups)
        {
            var result = new List<BackwardDisplayNodeGroup>();

            if (groups == null || groups.Count == 0)
                return result;

            foreach (var group in groups.Where(x => x != null))
            {
                var displayGroup = new BackwardDisplayNodeGroup
                {
                    Level = group.Level,
                    Group = group
                };

                displayGroup.ChildNodes.AddRange(
                    group.ChildNodes
                        .Where(x => x != null)
                        .GroupBy(x => x.NodeIdentityKey, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new DisplayLaneNode
                        {
                            SourceNode = x.First(),
                            MergeKey = x.Key
                        }));

                displayGroup.ParentNodes.AddRange(
                    group.ParentNodes
                        .Where(x => x != null)
                        .GroupBy(BuildBackwardParentNodeSetMemberKey, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new DisplayLaneNode
                        {
                            SourceNode = x.First(),
                            MergeKey = x.Key
                        }));

                result.Add(displayGroup);
            }
            
            return result;
            
        }

        

        private List<BackwardParentSetCandidateGroup> BuildBackwardParentSetCandidateGroups(
    List<BackwardParentCandidate> candidates)
        {

            var result = new List<BackwardParentSetCandidateGroup>();

            if (candidates == null || candidates.Count == 0)
                return result;

            var validCandidates = candidates;
                

            var parentSetGroups = validCandidates
                .GroupBy(candidate =>
                    BuildBackwardParentSetKey(
                        validCandidates
                            .Where(x =>
                                x.ChildNode != null &&
                                string.Equals(
                                    x.ChildNode.NodeIdentityKey,
                                    candidate.ChildNode.NodeIdentityKey,
                                    StringComparison.OrdinalIgnoreCase))
                            .Select(x => x.Node)),
                    StringComparer.OrdinalIgnoreCase);

            foreach (var parentSetGroup in parentSetGroups)
            {
               

                var groupCandidates = parentSetGroup.ToList();

                var group = new BackwardParentSetCandidateGroup
                {
                    ParentSetKey = string.IsNullOrWhiteSpace(parentSetGroup.Key)
        ? "TERMINAL|" + string.Join("|",
            groupCandidates
                .Where(x => x != null && x.ChildNode != null)
                .Select(x => x.ChildNode.NodeIdentityKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        : parentSetGroup.Key
                };

                group.ChildNodes.AddRange(
                    groupCandidates
                        .Select(x => x.ChildNode)
                        .Where(x => x != null)
                        .GroupBy(x => x.NodeIdentityKey, StringComparer.OrdinalIgnoreCase)
                        .Select(x => x.First()));

                group.ParentNodes.AddRange(
                    groupCandidates
                        .Select(x => x.Node)
                        .Where(x => x != null)
                        .GroupBy(BuildBackwardParentNodeSetMemberKey, StringComparer.OrdinalIgnoreCase)
                        .Select(x => x.First()));

                group.Candidates.AddRange(groupCandidates);

                PruneBackwardIntermediateDrumcanChildNodes(group);

                result.Add(group);
            }

            return result;
        }

        private void PruneBackwardIntermediateDrumcanChildNodes(BackwardParentSetCandidateGroup group)
        {
            if (group == null || group.ChildNodes == null || group.ChildNodes.Count <= 1)
                return;

            bool hasNormalChild = group.ChildNodes.Any(x => x != null && !IsBackwardIntermediateDrumcanChildNode(x));
            if (!hasNormalChild)
                return;

            var pruneChildNodeKeys = group.ChildNodes
                .Where(IsBackwardIntermediateDrumcanChildNode)
                .Select(x => x.NodeIdentityKey)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (pruneChildNodeKeys.Count == 0)
                return;

            group.ChildNodes.RemoveAll(x =>
                x != null &&
                pruneChildNodeKeys.Contains(x.NodeIdentityKey, StringComparer.OrdinalIgnoreCase));

            if (group.Candidates != null)
            {
                group.Candidates.RemoveAll(x =>
                    x != null &&
                    x.ChildNode != null &&
                    pruneChildNodeKeys.Contains(x.ChildNode.NodeIdentityKey, StringComparer.OrdinalIgnoreCase));
            }
        }

        private bool IsBackwardIntermediateDrumcanChildNode(ProductionResultNode node)
        {
            if (node == null)
                return false;

            return 
                   string.Equals(node.RouteSystem, "A", StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(node.InputSourceType, "Drumcan", StringComparison.OrdinalIgnoreCase) &&
                   !node.InputSlotNo.HasValue &&
                   !node.Weight.HasValue;
        }

        

        private string BuildBackwardParentNodeSetMemberKey(ProductionResultNode node)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.NodeIdentityKey))
                return null;

            return node.NodeIdentityKey.Trim().ToUpperInvariant();
        }

        private string BuildBackwardParentSetKey(IEnumerable<ProductionResultNode> parentNodes)
        {
            if (parentNodes == null)
                return null;

            var parentNodeKeys = parentNodes
                .Select(BuildBackwardParentNodeSetMemberKey)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (parentNodeKeys.Count == 0)
                return null;

            return string.Join("|", parentNodeKeys);
        }


        
        private sealed class BackwardDisplayNodeGroup
        {
            public int Level { get; set; }
            public int BaseY { get; set; }
            public List<DisplayLaneNode> ChildNodes { get; set; }
            public List<DisplayLaneNode> ParentNodes { get; set; }

            public BackwardParentSetCandidateGroup Group { get; set; }

            public BackwardDisplayNodeGroup()
            {
                ChildNodes = new List<DisplayLaneNode> ();
                ParentNodes = new List<DisplayLaneNode> ();
            }
                
        }
       

        
        private sealed class BackwardParentSetCandidateGroup
        {
            public int Level { get; set; }
            public string ParentSetKey { get; set; }
            public List<ProductionResultNode> ChildNodes { get; set; }
        
            public List<ProductionResultNode> ParentNodes { get; set; }

            public List<BackwardParentCandidate> Candidates { get; set; }
            
            public BackwardParentSetCandidateGroup()
            {
              
                ParentNodes = new List<ProductionResultNode>();
                ChildNodes = new List<ProductionResultNode>();
                Candidates = new List<BackwardParentCandidate>();
            }
        }

        

        

        

        

      
        private void RegisterBackwardParentCandidateForDisplay(
    Dictionary<string, List<BackwardParentCandidate>> backwardCandidatesByChildNodeKey,BackwardParentCandidate candidate)
        {
            if (backwardCandidatesByChildNodeKey == null || candidate == null || candidate.ChildNode == null)
                return;

            string childNodeKey = candidate.ChildNode.NodeIdentityKey;
            if (string.IsNullOrWhiteSpace(childNodeKey))
                return;

            List<BackwardParentCandidate> list;
            if (!backwardCandidatesByChildNodeKey.TryGetValue(childNodeKey, out list) || list == null)
            {
                list = new List<BackwardParentCandidate>();
                backwardCandidatesByChildNodeKey[childNodeKey] = list;
            }

            list.Add(candidate);
        }


        
        private void BuildBackwardPathRowsFromDisplayLaneNodes(
    TraceResult result,
    ForwardDisplayLaneBuildResult laneBuildResult)
        {
            if (result == null)
                return;

            result.PathRows.Clear();

            if (laneBuildResult == null ||
                laneBuildResult.DisplayNodes == null ||
                laneBuildResult.DisplayNodes.Count == 0)
            {
                return;
            }

            var displayNodes = laneBuildResult.DisplayNodes
                .Where(x => x != null)
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.XLevel)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var endInfos = laneBuildResult.EndInfos ?? new List<EndDisplayNodeInfo>();

            var endInfoByDisplayNodeKey = endInfos
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var laneGroups = displayNodes
                .GroupBy(x => x.YLane)
                .OrderBy(g => g.Key)
                .ToList();

            int pathOrder = 0;

            foreach (var laneGroup in laneGroups)
            {
                var laneNodes = laneGroup
                    .Where(x => x != null)
                    .OrderBy(x => x.XLevel)
                    .ThenBy(x => x.ChildIndex)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (laneNodes.Count == 0)
                    continue;

                var row = new TracePathRow();
                row.PathOrder = pathOrder++;

                // ------------------------------------------------------------
                // STEP1:
                // まず Start / Middle だけを機械的に配置する
                // End はまだ決めない
                // ------------------------------------------------------------
                foreach (var displayNode in laneNodes)
                {
                    if (displayNode == null || displayNode.SourceNode == null)
                        continue;

                    if (displayNode.XLevel <= 0)
                    {
                        if (row.StartNode == null)
                        {
                            row.StartNode = displayNode.SourceNode;
                        }

                        continue;
                    }

                    int middleIndex = displayNode.XLevel - 1;
                    EnsureMiddleNodeCapacity(row, middleIndex + 1);
                    row.MiddleNodes[middleIndex] = displayNode.SourceNode;
                }

                // ------------------------------------------------------------
                // STEP1.5:
                // Middle の LotGroup 情報を UI橋渡し用に転写する
                // ※ forward と同じ流れに合わせる
                // ------------------------------------------------------------
                //AddForwardMiddleLotGroupInfosFromLaneNodes(row, laneNodes);

                // ------------------------------------------------------------
                // STEP2:
                // 行の代表情報を最低限埋める
                // ------------------------------------------------------------
                // ------------------------------------------------------------
                // STEP3:
                // EndInfos を使って End を確定する
                // End に採用した node は Middle から外す
                // ------------------------------------------------------------
                foreach (var displayNode in laneNodes)
                {
                    if (displayNode == null || displayNode.SourceNode == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(displayNode.DisplayNodeKey))
                        continue;

                    EndDisplayNodeInfo endInfo;
                    if (!endInfoByDisplayNodeKey.TryGetValue(displayNode.DisplayNodeKey, out endInfo) ||
                        endInfo == null)
                    {
                        continue;
                    }

                    row.EndNode = displayNode.SourceNode;
                    row.EndIsDuplicate = endInfo.IsDuplicate;
                    row.EndDuplicateDisplayGroupIndex = endInfo.DuplicateDisplayIndex;

                    if (displayNode.XLevel > 0)
                    {
                        int middleIndex = displayNode.XLevel - 1;
                        if (middleIndex >= 0 && middleIndex < row.MiddleNodes.Count)
                        {
                            if (object.ReferenceEquals(row.MiddleNodes[middleIndex], displayNode.SourceNode))
                            {
                                row.MiddleNodes[middleIndex] = null;
                            }
                        }
                    }

                    break;
                }

                // ------------------------------------------------------------
                // STEP4:
                // 末尾の不要 null Middle を落とす
                // ------------------------------------------------------------
                TrimTrailingNullMiddleNodes(row);

                result.PathRows.Add(row);
            }

        }


        /// <summary>
        /// こいつは使う
        /// </summary>
        /// <param name="displayNodes"></param>
        /// <returns></returns>
        private List<EndDisplayNodeInfo> FinalizeBackwardEndNodeGroupsAndNormalizeX(
    List<DisplayLaneNode> displayNodes)
        {
            var output = new List<EndDisplayNodeInfo>();

            if (displayNodes == null || displayNodes.Count == 0)
                return output;

            var terminalNodes = displayNodes
                .Where(x => x != null && IsBackwardTerminalDisplayNode(x))
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.XLevel)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (terminalNodes.Count == 0)
                return output;

            int maxNonTerminalX = displayNodes
                .Where(x => x != null && !terminalNodes.Contains(x))
                .Select(x => x.XLevel)
                .DefaultIfEmpty(0)
                .Max();

            int endXLevel = maxNonTerminalX + 1;

            var grouped = terminalNodes
                .GroupBy(x => BuildBackwardEndGroupKey(x), StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Min(n => n.YLane))
                .ThenBy(g => g.Key ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int nextDuplicateDisplayIndex = 1;

            foreach (var group in grouped)
            {
                var orderedGroupNodes = group
                    .Where(x => x != null)
                    .OrderBy(x => x.YLane)
                    .ThenBy(x => x.XLevel)
                    .ThenBy(x => x.ChildIndex)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                int? duplicateDisplayIndex = orderedGroupNodes.Count >= 2
                    ? (int?)nextDuplicateDisplayIndex++
                    : null;

                for (int i = 0; i < orderedGroupNodes.Count; i++)
                {
                    var node = orderedGroupNodes[i];
                    if (node == null)
                        continue;

                    string oldDisplayNodeKey = node.DisplayNodeKey;

                    node.XLevel = endXLevel;
                    node.DisplayNodeKey = BuildDisplayLaneNodeKey(
                        node.SourceNode,
                        node.XLevel,
                        node.YLane);

                    ReplaceBackwardParentDisplayNodeKey(
                        displayNodes,
                        oldDisplayNodeKey,
                        node.DisplayNodeKey);

                    output.Add(new EndDisplayNodeInfo
                    {
                        DisplayNodeKey = node.DisplayNodeKey,
                        IsDuplicate = (i > 0),
                        DuplicateDisplayIndex = (i > 0) ? duplicateDisplayIndex : null
                    });
                }

            }

            return output;
        }

        private bool IsBackwardTerminalDisplayNode(DisplayLaneNode node)
        {
            if (node == null || node.SourceNode == null)
                return false;

            if (_currentBackwardCandidatesByChildNodeKey == null)
                return true;

            List<BackwardParentCandidate> candidates;
            if (!_currentBackwardCandidatesByChildNodeKey.TryGetValue(
                    node.SourceNode.NodeIdentityKey,
                    out candidates) ||
                candidates == null ||
                candidates.Count == 0)
            {
                return true;
            }

            return false;
        }

        private void ReplaceBackwardParentDisplayNodeKey(
    List<DisplayLaneNode> displayNodes,
    string oldDisplayNodeKey,
    string newDisplayNodeKey)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(oldDisplayNodeKey) ||
                string.IsNullOrWhiteSpace(newDisplayNodeKey))
            {
                return;
            }

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                if (string.Equals(
                    node.ParentDisplayNodeKey,
                    oldDisplayNodeKey,
                    StringComparison.OrdinalIgnoreCase))
                {
                    node.ParentDisplayNodeKey = newDisplayNodeKey;
                }
            }
        }
        private string BuildBackwardEndGroupKey(DisplayLaneNode node)
        {
            if (node == null || node.SourceNode == null)
                return string.Empty;

            string masterKey = node.SourceNode.ControlMasterKey;

            if (string.IsNullOrWhiteSpace(masterKey))
                return string.Empty;

            return masterKey.Trim();
        }
        

        


        #endregion

        #region バックワード枝剪定

        private sealed class BackwardPruneGroup
        {
            public int XLevel { get; set; }
            public string RelationKey { get; set; }
            public List<BackwardPruneTarget> Members { get; private set; }

            public BackwardPruneGroup()
            {
                Members = new List<BackwardPruneTarget>();
            }
        }

        private sealed class BackwardPruneTarget
        {
            public DisplayLaneNode StartDisplayNode { get; set; }
            public BackwardParentCandidate Candidate { get; set; }
        }

        private List<BackwardParentCandidate> GetBackwardParentCandidatesForDisplayNode(
    DisplayLaneNode node)
        {
            var result = new List<BackwardParentCandidate>();

            if (node == null || node.SourceNode == null)
                return result;

            if (_currentBackwardCandidatesByChildNodeKey == null)
                return result;

            List<BackwardParentCandidate> candidates;
            if (!_currentBackwardCandidatesByChildNodeKey.TryGetValue(
                    node.SourceNode.NodeIdentityKey,
                    out candidates) ||
                candidates == null ||
                candidates.Count == 0)
            {
                return result;
            }

            result.AddRange(candidates.Where(x => x != null));

            return result;
        }

        private List<BackwardPruneGroup> BuildBackwardPruneGroupsAtLevel(
    IList<DisplayLaneNode> displayNodes,
    int xLevel)
        {
            var result = new List<BackwardPruneGroup>();

            if (displayNodes == null || displayNodes.Count == 0)
                return result;

            var groupMap = new Dictionary<string, BackwardPruneGroup>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                if (node.XLevel != xLevel)
                    continue;

                if (IsBackwardTerminalDisplayNode(node))
                    continue;

                var candidates = GetBackwardParentCandidatesForDisplayNode(node);
                if (candidates == null || candidates.Count == 0)
                    continue;

                foreach (var candidate in candidates)
                {
                    if (candidate == null)
                        continue;

                    string relationKey = candidate.RelationKey;
                    if (string.IsNullOrWhiteSpace(relationKey))
                        continue;

                    string pruneGroupKey = xLevel.ToString() + "|" + relationKey;

                    BackwardPruneGroup pruneGroup;
                    if (!groupMap.TryGetValue(pruneGroupKey, out pruneGroup))
                    {
                        pruneGroup = new BackwardPruneGroup
                        {
                            XLevel = xLevel,
                            RelationKey = relationKey
                        };

                        groupMap[pruneGroupKey] = pruneGroup;
                        result.Add(pruneGroup);
                    }

                    pruneGroup.Members.Add(new BackwardPruneTarget
                    {
                        StartDisplayNode = node,
                        Candidate = candidate
                    });
                }
            }

            return result;
        }

        private List<BackwardPruneTarget> SelectBackwardPruneTargets(
    BackwardPruneGroup pruneGroup,
    IList<DisplayLaneNode> displayNodes)
        {
            var result = new List<BackwardPruneTarget>();

            if (pruneGroup == null ||
                pruneGroup.Members == null ||
                pruneGroup.Members.Count <= 1)
            {
                return result;
            }

            var seenDisplayNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var member in pruneGroup.Members)
            {
                if (member == null || member.StartDisplayNode == null)
                    continue;

                var sourceNode = member.StartDisplayNode.SourceNode;
                if (sourceNode == null)
                    continue;

                if (!string.Equals(sourceNode.RouteSystem, "A", StringComparison.OrdinalIgnoreCase))
                    continue;

                string displayNodeKey = member.StartDisplayNode.DisplayNodeKey;
                if (!string.IsNullOrWhiteSpace(displayNodeKey) &&
                    !seenDisplayNodeKeys.Add(displayNodeKey))
                {
                    continue;
                }

                result.Add(member);
            }

            return result;
        }


        /// <summary>
        /// 枝選定用の準備メソッド。必要が無ければ使わない
        /// </summary>
        /// <param name="backwardLaneBuildResult"></param>
        private void ApplyBackwardBranchPruning(
    ForwardDisplayLaneBuildResult backwardLaneBuildResult)
        {
            if (backwardLaneBuildResult == null ||
                backwardLaneBuildResult.DisplayNodes == null ||
                backwardLaneBuildResult.DisplayNodes.Count == 0)
            {
                return;
            }

            var childrenMap = BuildBackwardChildrenMap(backwardLaneBuildResult.DisplayNodes);
            var prunedDisplayNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int maxXLevel = backwardLaneBuildResult.DisplayNodes.Max(x => x == null ? 0 : x.XLevel);

            for (int xLevel = 0; xLevel <= maxXLevel; xLevel++)
            {
                var pruneGroups = BuildBackwardPruneGroupsAtLevel(
                    backwardLaneBuildResult.DisplayNodes,
                    xLevel);

                foreach (var pruneGroup in pruneGroups)
                {
                    var pruneTargets = SelectBackwardPruneTargets(
                        pruneGroup,
                        backwardLaneBuildResult.DisplayNodes);

                    foreach (var pruneTarget in pruneTargets)
                    {
                        if (pruneTarget == null || pruneTarget.StartDisplayNode == null)
                            continue;

                        MarkBackwardPrunedSubtree(
                            pruneTarget.StartDisplayNode,
                            childrenMap,
                            prunedDisplayNodeKeys);
                    }
                }
            }

            // 今回はまだ物理削除しなくてもよい
        }

        private Dictionary<string, List<DisplayLaneNode>> BuildBackwardChildrenMap(
    IList<DisplayLaneNode> displayNodes)
        {
            var result = new Dictionary<string, List<DisplayLaneNode>>(
                StringComparer.OrdinalIgnoreCase);

            if (displayNodes == null || displayNodes.Count == 0)
                return result;

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                string parentDisplayNodeKey = node.ParentDisplayNodeKey;
                if (string.IsNullOrWhiteSpace(parentDisplayNodeKey))
                    continue;

                List<DisplayLaneNode> children;
                if (!result.TryGetValue(parentDisplayNodeKey, out children))
                {
                    children = new List<DisplayLaneNode>();
                    result[parentDisplayNodeKey] = children;
                }

                children.Add(node);
            }

            return result;
        }

        private void MarkBackwardPrunedSubtree(
            DisplayLaneNode pruneStartNode,
            IDictionary<string, List<DisplayLaneNode>> childrenMap,
            ISet<string> prunedDisplayNodeKeys)
        {
            if (pruneStartNode == null)
                return;

            if (childrenMap == null)
                return;

            if (prunedDisplayNodeKeys == null)
                return;

            var stack = new Stack<DisplayLaneNode>();
            stack.Push(pruneStartNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current == null)
                    continue;

                string currentDisplayNodeKey = current.DisplayNodeKey;
                if (string.IsNullOrWhiteSpace(currentDisplayNodeKey))
                    continue;

                // 既に処理済みならスキップ
                if (!prunedDisplayNodeKeys.Add(currentDisplayNodeKey))
                    continue;

                List<DisplayLaneNode> children;
                if (!childrenMap.TryGetValue(currentDisplayNodeKey, out children) ||
                    children == null ||
                    children.Count == 0)
                {
                    continue;
                }

                foreach (var child in children)
                {
                    if (child == null)
                        continue;

                    stack.Push(child);
                }
            }
        }

        private void DumpBackwardPrunedDisplayNodeKeys(
            ISet<string> prunedDisplayNodeKeys,
            string fileName)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string debugDir = Path.Combine(baseDir, "Debug");
                Directory.CreateDirectory(debugDir);

                string safeFileName = string.IsNullOrWhiteSpace(fileName)
                    ? "LotTrace_Debug_BackwardPrunedDisplayNodeKeys.csv"
                    : fileName;

                string outputPath = Path.Combine(debugDir, safeFileName);

                using (var sw = new StreamWriter(outputPath, false, new UTF8Encoding(true)))
                {
                    sw.WriteLine(
                        "DisplayNodeKey,ParentDisplayNodeKey,XLevel,YLane,NodeIdentityKey,RouteSystem,LotNumber,ControlMasterKey");

                    if (prunedDisplayNodeKeys == null || prunedDisplayNodeKeys.Count == 0)
                        return;

                    // ダンプ見やすさのため、キー→Node を一旦引けるようにしておく
                    var allDisplayNodes = new List<DisplayLaneNode>();

                    if (_currentForwardLaneBuildResult != null &&
                        _currentForwardLaneBuildResult.DisplayNodes != null)
                    {
                        allDisplayNodes.AddRange(_currentForwardLaneBuildResult.DisplayNodes);
                    }

                    var nodeMap = allDisplayNodes
                        .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                        .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                    foreach (var key in prunedDisplayNodeKeys
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    {
                        DisplayLaneNode node;
                        nodeMap.TryGetValue(key, out node);

                        var sourceNode = node == null ? null : node.SourceNode;

                        sw.WriteLine(string.Join(",",
                            EscapeCsv(key),
                            EscapeCsv(node == null ? null : node.ParentDisplayNodeKey),
                            EscapeCsv(node == null ? null : node.XLevel.ToString()),
                            EscapeCsv(node == null ? null : node.YLane.ToString()),
                            EscapeCsv(sourceNode == null ? null : sourceNode.NodeIdentityKey),
                            EscapeCsv(sourceNode == null ? null : sourceNode.RouteSystem),
                            EscapeCsv(sourceNode == null ? null : sourceNode.LotNumber),
                            EscapeCsv(sourceNode == null ? null : sourceNode.ControlMasterKey)
                        ));
                    }
                }
            }
            catch
            {
                // Debug dump failure must not break trace flow.
            }
        }

        #endregion

        #region デバッグCSV出力

        

       

        private void DumpTraceDisplayLaneNodes(
            TraceDirection direction,
            ForwardDisplayLaneBuildResult laneBuildResult,
            string phase)
        {
            try
            {
                DumpDisplayLaneNodesToFile(
                    direction,
                    laneBuildResult == null ? null : laneBuildResult.DisplayNodes,
                    phase);
            }
            catch
            {
                // デバッグ出力失敗は本処理を止めない
            }
        }

        
        private string EnsureTraceDebugFolderExists()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string debugDir = Path.Combine(baseDir, "Debug", "Trace");

            if (!Directory.Exists(debugDir))
            {
                Directory.CreateDirectory(debugDir);
            }

            return debugDir;
        }

        private void DumpDisplayLaneNodesToFile(
            TraceDirection direction,
            List<DisplayLaneNode> nodes,
            string phase)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            try
            {
                string folder = EnsureTraceDebugFolderExists();
                string safeDirection = direction.ToString();
                string path = Path.Combine(
                    folder,
                    "LotTrace_Debug_" + safeDirection + "_DisplayLaneNodes.csv");

                var sb = new StringBuilder();

                sb.AppendLine(
                    "Direction,Phase,DisplayNodeKey,ParentDisplayNodeKey,MergeKey,XLevel,YLane,ChildIndex,LotGroupKey,RepresentativeNodeKey,OccupiedFirstY,OccupiedLastY,ProductionOrderNumber,ItemCode,ItemName,LotNumber,ControlMasterKey,RouteSystem,InputSlotNo,InputSourceType,Depth,NodeType");

                foreach (var node in nodes
                    .Where(x => x != null)
                    .OrderBy(x => x.XLevel)
                    .ThenBy(x => x.YLane)
                    .ThenBy(x => x.ChildIndex))
                {
                    var src = node.SourceNode;

                    sb.Append(EscapeCsv(safeDirection)).Append(",");
                    sb.Append(EscapeCsv(phase)).Append(",");
                    sb.Append(EscapeCsv(node.DisplayNodeKey)).Append(",");
                    sb.Append(EscapeCsv(node.ParentDisplayNodeKey)).Append(",");
                    sb.Append(EscapeCsv(node.MergeKey)).Append(",");
                    sb.Append(node.XLevel.ToString()).Append(",");
                    sb.Append(node.YLane.ToString()).Append(",");
                    sb.Append(node.ChildIndex.ToString()).Append(",");
                    sb.Append(EscapeCsv(node.LotGroupKey)).Append(",");
                    sb.Append(EscapeCsv(node.RepresentativeNodeKey)).Append(",");
                    sb.Append(node.OccupiedFirstY.ToString()).Append(",");
                    sb.Append(node.OccupiedLastY.ToString()).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.ProductionOrderNumber)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.ItemCode)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.ItemName)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.LotNumber)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.ControlMasterKey)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.RouteSystem)).Append(",");
                    sb.Append(EscapeCsv(src == null || !src.InputSlotNo.HasValue
                        ? null
                        : src.InputSlotNo.Value.ToString())).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.InputSourceType)).Append(",");
                    sb.Append(src == null ? "" : src.Depth.ToString()).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.NodeType));
                    sb.AppendLine();
                }

                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // ダンプ失敗は本処理を止めない
            }
        }

        
        private string EscapeCsv(string value)
        {
            if (value == null)
                return "\"\"";

            string s = value.Replace("\"", "\"\"");
            return "\"" + s + "\"";
        }


        #endregion
    }
}
