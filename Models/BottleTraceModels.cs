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
        public string MasterKey { get; set; } = string.Empty;
        public string? ProcessType { get; set; }
        public string? OrderNumber { get; set; }
        public string? ProductItemName { get; set; }
        public string? ProductItemCode { get; set; }
        public string? ProductLotNumber { get; set; }
        public string? MiddleProductItemCode { get; set; }
        public string? MiddleProductLotNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int FillingBottleNum_OK { get; set; }
        public int FillingBottleNum_NG { get; set; }
        public int FillingBottleNum_Total
        {
            get { return FillingBottleNum_OK + FillingBottleNum_NG; }
        }

        /// <summary>
        /// 不要。後で削除
        /// </summary>
        public string? RouteSystem { get; set; }

        //public int Depth { get; set; }
        //public string? NodeType { get; set; }
        public string NodeIdentifyKey => MasterKey;
    }

    public class BottleTraceResult
    {
        public DataTable BottleTable { get; set; } = new DataTable();
        public List<Bottle_ProductionResultNode> BottleNodes { get; set; } = new List<Bottle_ProductionResultNode>();
        public List<BottleLineRanges>? LineRangesNew { get; set; }
    }
   
    public class BottleLineRanges
    {
        public int BorderType { get; set; }
        public int BorderIndex {  get; set; }
    }

    #region 瓶詳細

    public class BottleRowSettings
    {
        public int SetNo { get; set; }
        public bool Visility { get; set; }
        public int Index { get; set; }

    }

    public class FillingResultTable
    {
        public DataTable BottleTable { get; set; }
        public DataTable DrumTable { get; set; }
        public int TotalCount { get; set; }
        public int MaxPageNo { get; set; }
        public int PageNo { get; set; }

        public FillingResultTable()
        {
            BottleTable = new DataTable();
            DrumTable = new DataTable();
        }

        public void Clear()
        {
            BottleTable.Clear();
            DrumTable.Clear();
        }
    }

    public class BottleNodeInfo
    {
        public string? OrderNumber {  get; set; }
        public string? ProductLotNo {  get; set; }
        public string? ProductItemCode { get; set; }
        public string? ProductItemName { get; set; }
        public string MasterKey {  get; set; } = string.Empty;
    }

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
        public string? OrderNumber { get; set; }
        public string? ProcessType { get; set; }
        public string? ProductLotNumber { get; set; }
        public string? ProductItemCode { get; set; }
        public string? MiddleProductLotNumber { get; set; }
        public string? MiddleProductItemCode { get; set; }

        public string? BottleID { get; set; }

        public string? SamplingGroup { get; set; }
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
        public string? OrderNumber { get; set; }
        public string? ProcessType { get; set; }
        public string? ProductLotNumber { get; set; }
        public string? ProductItemCode { get; set; }
        public string? MiddleProductLotNumber { get; set; }
        public string? MiddleProductItemCode { get; set; }

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

    public class BottleTraceGridRow
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string? ProcessType { get; set; }
        public string? ProductLotNumber { get; set; }
        public string? ProductItemCode { get; set; }
        public string? MiddleProductLotNumber { get; set; }
        public string? MiddleProductItemCode { get; set; }
        public DateTime? StartDate { get; set; }             
        public DateTime? EndDate { get; set; }              
        public int FillingBottleNum_OK { get; set; }
        public int FillingBottleNum_NG { get; set; }
        public int FillingBottleNum_Total
        {
            get { return FillingBottleNum_OK + FillingBottleNum_NG; }
        }

    }


    #endregion
}
