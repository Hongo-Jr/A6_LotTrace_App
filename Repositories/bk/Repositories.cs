using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using LotTraceApp.Models;

namespace LotTraceApp.Repositories
{
    /// <summary>
    /// MES31 データベースへのアクセスクラス
    /// </summary>
    public class LotTraceRepository
    {
        private readonly string _connectionString;
        private const int MaxChildTableKeyIndex = 30;


        public LotTraceRepository(string connectionString)
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

        #region 検索起点の取得（ProductionResultsTable）

        


        public List<ProductionResultNode> FindStartNodesFromMaterialTableA(TraceSearchParameters p)
        {
            var list = new List<ProductionResultNode>();
            var uniqueMasterKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                var sql = @"
SELECT
    ma.ForeignKey,   -- 0
    ma.LotNumber,    -- 1
    ma.ItemCode,     -- 2
    ma.MasterKey     -- 3
FROM dbo.MaterialTableA ma
WHERE 1 = 1
";

                if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
                {
                    sql += " AND ma.ForeignKey LIKE @Order";
                    cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
                }

                if (!string.IsNullOrWhiteSpace(p.LotNumber))
                {
                    sql += " AND ma.LotNumber LIKE @Lot";
                    cmd.Parameters.AddWithValue("@Lot", "%" + p.LotNumber + "%");
                }

                if (!string.IsNullOrWhiteSpace(p.ItemCode))
                {
                    sql += " AND ma.ItemCode LIKE @ItemCode";
                    cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
                }

                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                        node.LotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
                        node.ItemName = null;
                        node.ItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
                        node.StartDate = null;
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;
                        node.Weight = null;
                        node.ControlMasterKey = reader.IsDBNull(3) ? null : reader.GetString(3);
                        node.Depth = 0;
                        node.NodeType = "Start";
                        node.ParentKey = null;

                        string masterKey = node.ControlMasterKey ?? "";

                        if (uniqueMasterKeys.Add(masterKey))
                        {
                            list.Add(node);
                        }
                    }
                }
            }

            return list;
        }

        public List<ProductionResultNode> FindStartNodesFromMaterialTableB(TraceSearchParameters p)
        {
            var list = new List<ProductionResultNode>();
            var uniqueMasterKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = CreateConnection())
            {
                conn.Open();

                // ============================================================
                // Step1: 元の条件で検索
                // ============================================================
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"
SELECT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.SingleControlProcessTable scp
WHERE 1 = 1
  AND SUBSTRING(
          scp.MasterKey,
          CHARINDEX('_', scp.MasterKey, CHARINDEX('_', scp.MasterKey) + 1) + 2,
          1
      ) IN ('2','3')
";

                    // 既存Appendをそのまま使用
                    AppendStartNodeSearchConditions(p, cmd, ref sql, "scp");

                    cmd.CommandText = sql;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var node = new ProductionResultNode();

                            node.ProductionOrderNumber =
                                reader.IsDBNull(0) ? null : reader.GetString(0);

                            node.LotNumber =
                                reader.IsDBNull(1) ? null : reader.GetString(1);

                            node.ItemName = null;

                            node.ItemCode =
                                reader.IsDBNull(2) ? null : reader.GetString(2);

                            node.StartDate =
                                reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                            node.EndDate = null;
                            node.ManufacturingProcessName = null;
                            node.ManufacturingTankName = null;

                            node.Weight =
                                reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                            node.ControlMasterKey =
                                reader.IsDBNull(5) ? null : reader.GetString(5);

                            node.Depth = 0;
                            node.NodeType = "Start";
                            node.ParentKey = null;

                            // StartB内だけ MasterKey で重複削除
                            string masterKey = node.ControlMasterKey ?? "";
                            if (uniqueMasterKeys.Add(masterKey))
                            {
                                list.Add(node);
                            }
                        }
                    }
                }

                // ============================================================
                // Step2: Step1 が 0件のときだけ補完条件で検索
                // ============================================================
                if (list.Count == 0)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        var sql = @"
SELECT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.SingleControlProcessTable scp
WHERE 1 = 1
  AND scp.MasterKey LIKE '%[_]%'
  AND SUBSTRING(
          scp.MasterKey,
          CHARINDEX('_', scp.MasterKey) - 3,
          1
      ) = '2'
  AND SUBSTRING(
          scp.MasterKey,
          CHARINDEX('_', scp.MasterKey) - 1,
          1
      ) IN ('4','7')
