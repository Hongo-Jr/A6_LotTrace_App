using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using LotTraceApp.Models;
using LotTraceApp.Repositories;

using System.Reflection;


namespace LotTraceApp.Services
{
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

        private sealed class ForwardChildBundle
        {
            public int Level { get; set; }
            public string LotKey { get; set; }
            public string ParentMergeKey { get; set; }

            public List<ChildCandidate> Members { get; private set; }

            public ChildCandidate Representative { get; set; }

            public ForwardChildBundle()
            {
                Members = new List<ChildCandidate>();
            }
        }



        public LotTraceService(LotTraceRepository repo, ICustomerItemMasterRepository customerItemMasterRepository)
        {
            if (repo == null) throw new ArgumentNullException("repo");
            _repo = repo;
            _customerItemMasterRepository = customerItemMasterRepository;
        }

        public TraceResult ExecuteTrace(TraceSearchParameters p)
        {
            if (p == null) throw new ArgumentNullException("p");

            if (p.Direction == TraceDirection.Forward)
            {
                return TraceForward(p);
            }
            else
            {
                return TraceBackward(p);
            }
        }
       

        public DataTable BuildDisplayTable(TraceResult traceResult)
        {
            var displayResult = BuildDisplayResult(traceResult, new TraceDisplayBuildOptions
            {
                SuppressDuplicateStartCells = false,
                SuppressDuplicateMiddleCells = false,
                SuppressDuplicateEndCells = false,
                SortBeforeSuppress = false
            });

            return BuildDisplayTableFromDisplayResult(displayResult);
        }

        public TraceDisplayResult BuildDisplayResult(
    TraceResult traceResult,
    TraceDisplayBuildOptions options = null)
        {
            var displayResult = new TraceDisplayResult();
            displayResult.Options = options ?? new TraceDisplayBuildOptions();

            if (traceResult == null || traceResult.PathRows == null || traceResult.PathRows.Count == 0)
                return displayResult;

            for (int rowIndex = 0; rowIndex < traceResult.PathRows.Count; rowIndex++)
            {
                var pathRow = traceResult.PathRows[rowIndex];
                if (pathRow == null)
                    continue;

                

                var row = new TraceDisplayRow
                {
                    DisplayOrder = rowIndex,
                    RootGroupKey = pathRow.RootGroupKey,
                    RootNodeKey = GetNodeIdentityKey(pathRow.StartNode),
                    PathKey = BuildPathKey(pathRow),
                    RouteSystem = ResolveRouteSystem(pathRow),
                    StartTrunkGroupKey = pathRow.StartTrunkGroupKey,
                    StartTrunkOrder = pathRow.StartTrunkOrder,
                    FullPathSignature = pathRow.FullPathSignature,

                    LotTraceKey = pathRow == null ? null : BuildLotTraceKey(pathRow),
                    UpstreamLotOnlyKey = pathRow == null ? null : BuildUpstreamLotOnlyKey(pathRow),

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
                row.StartRowOrderInGroup = pathRow.PathOrder;
                row.IsFirstRowOfStartGroup = false;
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

            MarkRootGroupBoundaries(displayResult.Rows);


            // 例外配置:
            // IsLastRowOfStartGroup は UI契約のため残す。
            // ただし判定根拠は rows 後段推測ではなく、
            // フォワード枝構造確定時の OccupiedLotGroupRanges(Level=0) を使用する。
            MarkStartGroupBoundaries(
                displayResult.Rows,
                _currentForwardLaneBuildResult);

            // 構造側で確定した線情報をそのまま橋渡し
            PopulateLineRanges(displayResult, traceResult.TraceLineRanges);

            DumpLineDecisionKeys(displayResult);

            return displayResult;
        }

        #region トレースフォワード（仕様書 7.1）




        private TraceResult TraceForward(TraceSearchParameters p)
        {
            var result = new TraceResult();

            var nodeMap = new Dictionary<string, ProductionResultNode>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<ProductionResultNode>();
            var queuedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 追加:
            // Link の全体重複排除用
            var linkMap = new Dictionary<string, ProductionResultLink>(StringComparer.OrdinalIgnoreCase);

            // 1. 始点取得
            // 1. 始点取得
            var startB = _repo.FindStartNodesFromMaterialTableB(p);
            var startA = _repo.FindStartNodesFromMaterialTableAManualInput(p);

            // StartB に 新StartA を統合
            if (startA != null && startA.Count > 0)
            {
                if (startB == null)
                    startB = new List<ProductionResultNode>();

                startB.AddRange(startA);
            }

            // 統合後の startB が 0 件のときだけ、既存StartA を取得
            if (startB == null || startB.Count == 0)
            {
                startA = _repo.FindStartNodesFromMaterialTableA(p);
            }
            else
            {
                // フォールバック未使用を明示
                startA = new List<ProductionResultNode>();
            }

            // B始点群と同じLotNoを持つA始点は採用しない
            var filteredStartA = FilterAStartNodesByBLots(startA, startB);

            // フォワード専用の始点前処理
            var preparedStartB = PrepareForwardStartNodesForExpansion(startB, "B");
            var preparedStartA = PrepareForwardStartNodesForExpansion(filteredStartA, "A");

            // B始点優先
            foreach (var entry in preparedStartB)
            {
                if (entry == null || entry.Node == null)
                    continue;

                var raw = entry.Node;

                raw.Depth = 0;
                raw.ParentKey = null;

                ApplyStartNodeBusinessAttributes(raw, "B");

                var node = GetOrAddNode(nodeMap, raw);
                EnsureNodeRegistered(result, node);
                EnsureRootRegistered(result, node);

                if (entry.ExpandChildren)
                {
                    string queueKey = GetNodeMergeKey(node);
                    if (string.IsNullOrWhiteSpace(queueKey) || queuedKeys.Add(queueKey))
                    {
                        queue.Enqueue(node);
                    }
                }
            }

            // A始点
            foreach (var entry in preparedStartA)
            {
                if (entry == null || entry.Node == null)
                    continue;

                var raw = entry.Node;

                raw.Depth = 0;
                raw.ParentKey = null;
                raw.StartDateLabel = ResolveStartDateLabel(raw, null);

                ApplyStartNodeBusinessAttributes(raw, "A");

                var node = GetOrAddNode(nodeMap, raw);
                EnsureNodeRegistered(result, node);
                EnsureRootRegistered(result, node);

                if (entry.ExpandChildren)
                {
                    string queueKey = GetNodeMergeKey(node);
                    if (string.IsNullOrWhiteSpace(queueKey) || queuedKeys.Add(queueKey))
                    {
                        queue.Enqueue(node);
                    }
                }
            }

            if (queue.Count == 0)
            {
                RebuildLegacyNodeLists(result);
                return result;
            }

            // 2. BFS
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == null)
                    continue;

                if (current.Depth >= MaxDepth)
                    continue;

                WriteCurrentNodeDebug(current);

                var childCandidates = new List<ChildCandidate>();

                var candidatesA = _repo.FindForwardChildrenFromMaterialTableA(current, current.Depth + 1);
                if (candidatesA != null && candidatesA.Count > 0)
                {
                    childCandidates.AddRange(candidatesA);
                }

                var candidatesB = _repo.FindForwardChildrenFromMaterialTableB(current, current.Depth + 1);
                if (candidatesB != null && candidatesB.Count > 0)
                {
                    childCandidates.AddRange(candidatesB);
                }

                WriteChildCandidatesDebugCsv(current, candidatesA, candidatesB, childCandidates);
                WriteChildCandidateDetailsDebugCsv(current, candidatesA, "A");
                WriteChildCandidateDetailsDebugCsv(current, candidatesB, "B");
                WriteChildCandidateDetailsDebugCsv(current, childCandidates, "Merged");

                foreach (var candidate in childCandidates)
                {
                    if (candidate == null || candidate.Node == null)
                        continue;

                    var rawChild = candidate.Node;

                    if (string.IsNullOrWhiteSpace(rawChild.LotNumber) &&
                        string.IsNullOrWhiteSpace(rawChild.ControlMasterKey))
                    {
                        continue;
                    }

                    rawChild.Depth = current.Depth + 1;
                    rawChild.StartDateLabel = ResolveStartDateLabel(rawChild, candidate);

                    ApplyChildNodeBusinessAttributes(rawChild, candidate);

                    // 旧互換
                    rawChild.ParentKey = GetNodeMergeKey(current);

                    // Nodeは MergeKey で統合
                    var childNode = GetOrAddNode(nodeMap, rawChild);
                    EnsureNodeRegistered(result, childNode);

                    // Linkは LinkIdentityKey で統合
                    bool linkAdded = AddLinkFromCandidate(result, linkMap, current, childNode, candidate);

                    // 新規リンクが追加されたときだけ、下流探索候補として考える
                    if (linkAdded)
                    {
                        string childQueueKey = GetNodeMergeKey(childNode);
                        if (string.IsNullOrWhiteSpace(childQueueKey) || queuedKeys.Add(childQueueKey))
                        {
                            queue.Enqueue(childNode);
                        }
                    }
                }
            }

            RebuildLegacyNodeLists(result);

            // 木構造の構築と枝Sort確定は入口メソッドへ集約
            var laneBuildResult = BuildSortedForwardDisplayLaneNodes(result);

            // ここで構造ベースの肉付け
            PopulateLineDecisionInfos(laneBuildResult.DisplayNodes);


            // Occupied 確定
            laneBuildResult.OccupiedLotGroupRanges =
                NormalizeOccupiedRangeByLotGroup(laneBuildResult.DisplayNodes);

            // 追加：BuildDisplayResult から始点境界判定に使用するため一時保持
            _currentForwardLaneBuildResult = laneBuildResult;

            // ★追加：橋渡し確認ダンプ
            DumpDisplayLaneNodes(laneBuildResult.DisplayNodes);
            DumpOccupiedLotGroupRanges(laneBuildResult.OccupiedLotGroupRanges);

            // ★追加（構造 → 線生成）
            var traceLineRanges =
            BuildTraceLineRangesFromOccupiedGroups(
                laneBuildResult.OccupiedLotGroupRanges);

            // ★追加：
            // PathRows ではなく、枝構造確定直後の laneBuildResult を使って
            // Middle 実体を持たない Start→End 枝の Middle 幹線を補完する
            AppendSyntheticMiddleLineRangesForDirectStartEndBranches(
                traceLineRanges,
                laneBuildResult);

            // ★追加：橋渡し確認ダンプ
            DumpTraceLineRanges(traceLineRanges);

            // ★追加（橋渡し：Forward → TraceResult）
            result.TraceLineRanges.Clear();

            if (traceLineRanges != null && traceLineRanges.Count > 0)
            {
                result.TraceLineRanges.AddRange(traceLineRanges);
            }

            // 構造 → PathRows
            BuildPathRowsFromDisplayLaneNodes(result, laneBuildResult);

            //もう使わない
            //BuildPathRows(result, true);

            ResolveItemNamesForNodes(result.AllNodes);
            ExportAllNodesDebugCsv(result.AllNodes, "LotTrace_Debug_AllNodes.csv");

            return result;
        }

        private List<ForwardStartEntry> PrepareForwardStartNodesForExpansion(
    List<ProductionResultNode> rawStarts,
    string routeSystem)
        {
            var result = new List<ForwardStartEntry>();

            if (rawStarts == null || rawStarts.Count == 0)
                return result;

            // ------------------------------------------------------------
            // STEP1:
            // Node統合キーが同じ始点は最初の1件だけ採用
            // ------------------------------------------------------------
            var acceptedNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var step1Nodes = new List<ProductionResultNode>();

            foreach (var raw in rawStarts)
            {
                if (raw == null)
                    continue;

                ApplyStartNodeBusinessAttributes(raw, routeSystem);

                string nodeKey = GetNodeMergeKey(raw);

                // merge key が取れないものは安全側で通す
                if (string.IsNullOrWhiteSpace(nodeKey))
                {
                    step1Nodes.Add(raw);
                    continue;
                }

                if (!acceptedNodeKeys.Add(nodeKey))
                {
                    // 同一Node識別子は不採用
                    continue;
                }

                step1Nodes.Add(raw);
            }

            if (step1Nodes.Count == 0)
                return result;

            // ------------------------------------------------------------
            // STEP2:
            // Lot同一の始点は全件表示対象に残す
            // ただし、子探索して木を伸ばすのは各Lotで1件だけ
            // ------------------------------------------------------------
            var expandedLotKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in step1Nodes)
            {
                if (node == null)
                    continue;

                string lotKey = NormalizeForwardStartLot(node.LotNumber);

                bool expandChildren;

                if (string.IsNullOrWhiteSpace(lotKey))
                {
                    // Lotなし始点は個別に展開対象
                    expandChildren = true;
                }
                else
                {
                    // 同一Lotで最初の1件だけ展開
                    expandChildren = expandedLotKeys.Add(lotKey);
                }

                result.Add(new ForwardStartEntry
                {
                    Node = node,
                    ExpandChildren = expandChildren
                });
            }

