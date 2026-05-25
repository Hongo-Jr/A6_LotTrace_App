using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using LotTraceApp.Models;
using LotTraceApp.Repositories;


namespace LotTraceApp.Services
{
    /// <summary>
    /// 瓶設備ロットトレースの実装
    /// 図 7.2 / 7.4 のフローをそのままコード化（単段検索）
    /// </summary>
    public class BottleTraceService
    {
        private readonly BottleTraceRepository _repo;

        public BottleTraceService(BottleTraceRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        #region フォワード

        public BottleDisplayTables B_TraceForward(TraceSearchParameters p)
        {
            
            //検索条件からCandidates作成
            var candidate = _repo.B_FindForwardCandidate(p);

            //Candidateをグループ化したDisplayNodeにする。
            var displayGroups = BuildForwardDisplaylane(candidate);

            var result = B_BuildDisplayTable(displayGroups);

            return result;


        }

        private List<BottleDisplayGroup> BuildForwardDisplaylane(List<BottleCandidate> candidates)
        {
            var result = new List<BottleDisplayGroup>();
            int StartY = 0;

            foreach (var candidate in candidates)
            {
                var DisplayGroup = new BottleDisplayGroup();
                DisplayGroup.LiquidNodes.AddRange(BuildDisplayNodes(candidate.LiquidNodes,StartY));
                DisplayGroup.BottleNodes.AddRange(BuildDisplayNodes(candidate.BottleNodes, StartY));
                DisplayGroup.StartY = StartY;

                int liquidY = DisplayGroup.LiquidNodes.Count;
                int bottleY = DisplayGroup.BottleNodes.Count;
                int nextY = Math.Max(liquidY, bottleY);

                DisplayGroup.EndY = StartY + nextY;

                StartY += nextY;

                result.Add(DisplayGroup);

            }
            
            return result;  
        }

        #endregion



        #region 汎用

        private List<BottleDisplayLaneNode > BuildDisplayNodes(List<ProductionResultNode> liquidNodes, int BaseY)
        {
            var result = new List<BottleDisplayLaneNode>();
            int currentY = BaseY;
            foreach (var node in liquidNodes)
            {
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

        private List<BottleDisplayLaneNode> BuildDisplayNodes(List<Bottle_ProductionResultNode> liquidNodes, int BaseY)
        {
            var result = new List<BottleDisplayLaneNode>();
            int currentY = BaseY;
            foreach (var node in liquidNodes)
            {
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

        private BottleDisplayTables B_BuildDisplayTable(List<BottleDisplayGroup> groups)
        {
            var liquid = new DataTable();
            var bottle = new DataTable();
            var tables = new BottleDisplayTables(liquid, bottle);

            B_AddTableColumns(tables);

            int liquidBaseY = 0;
            int bottleBaseY = 0;

            foreach (var group in groups)
            {
                liquidBaseY = B_SetLiquidTable(tables,group.LiquidNodes,liquidBaseY);
                bottleBaseY = B_SetBottleTable(tables, group.BottleNodes,bottleBaseY);

                var groupLine = new BottleLineRanges();
                groupLine.BorderType = 0;
                groupLine.BorderIndex = group.EndY;
                tables.LineRanges.Add(groupLine);
                
            }

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

        }

        private int B_SetLiquidTable(BottleDisplayTables tables,List<BottleDisplayLaneNode> liquids,int BaseY)
        {
            int currentY = BaseY;

            foreach(var node in liquids.OrderBy(x => x.YLane))
            {
                while (currentY < node.YLane)
                {
                    tables.LiquidTable.Rows.Add(tables.LiquidTable.NewRow());
                    currentY++;
                }

                B_SetLiquidNode(tables,node);
                currentY++;

            }

            int groupY = currentY;
            return groupY;

        }

        public void B_SetLiquidNode(BottleDisplayTables tables, BottleDisplayLaneNode liquid)
        {
            string OrderNumber = liquid.SourceLiquidNode.ProductionOrderNumber;
            string Lot = liquid.SourceLiquidNode.LotNumber;
            string ItemName = liquid.SourceLiquidNode.ItemName;
            string StartDate = liquid.SourceLiquidNode.StartDate.ToString();
            object Weight = liquid.SourceLiquidNode.Weight == null ? (object)DBNull.Value: liquid.SourceLiquidNode.Weight;
            string NodeKey = liquid.SourceLiquidNode.NodeIdentityKey;
            string DisplayKey = liquid.DisplayNodeKey;


            tables.LiquidTable.Rows.Add(OrderNumber,Lot, ItemName, StartDate, Weight,NodeKey,DisplayKey);

        }


        private int B_SetBottleTable(BottleDisplayTables tables, List<BottleDisplayLaneNode> bottles,int BaseY)
        {

            int currentY = BaseY;
            

            foreach (var node in bottles.OrderBy(x=>x.YLane))
            {
                while (currentY < node.YLane)
                {
                    tables.BottleTable.Rows.Add(tables.BottleTable.NewRow());
                    currentY++;
                }

                B_SetBottleNode(tables, node);
                currentY++;

            }

            int groupY = currentY;
            return groupY;

        }

        private void B_SetBottleNode(BottleDisplayTables tables, BottleDisplayLaneNode bottle)
        {
            string OrderNumber = bottle.SourceBottleNode.OrderNumber;
            string Lot = bottle.SourceBottleNode.ProductLotNumber;
            string ItemName = bottle.SourceBottleNode.ProductItemName;
            string StartDate = bottle.SourceBottleNode.StartDate.ToString();
            int OK_Num = bottle.SourceBottleNode.FillingBottleNum_OK;
            int NG_Num = bottle.SourceBottleNode.FillingBottleNum_NG;
            int Total_Num = bottle.SourceBottleNode.FillingBottleNum_Total;

            string NodeKey = bottle.SourceBottleNode.NodeIdentifyKey;
            string DisplayKey = bottle.DisplayNodeKey;

            tables.BottleTable.Rows.Add(OrderNumber, Lot, ItemName, StartDate,OK_Num,NG_Num, Total_Num,NodeKey,DisplayKey);
        }

        #endregion
    }
}