";

                        // 既存Appendをそのまま使用
                        AppendStartNodeSearchConditions(p, cmd, ref sql, "scp");

                        cmd.CommandText = sql;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var node = new ProductionResultNode();

                                node.ProductionOrderNumber =
                                    reader.IsDBNull(0) ? null : reader.GetString(0);

                                node.LotNumber =
                                    reader.IsDBNull(1) ? null : reader.GetString(1);

                                node.ItemName = null;

                                node.ItemCode =
                                    reader.IsDBNull(2) ? null : reader.GetString(2);

                                node.StartDate =
                                    reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                                node.EndDate = null;
                                node.ManufacturingProcessName = null;
                                node.ManufacturingTankName = null;

                                node.Weight =
                                    reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                                node.ControlMasterKey =
                                    reader.IsDBNull(5) ? null : reader.GetString(5);

                                node.Depth = 0;
                                node.NodeType = "Start";
                                node.ParentKey = null;

                                // Step2 側でも同じ重複削除ルール
                                string masterKey = node.ControlMasterKey ?? "";
                                if (uniqueMasterKeys.Add(masterKey))
                                {
                                    list.Add(node);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        private void AppendStartNodeSearchConditions(
    TraceSearchParameters p,
    SqlCommand cmd,
    ref string sql,
    string alias)
        {
            if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
            {
                sql += " AND " + alias + ".ForeignKey LIKE @Order";
                cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
            }

            if (!string.IsNullOrWhiteSpace(p.LotNumber))
            {
                sql += " AND " + alias + ".LotNumber LIKE @Lot";
                cmd.Parameters.AddWithValue("@Lot", "%" + p.LotNumber + "%");
            }

            if (!string.IsNullOrWhiteSpace(p.ItemCode))
            {
                sql += " AND " + alias + ".ItemCode LIKE @ItemCode";
                cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
            }

            if (p.From.HasValue)
            {
                sql += " AND " + alias + ".StartDate >= @From";
                cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = p.From.Value;
            }

            if (p.To.HasValue)
            {
                sql += " AND " + alias + ".StartDate <= @To";
                cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = p.To.Value;
            }
        }

        private ProductionResultNode ReadStartNodeFromScp(SqlDataReader reader)
        {
            var node = new ProductionResultNode();

            node.ProductionOrderNumber =
                reader.IsDBNull(0) ? null : reader.GetString(0);

            node.LotNumber =
                reader.IsDBNull(1) ? null : reader.GetString(1);

            node.ItemName = null;

            node.ItemCode =
                reader.IsDBNull(2) ? null : reader.GetString(2);

            node.StartDate =
                reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

            node.EndDate = null;
            node.ManufacturingProcessName = null;
            node.ManufacturingTankName = null;

            node.Weight =
                reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

            node.ControlMasterKey =
                reader.IsDBNull(5) ? null : reader.GetString(5);

            node.Depth = 0;
            node.NodeType = "Start";
            node.ParentKey = null;

            return node;
        }

        #endregion

        public List<ChildCandidate> FindForwardChildrenFromMaterialTableA(
    ProductionResultNode parent, int depth)
        {
            var children = new List<ChildCandidate>();

            if (parent == null || string.IsNullOrEmpty(parent.LotNumber))
            {
                return children;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardFromMaterialTableASql();
                cmd.Parameters.AddWithValue("@ParentLot", parent.LotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sourceType =
                            reader.IsDBNull(5) ? "" : reader.GetString(5);

                        string slotNoText =
                            reader.IsDBNull(6) ? "" : reader.GetString(6);

                        int slotNo = 0;
                        int.TryParse(slotNoText, out slotNo);

                        MaterialAInputType inputType = MaterialAInputType.None;
                        if (string.Equals(sourceType, "Drumcan", StringComparison.OrdinalIgnoreCase))
                        {
                            inputType = MaterialAInputType.Drumcan;
                        }
                        else if (string.Equals(sourceType, "ManualInput", StringComparison.OrdinalIgnoreCase))
                        {
                            inputType = MaterialAInputType.ManualInput;
                        }

                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        node.LotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        node.ItemName = null;

                        node.ItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        node.StartDate = null;
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;

                        node.Weight =
                            reader.IsDBNull(3) ? (float?)null : Convert.ToSingle(reader.GetValue(3));

                        node.ControlMasterKey =
                            reader.IsDBNull(4) ? null : reader.GetString(4);

                        node.Depth = depth;
                        node.NodeType = "Middle";
                        node.ParentKey = parent.LotNumber;

                        // ★ 識別に必要な業務属性を repository 側で埋めて返す
                        node.RouteSystem = "A";
                        node.InputSlotNo = slotNo;
                        node.InputSourceType = inputType == MaterialAInputType.None
                            ? null
                            : inputType.ToString();
                        node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                        string childKey = node.ControlMasterKey ?? node.LotNumber ?? "";

                        string parentKey = !string.IsNullOrWhiteSpace(parent.ControlMasterKey)
                            ? parent.ControlMasterKey
                            : (parent.LotNumber ?? "");

                        string linkKey = parentKey
                            + "|A|"
                            + inputType.ToString()
                            + "|"
                            + slotNo.ToString()
                            + "|"
                            + childKey;

                        children.Add(new ChildCandidate
                        {
                            Node = node,
                            ParentLotNumber = parent.LotNumber,
                            SourceTable = TraceSourceTable.MaterialTableA,
                            MaterialAInputType = inputType,
                            SlotNo = slotNo,
                            LinkIdentityKey = linkKey
                        });
                    }
                }
            }

            return children;
        }

        private string BuildForwardFromMaterialTableASql()
        {
            var sql = new StringBuilder();

            bool first = true;

            // DrumcanLotNumber01～05
            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                               -- 0");
                sql.AppendLine("    ma.LotNumber,                                -- 1");
                sql.AppendLine("    ma.ItemCode,                                 -- 2");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS LoadingAmount, -- 3");
                sql.AppendLine("    ma.MasterKey,                                -- 4");
                sql.AppendLine("    'Drumcan' AS SourceType,                     -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                      -- 6");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.DrumcanLotNumber" + idx + " = @ParentLot");

                first = false;
            }

            // ManualInputLotNumber01～50
            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");

                sql.AppendLine("UNION ALL");
                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                   -- 0");
                sql.AppendLine("    ma.LotNumber,                                    -- 1");
                sql.AppendLine("    ma.ItemCode,                                     -- 2");
                sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS LoadingAmount, -- 3");
                sql.AppendLine("    ma.MasterKey,                                    -- 4");
                sql.AppendLine("    'ManualInput' AS SourceType,                     -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                          -- 6");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.ManualInputLotNumber" + idx + " = @ParentLot");
            }

            return sql.ToString();
        }

        public List<ChildCandidate> FindForwardChildrenFromMaterialTableB(
    ProductionResultNode parent, int depth)
        {
            var children = new List<ChildCandidate>();

            if (parent == null || string.IsNullOrEmpty(parent.LotNumber))
            {
                return children;
            }

            using (var conn = CreateConnection())
            {
                conn.Open();

                // ============================================================
                // STEP1: 既存条件で検索
                // ============================================================
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BuildForwardFromMaterialTableBStep1Sql();
                    cmd.Parameters.AddWithValue("@ParentLot", parent.LotNumber);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var node = new ProductionResultNode();

                            node.ProductionOrderNumber =
                                reader.IsDBNull(0) ? null : reader.GetString(0);

                            node.LotNumber =
                                reader.IsDBNull(1) ? null : reader.GetString(1);

                            node.ItemName = null;

                            node.ItemCode =
                                reader.IsDBNull(2) ? null : reader.GetString(2);

                            node.StartDate =
                                reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                            node.EndDate = null;
                            node.ManufacturingProcessName = null;
                            node.ManufacturingTankName = null;

                            node.Weight =
                                reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                            node.ControlMasterKey =
                                reader.IsDBNull(5) ? null : reader.GetString(5);

                            node.Depth = depth;
                            node.NodeType = "Middle";
                            node.ParentKey = parent.ControlMasterKey;

                            node.RouteSystem = "B";
                            node.InputSlotNo = 1;
                            node.InputSourceType = null;
                            node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                            string childKey = node.ControlMasterKey ?? node.LotNumber ?? "";
                            string parentKey = !string.IsNullOrWhiteSpace(parent.ControlMasterKey)
                                ? parent.ControlMasterKey
                                : (parent.LotNumber ?? "");

                            string linkKey = parentKey + "|B|None|1|" + childKey;

                            children.Add(new ChildCandidate
                            {
                                Node = node,
                                ParentLotNumber = parent.LotNumber,
                                SourceTable = TraceSourceTable.MaterialTableB,
                                MaterialAInputType = MaterialAInputType.None,
                                SlotNo = 1,
                                LinkIdentityKey = linkKey
                            });
                        }
                    }
                }

                // ============================================================
                // STEP2: STEP1が0件のときだけ補完条件で検索
                // ============================================================
                if (children.Count == 0)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = BuildForwardFromMaterialTableBStep2SupplementSql();
                        cmd.Parameters.AddWithValue("@ParentLot", parent.LotNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var node = new ProductionResultNode();

                                node.ProductionOrderNumber =
                                    reader.IsDBNull(0) ? null : reader.GetString(0);

                                node.LotNumber =
                                    reader.IsDBNull(1) ? null : reader.GetString(1);

                                node.ItemName = null;

                                node.ItemCode =
                                    reader.IsDBNull(2) ? null : reader.GetString(2);

                                node.StartDate =
                                    reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                                node.EndDate = null;
                                node.ManufacturingProcessName = null;
                                node.ManufacturingTankName = null;

                                node.Weight =
                                    reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                                node.ControlMasterKey =
                                    reader.IsDBNull(5) ? null : reader.GetString(5);

                                node.Depth = depth;
                                node.NodeType = "Middle";
                                node.ParentKey = parent.ControlMasterKey;

                                node.RouteSystem = "B";
                                node.InputSlotNo = 1;
                                node.InputSourceType = null;
                                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                                string childKey = node.ControlMasterKey ?? node.LotNumber ?? "";
                                string parentKey = !string.IsNullOrWhiteSpace(parent.ControlMasterKey)
                                    ? parent.ControlMasterKey
                                    : (parent.LotNumber ?? "");

                                string linkKey = parentKey + "|B|None|1|" + childKey;

                                children.Add(new ChildCandidate
                                {
                                    Node = node,
                                    ParentLotNumber = parent.LotNumber,
                                    SourceTable = TraceSourceTable.MaterialTableB,
                                    MaterialAInputType = MaterialAInputType.None,
                                    SlotNo = 1,
                                    LinkIdentityKey = linkKey
                                });
                            }
                        }
                    }
                }
            }

            return children;
        }

        public List<ProductionResultNode> FindBackwardStartNodes(TraceSearchParameters p)
        {
            // 現時点ではバック検索の開始点は、
            // フォワード側の B始点取得と同じ基準を採用する。
            return FindStartNodesFromMaterialTableB(p);
        }

        public List<ChildCandidate> FindBackwardParentsFromMaterialTableA(
    ProductionResultNode current, int depth)
        {
            var parents = new List<ChildCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.ControlMasterKey))
            {
                return parents;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildBackwardFromMaterialTableASql();
                cmd.Parameters.AddWithValue("@ChildMasterKey", current.ControlMasterKey);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sourceType =
                            reader.IsDBNull(5) ? "" : reader.GetString(5);

                        string slotNoText =
                            reader.IsDBNull(6) ? "" : reader.GetString(6);

                        int slotNo = 0;
                        int.TryParse(slotNoText, out slotNo);

                        MaterialAInputType inputType = MaterialAInputType.None;
                        if (string.Equals(sourceType, "Drumcan", StringComparison.OrdinalIgnoreCase))
                        {
                            inputType = MaterialAInputType.Drumcan;
                        }
                        else if (string.Equals(sourceType, "ManualInput", StringComparison.OrdinalIgnoreCase))
                        {
                            inputType = MaterialAInputType.ManualInput;
                        }

                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        node.LotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        node.ItemName = null;

                        node.ItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        node.StartDate = null;
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;

                        node.Weight =
                            reader.IsDBNull(3) ? (float?)null : Convert.ToSingle(reader.GetValue(3));

                        node.ControlMasterKey =
                            reader.IsDBNull(4) ? null : reader.GetString(4);

                        node.Depth = depth;
                        node.NodeType = "Middle";
                        node.ParentKey = null;

                        // ★ 識別に必要な業務属性を repository 側で埋めて返す
                        node.RouteSystem = "A";
                        node.InputSlotNo = slotNo;
                        node.InputSourceType = inputType == MaterialAInputType.None
                            ? null
                            : inputType.ToString();
                        node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                        string parentKey = node.ControlMasterKey ?? node.LotNumber ?? "";
                        string childKey = !string.IsNullOrWhiteSpace(current.ControlMasterKey)
                            ? current.ControlMasterKey
                            : (current.LotNumber ?? "");

                        string linkKey = parentKey
                            + "|A|"
                            + inputType.ToString()
                            + "|"
                            + slotNo.ToString()
                            + "|"
                            + childKey;

                        parents.Add(new ChildCandidate
                        {
                            Node = node,
                            ParentLotNumber = node.LotNumber,
                            SourceTable = TraceSourceTable.MaterialTableA,
                            MaterialAInputType = inputType,
                            SlotNo = slotNo,
                            LinkIdentityKey = linkKey
                        });
                    }
                }
            }

            return parents;
        }

        private string BuildBackwardFromMaterialTableASql()
        {
            var sql = new StringBuilder();

            bool first = true;

            // DrumcanLotNumber01～05
            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                -- 0");
                sql.AppendLine("    ma.DrumcanLotNumber" + idx + " AS LotNumber,  -- 1");
                sql.AppendLine("    ma.DrumcanItemCode" + idx + " AS ItemCode,    -- 2");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS Weight, -- 3");
                sql.AppendLine("    NULL AS MasterKey,                            -- 4");
                sql.AppendLine("    'Drumcan' AS SourceType,                      -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                       -- 6");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.MasterKey = @ChildMasterKey");
                sql.AppendLine("  AND ma.DrumcanLotNumber" + idx + " IS NOT NULL");
                sql.AppendLine("  AND LTRIM(RTRIM(ma.DrumcanLotNumber" + idx + ")) <> ''");

                first = false;
            }

            // ManualInputLotNumber01～50
            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");

                sql.AppendLine("UNION ALL");
                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                     -- 0");
                sql.AppendLine("    ma.ManualInputLotNumber" + idx + " AS LotNumber,   -- 1");
                sql.AppendLine("    ma.ManualInputItemCode" + idx + " AS ItemCode,     -- 2");
                sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS Weight,  -- 3");
                sql.AppendLine("    NULL AS MasterKey,                                 -- 4");
                sql.AppendLine("    'ManualInput' AS SourceType,                       -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                            -- 6");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.MasterKey = @ChildMasterKey");
                sql.AppendLine("  AND ma.ManualInputLotNumber" + idx + " IS NOT NULL");
                sql.AppendLine("  AND LTRIM(RTRIM(ma.ManualInputLotNumber" + idx + ")) <> ''");
            }

            return sql.ToString();
        }

        public List<ChildCandidate> FindBackwardParentsFromMaterialTableB(
    ProductionResultNode current, int depth)
        {
            var parents = new List<ChildCandidate>();

            // ★ バックB探索は「現在Nodeの MasterKey から現在B実績を特定する」ので、
            //    LotNumber ではなく ControlMasterKey を必須にする
            if (current == null || string.IsNullOrWhiteSpace(current.ControlMasterKey))
            {
                return parents;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildBackwardFromMaterialTableBSql();
                cmd.Parameters.AddWithValue("@CurrentMasterKey", current.ControlMasterKey);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        node.LotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        node.ItemName = null;

                        node.ItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        node.StartDate =
                            reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;

                        node.Weight =
                            reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                        node.ControlMasterKey =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

                        node.Depth = depth;
                        node.NodeType = "Middle";
                        node.ParentKey = null;

                        // ★ B系統の業務属性
                        node.RouteSystem = "B";
                        node.InputSlotNo = 1;
                        node.InputSourceType = null;
                        node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                        string parentKey = node.ControlMasterKey ?? node.LotNumber ?? "";
                        string childKey = !string.IsNullOrWhiteSpace(current.ControlMasterKey)
                            ? current.ControlMasterKey
                            : (current.LotNumber ?? "");

                        string linkKey = parentKey + "|B|None|1|" + childKey;

                        parents.Add(new ChildCandidate
                        {
                            Node = node,

                            // ★ 親判定に使ったロットは、今回確定した上流ロット（＝親Node側Lot）
                            ParentLotNumber = node.LotNumber,

                            SourceTable = TraceSourceTable.MaterialTableB,
                            MaterialAInputType = MaterialAInputType.None,
                            SlotNo = 1,
                            LinkIdentityKey = linkKey
                        });
                    }
                }
            }

            return parents;
        }

        private string BuildBackwardFromMaterialTableBSql()
        {
            var sql = @"
;WITH CurrentB AS
(
    -- 現在Node（scp.MasterKey）に対応する MaterialTableB を特定し、
    -- そこに入っている SourceTankLotNumber01 を上流ロットとして取り出す
    SELECT DISTINCT
        mbCurrent.SourceTankLotNumber01 AS UpstreamLot
    FROM dbo.MaterialTableB mbCurrent
    WHERE mbCurrent.MasterKey = @CurrentMasterKey
      AND mbCurrent.SourceTankLotNumber01 IS NOT NULL
      AND LTRIM(RTRIM(mbCurrent.SourceTankLotNumber01)) <> ''
)
SELECT DISTINCT
    scpParent.ForeignKey,   -- 0
    scpParent.LotNumber,    -- 1
    scpParent.ItemCode,     -- 2
    scpParent.StartDate,    -- 3
    scpParent.Weight,       -- 4
    scpParent.MasterKey     -- 5
FROM CurrentB cb
INNER JOIN dbo.MaterialTableB mbParent
        ON mbParent.LotNumber = cb.UpstreamLot
INNER JOIN dbo.SingleControlProcessTable scpParent
        ON scpParent.MasterKey = mbParent.MasterKey
       AND scpParent.LotNumber = mbParent.LotNumber
       AND scpParent.LotNumber = cb.UpstreamLot
";
            return sql;
        }

        private string EscapeCsv(string value)
        {
            if (value == null)
                return "";

            bool needQuote =
                value.Contains(",") ||
                value.Contains("\"") ||
                value.Contains("\r") ||
                value.Contains("\n");

            if (!needQuote)
                return value;

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private string BuildForwardFromMaterialTableBStep1Sql()
        {
            return @"
SELECT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.MaterialTableB mb
INNER JOIN dbo.SingleControlProcessTable scp
        ON scp.MasterKey = mb.MasterKey
WHERE mb.SourceTankLotNumber01 = @ParentLot
";
        }

        private string BuildForwardFromMaterialTableBStep2SupplementSql()
        {
            return @"
SELECT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.SingleControlProcessTable scp
WHERE scp.LotNumber = @ParentLot
  AND scp.MasterKey LIKE '%[_]%'
  AND SUBSTRING(
          scp.MasterKey,
          CHARINDEX('_', scp.MasterKey) - 3,
          1
      ) = '2'
  AND SUBSTRING(
          scp.MasterKey,
          CHARINDEX('_', scp.MasterKey) - 1,
          1
      ) IN ('4','7')
";
        }

        private string BuildForwardFromMaterialTableBSql()
        {
            var sql = @"
SELECT
    x.ForeignKey,   -- 0
    x.LotNumber,    -- 1
    x.ItemCode,     -- 2
    x.StartDate,    -- 3
    x.Weight,       -- 4
    x.MasterKey     -- 5
FROM
(
    -- =========================================================
    -- ① 既存ルート
    -- MaterialTableB に存在し、親Lotから素直につながるケース
    -- =========================================================
    SELECT
        scp.ForeignKey,
        scp.LotNumber,
        scp.ItemCode,
        scp.StartDate,
        scp.Weight,
        scp.MasterKey
    FROM dbo.MaterialTableB mb
    INNER JOIN dbo.SingleControlProcessTable scp
            ON scp.MasterKey = mb.MasterKey
    WHERE mb.SourceTankLotNumber01 = @ParentLot

    UNION

    -- =========================================================
    -- ② 補完ルート
    -- MaterialTableB には無いが、scp には存在するケース
    -- Lot一致 + 種別=2 + 工程=4/7 で B系統補完
    -- =========================================================
    SELECT
        scp.ForeignKey,
        scp.LotNumber,
        scp.ItemCode,
        scp.StartDate,
        scp.Weight,
        scp.MasterKey
    FROM dbo.SingleControlProcessTable scp
    WHERE scp.LotNumber = @ParentLot
      AND scp.MasterKey LIKE '%[_]%'
      AND SUBSTRING(
            scp.MasterKey,
            CHARINDEX('_', scp.MasterKey) - 3,
            1
          ) = '2'
      AND SUBSTRING(
            scp.MasterKey,
            CHARINDEX('_', scp.MasterKey) - 1,
            1
          ) IN ('4', '7')
) x
";
            return sql;
        }

        #region MaterialTableA/B からの材料抽出

        public List<ProductionResultNode> FindDownstreamByMaterial(MaterialPair material, int depth)
        {
            if (material == null) throw new ArgumentNullException("material");

            var result = new List<ProductionResultNode>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                // ==== MaterialTableA ====
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BuildMaterialTableAForwardSql_New();
                    cmd.Parameters.AddWithValue("@ItemCode", (object)material.ItemCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LotNumber", material.LotNumber);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapProductionResultNodeFromJoinedReader(reader, depth));
                        }
                    }
                }

                // ==== MaterialTableB ====
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BuildMaterialTableBForwardSql_New();
                    cmd.Parameters.AddWithValue("@ItemCode", (object)material.ItemCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LotNumber", material.LotNumber);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapProductionResultNodeFromJoinedReader(reader, depth));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 指定された製品（ProductionOrderNumber）の原材料一覧を取得
        /// 仕様書 7.2 の 2,3 ステップに相当
        /// </summary>
        public List<MaterialPair> FindMaterialsByProductionOrder(string productionOrderNumber)
        {
            var materials = new List<MaterialPair>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                // ---- MaterialTableA ----
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.MaterialTableA WHERE ForeignKey = @FK";
                    cmd.Parameters.AddWithValue("@FK", productionOrderNumber);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ExtractMaterialPairsFromMaterialTableA(reader, materials);
                        }
                    }
                }

                // ---- MaterialTableB ----
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM dbo.MaterialTableB WHERE ForeignKey = @FK";
                    cmd.Parameters.AddWithValue("@FK", productionOrderNumber);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ExtractMaterialPairsFromMaterialTableB(reader, materials);
                        }
                    }
                }

                // ※ FilterTable を含める場合は同様に追記
            }

            return materials;
        }

        #endregion

        #region MaterialTableA / B の動的 SQL & マッピング

        /// <summary>
        /// MaterialTableA を使用したフォワード検索用 SQL を動的に生成
        /// （ItemCode + LotNumber が一致する行を抽出し、ProductionResultsTable に JOIN）
        /// </summary>
        /// <summary>
        /// トレースフォワード用:
        ///   親ロット(@ItemCode,@LotNumber) を原料として使用している
        ///   ProductionResultsTable の行を MaterialTableA 経由で取得する。
        /// 関連付け:
        ///   pr.ChildTableKey01～ChildTableKeyXX と MaterialTableA.MasterKey
        ///   かつ MaterialTableA の
        ///     DrumcanLotNumber01～05 または ManualInputLotNumber01～50
        ///   のいずれかに @LotNumber が含まれる。
        /// </summary>
        private string BuildMaterialTableAForwardSql_New()
        {
            // pr = ProductionResultsTable, ma = MaterialTableA
            var sql = @"
SELECT 
       pr.ProductionOrderNumber, pr.LotNumber, pr.ItemName, pr.ItemCode,
       pr.StartDate, pr.EndDate, pr.ManufacturingProcessName, pr.ManufacturingTankName
FROM   dbo.ProductionResultsTable pr
JOIN   dbo.MaterialTableA ma
       ON (
";

            // pr.ChildTableKey01 ～ ChildTableKeyXX と ma.MasterKey の関連付け
            for (int i = 1; i <= MaxChildTableKeyIndex; i++)
            {
                string idx = i.ToString("00");
                if (i > 1)
                {
                    sql += " OR ";
                }
                sql += "pr.ChildTableKey" + idx + " = ma.MasterKey";
            }

            sql += @"
       )
WHERE
      (
";

            // DrumncanLotNumber01 ～ 05
            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");
                if (i > 1)
                {
                    sql += "   OR ";
                }
                sql += "ma.DrumcanLotNumber" + idx + " = @LotNumber\r\n";
            }

            // ManualInputLotNumber01 ～ 50
            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");
                sql += "   OR ma.ManualInputLotNumber" + idx + " = @LotNumber\r\n";
            }

            sql += @"
      )
