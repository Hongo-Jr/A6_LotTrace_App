using LotTraceApp.Models;
using LotTraceApp.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;


namespace LotTraceApp.Services
{
    /// <summary>
    /// 瓶設備ロットトレースの実装
    /// 図 7.2 / 7.4 のフローをそのままコード化（単段検索）
    /// </summary>
    public class BottleTraceService
    {
        private readonly BottleTraceRepository _repo;
        private readonly ICustomerItemMasterRepository _customerItemMasterRepository;

     
        public BottleTraceService(BottleTraceRepository repo, ICustomerItemMasterRepository customerItemMasterRepository)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _customerItemMasterRepository = customerItemMasterRepository;
        }

        private bool ResolveItemNameCondition(TraceSearchParameters p)
        {
            if (p == null)
                return true;

            if (string.IsNullOrWhiteSpace(p.ItemName))
                return true;

            p.ResolvedItemCodes =
                _customerItemMasterRepository.GetItemCodeByName(p.ItemName);

            p.ItemCode = null; // ItemName優先

            return p.ResolvedItemCodes != null &&
                   p.ResolvedItemCodes.Count > 0;
        }

        #region フォワード


        public BottleTraceResult B_TraceForwardResult(
            TraceSearchParameters p,
            IProgress<TraceProgressState>? progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (p == null) throw new ArgumentNullException("p");

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "品目名条件を解決しています...", 12);