            return result;
        }

        private List<ProductionResultNode> FilterAStartNodesByBLots(
    List<ProductionResultNode> startA,
    List<ProductionResultNode> startB)
        {
            var result = new List<ProductionResultNode>();

            if (startA == null || startA.Count == 0)
                return result;

            var bLots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (startB != null)
            {
                foreach (var b in startB)
                {
                    if (b == null)
                        continue;

                    string lot = NormalizeLotForStartFilter(b.LotNumber);
                    if (string.IsNullOrWhiteSpace(lot))
                        continue;

                    bLots.Add(lot);
                }
            }

            foreach (var a in startA)
            {
                if (a == null)
                    continue;

                string lot = NormalizeLotForStartFilter(a.LotNumber);

                // A始点のLotが空なら比較不能なので残す
                if (string.IsNullOrWhiteSpace(lot))
                {
                    result.Add(a);
                    continue;
                }

                // B始点群に同LotがいるA始点は採用しない
                if (bLots.Contains(lot))
                    continue;

                result.Add(a);
            }

            return result;
        }

        private void ApplyStartNodeBusinessAttributes(ProductionResultNode node, string routeSystem)
        {
            if (node == null)
                return;

            if (string.IsNullOrWhiteSpace(node.RouteSystem))
                node.RouteSystem = routeSystem;

            // 始点は A/B だけ持てばまず十分
            if (!node.InputSlotNo.HasValue)
                node.InputSlotNo = 0;

            if (string.IsNullOrWhiteSpace(node.InputSourceType))
                node.InputSourceType = null;

            if (!node.IsTraceTerminal)
                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);
        }

        private void ApplyChildNodeBusinessAttributes(ProductionResultNode node, ChildCandidate candidate)
        {
            if (node == null || candidate == null)
                return;

            if (string.IsNullOrWhiteSpace(node.RouteSystem))
            {
                if (candidate.SourceTable == TraceSourceTable.MaterialTableB)
                {
                    node.RouteSystem = "B";
                }
                else if (candidate.SourceTable == TraceSourceTable.MaterialTableA)
                {
                    node.RouteSystem = "A";
                }
            }

            if (candidate.SourceTable == TraceSourceTable.MaterialTableA)
            {
                if (!node.InputSlotNo.HasValue)
                    node.InputSlotNo = candidate.SlotNo;

                if (string.IsNullOrWhiteSpace(node.InputSourceType))
                    node.InputSourceType = candidate.MaterialAInputType.ToString();
            }
            else if (candidate.SourceTable == TraceSourceTable.MaterialTableB)
            {
                if (!node.InputSlotNo.HasValue)
                    node.InputSlotNo = candidate.SlotNo;

                if (string.IsNullOrWhiteSpace(node.InputSourceType))
                    node.InputSourceType = null;
            }

            if (!node.IsTraceTerminal)
                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);
        }

        private ProductionResultNode GetOrAddNode(
    Dictionary<string, ProductionResultNode> nodeMap,
    ProductionResultNode rawNode)
        {
            if (nodeMap == null)
                throw new ArgumentNullException("nodeMap");

            if (rawNode == null)
                return null;

            string mergeKey = GetNodeMergeKey(rawNode);

            // MergeKey が取れないものは安全側で都度そのまま返す
            if (string.IsNullOrWhiteSpace(mergeKey))
            {
                EnsureNodeCollections(rawNode);
                return rawNode;
            }

            ProductionResultNode existing;
            if (nodeMap.TryGetValue(mergeKey, out existing) && existing != null)
            {
                MergeNodeSnapshot(existing, rawNode);
                EnsureNodeCollections(existing);
                return existing;
            }

            EnsureNodeCollections(rawNode);
            nodeMap[mergeKey] = rawNode;
            return rawNode;
        }

        private void MergeNodeSnapshot(ProductionResultNode target, ProductionResultNode source)
        {
            if (target == null || source == null)
                return;

            // 基本は「空なら埋める」
            if (string.IsNullOrWhiteSpace(target.ProductionOrderNumber))
                target.ProductionOrderNumber = source.ProductionOrderNumber;

            if (string.IsNullOrWhiteSpace(target.LotNumber))
                target.LotNumber = source.LotNumber;

            if (string.IsNullOrWhiteSpace(target.ItemCode))
                target.ItemCode = source.ItemCode;

            if (string.IsNullOrWhiteSpace(target.ItemName))
                target.ItemName = source.ItemName;

            if (!target.StartDate.HasValue && source.StartDate.HasValue)
                target.StartDate = source.StartDate;

            if (string.IsNullOrWhiteSpace(target.StartDateLabel))
                target.StartDateLabel = source.StartDateLabel;

            if (!target.EndDate.HasValue && source.EndDate.HasValue)
                target.EndDate = source.EndDate;

            if (string.IsNullOrWhiteSpace(target.ManufacturingProcessName))
                target.ManufacturingProcessName = source.ManufacturingProcessName;

            if (string.IsNullOrWhiteSpace(target.ManufacturingTankName))
                target.ManufacturingTankName = source.ManufacturingTankName;

            if (!target.Weight.HasValue && source.Weight.HasValue)
                target.Weight = source.Weight;

            if (string.IsNullOrWhiteSpace(target.ControlMasterKey))
                target.ControlMasterKey = source.ControlMasterKey;

            if (string.IsNullOrWhiteSpace(target.ParentKey))
                target.ParentKey = source.ParentKey;

            if (string.IsNullOrWhiteSpace(target.ParentMasterKey))
                target.ParentMasterKey = source.ParentMasterKey;

            if (string.IsNullOrWhiteSpace(target.RouteSystem))
                target.RouteSystem = source.RouteSystem;

            if (!target.InputSlotNo.HasValue && source.InputSlotNo.HasValue)
                target.InputSlotNo = source.InputSlotNo;

            if (string.IsNullOrWhiteSpace(target.InputSourceType))
                target.InputSourceType = source.InputSourceType;

            if (!target.IsTraceTerminal)
                target.IsTraceTerminal = source.IsTraceTerminal;

            // Depth は浅い方を採用
            if (source.Depth < target.Depth)
                target.Depth = source.Depth;
        }

        private void EnsureNodeCollections(ProductionResultNode node)
        {
            if (node == null)
                return;

            if (node.ParentNodes == null)
                throw new InvalidOperationException("ProductionResultNode.ParentNodes must be initialized.");

            if (node.ChildNodes == null)
                throw new InvalidOperationException("ProductionResultNode.ChildNodes must be initialized.");

            if (node.ParentLinks == null)
                throw new InvalidOperationException("ProductionResultNode.ParentLinks must be initialized.");

            if (node.ChildLinks == null)
                throw new InvalidOperationException("ProductionResultNode.ChildLinks must be initialized.");
        }

        private bool AddLinkFromCandidate(
    TraceResult result,
    Dictionary<string, ProductionResultLink> linkMap,
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ChildCandidate candidate)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (linkMap == null)
                throw new ArgumentNullException("linkMap");

            if (parentNode == null || childNode == null || candidate == null)
                return false;

            EnsureNodeCollections(parentNode);
            EnsureNodeCollections(childNode);

            string linkKey = BuildLinkIdentityKey(parentNode, childNode, candidate);
            if (string.IsNullOrWhiteSpace(linkKey))
                return false;

            ProductionResultLink existingLink;
            if (linkMap.TryGetValue(linkKey, out existingLink) && existingLink != null)
            {
                // 既存リンクは再利用
                EnsureParentChildNodeRelationship(parentNode, childNode);
                EnsureParentChildLinkRelationship(parentNode, childNode, existingLink);
                return false;
            }

            var link = new ProductionResultLink
            {
                ParentNode = parentNode,
                ChildNode = childNode,
                EdgeDirection = TraceEdgeDirection.ParentToChild,
                RootGroupKey = GetNodeIdentityKey(parentNode),
                ParentLotNumber = candidate.ParentLotNumber,
                SourceTable = candidate.SourceTable,
                MaterialAInputType = candidate.MaterialAInputType,
                SlotNo = candidate.SlotNo,
                LinkIdentityKey = linkKey
            };

            linkMap[linkKey] = link;
            result.AllLinks.Add(link);

            EnsureParentChildNodeRelationship(parentNode, childNode);
            EnsureParentChildLinkRelationship(parentNode, childNode, link);

            return true;
        }

        private void EnsureParentChildNodeRelationship(
    ProductionResultNode parentNode,
    ProductionResultNode childNode)
        {
            if (parentNode == null || childNode == null)
                return;

            EnsureNodeCollections(parentNode);
            EnsureNodeCollections(childNode);

            if (!parentNode.ChildNodes.Any(x => object.ReferenceEquals(x, childNode)))
            {
                parentNode.ChildNodes.Add(childNode);
            }

            if (!childNode.ParentNodes.Any(x => object.ReferenceEquals(x, parentNode)))
            {
                childNode.ParentNodes.Add(parentNode);
            }
        }

        private void EnsureParentChildLinkRelationship(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ProductionResultLink link)
        {
            if (parentNode == null || childNode == null || link == null)
                return;

            EnsureNodeCollections(parentNode);
            EnsureNodeCollections(childNode);

            if (!parentNode.ChildLinks.Any(x =>
                x != null &&
                string.Equals(x.LinkIdentityKey, link.LinkIdentityKey, StringComparison.OrdinalIgnoreCase)))
            {
                parentNode.ChildLinks.Add(link);
            }

            if (!childNode.ParentLinks.Any(x =>
                x != null &&
                string.Equals(x.LinkIdentityKey, link.LinkIdentityKey, StringComparison.OrdinalIgnoreCase)))
            {
                childNode.ParentLinks.Add(link);
            }
        }

        private string BuildLinkIdentityKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ChildCandidate candidate)
        {
            if (candidate != null && !string.IsNullOrWhiteSpace(candidate.LinkIdentityKey))
                return candidate.LinkIdentityKey.Trim();

            string parentMergeKey = GetNodeMergeKey(parentNode) ?? string.Empty;
            string childMergeKey = GetNodeMergeKey(childNode) ?? string.Empty;

            string sourceTable = candidate == null
                ? string.Empty
                : candidate.SourceTable.ToString();

            string inputType = candidate == null
                ? string.Empty
                : candidate.MaterialAInputType.ToString();

            string slotNo = candidate == null
                ? string.Empty
                : candidate.SlotNo.ToString();

            string parentLot = candidate == null || string.IsNullOrWhiteSpace(candidate.ParentLotNumber)
                ? string.Empty
                : candidate.ParentLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                parentMergeKey,
                sourceTable,
                inputType,
                slotNo,
                parentLot,
                childMergeKey);
        }

        

        private string BuildStartTrunkGroupKey(ProductionResultNode startNode)
        {
            if (startNode == null)
                return null;

            // Lot優先
            if (!string.IsNullOrWhiteSpace(startNode.LotNumber))
            {
                return "STARTLOT|" + startNode.LotNumber.Trim().ToUpperInvariant();
            }

            // fallback: NodeMergeKey
            string mergeKey = GetNodeMergeKey(startNode);
            if (!string.IsNullOrWhiteSpace(mergeKey))
            {
                return "STARTNODE|" + mergeKey;
            }

            return null;
        }

        private sealed class ForwardStartEntry
        {
            public ProductionResultNode Node { get; set; }
            public bool ExpandChildren { get; set; }
        }

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

            // 既存：内部計算用として当面残す
            public int SubtreeLaneSpan { get; set; }

            // 追加：意味を明示した高さ・占有情報
            public int RootGroupHeight { get; set; }     // 同一始点LotグループのLv0高さ
            public int ChildBranchHeight { get; set; }   // このNode直下の子枝総高さ
            public int OccupiedFirstY { get; set; }      // このNode起点の占有開始Y
            public int OccupiedLastY { get; set; }       // このNode起点の占有終了Y

            public List<DisplayLaneEdge> OutgoingEdges { get; private set; }
            public bool IsLotGroupRepresentative { get; set; }
            public bool IsLotGroupNonRepresentative { get; set; }

            public string LotGroupKey { get; set; }
            public string RepresentativeNodeKey { get; set; }


            public DisplayLaneNode()
            {
                OutgoingEdges = new List<DisplayLaneEdge>();

                SubtreeLaneSpan = 1;

                RootGroupHeight = 1;
                ChildBranchHeight = 0;
                OccupiedFirstY = -1;
                OccupiedLastY = -1;
            }
        }


        private sealed class DisplayLaneEdge
        {
            public string EdgeIdentityKey { get; set; }

            public string ParentDisplayNodeKey { get; set; }
            public string ChildDisplayNodeKey { get; set; }

            public string ParentMergeKey { get; set; }
            public string ChildMergeKey { get; set; }

            public int FromXLevel { get; set; }
            public int ToXLevel { get; set; }

            public int FromYLane { get; set; }
            public int ToYLane { get; set; }

            public int ChildIndex { get; set; }
            public string LotGroupKey { get; set; }

            // 将来拡張用
            public List<List<string>> EdgeContextMatrix { get; private set; }

            public DisplayLaneEdge()
            {
                EdgeContextMatrix = new List<List<string>>();
            }
        }




        private List<ProductionResultNode> PrepareForwardStartNodes(
    List<ProductionResultNode> rawStarts,
    string routeSystem)
        {
            var result = new List<ProductionResultNode>();

            if (rawStarts == null || rawStarts.Count == 0)
                return result;

            // ------------------------------------------------------------
            // STEP1:
            // Node統合キーが同じ始点は最初の1件だけ採用
            // ------------------------------------------------------------
            var acceptedNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var step1 = new List<ProductionResultNode>();

            foreach (var raw in rawStarts)
            {
                if (raw == null)
                    continue;

                // 始点の業務属性を先に最低限そろえる
                ApplyStartNodeBusinessAttributes(raw, routeSystem);

                string nodeKey = GetNodeMergeKey(raw);

                // キーが取れないものは安全側でそのまま通す
                if (string.IsNullOrWhiteSpace(nodeKey))
                {
                    step1.Add(raw);
                    continue;
                }

                if (!acceptedNodeKeys.Add(nodeKey))
                {
                    // 同一Node識別子は不採用
                    continue;
                }

                step1.Add(raw);
            }

            if (step1.Count == 0)
                return result;

            // ------------------------------------------------------------
            // STEP2:
            // Lot同一の始点は別Nodeとして保持しつつ、
            // 同一幹グループとして扱う前提で順番をまとめる
            //
            // ※ Edgeはここでは作らない。
            // ※ 右グリッド上詰めのための完全処理は表示構築側だが、
            //    まずは同Lot始点が連続して並ぶ順序をここで保証する。
            // ------------------------------------------------------------
            var lotBuckets = new Dictionary<string, List<ProductionResultNode>>(StringComparer.OrdinalIgnoreCase);
            var lotOrder = new List<string>();
            var noLotNodes = new List<ProductionResultNode>();

            foreach (var node in step1)
            {
                if (node == null)
                    continue;

                string lotKey = NormalizeForwardStartLot(node.LotNumber);
                if (string.IsNullOrWhiteSpace(lotKey))
                {
                    noLotNodes.Add(node);
                    continue;
                }

                List<ProductionResultNode> bucket;
                if (!lotBuckets.TryGetValue(lotKey, out bucket))
                {
                    bucket = new List<ProductionResultNode>();
                    lotBuckets[lotKey] = bucket;
                    lotOrder.Add(lotKey);
                }

                bucket.Add(node);
            }

            // Lotあり始点を同Lotごとに連続配置
            foreach (var lotKey in lotOrder)
            {
                List<ProductionResultNode> bucket;
                if (!lotBuckets.TryGetValue(lotKey, out bucket) || bucket == null)
                    continue;

                // 安定順序:
                //  1. StartDateありを優先
                //  2. StartDate昇順
                //  3. ControlMasterKey
                bucket.Sort(CompareForwardStartNodesForSameLot);

                foreach (var node in bucket)
                {
                    result.Add(node);
                }
            }

            // Lotなし始点は最後
            if (noLotNodes.Count > 0)
            {
                noLotNodes.Sort(CompareForwardStartNodesForSameLot);

                foreach (var node in noLotNodes)
                {
                    result.Add(node);
                }
            }

            return result;
        }

        private string NormalizeForwardStartLot(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                return null;

            return lotNumber.Trim();
        }

        private List<ForwardChildBundle> PrepareForwardMiddleChildBundles(
    ProductionResultNode parentNode,
    List<ChildCandidate> childCandidates)
        {
            var result = new List<ForwardChildBundle>();

            if (parentNode == null || childCandidates == null || childCandidates.Count == 0)
                return result;

            int nextLevel = parentNode.Depth + 1;
            string parentMergeKey = GetNodeMergeKey(parentNode) ?? string.Empty;

            var bundleMap = new Dictionary<string, ForwardChildBundle>(StringComparer.OrdinalIgnoreCase);
            var bundleOrder = new List<string>();

            foreach (var candidate in childCandidates)
            {
                if (candidate == null || candidate.Node == null)
                    continue;

                var node = candidate.Node;

                if (string.IsNullOrWhiteSpace(node.LotNumber) &&
                    string.IsNullOrWhiteSpace(node.ControlMasterKey))
                {
                    continue;
                }

                string lotKey = NormalizeLotForForwardMiddleBundle(node.LotNumber);

                // Lotなしは束ねず個別枝として扱う
                string bundleKey;
                if (string.IsNullOrWhiteSpace(lotKey))
                {
                    string nodeMergeKey = GetNodeMergeKey(node) ?? string.Empty;
                    string edgeKey = BuildForwardMiddleBundleEdgeKey(parentNode, node, candidate);

                    bundleKey = string.Join("|",
                        "LV", nextLevel.ToString(),
                        "PARENT", parentMergeKey,
                        "NOLOT",
                        "NODE", nodeMergeKey,
                        "EDGE", edgeKey);
                }
                else
                {
                    bundleKey = string.Join("|",
                        "LV", nextLevel.ToString(),
                        "PARENT", parentMergeKey,
                        "LOT", lotKey);
                }

                ForwardChildBundle bundle;
                if (!bundleMap.TryGetValue(bundleKey, out bundle))
                {
                    bundle = new ForwardChildBundle
                    {
                        Level = nextLevel,
                        LotKey = lotKey,
                        ParentMergeKey = parentMergeKey
                    };

                    bundleMap[bundleKey] = bundle;
                    bundleOrder.Add(bundleKey);
                }

                bundle.Members.Add(candidate);
            }

            foreach (var key in bundleOrder)
            {
                ForwardChildBundle bundle;
                if (!bundleMap.TryGetValue(key, out bundle) || bundle == null)
                    continue;

                bundle.Representative = SelectRepresentativeChildCandidateForBundle(bundle);
                result.Add(bundle);
            }

            return result;
        }

        private ChildCandidate SelectRepresentativeChildCandidateForBundle(ForwardChildBundle bundle)
        {
            if (bundle == null || bundle.Members == null || bundle.Members.Count == 0)
                return null;

            return bundle.Members
                .Where(x => x != null && x.Node != null)
                .OrderBy(x => BuildRepresentativeChildCandidateSortKey(x), StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }

        private string BuildRepresentativeChildCandidateSortKey(ChildCandidate candidate)
        {
            if (candidate == null || candidate.Node == null)
                return "~~~";

            var node = candidate.Node;

            string mergeKey = GetNodeMergeKey(node) ?? string.Empty;
            string startDateKey = node.StartDate.HasValue
                ? node.StartDate.Value.ToString("yyyyMMddHHmmssfff")
                : "99999999999999999";
            string masterKey = string.IsNullOrWhiteSpace(node.ControlMasterKey)
                ? string.Empty
                : node.ControlMasterKey.Trim();
            string lotKey = string.IsNullOrWhiteSpace(node.LotNumber)
                ? string.Empty
                : node.LotNumber.Trim();

            return string.Join("|",
                mergeKey,
                startDateKey,
                masterKey,
                lotKey);
        }

        private string NormalizeLotForForwardMiddleBundle(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                return null;

            return lotNumber.Trim();
        }

        private string BuildForwardMiddleBundleEdgeKey(
            ProductionResultNode parentNode,
            ProductionResultNode childNode,
            ChildCandidate candidate)
        {
            string parentMergeKey = GetNodeMergeKey(parentNode) ?? string.Empty;
            string childMergeKey = GetNodeMergeKey(childNode) ?? string.Empty;

            string sourceTable = candidate == null
                ? string.Empty
                : candidate.SourceTable.ToString();

            string inputType = candidate == null
                ? string.Empty
                : candidate.MaterialAInputType.ToString();

            string slotNo = candidate == null
                ? string.Empty
                : candidate.SlotNo.ToString();

            string parentLot = candidate == null || string.IsNullOrWhiteSpace(candidate.ParentLotNumber)
                ? string.Empty
                : candidate.ParentLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                "EDGE",
                parentMergeKey,
                childMergeKey,
                sourceTable,
                inputType,
                slotNo,
                parentLot);
        }

        private int CompareForwardStartNodesForSameLot(
    ProductionResultNode x,
    ProductionResultNode y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            // StartDateあり優先
            bool xHasDate = x.StartDate.HasValue;
            bool yHasDate = y.StartDate.HasValue;

            if (xHasDate && !yHasDate)
                return -1;

            if (!xHasDate && yHasDate)
                return 1;

            // StartDate昇順
            if (xHasDate && yHasDate)
            {
                int dateCompare = DateTime.Compare(x.StartDate.Value, y.StartDate.Value);
                if (dateCompare != 0)
                    return dateCompare;
            }

            // MasterKey安定順
            string xKey = string.IsNullOrWhiteSpace(x.ControlMasterKey) ? "" : x.ControlMasterKey.Trim();
            string yKey = string.IsNullOrWhiteSpace(y.ControlMasterKey) ? "" : y.ControlMasterKey.Trim();

            int keyCompare = string.Compare(xKey, yKey, StringComparison.OrdinalIgnoreCase);
            if (keyCompare != 0)
                return keyCompare;

            // Lot安定順
            string xLot = string.IsNullOrWhiteSpace(x.LotNumber) ? "" : x.LotNumber.Trim();
            string yLot = string.IsNullOrWhiteSpace(y.LotNumber) ? "" : y.LotNumber.Trim();

            return string.Compare(xLot, yLot, StringComparison.OrdinalIgnoreCase);
        }

        private string BuildEdgeExpandKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ChildCandidate candidate)
        {
            string parentMergeKey = GetNodeMergeKey(parentNode);
            string childMergeKey = GetNodeMergeKey(childNode);

            string sourceTable = candidate == null
                ? ""
                : candidate.SourceTable.ToString();

            string inputType = candidate == null
                ? ""
                : candidate.MaterialAInputType.ToString();

            string slotNo = candidate == null
                ? ""
                : candidate.SlotNo.ToString();

            string parentLot = candidate == null || string.IsNullOrWhiteSpace(candidate.ParentLotNumber)
                ? ""
                : candidate.ParentLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                "EXPAND_EDGE",
                parentMergeKey ?? "",
                childMergeKey ?? "",
                sourceTable,
                inputType,
                slotNo,
                parentLot);
        }



        
        

        private string NormalizeLotForStartFilter(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                return null;

            return lotNumber.Trim();
        }
        private string GetPathNodeKey(ProductionResultNode node)
        {
            if (node == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(node.ControlMasterKey))
                return node.ControlMasterKey;

            if (!string.IsNullOrWhiteSpace(node.LotNumber))
                return "LOT|" + node.LotNumber;

            return string.Empty;
        }

        

        

        

        

        

        

        

        

        

        private DataTable BuildDisplayTableFromDisplayResult(TraceDisplayResult displayResult)
        {
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
                AddDisplayTableMiddleColumns(table, level);
            }

            AddDisplayTableEndColumns(table);

            if (displayResult.Rows == null || displayResult.Rows.Count == 0)
                return table;

            for (int rowIndex = 0; rowIndex < displayResult.Rows.Count; rowIndex++)
            {
                var displayRow = displayResult.Rows[rowIndex];
                if (displayRow == null)
                    continue;

                var dr = table.NewRow();

                FillDisplayTableCommonColumns(dr, displayRow, rowIndex);
                FillDisplayTableStartColumns(dr, displayRow.Start);

                for (int level = 1; level <= maxMiddleDepth; level++)
                {
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

            table.Columns.Add("RootGroupKey", typeof(string));
            table.Columns.Add("RootNodeKey", typeof(string));
            table.Columns.Add("PathKey", typeof(string));
            table.Columns.Add("RouteSystem", typeof(string));

            table.Columns.Add("StartTrunkGroupKey", typeof(string));
            table.Columns.Add("StartTrunkOrder", typeof(int));
            table.Columns.Add("StartRowOrderInGroup", typeof(int));
            table.Columns.Add("IsFirstRowOfStartGroup", typeof(bool));
            table.Columns.Add("IsLastRowOfStartGroup", typeof(bool));

            table.Columns.Add("IsFirstRowOfRootGroup", typeof(bool));
            table.Columns.Add("IsLastRowOfRootGroup", typeof(bool));

            table.Columns.Add("FullPathSignature", typeof(string));
            table.Columns.Add("LotTraceKey", typeof(string));
            table.Columns.Add("UpstreamLotOnlyKey", typeof(string));

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
            table.Columns.Add("Start_ParentKey", typeof(string));
            table.Columns.Add("Start_DisplayParentNodeKey", typeof(string));
            table.Columns.Add("Start_IncomingLinkKey", typeof(string));
            table.Columns.Add("Start_UpstreamPathKey", typeof(string));
            table.Columns.Add("Start_DownstreamPathKey", typeof(string));

            table.Columns.Add("Start_Order", typeof(string));
            table.Columns.Add("Start_Lot", typeof(string));
            table.Columns.Add("Start_ItemCode", typeof(string));
            table.Columns.Add("Start_ItemName", typeof(string));
            table.Columns.Add("Start_StartTime", typeof(DateTime));
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
            table.Columns.Add(prefix + "ParentKey", typeof(string));
            table.Columns.Add(prefix + "DisplayParentNodeKey", typeof(string));
            table.Columns.Add(prefix + "IncomingLinkKey", typeof(string));
            table.Columns.Add(prefix + "UpstreamPathKey", typeof(string));
            table.Columns.Add(prefix + "DownstreamPathKey", typeof(string));

            table.Columns.Add(prefix + "Order", typeof(string));
            table.Columns.Add(prefix + "Lot", typeof(string));
            table.Columns.Add(prefix + "ItemCode", typeof(string));
            table.Columns.Add(prefix + "ItemName", typeof(string));
            table.Columns.Add(prefix + "StartTime", typeof(DateTime));
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
            table.Columns.Add("End_ParentKey", typeof(string));
            table.Columns.Add("End_DisplayParentNodeKey", typeof(string));
            table.Columns.Add("End_IncomingLinkKey", typeof(string));
            table.Columns.Add("End_UpstreamPathKey", typeof(string));
            table.Columns.Add("End_DownstreamPathKey", typeof(string));

            table.Columns.Add("End_Order", typeof(string));
            table.Columns.Add("End_Lot", typeof(string));
            table.Columns.Add("End_ItemCode", typeof(string));
            table.Columns.Add("End_ItemName", typeof(string));
            table.Columns.Add("End_StartTime", typeof(DateTime));
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

            SetValue(row, "RootGroupKey", displayRow.RootGroupKey);
            SetValue(row, "RootNodeKey", displayRow.RootNodeKey);
            SetValue(row, "PathKey", displayRow.PathKey);
            SetValue(row, "RouteSystem", displayRow.RouteSystem);

            SetValue(row, "StartTrunkGroupKey", displayRow.StartTrunkGroupKey);
            row["StartTrunkOrder"] = displayRow.StartTrunkOrder;
            row["StartRowOrderInGroup"] = displayRow.StartRowOrderInGroup;
            row["IsFirstRowOfStartGroup"] = displayRow.IsFirstRowOfStartGroup;
            row["IsLastRowOfStartGroup"] = displayRow.IsLastRowOfStartGroup;

            row["IsFirstRowOfRootGroup"] = displayRow.IsFirstRowOfRootGroup;
            row["IsLastRowOfRootGroup"] = displayRow.IsLastRowOfRootGroup;

            SetValue(row, "FullPathSignature", displayRow.FullPathSignature);
            SetValue(row, "LotTraceKey", displayRow.LotTraceKey);
            SetValue(row, "UpstreamLotOnlyKey", displayRow.UpstreamLotOnlyKey);

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
            SetValue(row, prefix + "ParentKey", cell.ParentKey);
            SetValue(row, prefix + "DisplayParentNodeKey", cell.DisplayParentNodeKey);
            SetValue(row, prefix + "IncomingLinkKey", cell.IncomingLinkKey);
            SetValue(row, prefix + "UpstreamPathKey", cell.UpstreamPathKey);
            SetValue(row, prefix + "DownstreamPathKey", cell.DownstreamPathKey);

            SetValue(row, prefix + "Order", cell.ProductionOrderNumber);
            SetValue(row, prefix + "Lot", cell.LotNumber);
            SetValue(row, prefix + "ItemCode", cell.ItemCode);
            SetValue(row, prefix + "ItemName", cell.ItemName);

            if (cell.StartDate.HasValue)
            {
                row[prefix + "StartTime"] = cell.StartDate.Value;
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

        





        private void PopulateRenderRanges(TraceDisplayResult displayResult)
        {
            if (displayResult == null)
                return;

            displayResult.NodeRenderRanges.Clear();
            displayResult.MiddleTreeRenderRanges.Clear();

            if (displayResult.Rows == null || displayResult.Rows.Count == 0)
                return;

            var visibleRows = displayResult.Rows
                .Where(r => r != null && r.IsDisplayTarget)
                .ToList();

            if (visibleRows.Count == 0)
                return;

            // ★ ここでは再並び替えしない
            // displayResult.Rows は suppress 後・最終順確定済み
            var nodeRanges = BuildNodeRenderRangesForDisplayRows(visibleRows);
            foreach (var pair in nodeRanges)
            {
                displayResult.NodeRenderRanges[pair.Key] = pair.Value;
            }

            var middleRanges = BuildMiddleTreeRenderRangesForDisplayRows(
                visibleRows,
                displayResult.MaxMiddleDepth);

            foreach (var pair in middleRanges)
            {
                displayResult.MiddleTreeRenderRanges[pair.Key] = pair.Value;
            }
        }

        private List<TraceDisplayRow> SortDisplayRowsForFinalDisplayOrder(
    IList<TraceDisplayRow> rows)
        {
            var result = new List<TraceDisplayRow>();

            if (rows == null || rows.Count == 0)
                return result;

            var groupBuckets = new Dictionary<string, List<TraceDisplayRow>>(StringComparer.OrdinalIgnoreCase);
            var groupOrder = new List<string>();
            var groupSortOrders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var groupFirstDisplayOrders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                if (row == null)
                    continue;

                string groupKey = GetFinalDisplayGroupKey(row);
                if (string.IsNullOrWhiteSpace(groupKey))
                {
                    groupKey = "__ROW__|" + row.DisplayOrder.ToString();
                }

                List<TraceDisplayRow> bucket;
                if (!groupBuckets.TryGetValue(groupKey, out bucket))
                {
                    bucket = new List<TraceDisplayRow>();
                    groupBuckets[groupKey] = bucket;
                    groupOrder.Add(groupKey);

                    groupSortOrders[groupKey] = GetFinalDisplayGroupOrder(row);
                    groupFirstDisplayOrders[groupKey] = row.DisplayOrder;
                }

                bucket.Add(row);

                if (row.DisplayOrder < groupFirstDisplayOrders[groupKey])
                {
                    groupFirstDisplayOrders[groupKey] = row.DisplayOrder;
                }

                int order = GetFinalDisplayGroupOrder(row);
                if (order < groupSortOrders[groupKey])
                {
                    groupSortOrders[groupKey] = order;
                }
            }

            var orderedGroupKeys = groupOrder
                .OrderBy(k => groupSortOrders.ContainsKey(k) ? groupSortOrders[k] : int.MaxValue)
                .ThenBy(k => groupFirstDisplayOrders.ContainsKey(k) ? groupFirstDisplayOrders[k] : int.MaxValue)
                .ThenBy(k => k, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (string groupKey in orderedGroupKeys)
            {
                List<TraceDisplayRow> bucket;
                if (!groupBuckets.TryGetValue(groupKey, out bucket) || bucket == null || bucket.Count == 0)
                    continue;

                var normalRows = bucket
                    .Where(r => !HasNoMiddleNodesForRender(r))
                    .OrderBy(r => GetFinalDisplayRowOrderInGroup(r))
                    .ThenBy(r => NormalizePathKeyForFinalDisplay(r), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(r => r.DisplayOrder)
                    .ToList();

                var depthZeroRows = bucket
                    .Where(r => HasNoMiddleNodesForRender(r))
                    .OrderBy(r => GetFinalDisplayRowOrderInGroup(r))
                    .ThenBy(r => NormalizePathKeyForFinalDisplay(r), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(r => r.DisplayOrder)
                    .ToList();

                result.AddRange(normalRows);
                result.AddRange(depthZeroRows);
            }

            return result;
        }

        private string GetFinalDisplayGroupKey(TraceDisplayRow row)
        {
            if (row == null)
                return null;

            if (!string.IsNullOrWhiteSpace(row.StartTrunkGroupKey))
            {
                return "TRUNK|" + row.StartTrunkGroupKey.Trim();
            }

            if (!string.IsNullOrWhiteSpace(row.RootGroupKey))
            {
                return "ROOT|" + row.RootGroupKey.Trim();
            }

            return null;
        }

        private int GetFinalDisplayGroupOrder(TraceDisplayRow row)
        {
            if (row == null)
                return int.MaxValue;

            if (!string.IsNullOrWhiteSpace(row.StartTrunkGroupKey))
            {
                return row.StartTrunkOrder;
            }

            // フォワード幹情報を持たない行は元順維持を優先
            return row.DisplayOrder;
        }

        private int GetFinalDisplayRowOrderInGroup(TraceDisplayRow row)
        {
            if (row == null)
                return int.MaxValue;

            // 0 は正当な先頭順序なので採用する
            if (row.StartRowOrderInGroup >= 0)
                return row.StartRowOrderInGroup;

            return row.DisplayOrder;
        }

        private string NormalizePathKeyForFinalDisplay(TraceDisplayRow row)
        {
            if (row == null || string.IsNullOrWhiteSpace(row.PathKey))
                return string.Empty;

            return row.PathKey.Trim();
        }

        private Dictionary<string, NodeRenderRange> BuildNodeRenderRangesForDisplayRows(
    IList<TraceDisplayRow> rows)
        {
            var result = new Dictionary<string, NodeRenderRange>(StringComparer.OrdinalIgnoreCase);

            if (rows == null || rows.Count == 0)
                return result;

            // まず各Node自身の最初の表示行を拾う
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];

                foreach (var cell in EnumerateDisplayCells(row))
                {
                    if (cell == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(cell.NodeKey))
                        continue;

                    if (!result.ContainsKey(cell.NodeKey))
                    {
                        result[cell.NodeKey] = new NodeRenderRange
                        {
                            NodeKey = cell.NodeKey,
                            RowIndex = rowIndex
                        };
                    }
                }
            }

            // 次に親 -> 子 の表示行を集計
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];

                foreach (var cell in EnumerateDisplayCells(row))
                {
                    if (cell == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(cell.NodeKey))
                        continue;

                    if (string.IsNullOrWhiteSpace(cell.DisplayParentNodeKey))
                        continue;

                    NodeRenderRange parentRange;
                    if (!result.TryGetValue(cell.DisplayParentNodeKey, out parentRange))
                        continue;

                    if (!parentRange.ChildRowIndices.Contains(rowIndex))
                    {
                        parentRange.ChildRowIndices.Add(rowIndex);
                    }
                }
            }

            return result;
        }

        private Dictionary<string, MiddleTreeRenderRange> BuildMiddleTreeRenderRangesForDisplayRows(
    IList<TraceDisplayRow> rows,
    int maxDepth)
        {
            var result = new Dictionary<string, MiddleTreeRenderRange>(StringComparer.OrdinalIgnoreCase);

            if (rows == null || rows.Count == 0)
                return result;

            if (maxDepth <= 0)
                return result;

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                if (row == null)
                    continue;

                string rootGroupKey = row.RootGroupKey;

                for (int level = 1; level <= maxDepth; level++)
                {
                    TraceDisplayCell middleCell = GetMiddleCell(row, level);
                    if (middleCell == null)
                        continue;

                    string nodeKey = middleCell.NodeKey;
                    if (string.IsNullOrWhiteSpace(nodeKey))
                        continue;

                    string displayParentNodeKey = middleCell.DisplayParentNodeKey;
                    string incomingLinkKey = middleCell.IncomingLinkKey;
                    string upstreamPathKey = middleCell.UpstreamPathKey;
                    string downstreamPathKey = middleCell.DownstreamPathKey;

                    string rangeKey = BuildMiddleTreeRenderRangeKey(
                        rootGroupKey,
                        level,
                        nodeKey,
                        displayParentNodeKey,
                        incomingLinkKey,
                        upstreamPathKey,
                        downstreamPathKey);

                    MiddleTreeRenderRange range;
                    if (!result.TryGetValue(rangeKey, out range))
                    {
                        range = new MiddleTreeRenderRange
                        {
                            GroupKey = rootGroupKey,
                            Level = level,
                            NodeKey = nodeKey,
                            DisplayParentNodeKey = displayParentNodeKey,
                            IncomingLinkKey = incomingLinkKey,
                            UpstreamPathKey = upstreamPathKey,
                            DownstreamPathKey = downstreamPathKey,
                            StartRowIndex = rowIndex,
                            EndRowIndex = rowIndex
                        };

                        result[rangeKey] = range;
                    }

                    if (rowIndex < range.StartRowIndex)
                    {
                        range.StartRowIndex = rowIndex;
                    }

                    if (rowIndex > range.EndRowIndex)
                    {
                        range.EndRowIndex = rowIndex;
                    }
                }
            }

            return result;
        }

        private string BuildMiddleTreeRenderRangeKey(
    string rootGroupKey,
    int level,
    string nodeKey,
    string displayParentNodeKey,
    string incomingLinkKey,
    string upstreamPathKey,
    string downstreamPathKey)
        {
            return string.Join("|",
                rootGroupKey ?? string.Empty,
                level.ToString(),
                nodeKey ?? string.Empty,
                displayParentNodeKey ?? string.Empty,
                incomingLinkKey ?? string.Empty,
                upstreamPathKey ?? string.Empty,
                downstreamPathKey ?? string.Empty);
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

        private IEnumerable<TraceDisplayCell> EnumerateDisplayCells(TraceDisplayRow row)
        {
            if (row == null)
                yield break;

            if (row.Start != null)
                yield return row.Start;

            if (row.Middles != null)
            {
                foreach (var middle in row.Middles)
                {
                    if (middle != null)
                        yield return middle;
                }
            }

            if (row.End != null)
                yield return row.End;
        }

        private List<TraceDisplayRow> ReorderDisplayRowsKeepTreeAndMoveDepthZeroLastForRender(
    IList<TraceDisplayRow> rows)
        {
            var result = new List<TraceDisplayRow>();

            if (rows == null || rows.Count == 0)
                return result;

            var blocks = rows
                .Where(r => r != null)
                .GroupBy(r => GetReorderBlockKeyForRender(r))
                .ToList();

            foreach (var block in blocks)
            {
                var blockRows = block.ToList();

                var normalRows = blockRows
                    .Where(r => !HasNoMiddleNodesForRender(r))
                    .OrderBy(r => r.DisplayOrder)
                    .ToList();

                var depthZeroRows = blockRows
                    .Where(r => HasNoMiddleNodesForRender(r))
                    .OrderBy(r => r.DisplayOrder)
                    .ToList();

                foreach (var row in normalRows)
                {
                    result.Add(row);
                }

                foreach (var row in depthZeroRows)
                {
                    result.Add(row);
                }
            }

            return result;
        }

        private string GetReorderBlockKeyForRender(TraceDisplayRow row)
        {
            if (row == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(row.StartTrunkGroupKey))
                return "TRUNK|" + row.StartTrunkGroupKey.Trim();

            return "ROOT|" + (row.RootGroupKey ?? string.Empty).Trim();
        }

        private bool HasNoMiddleNodesForRender(TraceDisplayRow row)
        {
            if (row == null)
                return false;

            return GetMiddleDepthForRender(row) <= 0;
        }

        private int GetMiddleDepthForRender(TraceDisplayRow row)
        {
            if (row == null || row.Middles == null || row.Middles.Count == 0)
                return 0;

            int depth = 0;

            for (int i = 0; i < row.Middles.Count; i++)
            {
                var middle = row.Middles[i];
                if (middle == null)
                    continue;

                if (string.IsNullOrWhiteSpace(middle.NodeKey))
                    continue;

                depth = i + 1;
            }

            return depth;
        }

        

        

        

        

        private void AppendNodeAndLot(List<string> parts, ProductionResultNode node, bool isEnd = false)
        {
            if (parts == null || node == null)
                return;

            // ------------------------------------------------------------
            // 1) Node表現
            // ------------------------------------------------------------
            string nodePart;
            if (IsBSystemNode(node))
            {
                // B系統：Node ID = MasterKey
                nodePart = "N|B|" + Safe(node.ControlMasterKey);
            }
            else
            {
                // A系統：Node ID = MasterKey + SlotNo
                int slotNo = GetASlotNumber(node);
                string slotPart = slotNo > 0 ? slotNo.ToString() : "?";

                nodePart = "N|A|" + Safe(node.ControlMasterKey) + "|S" + slotPart;
            }

            parts.Add(nodePart);

            // ------------------------------------------------------------
            // 2) Lot接続表現
            // ------------------------------------------------------------
            string lot = NormalizeLot(node.LotNumber);
            if (!string.IsNullOrEmpty(lot))
            {
                parts.Add("L|" + lot);
                return;
            }

            // ------------------------------------------------------------
            // 3) Lotなし表現
            // ------------------------------------------------------------
            if (node.IsTraceTerminal)
            {
                parts.Add(isEnd ? "END|NO_LOT|T" : "NO_LOT|T");
                return;
            }

            if (isEnd)
            {
                parts.Add("END|NO_LOT");
            }
        }

        private string NormalizeLot(string lot)
        {
            if (string.IsNullOrWhiteSpace(lot))
                return null;

            return lot.Trim();
        }

        private string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "?" : value.Trim();
        }

        private bool IsBSystemNode(ProductionResultNode node)
        {
            if (node == null)
                return false;

            return string.Equals(node.RouteSystem, "B", StringComparison.OrdinalIgnoreCase);
        }

        private int GetASlotNumber(ProductionResultNode node)
        {
            if (node == null)
                return 0;

            return node.InputSlotNo ?? 0;
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
                NodeKey = BuildNodeOnlyKey(node),

                // 旧互換
                ParentKey = GetParentKeyFromPathRow(pathRow, nodeIndexInPath),
                ParentMasterKey = node.ParentMasterKey,

                // 線描画・経路識別
                DisplayParentNodeKey = GetDisplayParentNodeKeyFromPathRow(pathRow, nodeIndexInPath),
                IncomingLinkKey = GetIncomingLinkKeyFromPathRow(pathRow, nodeIndexInPath),
                UpstreamPathKey = GetUpstreamPathKeyFromPathRow(pathRow, nodeIndexInPath),
                DownstreamPathKey = GetDownstreamPathKeyFromPathRow(pathRow, nodeIndexInPath),

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

        private void MarkStartGroupBoundaries(
    List<TraceDisplayRow> rows,
    ForwardDisplayLaneBuildResult laneBuildResult)
        {
            if (rows == null || rows.Count == 0)
                return;

            // 初期化
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row == null)
                    continue;

                row.IsFirstRowOfStartGroup = false;
                row.IsLastRowOfStartGroup = false;
            }

            // 例外配置:
            // UI契約の IsFirst/IsLastRowOfStartGroup を埋めるため、
            // BuildDisplayResult から呼ばれる。
            // ただし判定根拠は rows の後段推測ではなく、
            // フォワード枝構造確定済み occupied の Lv0 のみを使用する。
            if (laneBuildResult == null ||
                laneBuildResult.OccupiedLotGroupRanges == null ||
                laneBuildResult.OccupiedLotGroupRanges.Count == 0)
            {
                return;
            }

            foreach (var group in laneBuildResult.OccupiedLotGroupRanges)
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
                        firstRow.IsFirstRowOfStartGroup = true;
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



        private int FindLastRowIndexOfStartGroup(
            List<TraceDisplayRow> rows,
            int firstRowIndex,
            string startTrunkGroupKey,
            List<OccupiedLotGroupRange> lv1OccupiedRanges)
        {
            int seedLastRowIndex = firstRowIndex;

            for (int i = firstRowIndex + 1; i < rows.Count; i++)
            {
                var row = rows[i];
                string key = row == null ? string.Empty : (row.StartTrunkGroupKey ?? string.Empty);

                if (!string.Equals(key, startTrunkGroupKey ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                    break;

                seedLastRowIndex = i;
            }

            int occupiedLastRowIndex = seedLastRowIndex;

            if (lv1OccupiedRanges != null && lv1OccupiedRanges.Count > 0)
            {
                foreach (var range in lv1OccupiedRanges)
                {
                    if (range == null)
                        continue;

                    if (range.Level != 1)
                        continue;

                    // 始点グループ起点 row から始まる Lv1 occupied を採用
                    if (range.OccupiedFirstY < firstRowIndex)
                        continue;

                    if (range.OccupiedFirstY > seedLastRowIndex)
                        continue;

                    if (range.OccupiedLastY > occupiedLastRowIndex)
                        occupiedLastRowIndex = range.OccupiedLastY;
                }
            }

            return occupiedLastRowIndex;
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

        

        

        

        

        

        

        

        private void MarkRootGroupBoundaries(List<TraceDisplayRow> rows)
        {
            if (rows == null || rows.Count == 0)
                return;

            for (int i = 0; i < rows.Count; i++)
            {
                var current = rows[i];
                var prev = i > 0 ? rows[i - 1] : null;
                var next = i < rows.Count - 1 ? rows[i + 1] : null;

                current.IsFirstRowOfRootGroup = prev == null ||
                    !string.Equals(prev.RootGroupKey ?? string.Empty, current.RootGroupKey ?? string.Empty, StringComparison.OrdinalIgnoreCase);

                current.IsLastRowOfRootGroup = next == null ||
                    !string.Equals(next.RootGroupKey ?? string.Empty, current.RootGroupKey ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }

        

        private sealed class NodeDeletionCandidate
        {
            public TracePathRow Row { get; set; }
            public int Level { get; set; } // Start=0, Middle=1..N
            public ProductionResultNode Node { get; set; }
            public string LotKey { get; set; }
            public string EquivalenceKey { get; set; }
            public string RepresentativeSortKey { get; set; }
        }

        

        

        private string BuildStartImmediateChildConnectionSignature(TracePathRow row)
        {
            if (row == null)
                return null;

            var link = GetAdjacentLinkFromCurrentLevel(row, 0);
            if (link == null)
            {
                // ------------------------------------------------------------
                // NO_CHILD は固定値にせず、
                // StartNode の MergeKey を付与して返す
                //
                // 理由:
                //   同一Lot の NO_CHILD 始点でも、
                //   Node 自体が異なるものまで同値判定で
                //   まとめてしまわないため
                // ------------------------------------------------------------
                string startMergeKey = row.StartNode == null
                    ? string.Empty
                    : (GetNodeMergeKey(row.StartNode) ?? string.Empty);

                return "NO_CHILD|NODE:" + startMergeKey;
            }

            string childNodeKey = link.ChildNode == null
                ? string.Empty
                : (GetNodeMergeKey(link.ChildNode) ?? string.Empty);

            string linkKey = GetPathLinkKey(link) ?? string.Empty;

            return "CHILD:" + childNodeKey + "|LINK:" + linkKey;
        }


        private int GetAdjacentLinkIndexForCurrentLevel(int currentLevel)
        {
            // Start = Lv0
            // Lv0 -> Lv1 は PathLinks[0]
            // Lv1 -> Lv2 は PathLinks[1]
            // Lvn -> Lv(n+1) は PathLinks[n]
            return currentLevel;
        }

        private ProductionResultLink GetAdjacentLinkFromCurrentLevel(TracePathRow row, int currentLevel)
        {
            if (row == null || row.PathLinks == null || row.PathLinks.Count == 0)
                return null;

            int linkIndex = GetAdjacentLinkIndexForCurrentLevel(currentLevel);
            if (linkIndex < 0 || linkIndex >= row.PathLinks.Count)
                return null;

            return row.PathLinks[linkIndex];
        }
        private string BuildImmediateChildConnectionSignature(TracePathRow row, int level)
        {
            if (row == null)
                return null;

            var link = GetAdjacentLinkFromCurrentLevel(row, level);
            if (link == null)
                return "NO_CHILD";

            string childNodeKey = link.ChildNode == null
                ? string.Empty
                : (GetNodeMergeKey(link.ChildNode) ?? string.Empty);

            string linkKey = GetPathLinkKey(link) ?? string.Empty;

            return "CHILD:" + childNodeKey + "|LINK:" + linkKey;
        }



        private void DeleteStartNodeOnly(TracePathRow row, string reason)
        {
            if (row == null || row.StartNode == null)
                return;

            row.StartNode = null;
            row.IsPruned = true;
            AppendNodeDeletionReason(row, reason);
        }

        private void DeleteMiddleNodeOnlyAtLevel(TracePathRow row, int level, string reason)
        {
            if (row == null || row.MiddleNodes == null)
                return;

            int index = level - 1;
            if (index < 0 || index >= row.MiddleNodes.Count)
                return;

            if (row.MiddleNodes[index] == null)
                return;

            row.MiddleNodes[index] = null;
            row.IsPruned = true;
            AppendNodeDeletionReason(row, reason);
        }

        private ProductionResultNode GetMiddleNodeAtLevelOrNull(TracePathRow row, int level)
        {
            if (row == null || row.MiddleNodes == null)
                return null;

            int index = level - 1;
            if (index < 0 || index >= row.MiddleNodes.Count)
                return null;

            return row.MiddleNodes[index];
        }

        private void AppendNodeDeletionReason(TracePathRow row, string reason)
        {
            if (row == null || string.IsNullOrWhiteSpace(reason))
                return;

            if (string.IsNullOrWhiteSpace(row.PruneReason))
            {
                row.PruneReason = reason;
                return;
            }

            var parts = row.PruneReason
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x == null ? null : x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (!parts.Any(x => string.Equals(x, reason, StringComparison.OrdinalIgnoreCase)))
            {
                parts.Add(reason);
            }

            row.PruneReason = string.Join(";", parts);
        }



        private void SortForwardPathRowsWeak(List<TracePathRow> rows)
        {
            if (rows == null || rows.Count <= 1)
                return;

            // 1. NODE実体ありを上（マージソート）
            var step1 = MergeSortByStartExists(rows);

            rows.Clear();
            rows.AddRange(step1);

            // 2. StartNode の LotNo でバブルソート
            BubbleSortByStartTrunkGroupKey(rows);
        }

        private void BubbleSortByStartTrunkGroupKey(List<TracePathRow> rows)
        {
            if (rows == null || rows.Count <= 1)
                return;

            int n = rows.Count;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    string keyA = rows[j] == null
                        ? string.Empty
                        : (rows[j].StartTrunkGroupKey ?? string.Empty);

                    string keyB = rows[j + 1] == null
                        ? string.Empty
                        : (rows[j + 1].StartTrunkGroupKey ?? string.Empty);

                    if (string.Compare(keyA, keyB, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var temp = rows[j];
                        rows[j] = rows[j + 1];
                        rows[j + 1] = temp;
                    }
                }
            }
        }


        private string GetStartLotSortValue(TracePathRow row)
        {
            if (row == null || row.StartNode == null || string.IsNullOrWhiteSpace(row.StartNode.LotNumber))
                return string.Empty;

            return row.StartNode.LotNumber.Trim();
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

        private void BubbleSortByRootGroupKey(List<TracePathRow> rows)
        {
            if (rows == null || rows.Count <= 1)
                return;

            int n = rows.Count;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    string keyA = rows[j] == null ? string.Empty : (rows[j].RootGroupKey ?? string.Empty);
                    string keyB = rows[j + 1] == null ? string.Empty : (rows[j + 1].RootGroupKey ?? string.Empty);

                    if (string.Compare(keyA, keyB, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var temp = rows[j];
                        rows[j] = rows[j + 1];
                        rows[j + 1] = temp;
                    }
                }
            }
        }

        private void ExportForwardStructureTableBeforeDeletionCsv(List<TracePathRow> rows)
        {
            ExportForwardStructureTableCsv("LotTrace_Debug_TreeStructure_BeforeDeletion.csv", rows);
        }

        private void ExportForwardStructureTableAfterDeletionCsv(List<TracePathRow> rows)
        {
            ExportForwardDisplayLikeTableCsv("LotTrace_Debug_TreeStructure_AfterDeletion.csv", rows);
        }

        private void ExportForwardDisplayLikeTableCsv(string fileName, List<TracePathRow> rows)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                int maxLevel = 0;
                if (rows != null && rows.Count > 0)
                {
                    maxLevel = rows.Max(r => r == null || r.MiddleNodes == null ? 0 : r.MiddleNodes.Count);
                }

                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(BuildForwardDisplayLikeTableHeader(maxLevel));

                    if (rows == null || rows.Count == 0)
                        return;

                    foreach (var row in rows.OrderBy(r => r == null ? int.MaxValue : r.PathOrder))
                    {
                        sw.WriteLine(BuildForwardDisplayLikeTableRow(row, maxLevel));
                    }
                }
            }
            catch
            {
            }
        }

        private string BuildForwardDisplayLikeTableHeader(int maxLevel)
        {
            var headers = new List<string>();

            headers.Add("PathOrder");
            headers.Add("RootGroupKey");
            headers.Add("StartTrunkGroupKey");
            headers.Add("StartTrunkOrder");
            headers.Add("FullPathSignature");
            headers.Add("IsPruned");
            headers.Add("PruneReason");
            headers.Add("RouteSystem");
            headers.Add("PathKey");
            headers.Add("LotTraceKey");

            headers.Add("Start_Exists");
            headers.Add("Start_NodeMergeKey");
            headers.Add("Start_LotNo");

            for (int level = 1; level <= maxLevel; level++)
            {
                headers.Add("Lv" + level + "_Exists");
                headers.Add("Lv" + level + "_NodeMergeKey");
                headers.Add("Lv" + level + "_LotNo");
            }

            headers.Add("End_Exists");
            headers.Add("End_NodeMergeKey");
            headers.Add("End_LotNo");

            return string.Join(",", headers.Select(EscapeCsv));
        }

        private string BuildForwardDisplayLikeTableRow(TracePathRow row, int maxLevel)
        {
            var values = new List<string>();

            values.Add(EscapeCsv(row == null ? null : row.PathOrder.ToString()));
            values.Add(EscapeCsv(row == null ? null : row.RootGroupKey));
            values.Add(EscapeCsv(row == null ? null : row.StartTrunkGroupKey));
            values.Add(EscapeCsv(row == null ? null : row.StartTrunkOrder.ToString()));
            values.Add(EscapeCsv(row == null ? null : row.FullPathSignature));
            values.Add(EscapeCsv(row == null ? null : row.IsPruned.ToString()));
            values.Add(EscapeCsv(row == null ? null : row.PruneReason));
            values.Add(EscapeCsv(row == null ? null : ResolveRouteSystem(row)));
            values.Add(EscapeCsv(row == null ? null : BuildPathKey(row)));
            values.Add(EscapeCsv(row == null ? null : BuildLotTraceKey(row)));

            if (row == null)
            {
                values.Add(EscapeCsv("False"));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
            }
            else
            {
                values.Add(EscapeCsv((row.StartNode != null).ToString()));
                values.Add(EscapeCsv(row.StartNode == null ? null : GetNodeMergeKey(row.StartNode)));
                values.Add(EscapeCsv(row.StartNode == null ? null : NormalizeLotForGrouping(row.StartNode.LotNumber)));
            }

            for (int level = 1; level <= maxLevel; level++)
            {
                AppendForwardDisplayLikeNodeColumns(values, row, level);
            }

            if (row == null)
            {
                values.Add(EscapeCsv("False"));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
            }
            else
            {
                values.Add(EscapeCsv((row.EndNode != null).ToString()));
                values.Add(EscapeCsv(row.EndNode == null ? null : GetNodeMergeKey(row.EndNode)));
                values.Add(EscapeCsv(row.EndNode == null ? null : NormalizeLotForGrouping(row.EndNode.LotNumber)));
            }

            return string.Join(",", values);
        }

        private void AppendForwardDisplayLikeNodeColumns(List<string> values, TracePathRow row, int level)
        {
            if (values == null)
                return;

            if (row == null)
            {
                values.Add(EscapeCsv("False"));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                return;
            }

            var node = GetMiddleNodeAtLevelOrNull(row, level);

            values.Add(EscapeCsv((node != null).ToString()));
            values.Add(EscapeCsv(node == null ? null : GetNodeMergeKey(node)));
            values.Add(EscapeCsv(node == null ? null : NormalizeLotForGrouping(node.LotNumber)));
        }

        private void ExportForwardStructureTableCsv(string fileName, List<TracePathRow> rows)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                int maxLevel = 0;
                if (rows != null && rows.Count > 0)
                {
                    maxLevel = rows.Max(r => r == null || r.MiddleNodes == null ? 0 : r.MiddleNodes.Count);
                }

                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(BuildForwardStructureTableHeader(maxLevel));

                    if (rows == null || rows.Count == 0)
                        return;

                    foreach (var row in rows.OrderBy(r => r == null ? int.MaxValue : r.PathOrder))
                    {
                        sw.WriteLine(BuildForwardStructureTableRow(row, maxLevel));
                    }
                }
            }
            catch
            {
            }
        }

        private string BuildForwardStructureTableHeader(int maxLevel)
        {
            var headers = new List<string>();

            headers.Add("PathOrder");
            headers.Add("RootGroupKey");
            headers.Add("StartTrunkGroupKey");
            headers.Add("StartTrunkOrder");
            headers.Add("FullPathSignature");
            headers.Add("IsPruned");
            headers.Add("PruneReason");

            headers.Add("Start_Exists");
            headers.Add("Start_NodeMergeKey");
            headers.Add("Start_LotNo");
            headers.Add("Start_LinkIndex");
            headers.Add("Start_LinkParentMergeKey");
            headers.Add("Start_LinkChildMergeKey");
            headers.Add("Start_LinkKey");
            headers.Add("Start_ImmediateChildConnectionSignature");

            for (int level = 1; level <= maxLevel; level++)
            {
                headers.Add("Lv" + level + "_Exists");
                headers.Add("Lv" + level + "_NodeMergeKey");
                headers.Add("Lv" + level + "_LotNo");
                headers.Add("Lv" + level + "_DisplayParentNodeKey");
                headers.Add("Lv" + level + "_IncomingLinkKey");
                headers.Add("Lv" + level + "_LinkIndex");
                headers.Add("Lv" + level + "_LinkParentMergeKey");
                headers.Add("Lv" + level + "_LinkChildMergeKey");
                headers.Add("Lv" + level + "_LinkKey");
                headers.Add("Lv" + level + "_ImmediateChildConnectionSignature");
            }

            headers.Add("End_NodeMergeKey");
            headers.Add("End_LotNo");

            return string.Join(",", headers.Select(EscapeCsv));
        }

        private string BuildForwardStructureTableRow(TracePathRow row, int maxLevel)
        {
            var values = new List<string>();

            values.Add(EscapeCsv(row == null ? null : row.PathOrder.ToString()));
            values.Add(EscapeCsv(row == null ? null : row.RootGroupKey));
            values.Add(EscapeCsv(row == null ? null : row.StartTrunkGroupKey));
            values.Add(EscapeCsv(row == null ? null : row.StartTrunkOrder.ToString()));
            values.Add(EscapeCsv(row == null ? null : row.FullPathSignature));
            values.Add(EscapeCsv(row == null ? null : row.IsPruned.ToString()));
            values.Add(EscapeCsv(row == null ? null : row.PruneReason));

            if (row == null)
            {
                values.Add(EscapeCsv("False"));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
            }
            else
            {
                var startLink = GetAdjacentLinkFromCurrentLevel(row, 0);

                values.Add(EscapeCsv((row.StartNode != null).ToString()));
                values.Add(EscapeCsv(row.StartNode == null ? null : GetNodeMergeKey(row.StartNode)));
                values.Add(EscapeCsv(row.StartNode == null ? null : NormalizeLotForGrouping(row.StartNode.LotNumber)));
                values.Add(EscapeCsv(GetAdjacentLinkIndexForCurrentLevel(0).ToString()));
                values.Add(EscapeCsv(startLink == null || startLink.ParentNode == null ? null : GetNodeMergeKey(startLink.ParentNode)));
                values.Add(EscapeCsv(startLink == null || startLink.ChildNode == null ? null : GetNodeMergeKey(startLink.ChildNode)));
                values.Add(EscapeCsv(startLink == null ? null : GetPathLinkKey(startLink)));
                values.Add(EscapeCsv(BuildStartImmediateChildConnectionSignature(row)));
            }

            for (int level = 1; level <= maxLevel; level++)
            {
                AppendForwardStructureNodeColumns(values, row, level);
            }

            if (row == null)
            {
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
            }
            else
            {
                values.Add(EscapeCsv(row.EndNode == null ? null : GetNodeMergeKey(row.EndNode)));
                values.Add(EscapeCsv(row.EndNode == null ? null : NormalizeLotForGrouping(row.EndNode.LotNumber)));
            }

            return string.Join(",", values);
        }

        private void AppendForwardStructureNodeColumns(List<string> values, TracePathRow row, int level)
        {
            if (values == null)
                return;

            if (row == null)
            {
                values.Add(EscapeCsv("False"));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                values.Add(EscapeCsv(null));
                return;
            }

            var node = GetMiddleNodeAtLevelOrNull(row, level);
            var link = GetAdjacentLinkFromCurrentLevel(row, level);

            values.Add(EscapeCsv((node != null).ToString()));
            values.Add(EscapeCsv(node == null ? null : GetNodeMergeKey(node)));
            values.Add(EscapeCsv(node == null ? null : NormalizeLotForGrouping(node.LotNumber)));
            values.Add(EscapeCsv(GetDisplayParentNodeKeyFromPathRow(row, level)));
            values.Add(EscapeCsv(GetIncomingLinkKeyFromPathRow(row, level)));
            values.Add(EscapeCsv(GetAdjacentLinkIndexForCurrentLevel(level).ToString()));
            values.Add(EscapeCsv(link == null || link.ParentNode == null ? null : GetNodeMergeKey(link.ParentNode)));
            values.Add(EscapeCsv(link == null || link.ChildNode == null ? null : GetNodeMergeKey(link.ChildNode)));
            values.Add(EscapeCsv(link == null ? null : GetPathLinkKey(link)));
            values.Add(EscapeCsv(BuildImmediateChildConnectionSignature(row, level)));
        }


        

        

        

        

        private void AssignPathRowBaseMetadata(List<TracePathRow> rows)
        {
            if (rows == null)
                return;

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row == null)
                    continue;

                row.PathOrder = i;
                row.FullPathSignature = BuildFullPathSignature(row);
                row.StartBranchSignature = BuildDownstreamSignature(row, 0);
            }
        }

        

        

        private TracePathRow SelectRepresentativeRowForMiddleGroup(List<TracePathRow> rows, int level)
        {
            if (rows == null || rows.Count == 0)
                return null;

            // 代表行選定方針：
            // 1. 代表行キー（NodeMergeKey + 下流署名）
            // 2. 下流署名
            // 3. 全経路署名
            // 4. PathOrder
            //
            // ※ Middle の同一Lv・同一Lotグループ内では、
            //    Node同一性は MergeKey 系で扱い、
            //    代表枝の決定は経路文脈を含めて安定化させる。
            return rows
                .OrderBy(r => BuildRepresentativeRowKey(r, level) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => BuildDownstreamSignature(r, level) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => r.FullPathSignature ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => r.PathOrder)
                .FirstOrDefault();
        }

        

        private void PopulateForwardLotGroupInfos(List<TracePathRow> rows)
        {
            if (rows == null || rows.Count == 0)
                return;

            foreach (var row in rows)
            {
                if (row == null)
                    continue;

                row.LevelLotGroups.Clear();
            }

            PopulateForwardStartLotGroupInfos(rows);
            PopulateForwardMiddleLotGroupInfos(rows);
        }

        private void PopulateForwardStartLotGroupInfos(List<TracePathRow> rows)
        {
            if (rows == null || rows.Count == 0)
                return;

            var targetRows = rows
                .Where(r => r != null && r.StartNode != null && !string.IsNullOrWhiteSpace(r.StartNode.LotNumber))
                .ToList();

            var groups = targetRows
                .GroupBy(r => BuildLotGroupKey(TraceGroupAxis.Start, 0, r.StartNode.LotNumber), StringComparer.OrdinalIgnoreCase)
                .ToList();

            int groupOrder = 0;

            foreach (var group in groups)
            {
                var groupRows = group
                    .OrderBy(r => r.PathOrder)
                    .ToList();

                if (groupRows.Count == 0)
                    continue;

                var representativeRow = groupRows
                    .FirstOrDefault(r => !r.IsPruned)
                    ?? groupRows[0];

                string representativeKey = representativeRow == null || representativeRow.StartNode == null
                    ? null
                    : GetNodeMergeKey(representativeRow.StartNode);

                for (int i = 0; i < groupRows.Count; i++)
                {
                    var row = groupRows[i];
                    if (row == null || row.StartNode == null)
                        continue;

                    string rowNodeKey = GetNodeMergeKey(row.StartNode);

                    row.LevelLotGroups.Add(new TraceLotGroupInfo
                    {
                        Axis = TraceGroupAxis.Start,
                        Level = 0,
                        GroupKey = group.Key,
                        LotNumber = NormalizeLotForGrouping(row.StartNode.LotNumber),
                        GroupOrder = groupOrder,
                        RowOrderInGroup = i,
                        IsFirstRowOfGroup = i == 0,
                        IsLastRowOfGroup = i == groupRows.Count - 1,
                        IsRepresentative = string.Equals(
                            rowNodeKey ?? string.Empty,
                            representativeKey ?? string.Empty,
                            StringComparison.OrdinalIgnoreCase) && !row.IsPruned,
                        RepresentativeKey = representativeKey,
                        DownstreamBranchSignature = BuildDownstreamSignature(row, 0),
                        UpstreamBranchSignature = BuildUpstreamSignature(row, 0),
                        IsPruned = row.IsPruned,
                        PruneReason = row.PruneReason,
                        DrawDividerTop = i == 0,
                        DrawDividerBottom = i == groupRows.Count - 1
                    });
                }

                groupOrder++;
            }
        }

        private void PopulateForwardMiddleLotGroupInfos(List<TracePathRow> rows)
        {
            if (rows == null || rows.Count == 0)
                return;

            int maxLevel = rows.Max(r => r == null || r.MiddleNodes == null ? 0 : r.MiddleNodes.Count);
            int groupOrder = 0;

            for (int level = 1; level <= maxLevel; level++)
            {
                var levelRows = rows
                    .Where(r =>
                        r != null &&
                        r.MiddleNodes != null &&
                        r.MiddleNodes.Count >= level &&
                        r.MiddleNodes[level - 1] != null &&
                        !string.IsNullOrWhiteSpace(r.MiddleNodes[level - 1].LotNumber))
                    .ToList();

                var groups = levelRows
                    .GroupBy(r => BuildLotGroupKey(TraceGroupAxis.Middle, level, r.MiddleNodes[level - 1].LotNumber), StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var group in groups)
                {
                    var groupRows = group
                        .OrderBy(r => r.PathOrder)
                        .ToList();

                    if (groupRows.Count == 0)
                        continue;

                    var representativeRow = SelectRepresentativeRowForMiddleGroup(groupRows, level);
                    string representativeKey = representativeRow == null
                        ? null
                        : BuildRepresentativeRowKey(representativeRow, level);

                    for (int i = 0; i < groupRows.Count; i++)
                    {
                        var row = groupRows[i];
                        if (row == null)
                            continue;

                        var middleNode = row.MiddleNodes[level - 1];
                        string rowRepresentativeKey = BuildRepresentativeRowKey(row, level);

                        row.LevelLotGroups.Add(new TraceLotGroupInfo
                        {
                            Axis = TraceGroupAxis.Middle,
                            Level = level,
                            GroupKey = group.Key,
                            LotNumber = NormalizeLotForGrouping(middleNode == null ? null : middleNode.LotNumber),
                            GroupOrder = groupOrder,
                            RowOrderInGroup = i,
                            IsFirstRowOfGroup = i == 0,
                            IsLastRowOfGroup = i == groupRows.Count - 1,

                            // 代表判定は BuildRepresentativeRowKey で統一
                            // （NodeMergeKey + 下流署名）
                            IsRepresentative = string.Equals(
                                rowRepresentativeKey ?? string.Empty,
                                representativeKey ?? string.Empty,
                                StringComparison.OrdinalIgnoreCase) && !row.IsPruned,

                            RepresentativeKey = representativeKey,
                            DownstreamBranchSignature = BuildDownstreamSignature(row, level),
                            UpstreamBranchSignature = BuildUpstreamSignature(row, level),
                            IsPruned = row.IsPruned && IsRowPrunedAtOrAfterLevel(row, level),
                            PruneReason = row.PruneReason,
                            DrawDividerTop = i == 0,
                            DrawDividerBottom = i == groupRows.Count - 1
                        });
                    }

                    groupOrder++;
                }
            }
        }

        private bool IsRowPrunedAtOrAfterLevel(TracePathRow row, int level)
        {
            if (row == null || !row.IsPruned)
                return false;

            if (string.IsNullOrWhiteSpace(row.PruneReason))
                return true;

            if (row.PruneReason.StartsWith("MiddleLotGroupNonRepresentative_Lv", StringComparison.OrdinalIgnoreCase))
            {
                int parsedLevel;
                string suffix = row.PruneReason.Substring("MiddleLotGroupNonRepresentative_Lv".Length);
                if (int.TryParse(suffix, out parsedLevel))
                {
                    return parsedLevel == level;
                }
            }

            return level == 0;
        }

        

        

        private void MarkRowPruned(TracePathRow row, string pruneReason)
        {
            if (row == null)
                return;

            row.IsPruned = true;
            row.PruneReason = pruneReason;
        }

        private string BuildLotGroupKey(TraceGroupAxis axis, int level, string lotNumber)
        {
            return string.Join("|",
                axis.ToString(),
                level.ToString(),
                NormalizeLotForGrouping(lotNumber) ?? "NO_LOT");
        }

        private string NormalizeLotForGrouping(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                return null;

            return lotNumber.Trim().ToUpperInvariant();
        }

        private string BuildRepresentativeRowKey(TracePathRow row, int level)
        {
            if (row == null)
                return null;

            var node = GetNodeFromPathRow(row, level);
            if (node == null)
                return null;

            // ★代表枝選定用キー
            // ※注意：NodeIdentityKeyは使用しない
            // ※Node同一性は必ず MergeKey 系で扱う
            // ※同一Lv・同一Lotグループ内で、同一Nodeかつ下流文脈が同じものを同一候補として扱う
            return (GetNodeMergeKey(node) ?? string.Empty)
                + "|"
                + (BuildDownstreamSignature(row, level) ?? string.Empty);
        }

        private string BuildFullPathSignature(TracePathRow row)
        {
            if (row == null)
                return null;

            return BuildPathKey(row);
        }

        private string BuildDownstreamSignature(TracePathRow row, int nodeIndexInPath)
        {
            if (row == null)
                return null;

            // 下流署名は経路文脈キーをそのまま使用する。
            // 内部では NodeMergeKey + LinkKey ベースに正規化された
            // DownstreamPathKey を返す前提。
            return GetDownstreamPathKeyFromPathRow(row, nodeIndexInPath);
        }

        private string BuildUpstreamSignature(TracePathRow row, int nodeIndexInPath)
        {
            if (row == null)
                return null;

            // 上流署名は経路文脈キーをそのまま使用する。
            // 内部では NodeMergeKey + LinkKey ベースに正規化された
            // UpstreamPathKey を返す前提。
            return GetUpstreamPathKeyFromPathRow(row, nodeIndexInPath);
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
                var pathRow = new TracePathRow
                {
                    RootGroupKey = rootGroupKey
                };

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

            WritePathRowsDebugLine(
                "VISIT_NODE",
                node,
                null,
                null,
                currentPathNodes.Count,
                node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                outgoingLinks.Count,
                BuildDebugPathNodesText(currentPathNodes),
                BuildDebugPathLinksText(currentPathLinks),
                currentPathNodes.Count,
                currentPathLinks.Count,
                "Enter recursive node");

            if (outgoingLinks.Count == 0)
            {
                var pathRow = new TracePathRow
                {
                    RootGroupKey = rootGroupKey
                };

                FillPathRowNodesAndLinks(pathRow, currentPathNodes, currentPathLinks);
                result.PathRows.Add(pathRow);

                WritePathRowsDebugLine(
                    "LEAF_PATH_ADDED",
                    node,
                    pathRow.StartNode,
                    pathRow.EndNode,
                    currentPathNodes.Count,
                    node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                    0,
                    BuildDebugPathNodesText(currentPathNodes),
                    BuildDebugPathLinksText(currentPathLinks),
                    pathRow.MiddleNodes == null ? 0 : pathRow.MiddleNodes.Count,
                    pathRow.PathLinks == null ? 0 : pathRow.PathLinks.Count,
                    "outgoingLinks.Count == 0");

                currentPathNodes.RemoveAt(currentPathNodes.Count - 1);
                return;
            }

            bool traversedAnyChild = false;

            foreach (var link in outgoingLinks)
            {
                string linkKey = GetPathLinkKey(link);

                if (string.IsNullOrWhiteSpace(linkKey))
                {
                    WritePathRowsDebugLine(
                        "SKIP_LINK_EMPTY_KEY",
                        node,
                        link == null ? null : link.ParentNode,
                        link == null ? null : link.ChildNode,
                        currentPathNodes.Count,
                        node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                        outgoingLinks.Count,
                        BuildDebugPathNodesText(currentPathNodes),
                        BuildDebugPathLinksText(currentPathLinks),
                        currentPathNodes.Count,
                        currentPathLinks.Count,
                        "GetPathLinkKey(link) is empty");
                    continue;
                }

                if (visitedLinks.Contains(linkKey))
                {
                    WritePathRowsDebugLine(
                        "SKIP_LINK_VISITED",
                        node,
                        link.ParentNode,
                        link.ChildNode,
                        currentPathNodes.Count,
                        node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                        outgoingLinks.Count,
                        BuildDebugPathNodesText(currentPathNodes),
                        BuildDebugPathLinksText(currentPathLinks),
                        currentPathNodes.Count,
                        currentPathLinks.Count,
                        "visitedLinks already contains linkKey: " + linkKey+ " DIR=" + link.EdgeDirection);
                    continue;
                }

                visitedLinks.Add(linkKey);
                currentPathLinks.Add(link);

                WritePathRowsDebugLine(
                    "TRAVERSE_LINK",
                    node,
                    link.ParentNode,
                    link.ChildNode,
                    currentPathNodes.Count,
                    node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                    outgoingLinks.Count,
                    BuildDebugPathNodesText(currentPathNodes),
                    BuildDebugPathLinksText(currentPathLinks)
                        + " | DIR=" + (link.EdgeDirection.ToString()),
                    currentPathNodes.Count,
                    currentPathLinks.Count,
                    "Traverse child");

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

                WritePathRowsDebugLine(
                    "RETURN_FROM_CHILD",
                    node,
                    link.ParentNode,
                    link.ChildNode,
                    currentPathNodes.Count,
                    node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                    outgoingLinks.Count,
                    BuildDebugPathNodesText(currentPathNodes),
                    BuildDebugPathLinksText(currentPathLinks),
                    currentPathNodes.Count,
                    currentPathLinks.Count,
                    "Return from child");
            }

            if (!traversedAnyChild)
            {
                var pathRow = new TracePathRow
                {
                    RootGroupKey = rootGroupKey
                };

                FillPathRowNodesAndLinks(pathRow, currentPathNodes, currentPathLinks);
                result.PathRows.Add(pathRow);

                WritePathRowsDebugLine(
                    "FALLBACK_PATH_ADDED",
                    node,
                    pathRow.StartNode,
                    pathRow.EndNode,
                    currentPathNodes.Count,
                    node.ChildLinks == null ? 0 : node.ChildLinks.Count,
                    outgoingLinks.Count,
                    BuildDebugPathNodesText(currentPathNodes),
                    BuildDebugPathLinksText(currentPathLinks),
                    pathRow.MiddleNodes == null ? 0 : pathRow.MiddleNodes.Count,
                    pathRow.PathLinks == null ? 0 : pathRow.PathLinks.Count,
                    "!traversedAnyChild");
            }

            currentPathNodes.RemoveAt(currentPathNodes.Count - 1);
        }

        private void InitializePathRowsDebugCsv()
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_PathRows.csv");

                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join(",",
                        "Action",
                        "NodeKey",
                        "NodeMergeKey",
                        "NodeMasterKey",
                        "NodeLotNumber",
                        "ParentNodeKey",
                        "ChildNodeKey",
                        "RootCount",
                        "ChildLinksCount",
                        "OutgoingLinksCount",
                        "PathNodesText",
                        "PathLinksText",
                        "PathNodeCount",
                        "PathLinkCount",
                        "StartNodeKey",
                        "EndNodeKey",
                        "MiddleCount",
                        "Note"));
                }
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }


        //private List<TracePathRow> BuildPathsFromRoot(ProductionResultNode root)
        //{
        //    var pathRows = new List<TracePathRow>();

        //    var currentNodes = new List<ProductionResultNode>();
        //    var currentLinks = new List<ProductionResultLink>();
        //    var visitedLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //    // ★ TraceResult使わない
        //    BuildPathRowsRecursiveCore(
        //        pathRows,
        //        root,
        //        root.NodeIdentityKey,
        //        currentNodes,
        //        currentLinks,
        //        visitedLinks);

        //    return pathRows;
        //}



        private void WritePathRowsDebugLine(
            string action,
            ProductionResultNode node,
            ProductionResultNode startNode,
            ProductionResultNode endNode,
            int rootCount,
            int childLinksCount,
            int outgoingLinksCount,
            string pathNodesText,
            string pathLinksText,
            int pathNodeCount,
            int pathLinkCount,
            string note)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_PathRows.csv");

                using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join(",",
                        EscapeCsv(action),
                        EscapeCsv(GetDebugNodeKey(node)),
                        EscapeCsv(GetSafeNodeMergeKey(node)),
                        EscapeCsv(node == null ? null : node.ControlMasterKey),
                        EscapeCsv(node == null ? null : node.LotNumber),
                        EscapeCsv(GetDebugNodeKey(startNode)),
                        EscapeCsv(GetDebugNodeKey(endNode)),
                        EscapeCsv(rootCount.ToString()),
                        EscapeCsv(childLinksCount.ToString()),
                        EscapeCsv(outgoingLinksCount.ToString()),
                        EscapeCsv(pathNodesText),
                        EscapeCsv(pathLinksText),
                        EscapeCsv(pathNodeCount.ToString()),
                        EscapeCsv(pathLinkCount.ToString()),
                        EscapeCsv(GetDebugNodeKey(startNode)),
                        EscapeCsv(GetDebugNodeKey(endNode)),
                        EscapeCsv(Math.Max(0, pathNodeCount - 2).ToString()),
                        EscapeCsv(note)));
                }
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        private string GetSafeNodeMergeKey(ProductionResultNode node)
        {
            try
            {
                return node == null ? null : GetNodeMergeKey(node);
            }
            catch
            {
                return null;
            }
        }

        private string BuildDebugPathNodesText(List<ProductionResultNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return null;

            var parts = new List<string>();

            foreach (var node in nodes)
            {
                parts.Add(GetDebugNodeKey(node));
            }

            return string.Join(" -> ", parts);
        }

        private string BuildDebugPathLinksText(List<ProductionResultLink> links)
        {
            if (links == null || links.Count == 0)
                return null;

            var parts = new List<string>();

            foreach (var link in links)
            {
                if (link == null)
                {
                    parts.Add("(null)");
                    continue;
                }

                string parentKey = GetDebugNodeKey(link.ParentNode);
                string childKey = GetDebugNodeKey(link.ChildNode);
                string source = link.SourceTable.ToString();
                string slotNo = link.SlotNo.ToString();

                parts.Add(
                    (parentKey ?? "?")
                    + "=>"
                    + (childKey ?? "?")
                    + "|"
                    + source
                    + "|S"
                    + slotNo);
            }

            return string.Join(" / ", parts);
        }

        private string GetNodeIdentityKey(ProductionResultNode node)
        {
            if (node == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(node.ControlMasterKey))
                return node.ControlMasterKey;

            if (!string.IsNullOrWhiteSpace(node.LotNumber))
                return "LOT|" + node.LotNumber;

            return string.Empty;
        }

        private string ResolveStartDateLabel(ProductionResultNode node, ChildCandidate candidate)
        {
            if (node == null)
                return null;

            if (!string.IsNullOrWhiteSpace(node.StartDateLabel))
                return node.StartDateLabel;

            bool isMaterialA = candidate != null
                && candidate.SourceTable == TraceSourceTable.MaterialTableA;

            if (!isMaterialA)
                return null;

            return node.StartDate.HasValue ? null : "手投入";
        }

        
        

        

        private string GetNodeMergeKey(ProductionResultNode node)
        {
            if (node == null)
                return Guid.NewGuid().ToString();

            string routeSystem = string.IsNullOrWhiteSpace(node.RouteSystem)
                ? ""
                : node.RouteSystem.Trim().ToUpperInvariant();

            string masterKey = string.IsNullOrWhiteSpace(node.ControlMasterKey)
                ? ""
                : node.ControlMasterKey.Trim();

            string itemCode = string.IsNullOrWhiteSpace(node.ItemCode)
                ? ""
                : node.ItemCode.Trim().ToUpperInvariant();

            string lotNumber = string.IsNullOrWhiteSpace(node.LotNumber)
                ? ""
                : node.LotNumber.Trim().ToUpperInvariant();

            string inputSourceType = string.IsNullOrWhiteSpace(node.InputSourceType)
                ? ""
                : node.InputSourceType.Trim().ToUpperInvariant();

            string slotPart = node.InputSlotNo.HasValue
                ? node.InputSlotNo.Value.ToString()
                : "";

            // 1. B系統：MasterKey で識別
            if (string.Equals(routeSystem, "B", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(masterKey))
                    return "N|B|" + masterKey;

                // 念のためのフォールバック
                return string.Join("|", "N", "B", itemCode, lotNumber);
            }

            // 2. A系統：MasterKey + SlotNo で識別
            if (string.Equals(routeSystem, "A", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(masterKey))
                    return "N|A|" + masterKey + "|S" + (string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);

                // A特殊系フォールバック：
                // MasterKeyが無い場合でも Lot だけで潰さないよう、ItemCode / 入力種別 / Slot を含める
                return string.Join("|",
                    "N",
                    "A",
                    itemCode,
                    lotNumber,
                    inputSourceType,
                    string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);
            }

            // 3. RouteSystem未設定時のフォールバック
            if (!string.IsNullOrWhiteSpace(masterKey))
                return "N|U|" + masterKey;

            if (!string.IsNullOrWhiteSpace(lotNumber))
            {
                return string.Join("|",
                    "N",
                    "U",
                    itemCode,
                    lotNumber,
                    inputSourceType,
                    string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);
            }

            return Guid.NewGuid().ToString();
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

        private void EnsureRootRegistered(TraceResult result, ProductionResultNode node)
        {
            if (result == null || node == null)
                return;

            if (!result.RootNodes.Contains(node))
            {
                result.RootNodes.Add(node);
            }
        }

        

        
        

        private string BuildEffectiveLinkDedupKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ChildCandidate candidate)
        {
            string parentMergeKey = GetNodeMergeKey(parentNode);
            string childMergeKey = GetNodeMergeKey(childNode);

            string sourceTable = candidate == null
                ? ""
                : candidate.SourceTable.ToString();

            string inputType = candidate == null
                ? ""
                : candidate.MaterialAInputType.ToString();

            string slotNo = candidate == null
                ? ""
                : candidate.SlotNo.ToString();

            string parentLot = candidate == null || string.IsNullOrWhiteSpace(candidate.ParentLotNumber)
                ? ""
                : candidate.ParentLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                "EDGE",
                parentMergeKey ?? "",
                childMergeKey ?? "",
                sourceTable,
                inputType,
                slotNo,
                parentLot);
        }

        private string BuildEffectiveLinkDedupKey(
            ProductionResultNode parentNode,
            ProductionResultNode childNode,
            ProductionResultLink link)
        {
            string parentMergeKey = GetNodeMergeKey(parentNode);
            string childMergeKey = GetNodeMergeKey(childNode);

            string sourceTable = link == null
                ? ""
                : link.SourceTable.ToString();

            string inputType = link == null
                ? ""
                : link.MaterialAInputType.ToString();

            string slotNo = link == null
                ? ""
                : link.SlotNo.ToString();

            string parentLot = link == null || string.IsNullOrWhiteSpace(link.ParentLotNumber)
                ? ""
                : link.ParentLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                "EDGE",
                parentMergeKey ?? "",
                childMergeKey ?? "",
                sourceTable,
                inputType,
                slotNo,
                parentLot);
        }

        private void WriteLinkDebugCsv(
    string action,
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ChildCandidate candidate,
    ProductionResultLink existingLink = null)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_AddLink.csv");

                bool exists = File.Exists(filePath);

                using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    if (!exists)
                    {
                        sw.WriteLine(string.Join(",",
                            "Action",
                            "CandidateLinkIdentityKey",
                            "CandidateSourceTable",
                            "CandidateMaterialAInputType",
                            "CandidateSlotNo",
                            "CandidateParentLotNumber",
                            "ParentNodeKey",
                            "ParentMasterKey",
                            "ParentLotNumber",
                            "ParentOrder",
                            "ChildNodeKey",
                            "ChildMasterKey",
                            "ChildLotNumber",
                            "ChildOrder",
                            "ExistingLinkIdentityKey",
                            "ExistingParentNodeKey",
                            "ExistingParentMasterKey",
                            "ExistingParentLotNumber",
                            "ExistingChildNodeKey",
                            "ExistingChildMasterKey",
                            "ExistingChildLotNumber"));
                    }

                    sw.WriteLine(string.Join(",",
                        EscapeCsv(action),
                        EscapeCsv(candidate == null ? null : candidate.LinkIdentityKey),
                        EscapeCsv(candidate == null ? null : candidate.SourceTable.ToString()),
                        EscapeCsv(candidate == null ? null : candidate.MaterialAInputType.ToString()),
                        EscapeCsv(candidate == null ? null : candidate.SlotNo.ToString()),
                        EscapeCsv(candidate == null ? null : candidate.ParentLotNumber),

                        EscapeCsv(GetDebugNodeKey(parentNode)),
                        EscapeCsv(parentNode == null ? null : parentNode.ControlMasterKey),
                        EscapeCsv(parentNode == null ? null : parentNode.LotNumber),
                        EscapeCsv(parentNode == null ? null : parentNode.ProductionOrderNumber),

                        EscapeCsv(GetDebugNodeKey(childNode)),
                        EscapeCsv(childNode == null ? null : childNode.ControlMasterKey),
                        EscapeCsv(childNode == null ? null : childNode.LotNumber),
                        EscapeCsv(childNode == null ? null : childNode.ProductionOrderNumber),

                        EscapeCsv(existingLink == null ? null : existingLink.LinkIdentityKey),
                        EscapeCsv(existingLink == null ? null : GetDebugNodeKey(existingLink.ParentNode)),
                        EscapeCsv(existingLink == null || existingLink.ParentNode == null ? null : existingLink.ParentNode.ControlMasterKey),
                        EscapeCsv(existingLink == null || existingLink.ParentNode == null ? null : existingLink.ParentNode.LotNumber),
                        EscapeCsv(existingLink == null ? null : GetDebugNodeKey(existingLink.ChildNode)),
                        EscapeCsv(existingLink == null || existingLink.ChildNode == null ? null : existingLink.ChildNode.ControlMasterKey),
                        EscapeCsv(existingLink == null || existingLink.ChildNode == null ? null : existingLink.ChildNode.LotNumber)
                    ));
                }
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        private string GetDebugNodeKey(ProductionResultNode node)
        {
            if (node == null)
                return null;

            if (!string.IsNullOrWhiteSpace(node.ControlMasterKey))
                return node.ControlMasterKey;

            if (!string.IsNullOrWhiteSpace(node.LotNumber))
                return "LOT|" + node.LotNumber;

            return null;
        }

        

        private void RebuildLegacyNodeLists(TraceResult result)
        {
            result.StartNodes.Clear();
            result.MiddleNodes.Clear();
            result.EndNodes.Clear();

            foreach (var node in result.AllNodes)
            {
                if (node == null)
                    continue;

                if (node.ParentNodes.Count == 0)
                {
                    result.StartNodes.Add(node);
                }
                else if (node.ChildNodes.Count == 0)
                {
                    result.EndNodes.Add(node);
                }
                else
                {
                    result.MiddleNodes.Add(node);
                }
            }

            result.RootNodes.Clear();
            foreach (var node in result.StartNodes)
            {
                result.RootNodes.Add(node);
            }
        }



        #endregion

        #region トレースバック（仕様書 7.2）
        /// <summary>
        /// 枝構造化に向けて工事中。新中間モデル適用版
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private TraceResult TraceBackward(TraceSearchParameters p)
        {
            var result = new TraceResult();

            var nodeMap = new Dictionary<string, ProductionResultNode>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<ProductionResultNode>();
            var queuedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var linkMap = new Dictionary<string, ProductionResultLink>(StringComparer.OrdinalIgnoreCase);

            // 追加:
            // backward 枝構造用に、childNode 単位で親候補を保持する
            var backwardCandidatesByChildNodeKey =
                new Dictionary<string, List<BackwardParentCandidate>>(StringComparer.OrdinalIgnoreCase);

            // ------------------------------------------------------------
            // STEP1:
            // バック検索の開始ノード取得
            // ------------------------------------------------------------
            var startNodes = BuildBackwardStartNodes(p);

            if (startNodes != null)
            {
                foreach (var raw in startNodes)
                {
                    if (raw == null)
                        continue;

                    raw.Depth = 0;
                    raw.ParentKey = null;
                    raw.StartDateLabel = ResolveStartDateLabel(raw, null);

                    var node = GetOrAddNode(nodeMap, raw);
                    EnsureNodeRegistered(result, node);
                    EnsureRootRegistered(result, node);

                    string queueKey = GetNodeMergeKey(node);
                    if (string.IsNullOrWhiteSpace(queueKey) || queuedKeys.Add(queueKey))
                    {
                        queue.Enqueue(node);
                    }
                }
            }

            if (queue.Count == 0)
            {
                DumpBackwardTraceArtifacts(result, null, "QUEUE_EMPTY");
                return result;
            }

            // ------------------------------------------------------------
            // STEP2:
            // BFS（current = child / candidate.Node = parent）
            // ------------------------------------------------------------
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == null)
                    continue;

                if (current.Depth >= MaxDepth)
                    continue;

                WriteCurrentNodeDebug(current);

                var parentCandidates = _repo.FindBackwardParentsByLotStepFlow(current, current.Depth + 1);
                if (parentCandidates == null || parentCandidates.Count == 0)
                    continue;

                var localEdgeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var candidate in parentCandidates)
                {
                    if (candidate == null || candidate.Node == null)
                        continue;

                    var rawParent = candidate.Node;

                    if (string.IsNullOrWhiteSpace(rawParent.LotNumber) &&
                        string.IsNullOrWhiteSpace(rawParent.ControlMasterKey))
                    {
                        continue;
                    }

                    rawParent.Depth = current.Depth + 1;
                    rawParent.StartDateLabel = ResolveBackwardStartDateLabel(rawParent);
                    rawParent.ParentKey = null;

                    ApplyBackwardParentNodeBusinessAttributes(rawParent, candidate);

                    var parentNode = GetOrAddNode(nodeMap, rawParent);
                    EnsureNodeRegistered(result, parentNode);

                    string localEdgeKey = BuildBackwardLocalEdgeKey(parentNode, current, candidate);
                    if (!string.IsNullOrWhiteSpace(localEdgeKey) &&
                        !localEdgeKeys.Add(localEdgeKey))
                    {
                        continue;
                    }

                    bool linkAdded = AddBackwardLinkFromParentCandidate(
                        result,
                        linkMap,
                        parentNode,
                        current,
                        candidate);

                    // 追加:
                    // backward 枝構造は ParentNodes 直読みではなく、
                    // candidate の RelationKey 文脈を保持して組み立てる
                    RegisterBackwardParentCandidateForDisplay(
                        backwardCandidatesByChildNodeKey,
                        current,
                        parentNode,
                        candidate);

                    if (linkAdded)
                    {
                        string parentQueueKey = GetNodeMergeKey(parentNode);
                        if (string.IsNullOrWhiteSpace(parentQueueKey) || queuedKeys.Add(parentQueueKey))
                        {
                            queue.Enqueue(parentNode);
                        }
                    }
                }
            }

            // ------------------------------------------------------------
            // STEP3:
            // バック探索結果 → 枝構造化
            // ------------------------------------------------------------
            var backwardLaneBuildResult = BuildBackwardDisplayLaneNodes(
                result,
                startNodes,
                backwardCandidatesByChildNodeKey);

            DumpBackwardTraceArtifacts(result, backwardLaneBuildResult, "COMPLETED");

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

                string mergeKey = GetNodeMergeKey(raw);

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

        #region 経路展開

        
        private void BuildPathsFromRootRecursive(
    ProductionResultNode current,
    List<ProductionResultNode> currentPathNodes,
    List<ProductionResultLink> currentPathLinks,
    HashSet<string> visitingNodeKeys,
    List<TracePathRow> result)
        {
            if (current == null || result == null)
                return;

            string currentMergeKey = GetNodeMergeKey(current) ?? string.Empty;

            // DAG前提だが、安全のため循環は打ち切る
            if (!string.IsNullOrWhiteSpace(currentMergeKey))
            {
                if (!visitingNodeKeys.Add(currentMergeKey))
                {
                    var cycleRow = new TracePathRow();
                    var cycleNodes = new List<ProductionResultNode>(currentPathNodes);
                    cycleNodes.Add(current);

                    FillPathRowNodesAndLinks(cycleRow, cycleNodes, currentPathLinks);
                    result.Add(cycleRow);
                    return;
                }
            }

            currentPathNodes.Add(current);

            var orderedChildLinks = GetOrderedChildLinks(current);

            if (orderedChildLinks.Count == 0 || current.IsTraceTerminal)
            {
                var pathRow = new TracePathRow();
                FillPathRowNodesAndLinks(pathRow, currentPathNodes, currentPathLinks);
                result.Add(pathRow);
            }
            else
            {
                foreach (var link in orderedChildLinks)
                {
                    if (link == null || link.ChildNode == null)
                        continue;

                    currentPathLinks.Add(link);

                    BuildPathsFromRootRecursive(
                        link.ChildNode,
                        currentPathNodes,
                        currentPathLinks,
                        visitingNodeKeys,
                        result);

                    currentPathLinks.RemoveAt(currentPathLinks.Count - 1);
                }
            }

            currentPathNodes.RemoveAt(currentPathNodes.Count - 1);

            if (!string.IsNullOrWhiteSpace(currentMergeKey))
            {
                visitingNodeKeys.Remove(currentMergeKey);
            }
        }

        private List<ProductionResultLink> GetOrderedChildLinks(ProductionResultNode node)
        {
            if (node == null || node.ChildLinks == null || node.ChildLinks.Count == 0)
                return new List<ProductionResultLink>();

            return node.ChildLinks
                .Where(x => x != null && x.ChildNode != null)
                .OrderBy(x => BuildChildLinkSortKey(x), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private string BuildChildLinkSortKey(ProductionResultLink link)
        {
            if (link == null)
                return "~~~";

            string childMergeKey = GetNodeMergeKey(link.ChildNode) ?? string.Empty;
            string sourceTable = link.SourceTable.ToString();
            string inputType = link.MaterialAInputType.ToString();
            string slotNo = link.SlotNo.ToString("D4");
            string parentLot = string.IsNullOrWhiteSpace(link.ParentLotNumber)
                ? string.Empty
                : link.ParentLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                childMergeKey,
                sourceTable,
                inputType,
                slotNo,
                parentLot,
                link.LinkIdentityKey ?? string.Empty);
        }

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
                string startMergeKey = GetNodeMergeKey(pathRow.StartNode);
                string endMergeKey = GetNodeMergeKey(candidateEndNode);

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

        #region　メタ・経路キー

        private ProductionResultNode GetNodeFromPathRow(TracePathRow pathRow, int nodeIndexInPath)
        {
            if (pathRow == null)
                return null;

            if (nodeIndexInPath < 0)
                return null;

            if (nodeIndexInPath == 0)
                return pathRow.StartNode;

            int middleCount = pathRow.MiddleNodes == null ? 0 : pathRow.MiddleNodes.Count;

            if (nodeIndexInPath >= 1 && nodeIndexInPath <= middleCount)
            {
                return pathRow.MiddleNodes[nodeIndexInPath - 1];
            }

            if (nodeIndexInPath == middleCount + 1)
                return pathRow.EndNode;

            return null;
        }

        private string GetDisplayParentNodeKeyFromPathRow(TracePathRow pathRow, int nodeIndexInPath)
        {
            if (pathRow == null)
                return null;

            if (nodeIndexInPath <= 0)
                return null;

            if (pathRow.PathLinks == null)
                return null;

            int linkIndex = nodeIndexInPath - 1;
            if (linkIndex < 0 || linkIndex >= pathRow.PathLinks.Count)
                return null;

            var link = pathRow.PathLinks[linkIndex];
            if (link == null || link.ParentNode == null)
                return null;

            // ★描画接続用：NodeIdentityKeyではなくMergeKeyを使用
            // ※Node同一性の正規判定に合わせる
            string parentMergeKey = GetNodeMergeKey(link.ParentNode);
            return string.IsNullOrWhiteSpace(parentMergeKey) ? null : parentMergeKey;
        }

        private string GetIncomingLinkKeyFromPathRow(TracePathRow pathRow, int nodeIndexInPath)
        {
            if (pathRow == null)
                return null;

            if (nodeIndexInPath <= 0)
                return null;

            if (pathRow.PathLinks == null)
                return null;

            int linkIndex = nodeIndexInPath - 1;
            if (linkIndex < 0 || linkIndex >= pathRow.PathLinks.Count)
                return null;

            var link = pathRow.PathLinks[linkIndex];
            if (link == null)
                return null;

            return GetPathLinkKey(link);
        }

        private string GetParentKeyFromPathRow(TracePathRow pathRow, int nodeIndexInPath)
        {
            if (pathRow == null)
                return null;

            if (nodeIndexInPath <= 0)
                return null;

            if (pathRow.PathLinks == null)
                return null;

            int linkIndex = nodeIndexInPath - 1;
            if (linkIndex < 0 || linkIndex >= pathRow.PathLinks.Count)
                return null;

            var link = pathRow.PathLinks[linkIndex];
            if (link == null || link.ParentNode == null)
                return null;

            // ★旧互換ParentKeyだが、値はMergeKeyベースへ正規化する
            // ※NodeIdentityKeyは使用しない
            string parentMergeKey = GetNodeMergeKey(link.ParentNode);
            return string.IsNullOrWhiteSpace(parentMergeKey) ? null : parentMergeKey;
        }

        private string GetUpstreamPathKeyFromPathRow(TracePathRow pathRow, int nodeIndexInPath)
        {
            if (pathRow == null)
                return null;

            var parts = new List<string>();

            var startNode = GetNodeFromPathRow(pathRow, 0);
            if (startNode == null)
                return null;

            parts.Add("N:" + GetSafeNodeMergeKey(startNode));

            if (nodeIndexInPath <= 0)
            {
                return string.Join("->", parts);
            }

            if (pathRow.PathLinks == null || pathRow.PathLinks.Count == 0)
                return string.Join("->", parts);

            int maxLinkIndex = Math.Min(nodeIndexInPath - 1, pathRow.PathLinks.Count - 1);

            for (int i = 0; i <= maxLinkIndex; i++)
            {
                var link = pathRow.PathLinks[i];
                if (link == null)
                    continue;

                parts.Add("L:" + GetPathLinkKey(link));

                var childNode = link.ChildNode;
                if (childNode != null)
                {
                    parts.Add("N:" + GetSafeNodeMergeKey(childNode));
                }
            }

            return string.Join("->", parts);
        }

        private string GetDownstreamPathKeyFromPathRow(TracePathRow pathRow, int nodeIndexInPath)
        {
            if (pathRow == null)
                return null;

            var currentNode = GetNodeFromPathRow(pathRow, nodeIndexInPath);
            if (currentNode == null)
                return null;

            var parts = new List<string>();
            parts.Add("N:" + GetSafeNodeMergeKey(currentNode));

            if (pathRow.PathLinks == null || pathRow.PathLinks.Count == 0)
                return string.Join("->", parts);

            for (int i = nodeIndexInPath; i < pathRow.PathLinks.Count; i++)
            {
                var link = pathRow.PathLinks[i];
                if (link == null)
                    continue;

                parts.Add("L:" + GetPathLinkKey(link));

                var childNode = link.ChildNode;
                if (childNode != null)
                {
                    parts.Add("N:" + GetSafeNodeMergeKey(childNode));
                }
            }

            return string.Join("->", parts);
        }

        private string GetPathLinkKey(ProductionResultLink link)
        {
            if (link == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(link.LinkIdentityKey))
                return link.LinkIdentityKey;

            string parentKey = GetSafeNodeMergeKey(link.ParentNode);
            string childKey = GetSafeNodeMergeKey(link.ChildNode);

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

        private string BuildPathKey(TracePathRow pathRow)
        {
            if (pathRow == null)
                return string.Empty;

            var parts = new List<string>();

            if (pathRow.StartNode != null)
                parts.Add("N:" + GetSafeNodeMergeKey(pathRow.StartNode));

            if (pathRow.PathLinks != null)
            {
                foreach (var link in pathRow.PathLinks)
                {
                    parts.Add("L:" + GetPathLinkKey(link));

                    if (link != null && link.ChildNode != null)
                    {
                        parts.Add("N:" + GetSafeNodeMergeKey(link.ChildNode));
                    }
                }
            }
            else
            {
                if (pathRow.MiddleNodes != null)
                {
                    foreach (var node in pathRow.MiddleNodes)
                    {
                        parts.Add("N:" + GetSafeNodeMergeKey(node));
                    }
                }

                if (pathRow.EndNode != null)
                    parts.Add("N:" + GetSafeNodeMergeKey(pathRow.EndNode));
            }

            return string.Join("->", parts);
        }

        private string BuildLotTraceKey(TracePathRow pathRow)
        {
            if (pathRow == null)
                return null;

            var parts = new List<string>();

            // Start
            AppendNodeAndLot(parts, pathRow.StartNode, isEnd: false);

            // Middle
            if (pathRow.MiddleNodes != null)
            {
                foreach (var node in pathRow.MiddleNodes)
                {
                    AppendNodeAndLot(parts, node, isEnd: false);
                }
            }

            // End
            AppendNodeAndLot(parts, pathRow.EndNode, isEnd: true);

            return parts.Count == 0
                ? null
                : string.Join("->", parts);
        }

        private string BuildUpstreamLotOnlyKey(TracePathRow pathRow)
        {
            if (pathRow == null)
                return null;

            // 行単位キーなので、その行の到達先（EndNode）を自Nodeとみなす
            var selfNode = pathRow.EndNode ?? pathRow.StartNode;
            if (selfNode == null)
                return null;

            var parts = new List<string>();

            // 1) 自Node識別だけは持たせる
            parts.Add(BuildNodeOnlyKey(selfNode));

            // 2) そこに至るまでの接続Lotだけを積む
            if (pathRow.PathLinks != null)
            {
                foreach (var link in pathRow.PathLinks)
                {
                    string lot = GetLinkLotNumber(link);
                    if (!string.IsNullOrEmpty(lot))
                    {
                        parts.Add("L|" + lot);
                    }
                    else
                    {
                        parts.Add("L|NO_LOT");
                    }
                }
            }

            return parts.Count == 0
                ? null
                : string.Join("<=", parts);
        }

        private string BuildNodeOnlyKey(ProductionResultNode node)
        {
            if (node == null)
                return "N|?";

            if (IsBSystemNode(node))
            {
                return "N|B|" + Safe(node.ControlMasterKey);
            }

            int slotNo = GetASlotNumber(node);
            string slotPart = slotNo > 0 ? slotNo.ToString() : "?";

            return "N|A|" + Safe(node.ControlMasterKey) + "|S" + slotPart;
        }

        private string GetLinkLotNumber(ProductionResultLink link)
        {
            if (link == null)
                return null;

            // 今回の「接続要素としてのLot」はまず ParentLotNumber を正採用
            string lot = NormalizeLot(link.ParentLotNumber);
            if (!string.IsNullOrEmpty(lot))
                return lot;

            return null;
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

        #region 表示座標化


        

        #endregion

        /// <summary>
        /// 重複チェックに使うキー:
        ///   ProductionOrderNumber + "|" + LotNumber + "|" + 工程種別
        /// 工程種別は ControlMasterKey の末尾の '_' の 3 文字前の 1 文字を想定。
        /// ControlMasterKey が無い / 異常な形式の場合は、NodeKey のみを返す。
        /// </summary>
        //private string BuildProcessTypeKey(ProductionResultNode node)
        //{
        //    if (node == null)
        //    {
        //        return string.Empty;
        //    }

        //    string baseKey = node.NodeKey;

        //    if (string.IsNullOrEmpty(node.ControlMasterKey))
        //    {
        //        return baseKey;
        //    }

        //    string mk = node.ControlMasterKey;

        //    int idxUnd = mk.LastIndexOf('_');
        //    if (idxUnd <= 0)
        //    {
        //        return baseKey;
        //    }

        //    string before = mk.Substring(0, idxUnd);
        //    if (before.Length < 3)
        //    {
        //        return baseKey;
        //    }

        //    char typeChar = before[before.Length - 3];

        //    return baseKey + "|" + typeChar;
        //}

        #region 交点検出（仕様書 7.3）

        /// <summary>
        /// 複数タブのトレース結果から交点を検出
        /// key: (ItemCode, LotNumber)
        /// value: (タブ番号リスト, 代表ノード情報)
        /// </summary>
        public List<CrossPointRecord> DetectCrossPoints(
            Dictionary<int, TraceResult> tabResults)
        {
            // key = MaterialPair, value = (タブ番号リスト, 代表ノード)
            var dict = new Dictionary<MaterialPair, (HashSet<int> Tabs, ProductionResultNode Node)>();

            foreach (var kv in tabResults)
            {
                int tabNo = kv.Key;
                var trace = kv.Value;

                IEnumerable < ProductionResultNode > allNodes = trace.AllNodes;

                foreach (var node in allNodes)
                {
                    if (string.IsNullOrWhiteSpace(node.ItemCode) || string.IsNullOrWhiteSpace(node.LotNumber))
                        continue;

                    var key = new MaterialPair(node.ItemCode, node.LotNumber);

                    if (!dict.TryGetValue(key, out var entry))
                    {
                        entry = (new HashSet<int>(), node);
                        dict[key] = entry;
                    }

                    entry.Tabs.Add(tabNo);
                }
            }

            // 交点の有無に関わらず全て列挙する場合：
            var records = new List<CrossPointRecord>();
            foreach (var kv in dict)
            {
                var node = kv.Value.Node;
                var tabNos = string.Join(",", kv.Value.Tabs.OrderBy(x => x));
                records.Add(new CrossPointRecord
                {
                    ProductionOrderNumber = node.ProductionOrderNumber,
                    ItemName = node.ItemName,
                    ItemCode = node.ItemCode,
                    LotNumber = node.LotNumber,
                    TabNumbers = tabNos
                });
            }

            return records;
        }

        #endregion

        private void WriteChildCandidatesDebugCsv(
    ProductionResultNode current,
    List<ChildCandidate> candidatesA,
    List<ChildCandidate> candidatesB,
    List<ChildCandidate> childCandidates)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_ChildCandidates.csv");

                bool exists = File.Exists(filePath);

                using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    if (!exists)
                    {
                        sw.WriteLine(string.Join(",",
                            "CurrentNodeKey",
                            "CurrentMasterKey",
                            "CurrentLotNumber",
                            "CurrentOrder",
                            "CandidatesACount",
                            "CandidatesBCount",
                            "MergedCount",
                            "CandidateSourceTable",
                            "CandidateLinkIdentityKey",
                            "CandidateChildNodeKey",
                            "CandidateChildMasterKey",
                            "CandidateChildLotNumber",
                            "CandidateChildOrder"));
                    }

                    if (childCandidates == null || childCandidates.Count == 0)
                    {
                        sw.WriteLine(string.Join(",",
                            EscapeCsv(GetDebugNodeKey(current)),
                            EscapeCsv(current == null ? null : current.ControlMasterKey),
                            EscapeCsv(current == null ? null : current.LotNumber),
                            EscapeCsv(current == null ? null : current.ProductionOrderNumber),
                            EscapeCsv(candidatesA == null ? "0" : candidatesA.Count.ToString()),
                            EscapeCsv(candidatesB == null ? "0" : candidatesB.Count.ToString()),
                            EscapeCsv("0"),
                            "",
                            "",
                            "",
                            "",
                            "",
                            ""));
                        return;
                    }

                    foreach (var candidate in childCandidates)
                    {
                        var node = candidate == null ? null : candidate.Node;

                        sw.WriteLine(string.Join(",",
                            EscapeCsv(GetDebugNodeKey(current)),
                            EscapeCsv(current == null ? null : current.ControlMasterKey),
                            EscapeCsv(current == null ? null : current.LotNumber),
                            EscapeCsv(current == null ? null : current.ProductionOrderNumber),
                            EscapeCsv(candidatesA == null ? "0" : candidatesA.Count.ToString()),
                            EscapeCsv(candidatesB == null ? "0" : candidatesB.Count.ToString()),
                            EscapeCsv(childCandidates.Count.ToString()),
                            EscapeCsv(candidate == null ? null : candidate.SourceTable.ToString()),
                            EscapeCsv(candidate == null ? null : candidate.LinkIdentityKey),
                            EscapeCsv(GetDebugNodeKey(node)),
                            EscapeCsv(node == null ? null : node.ControlMasterKey),
                            EscapeCsv(node == null ? null : node.LotNumber),
                            EscapeCsv(node == null ? null : node.ProductionOrderNumber)));
                    }
                }
            }
            catch
            {
            }
        }
        private void WriteCurrentNodeDebug(ProductionResultNode current)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_CurrentNodes.csv");

                bool exists = File.Exists(filePath);

                using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    if (!exists)
                    {
                        sw.WriteLine("NodeKey,MasterKey,LotNumber,ProductionOrderNumber,Depth");
                    }

                    sw.WriteLine(string.Join(",",
                        EscapeCsv(GetDebugNodeKey(current)),
                        EscapeCsv(current == null ? null : current.ControlMasterKey),
                        EscapeCsv(current == null ? null : current.LotNumber),
                        EscapeCsv(current == null ? null : current.ProductionOrderNumber),
                        EscapeCsv(current == null ? null : current.Depth.ToString())));
                }
            }
            catch
            {
            }
        }

        #region 品目名処理群
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


        
        #region デバッグ群


        private void WriteChildCandidateDetailsDebugCsv(
    ProductionResultNode current,
    IEnumerable<ChildCandidate> candidates,
    string sourceName)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_ChildCandidateDetails.csv");

                bool exists = File.Exists(filePath);

                using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    if (!exists)
                    {
                        sw.WriteLine(string.Join(",",
                            "SourceName",
                            "CurrentMergeKey",
                            "CurrentRouteSystem",
                            "CurrentMasterKey",
                            "CurrentLotNumber",
                            "CurrentItemCode",
                            "CandidateLinkIdentityKey",
                            "CandidateSourceTable",
                            "CandidateInputType",
                            "CandidateSlotNo",
                            "CandidateParentLotNumber",
                            "NodeMergeKey",
                            "NodeOnlyKey",
                            "NodeRouteSystem",
                            "NodeMasterKey",
                            "NodeLotNumber",
                            "NodeItemCode",
                            "NodeInputSourceType",
                            "NodeInputSlotNo",
                            "NodeStartDateLabel",
                            "NodeWeight"));
                    }

                    if (candidates == null)
                        return;

                    foreach (var candidate in candidates)
                    {
                        var node = candidate == null ? null : candidate.Node;

                        sw.WriteLine(string.Join(",",
                            EscapeCsv(sourceName),
                            EscapeCsv(current == null ? null : GetNodeMergeKey(current)),
                            EscapeCsv(current == null ? null : current.RouteSystem),
                            EscapeCsv(current == null ? null : current.ControlMasterKey),
                            EscapeCsv(current == null ? null : current.LotNumber),
                            EscapeCsv(current == null ? null : current.ItemCode),

                            EscapeCsv(candidate == null ? null : candidate.LinkIdentityKey),
                            EscapeCsv(candidate == null ? null : candidate.SourceTable.ToString()),
                            EscapeCsv(candidate == null ? null : candidate.MaterialAInputType.ToString()),
                            EscapeCsv(candidate == null ? null : candidate.SlotNo.ToString()),
                            EscapeCsv(candidate == null ? null : candidate.ParentLotNumber),

                            EscapeCsv(node == null ? null : GetNodeMergeKey(node)),
                            EscapeCsv(node == null ? null : BuildNodeOnlyKey(node)),
                            EscapeCsv(node == null ? null : node.RouteSystem),
                            EscapeCsv(node == null ? null : node.ControlMasterKey),
                            EscapeCsv(node == null ? null : node.LotNumber),
                            EscapeCsv(node == null ? null : node.ItemCode),
                            EscapeCsv(node == null ? null : node.InputSourceType),
                            EscapeCsv(node == null || !node.InputSlotNo.HasValue ? null : node.InputSlotNo.Value.ToString()),
                            EscapeCsv(node == null ? null : node.StartDateLabel),
                            EscapeCsv(node == null || !node.Weight.HasValue ? null : node.Weight.Value.ToString())));
                    }
                }
            }
            catch
            {
            }


        }

        

        private void ExportAllNodesDebugCsv(IEnumerable<ProductionResultNode> nodes, string fileName)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    fileName);

                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join(",",
                        "MergeKey",
                        "NodeOnlyKey",
                        "RouteSystem",
                        "MasterKey",
                        "LotNumber",
                        "ItemCode",
                        "ItemName",
                        "InputSourceType",
                        "InputSlotNo",
                        "StartDateLabel",
                        "Depth",
                        "ParentCount",
                        "ChildCount"));

                    if (nodes == null)
                        return;

                    foreach (var node in nodes)
                    {
                        if (node == null)
                            continue;

                        sw.WriteLine(string.Join(",",
                            EscapeCsv(GetNodeMergeKey(node)),
                            EscapeCsv(BuildNodeOnlyKey(node)),
                            EscapeCsv(node.RouteSystem),
                            EscapeCsv(node.ControlMasterKey),
                            EscapeCsv(node.LotNumber),
                            EscapeCsv(node.ItemCode),
                            EscapeCsv(node.ItemName),
                            EscapeCsv(node.InputSourceType),
                            EscapeCsv(node.InputSlotNo.HasValue ? node.InputSlotNo.Value.ToString() : null),
                            EscapeCsv(node.StartDateLabel),
                            EscapeCsv(node.Depth.ToString()),
                            EscapeCsv(node.ParentNodes == null ? "0" : node.ParentNodes.Count.ToString()),
                            EscapeCsv(node.ChildNodes == null ? "0" : node.ChildNodes.Count.ToString())));
                    }
                }
            }
            catch
            {
            }
        }


        

        

        

        private void WriteForwardPathPruneDebugLine(
            string action,
            TracePathRow row,
            int level,
            string groupKey,
            string representativeKey,
            string note)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_ForwardPathPrune.csv");

                using (var sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join(",",
                        EscapeCsv(action),
                        EscapeCsv(row == null ? null : row.PathOrder.ToString()),
                        EscapeCsv(level.ToString()),
                        EscapeCsv(groupKey),
                        EscapeCsv(representativeKey),
                        EscapeCsv(row == null || row.StartNode == null ? null : GetNodeIdentityKey(row.StartNode)),
                        EscapeCsv(row == null || row.EndNode == null ? null : GetNodeIdentityKey(row.EndNode)),
                        EscapeCsv(row == null ? null : row.FullPathSignature),
                        EscapeCsv(row == null ? null : row.PruneReason),
                        EscapeCsv(note)));
                }
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        //private void ExportForwardLv1DisplayLaneDebugCsv(TraceResult result)
        //{
        //    if (result == null || result.RootNodes == null || result.RootNodes.Count == 0)
        //        return;

        //    try
        //    {
        //        var laneNodes = BuildForwardDisplayLaneNodes(result);
        //        if (laneNodes == null || laneNodes.Count == 0)
        //            return;

        //        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        //        string debugDir = Path.Combine(baseDir, "Debug");

        //        if (!Directory.Exists(debugDir))
        //            Directory.CreateDirectory(debugDir);

        //        string filePath = Path.Combine(debugDir, "LotTrace_Debug_DisplayLane_Lv1.csv");

        //        using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
        //        {
        //            writer.WriteLine(
        //                "DisplayNodeKey,MergeKey,ParentDisplayNodeKey,XLevel,YLane,SubtreeLaneSpan,ChildIndex,LotNumber,ControlMasterKey,ItemCode,ItemName");

        //            foreach (var node in laneNodes
        //                .OrderBy(x => x.XLevel)
        //                .ThenBy(x => x.YLane)
        //                .ThenBy(x => x.ChildIndex)
        //                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase))
        //            {
        //                writer.WriteLine(string.Join(",",
        //                    EscapeCsv(node.DisplayNodeKey),
        //                    EscapeCsv(node.MergeKey),
        //                    EscapeCsv(node.ParentDisplayNodeKey),
        //                    EscapeCsv(node.XLevel.ToString()),
        //                    EscapeCsv(node.YLane.ToString()),
        //                    EscapeCsv(node.SubtreeLaneSpan.ToString()),
        //                    EscapeCsv(node.ChildIndex.ToString()),
        //                    EscapeCsv(node.SourceNode == null ? null : node.SourceNode.LotNumber),
        //                    EscapeCsv(node.SourceNode == null ? null : node.SourceNode.ControlMasterKey),
        //                    EscapeCsv(node.SourceNode == null ? null : node.SourceNode.ItemCode),
        //                    EscapeCsv(node.SourceNode == null ? null : node.SourceNode.ItemName)
        //                ));
        //            }
        //        }

        //        ExportForwardLv1DisplayLaneEdgeDebugCsv(laneNodes, debugDir);
        //    }
        //    catch
        //    {
        //        // Debugダンプ失敗で本処理は落とさない
        //    }
        //}

        #region 枝構造関数群


        //枝構造処理呼び出し関数。後で再帰処理化して完成となる
        private ForwardDisplayLaneBuildResult BuildSortedForwardDisplayLaneNodes(TraceResult result)
        {
            var buildResult = new ForwardDisplayLaneBuildResult();

            if (result == null)
                return buildResult;

            // ① 木構築
            buildResult.DisplayNodes = BuildForwardDisplayLaneNodes(result);
            if (buildResult.DisplayNodes == null || buildResult.DisplayNodes.Count == 0)
            {
                buildResult.DisplayNodes = new List<DisplayLaneNode>();
                buildResult.EndInfos = new List<EndDisplayNodeInfo>();
                return buildResult;
            }

            // ② 終点整理
            buildResult.EndInfos = FinalizeForwardEndNodeGroupsAndNormalizeX(buildResult.DisplayNodes);

            // ③ 途中ダンプ
            ExportForwardDisplayLaneNodesDebugCsv(
                buildResult.DisplayNodes,
                buildResult.EndInfos,
                "LotTrace_Debug_DisplayLane_AfterEndNormalize.csv");

            // ④ route復元 → route長Sort
            var routes = BuildForwardRoutesFromDisplayNodes(
                buildResult.DisplayNodes,
                buildResult.EndInfos);

            var sortedRoutes = SortForwardRoutesByLength(routes, true);

            // ⑤ 枝構造へ反映(並び替えは一旦あきらめる）
            //ApplySortedRoutesToDisplayNodes(buildResult.DisplayNodes, sortedRoutes);

            // ⑥ 反映後ダンプ
            var nodeDump = BuildDisplayLaneNodeDumpTable(buildResult.DisplayNodes);
            ExportDataTableToCsvSafe(nodeDump, "LotTrace_Debug_DisplayNodes_AfterSort", false);

            return buildResult;
        }


        //枝構造をPathRowに流し込む
        private void BuildPathRowsFromDisplayLaneNodes(
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
                // ------------------------------------------------------------
                AddForwardMiddleLotGroupInfosFromLaneNodes(row, laneNodes);

                // ------------------------------------------------------------
                // STEP2:
                // 行の代表情報を最低限埋める
                // ------------------------------------------------------------
                var firstNode = laneNodes.FirstOrDefault(x => x != null && x.SourceNode != null);
                var startBasisNode = row.StartNode ?? (firstNode == null ? null : firstNode.SourceNode);

                row.RootGroupKey = ResolvePathRowRootGroupKey(startBasisNode, laneNodes);
                row.StartTrunkGroupKey = BuildStartTrunkGroupKey(row.StartNode);
                row.StartTrunkOrder = row.PathOrder;

                // 必要ならここで署名を埋める
                row.FullPathSignature = BuildPathRowFullPathSignature(row);

                // ------------------------------------------------------------
                // STEP3:
                // EndInfo を正として EndNode を最後に確定する
                // ------------------------------------------------------------
                ApplyEndNodeToPathRow(row, laneNodes, laneBuildResult.EndInfos);

                // ★追加:
                // EndNode確定後、サービス側で確定済みの重複情報だけを転写する
                row.EndIsDuplicate = false;
                row.EndDuplicateDisplayGroupIndex = null;

                var endDisplayNodeKey = FindDisplayNodeKeyForNodeInLane(row.EndNode, null, laneNodes);
                if (!string.IsNullOrWhiteSpace(endDisplayNodeKey))
                {
                    EndDisplayNodeInfo endInfo;
                    if (endInfoByDisplayNodeKey.TryGetValue(endDisplayNodeKey, out endInfo) &&
                        endInfo != null)
                    {
                        row.EndIsDuplicate = endInfo.IsDuplicate;
                        row.EndDuplicateDisplayGroupIndex = endInfo.DuplicateDisplayIndex;
                    }
                }


                // ------------------------------------------------------------
                // STEP4:
                // End に採用した node を Middle から除去
                // ------------------------------------------------------------
                RemoveEndNodeFromMiddleNodes(row, laneNodes, laneBuildResult.EndInfos);

                TrimTrailingNullMiddleNodes(row);

                result.PathRows.Add(row);
            }

            ExportPathRowsFromDisplayLaneNodesDebugCsv(
                result.PathRows,
                laneBuildResult,
                "LotTrace_Debug_PathRows_FromDisplayLaneNodes");
        }

        private void ApplyEndNodeToPathRow(
    TracePathRow row,
    List<DisplayLaneNode> laneNodes,
    List<EndDisplayNodeInfo> endInfos)
        {
            if (row == null || laneNodes == null || laneNodes.Count == 0)
                return;

            if (endInfos == null || endInfos.Count == 0)
                return;

            // この lane に対応する終点列 X を求める
            // EndInfo.DisplayNodeKey 文字列は使わず、EndXLevel を正として使う
            var laneEndXLevels = endInfos
                .Where(x => x != null)
                .Select(x => x.EndXLevel)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            if (laneEndXLevels.Count == 0)
                return;

            // laneNodes 側で実在する終点列候補を探す
            var endCandidates = laneNodes
                .Where(x => x != null && x.SourceNode != null)
                .Where(x => laneEndXLevels.Contains(x.XLevel))
                .OrderByDescending(x => x.XLevel)
                .ThenByDescending(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (endCandidates.Count == 0)
                return;

            // 一番右の列を終点とみなす
            var selectedEndNode = endCandidates.First();
            row.EndNode = selectedEndNode.SourceNode;
        }

        private void RemoveEndNodeFromMiddleNodes(
            TracePathRow row,
            List<DisplayLaneNode> laneNodes,
            List<EndDisplayNodeInfo> endInfos)
        {
            if (row == null || row.EndNode == null)
                return;

            if (row.MiddleNodes == null || row.MiddleNodes.Count == 0)
                return;

            if (laneNodes == null || laneNodes.Count == 0)
                return;

            if (endInfos == null || endInfos.Count == 0)
                return;

            string endNodeKey = GetNodeIdentityKey(row.EndNode) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(endNodeKey))
                return;

            var laneEndXLevels = endInfos
                .Where(x => x != null)
                .Select(x => x.EndXLevel)
                .Distinct()
                .ToList();

            foreach (var laneNode in laneNodes)
            {
                if (laneNode == null || laneNode.SourceNode == null)
                    continue;

                if (!laneEndXLevels.Contains(laneNode.XLevel))
                    continue;

                string laneNodeKey = GetNodeIdentityKey(laneNode.SourceNode) ?? string.Empty;
                if (!string.Equals(laneNodeKey, endNodeKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                int middleIndex = laneNode.XLevel - 1;
                if (middleIndex < 0 || middleIndex >= row.MiddleNodes.Count)
                    continue;

                if (object.ReferenceEquals(row.MiddleNodes[middleIndex], row.EndNode))
                {
                    row.MiddleNodes[middleIndex] = null;
                }
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

        private string ResolvePathRowRootGroupKey(
            ProductionResultNode startBasisNode,
            List<DisplayLaneNode> laneNodes)
        {
            if (startBasisNode != null)
            {
                string key = GetNodeIdentityKey(startBasisNode);
                if (!string.IsNullOrWhiteSpace(key))
                    return key;
            }

            if (laneNodes == null || laneNodes.Count == 0)
                return null;

            var firstNode = laneNodes.FirstOrDefault(x => x != null && x.SourceNode != null);
            if (firstNode == null || firstNode.SourceNode == null)
                return null;

            return GetNodeIdentityKey(firstNode.SourceNode);
        }

        private string BuildPathRowFullPathSignature(TracePathRow row)
        {
            if (row == null)
                return null;

            var parts = new List<string>();

            if (row.StartNode != null)
            {
                parts.Add("S:" + (GetNodeIdentityKey(row.StartNode) ?? string.Empty));
            }

            if (row.MiddleNodes != null && row.MiddleNodes.Count > 0)
            {
                for (int i = 0; i < row.MiddleNodes.Count; i++)
                {
                    var node = row.MiddleNodes[i];
                    if (node == null)
                    {
                        parts.Add("M" + i.ToString() + ":");
                    }
                    else
                    {
                        parts.Add("M" + i.ToString() + ":" + (GetNodeIdentityKey(node) ?? string.Empty));
                    }
                }
            }

            if (row.EndNode != null)
            {
                parts.Add("E:" + (GetNodeIdentityKey(row.EndNode) ?? string.Empty));
            }

            return string.Join("|", parts);
        }

        private void ExportForwardDisplayLaneNodesDebugCsv(
    List<DisplayLaneNode> displayNodes,
    List<EndDisplayNodeInfo> endInfos,
    string fileName)
        {
            if (displayNodes == null)
                return;

            var endInfoMap = (endInfos ?? new List<EndDisplayNodeInfo>())
                .GroupBy(x => x.DisplayNodeKey ?? string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.First(),
                    StringComparer.OrdinalIgnoreCase);

            var lines = new List<string>();
            lines.Add(string.Join(",",
                EscapeCsv("DisplayNodeKey"),
                EscapeCsv("ParentDisplayNodeKey"),
                EscapeCsv("MergeKey"),
                EscapeCsv("XLevel"),
                EscapeCsv("YLane"),
                EscapeCsv("ChildIndex"),
                EscapeCsv("SubtreeLaneSpan"),
                EscapeCsv("LotNumber"),
                EscapeCsv("ControlMasterKey"),
                EscapeCsv("IsTerminal"),
                EscapeCsv("EndGroupKey"),
                EscapeCsv("EndGroupIndex"),
                EscapeCsv("End_IsFirstOfGroup"),
                EscapeCsv("End_IsDuplicate"),
                EscapeCsv("OriginalXLevel"),
                EscapeCsv("EndXLevel")));

            foreach (var node in displayNodes.OrderBy(x => x.YLane).ThenBy(x => x.XLevel))
            {
                if (node == null)
                    continue;

                EndDisplayNodeInfo endInfo = null;
                endInfoMap.TryGetValue(node.DisplayNodeKey ?? string.Empty, out endInfo);

                lines.Add(string.Join(",",
                    EscapeCsv(node.DisplayNodeKey),
                    EscapeCsv(node.ParentDisplayNodeKey),
                    EscapeCsv(node.MergeKey),
                    EscapeCsv(node.XLevel.ToString()),
                    EscapeCsv(node.YLane.ToString()),
                    EscapeCsv(node.ChildIndex.ToString()),
                    EscapeCsv(node.SubtreeLaneSpan.ToString()),
                    EscapeCsv(node.SourceNode == null ? null : node.SourceNode.LotNumber),
                    EscapeCsv(node.SourceNode == null ? null : node.SourceNode.ControlMasterKey),
                    EscapeCsv(IsTerminalDisplayNode(node) ? "1" : "0"),
                    EscapeCsv(endInfo == null ? null : endInfo.EndGroupKey),
                    EscapeCsv(endInfo == null ? null : endInfo.EndGroupIndex.ToString()),
                    EscapeCsv(endInfo != null && endInfo.IsFirstOfGroup ? "1" : "0"),
                    EscapeCsv(endInfo != null && endInfo.IsDuplicate ? "1" : "0"),
                    EscapeCsv(endInfo == null ? null : endInfo.OriginalXLevel.ToString()),
                    EscapeCsv(endInfo == null ? null : endInfo.EndXLevel.ToString())));
            }

            WriteDebugCsvLines(fileName, lines);
        }

        private List<DisplayLaneNode> BuildForwardDisplayLaneNodes(TraceResult result)
        {
            var output = new List<DisplayLaneNode>();

            if (result == null || result.RootNodes == null || result.RootNodes.Count == 0)
                return output;

            var lotGroups = result.RootNodes
                .Where(x => x != null)
                .GroupBy(x => NormalizeDisplayLaneLot(x.LotNumber) ?? string.Empty)
                .OrderBy(g => g.Key)
                .ToList();

            int globalY = 0;

            foreach (var lotGroup in lotGroups)
            {
                int groupStartIndex = output.Count;

                var roots = lotGroup.ToList();
                if (roots.Count == 0)
                    continue;

                int groupBaseY = globalY;

                var parentDisplayNodes = new List<DisplayLaneNode>();

                // Lv0配置
                for (int i = 0; i < roots.Count; i++)
                {
                    var root = roots[i];
                    int y = groupBaseY + i;

                    var node = new DisplayLaneNode
                    {
                        DisplayNodeKey = BuildDisplayLaneNodeKey(root, 0, y),
                        MergeKey = GetNodeMergeKey(root),
                        ParentDisplayNodeKey = null,
                        SourceNode = root,
                        XLevel = 0,
                        YLane = y,
                        ChildIndex = i,
                        SubtreeLaneSpan = 1,

                        IsLotGroupRepresentative = (i == 0),
                        IsLotGroupNonRepresentative = (i != 0),
                        
                    };

                    parentDisplayNodes.Add(node);
                    output.Add(node);
                }

                var representativeParent = roots[0];
                var representativeDisplayNode = parentDisplayNodes[0];

                var range = PlaceForwardBranchRecursive(
                    representativeParent,
                    representativeDisplayNode,
                    0,
                    representativeDisplayNode.YLane,
                    output);

                representativeDisplayNode.SubtreeLaneSpan = range.LaneSpan;

                int groupMaxY = -1;

                for (int i = groupStartIndex; i < output.Count; i++)
                {
                    var n = output[i];
                    if (n != null && n.YLane > groupMaxY)
                        groupMaxY = n.YLane;
                }

                //int rootGroupLastY = groupBaseY + roots.Count - 1;
                //if (groupMaxY < rootGroupLastY)
                //    groupMaxY = rootGroupLastY;

                globalY = groupMaxY + 1;
            }

            // ★DEBUG: 木構築完了時点ダンプ（終点整理前）
            DumpForwardDisplayLaneNodesBeforeEndNormalize(output);

            return output;
        }

        private BranchLaneRange PlaceForwardBranchRecursive(
    ProductionResultNode parentSourceNode,
    DisplayLaneNode parentDisplayNode,
    int parentXLevel,
    int startY,
    List<DisplayLaneNode> output)
        {
            if (parentSourceNode == null)
            {
                return new BranchLaneRange { FirstY = startY, LastY = startY, LaneSpan = 1 };
            }

            var children = parentSourceNode.ChildNodes
                .Where(x => x != null)
                .OrderBy(x => NormalizeDisplayLaneLot(x.LotNumber) ?? string.Empty)
                .ThenBy(x => GetNodeMergeKey(x) ?? string.Empty)
                .ToList();

            if (children == null || children.Count == 0)
            {
                parentDisplayNode.SubtreeLaneSpan = 1;

                return new BranchLaneRange
                {
                    FirstY = startY,
                    LastY = startY,
                    LaneSpan = 1
                };
            }

            var lotGroups = new List<List<ProductionResultNode>>();
            var lotGroupMap = new Dictionary<string, List<ProductionResultNode>>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (child == null)
                    continue;

                string lotKey = NormalizeDisplayLaneLot(child.LotNumber) ?? string.Empty;

                List<ProductionResultNode> group;
                if (!lotGroupMap.TryGetValue(lotKey, out group))
                {
                    group = new List<ProductionResultNode>();
                    lotGroupMap[lotKey] = group;
                    lotGroups.Add(group);
                }

                group.Add(child);
            }

            if (lotGroups.Count == 0)
            {
                parentDisplayNode.SubtreeLaneSpan = 1;

                return new BranchLaneRange
                {
                    FirstY = startY,
                    LastY = startY,
                    LaneSpan = 1
                };
            }

            int nextBaseY = startY;
            int firstY = int.MaxValue;
            int lastY = int.MinValue;
            int nextChildOrder = 0;

            foreach (var groupedChildren in lotGroups)
            {
                if (groupedChildren == null || groupedChildren.Count == 0)
                    continue;

                int groupStartY = nextBaseY;
                int x = parentXLevel + 1;

                var groupDisplayNodes = new List<DisplayLaneNode>();

                // 同一LotNo子Nodeを全部表示
                for (int i = 0; i < groupedChildren.Count; i++)
                {
                    var child = groupedChildren[i];
                    int y = groupStartY + i;

                    var childNode = new DisplayLaneNode
                    {
                        DisplayNodeKey = BuildDisplayLaneNodeKey(child, x, y),
                        MergeKey = GetNodeMergeKey(child),
                        ParentDisplayNodeKey = parentDisplayNode.DisplayNodeKey,
                        SourceNode = child,
                        XLevel = x,
                        YLane = y,
                        ChildIndex = i,
                        SubtreeLaneSpan = 1,

                        IsLotGroupRepresentative = false,
                        IsLotGroupNonRepresentative = false,
                        
                    };

                    output.Add(childNode);
                    groupDisplayNodes.Add(childNode);
                }

                // 代表1本だけ下流展開
                int representativeIndex = SelectRepresentativeChildIndex(groupedChildren);
                if (representativeIndex < 0 || representativeIndex >= groupedChildren.Count)
                    representativeIndex = 0;

                var representativeChild = groupedChildren[representativeIndex];
                var representativeDisplayNode = groupDisplayNodes[representativeIndex];

                for (int i = 0; i < groupDisplayNodes.Count; i++)
                {
                    groupDisplayNodes[i].IsLotGroupRepresentative = (i == representativeIndex);
                    groupDisplayNodes[i].IsLotGroupNonRepresentative = (i != representativeIndex);
                }

                // 親→同Lot子Node のエッジは全件作る
                for (int i = 0; i < groupedChildren.Count; i++)
                {
                    var child = groupedChildren[i];
                    var childNode = groupDisplayNodes[i];

                    var edge = new DisplayLaneEdge
                    {
                        EdgeIdentityKey = BuildDisplayLaneEdgeKey(parentSourceNode, child, x, nextChildOrder),
                        ParentDisplayNodeKey = parentDisplayNode.DisplayNodeKey,
                        ChildDisplayNodeKey = childNode.DisplayNodeKey,
                        ParentMergeKey = GetNodeMergeKey(parentSourceNode),
                        ChildMergeKey = GetNodeMergeKey(child),
                        FromXLevel = parentXLevel,
                        ToXLevel = x,
                        FromYLane = parentDisplayNode.YLane,
                        ToYLane = childNode.YLane,
                        ChildIndex = i,
                        LotGroupKey = NormalizeDisplayLaneLot(child.LotNumber)
                    };

                    edge.EdgeContextMatrix.Add(new List<string> { edge.ParentMergeKey ?? "" });
                    edge.EdgeContextMatrix.Add(new List<string> { edge.ChildMergeKey ?? "" });

                    parentDisplayNode.OutgoingEdges.Add(edge);
                    nextChildOrder++;
                }

                var representativeRange = PlaceForwardBranchRecursive(
                    representativeChild,
                    representativeDisplayNode,
                    x,
                    representativeDisplayNode.YLane,
                    output);

                representativeDisplayNode.SubtreeLaneSpan = representativeRange.LaneSpan;

                int groupLastY = groupStartY + groupedChildren.Count - 1;
                if (representativeRange.LastY > groupLastY)
                    groupLastY = representativeRange.LastY;

                if (groupStartY < firstY) firstY = groupStartY;
                if (groupLastY > lastY) lastY = groupLastY;

                nextBaseY = groupLastY + 1;
            }

            if (firstY == int.MaxValue)
                firstY = startY;

            if (lastY == int.MinValue)
                lastY = startY;

            int span = lastY - firstY + 1;
            if (span <= 0)
                span = 1;

            parentDisplayNode.SubtreeLaneSpan = span;

            return new BranchLaneRange
            {
                FirstY = firstY,
                LastY = lastY,
                LaneSpan = span
            };
        }


        private int SelectRepresentativeChildIndex(List<ProductionResultNode> groupedChildren)
        {
            if (groupedChildren == null || groupedChildren.Count == 0)
                return -1;

            for (int i = 0; i < groupedChildren.Count; i++)
            {
                if (groupedChildren[i] != null)
                    return i;
            }

            return 0;
        }

        

        private List<EndDisplayNodeInfo> FinalizeForwardEndNodeGroupsAndNormalizeX(List<DisplayLaneNode> displayNodes)
        {
            var output = new List<EndDisplayNodeInfo>();

            if (displayNodes == null || displayNodes.Count == 0)
                return output;

            // ------------------------------------------------------------
            // STEP1: 終点Node抽出
            // ------------------------------------------------------------
            var terminalNodes = displayNodes
                .Where(x => IsTerminalDisplayNode(x))
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.XLevel)
                .ToList();

            if (terminalNodes.Count == 0)
                return output;

            // ------------------------------------------------------------
            // STEP2: 終点共通X列を決定
            // 実装上の X=E は int の共通終点列として扱う
            // 非終点の最大Xのさらに右に寄せる
            // ------------------------------------------------------------
            int maxNonTerminalX = displayNodes
                .Where(x => x != null && !IsTerminalDisplayNode(x))
                .Select(x => x.XLevel)
                .DefaultIfEmpty(0)
                .Max();

            int endXLevel = maxNonTerminalX + 1;

            // ------------------------------------------------------------
            // STEP3: 終点Nodeをグループ化
            // グループキーは GetNodeMergeKey(SourceNode) ベース
            // ------------------------------------------------------------
            var grouped = terminalNodes
                .GroupBy(x => BuildEndGroupKey(x))
                .OrderBy(g => g.Min(n => n.YLane))
                .ThenBy(g => g.Key ?? string.Empty)
                .ToList();




            int groupIndex = 1;

            // ★追加
            int nextDuplicateDisplayIndex = 1;
            foreach (var group in grouped)
            {
                var orderedGroupNodes = group
                    .Where(x => x != null)
                    .OrderBy(x => x.YLane)
                    .ThenBy(x => x.XLevel)
                    .ToList();

                // ★追加
                int? duplicateDisplayIndex = orderedGroupNodes.Count >= 2
                    ? (int?)nextDuplicateDisplayIndex++
                    : null;


                for (int i = 0; i < orderedGroupNodes.Count; i++)
                {
                    var node = orderedGroupNodes[i];
                    if (node == null)
                        continue;

                    int oldXLevel = node.XLevel;
                    string oldDisplayNodeKey = node.DisplayNodeKey;

                    // --------------------------------------------
                    // 終点Xを共通列へ移動
                    // --------------------------------------------
                    node.XLevel = endXLevel;
                    node.DisplayNodeKey = BuildDisplayLaneNodeKey(node.SourceNode, node.XLevel, node.YLane);

                    // 親→終点 edge の参照先も更新
                    UpdateEndNodeKeyAndIncomingEdgesXOnly(
                        displayNodes,
                        oldDisplayNodeKey,
                        node.DisplayNodeKey,
                        endXLevel);

                    // ★追加: 代表NodeのX移動に追従して、同一Lotグループ非代表Nodeも同期移動
                    string syncLot = NormalizeDisplayLaneLot(node.SourceNode == null ? null : node.SourceNode.LotNumber);
                    string syncParentDisplayNodeKey = node.ParentDisplayNodeKey;

                    var syncTargets = displayNodes
                        .Where(x =>
                            x != null &&
                            !object.ReferenceEquals(x, node) &&
                            x.IsLotGroupNonRepresentative &&
                            string.Equals(
                                x.ParentDisplayNodeKey ?? string.Empty,
                                syncParentDisplayNodeKey ?? string.Empty,
                                StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(
                                NormalizeDisplayLaneLot(x.SourceNode == null ? null : x.SourceNode.LotNumber) ?? string.Empty,
                                syncLot ?? string.Empty,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var syncNode in syncTargets)
                    {
                        string syncOldDisplayNodeKey = syncNode.DisplayNodeKey;

                        syncNode.XLevel = endXLevel;
                        syncNode.DisplayNodeKey = BuildDisplayLaneNodeKey(
                            syncNode.SourceNode,
                            syncNode.XLevel,
                            syncNode.YLane);

                        UpdateEndNodeKeyAndIncomingEdgesXOnly(
                            displayNodes,
                            syncOldDisplayNodeKey,
                            syncNode.DisplayNodeKey,
                            endXLevel);
                    }

                    var info = new EndDisplayNodeInfo
                    {
                        DisplayNodeKey = node.DisplayNodeKey,
                        MergeKey = node.MergeKey,
                        EndGroupKey = group.Key ?? string.Empty,
                        EndGroupIndex = groupIndex,
                        IsFirstOfGroup = (i == 0),
                        IsDuplicate = (i > 0),
                        OriginalXLevel = oldXLevel,
                        EndXLevel = endXLevel,
                        YLane = node.YLane,
                        LotNumber = node.SourceNode == null ? null : node.SourceNode.LotNumber,
                        ControlMasterKey = node.SourceNode == null ? null : node.SourceNode.ControlMasterKey,
                        // ★追加
                        DuplicateDisplayIndex = (i > 0) ? duplicateDisplayIndex : null,

                    };

                    output.Add(info);
                }

                groupIndex++;
            }

            return output;
        }

        private void UpdateEndNodeKeyAndIncomingEdgesXOnly(
    List<DisplayLaneNode> displayNodes,
    string oldDisplayNodeKey,
    string newDisplayNodeKey,
    int endXLevel)
        {
            if (displayNodes == null || string.IsNullOrEmpty(oldDisplayNodeKey) || string.IsNullOrEmpty(newDisplayNodeKey))
                return;

            foreach (var node in displayNodes)
            {
                if (node == null || node.OutgoingEdges == null || node.OutgoingEdges.Count == 0)
                    continue;

                foreach (var edge in node.OutgoingEdges)
                {
                    if (edge == null)
                        continue;

                    if (!string.Equals(edge.ChildDisplayNodeKey, oldDisplayNodeKey, StringComparison.OrdinalIgnoreCase))
                        continue;

                    edge.ChildDisplayNodeKey = newDisplayNodeKey;
                    edge.ToXLevel = endXLevel;
                }
            }
        }

        private bool IsTerminalDisplayNode(DisplayLaneNode node)
        {
            if (node == null)
                return false;

            if (node.SourceNode == null)
                return false;

            // 非代表Lotは終点に行かせない
            if (node.IsLotGroupNonRepresentative)
                return false;

            if (node.SourceNode.IsTraceTerminal)
                return true;

            if (node.OutgoingEdges == null || node.OutgoingEdges.Count == 0)
                return true;

            if (node.SourceNode.ChildNodes == null || node.SourceNode.ChildNodes.Count == 0)
                return true;

            return false;
        }

        private string BuildEndGroupKey(DisplayLaneNode node)
        {
            if (node == null || node.SourceNode == null)
                return string.Empty;

            var mergeKey = GetNodeMergeKey(node.SourceNode);
            if (!string.IsNullOrWhiteSpace(mergeKey))
                return mergeKey;

            if (!string.IsNullOrWhiteSpace(node.SourceNode.ControlMasterKey))
                return "CMK|" + node.SourceNode.ControlMasterKey;

            if (!string.IsNullOrWhiteSpace(node.SourceNode.NodeIdentityKey))
                return "NID|" + node.SourceNode.NodeIdentityKey;

            return string.Empty;
        }

        private void UpdateIncomingEdgesForDisplayNodeKeyChange(
    List<DisplayLaneNode> displayNodes,
    string oldDisplayNodeKey,
    string newDisplayNodeKey,
    int newToXLevel)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            foreach (var node in displayNodes)
            {
                if (node == null || node.OutgoingEdges == null || node.OutgoingEdges.Count == 0)
                    continue;

                foreach (var edge in node.OutgoingEdges)
                {
                    if (edge == null)
                        continue;

                    if (!string.Equals(edge.ChildDisplayNodeKey, oldDisplayNodeKey, StringComparison.Ordinal))
                        continue;

                    edge.ChildDisplayNodeKey = newDisplayNodeKey;
                    edge.ToXLevel = newToXLevel;
                }
            }

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                if (string.Equals(node.ParentDisplayNodeKey, oldDisplayNodeKey, StringComparison.Ordinal))
                    node.ParentDisplayNodeKey = newDisplayNodeKey;
            }
        }



        private List<ForwardDisplayRoute> BuildForwardRoutesFromDisplayNodes(
    List<DisplayLaneNode> displayNodes,
    List<EndDisplayNodeInfo> endInfos)
        {
            var routes = new List<ForwardDisplayRoute>();

            if (displayNodes == null || displayNodes.Count == 0)
                return routes;

            var nodeByKey = displayNodes
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var endInfoByNodeKey = new Dictionary<string, EndDisplayNodeInfo>(StringComparer.OrdinalIgnoreCase);
            if (endInfos != null)
            {
                foreach (var info in endInfos)
                {
                    if (info == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(info.DisplayNodeKey))
                        continue;

                    if (!endInfoByNodeKey.ContainsKey(info.DisplayNodeKey))
                        endInfoByNodeKey.Add(info.DisplayNodeKey, info);
                }
            }

            var terminalNodes = GetTerminalDisplayNodes(displayNodes, endInfos);

            foreach (var terminalNode in terminalNodes)
            {
                if (terminalNode == null)
                    continue;

                EndDisplayNodeInfo endInfo = null;
                endInfoByNodeKey.TryGetValue(terminalNode.DisplayNodeKey, out endInfo);

                var route = BuildRouteFromTerminalNode(terminalNode, nodeByKey, endInfo);
                if (route == null)
                    continue;

                routes.Add(route);
            }

            return routes;
        }

        private ForwardDisplayRoute BuildRouteFromTerminalNode(
    DisplayLaneNode terminalNode,
    Dictionary<string, DisplayLaneNode> nodeByKey,
    EndDisplayNodeInfo endInfo)
        {
            if (terminalNode == null)
                return null;

            if (nodeByKey == null || nodeByKey.Count == 0)
                return null;

            var reverseNodes = new List<DisplayLaneNode>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var current = terminalNode;

            while (current != null)
            {
                if (!string.IsNullOrWhiteSpace(current.DisplayNodeKey))
                {
                    if (!visited.Add(current.DisplayNodeKey))
                    {
                        // ループ防止
                        break;
                    }
                }

                reverseNodes.Add(current);

                if (string.IsNullOrWhiteSpace(current.ParentDisplayNodeKey))
                    break;

                DisplayLaneNode parent;
                if (!nodeByKey.TryGetValue(current.ParentDisplayNodeKey, out parent))
                    break;

                current = parent;
            }

            reverseNodes.Reverse();

            if (reverseNodes.Count == 0)
                return null;

            var startNode = reverseNodes[0];
            var endNode = reverseNodes[reverseNodes.Count - 1];

            var pathMergeKeys = reverseNodes
                .Select(GetDisplayLaneNodeMergeKeySafe)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var route = new ForwardDisplayRoute();
            route.StartNode = startNode;
            route.EndNode = endNode;
            route.Nodes = reverseNodes;
            route.NodeCount = reverseNodes.Count;
            route.EdgeCount = reverseNodes.Count <= 0 ? 0 : reverseNodes.Count - 1;
            route.StartY = startNode.YLane;
            route.EndY = endNode.YLane;
            route.StartMergeKey = GetDisplayLaneNodeMergeKeySafe(startNode);
            route.StartLotNumber = GetDisplayLaneNodeLotNumberSafe(startNode);
            route.EndGroupKey = endInfo == null ? null : endInfo.EndGroupKey;
            route.EndGroupIndex = endInfo == null ? 0 : endInfo.EndGroupIndex;
            route.PathMergeKeyText = string.Join(" > ", pathMergeKeys);

            route.RouteKey =
                (route.StartMergeKey ?? string.Empty) + "||" +
                (route.StartLotNumber ?? string.Empty) + "||" +
                (route.EndGroupKey ?? string.Empty) + "||" +
                route.EndGroupIndex.ToString() + "||" +
                (route.PathMergeKeyText ?? string.Empty);

            return route;
        }

        private List<DisplayLaneNode> GetTerminalDisplayNodes(
    List<DisplayLaneNode> displayNodes,
    List<EndDisplayNodeInfo> endInfos)
        {
            var result = new List<DisplayLaneNode>();

            if (displayNodes == null || displayNodes.Count == 0)
                return result;

            if (endInfos != null && endInfos.Count > 0)
            {
                var nodeByKey = displayNodes
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                    .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                foreach (var info in endInfos)
                {
                    if (info == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(info.DisplayNodeKey))
                        continue;

                    DisplayLaneNode node;
                    if (nodeByKey.TryGetValue(info.DisplayNodeKey, out node))
                    {
                        result.Add(node);
                    }
                }

                return result;
            }

            // endInfos が無い場合の保険
            var parentKeys = new HashSet<string>(
                displayNodes
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.ParentDisplayNodeKey))
                    .Select(x => x.ParentDisplayNodeKey),
                StringComparer.OrdinalIgnoreCase);

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                if (string.IsNullOrWhiteSpace(node.DisplayNodeKey))
                    continue;

                if (!parentKeys.Contains(node.DisplayNodeKey))
                    result.Add(node);
            }

            return result;
        }

        private List<ForwardDisplayRoute> SortForwardRoutesByLength(
    List<ForwardDisplayRoute> routes,
    bool descending)
        {
            if (routes == null)
                return new List<ForwardDisplayRoute>();

            IOrderedEnumerable<ForwardDisplayRoute> ordered;

            if (descending)
            {
                ordered = routes
                    .OrderByDescending(x => x == null ? -1 : x.EdgeCount)
                    .ThenBy(x => x == null ? null : x.StartLotNumber, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x == null ? null : x.StartMergeKey, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x == null ? int.MaxValue : x.EndGroupIndex)
                    .ThenBy(x => x == null ? null : x.PathMergeKeyText, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                ordered = routes
                    .OrderBy(x => x == null ? int.MaxValue : x.EdgeCount)
                    .ThenBy(x => x == null ? null : x.StartLotNumber, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x == null ? null : x.StartMergeKey, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x == null ? int.MaxValue : x.EndGroupIndex)
                    .ThenBy(x => x == null ? null : x.PathMergeKeyText, StringComparer.OrdinalIgnoreCase);
            }

            return ordered.ToList();
        }

        private string GetDisplayLaneNodeMergeKeySafe(DisplayLaneNode node)
        {
            if (node == null)
                return null;

            return string.IsNullOrWhiteSpace(node.MergeKey) ? null : node.MergeKey;
        }

        private string GetDisplayLaneNodeLotNumberSafe(DisplayLaneNode node)
        {
            if (node == null)
                return null;

            if (node.SourceNode == null)
                return null;

            return string.IsNullOrWhiteSpace(node.SourceNode.LotNumber)
                ? null
                : node.SourceNode.LotNumber;
        }

        

        private void ApplySortedRoutesToDisplayNodes(
    List<DisplayLaneNode> displayNodes,
    List<ForwardDisplayRoute> sortedRoutes)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            // ------------------------------------------------------------
            // NOTE:
            // sortedRoutes はデバッグ／検証用として受けるが、
            // 実際の枝Sort・再配置は DisplayLaneNode 構造そのものを使って行う。
            //
            // 理由:
            //   - Sort対象は route ではなく branch
            //   - YLane は差分移動ではなく再確定が必要
            //   - SubtreeLaneSpan を再帰で再計算する必要がある
            // ------------------------------------------------------------

            var nodeByKey = displayNodes
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var childrenMap = BuildDisplayLaneChildrenMap(displayNodes);

            // 枝長メトリクスを全Nodeに対して計算
            var metricMap = new Dictionary<string, BranchSortMetric>(StringComparer.OrdinalIgnoreCase);

            var rootNodes = displayNodes
                .Where(x =>
                    x != null &&
                    x.XLevel == 0 &&
                    string.IsNullOrWhiteSpace(x.ParentDisplayNodeKey))
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var root in rootNodes)
            {
                ComputeBranchSortMetricRecursive(root, childrenMap, metricMap);
            }

            // ------------------------------------------------------------
            // Root(Lv0) は Lot単位グループを維持しながら上から再採番
            // 同一Lot始点群は連続配置し、代表(root[0])のみ枝を持つ前提
            // ------------------------------------------------------------
            var lotGroups = rootNodes
                .GroupBy(x => NormalizeDisplayLaneLot(GetDisplayLaneNodeLotNumberSafe(x)) ?? string.Empty)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int nextGlobalY = 0;

            foreach (var lotGroup in lotGroups)
            {
                var rootsInGroup = lotGroup
                    .Where(x => x != null)
                    .OrderBy(x => x.YLane)
                    .ThenBy(x => x.ChildIndex)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (rootsInGroup.Count == 0)
                    continue;

                // Lv0群を連続再配置
                for (int i = 0; i < rootsInGroup.Count; i++)
                {
                    var root = rootsInGroup[i];
                    root.YLane = nextGlobalY + i;
                    root.ChildIndex = i;
                    root.SubtreeLaneSpan = 1;
                }

                int groupMaxY = nextGlobalY + rootsInGroup.Count - 1;

                // 代表rootのみ枝を持つ
                var representativeRoot = rootsInGroup[0];

                BranchLaneRange range;
                if (HasDisplayLaneChildren(representativeRoot, childrenMap))
                {
                    range = RelayoutBranchRecursive(
                        representativeRoot,
                        representativeRoot.YLane,
                        childrenMap,
                        metricMap);
                }
                else
                {
                    representativeRoot.SubtreeLaneSpan = 1;
                    range = new BranchLaneRange
                    {
                        FirstY = representativeRoot.YLane,
                        LastY = representativeRoot.YLane,
                        LaneSpan = 1
                    };
                }

                if (range.LastY > groupMaxY)
                    groupMaxY = range.LastY;

                nextGlobalY = groupMaxY + 1;
            }
            SyncNonRepresentativeNodesToRepresentativeOrder(displayNodes, childrenMap);

            // 最後に key / 親参照 / edge座標 を全同期更新
            RefreshDisplayLaneTopologyAfterRelayout(displayNodes);
        }

        private void SyncNonRepresentativeNodesToRepresentativeOrder(
    List<DisplayLaneNode> displayNodes,
    Dictionary<string, List<DisplayLaneNode>> childrenMap)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            if (childrenMap == null || childrenMap.Count == 0)
                return;

            foreach (var pair in childrenMap)
            {
                var siblings = pair.Value;
                if (siblings == null || siblings.Count == 0)
                    continue;

                var lotGroups = siblings
                    .Where(x => x != null)
                    .GroupBy(x => NormalizeDisplayLaneLot(GetDisplayLaneNodeLotNumberSafe(x)) ?? string.Empty)
                    .ToList();

                foreach (var lotGroup in lotGroups)
                {
                    var groupNodes = lotGroup
                        .Where(x => x != null)
                        .OrderBy(x => x.YLane)
                        .ThenBy(x => x.ChildIndex)
                        .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (groupNodes.Count <= 1)
                        continue;

                    var representative = groupNodes.FirstOrDefault(x => x.IsLotGroupRepresentative);
                    if (representative == null)
                        continue;

                    int baseY = representative.YLane;
                    int childIndex = 0;

                    foreach (var node in groupNodes)
                    {
                        if (node == null)
                            continue;

                        node.YLane = baseY + childIndex;
                        node.ChildIndex = childIndex;
                        childIndex++;
                    }
                }
            }
        }

        private Dictionary<string, List<DisplayLaneNode>> BuildDisplayLaneChildrenMap(
    List<DisplayLaneNode> displayNodes)
        {
            var map = new Dictionary<string, List<DisplayLaneNode>>(StringComparer.OrdinalIgnoreCase);

            if (displayNodes == null || displayNodes.Count == 0)
                return map;

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                string parentKey = node.ParentDisplayNodeKey ?? string.Empty;

                List<DisplayLaneNode> bucket;
                if (!map.TryGetValue(parentKey, out bucket))
                {
                    bucket = new List<DisplayLaneNode>();
                    map[parentKey] = bucket;
                }

                bucket.Add(node);
            }

            foreach (var pair in map)
            {
                pair.Value.Sort(CompareDisplayLaneNodeStableOrder);
            }

            return map;
        }

        private bool HasDisplayLaneChildren(
            DisplayLaneNode parentNode,
            Dictionary<string, List<DisplayLaneNode>> childrenMap)
        {
            if (parentNode == null || childrenMap == null)
                return false;

            List<DisplayLaneNode> children;
            if (!childrenMap.TryGetValue(parentNode.DisplayNodeKey ?? string.Empty, out children))
                return false;

            return children != null && children.Count > 0;
        }

        private BranchSortMetric ComputeBranchSortMetricRecursive(
            DisplayLaneNode node,
            Dictionary<string, List<DisplayLaneNode>> childrenMap,
            Dictionary<string, BranchSortMetric> metricMap)
        {
            if (node == null)
                return new BranchSortMetric();

            string nodeKey = node.DisplayNodeKey ?? string.Empty;

            BranchSortMetric cached;
            if (metricMap != null && metricMap.TryGetValue(nodeKey, out cached))
                return cached;

            List<DisplayLaneNode> children;
            childrenMap.TryGetValue(nodeKey, out children);

            BranchSortMetric metric;

            if (children == null || children.Count == 0)
            {
                metric = new BranchSortMetric
                {
                    LongestEdgeCountToTerminal = 0,
                    LongestNodeCountToTerminal = 1,
                    MaxTerminalXLevel = node.XLevel,
                    SubtreeLeafCount = 1
                };
            }
            else
            {
                int maxEdgeCount = 0;
                int maxNodeCount = 1;
                int maxTerminalX = node.XLevel;
                int leafCount = 0;

                foreach (var child in children)
                {
                    if (child == null)
                        continue;

                    var childMetric = ComputeBranchSortMetricRecursive(child, childrenMap, metricMap);

                    if (childMetric.LongestEdgeCountToTerminal + 1 > maxEdgeCount)
                        maxEdgeCount = childMetric.LongestEdgeCountToTerminal + 1;

                    if (childMetric.LongestNodeCountToTerminal + 1 > maxNodeCount)
                        maxNodeCount = childMetric.LongestNodeCountToTerminal + 1;

                    if (childMetric.MaxTerminalXLevel > maxTerminalX)
                        maxTerminalX = childMetric.MaxTerminalXLevel;

                    leafCount += Math.Max(1, childMetric.SubtreeLeafCount);
                }

                metric = new BranchSortMetric
                {
                    LongestEdgeCountToTerminal = maxEdgeCount,
                    LongestNodeCountToTerminal = maxNodeCount,
                    MaxTerminalXLevel = maxTerminalX,
                    SubtreeLeafCount = Math.Max(1, leafCount)
                };
            }

            if (metricMap != null)
                metricMap[nodeKey] = metric;

            return metric;
        }

        private BranchLaneRange RelayoutBranchRecursive(
            DisplayLaneNode parentNode,
            int startY,
            Dictionary<string, List<DisplayLaneNode>> childrenMap,
            Dictionary<string, BranchSortMetric> metricMap)
        {
            if (parentNode == null)
            {
                return new BranchLaneRange
                {
                    FirstY = startY,
                    LastY = startY,
                    LaneSpan = 1
                };
            }

            string parentKey = parentNode.DisplayNodeKey ?? string.Empty;

            List<DisplayLaneNode> children;
            childrenMap.TryGetValue(parentKey, out children);

            if (children == null || children.Count == 0)
            {
                parentNode.SubtreeLaneSpan = 1;

                return new BranchLaneRange
                {
                    FirstY = startY,
                    LastY = startY,
                    LaneSpan = 1
                };
            }

            var orderedChildren = children
                .Where(x => x != null)
                .OrderByDescending(x => GetBranchLongestEdgeCount(metricMap, x))
                .ThenByDescending(x => GetBranchMaxTerminalX(metricMap, x))
                .ThenByDescending(x => GetBranchLeafCount(metricMap, x))
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.MergeKey, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int nextY = startY;
            int firstY = int.MaxValue;
            int lastY = int.MinValue;

            for (int i = 0; i < orderedChildren.Count; i++)
            {
                var child = orderedChildren[i];
                if (child == null)
                    continue;

                child.ChildIndex = i;
                child.YLane = nextY;

                var childRange = RelayoutBranchRecursive(
                    child,
                    child.YLane,
                    childrenMap,
                    metricMap);

                child.SubtreeLaneSpan = Math.Max(1, childRange.LaneSpan);

                if (childRange.FirstY < firstY)
                    firstY = childRange.FirstY;

                if (childRange.LastY > lastY)
                    lastY = childRange.LastY;

                nextY = childRange.LastY + 1;
            }

            if (firstY == int.MaxValue)
                firstY = startY;

            if (lastY == int.MinValue)
                lastY = startY;

            parentNode.SubtreeLaneSpan = lastY - firstY + 1;

            return new BranchLaneRange
            {
                FirstY = firstY,
                LastY = lastY,
                LaneSpan = parentNode.SubtreeLaneSpan
            };
        }

        private int GetBranchLongestEdgeCount(
            Dictionary<string, BranchSortMetric> metricMap,
            DisplayLaneNode node)
        {
            if (metricMap == null || node == null)
                return 0;

            BranchSortMetric metric;
            if (!metricMap.TryGetValue(node.DisplayNodeKey ?? string.Empty, out metric) || metric == null)
                return 0;

            return metric.LongestEdgeCountToTerminal;
        }

        private int GetBranchMaxTerminalX(
            Dictionary<string, BranchSortMetric> metricMap,
            DisplayLaneNode node)
        {
            if (metricMap == null || node == null)
                return node == null ? 0 : node.XLevel;

            BranchSortMetric metric;
            if (!metricMap.TryGetValue(node.DisplayNodeKey ?? string.Empty, out metric) || metric == null)
                return node.XLevel;

            return metric.MaxTerminalXLevel;
        }

        private int GetBranchLeafCount(
            Dictionary<string, BranchSortMetric> metricMap,
            DisplayLaneNode node)
        {
            if (metricMap == null || node == null)
                return 1;

            BranchSortMetric metric;
            if (!metricMap.TryGetValue(node.DisplayNodeKey ?? string.Empty, out metric) || metric == null)
                return 1;

            return metric.SubtreeLeafCount;
        }

        private int CompareDisplayLaneNodeStableOrder(DisplayLaneNode x, DisplayLaneNode y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            int byY = x.YLane.CompareTo(y.YLane);
            if (byY != 0)
                return byY;

            int byX = x.XLevel.CompareTo(y.XLevel);
            if (byX != 0)
                return byX;

            int byChildIndex = x.ChildIndex.CompareTo(y.ChildIndex);
            if (byChildIndex != 0)
                return byChildIndex;

            int byMerge = string.Compare(
                x.MergeKey ?? string.Empty,
                y.MergeKey ?? string.Empty,
                StringComparison.OrdinalIgnoreCase);
            if (byMerge != 0)
                return byMerge;

            return string.Compare(
                x.DisplayNodeKey ?? string.Empty,
                y.DisplayNodeKey ?? string.Empty,
                StringComparison.OrdinalIgnoreCase);
        }

        private void RefreshDisplayLaneTopologyAfterRelayout(List<DisplayLaneNode> displayNodes)
        {
            if (displayNodes == null || displayNodes.Count == 0)
                return;

            var oldToNewKeyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                string oldKey = node.DisplayNodeKey ?? string.Empty;
                string newKey = BuildDisplayLaneNodeKey(node.SourceNode, node.XLevel, node.YLane);

                oldToNewKeyMap[oldKey] = newKey;
            }

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                string oldKey = node.DisplayNodeKey ?? string.Empty;
                string newKey;
                if (oldToNewKeyMap.TryGetValue(oldKey, out newKey))
                {
                    node.DisplayNodeKey = newKey;
                }
            }

            var nodeByKey = displayNodes
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                if (string.IsNullOrWhiteSpace(node.ParentDisplayNodeKey))
                    continue;

                string mapped;
                if (oldToNewKeyMap.TryGetValue(node.ParentDisplayNodeKey, out mapped))
                {
                    node.ParentDisplayNodeKey = mapped;
                }
            }

            foreach (var parent in displayNodes)
            {
                if (parent == null || parent.OutgoingEdges == null || parent.OutgoingEdges.Count == 0)
                    continue;

                foreach (var edge in parent.OutgoingEdges)
                {
                    if (edge == null)
                        continue;

                    string mappedParentKey;
                    if (oldToNewKeyMap.TryGetValue(edge.ParentDisplayNodeKey ?? string.Empty, out mappedParentKey))
                    {
                        edge.ParentDisplayNodeKey = mappedParentKey;
                    }

                    string mappedChildKey;
                    if (oldToNewKeyMap.TryGetValue(edge.ChildDisplayNodeKey ?? string.Empty, out mappedChildKey))
                    {
                        edge.ChildDisplayNodeKey = mappedChildKey;
                    }

                    edge.FromXLevel = parent.XLevel;
                    edge.FromYLane = parent.YLane;

                    DisplayLaneNode childNode;
                    if (nodeByKey.TryGetValue(edge.ChildDisplayNodeKey ?? string.Empty, out childNode) && childNode != null)
                    {
                        edge.ToXLevel = childNode.XLevel;
                        edge.ToYLane = childNode.YLane;
                    }
                }

                var reorderedEdges = parent.OutgoingEdges
                    .Where(x => x != null)
                    .OrderBy(x => x.ChildIndex)
                    .ThenBy(x => x.ChildMergeKey ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                parent.OutgoingEdges.Clear();
                parent.OutgoingEdges.AddRange(reorderedEdges);
            }
        }

        private sealed class BranchSortMetric
        {
            public int LongestEdgeCountToTerminal { get; set; }
            public int LongestNodeCountToTerminal { get; set; }
            public int MaxTerminalXLevel { get; set; }
            public int SubtreeLeafCount { get; set; }
        }

        private sealed class BranchLaneRange
        {
            public int FirstY { get; set; }
            public int LastY { get; set; }
            public int LaneSpan { get; set; }
        }


        private sealed class ForwardDisplayRoute
        {
            public string RouteKey { get; set; }

            public DisplayLaneNode StartNode { get; set; }
            public DisplayLaneNode EndNode { get; set; }

            public List<DisplayLaneNode> Nodes { get; set; }

            public int NodeCount { get; set; }
            public int EdgeCount { get; set; }

            public int StartX { get; set; }
            public int EndX { get; set; }
            public int StartY { get; set; }
            public int EndY { get; set; }

            public string StartMergeKey { get; set; }
            public string StartLotNumber { get; set; }

            public string EndMergeKey { get; set; }
            public string EndGroupKey { get; set; }
            public int EndGroupIndex { get; set; }

            public string PathMergeKeyText { get; set; }
        }



        private sealed class EndDisplayNodeInfo
        {
            public string DisplayNodeKey { get; set; }
            public string MergeKey { get; set; }

            public string EndGroupKey { get; set; }
            public int EndGroupIndex { get; set; }
            public bool IsFirstOfGroup { get; set; }
            public bool IsDuplicate { get; set; }

            // ★追加
            public int? DuplicateDisplayIndex { get; set; }

            public int OriginalXLevel { get; set; }
            public int EndXLevel { get; set; }
            public int YLane { get; set; }

            public string LotNumber { get; set; }
            public string ControlMasterKey { get; set; }
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
            string mergeKey = GetNodeMergeKey(node) ?? string.Empty;
            return string.Join("|",
                "DISPLAYNODE",
                xLevel.ToString(),
                yLane.ToString(),
                mergeKey);
        }

        private string BuildDisplayLaneEdgeKey(
            ProductionResultNode parent,
            ProductionResultNode child,
            int toLevel,
            int childIndex)
        {
            string parentMergeKey = GetNodeMergeKey(parent) ?? string.Empty;
            string childMergeKey = GetNodeMergeKey(child) ?? string.Empty;
            string lotKey = NormalizeDisplayLaneLot(child == null ? null : child.LotNumber) ?? string.Empty;

            return string.Join("|",
                "DISPLAYEDGE",
                "LV" + toLevel.ToString(),
                "IDX" + childIndex.ToString(),
                parentMergeKey,
                childMergeKey,
                lotKey);
        }

        private string NormalizeDisplayLaneLot(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                return null;

            return lotNumber.Trim().ToUpperInvariant();
        }

        #endregion

        #region 新罫線メソッド群


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

        private List<OccupiedLotGroupRange> NormalizeOccupiedRangeByLotGroup(List<DisplayLaneNode> nodes)
        {
            var result = new List<OccupiedLotGroupRange>();

            if (nodes == null || nodes.Count == 0)
                return result;

            // ============================================================
            // STEP1:
            // 罫線用に有効なNodeだけを対象とする
            // - DisplayNodeKey がある
            // - LotGroupKey がある
            // - Occupied が設定済み
            // ============================================================
            var validNodes = nodes
                .Where(n =>
                    n != null &&
                    !string.IsNullOrWhiteSpace(n.DisplayNodeKey) &&
                    !string.IsNullOrWhiteSpace(n.LotGroupKey) &&
                    n.OccupiedFirstY >= 0 &&
                    n.OccupiedLastY >= 0)
                .ToList();

            if (validNodes.Count == 0)
                return result;

            // ============================================================
            // STEP2:
            // 同一
            //   Lv
            //   ParentDisplayNodeKey
            //   LotGroupKey
            // ごとに Occupied を集約する
            // ============================================================
            var groups = validNodes
                .GroupBy(n => new
                {
                    Level = n.XLevel,
                    ParentDisplayNodeKey = n.ParentDisplayNodeKey ?? string.Empty,
                    LotGroupKey = n.LotGroupKey ?? string.Empty
                })
                .ToList();

            // ============================================================
            // STEP3:
            // Group単位で Occupied の min/max を求める
            // ============================================================
            foreach (var group in groups)
            {
                int occupiedFirstY = group.Min(x => x.OccupiedFirstY);
                int occupiedLastY = group.Max(x => x.OccupiedLastY);

                result.Add(new OccupiedLotGroupRange
                {
                    Level = group.Key.Level,
                    ParentDisplayNodeKey = group.Key.ParentDisplayNodeKey,
                    LotGroupKey = group.Key.LotGroupKey,
                    OccupiedFirstY = occupiedFirstY,
                    OccupiedLastY = occupiedLastY
                });
            }

            return result;
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
                    x.Level > 0 &&
                    x.OccupiedLastY >= 0)
                .ToList();

            if (validGroups.Count == 0)
                return result;

            // 完成木構造上に実在する XLevel 群だけを使う
            var usedLevels = validGroups
                .Select(x => x.Level)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (usedLevels.Count == 0)
                return result;

            // 最右の実在列を End とみなす
            int endX = usedLevels[usedLevels.Count - 1];

            // Middle の最終列は「endX 未満で実在する最大X」
            int middleMaxX = usedLevels
                .Where(x => x < endX)
                .DefaultIfEmpty(-1)
                .Max();

            foreach (var group in validGroups
                .OrderBy(x => x.Level)
                .ThenBy(x => x.OccupiedLastY)
                .ThenBy(x => x.ParentDisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LotGroupKey, StringComparer.OrdinalIgnoreCase))
            {
                bool isEndGroup = group.Level == endX;

                // Middle 線なのに伸ばす先の Middle 列が存在しない場合は作らない
                if (!isEndGroup && middleMaxX < group.Level)
                    continue;

                result.Add(new TraceLineRange
                {
                    GridKind = isEndGroup ? "End" : "Middle",
                    LineKind = group.Level == 1 ? "Trunk" : "Branch",
                    Level = group.Level,
                    StartRowIndex = group.OccupiedLastY,
                    EndRowIndex = group.OccupiedLastY,
                    FromXLevel = group.Level,
                    ToXLevel = isEndGroup ? group.Level : middleMaxX
                });
            }

            DeduplicateTraceLineRanges(result);
            return result;
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

        private sealed class SyntheticMiddleCandidate
        {
            public int RowIndex { get; set; }
            public int EndXLevel { get; set; }
        }

        private List<Tuple<int, int>> BuildContiguousRowBlocksByEndXLevel(
            IList<SyntheticMiddleCandidate> candidates)
        {
            var result = new List<Tuple<int, int>>();

            if (candidates == null || candidates.Count == 0)
                return result;

            var groups = candidates
                .Where(x => x != null)
                .GroupBy(x => x.EndXLevel)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var group in groups)
            {
                var rows = group
                    .Select(x => x.RowIndex)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                if (rows.Count == 0)
                    continue;

                int blockStart = rows[0];
                int blockEnd = rows[0];

                for (int i = 1; i < rows.Count; i++)
                {
                    int row = rows[i];

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
            }

            return result;
        }

        private string BuildSyntheticBranchKeyFromDisplayNode(DisplayLaneNode node)
        {
            if (node == null)
                return null;

            return (node.ParentDisplayNodeKey ?? string.Empty)
                 + "|"
                 + (node.LotGroupKey ?? string.Empty);
        }

        private string BuildSyntheticBranchKeyFromOccupiedRange(OccupiedLotGroupRange range)
        {
            if (range == null)
                return null;

            return (range.ParentDisplayNodeKey ?? string.Empty)
                 + "|"
                 + (range.LotGroupKey ?? string.Empty);
        }

        private string BuildBranchKey(OccupiedLotGroupRange range)
        {
            if (range == null)
                return null;

            return (range.ParentDisplayNodeKey ?? string.Empty)
                 + "|"
                 + (range.LotGroupKey ?? string.Empty);
        }

        private HashSet<string> BuildMiddleOccupiedBranchKeySet(
    IList<OccupiedLotGroupRange> occupiedRanges)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (occupiedRanges == null)
                return set;

            foreach (var r in occupiedRanges)
            {
                if (r == null)
                    continue;

                if (r.Level <= 0)
                    continue;

                var key =
                    (r.ParentDisplayNodeKey ?? string.Empty)
                    + "|"
                    + (r.LotGroupKey ?? string.Empty);

                set.Add(key);
            }

            return set;
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

        private bool HasEffectiveMiddleNodeOnLane(
    IList<DisplayLaneNode> laneNodes,
    DisplayLaneNode endNode)
        {
            if (laneNodes == null || laneNodes.Count == 0)
                return false;

            foreach (var node in laneNodes)
            {
                if (node == null)
                    continue;

                // Start は除外
                if (node.XLevel <= 0)
                    continue;

                // 今回の終点node自身は除外
                if (object.ReferenceEquals(node, endNode))
                    continue;

                // XLevel>=1 で終点以外の実nodeがあれば Middle 実体あり
                return true;
            }

            return false;
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
        

        private bool HasStartNodeInLaneGroup(IList<DisplayLaneNode> laneNodes)
        {
            if (laneNodes == null || laneNodes.Count == 0)
                return false;

            foreach (var node in laneNodes)
            {
                if (node == null)
                    continue;

                if (node.XLevel == 0)
                    return true;
            }

            return false;
        }

        private bool HasEndNodeInLaneGroup(
            IList<DisplayLaneNode> laneNodes,
            ISet<string> endNodeKeys)
        {
            if (laneNodes == null || laneNodes.Count == 0)
                return false;

            if (endNodeKeys == null || endNodeKeys.Count == 0)
                return false;

            foreach (var node in laneNodes)
            {
                if (node == null)
                    continue;

                if (string.IsNullOrWhiteSpace(node.DisplayNodeKey))
                    continue;

                if (endNodeKeys.Contains(node.DisplayNodeKey))
                    return true;
            }

            return false;
        }

        private bool HasAnyEffectiveMiddleNodeInLaneGroup(
            IList<DisplayLaneNode> laneNodes,
            ISet<string> endNodeKeys)
        {
            if (laneNodes == null || laneNodes.Count == 0)
                return false;

            foreach (var node in laneNodes)
            {
                if (node == null)
                    continue;

                // Start は除外
                if (node.XLevel <= 0)
                    continue;

                // End は除外
                if (!string.IsNullOrWhiteSpace(node.DisplayNodeKey) &&
                    endNodeKeys != null &&
                    endNodeKeys.Contains(node.DisplayNodeKey))
                {
                    continue;
                }

                // XLevel>=1 で End ではない実ノードがいれば、
                // その枝は Middle 実体あり
                return true;
            }

            return false;
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

        private bool IsDirectStartToEndBranchWithoutMiddle(TracePathRow row)
        {
            if (row == null)
                return false;

            if (row.StartNode == null || row.EndNode == null)
                return false;

            // 実リンクを持たない fallback 行は補完対象にしない
            if (row.PathLinks == null || row.PathLinks.Count == 0)
                return false;

            if (HasAnyEffectiveMiddleNode(row))
                return false;

            return true;
        }

        private bool HasAnyEffectiveMiddleNode(TracePathRow row)
        {
            if (row == null || row.MiddleNodes == null || row.MiddleNodes.Count == 0)
                return false;

            foreach (var node in row.MiddleNodes)
            {
                if (node != null)
                    return true;
            }

            return false;
        }

        private string BuildSyntheticBranchKey(DisplayLaneNode node)
        {
            if (node == null)
                return null;

            return (node.ParentDisplayNodeKey ?? string.Empty)
                 + "|"
                 + (node.LotGroupKey ?? string.Empty)
                 + "|"
                 + (node.RepresentativeNodeKey ?? string.Empty)
                 + "|"
                 + (node.MergeKey ?? string.Empty);
        }

        private int ResolveSyntheticMiddleToXLevel(
            IList<TraceLineRange> lineRanges,
            IList<TracePathRow> pathRows)
        {
            int maxMiddleXFromExistingLines = -1;

            if (lineRanges != null && lineRanges.Count > 0)
            {
                maxMiddleXFromExistingLines = lineRanges
                    .Where(x =>
                        x != null &&
                        string.Equals(x.GridKind, "Middle", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.ToXLevel)
                    .DefaultIfEmpty(-1)
                    .Max();
            }

            if (maxMiddleXFromExistingLines >= 1)
                return maxMiddleXFromExistingLines;

            int maxMiddleCountFromRows = 0;

            if (pathRows != null && pathRows.Count > 0)
            {
                maxMiddleCountFromRows = pathRows
                    .Where(x => x != null)
                    .Select(x => CountEffectiveMiddleNodes(x))
                    .DefaultIfEmpty(0)
                    .Max();
            }

            // Middle 実列が全く無いケースでも、
            // UI契約上 Middle 幹線を1列目に補完できるよう最低1を返す
            if (maxMiddleCountFromRows <= 0)
                return 1;

            return maxMiddleCountFromRows;
        }

        private int CountEffectiveMiddleNodes(TracePathRow row)
        {
            if (row == null || row.MiddleNodes == null || row.MiddleNodes.Count == 0)
                return 0;

            int count = 0;

            foreach (var node in row.MiddleNodes)
            {
                if (node != null)
                    count++;
            }

            return count;
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

        

        private DataTable BuildLineDecisionKeyDumpTable(TraceDisplayResult displayResult)
        {
            var dt = new DataTable();

            dt.Columns.Add("RowIndex", typeof(int));
            dt.Columns.Add("Level", typeof(int));
            dt.Columns.Add("NodeKey", typeof(string));
            dt.Columns.Add("LotNo", typeof(string));
            dt.Columns.Add("LotGroupKey", typeof(string));
            dt.Columns.Add("RepresentativeNodeKey", typeof(string));
            dt.Columns.Add("IsRepresentativeInLotGroup", typeof(bool));
            dt.Columns.Add("BranchSignature", typeof(string));

            if (displayResult == null || displayResult.Rows == null || displayResult.Rows.Count == 0)
                return dt;

            for (int rowIndex = 0; rowIndex < displayResult.Rows.Count; rowIndex++)
            {
                var row = displayResult.Rows[rowIndex];
                if (row == null || row.Middles == null || row.Middles.Count == 0)
                    continue;

                foreach (var cell in row.Middles)
                {
                    if (cell == null)
                        continue;

                    var dr = dt.NewRow();

                    dr["RowIndex"] = rowIndex;
                    dr["Level"] = cell.Level;
                    dr["NodeKey"] = (object)cell.NodeKey ?? DBNull.Value;
                    dr["LotNo"] = (object)cell.LotNumber ?? DBNull.Value;
                    dr["LotGroupKey"] = (object)cell.LotGroupKey ?? DBNull.Value;

                    // 現時点では未搭載想定。後で正式に載せたらここだけ差し替えればよい
                    dr["RepresentativeNodeKey"] = DBNull.Value;

                    dr["IsRepresentativeInLotGroup"] = cell.IsRepresentativeInLotGroup;
                    dr["BranchSignature"] = (object)cell.BranchSignature ?? DBNull.Value;

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }



        private void DumpLineDecisionKeys(TraceDisplayResult displayResult)
        {
            try
            {
                var dt = BuildLineDecisionKeyDumpTable(displayResult);
                ExportDataTableToCsvSafe(dt, "LotTrace_Debug_LineDecisionKeys", false);
            }
            catch
            {
                // デバッグ出力失敗で本処理は落とさない
            }
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

        private void AddForwardMiddleLotGroupInfosFromLaneNodes(
    TracePathRow row,
    List<DisplayLaneNode> laneNodes)
        {
            if (row == null || laneNodes == null || laneNodes.Count == 0)
                return;

            // 同一row内の MiddleNode について、
            // XLevel ごとに罫線判定用 LotGroup 情報を row.LevelLotGroups に転写する
            foreach (var displayNode in laneNodes
                .Where(x => x != null && x.SourceNode != null && x.XLevel > 0)
                .OrderBy(x => x.XLevel)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase))
            {
                int level = displayNode.XLevel;

                // 同一Levelは1件だけ積む
                if (row.LevelLotGroups.Any(g =>
                    g != null &&
                    g.Axis == TraceGroupAxis.Middle &&
                    g.Level == level))
                {
                    continue;
                }

                string lotNumber = displayNode.SourceNode == null
                    ? null
                    : displayNode.SourceNode.LotNumber;

                string groupKey = string.IsNullOrWhiteSpace(displayNode.LotGroupKey)
                    ? BuildLotGroupKey(TraceGroupAxis.Middle, level, lotNumber)
                    : displayNode.LotGroupKey;

                row.LevelLotGroups.Add(new TraceLotGroupInfo
                {
                    Axis = TraceGroupAxis.Middle,
                    Level = level,
                    GroupKey = groupKey,
                    LotNumber = NormalizeLotForGrouping(lotNumber),

                    // 今回はUI橋渡しが目的なので最小限
                    IsRepresentative = displayNode.IsLotGroupRepresentative,
                    RepresentativeKey = displayNode.IsLotGroupRepresentative
                        ? displayNode.DisplayNodeKey
                        : null,

                    // まずはダンプ確認用に DisplayNodeKey を入れておく
                    DownstreamBranchSignature = displayNode.DisplayNodeKey
                });
            }
        }

        #endregion

        #region 新トレースバック関連群

        private ForwardDisplayLaneBuildResult BuildBackwardDisplayLaneNodes(
    TraceResult result,
    List<ProductionResultNode> startNodes,
    Dictionary<string, List<BackwardParentCandidate>> backwardCandidatesByChildNodeKey)
        {
            var buildResult = new ForwardDisplayLaneBuildResult();

            if (result == null || result.RootNodes == null || result.RootNodes.Count == 0)
                return buildResult;

            var displayNodes = new List<DisplayLaneNode>();
            var displayNodeByBackwardKey = new Dictionary<string, DisplayLaneNode>(StringComparer.OrdinalIgnoreCase);
            int nextY = 0;

            var roots = (startNodes != null && startNodes.Count > 0)
                ? startNodes
                : result.RootNodes;

            foreach (var root in roots)
            {
                if (root == null)
                    continue;

                var pathVisitedNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                PlaceBackwardBranchRecursive(
                    currentNode: root,
                    parentDisplayNode: null,
                    xLevel: 0,
                    nextYRef: ref nextY,
                    output: displayNodes,
                    pathVisitedNodeKeys: pathVisitedNodeKeys,
                    displayNodeByBackwardKey: displayNodeByBackwardKey,
                    backwardCandidatesByChildNodeKey: backwardCandidatesByChildNodeKey);
            }

            buildResult.DisplayNodes = displayNodes;
            return buildResult;
        }

        private DisplayLaneNode PlaceBackwardBranchRecursive(
    ProductionResultNode currentNode,
    DisplayLaneNode parentDisplayNode,
    int xLevel,
    ref int nextYRef,
    List<DisplayLaneNode> output,
    HashSet<string> pathVisitedNodeKeys,
    Dictionary<string, DisplayLaneNode> displayNodeByBackwardKey,
    Dictionary<string, List<BackwardParentCandidate>> backwardCandidatesByChildNodeKey)
        {
            if (currentNode == null)
                return null;

            if (output == null)
                throw new ArgumentNullException("output");

            if (pathVisitedNodeKeys == null)
                throw new ArgumentNullException("pathVisitedNodeKeys");

            if (displayNodeByBackwardKey == null)
                throw new ArgumentNullException("displayNodeByBackwardKey");

            string currentVisitKey = GetBackwardVisitedKey(currentNode);
            if (!string.IsNullOrWhiteSpace(currentVisitKey) &&
                !pathVisitedNodeKeys.Add(currentVisitKey))
            {
                return null;
            }

            try
            {
                string backwardNodeKey = GetNodeMergeKey(currentNode);
                if (string.IsNullOrWhiteSpace(backwardNodeKey))
                    backwardNodeKey = currentVisitKey;

                DisplayLaneNode currentDisplayNode = null;

                // backward でも DisplayNode 実体は 1 回だけ作る
                if (!string.IsNullOrWhiteSpace(backwardNodeKey))
                {
                    displayNodeByBackwardKey.TryGetValue(backwardNodeKey, out currentDisplayNode);
                }

                if (currentDisplayNode == null)
                {
                    int currentY = nextYRef;
                    nextYRef++;

                    currentDisplayNode = new DisplayLaneNode
                    {
                        DisplayNodeKey = BuildDisplayLaneNodeKey(currentNode, xLevel, currentY),
                        MergeKey = GetNodeMergeKey(currentNode),
                        ParentDisplayNodeKey = parentDisplayNode == null ? null : parentDisplayNode.DisplayNodeKey,
                        SourceNode = currentNode,
                        XLevel = xLevel,
                        YLane = currentY,
                        ChildIndex = 0,
                        SubtreeLaneSpan = 1
                    };

                    output.Add(currentDisplayNode);

                    if (!string.IsNullOrWhiteSpace(backwardNodeKey))
                    {
                        displayNodeByBackwardKey[backwardNodeKey] = currentDisplayNode;
                    }
                }

                // ------------------------------------------------------------
                // backward では ParentNodes を直接使わず、
                // childNode に紐づく BackwardParentCandidate を RelationKey 単位で束ねて使う
                // ------------------------------------------------------------
                var relationGroups = GetBackwardRelationGroups(
                    currentNode,
                    backwardCandidatesByChildNodeKey);

                if (relationGroups.Count == 0)
                {
                    currentDisplayNode.RootGroupHeight = 1;
                    currentDisplayNode.ChildBranchHeight = 0;
                    currentDisplayNode.OccupiedFirstY = currentDisplayNode.YLane;
                    currentDisplayNode.OccupiedLastY = currentDisplayNode.YLane;
                    currentDisplayNode.SubtreeLaneSpan = 1;
                    return currentDisplayNode;
                }

                int firstY = currentDisplayNode.YLane;
                int lastY = currentDisplayNode.YLane;
                int childIndex = 0;
                int createdChildCount = 0;

                foreach (var relationGroup in relationGroups)
                {
                    if (relationGroup == null || relationGroup.Count == 0)
                        continue;

                    // 同一 RelationKey は同一接続文脈なので、枝構造上は代表1件だけ使う
                    var representative = relationGroup
                        .FirstOrDefault(x => x != null && x.Node != null);

                    if (representative == null || representative.Node == null)
                        continue;

                    var parentNode = representative.Node;

                    var childPathVisitedNodeKeys =
                        new HashSet<string>(pathVisitedNodeKeys, StringComparer.OrdinalIgnoreCase);

                    var parentDisplayNodeCreated = PlaceBackwardBranchRecursive(
                        currentNode: parentNode,
                        parentDisplayNode: currentDisplayNode,
                        xLevel: xLevel + 1,
                        nextYRef: ref nextYRef,
                        output: output,
                        pathVisitedNodeKeys: childPathVisitedNodeKeys,
                        displayNodeByBackwardKey: displayNodeByBackwardKey,
                        backwardCandidatesByChildNodeKey: backwardCandidatesByChildNodeKey);

                    if (parentDisplayNodeCreated == null)
                        continue;

                    string relationKey = NormalizeBackwardRelationKey(representative);

                    bool edgeExists = currentDisplayNode.OutgoingEdges.Any(e =>
                        e != null &&
                        string.Equals(e.EdgeIdentityKey, relationKey, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(e.ChildDisplayNodeKey, parentDisplayNodeCreated.DisplayNodeKey, StringComparison.OrdinalIgnoreCase));

                    if (edgeExists)
                        continue;

                    parentDisplayNodeCreated.ChildIndex = childIndex;
                    childIndex++;
                    createdChildCount++;

                    currentDisplayNode.OutgoingEdges.Add(new DisplayLaneEdge
                    {
                        EdgeIdentityKey = relationKey,
                        ParentDisplayNodeKey = currentDisplayNode.DisplayNodeKey,
                        ChildDisplayNodeKey = parentDisplayNodeCreated.DisplayNodeKey,
                        ParentMergeKey = currentDisplayNode.MergeKey,
                        ChildMergeKey = parentDisplayNodeCreated.MergeKey,
                        FromXLevel = currentDisplayNode.XLevel,
                        ToXLevel = parentDisplayNodeCreated.XLevel,
                        FromYLane = currentDisplayNode.YLane,
                        ToYLane = parentDisplayNodeCreated.YLane,
                        ChildIndex = parentDisplayNodeCreated.ChildIndex,
                        LotGroupKey = null
                    });

                    if (parentDisplayNodeCreated.OccupiedFirstY < firstY)
                        firstY = parentDisplayNodeCreated.OccupiedFirstY;

                    if (parentDisplayNodeCreated.OccupiedLastY > lastY)
                        lastY = parentDisplayNodeCreated.OccupiedLastY;
                }

                currentDisplayNode.RootGroupHeight = 1;
                currentDisplayNode.ChildBranchHeight = createdChildCount;
                currentDisplayNode.OccupiedFirstY = firstY;
                currentDisplayNode.OccupiedLastY = lastY;
                currentDisplayNode.SubtreeLaneSpan = Math.Max(1, lastY - firstY + 1);

                return currentDisplayNode;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(currentVisitKey))
                {
                    pathVisitedNodeKeys.Remove(currentVisitKey);
                }
            }
        }

        private void RegisterBackwardParentCandidateForDisplay(
    Dictionary<string, List<BackwardParentCandidate>> backwardCandidatesByChildNodeKey,
    ProductionResultNode childNode,
    ProductionResultNode normalizedParentNode,
    BackwardParentCandidate candidate)
        {
            if (backwardCandidatesByChildNodeKey == null || childNode == null || candidate == null)
                return;

            string childNodeKey = GetBackwardDisplayChildNodeKey(childNode, candidate);
            if (string.IsNullOrWhiteSpace(childNodeKey))
                return;

            List<BackwardParentCandidate> list;
            if (!backwardCandidatesByChildNodeKey.TryGetValue(childNodeKey, out list))
            {
                list = new List<BackwardParentCandidate>();
                backwardCandidatesByChildNodeKey[childNodeKey] = list;
            }

            // 枝構造側では GetOrAddNode 後の正規 parent 実体を使う
            candidate.Node = normalizedParentNode;

            string relationKey = NormalizeBackwardRelationKey(candidate);
            string parentMergeKey = GetNodeMergeKey(normalizedParentNode) ?? string.Empty;

            bool exists = list.Any(x =>
                x != null &&
                string.Equals(NormalizeBackwardRelationKey(x), relationKey, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(GetNodeMergeKey(x.Node) ?? string.Empty, parentMergeKey, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                list.Add(candidate);
            }
        }

        private List<List<BackwardParentCandidate>> GetBackwardRelationGroups(
            ProductionResultNode childNode,
            Dictionary<string, List<BackwardParentCandidate>> backwardCandidatesByChildNodeKey)
        {
            var result = new List<List<BackwardParentCandidate>>();

            if (childNode == null || backwardCandidatesByChildNodeKey == null || backwardCandidatesByChildNodeKey.Count == 0)
                return result;

            string childNodeKey = GetBackwardDisplayChildNodeKey(childNode, null);
            if (string.IsNullOrWhiteSpace(childNodeKey))
                return result;

            List<BackwardParentCandidate> candidates;
            if (!backwardCandidatesByChildNodeKey.TryGetValue(childNodeKey, out candidates) ||
                candidates == null ||
                candidates.Count == 0)
            {
                return result;
            }

            var grouped = candidates
                .Where(x => x != null && x.Node != null)
                .GroupBy(x => NormalizeBackwardRelationKey(x), StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var group in grouped)
            {
                var members = group.ToList();
                if (members.Count > 0)
                {
                    result.Add(members);
                }
            }

            return result;
        }

        private string GetBackwardDisplayChildNodeKey(
            ProductionResultNode childNode,
            BackwardParentCandidate candidate)
        {
            if (candidate != null && !string.IsNullOrWhiteSpace(candidate.ChildNodeKey))
                return candidate.ChildNodeKey.Trim();

            string mergeKey = GetNodeMergeKey(childNode);
            if (!string.IsNullOrWhiteSpace(mergeKey))
                return mergeKey;

            if (childNode != null && !string.IsNullOrWhiteSpace(childNode.ControlMasterKey))
                return childNode.ControlMasterKey.Trim();

            if (childNode != null && !string.IsNullOrWhiteSpace(childNode.LotNumber))
                return childNode.LotNumber.Trim();

            return null;
        }

        private string NormalizeBackwardRelationKey(BackwardParentCandidate candidate)
        {
            if (candidate == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(candidate.RelationKey))
                return candidate.RelationKey.Trim();

            string parentMergeKey = candidate.Node == null
                ? string.Empty
                : (GetNodeMergeKey(candidate.Node) ?? string.Empty);

            string childNodeKey = string.IsNullOrWhiteSpace(candidate.ChildNodeKey)
                ? string.Empty
                : candidate.ChildNodeKey.Trim();

            return string.Join("|",
                parentMergeKey,
                candidate.SourceTable.ToString(),
                candidate.MaterialAInputType.ToString(),
                candidate.SlotNo.ToString(),
                childNodeKey,
                candidate.ChildLotNumber ?? string.Empty);
        }



        private void EnsureBackwardParentChildNodeRelationship(
    ProductionResultNode parentNode,
    ProductionResultNode childNode)
        {
            if (parentNode == null || childNode == null)
                return;

            EnsureNodeCollections(parentNode);
            EnsureNodeCollections(childNode);

            if (!parentNode.ChildNodes.Any(x => object.ReferenceEquals(x, childNode)))
            {
                parentNode.ChildNodes.Add(childNode);
            }

            if (!childNode.ParentNodes.Any(x => object.ReferenceEquals(x, parentNode)))
            {
                childNode.ParentNodes.Add(parentNode);
            }
        }

        private void EnsureBackwardParentChildLinkRelationship(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    ProductionResultLink link)
        {
            if (parentNode == null || childNode == null || link == null)
                return;

            EnsureNodeCollections(parentNode);
            EnsureNodeCollections(childNode);

            if (!parentNode.ChildLinks.Any(x =>
                x != null &&
                string.Equals(x.LinkIdentityKey, link.LinkIdentityKey, StringComparison.OrdinalIgnoreCase)))
            {
                parentNode.ChildLinks.Add(link);
            }

            if (!childNode.ParentLinks.Any(x =>
                x != null &&
                string.Equals(x.LinkIdentityKey, link.LinkIdentityKey, StringComparison.OrdinalIgnoreCase)))
            {
                childNode.ParentLinks.Add(link);
            }
        }

        private bool ShouldIgnoreBackwardTreeNode(ProductionResultNode node)
        {
            if (node == null)
                return false;

            return string.Equals(
                node.InputSourceType,
                MaterialAInputType.Drumcan.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        

        private string ResolveBackwardStartDateLabel(ProductionResultNode node)
        {
            if (node == null)
                return null;

            if (node.StartDate.HasValue)
                return node.StartDate.Value.ToString("yyyy/MM/dd HH:mm");

            return null;
        }

        private string BuildBackwardLocalEdgeKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    BackwardParentCandidate candidate)
        {
            if (candidate != null && !string.IsNullOrWhiteSpace(candidate.RelationKey))
            {
                return "BACKWARD_LOCAL|" + candidate.RelationKey.Trim();
            }

            string parentMergeKey = GetNodeMergeKey(parentNode) ?? string.Empty;
            string childMergeKey = GetNodeMergeKey(childNode) ?? string.Empty;

            string sourceTable = candidate == null
                ? string.Empty
                : candidate.SourceTable.ToString();

            string inputType = candidate == null
                ? string.Empty
                : candidate.MaterialAInputType.ToString();

            string slotNo = candidate == null
                ? string.Empty
                : candidate.SlotNo.ToString();

            string childLot = candidate == null || string.IsNullOrWhiteSpace(candidate.ChildLotNumber)
                ? string.Empty
                : candidate.ChildLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                "BACKWARD_LOCAL",
                parentMergeKey,
                childMergeKey,
                sourceTable,
                inputType,
                slotNo,
                childLot);
        }

        private string BuildBackwardLinkIdentityKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    BackwardParentCandidate candidate)
        {
            if (candidate != null && !string.IsNullOrWhiteSpace(candidate.RelationKey))
                return candidate.RelationKey.Trim();

            string parentMergeKey = GetNodeMergeKey(parentNode) ?? string.Empty;
            string childMergeKey = GetNodeMergeKey(childNode) ?? string.Empty;

            string sourceTable = candidate == null
                ? string.Empty
                : candidate.SourceTable.ToString();

            string inputType = candidate == null
                ? string.Empty
                : candidate.MaterialAInputType.ToString();

            string slotNo = candidate == null
                ? string.Empty
                : candidate.SlotNo.ToString();

            string childLot = candidate == null || string.IsNullOrWhiteSpace(candidate.ChildLotNumber)
                ? string.Empty
                : candidate.ChildLotNumber.Trim().ToUpperInvariant();

            return string.Join("|",
                parentMergeKey,
                sourceTable,
                inputType,
                slotNo,
                childLot,
                childMergeKey);
        }


        private bool AddBackwardLinkFromParentCandidate(
    TraceResult result,
    Dictionary<string, ProductionResultLink> linkMap,
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    BackwardParentCandidate candidate)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (linkMap == null)
                throw new ArgumentNullException("linkMap");

            if (parentNode == null || childNode == null || candidate == null)
                return false;

            EnsureNodeCollections(parentNode);
            EnsureNodeCollections(childNode);

            string linkKey = BuildBackwardLinkIdentityKey(parentNode, childNode, candidate);
            if (string.IsNullOrWhiteSpace(linkKey))
                return false;

            ProductionResultLink existingLink;
            if (linkMap.TryGetValue(linkKey, out existingLink) && existingLink != null)
            {
                EnsureBackwardParentChildNodeRelationship(parentNode, childNode);
                EnsureBackwardParentChildLinkRelationship(parentNode, childNode, existingLink);
                return false;
            }

            var link = new ProductionResultLink
            {
                ParentNode = parentNode,
                ChildNode = childNode,
                EdgeDirection = TraceEdgeDirection.ParentToChild,
                RootGroupKey = GetNodeIdentityKey(childNode),

                // backward では child 文脈が重要なので、
                // 旧 ParentLotNumber 名義には childLot を保持する
                ParentLotNumber = candidate.ChildLotNumber,

                SourceTable = candidate.SourceTable,
                MaterialAInputType = candidate.MaterialAInputType,
                SlotNo = candidate.SlotNo,
                LinkIdentityKey = linkKey
            };

            linkMap[linkKey] = link;
            result.AllLinks.Add(link);

            EnsureBackwardParentChildNodeRelationship(parentNode, childNode);
            EnsureBackwardParentChildLinkRelationship(parentNode, childNode, link);

            return true;
        }

        private void ApplyBackwardParentNodeBusinessAttributes(
    ProductionResultNode node,
    BackwardParentCandidate candidate)
        {
            if (node == null || candidate == null)
                return;

            if (string.IsNullOrWhiteSpace(node.RouteSystem))
            {
                if (candidate.SourceTable == TraceSourceTable.MaterialTableB)
                {
                    node.RouteSystem = "B";
                }
                else if (candidate.SourceTable == TraceSourceTable.MaterialTableA)
                {
                    node.RouteSystem = "A";
                }
            }

            if (candidate.SourceTable == TraceSourceTable.MaterialTableA)
            {
                if (!node.InputSlotNo.HasValue)
                    node.InputSlotNo = candidate.SlotNo;

                if (string.IsNullOrWhiteSpace(node.InputSourceType))
                {
                    node.InputSourceType =
                        candidate.MaterialAInputType == MaterialAInputType.None
                            ? null
                            : candidate.MaterialAInputType.ToString();
                }
            }
            else if (candidate.SourceTable == TraceSourceTable.MaterialTableB)
            {
                if (!node.InputSlotNo.HasValue)
                    node.InputSlotNo = candidate.SlotNo;

                if (string.IsNullOrWhiteSpace(node.InputSourceType))
                    node.InputSourceType = null;
            }

            if (!node.IsTraceTerminal)
                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);
        }

        private string GetBackwardVisitedKey(ProductionResultNode node)
        {
            if (node == null)
                return null;

            string routeSystem = string.IsNullOrWhiteSpace(node.RouteSystem)
                ? string.Empty
                : node.RouteSystem.Trim().ToUpperInvariant();

            string masterKey = string.IsNullOrWhiteSpace(node.ControlMasterKey)
                ? string.Empty
                : node.ControlMasterKey.Trim();

            string lotNumber = string.IsNullOrWhiteSpace(node.LotNumber)
                ? string.Empty
                : node.LotNumber.Trim().ToUpperInvariant();

            string itemCode = string.IsNullOrWhiteSpace(node.ItemCode)
                ? string.Empty
                : node.ItemCode.Trim().ToUpperInvariant();

            string inputSourceType = string.IsNullOrWhiteSpace(node.InputSourceType)
                ? string.Empty
                : node.InputSourceType.Trim().ToUpperInvariant();

            string slotPart = node.InputSlotNo.HasValue
                ? node.InputSlotNo.Value.ToString()
                : string.Empty;

            // 1. RouteSystem + MasterKey があるなら最優先
            if (!string.IsNullOrWhiteSpace(routeSystem) &&
                !string.IsNullOrWhiteSpace(masterKey))
            {
                return string.Join("|", "BV", routeSystem, masterKey, slotPart);
            }

            // 2. 特殊A向けフォールバック
            if (string.Equals(routeSystem, "A", StringComparison.OrdinalIgnoreCase))
            {
                return string.Join("|",
                    "BV",
                    "A",
                    masterKey,
                    itemCode,
                    lotNumber,
                    inputSourceType,
                    string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);
            }

            // 3. B / 不明系フォールバック
            if (!string.IsNullOrWhiteSpace(masterKey))
            {
                return string.Join("|", "BV", routeSystem, masterKey);
            }

            if (!string.IsNullOrWhiteSpace(lotNumber))
            {
                return string.Join("|",
                    "BV",
                    routeSystem,
                    itemCode,
                    lotNumber,
                    inputSourceType,
                    string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);
            }

            return null;
        }

        private List<ForwardDisplayRoute> BuildBackwardRoutesFromDisplayNodes(
    List<DisplayLaneNode> displayNodes)
        {
            var routes = new List<ForwardDisplayRoute>();

            if (displayNodes == null || displayNodes.Count == 0)
                return routes;

            var nodeByKey = displayNodes
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.DisplayNodeKey))
                .GroupBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var childSideParentKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in displayNodes)
            {
                if (node == null)
                    continue;

                if (string.IsNullOrWhiteSpace(node.ParentDisplayNodeKey))
                    continue;

                childSideParentKeys.Add(node.ParentDisplayNodeKey);
            }

            // backward の終端 = 他Nodeの ParentDisplayNodeKey として参照されないNode
            var terminalNodes = displayNodes
                .Where(x =>
                    x != null &&
                    !string.IsNullOrWhiteSpace(x.DisplayNodeKey) &&
                    !childSideParentKeys.Contains(x.DisplayNodeKey))
                .OrderByDescending(x => x.XLevel)
                .ThenBy(x => x.YLane)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var terminalNode in terminalNodes)
            {
                //var route = BuildBackwardRouteFromTerminalNode(
                //    terminalNode,
                //    nodeByKey);

                //if (route == null)
                    continue;

                //routes.Add(route);
            }

            return routes;
        }

        private ForwardDisplayRoute BuildBackwardRouteFromTerminalNode(
            DisplayLaneNode terminalNode,
            Dictionary<string, DisplayLaneNode> nodeByKey)
        {
            if (terminalNode == null)
                return null;

            if (nodeByKey == null || nodeByKey.Count == 0)
                return null;

            var orderedNodes = new List<DisplayLaneNode>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var current = terminalNode;

            while (current != null)
            {
                if (!string.IsNullOrWhiteSpace(current.DisplayNodeKey))
                {
                    if (!visited.Add(current.DisplayNodeKey))
                    {
                        // ループ防止
                        break;
                    }
                }

                orderedNodes.Add(current);

                if (string.IsNullOrWhiteSpace(current.ParentDisplayNodeKey))
                    break;

                DisplayLaneNode next;
                if (!nodeByKey.TryGetValue(current.ParentDisplayNodeKey, out next))
                    break;

                current = next;
            }

            if (orderedNodes.Count == 0)
                return null;

            var startNode = orderedNodes[0];
            var endNode = orderedNodes[orderedNodes.Count - 1];

            var pathMergeKeys = orderedNodes
                .Select(GetDisplayLaneNodeMergeKeySafe)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var route = new ForwardDisplayRoute();
            route.StartNode = startNode;
            route.EndNode = endNode;
            route.Nodes = orderedNodes;
            route.NodeCount = orderedNodes.Count;
            route.EdgeCount = orderedNodes.Count <= 0 ? 0 : orderedNodes.Count - 1;
            route.StartY = startNode.YLane;
            route.EndY = endNode.YLane;
            route.StartMergeKey = GetDisplayLaneNodeMergeKeySafe(startNode);
            route.StartLotNumber = GetDisplayLaneNodeLotNumberSafe(startNode);
            route.EndGroupKey = null;
            route.EndGroupIndex = 0;
            route.PathMergeKeyText = string.Join(" > ", pathMergeKeys);

            route.RouteKey =
                (route.StartMergeKey ?? string.Empty) + "||" +
                (route.StartLotNumber ?? string.Empty) + "||" +
                (route.PathMergeKeyText ?? string.Empty);

            return route;
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
                AddForwardMiddleLotGroupInfosFromLaneNodes(row, laneNodes);

                // ------------------------------------------------------------
                // STEP2:
                // 行の代表情報を最低限埋める
                // ------------------------------------------------------------
                var firstNode = laneNodes.FirstOrDefault(x => x != null && x.SourceNode != null);
                var startBasisNode = row.StartNode ?? (firstNode == null ? null : firstNode.SourceNode);

                row.RootGroupKey = ResolvePathRowRootGroupKey(startBasisNode, laneNodes);
                row.StartTrunkGroupKey = BuildStartTrunkGroupKey(row.StartNode);
                row.StartTrunkOrder = row.PathOrder;

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

                // ------------------------------------------------------------
                // STEP5:
                // 署名確定
                // ------------------------------------------------------------
                row.FullPathSignature = BuildPathRowFullPathSignature(row);

                result.PathRows.Add(row);
            }
        }

        private bool IsBackwardTerminalDisplayNode(DisplayLaneNode node)
        {
            if (node == null)
                return false;

            // 最優先:
            // 業務上の理由で明示的に終端にしたものはそのまま終端扱い
            if (node.SourceNode != null && node.SourceNode.IsTraceTerminal)
                return true;

            // 基本パターン:
            // backward 枝構造上、この DisplayNode から右へ枝が伸びていない
            if (node.OutgoingEdges == null || node.OutgoingEdges.Count == 0)
                return true;

            return false;
        }

        
        private List<EndDisplayNodeInfo> FinalizeBackwardEndNodeGroupsAndNormalizeX(
    List<DisplayLaneNode> displayNodes)
        {
            var output = new List<EndDisplayNodeInfo>();

            if (displayNodes == null || displayNodes.Count == 0)
                return output;

            var terminalNodes = displayNodes
                .Where(x => x != null)
                .Where(x => IsBackwardTerminalDisplayNode(x))
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
                .GroupBy(x => BuildEndGroupKey(x))
                .OrderBy(g => g.Min(n => n.YLane))
                .ThenBy(g => g.Key ?? string.Empty)
                .ToList();

            int groupIndex = 1;
            int nextDuplicateDisplayIndex = 1;

            foreach (var group in grouped)
            {
                var orderedGroupNodes = group
                    .Where(x => x != null)
                    .OrderBy(x => x.YLane)
                    .ThenBy(x => x.XLevel)
                    .ToList();

                int? duplicateDisplayIndex = orderedGroupNodes.Count >= 2
                    ? (int?)nextDuplicateDisplayIndex++
                    : null;

                for (int i = 0; i < orderedGroupNodes.Count; i++)
                {
                    var node = orderedGroupNodes[i];
                    if (node == null)
                        continue;

                    int oldXLevel = node.XLevel;
                    string oldDisplayNodeKey = node.DisplayNodeKey;

                    node.XLevel = endXLevel;
                    node.DisplayNodeKey = BuildDisplayLaneNodeKey(
                        node.SourceNode,
                        node.XLevel,
                        node.YLane);

                    UpdateEndNodeKeyAndIncomingEdgesXOnly(
                        displayNodes,
                        oldDisplayNodeKey,
                        node.DisplayNodeKey,
                        endXLevel);

                    var info = new EndDisplayNodeInfo
                    {
                        DisplayNodeKey = node.DisplayNodeKey,
                        MergeKey = node.MergeKey,
                        EndGroupKey = group.Key ?? string.Empty,
                        EndGroupIndex = groupIndex,
                        IsFirstOfGroup = (i == 0),
                        IsDuplicate = (i > 0),
                        OriginalXLevel = oldXLevel,
                        EndXLevel = endXLevel,
                        YLane = node.YLane,
                        LotNumber = node.SourceNode == null ? null : node.SourceNode.LotNumber,
                        ControlMasterKey = node.SourceNode == null ? null : node.SourceNode.ControlMasterKey,
                        DuplicateDisplayIndex = (i > 0) ? duplicateDisplayIndex : null
                    };

                    output.Add(info);
                }

                groupIndex++;
            }

            return output;
        }

        #endregion

        private void ExportDataTableToCsvSafe(
    DataTable table,
    string fileNameWithoutExtension,
    bool overwrite,
    string subFolderName)
        {
            try
            {
                if (table == null)
                    return;

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string debugDir = Path.Combine(baseDir, "Debug");

                if (!string.IsNullOrWhiteSpace(subFolderName))
                {
                    debugDir = Path.Combine(debugDir, subFolderName);
                }

                Directory.CreateDirectory(debugDir);

                string filePath = Path.Combine(
                    debugDir,
                    fileNameWithoutExtension + ".csv");

                using (var fs = new FileStream(
                    filePath,
                    overwrite ? FileMode.Create : FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    // header
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (i > 0)
                            sw.Write(",");

                        sw.Write(EscapeCsv(table.Columns[i].ColumnName));
                    }
                    sw.WriteLine();

                    // rows
                    foreach (DataRow row in table.Rows)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            if (i > 0)
                                sw.Write(",");

                            var value = row[i];
                            string text = value == null || value == DBNull.Value
                                ? string.Empty
                                : value.ToString();

                            sw.Write(EscapeCsv(text));
                        }

                        sw.WriteLine();
                    }
                }
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        private void ExportDataTableToCsvSafe(
    DataTable table,
    string fileNameWithoutExtension,
    bool overwrite = false)
        {
            if (table == null)
                return;

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string debugDir = Path.Combine(baseDir, "Debug");

                if (!Directory.Exists(debugDir))
                    Directory.CreateDirectory(debugDir);

                string filePath;

                if (overwrite)
                {
                    filePath = Path.Combine(debugDir, fileNameWithoutExtension + ".csv");
                }
                else
                {
                    string time = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                    filePath = Path.Combine(debugDir, fileNameWithoutExtension + "_" + time + ".csv");
                }

                // ★ ロック回避：FileShare.ReadWrite
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    // ヘッダー
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (i > 0) sw.Write(",");
                        sw.Write(EscapeCsv(table.Columns[i].ColumnName));
                    }
                    sw.WriteLine();

                    // データ
                    foreach (DataRow row in table.Rows)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            if (i > 0) sw.Write(",");

                            var value = row[i];
                            string text = value == null || value == DBNull.Value
                                ? string.Empty
                                : value.ToString();

                            sw.Write(EscapeCsv(text));
                        }
                        sw.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                // ★ 落とさない
                System.Diagnostics.Debug.WriteLine("CSV出力失敗: " + ex.Message);
            }
        }

        private DataTable BuildDisplayLaneNodeDumpTable(IList<DisplayLaneNode> nodes)
        {
            var dt = new DataTable();

            dt.Columns.Add("DisplayNodeKey", typeof(string));
            dt.Columns.Add("ParentDisplayNodeKey", typeof(string));
            dt.Columns.Add("MergeKey", typeof(string));

            dt.Columns.Add("XLevel", typeof(int));
            dt.Columns.Add("YLane", typeof(int));
            dt.Columns.Add("ChildIndex", typeof(int));
            dt.Columns.Add("SubtreeLaneSpan", typeof(int));

            dt.Columns.Add("RootGroupHeight", typeof(int));
            dt.Columns.Add("ChildBranchHeight", typeof(int));
            dt.Columns.Add("OccupiedFirstY", typeof(int));
            dt.Columns.Add("OccupiedLastY", typeof(int));

            dt.Columns.Add("LotGroupKey", typeof(string));
            dt.Columns.Add("RepresentativeNodeKey", typeof(string));
            dt.Columns.Add("IsLotGroupRepresentative", typeof(bool));
            dt.Columns.Add("IsLotGroupNonRepresentative", typeof(bool));

            dt.Columns.Add("SourceLotNumber", typeof(string));
            dt.Columns.Add("SourceMasterKey", typeof(string));
            dt.Columns.Add("SourceItemCode", typeof(string));
            dt.Columns.Add("SourceItemName", typeof(string));

            if (nodes == null || nodes.Count == 0)
                return dt;

            foreach (var node in nodes
                .Where(x => x != null)
                .OrderBy(x => x.YLane)
                .ThenBy(x => x.XLevel)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase))
            {
                var row = dt.NewRow();

                row["DisplayNodeKey"] = (object)node.DisplayNodeKey ?? DBNull.Value;
                row["ParentDisplayNodeKey"] = (object)node.ParentDisplayNodeKey ?? DBNull.Value;
                row["MergeKey"] = (object)node.MergeKey ?? DBNull.Value;

                row["XLevel"] = node.XLevel;
                row["YLane"] = node.YLane;
                row["ChildIndex"] = node.ChildIndex;
                row["SubtreeLaneSpan"] = node.SubtreeLaneSpan;

                row["RootGroupHeight"] = node.RootGroupHeight;
                row["ChildBranchHeight"] = node.ChildBranchHeight;
                row["OccupiedFirstY"] = node.OccupiedFirstY;
                row["OccupiedLastY"] = node.OccupiedLastY;

                row["LotGroupKey"] = (object)node.LotGroupKey ?? DBNull.Value;
                row["RepresentativeNodeKey"] = (object)node.RepresentativeNodeKey ?? DBNull.Value;
                row["IsLotGroupRepresentative"] = node.IsLotGroupRepresentative;
                row["IsLotGroupNonRepresentative"] = node.IsLotGroupNonRepresentative;

                row["SourceLotNumber"] = node.SourceNode == null
                    ? (object)DBNull.Value
                    : (object)node.SourceNode.LotNumber ?? DBNull.Value;

                row["SourceMasterKey"] = node.SourceNode == null
                    ? (object)DBNull.Value
                    : (object)node.SourceNode.ControlMasterKey ?? DBNull.Value;

                row["SourceItemCode"] = node.SourceNode == null
                    ? (object)DBNull.Value
                    : (object)node.SourceNode.ItemCode ?? DBNull.Value;

                row["SourceItemName"] = node.SourceNode == null
                    ? (object)DBNull.Value
                    : (object)node.SourceNode.ItemName ?? DBNull.Value;

                dt.Rows.Add(row);
            }

            return dt;
        }

        private void DumpDisplayLaneNodes(IList<DisplayLaneNode> nodes)
        {
            try
            {
                var dt = BuildDisplayLaneNodeDumpTable(nodes);
                ExportDataTableToCsvSafe(
                    dt,
                    "LotTrace_Debug_DisplayLaneNodes",
                    true,
                    "LineBridgeCheck");
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        private DataTable BuildOccupiedLotGroupRangeDumpTable(IList<OccupiedLotGroupRange> ranges)
        {
            var dt = new DataTable();

            dt.Columns.Add("Level", typeof(int));
            dt.Columns.Add("ParentDisplayNodeKey", typeof(string));
            dt.Columns.Add("LotGroupKey", typeof(string));
            dt.Columns.Add("OccupiedFirstY", typeof(int));
            dt.Columns.Add("OccupiedLastY", typeof(int));

            if (ranges == null || ranges.Count == 0)
                return dt;

            foreach (var range in ranges
                .Where(x => x != null)
                .OrderBy(x => x.Level)
                .ThenBy(x => x.OccupiedFirstY)
                .ThenBy(x => x.OccupiedLastY)
                .ThenBy(x => x.ParentDisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LotGroupKey, StringComparer.OrdinalIgnoreCase))
            {
                var row = dt.NewRow();

                row["Level"] = range.Level;
                row["ParentDisplayNodeKey"] = (object)range.ParentDisplayNodeKey ?? DBNull.Value;
                row["LotGroupKey"] = (object)range.LotGroupKey ?? DBNull.Value;
                row["OccupiedFirstY"] = range.OccupiedFirstY;
                row["OccupiedLastY"] = range.OccupiedLastY;

                dt.Rows.Add(row);
            }

            return dt;
        }

        private void DumpOccupiedLotGroupRanges(IList<OccupiedLotGroupRange> ranges)
        {
            try
            {
                var dt = BuildOccupiedLotGroupRangeDumpTable(ranges);
                ExportDataTableToCsvSafe(
                    dt,
                    "LotTrace_Debug_OccupiedLotGroupRanges",
                    true,
                    "LineBridgeCheck");
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        private DataTable BuildTraceLineRangeDumpTable(IList<TraceLineRange> lineRanges)
        {
            var dt = new DataTable();

            dt.Columns.Add("GridKind", typeof(string));
            dt.Columns.Add("LineKind", typeof(string));
            dt.Columns.Add("Level", typeof(int));
            dt.Columns.Add("StartRowIndex", typeof(int));
            dt.Columns.Add("EndRowIndex", typeof(int));
            dt.Columns.Add("FromXLevel", typeof(int));
            dt.Columns.Add("ToXLevel", typeof(int));

            if (lineRanges == null || lineRanges.Count == 0)
                return dt;

            foreach (var line in lineRanges
                .Where(x => x != null)
                .OrderBy(x => x.StartRowIndex)
                .ThenBy(x => x.Level)
                .ThenBy(x => x.FromXLevel)
                .ThenBy(x => x.ToXLevel)
                .ThenBy(x => x.LineKind, StringComparer.OrdinalIgnoreCase))
            {
                var row = dt.NewRow();

                row["GridKind"] = (object)line.GridKind ?? DBNull.Value;
                row["LineKind"] = (object)line.LineKind ?? DBNull.Value;
                row["Level"] = line.Level;
                row["StartRowIndex"] = line.StartRowIndex;
                row["EndRowIndex"] = line.EndRowIndex;
                row["FromXLevel"] = line.FromXLevel;
                row["ToXLevel"] = line.ToXLevel;

                dt.Rows.Add(row);
            }

            return dt;
        }

        private void DumpTraceLineRanges(IList<TraceLineRange> lineRanges)
        {
            try
            {
                var dt = BuildTraceLineRangeDumpTable(lineRanges);
                ExportDataTableToCsvSafe(
                    dt,
                    "LotTrace_Debug_TraceLineRanges",
                    true,
                    "LineBridgeCheck");
            }
            catch
            {
                // デバッグ出力失敗は握りつぶす
            }
        }

        private void ExportPathRowsFromDisplayLaneNodesDebugCsv(
    List<TracePathRow> rows,
    ForwardDisplayLaneBuildResult laneBuildResult,
    string fileNameWithoutExtension)
        {
            if (rows == null)
                return;

            var orderedRows = rows
                .Where(x => x != null)
                .OrderBy(x => x.PathOrder)
                .ToList();

            var orderedLaneGroups = (laneBuildResult == null || laneBuildResult.DisplayNodes == null)
                ? new List<IGrouping<int, DisplayLaneNode>>()
                : laneBuildResult.DisplayNodes
                    .Where(x => x != null)
                    .OrderBy(x => x.YLane)
                    .ThenBy(x => x.XLevel)
                    .ThenBy(x => x.ChildIndex)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .GroupBy(x => x.YLane)
                    .OrderBy(g => g.Key)
                    .ToList();

            int maxMiddleCount = orderedRows.Count == 0
                ? 0
                : orderedRows.Max(x => x.MiddleNodes == null ? 0 : x.MiddleNodes.Count);

            var dt = new DataTable();

            dt.Columns.Add("PathOrder", typeof(int));
            dt.Columns.Add("Row_SourceYLane", typeof(string));
            dt.Columns.Add("Row_SourceDisplayNodeKeys", typeof(string));

            dt.Columns.Add("RootGroupKey", typeof(string));
            dt.Columns.Add("StartTrunkGroupKey", typeof(string));
            dt.Columns.Add("StartTrunkOrder", typeof(int));
            dt.Columns.Add("FullPathSignature", typeof(string));
            dt.Columns.Add("IsPruned", typeof(bool));
            dt.Columns.Add("PruneReason", typeof(string));

            dt.Columns.Add("Start_NodeKey", typeof(string));
            dt.Columns.Add("Start_MergeKey", typeof(string));
            dt.Columns.Add("Start_NodeOnlyKey", typeof(string));
            dt.Columns.Add("Start_Lot", typeof(string));
            dt.Columns.Add("Start_MasterKey", typeof(string));
            dt.Columns.Add("Start_SourceDisplayNodeKey", typeof(string));

            dt.Columns.Add("Middle_Count", typeof(int));
            dt.Columns.Add("Middle_FilledCount", typeof(int));
            dt.Columns.Add("Middle_Summary", typeof(string));

            for (int i = 0; i < maxMiddleCount; i++)
            {
                int level = i + 1;

                dt.Columns.Add("Middle" + level + "_NodeKey", typeof(string));
                dt.Columns.Add("Middle" + level + "_MergeKey", typeof(string));
                dt.Columns.Add("Middle" + level + "_NodeOnlyKey", typeof(string));
                dt.Columns.Add("Middle" + level + "_Lot", typeof(string));
                dt.Columns.Add("Middle" + level + "_MasterKey", typeof(string));
                dt.Columns.Add("Middle" + level + "_SourceDisplayNodeKey", typeof(string));
            }

            dt.Columns.Add("End_NodeKey", typeof(string));
            dt.Columns.Add("End_MergeKey", typeof(string));
            dt.Columns.Add("End_NodeOnlyKey", typeof(string));
            dt.Columns.Add("End_Lot", typeof(string));
            dt.Columns.Add("End_MasterKey", typeof(string));
            dt.Columns.Add("End_SourceDisplayNodeKey", typeof(string));

            dt.Columns.Add("End_DuplicateDisplayGroupIndex", typeof(string));

            dt.Columns.Add("PathLinks_Count", typeof(int));
            dt.Columns.Add("PathLinks_Summary", typeof(string));

            for (int rowIndex = 0; rowIndex < orderedRows.Count; rowIndex++)
            {
                var row = orderedRows[rowIndex];
                if (row == null)
                    continue;

                var dr = dt.NewRow();

                var laneNodes = rowIndex < orderedLaneGroups.Count
                    ? orderedLaneGroups[rowIndex]
                        .Where(x => x != null)
                        .OrderBy(x => x.XLevel)
                        .ThenBy(x => x.ChildIndex)
                        .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                        .ToList()
                    : new List<DisplayLaneNode>();

                dr["PathOrder"] = row.PathOrder;
                dr["Row_SourceYLane"] = laneNodes.Count == 0
                    ? string.Empty
                    : laneNodes.First().YLane.ToString();
                dr["Row_SourceDisplayNodeKeys"] = BuildDisplayNodeKeyListSummary(laneNodes);

                dr["RootGroupKey"] = row.RootGroupKey ?? string.Empty;
                dr["StartTrunkGroupKey"] = row.StartTrunkGroupKey ?? string.Empty;
                dr["StartTrunkOrder"] = row.StartTrunkOrder;
                dr["FullPathSignature"] = row.FullPathSignature ?? string.Empty;
                dr["IsPruned"] = row.IsPruned;
                dr["PruneReason"] = row.PruneReason ?? string.Empty;

                // Start
                dr["Start_NodeKey"] = row.StartNode == null ? string.Empty : (GetNodeIdentityKey(row.StartNode) ?? string.Empty);
                dr["Start_MergeKey"] = row.StartNode == null ? string.Empty : (GetNodeMergeKey(row.StartNode) ?? string.Empty);
                dr["Start_NodeOnlyKey"] = row.StartNode == null ? string.Empty : (BuildNodeOnlyKey(row.StartNode) ?? string.Empty);
                dr["Start_Lot"] = row.StartNode == null ? string.Empty : (row.StartNode.LotNumber ?? string.Empty);
                dr["Start_MasterKey"] = row.StartNode == null ? string.Empty : (row.StartNode.ControlMasterKey ?? string.Empty);
                dr["Start_SourceDisplayNodeKey"] = FindDisplayNodeKeyForNodeInLane(row.StartNode, 0, laneNodes) ?? string.Empty;

                // Middle
                var middleNodes = row.MiddleNodes ?? new List<ProductionResultNode>();
                dr["Middle_Count"] = middleNodes.Count;
                dr["Middle_FilledCount"] = middleNodes.Count(x => x != null);
                dr["Middle_Summary"] = BuildPathRowMiddleSummary(middleNodes);

                for (int i = 0; i < maxMiddleCount; i++)
                {
                    int level = i + 1;
                    var middleNode = i < middleNodes.Count ? middleNodes[i] : null;

                    dr["Middle" + level + "_NodeKey"] = middleNode == null ? string.Empty : (GetNodeIdentityKey(middleNode) ?? string.Empty);
                    dr["Middle" + level + "_MergeKey"] = middleNode == null ? string.Empty : (GetNodeMergeKey(middleNode) ?? string.Empty);
                    dr["Middle" + level + "_NodeOnlyKey"] = middleNode == null ? string.Empty : (BuildNodeOnlyKey(middleNode) ?? string.Empty);
                    dr["Middle" + level + "_Lot"] = middleNode == null ? string.Empty : (middleNode.LotNumber ?? string.Empty);
                    dr["Middle" + level + "_MasterKey"] = middleNode == null ? string.Empty : (middleNode.ControlMasterKey ?? string.Empty);
                    dr["Middle" + level + "_SourceDisplayNodeKey"] = FindDisplayNodeKeyForNodeInLane(middleNode, level, laneNodes) ?? string.Empty;
                }

                // End
                dr["End_NodeKey"] = row.EndNode == null ? string.Empty : (GetNodeIdentityKey(row.EndNode) ?? string.Empty);
                dr["End_MergeKey"] = row.EndNode == null ? string.Empty : (GetNodeMergeKey(row.EndNode) ?? string.Empty);
                dr["End_NodeOnlyKey"] = row.EndNode == null ? string.Empty : (BuildNodeOnlyKey(row.EndNode) ?? string.Empty);
                dr["End_Lot"] = row.EndNode == null ? string.Empty : (row.EndNode.LotNumber ?? string.Empty);
                dr["End_MasterKey"] = row.EndNode == null ? string.Empty : (row.EndNode.ControlMasterKey ?? string.Empty);
                dr["End_SourceDisplayNodeKey"] = FindDisplayNodeKeyForNodeInLane(row.EndNode, null, laneNodes) ?? string.Empty;

                dr["End_DuplicateDisplayGroupIndex"] =
                row.EndDuplicateDisplayGroupIndex.HasValue
                    ? (object)row.EndDuplicateDisplayGroupIndex.Value.ToString()
                    : DBNull.Value;

                // PathLinks
                var pathLinks = row.PathLinks ?? new List<ProductionResultLink>();
                dr["PathLinks_Count"] = pathLinks.Count;
                dr["PathLinks_Summary"] = BuildPathLinksSummary(pathLinks);

                dt.Rows.Add(dr);
            }

            ExportDataTableToCsvSafe(dt, fileNameWithoutExtension, false);
        }

        private string FindDisplayNodeKeyForNodeInLane(
            ProductionResultNode targetNode,
            int? xLevel,
            List<DisplayLaneNode> laneNodes)
        {
            if (targetNode == null || laneNodes == null || laneNodes.Count == 0)
                return string.Empty;

            string targetIdentityKey = GetNodeIdentityKey(targetNode) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(targetIdentityKey))
                return string.Empty;

            var candidates = laneNodes
                .Where(x => x != null && x.SourceNode != null)
                .Where(x => string.Equals(
                    GetNodeIdentityKey(x.SourceNode) ?? string.Empty,
                    targetIdentityKey,
                    StringComparison.OrdinalIgnoreCase));

            if (xLevel.HasValue)
            {
                candidates = candidates.Where(x => x.XLevel == xLevel.Value);
            }

            var matched = candidates
                .OrderBy(x => x.XLevel)
                .ThenBy(x => x.ChildIndex)
                .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();

            return matched == null ? string.Empty : (matched.DisplayNodeKey ?? string.Empty);
        }

        private string BuildDisplayNodeKeyListSummary(List<DisplayLaneNode> laneNodes)
        {
            if (laneNodes == null || laneNodes.Count == 0)
                return string.Empty;

            return string.Join(" || ",
                laneNodes
                    .Where(x => x != null)
                    .OrderBy(x => x.XLevel)
                    .ThenBy(x => x.ChildIndex)
                    .ThenBy(x => x.DisplayNodeKey, StringComparer.OrdinalIgnoreCase)
                    .Select(x =>
                        "X=" + x.XLevel.ToString() +
                        ",Y=" + x.YLane.ToString() +
                        ",ChildIndex=" + x.ChildIndex.ToString() +
                        ",DisplayNodeKey=" + (x.DisplayNodeKey ?? string.Empty)));
        }

        private string BuildPathLinksSummary(List<ProductionResultLink> pathLinks)
        {
            if (pathLinks == null || pathLinks.Count == 0)
                return string.Empty;

            return string.Join(" || ",
                pathLinks
                    .Where(x => x != null)
                    .Select(x =>
                        "Parent=" + (x.ParentNode == null ? string.Empty : (GetNodeIdentityKey(x.ParentNode) ?? string.Empty)) +
                        "->Child=" + (x.ChildNode == null ? string.Empty : (GetNodeIdentityKey(x.ChildNode) ?? string.Empty)) +
                        ",LinkKey=" + (x.LinkIdentityKey ?? string.Empty)));
        }
        private string BuildPathRowMiddleSummary(List<ProductionResultNode> middleNodes)
        {
            if (middleNodes == null || middleNodes.Count == 0)
                return string.Empty;

            var parts = new List<string>();

            for (int i = 0; i < middleNodes.Count; i++)
            {
                var node = middleNodes[i];
                if (node == null)
                {
                    parts.Add("M" + i.ToString() + "=null");
                    continue;
                }

                parts.Add(string.Format(
                    "M{0}={1}",
                    i,
                    GetNodeIdentityKey(node) ?? (node.LotNumber ?? string.Empty)));
            }

            return string.Join(" | ", parts);
        }

        private DataTable BuildDisplayLaneNodeDumpTable(List<DisplayLaneNode> nodes)
        {
            var dt = new DataTable();

            dt.Columns.Add("DisplayNodeKey", typeof(string));
            dt.Columns.Add("ParentDisplayNodeKey", typeof(string));
            dt.Columns.Add("MergeKey", typeof(string));

            dt.Columns.Add("XLevel", typeof(int));
            dt.Columns.Add("YLane", typeof(int));

            if (nodes == null)
                return dt;

            foreach (var n in nodes)
            {
                if (n == null)
                    continue;

                var row = dt.NewRow();

                row["DisplayNodeKey"] = (object)n.DisplayNodeKey ?? DBNull.Value;
                row["ParentDisplayNodeKey"] = (object)n.ParentDisplayNodeKey ?? DBNull.Value;
                row["MergeKey"] = (object)n.MergeKey ?? DBNull.Value;

                row["XLevel"] = n.XLevel;
                row["YLane"] = n.YLane;

                dt.Rows.Add(row);
            }

            return dt;
        }

        private void DumpForwardDisplayLaneNodesBeforeEndNormalize(List<DisplayLaneNode> nodes)
        {
            try
            {
                if (nodes == null)
                    return;

                var lines = new List<string>();
                lines.Add("Index,DisplayNodeKey,ParentDisplayNodeKey,MergeKey,LotNumber,XLevel,YLane,ChildIndex,SubtreeLaneSpan,NodeMasterKey,ParentLotNumber,NodeItemCode");

                for (int i = 0; i < nodes.Count; i++)
                {
                    var n = nodes[i];
                    if (n == null)
                        continue;

                    var src = n.SourceNode;

                    lines.Add(string.Join(",",
                        EscapeCsv(i.ToString()),
                        EscapeCsv(n.DisplayNodeKey),
                        EscapeCsv(n.ParentDisplayNodeKey),
                        EscapeCsv(n.MergeKey),
                        EscapeCsv(src == null ? null : src.LotNumber),
                        EscapeCsv(n.XLevel.ToString()),
                        EscapeCsv(n.YLane.ToString()),
                        EscapeCsv(n.ChildIndex.ToString()),
                        EscapeCsv(n.SubtreeLaneSpan.ToString()),
                        EscapeCsv(src == null ? null : src.ControlMasterKey),
                        
                        EscapeCsv(src == null ? null : src.ItemCode)
                    ));
                }

                var debugDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debug");
                if (!System.IO.Directory.Exists(debugDir))
                    System.IO.Directory.CreateDirectory(debugDir);

                var path = System.IO.Path.Combine(debugDir, "LotTrace_Debug_ForwardDisplayLaneNodes_BeforeEndNormalize.csv");
                System.IO.File.WriteAllLines(path, lines, Encoding.UTF8);
            }
            catch
            {
                // ダンプ失敗で本処理は落とさない
            }
        }

        private void DumpBackwardTraceArtifacts(
    TraceResult result,
    ForwardDisplayLaneBuildResult backwardLaneBuildResult,
    string phase)
        {
            try
            {
                DumpBackwardTraceSummary(result, backwardLaneBuildResult, phase);

                if (result != null)
                {
                    ExportAllNodesDebugCsv(
                        result.AllNodes,
                        "LotTrace_Debug_Backward_AllNodes.csv");
                }

                DumpDisplayLaneNodesToFile(
                    backwardLaneBuildResult == null ? null : backwardLaneBuildResult.DisplayNodes,
                    "LotTrace_Debug_BackwardDisplayLaneNodes.csv");

                DumpDisplayLaneEdgesToFile(
                    backwardLaneBuildResult == null ? null : backwardLaneBuildResult.DisplayNodes,
                    "LotTrace_Debug_BackwardDisplayLaneEdges.csv");
            }
            catch
            {
                // デバッグ出力失敗は本処理を止めない
            }
        }

        private void DumpBackwardTraceSummary(
    TraceResult result,
    ForwardDisplayLaneBuildResult backwardLaneBuildResult,
    string phase)
        {
            try
            {
                string folder = EnsureDebugFolderExists();
                string path = Path.Combine(folder, "LotTrace_Debug_BackwardTraceSummary.csv");

                var sb = new StringBuilder();
                sb.AppendLine("Phase,AllNodes,AllLinks,StartNodes,MiddleNodes,EndNodes,RootNodes,DisplayNodes");

                int allNodes = result == null || result.AllNodes == null ? 0 : result.AllNodes.Count;
                int allLinks = result == null || result.AllLinks == null ? 0 : result.AllLinks.Count;
                int startNodes = result == null || result.StartNodes == null ? 0 : result.StartNodes.Count;
                int middleNodes = result == null || result.MiddleNodes == null ? 0 : result.MiddleNodes.Count;
                int endNodes = result == null || result.EndNodes == null ? 0 : result.EndNodes.Count;
                int rootNodes = result == null || result.RootNodes == null ? 0 : result.RootNodes.Count;
                int displayNodes = backwardLaneBuildResult == null || backwardLaneBuildResult.DisplayNodes == null
                    ? 0
                    : backwardLaneBuildResult.DisplayNodes.Count;

                sb.AppendLine(string.Join(",",
                    EscapeCsv(phase),
                    allNodes.ToString(),
                    allLinks.ToString(),
                    startNodes.ToString(),
                    middleNodes.ToString(),
                    endNodes.ToString(),
                    rootNodes.ToString(),
                    displayNodes.ToString()));

                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // デバッグ出力失敗は本処理を止めない
            }
        }

        private string EnsureDebugFolderExists()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string debugDir = Path.Combine(baseDir, "Debug/Back");

            if (!Directory.Exists(debugDir))
            {
                Directory.CreateDirectory(debugDir);
            }

            return debugDir;
        }

        private void DumpDisplayLaneNodesToFile(
    List<DisplayLaneNode> nodes,
    string fileName)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            try
            {
                string folder = EnsureDebugFolderExists();
                string path = Path.Combine(folder, fileName);

                var sb = new StringBuilder();

                sb.AppendLine(
                    "DisplayNodeKey,ParentDisplayNodeKey,MergeKey,XLevel,YLane,ChildIndex,SubtreeLaneSpan,RootGroupHeight,ChildBranchHeight,OccupiedFirstY,OccupiedLastY,LotGroupKey,RepresentativeNodeKey,MasterKey,LotNumber,RouteSystem,InputSlotNo,InputSourceType,Depth,NodeType");

                foreach (var node in nodes
                    .Where(x => x != null)
                    .OrderBy(x => x.XLevel)
                    .ThenBy(x => x.YLane)
                    .ThenBy(x => x.ChildIndex))
                {
                    var src = node.SourceNode;

                    sb.Append(EscapeCsv(node.DisplayNodeKey)).Append(",");
                    sb.Append(EscapeCsv(node.ParentDisplayNodeKey)).Append(",");
                    sb.Append(EscapeCsv(node.MergeKey)).Append(",");
                    sb.Append(node.XLevel.ToString()).Append(",");
                    sb.Append(node.YLane.ToString()).Append(",");
                    sb.Append(node.ChildIndex.ToString()).Append(",");
                    sb.Append(node.SubtreeLaneSpan.ToString()).Append(",");
                    sb.Append(node.RootGroupHeight.ToString()).Append(",");
                    sb.Append(node.ChildBranchHeight.ToString()).Append(",");
                    sb.Append(node.OccupiedFirstY.ToString()).Append(",");
                    sb.Append(node.OccupiedLastY.ToString()).Append(",");
                    sb.Append(EscapeCsv(node.LotGroupKey)).Append(",");
                    sb.Append(EscapeCsv(node.RepresentativeNodeKey)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.ControlMasterKey)).Append(",");
                    sb.Append(EscapeCsv(src == null ? null : src.LotNumber)).Append(",");
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

        private void DumpDisplayLaneEdgesToFile(
    List<DisplayLaneNode> nodes,
    string fileName)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            try
            {
                string folder = EnsureDebugFolderExists();
                string path = Path.Combine(folder, fileName);

                var sb = new StringBuilder();

                sb.AppendLine(
                    "OwnerDisplayNodeKey,EdgeIdentityKey,ParentDisplayNodeKey,ChildDisplayNodeKey,ParentMergeKey,ChildMergeKey,FromXLevel,ToXLevel,FromYLane,ToYLane,ChildIndex,LotGroupKey");

                foreach (var node in nodes
                    .Where(x => x != null)
                    .OrderBy(x => x.XLevel)
                    .ThenBy(x => x.YLane))
                {
                    if (node.OutgoingEdges == null || node.OutgoingEdges.Count == 0)
                        continue;

                    foreach (var edge in node.OutgoingEdges
                        .Where(x => x != null)
                        .OrderBy(x => x.FromXLevel)
                        .ThenBy(x => x.FromYLane)
                        .ThenBy(x => x.ChildIndex))
                    {
                        sb.Append(EscapeCsv(node.DisplayNodeKey)).Append(",");
                        sb.Append(EscapeCsv(edge.EdgeIdentityKey)).Append(",");
                        sb.Append(EscapeCsv(edge.ParentDisplayNodeKey)).Append(",");
                        sb.Append(EscapeCsv(edge.ChildDisplayNodeKey)).Append(",");
                        sb.Append(EscapeCsv(edge.ParentMergeKey)).Append(",");
                        sb.Append(EscapeCsv(edge.ChildMergeKey)).Append(",");
                        sb.Append(edge.FromXLevel.ToString()).Append(",");
                        sb.Append(edge.ToXLevel.ToString()).Append(",");
                        sb.Append(edge.FromYLane.ToString()).Append(",");
                        sb.Append(edge.ToYLane.ToString()).Append(",");
                        sb.Append(edge.ChildIndex.ToString()).Append(",");
                        sb.Append(EscapeCsv(edge.LotGroupKey));
                        sb.AppendLine();
                    }
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

        private void WriteDebugCsvLines(string fileName, List<string> lines)
        {
            if (string.IsNullOrWhiteSpace(fileName) || lines == null)
                return;

            string debugDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debug");
            Directory.CreateDirectory(debugDir);

            string path = Path.Combine(debugDir, fileName);
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        #endregion
    }
}