";

            // ItemCode も一致させたい場合（ItemCode が null の時は無視）
            sql += @"
  AND (@ItemCode IS NULL OR ma.ItemCode = @ItemCode)
";

            return sql;
        }

        /// <summary>
        /// トレースフォワード用:
        ///   親ロット(@ItemCode,@LotNumber) を原料として使用している
        ///   ProductionResultsTable の行を MaterialTableB 経由で取得する。
        /// 関連付け:
        ///   pr.ChildTableKey01～ChildTableKeyXX と MaterialTableB.MasterKey
        ///   かつ MaterialTableB.SourceTankLotNumber01 に @LotNumber が入っている。
        /// </summary>
        private string BuildMaterialTableBForwardSql_New()
        {
            var sql = @"
SELECT 
       pr.ProductionOrderNumber, pr.LotNumber, pr.ItemName, pr.ItemCode,
       pr.StartDate, pr.EndDate, pr.ManufacturingProcessName, pr.ManufacturingTankName
FROM   dbo.ProductionResultsTable pr
JOIN   dbo.MaterialTableB mb
       ON (
";

            for (int i = 1; i <= MaxChildTableKeyIndex; i++)
            {
                string idx = i.ToString("00");
                if (i > 1)
                {
                    sql += " OR ";
                }
                sql += "pr.ChildTableKey" + idx + " = mb.MasterKey";
            }

            sql += @"
       )
WHERE  mb.SourceTankLotNumber01 = @LotNumber
";

            // ItemCode も一致させる（必要に応じて削除可）
            sql += @"
  AND (@ItemCode IS NULL OR mb.ItemCode = @ItemCode)
";

            return sql;
        }

        /// <summary>
        /// MasterKey に付与された工程種別コードの「種別」が 2 または 3 かどうかを判定する。
        /// MasterKey フォーマット前提:
        ///   MasterKey = 製造指図番号 + 工程種別
        ///   工程種別 = 設備 + 種別 + グループNo + 工程 + "_" + 回数
        /// → 最後の '_' の 3 文字前が「種別」
        /// </summary>
        private bool IsTargetProcessType(string masterKey)
        {
            if (string.IsNullOrEmpty(masterKey))
            {
                return false;
            }

            // 末尾の "_" を探す（「工程_回数」の "_"）
            int idxUnd = masterKey.LastIndexOf('_');
            if (idxUnd <= 0)
            {
                return false;
            }

            // "_" より前の部分（設備 + 種別 + グループNo + 工程 + ...）
            string before = masterKey.Substring(0, idxUnd);
            if (before.Length < 3)
            {
                return false;
            }

            // 「_」の 3 文字前が 種別（設備 + [種別][グループNo][工程] + "_" のイメージ）
            char typeChar = before[before.Length - 3];

            return (typeChar == '2' || typeChar == '3');
        }



        private ProductionResultNode MapProductionResultNodeFromJoinedReader(SqlDataReader reader, int depth)
        {
            var node = new ProductionResultNode();
            node.ProductionOrderNumber = reader.GetString(0);
            node.LotNumber = reader.GetString(1);
            node.ItemName = reader.GetString(2);
            node.ItemCode = reader.GetString(3);
            node.StartDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
            node.EndDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
            node.ManufacturingProcessName = reader.IsDBNull(6) ? null : reader.GetString(6);
            node.ManufacturingTankName = reader.IsDBNull(7) ? null : reader.GetString(7);
            node.Depth = depth;
            node.NodeType = (depth == 0) ? "Start" : "Middle";
            return node;
        }

        private static void ExtractMaterialPairsFromMaterialTableA(SqlDataReader reader, List<MaterialPair> dest)
        {
            // Drumcan 系
            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");
                string columnItem = "DrumcanItemCode" + idx;
                string columnLot = "DrumcanLotNumber" + idx;

                string itemCode = reader[columnItem] as string;
                string lot = reader[columnLot] as string;

                if (!string.IsNullOrWhiteSpace(itemCode) && !string.IsNullOrWhiteSpace(lot))
                {
                    dest.Add(new MaterialPair(itemCode, lot));
                }
            }

            // ManualInput 系（01..50）
            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");
                string columnItem = "ManualInputItemCode" + idx;
                string columnLot = "ManualInputLotNumber" + idx;

                string itemCode = reader[columnItem] as string;
                string lot = reader[columnLot] as string;

                if (!string.IsNullOrWhiteSpace(itemCode) && !string.IsNullOrWhiteSpace(lot))
                {
                    dest.Add(new MaterialPair(itemCode, lot));
                }
            }
        }

        private static void ExtractMaterialPairsFromMaterialTableB(SqlDataReader reader, List<MaterialPair> dest)
        {
            // Solvent1..3
            for (int i = 1; i <= 3; i++)
            {
                string idx = i.ToString();
                string columnItem = "SourceTankItemCode_Solvent" + idx;
                string columnLot = "SourceTankLotNumber_Solvent" + idx;

                string itemCode = reader[columnItem] as string;
                string lot = reader[columnLot] as string;

                if (!string.IsNullOrWhiteSpace(itemCode) && !string.IsNullOrWhiteSpace(lot))
                {
                    dest.Add(new MaterialPair(itemCode, lot));
                }
            }

            // SourceTankItemCode01..50
            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");
                string columnItem = "SourceTankItemCode" + idx;
                string columnLot = "SourceTankLotNumber" + idx;

                string itemCode = reader[columnItem] as string;
                string lot = reader[columnLot] as string;

                if (!string.IsNullOrWhiteSpace(itemCode) && !string.IsNullOrWhiteSpace(lot))
                {
                    dest.Add(new MaterialPair(itemCode, lot));
                }
            }
        }

        #endregion

        #region 原材料から製品を探す／原材料が製品として存在するかを検索

        /// <summary>
        /// 原材料 (ItemCode, LotNumber) を作った ProductionResults が存在するか検索
        /// 仕様書 7.2 の「中間材も製造実績を持つ場合」の遡及に相当
        /// </summary>
        public List<ProductionResultNode> FindProductionByMaterial(MaterialPair material, int depth)
        {
            var result = new List<ProductionResultNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT ProductionOrderNumber, LotNumber, ItemName, ItemCode,
       StartDate, EndDate, ManufacturingProcessName, ManufacturingTankName
FROM   dbo.ProductionResultsTable
WHERE  ItemCode = @ItemCode
  AND  LotNumber = @LotNumber";

                cmd.Parameters.AddWithValue("@ItemCode", material.ItemCode);
                cmd.Parameters.AddWithValue("@LotNumber", material.LotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new ProductionResultNode();
                        node.ProductionOrderNumber = reader.GetString(0);
                        node.LotNumber = reader.GetString(1);
                        node.ItemName = reader.GetString(2);
                        node.ItemCode = reader.GetString(3);
                        node.StartDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
                        node.EndDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                        node.ManufacturingProcessName = reader.IsDBNull(6) ? null : reader.GetString(6);
                        node.ManufacturingTankName = reader.IsDBNull(7) ? null : reader.GetString(7);
                        node.Depth = depth;
                        node.NodeType = "Middle";

                        result.Add(node);
                    }
                }
            }

            return result;
        }

        #endregion

        #region 履歴詳細表示用（SingleControlProcessTable / FilterTable）

        /// <summary>
        /// 制御工程テーブルから詳細履歴を取得
        /// </summary>
        public DataTable GetSingleControlHistory(string productionOrderNumber, string itemCode, string lotNumber)
        {
            var dt = new DataTable();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
SELECT *
FROM   dbo.SingleControlProcessTable
WHERE  ItemCode = @ItemCode
  AND  LotNumber = @LotNumber";

                cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                cmd.Parameters.AddWithValue("@LotNumber", lotNumber);

                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// フィルタテーブルから履歴を取得
        /// </summary>
        public DataTable GetFilterHistory(string productionOrderNumber, string itemCode, string lotNumber)
        {
            var dt = new DataTable();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
SELECT *
FROM   dbo.FilterTable
WHERE  ItemCode = @ItemCode
  AND  LotNumber = @LotNumber";

                cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                cmd.Parameters.AddWithValue("@LotNumber", lotNumber);

                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }

        #endregion

        #region デバッグ機能群

        public List<ProductionResultNode> FindStartNodesFromMaterialTableBOnlyDebug(TraceSearchParameters p)
        {
            var list = new List<ProductionResultNode>();
            var uniqueMasterKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                var sql = @"
SELECT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.SingleControlProcessTable scp
WHERE 1 = 1
  AND SUBSTRING(
          scp.MasterKey,
          CHARINDEX('_', scp.MasterKey, CHARINDEX('_', scp.MasterKey) + 1) + 2,
          1
      ) IN ('2','3')
";

                if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
                {
                    sql += " AND scp.ForeignKey LIKE @Order";
                    cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
                }

                if (!string.IsNullOrWhiteSpace(p.LotNumber))
                {
                    sql += " AND scp.LotNumber LIKE @Lot";
                    cmd.Parameters.AddWithValue("@Lot", "%" + p.LotNumber + "%");
                }

                if (!string.IsNullOrWhiteSpace(p.ItemCode))
                {
                    sql += " AND scp.ItemCode LIKE @ItemCode";
                    cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
                }

                if (p.From.HasValue)
                {
                    sql += " AND scp.StartDate >= @From";
                    cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = p.From.Value;
                }

                if (p.To.HasValue)
                {
                    sql += " AND scp.StartDate <= @To";
                    cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = p.To.Value;
                }

                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        node.LotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        node.ItemName = null;

                        node.ItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        node.StartDate =
                            reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;

                        node.Weight =
                            reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                        node.ControlMasterKey =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

                        node.Depth = 0;
                        node.NodeType = "Start";
                        node.ParentKey = null;

                        string masterKey = node.ControlMasterKey ?? "";
                        if (uniqueMasterKeys.Add(masterKey))
                        {
                            list.Add(node);
                        }
                    }
                }
            }

            try
            {
                string logPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_BOnly_StartNodes.csv");

                bool exists = File.Exists(logPath);

                using (var sw = new StreamWriter(logPath, true, Encoding.UTF8))
                {
                    if (!exists)
                    {
                        sw.WriteLine("ProductionOrderNumber,LotNumber,ItemCode,StartDate,Weight,ControlMasterKey");
                    }

                    foreach (var node in list)
                    {
                        sw.WriteLine(string.Join(",",
                            EscapeCsv(node.ProductionOrderNumber),
                            EscapeCsv(node.LotNumber),
                            EscapeCsv(node.ItemCode),
                            EscapeCsv(node.StartDate.HasValue ? node.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null),
                            EscapeCsv(node.Weight.HasValue ? node.Weight.Value.ToString() : null),
                            EscapeCsv(node.ControlMasterKey)));
                    }
                }
            }
            catch
            {
            }

            return list;
        }

        public List<ChildCandidate> FindForwardChildrenFromMaterialTableBOnlyDebug(
            ProductionResultNode parent, int depth)
        {
            var children = new List<ChildCandidate>();

            if (parent == null || string.IsNullOrEmpty(parent.LotNumber))
            {
                return children;
            }

            int readCount = 0;
            int addedCount = 0;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardFromMaterialTableBOnlyDebugSql();
                cmd.Parameters.AddWithValue("@ParentLot", parent.LotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        readCount++;

                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        node.LotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        node.ItemName = null;

                        node.ItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        node.StartDate =
                            reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;

                        node.Weight =
                            reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                        node.ControlMasterKey =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

                        node.Depth = depth;
                        node.NodeType = "Middle";
                        node.ParentKey = parent.ControlMasterKey;

                        string childKey = node.ControlMasterKey ?? node.LotNumber ?? "";

                        string parentKey = !string.IsNullOrWhiteSpace(parent.ControlMasterKey)
                            ? parent.ControlMasterKey
                            : (parent.LotNumber ?? "");

                        string linkKey = parentKey
                            + "|B|None|1|"
                            + childKey;

                        children.Add(new ChildCandidate
                        {
                            Node = node,
                            ParentLotNumber = parent.LotNumber,
                            SourceTable = TraceSourceTable.MaterialTableB,
                            MaterialAInputType = MaterialAInputType.None,
                            SlotNo = 1,
                            LinkIdentityKey = linkKey
                        });

                        addedCount++;
                    }
                }
            }

            try
            {
                string logPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "LotTrace_Debug_BOnly_Children.csv");

                bool exists = File.Exists(logPath);

                using (var sw = new StreamWriter(logPath, true, Encoding.UTF8))
                {
                    if (!exists)
                    {
                        sw.WriteLine("ParentMasterKey,ParentLotNumber,Depth,ReadCount,AddedCount");
                    }

                    sw.WriteLine(string.Join(",",
                        EscapeCsv(parent.ControlMasterKey),
                        EscapeCsv(parent.LotNumber),
                        depth.ToString(),
                        readCount.ToString(),
                        addedCount.ToString()));
                }
            }
            catch
            {
            }

            return children;
        }

        private string BuildForwardFromMaterialTableBOnlyDebugSql()
        {
            var sql = @"
SELECT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.MaterialTableB mb
INNER JOIN dbo.SingleControlProcessTable scp
        ON scp.MasterKey = mb.MasterKey
WHERE mb.SourceTankLotNumber01 = @ParentLot
";

            return sql;
        }

        

        #endregion
    }
}