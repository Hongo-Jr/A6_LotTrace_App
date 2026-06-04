using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace LotTraceApp.Models
{
    
    /// <summary>
    /// 瓶の実績Node
    /// </summary>
    public class Bottle_ProductionResultNode
    {
        public string ProcessType { get; set; }
        public string OrderNumber { get; set; }              // 指図番号＞＞FillingOrderResultから取れる
        public string ProductItemName { get; set; }          // 製品名＞＞FillngBottelTable、またはFillngDrumcanTableから取れる
        public string ProductItemCode { get; set; }          // 製品コード＞＞FillngBottelTable、またはFillngDrumcanTableから取れる
        public string ProductLotNumber { get; set; }         // 製品ロット＞＞FillngBottelTable、またはFillngDrumcanTableから取れる
        public string MiddleProductItemCode { get; set; }    // 中間品コード＞＞FillngBottelTable、またはFillngDrumcanTableから取れる
        public string MiddleProductLotNumber { get; set; }   // 中間品ロット＞＞FillngBottelTable、またはFillngDrumcanTableから取れる
        public DateTime? StartDate { get; set; }             // 開始日時＞＞FillingOrderResultから取れる
        public DateTime? EndDate { get; set; }               // 終了日時＞＞FillingOrderResultから取れる
        public int FillingBottleNum_OK { get; set; }　　　 // 充填本数NG＞＞FillingOrderResultから取れる
        public int FillingBottleNum_NG { get; set; }　　　 // 充填本数OK＞＞FillingOrderResultから取れる
        public int FillingBottleNum_Total                   // 充填本数合計>>OKとNGの総数から合成
        {
            get { return FillingBottleNum_OK + FillingBottleNum_NG; }
        }

        public string RouteSystem { get; set; }　　　　　　 // FillngBottelTableからか、FillngDrumcanTableからか

        public int Depth { get; set; }
        public string NodeType { get; set; }

        public string NodeIdentifyKey
        {
            get
            {
                string orderNumber = string.IsNullOrWhiteSpace(OrderNumber) ? "" : OrderNumber.Trim().ToUpperInvariant();
                string lotNumber = string.IsNullOrWhiteSpace(ProductLotNumber) ? "" : ProductLotNumber.Trim().ToUpperInvariant();
                string itemcode = string.IsNullOrWhiteSpace(RouteSystem) ? "" : RouteSystem.Trim().ToUpperInvariant();
                string routeSystem = string.IsNullOrWhiteSpace(RouteSystem) ? "" : RouteSystem.Trim().ToUpperInvariant();
                string processType = string.IsNullOrWhiteSpace(ProcessType) ? "" : ProcessType.Trim().ToUpperInvariant();

                return string.Join("|", routeSystem, orderNumber);
            }
        }
    }


    public class BottleCandidate
    {
        public bool TraceDirection {  get; set; }

        public List<ProductionResultNode> LiquidNodes {  get; set; }
        public List<Bottle_ProductionResultNode> BottleNodes { get; set; }

        public BottleCandidate()
        {
            LiquidNodes = new List<ProductionResultNode>();
            BottleNodes = new List<Bottle_ProductionResultNode>();
        }
    }

    public class BottleDisplayLaneNode
    {
        public int NodeType {  get; set; }

        public string DisplayNodeKey { get; set; }

        public ProductionResultNode SourceLiquidNode {  get; set; }
        public Bottle_ProductionResultNode SourceBottleNode { get; set; }

        public int YLane { get; set; }
    }

    public class BottleDisplayGroup
    {
        public List<BottleDisplayLaneNode> LiquidNodes { get; set; }
        public List<BottleDisplayLaneNode> BottleNodes { get; set; }

        public int StartY { get; set; }
        public int EndY { get; set; }

        public BottleDisplayGroup()
        {
            LiquidNodes = new List<BottleDisplayLaneNode>();
            BottleNodes = new List<BottleDisplayLaneNode>();
        }

    }

    public class BottleDisplayTables
    {
        public DataTable LiquidTable { get; set; }
        public DataTable BottleTable { get; set; }

        public List<BottleLineRanges> LineRanges { get; set; }

        public BottleDisplayTables(DataTable liqied, DataTable bottle)
        {
            LiquidTable = liqied;
            BottleTable = bottle; 
            LineRanges = new List<BottleLineRanges>();
        }

        public bool IsEmpty
        {
            get
            {
                return (LiquidTable?.Rows?.Count ?? 0) == 0
                    && (BottleTable?.Rows?.Count ?? 0) == 0;
            }
        }
    }

    public class BottleTraceResult
    {
        public BottleDisplayTables DisplayTables { get; set; }
        public List<BottleDisplayGroup> DisplayGroups { get; set; }

        public BottleTraceResult()
        {
            
            DisplayGroups = new List<BottleDisplayGroup>();
        }

        public bool IsEmpty
        {
            get
            {
                return DisplayTables == null || DisplayTables.IsEmpty;
            }
        }
    }
   
    public class BottleLineRanges
    {
        public int BorderType { get; set; }
        public int BorderIndex {  get; set; }
    }

    #region 瓶詳細

    public class BottleResultPage
    {
        public List<FillingBottleRow> BottleRows { get; set; } = new List<FillingBottleRow>();
        public List<FillingDrumcanRow> DrumRows {  get; set; } = new List<FillingDrumcanRow>();

        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class FillingBottleRow
    {
        public string OrderNumber { get; set; }
        public string ProcessType { get; set; }
        public string ProductLotNumber { get; set; }
        public string ProductItemCode { get; set; }
        public string MiddleProductLotNumber { get; set; }
        public string MiddleProductItemCode { get; set; }

        public string BottleID { get; set; } = "";

        public string SamplingGroup { get; set; }
        public int? BottleINumber { get; set; }
        public int? FillingNozzleNumber { get; set; }
        public int? CapTighteningTorqueValue { get; set; }
        public int? CapTighteningTorqueJudgment { get; set; }
        public int? CapTiltDetectionJudgment { get; set; }
        public int? FillingMachineNumber { get; set; }
        public int? TotalCahckJudgment { get; set; }
        public int? BottleLocation { get; set; }
        public long? FillingWeight { get; set; }
        public TimeSpan? FillingTime { get; set; }
        public DateTime? FillingStartDate { get; set; }
        public DateTime? FillingEndDate { get; set; }
    }

    public class FillingDrumcanRow
    {
        public string OrderNumber { get; set; }
        public string ProcessType { get; set; }
        public string ProductLotNumber { get; set; }
        public string ProductItemCode { get; set; }
        public string MiddleProductLotNumber { get; set; }
        public string MiddleProductItemCode { get; set; }

        public int DrumcanNumber { get; set; }

        public int? FillingNozzleNumber { get; set; }
        public int? CapTighteningTorqueValue_Big { get; set; }
        public int? CapTighteningTorqueJudgment { get; set; }
        public int? CapTiltDetectionJudgment { get; set; }
        public int? TotalCahckJudgment { get; set; }
        public int? CapTighteningTorqueValue_Small { get; set; }
        public int? FillingWeightJudgment { get; set; }
        public int? BottleLocation { get; set; }
        public long? FillingWeight { get; set; }
        public TimeSpan? FillingTime { get; set; }
        public DateTime? FillingStartDate { get; set; }
        public DateTime? FillingEndDate { get; set; }
    }

    #endregion
}
