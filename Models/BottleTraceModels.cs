using System;
using System.Collections.Generic;

namespace LotTraceApp.Models
{
    /// <summary>
    /// 瓶設備の 1 つの充填オーダを表すノード（検索始点用）
    /// </summary>
    public class BottleOrderNode
    {
        public string OrderNumber { get; set; }              // オーダ番号
        public string ProcessType { get; set; }              // 工程種別
        public string ProductItemName { get; set; }          // 製品名
        public string ProductItemCode { get; set; }          // 製品コード
        public string ProductLotNumber { get; set; }         // 製品ロット
        public string MiddleProductItemCode { get; set; }    // 中間品コード
        public string MiddleProductLotNumber { get; set; }   // 中間品ロット
        public DateTime? StartDate { get; set; }             // オーダ開始日時
        public DateTime? EndDate { get; set; }               // オーダ終了日時

        public int Depth { get; set; } = 0;                  // 仕様書上は単段だが液設備と揃えておく
        public string NodeType { get; set; } = "Start";      // 固定で Start
    }

    /// <summary>
    /// 1 本のボトル or 1 本のドラム缶の充填結果
    /// </summary>
    public class BottleFillingNode
    {
        public string OrderNumber { get; set; }
        public string ProcessType { get; set; }

        // 製品
        public string ProductItemCode { get; set; }
        public string ProductLotNumber { get; set; }

        // 中間品（液設備との連携で使う可能性あり）
        public string MiddleProductItemCode { get; set; }
        public string MiddleProductLotNumber { get; set; }

        // 識別子（ボトルID or ドラム番号）
        public string BottleIdOrDrumcanNumber { get; set; }

        public string FillingType { get; set; }   // "Bottle" or "Drumcan"
        public int? FillingMachineNumber { get; set; }
        public int? FillingNozzleNumber { get; set; }

        public long? FillingWeight { get; set; }
        public DateTime? FillingStartDate { get; set; }
        public DateTime? FillingEndDate { get; set; }

        public int Depth { get; set; } = 1;       // 始点の 1 段下
        public string NodeType { get; set; } = "End";
    }

    /// <summary>
    /// 瓶設備トレース結果
    /// 「始点（オーダ）」＋「終点（ボトル／ドラム）」のみ
    /// </summary>
    public class BottleTraceResult
    {
        // 修正後（C# 7.3 で使える書き方）
        public List<BottleOrderNode> StartOrders { get; } = new List<BottleOrderNode>();
        public List<BottleFillingNode> Fillings { get; } = new List<BottleFillingNode>();
    }
}