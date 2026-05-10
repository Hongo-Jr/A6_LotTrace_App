using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LotTraceApp.Models;

namespace LotTraceApp.Repositories
{
    /// <summary>
    /// 瓶設備（MES33）用 DB アクセス
    /// </summary>
    public class BottleTraceRepository
    {
        private readonly string _connectionString;

        public BottleTraceRepository(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            _connectionString = connectionString;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region 検索始点（FillingOrderTable + FillingOrderResultTable）

        /// <summary>
        /// 検索条件に一致する充填オーダ（始点）一覧を取得
        /// 仕様書 7.2 / 図 7.2 の「検索始点データを指定する検索条件を設定」に対応
        /// </summary>
        public List<BottleOrderNode> FindStartOrders(TraceSearchParameters p)
        {
            var list = new List<BottleOrderNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                var sql = @"
SELECT  o.OrderNumber,
        o.ProcessType,
        o.ProductItemName,
        o.ProductItemCode,
        o.ProductLotNumber,
        o.MiddleProductItemCode,
        o.MiddleProductLotNumber,
        r.StartDate,
        r.EndDate
FROM    dbo.FillingOrderTable o
LEFT JOIN dbo.FillingOrderResultTable r
        ON r.OrderNumber = o.OrderNumber
WHERE   1 = 1
";

                if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
                {
                    sql += " AND o.OrderNumber = @OrderNumber";
                    cmd.Parameters.AddWithValue("@OrderNumber", p.ProductionOrderNumber);
                }
                if (!string.IsNullOrWhiteSpace(p.ItemName))
                {
                    sql += " AND o.ProductItemName = @ItemName";
                    cmd.Parameters.AddWithValue("@ItemName", p.ItemName);
                }
                if (!string.IsNullOrWhiteSpace(p.ItemCode))
                {
                    sql += " AND o.ProductItemCode = @ItemCode";
                    cmd.Parameters.AddWithValue("@ItemCode", p.ItemCode);
                }
                if (!string.IsNullOrWhiteSpace(p.LotNumber))
                {
                    sql += " AND o.ProductLotNumber = @LotNumber";
                    cmd.Parameters.AddWithValue("@LotNumber", p.LotNumber);
                }
                if (p.From.HasValue)
                {
                    sql += " AND r.StartDate >= @From";
                    cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = p.From.Value;
                }
                if (p.To.HasValue)
                {
                    sql += " AND r.EndDate <= @To";
                    cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = p.To.Value;
                }

                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new BottleOrderNode();
                        node.OrderNumber = reader.GetString(0);
                        node.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        node.ProductItemName = reader.IsDBNull(2) ? null : reader.GetString(2);
                        node.ProductItemCode = reader.IsDBNull(3) ? null : reader.GetString(3);
                        node.ProductLotNumber = reader.IsDBNull(4) ? null : reader.GetString(4);
                        node.MiddleProductItemCode = reader.IsDBNull(5) ? null : reader.GetString(5);
                        node.MiddleProductLotNumber = reader.IsDBNull(6) ? null : reader.GetString(6);
                        node.StartDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
                        node.EndDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);
                        node.Depth = 0;
                        node.NodeType = "Start";

                        list.Add(node);
                    }
                }
            }

            return list;
        }

        #endregion

        #region 充填結果（FillingBottleTable / FillingDrumcanTable）

        /// <summary>
        /// 指定したオーダ群に対するボトル／ドラムの充填結果を取得
        /// 仕様書 7.2 / 7.4 の「ロットトレース下流側（上流側）の充填情報一覧」に対応
        /// </summary>
        public List<BottleFillingNode> FindFillingsByOrders(
            IEnumerable<BottleOrderNode> orders,
            TraceDirection direction,
            DateTime? from,
            DateTime? to)
        {
            var list = new List<BottleFillingNode>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                foreach (var order in orders)
                {
                    // --- ボトル ---
                    using (var cmd = conn.CreateCommand())
                    {
                        var sql = @"
SELECT  OrderNumber, ProcessType,
        ProductItemCode, ProductLotNumber,
        MiddleProductItemCode, MiddleProductLotNumber,
        BottleID, FillingMachineNumber, FillingNozzleNumber,
        FillingWeight, FillingStartDate, FillingEndDate
FROM    dbo.FillingBottleTable
WHERE   OrderNumber = @OrderNumber
";

                        // 期間指定（仕様書上は「対象期間」で検索）
                        if (from.HasValue)
                        {
                            sql += " AND FillingStartDate >= @From";
                            cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = from.Value;
                        }
                        if (to.HasValue)
                        {
                            sql += " AND FillingEndDate <= @To";
                            cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = to.Value;
                        }

                        // 品目コード / ロット番号指定があれば絞り込み
                        if (!string.IsNullOrWhiteSpace(order.ProductItemCode))
                        {
                            sql += " AND ProductItemCode = @PItemCode";
                            cmd.Parameters.AddWithValue("@PItemCode", order.ProductItemCode);
                        }
                        if (!string.IsNullOrWhiteSpace(order.ProductLotNumber))
                        {
                            sql += " AND ProductLotNumber = @PLot";
                            cmd.Parameters.AddWithValue("@PLot", order.ProductLotNumber);
                        }

                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var node = new BottleFillingNode();
                                node.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                                node.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                                node.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
                                node.ProductLotNumber = reader.IsDBNull(3) ? null : reader.GetString(3);
                                node.MiddleProductItemCode = reader.IsDBNull(4) ? null : reader.GetString(4);
                                node.MiddleProductLotNumber = reader.IsDBNull(5) ? null : reader.GetString(5);
                                node.BottleIdOrDrumcanNumber = reader.IsDBNull(6) ? null : reader.GetString(6);
                                node.FillingMachineNumber = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7);
                                node.FillingNozzleNumber = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8);
                                node.FillingWeight = reader.IsDBNull(9) ? (long?)null : reader.GetInt64(9);
                                node.FillingStartDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10);
                                node.FillingEndDate = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
                                node.FillingType = "Bottle";
                                node.NodeType = "End";
                                node.Depth = 1;

                                list.Add(node);
                            }
                        }
                    }

                    // --- ドラム缶 ---
                    using (var cmd = conn.CreateCommand())
                    {
                        var sql = @"
SELECT  OrderNumber, ProcessType,
        ProductItemCode, ProductLotNumber,
        MiddleProductItemCode, MiddleProductLotNumber,
        DrumcanNumber, FillingNozzleNumber,
        FillingWeight, FillingStartDate, FillingEndDate
FROM    dbo.FillingDrumcanTable
WHERE   OrderNumber = @OrderNumber
";

                        if (from.HasValue)
                        {
                            sql += " AND FillingStartDate >= @From";
                            cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = from.Value;
                        }
                        if (to.HasValue)
                        {
                            sql += " AND FillingEndDate <= @To";
                            cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = to.Value;
                        }
                        if (!string.IsNullOrWhiteSpace(order.ProductItemCode))
                        {
                            sql += " AND ProductItemCode = @PItemCode";
                            cmd.Parameters.AddWithValue("@PItemCode", order.ProductItemCode);
                        }
                        if (!string.IsNullOrWhiteSpace(order.ProductLotNumber))
                        {
                            sql += " AND ProductLotNumber = @PLot";
                            cmd.Parameters.AddWithValue("@PLot", order.ProductLotNumber);
                        }

                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var node = new BottleFillingNode();
                                node.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                                node.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                                node.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
                                node.ProductLotNumber = reader.IsDBNull(3) ? null : reader.GetString(3);
                                node.MiddleProductItemCode = reader.IsDBNull(4) ? null : reader.GetString(4);
                                node.MiddleProductLotNumber = reader.IsDBNull(5) ? null : reader.GetString(5);
                                node.BottleIdOrDrumcanNumber = reader.GetInt32(6).ToString();
                                node.FillingNozzleNumber = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7);
                                node.FillingWeight = reader.IsDBNull(8) ? (long?)null : reader.GetInt64(8);
                                node.FillingStartDate = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9);
                                node.FillingEndDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10);
                                node.FillingType = "Drumcan";
                                node.NodeType = "End";
                                node.Depth = 1;

                                list.Add(node);
                            }
                        }
                    }
                }
            }

            // 方向によってソート順を変更（フォワード=昇順, バック=降順）
            list.Sort(
                delegate (BottleFillingNode a, BottleFillingNode b)
                {
                    int cmp = Nullable.Compare(a.FillingStartDate, b.FillingStartDate);
                    return direction == TraceDirection.Forward ? cmp : -cmp;
                });

            return list;
        }

        #endregion
    }
}