            if (!ResolveItemNameCondition(p))
            {
                return new BottleTraceResult();
            }

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "液設備から瓶設備への候補を取得しています...", 25);

            //検索条件からCandidates作成
            var candidate = _repo.B_FindForwardCandidate(p);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "表示レーンを構築しています...", 62);

            //Candidateをグループ化したDisplayNodeにする。
            var displayGroups = B_BuildDisplaylane(candidate, progress, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "液設備情報を補完しています...", 70);
            ResolveLiquidNodeComplements(displayGroups);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "グリッド用データを作成しています...", 78);
            var result = B_BuildDisplayTable(displayGroups, progress, cancellationToken);

            return new BottleTraceResult
            {
                DisplayTables = result,
                DisplayGroups = displayGroups
            };

        }


        #endregion

        #region バック

        
        public BottleTraceResult B_TraceBackwardResult(
            TraceSearchParameters p,
            IProgress<TraceProgressState>? progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            if (p == null) throw new ArgumentNullException("p");

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "品目名条件を解決しています...", 12);

            if (!ResolveItemNameCondition(p))
            {
                return new BottleTraceResult();
            }

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "瓶設備から液設備への候補を取得しています...", 25);

            //検索条件からCandidates作成
            var candidate = _repo.B_FindBackwardCandidate(p);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "表示レーンを構築しています...", 62);

            //Candidateをグループ化したDisplayNodeにする。
            var displayGroups = B_BuildDisplaylane(candidate, progress, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "液設備情報を補完しています...", 70);
            ResolveLiquidNodeComplements(displayGroups);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "グリッド用データを作成しています...", 78);
            var result = B_BuildDisplayTable(displayGroups, progress, cancellationToken);

            return new BottleTraceResult
            {
                DisplayTables = result,
                DisplayGroups = displayGroups
            };

        }

        #endregion




        #region 汎用


        private static void ReportProgress(IProgress<TraceProgressState>? progress, string message, int? percent = null)
        {
            if (progress != null)
                progress.Report(new TraceProgressState(message, percent));
        }

        private List<BottleDisplayGroup> B_BuildDisplaylane(
            List<BottleCandidate> candidates,
            IProgress<TraceProgressState>? progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<BottleDisplayGroup>();
            int StartY = 0;
            int total = candidates == null ? 0 : candidates.Count;
            int index = 0;

            foreach (var candidate in candidates ?? new List<BottleCandidate>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (total > 0 && index % 20 == 0)
                    ReportProgress(progress, "表示レーンを構築しています...", 62 + Math.Min(6, index * 6 / total));

                var DisplayGroup = new BottleDisplayGroup();
                DisplayGroup.LiquidNodes.AddRange(BuildDisplayNodes(candidate.LiquidNodes, StartY, cancellationToken));
                DisplayGroup.BottleNodes.AddRange(BuildDisplayNodes(candidate.BottleNodes, StartY, cancellationToken));
                DisplayGroup.StartY = StartY;

                int liquidY = DisplayGroup.LiquidNodes.Count;
                int bottleY = DisplayGroup.BottleNodes.Count;
                int nextY = Math.Max(liquidY, bottleY);

                DisplayGroup.EndY = StartY + nextY;

                StartY += nextY;

                result.Add(DisplayGroup);
                index++;

            }

            return result;
        }

        private void ResolveLiquidNodeComplements(List<BottleDisplayGroup>? displayGroups)
        {
            if (displayGroups == null || displayGroups.Count == 0)
                return;

            var nodes = displayGroups
                .Where(g => g != null && g.LiquidNodes != null)
                .SelectMany(g => g.LiquidNodes)
                .Where(x => x != null && x.SourceLiquidNode != null)
                .Select(x => x.SourceLiquidNode)
                .ToList();

            if (nodes.Count == 0)
                return;

            ResolveLiquidItemNames(nodes);
            ResolveLiquidStartDateLabels(nodes.Where(IsRouteSystemA));
        }

        private bool IsRouteSystemA(ProductionResultNode? node)
        {
            return node != null &&
                string.Equals(node.RouteSystem, "A", StringComparison.OrdinalIgnoreCase);
        }

        private void ResolveLiquidItemNames(
    IEnumerable<ProductionResultNode?>? nodes)
        {
            if (nodes == null || _customerItemMasterRepository == null)
            {
                return;
            }

            var nodeList = nodes
                .OfType<ProductionResultNode>()
                .Where(node => !string.IsNullOrWhiteSpace(node.ItemCode))
                .ToList();

            if (nodeList.Count == 0)
            {
                return;
            }

            var itemCodes = nodeList
                .Select(node => node.ItemCode!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            Dictionary<string, string> itemNameMap;

            try
            {
                itemNameMap =
                    _customerItemMasterRepository.GetItemNamesByCodes(itemCodes);
            }
            catch
            {
                return;
            }

            foreach (var node in nodeList)
            {
                var itemCode = node.ItemCode;

                if (string.IsNullOrWhiteSpace(itemCode))
                {
                    continue;
                }

                if (itemNameMap.TryGetValue(
                    itemCode.Trim(),
                    out var itemName))
                {
                    node.ItemName = itemName;
                }
            }
        }

        private void ResolveLiquidStartDateLabels(IEnumerable<ProductionResultNode?> nodes)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
            {
                if (node == null)
                    continue;

                if (!IsRouteSystemA(node))
                    continue;

                if (!string.IsNullOrWhiteSpace(node.StartDateLabel))
                    continue;

                if (node.StartDate.HasValue)
                    continue;

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

        private List<BottleDisplayLaneNode > BuildDisplayNodes(
            List<ProductionResultNode> liquidNodes,
            int BaseY,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<BottleDisplayLaneNode>();
            int currentY = BaseY;
            foreach (var node in liquidNodes ?? new List<ProductionResultNode>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var DisplayNode = new BottleDisplayLaneNode();

                DisplayNode.NodeType = 0;
                DisplayNode.YLane = currentY;
                DisplayNode.SourceLiquidNode = node;
                DisplayNode.DisplayNodeKey = String.Join("|", "L",currentY.ToString(), node.NodeIdentityKey); 

                currentY++;

                result.Add(DisplayNode);
            }

            return result;
        }

        private List<BottleDisplayLaneNode> BuildDisplayNodes(
            List<Bottle_ProductionResultNode> liquidNodes,
            int BaseY,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<BottleDisplayLaneNode>();
            int currentY = BaseY;
            foreach (var node in liquidNodes ?? new List<Bottle_ProductionResultNode>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var DisplayNode = new BottleDisplayLaneNode();

                DisplayNode.NodeType = 1;
                DisplayNode.YLane = currentY;
                DisplayNode.SourceBottleNode = node;
                DisplayNode.DisplayNodeKey = String.Join("|", "B", currentY.ToString(), node.NodeIdentifyKey);

                currentY++;

                result.Add(DisplayNode);
            }

            return result;
        }

        private BottleDisplayTables B_BuildDisplayTable(
            List<BottleDisplayGroup> groups,
            IProgress<TraceProgressState>? progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var liquid = new DataTable();
            var bottle = new DataTable();
            var tables = new BottleDisplayTables(liquid, bottle);

            B_AddTableColumns(tables);

            int liquidBaseY = 0;
            int bottleBaseY = 0;
            int total = groups == null ? 0 : groups.Count;
            int index = 0;

            foreach (var group in groups ?? new List<BottleDisplayGroup>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (total > 0 && index % 20 == 0)
                    ReportProgress(progress, "グリッド用データを作成しています...", 78 + Math.Min(10, index * 10 / total));

                liquidBaseY = B_SetLiquidTable(tables, group.LiquidNodes, liquidBaseY, cancellationToken);
                bottleBaseY = B_SetBottleTable(tables, group.BottleNodes, bottleBaseY, cancellationToken);

                var groupLine = new BottleLineRanges();
                groupLine.BorderType = 0;
                groupLine.BorderIndex = group.EndY;
                tables.LineRanges.Add(groupLine);
                index++;
                
            }

            cancellationToken.ThrowIfCancellationRequested();
            B_AdjustTableRow(tables);



            return tables;
        }

        private void B_AddTableColumns(BottleDisplayTables tables)
        {

            //液
            tables.LiquidTable.Columns.Add("OrderNumber",typeof(string)).Caption = "指図番号";
            tables.LiquidTable.Columns.Add("Lot", typeof(string)).Caption = "ロットNo,";
            tables.LiquidTable.Columns.Add("ItemName", typeof(string)).Caption = "品目名";
            tables.LiquidTable.Columns.Add("StartDate", typeof(string)).Caption = "開始日時";
            tables.LiquidTable.Columns.Add("Weight", typeof(decimal)).Caption = "重量";

            tables.LiquidTable.Columns.Add("NodeKey", typeof(string));
            tables.LiquidTable.Columns.Add("DisplayKey", typeof(string));
            tables.LiquidTable.Columns.Add("ItemCode", typeof(string));
            tables.LiquidTable.Columns.Add("MasterKey", typeof(string));
            tables.LiquidTable.Columns.Add("StartDateLabel", typeof(string));
            tables.LiquidTable.Columns.Add("InputSourceType", typeof(string));


            //瓶
            tables.BottleTable.Columns.Add("OrderNumber",typeof(string)).Caption = "指図番号";
            tables.BottleTable.Columns.Add("Lot", typeof(string)).Caption = "ロットNo,";
            tables.BottleTable.Columns.Add("ItemName", typeof(string)).Caption = "品目名";
            tables.BottleTable.Columns.Add("StartDate", typeof(string)).Caption = "開始日時";
            tables.BottleTable.Columns.Add("OK_Num", typeof(int)).Caption = "充填本数(OK)";
            tables.BottleTable.Columns.Add("NG_Num", typeof(int)).Caption = "充填本数(NG)";
            tables.BottleTable.Columns.Add("Total_Num", typeof(int)).Caption = "充填本数";

            tables.BottleTable.Columns.Add("NodeKey", typeof(string));
            tables.BottleTable  .Columns.Add("DisplayKey", typeof(string));
            tables.BottleTable.Columns.Add("ItemCode", typeof(string));
            tables.BottleTable.Columns.Add("MasterKey", typeof(string));
            tables.BottleTable.Columns.Add("StartDateLabel", typeof(string));
            tables.BottleTable.Columns.Add("InputSourceType", typeof(string));



        }

        private int B_SetLiquidTable(
            BottleDisplayTables tables,
            List<BottleDisplayLaneNode> liquids,
            int BaseY,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int currentY = BaseY;

            foreach(var node in liquids.OrderBy(x => x.YLane))
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (currentY < node.YLane)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    tables.LiquidTable.Rows.Add(tables.LiquidTable.NewRow());
                    currentY++;
                }

                B_SetLiquidNode(tables,node);
                currentY++;

            }

            int groupY = currentY;
            return groupY;

        }

        public void B_SetLiquidNode(
    BottleDisplayTables tables,
    BottleDisplayLaneNode liquid)
        {
            var sourceNode = liquid.SourceLiquidNode
                ?? throw new InvalidOperationException(
                    "BottleDisplayLaneNode.SourceLiquidNodeが設定されていません。");

            string? orderNumber = sourceNode.ProductionOrderNumber;
            string? lot = sourceNode.LotNumber;
            string? itemName = sourceNode.ItemName;

            string? startDate = sourceNode.StartDate.HasValue
                ? sourceNode.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                : sourceNode.StartDateLabel;

            object weight = sourceNode.Weight.HasValue
                ? sourceNode.Weight.Value
                : DBNull.Value;

            string nodeKey = sourceNode.NodeIdentityKey;
            string? displayKey = liquid.DisplayNodeKey;
            string? itemCode = sourceNode.ItemCode;
            string? masterKey = sourceNode.ControlMasterKey;
            string? startDateLabel = sourceNode.StartDateLabel;
            string? inputSourceType = sourceNode.InputSourceType;

            tables.LiquidTable.Rows.Add(
                orderNumber,
                lot,
                itemName,
                startDate,
                weight,
                nodeKey,
                displayKey,
                itemCode,
                masterKey,
                startDateLabel,
                inputSourceType);
        }


        private int B_SetBottleTable(
            BottleDisplayTables tables,
            List<BottleDisplayLaneNode> bottles,
            int BaseY,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            int currentY = BaseY;
            

            foreach (var node in bottles.OrderBy(x=>x.YLane))
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (currentY < node.YLane)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    tables.BottleTable.Rows.Add(tables.BottleTable.NewRow());
                    currentY++;
                }

                B_SetBottleNode(tables, node);
                currentY++;

            }

            int groupY = currentY;
            return groupY;

        }

        private void B_SetBottleNode(
    BottleDisplayTables tables,
    BottleDisplayLaneNode bottle)
        {
            var sourceNode = bottle.SourceBottleNode
                ?? throw new InvalidOperationException(
                    "BottleDisplayLaneNode.SourceBottleNodeが設定されていません。");

            string? orderNumber = sourceNode.OrderNumber;
            string? lot = sourceNode.ProductLotNumber;
            string? itemName = sourceNode.ProductItemName;
            string? startDate = sourceNode.StartDate.ToString();

            int okNum = sourceNode.FillingBottleNum_OK;
            int ngNum = sourceNode.FillingBottleNum_NG;
            int totalNum = sourceNode.FillingBottleNum_Total;

            string nodeKey = sourceNode.NodeIdentifyKey;
            string? displayKey = bottle.DisplayNodeKey;
            string? itemCode = sourceNode.ProductItemCode;

            tables.BottleTable.Rows.Add(
                orderNumber,
                lot,
                itemName,
                startDate,
                okNum,
                ngNum,
                totalNum,
                nodeKey,
                displayKey,
                itemCode,
                null,
                null,
                null);
        }

        private void B_AdjustTableRow(BottleDisplayTables tables)
        {
            int MaxRows = Math.Max(tables.LiquidTable.Rows.Count, tables.BottleTable.Rows.Count);

            while (tables.BottleTable.Rows.Count < MaxRows)
            {
                tables.BottleTable.Rows.Add(tables.BottleTable.NewRow());
            }

            while (tables.LiquidTable.Rows.Count < MaxRows)
            {
                tables.LiquidTable.Rows.Add(tables.LiquidTable.NewRow());
            }

        }


        #endregion
    }
}
