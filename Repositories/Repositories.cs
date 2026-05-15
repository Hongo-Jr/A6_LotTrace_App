using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Office.Word;
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



        #region 検索起点の取得（ほぼトレースバック専用）


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
  AND SUBSTRING(
        scp.MasterKey,
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),
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
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)) - 2,
        1
    ) = '2'
AND SUBSTRING(
        scp.MasterKey,
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),
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
        private void AppendStartNodeSearchConditionsForMaterialTableA(
    TraceSearchParameters p,
    StringBuilder sql,
    string alias)
        {
            if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
            {
                sql.AppendLine("  AND " + alias + ".ForeignKey LIKE @Order");
            }

            if (!string.IsNullOrWhiteSpace(p.LotNumber))
            {
                sql.AppendLine("  AND " + alias + ".LotNumber LIKE @Lot");
            }

            if (!string.IsNullOrWhiteSpace(p.ItemCode))
            {
                sql.AppendLine("  AND " + alias + ".ItemCode LIKE @ItemCode");
            }

            // From / To は必要ならここ（MaterialTableAに列がある場合のみ）

            // From / To はここでは載せない
        }


        #endregion

        /// <summary>
        ///　
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<ProductionResultNode> FindStartNodesFromMaterialTableAManualInput(
    TraceSearchParameters p)
        {
            var list = new List<ProductionResultNode>();
            var uniqueNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildStartNodesFromMaterialTableAManualInputSql(p, cmd);

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

                        node.ControlMasterKey =
                            reader.IsDBNull(3) ? null : reader.GetString(3);

                        node.Weight =
                            reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                        string sourceType =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

                        string slotNoText =
                            reader.IsDBNull(6) ? null : reader.GetString(6);

                        int slotNo = 0;
                        int.TryParse(slotNoText, out slotNo);

                        node.StartDate = null;
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;

                        node.Depth = 0;
                        node.NodeType = "Start";
                        node.ParentKey = null;

                        node.RouteSystem = "A";
                        node.InputSourceType = sourceType;
                        node.InputSlotNo = slotNo;

                        string masterKey = node.ControlMasterKey ?? "";
                        string nodeKey = masterKey + "|" + slotNo.ToString("00");

                        if (uniqueNodeKeys.Add(nodeKey))
                        {
                            list.Add(node);
                        }
                    }
                }
            }

            return list;
        }

        private string BuildStartNodesFromMaterialTableAManualInputSql(
    TraceSearchParameters p,
    SqlCommand cmd)
        {
            var sql = new StringBuilder();
            bool first = true;

            bool hasOrder = p != null && !string.IsNullOrWhiteSpace(p.ProductionOrderNumber);
            bool hasLot = p != null && !string.IsNullOrWhiteSpace(p.LotNumber);
            bool hasItemCode = p != null && !string.IsNullOrWhiteSpace(p.ItemCode);

            if (hasOrder)
            {
                cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
            }

            if (hasLot)
            {
                cmd.Parameters.AddWithValue("@Lot", "%" + p.LotNumber + "%");
            }

            if (hasItemCode)
            {
                cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
            }

            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                -- 0");
                sql.AppendLine("    ma.LotNumber,                                 -- 1");
                sql.AppendLine("    ma.ItemCode,                                  -- 2");
                sql.AppendLine("    ma.MasterKey,                                 -- 3");
                sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS LoadingAmount, -- 4");
                sql.AppendLine("    'ManualInput' AS SourceType,                  -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                       -- 6");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE 1 = 1");
                sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " IS NOT NULL");
                sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " <> 0");

                if (hasOrder)
                {
                    sql.AppendLine("  AND ma.ForeignKey LIKE @Order");
                }

                if (hasLot)
                {
                    sql.AppendLine("  AND ma.LotNumber LIKE @Lot");
                }

                if (hasItemCode)
                {
                    sql.AppendLine("  AND ma.ItemCode LIKE @ItemCode");
                }

                first = false;
            }

            return sql.ToString();
        }





        #region 新トレースフォワード関連群

        public List<ProductionResultNode> FindForwardStartNodes(
    TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();
            var addedStartNodeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // ------------------------------------------------------------
            // 1. START-D を先に評価
            //    HITしたら、始点は START-D 親だけを採用する。
            // ------------------------------------------------------------
            var startDCandidates = FindForwardStartDCandidates(p);
            if (startDCandidates != null && startDCandidates.Count > 0)
            {
                foreach (var candidate in startDCandidates)
                {
                    if (candidate == null || candidate.PearentNode == null)
                        continue;

                    var parentNode = candidate.PearentNode;
                    parentNode.Depth = 0;
                    parentNode.NodeType = "Start";
                    parentNode.ParentKey = null;

                    if (addedStartNodeKeys.Add(parentNode.NodeIdentityKey))
                    {
                        result.Add(parentNode);
                    }
                }

                return result;
            }

            // ------------------------------------------------------------
            // 2. 通常始点取得
            //    START-D が 0件のときだけ通常系へ進む
            // ------------------------------------------------------------
            var startB = FindStartNodesFromMaterialTableB(p);
            var startAManual = FindStartNodesFromMaterialTableAManualInput(p);

            if (startAManual != null && startAManual.Count > 0)
            {
                if (startB == null)
                    startB = new List<ProductionResultNode>();

                startB.AddRange(startAManual);
            }

            List<ProductionResultNode> startA;
            if (startB == null || startB.Count == 0)
            {
                startA = FindStartNodesFromMaterialTableA(p);
            }
            else
            {
                startA = new List<ProductionResultNode>();
            }

            AppendForwardStartNodes(result, addedStartNodeKeys, startB);
            AppendForwardStartNodes(result, addedStartNodeKeys, startA);

            return result;
        }

        public List<ChildCandidate> FindForwardInitialCandidates(
            IEnumerable<ProductionResultNode> startNodes,
            TraceSearchParameters p,
            int depth)
        {
            var result = new List<ChildCandidate>();

            if (startNodes != null)
            {
                foreach (var startNode in startNodes)
                {
                    if (startNode == null)
                        continue;

                    var candidates = FindForwardChildCandidates(startNode, depth);
                    if (candidates != null && candidates.Count > 0)
                    {
                        result.AddRange(candidates);
                    }
                }
            }

            // START-D もここでまとめて返す
            var startDCandidates = FindForwardStartDCandidates(p);
            if (startDCandidates != null && startDCandidates.Count > 0)
            {
                result.AddRange(startDCandidates);
            }

            return RemoveDuplicateChildCandidates(result);
        }

        private List<ChildCandidate> RemoveDuplicateChildCandidates(
    IEnumerable<ChildCandidate> candidates)
        {
            var result = new List<ChildCandidate>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (candidates == null)
                return result;

            foreach (var candidate in candidates)
            {
                if (candidate == null ||
                    candidate.PearentNode == null ||
                    candidate.ChildNode == null)
                {
                    continue;
                }

                string key = BuildCandidateNodePairKey(
                    candidate.PearentNode,
                    candidate.ChildNode);

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (!seen.Add(key))
                    continue;

                result.Add(candidate);
            }

            return result;
        }

        private List<BackwardParentCandidate> RemoveDuplicateBackwardParentCandidates(
    IEnumerable<BackwardParentCandidate> candidates)
        {
            var result = new List<BackwardParentCandidate>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (candidates == null)
                return result;

            foreach (var candidate in candidates)
            {
                if (candidate == null ||
                    candidate.Node == null ||
                    candidate.ChildNode == null)
                {
                    continue;
                }

                string key = BuildCandidateNodePairKey(
                    candidate.Node,
                    candidate.ChildNode);

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (!seen.Add(key))
                    continue;

                result.Add(candidate);
            }

            return result;
        }

        private string BuildCandidateNodePairKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode)
        {
            if (parentNode == null || childNode == null)
                return null;

            string parentKey = parentNode.NodeIdentityKey;
            string childKey = childNode.NodeIdentityKey;

            if (string.IsNullOrWhiteSpace(parentKey) ||
                string.IsNullOrWhiteSpace(childKey))
            {
                return null;
            }

            return parentKey + "=>" + childKey;
        }

        private void AppendForwardStartNodes(
            List<ProductionResultNode> target,
            HashSet<string> seenKeys,
            List<ProductionResultNode> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (seenKeys == null)
                throw new ArgumentNullException("seenKeys");

            if (source == null || source.Count == 0)
                return;

            foreach (var node in source)
            {
                if (node == null)
                    continue;

                node.Depth = 0;
                node.NodeType = "Start";
                node.ParentKey = null;

                if (seenKeys.Add(node.NodeIdentityKey))
                {
                    target.Add(node);
                }
            }
        }

        public List<ChildCandidate> FindForwardStartDCandidates(
    TraceSearchParameters p)
        {
            var result = new List<ChildCandidate>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var d1 = FindForwardStartDCandidatesFromDrumHit(p);
            var d2 = FindForwardStartDCandidatesFromMaHit(p);

            var raw = new List<ChildCandidate>();

            if (d1 != null && d1.Count > 0)
            {
                raw.AddRange(d1);
            }

            if (d2 != null && d2.Count > 0)
            {
                raw.AddRange(d2);
            }

            var adjusted = AdjustForwardStartDCandidatesForB(raw);

            AppendForwardStartDCandidatesDeduped(result, seen, adjusted);

            return result;
        }



        /// <summary>
        /// Forward BFS 通常系の repository 入口。
        /// current を親として STEP1-A → STEP1-B → STEP2-A → STEP2-B を順に実行し、
        /// service にそのまま渡せる completed ChildCandidate 一式を返す。
        ///
        /// ここではまだ重複除去を入れない。
        /// START-D は別入口で扱う。
        /// </summary>
        public List<ChildCandidate> FindForwardChildCandidates(
            ProductionResultNode current,
            int depth)
        {
            var result = new List<ChildCandidate>();

            if (current == null)
                return result;

            if (string.IsNullOrWhiteSpace(current.LotNumber))
                return result;

            // STEP1: 子Lot候補抽出
            var step1Candidates = new List<ChildCandidate>();

            var step1A = ExecuteForwardStep1ForA(current);
            if (step1A != null && step1A.Count > 0)
            {
                step1Candidates.AddRange(step1A);
            }

            var step1B = ExecuteForwardStep1ForB(current);
            if (step1B != null && step1B.Count > 0)
            {
                step1Candidates.AddRange(step1B);
            }

            if (step1Candidates.Count == 0)
                return result;

            // STEP2: child 実体化
            var step2A = ExecuteForwardStep2ForA(step1Candidates, depth);
            if (step2A != null && step2A.Count > 0)
            {
                result.AddRange(step2A);
            }

            var step2B = ExecuteForwardStep2ForB(step1Candidates, depth);
            if (step2B != null && step2B.Count > 0)
            {
                result.AddRange(step2B);
            }

            return result;
        }

        public List<ChildCandidate> FindForwardStartDCandidatesFromDrumHit(
    TraceSearchParameters p)
        {
            var result = new List<ChildCandidate>();

            //DrumLotは必須。なければ実行する意味が無い
            if (p == null || string.IsNullOrWhiteSpace(p.LotNumber))
                return result;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardStartD1FromDrumSql(p);
                cmd.Parameters.AddWithValue("@DrumLotNumber", p.LotNumber);

                if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
                {
                    cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
                }

                if (!string.IsNullOrWhiteSpace(p.ItemCode))
                {
                    cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var childNode = BuildForwardStartDChildMaNode(reader);
                        var parentNode = BuildForwardStartDParentDrumNode(reader, childNode);

                        var candidate = BuildForwardChildCandidate(
                            parentNode,
                            childNode,
                            "STARTD1");

                        if (candidate != null)
                        {
                            result.Add(candidate);
                        }
                    }
                }
            }

            return result;
        }

        public List<ChildCandidate> FindForwardStartDCandidatesFromMaHit(
    TraceSearchParameters p)
        {
            var result = new List<ChildCandidate>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardStartD2FromMaSql(p,cmd);

                AppendForwardStartDSearchParametersForMaterialTableA(p, cmd);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var childNode = BuildForwardStartDChildMaNode(reader);
                        var parentNode = BuildForwardStartDParentDrumNode(reader, childNode);

                        var candidate = BuildForwardChildCandidate(
                            parentNode,
                            childNode,
                            "STARTD2");

                        if (candidate != null)
                        {
                            result.Add(candidate);
                        }
                    }
                }
            }

            return result;
        }

        private List<ChildCandidate> AdjustForwardStartDCandidatesForB(
    List<ChildCandidate> source)
        {
            var result = new List<ChildCandidate>();

            if (source == null || source.Count == 0)
                return result;

            foreach (var candidate in source)
            {
                if (candidate == null ||
                    candidate.PearentNode == null ||
                    candidate.ChildNode == null)
                {
                    continue;
                }

                var resolvedChild = ResolveForwardStartDChildToBNode(
                    candidate.ChildLotNumber,
                    candidate.ChildNode.Depth);

                if (resolvedChild != null)
                {
                    candidate.ChildNode = resolvedChild;
                    candidate.ChildLotNumber = resolvedChild.LotNumber;
                    candidate.RelationKey = BuildForwardRelationKey(
                        candidate.PearentNode,
                        resolvedChild);
                    candidate.DebugSource = (candidate.DebugSource ?? string.Empty) + "-2.5B";
                }

                result.Add(candidate);
            }

            return result;
        }

        private ProductionResultNode ResolveForwardStartDChildToBNode(
    string childLotNumber,
    int depth)
        {
            if (string.IsNullOrWhiteSpace(childLotNumber))
                return null;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardStartD25MaterialTableBSql();
                cmd.Parameters.AddWithValue("@ChildLot", childLotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var childNode = new ProductionResultNode();

                    childNode.ProductionOrderNumber =
                        reader.IsDBNull(0) ? null : reader.GetString(0);

                    childNode.LotNumber =
                        reader.IsDBNull(1) ? null : reader.GetString(1);

                    childNode.ItemCode =
                        reader.IsDBNull(2) ? null : reader.GetString(2);

                    childNode.StartDate =
                        reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                    childNode.Weight =
                        reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                    childNode.ControlMasterKey =
                        reader.IsDBNull(5) ? null : reader.GetString(5);

                    childNode.ItemName = null;
                    childNode.EndDate = null;
                    childNode.ManufacturingProcessName = null;
                    childNode.ManufacturingTankName = null;

                    childNode.Depth = depth;
                    childNode.NodeType = "Middle";
                    childNode.ParentKey = childLotNumber;
                    childNode.RouteSystem = "B";
                    childNode.InputSourceType = null;
                    childNode.InputSlotNo = 1;
                    childNode.IsTraceTerminal = string.IsNullOrWhiteSpace(childNode.LotNumber);

                    return childNode;
                }
            }
        }

        private string BuildForwardStartD25MaterialTableBSql()
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
WHERE scp.LotNumber = @ChildLot
  AND scp.MasterKey LIKE '%[_]%'
  AND SUBSTRING(
        scp.MasterKey,
        CHARINDEX('_', scp.MasterKey, CHARINDEX('_', scp.MasterKey) + 1) + 2,
        1
      ) IN ('2','3')
  AND SUBSTRING(
        scp.MasterKey,
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),
        1
    ) IN ('4','7')
ORDER BY
    scp.StartDate,
    scp.MasterKey
";
        }

        private void AppendForwardStartDSearchParametersForMaterialTableA(
            TraceSearchParameters p,
            SqlCommand cmd)
        {
            if (!string.IsNullOrWhiteSpace(p.ProductionOrderNumber))
            {
                cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
            }

            if (!string.IsNullOrWhiteSpace(p.ItemCode))
            {
                cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
            }

            if (!string.IsNullOrWhiteSpace(p.LotNumber))
            {
                cmd.Parameters.AddWithValue("@Lot", "%" + p.LotNumber + "%");
            }
        }

        

        private string BuildForwardStartD1FromDrumSql(
            TraceSearchParameters p )
        {
            var sql = new StringBuilder();
            bool first = true;

            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                      -- 0 Child");
                sql.AppendLine("    ma.LotNumber,                                       -- 1");
                sql.AppendLine("    ma.ItemCode,                                        -- 2");
                sql.AppendLine("    ma.MasterKey,                                       -- 3");
                sql.AppendLine("    ma.DrumcanLotNumber" + idx + " AS DrumLotNumber,    -- 4 Parent");
                sql.AppendLine("    ma.DrumcanItemCode" + idx + " AS DrumItemCode,      -- 5");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS DrumWeight,   -- 6");
                sql.AppendLine("    '" + idx + "' AS SlotNo                             -- 7");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE 1 = 1");
                sql.AppendLine("  AND ma.DrumcanLotNumber" + idx + " = @DrumLotNumber");

                

                first = false;
            }

            return sql.ToString();
        }

        private string BuildForwardStartD2FromMaSql(
    TraceSearchParameters p,
    SqlCommand cmd)
        {
            var sql = new StringBuilder();
            bool first = true;

            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                      -- 0 Child: ProductionOrderNumber");
                sql.AppendLine("    ma.LotNumber,                                       -- 1 Child: LotNumber");
                sql.AppendLine("    ma.ItemCode,                                        -- 2 Child: ItemCode");
                sql.AppendLine("    ma.MasterKey,                                       -- 3 Child: ControlMasterKey");
                sql.AppendLine("    ma.DrumcanLotNumber" + idx + " AS DrumLotNumber,    -- 4 Parent: LotNumber");
                sql.AppendLine("    ma.DrumcanItemCode" + idx + " AS DrumItemCode,      -- 5 Parent: ItemCode");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS DrumWeight,   -- 6 Parent: Weight");
                sql.AppendLine("    '" + idx + "' AS SlotNo                             -- 7 Parent: InputSlotNo");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE 1 = 1");

                AppendStartNodeSearchConditionsForMaterialTableA(p, sql, "ma");

                sql.AppendLine("  AND ma.DrumcanLotNumber" + idx + " IS NOT NULL");
                sql.AppendLine("  AND LTRIM(RTRIM(ma.DrumcanLotNumber" + idx + ")) <> ''");

                first = false;
            }

            return sql.ToString();
        }



        /// <summary>
        /// Forward BFS STEP1-A:
        /// ManualInputLotNumber01～50 に current.LotNumber が入っている child lot 候補を抽出する。
        /// Drumcan は対象外。
        /// ここでは ChildNode 実体は作らず、ChildLotNumber だけを ChildCandidate に積む。
        /// </summary>
        private List<ChildCandidate> ExecuteForwardStep1ForA(
            ProductionResultNode current)
        {
            var result = new List<ChildCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return result;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardStep1MaterialTableASql();
                cmd.Parameters.AddWithValue("@ParentLot", current.LotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string childLotNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        var candidate = BuildForwardStep1Candidate(
                            current,
                            childLotNumber,
                            "STEP1-A");

                        if (candidate != null)
                        {
                            result.Add(candidate);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Forward BFS STEP1-B:
        /// MaterialTableB.SourceTankLotNumber01 に current.LotNumber が入っている child lot 候補を抽出する。
        /// ここでは ChildNode 実体は作らず、ChildLotNumber だけを ChildCandidate に積む。
        /// </summary>
        private List<ChildCandidate> ExecuteForwardStep1ForB(
            ProductionResultNode current)
        {
            var result = new List<ChildCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return result;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildForwardStep1MaterialTableBSql();
                cmd.Parameters.AddWithValue("@ParentLot", current.LotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string childLotNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        var candidate = BuildForwardStep1Candidate(
                            current,
                            childLotNumber,
                            "STEP1-B");

                        if (candidate != null)
                        {
                            result.Add(candidate);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Forward BFS STEP2-A:
        /// STEP1-A / STEP1-B で得た candidate を受けて、
        /// MaterialTableA から ma 本体を child node として実体化する。
        /// 
        /// 条件:
        ///   ma.ManualInputLotNumber01～50 = candidate.ParentNode.LotNumber
        /// 
        /// ChildNode に積む:
        ///   ProductionOrderNumber = ma.ForeignKey
        ///   LotNumber             = ma.LotNumber
        ///   ControlMasterKey      = ma.MasterKey
        ///   ItemCode              = ma.ItemCode
        ///   Weight                = 該当 ManualInputLoadingAmountXX
        /// </summary>
        private List<ChildCandidate> ExecuteForwardStep2ForA(
            IEnumerable<ChildCandidate> step1Candidates,
            int depth)
        {
            var result = new List<ChildCandidate>();

            if (step1Candidates == null)
                return result;

            using (var conn = CreateConnection())
            {
                conn.Open();

                foreach (var step1Candidate in step1Candidates)
                {
                    if (step1Candidate == null)
                        continue;

                    if (step1Candidate.PearentNode == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(step1Candidate.PearentNode.LotNumber))
                        continue;

                    if (string.IsNullOrWhiteSpace(step1Candidate.ChildLotNumber))
                        continue;

                    // 将来の分岐用フック
                    string parentRouteSystem = step1Candidate.PearentNode.RouteSystem;

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = BuildForwardStep2MaterialTableASql();
                        cmd.Parameters.AddWithValue("@ParentLot", step1Candidate.PearentNode.LotNumber);
                        cmd.Parameters.AddWithValue("@ChildLot", step1Candidate.ChildLotNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var childNode = new ProductionResultNode();

                                childNode.ProductionOrderNumber =
                                    reader.IsDBNull(0) ? null : reader.GetString(0);

                                childNode.LotNumber =
                                    reader.IsDBNull(1) ? null : reader.GetString(1);

                                childNode.ControlMasterKey =
                                    reader.IsDBNull(2) ? null : reader.GetString(2);

                                childNode.ItemCode =
                                    reader.IsDBNull(3) ? null : reader.GetString(3);

                                childNode.Weight =
                                    reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                                string sourceType =
                                    reader.IsDBNull(5) ? null : reader.GetString(5);

                                string slotNoText =
                                    reader.IsDBNull(6) ? null : reader.GetString(6);

                                int slotNo = 0;
                                int.TryParse(slotNoText, out slotNo);

                                childNode.ItemName = null;
                                childNode.StartDate = null;
                                childNode.EndDate = null;
                                childNode.ManufacturingProcessName = null;
                                childNode.ManufacturingTankName = null;

                                childNode.Depth = depth;
                                childNode.NodeType = "Middle";
                                childNode.ParentKey = step1Candidate.PearentNode.LotNumber;
                                childNode.RouteSystem = "A";
                                childNode.InputSourceType = sourceType;
                                childNode.InputSlotNo = slotNo;
                                childNode.IsTraceTerminal = string.IsNullOrWhiteSpace(childNode.LotNumber);

                                var candidate = BuildForwardChildCandidate(
                                    step1Candidate.PearentNode,
                                    childNode,
                                    "STEP2-A");

                                if (candidate != null)
                                {
                                    result.Add(candidate);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Forward BFS STEP2-B:
        /// STEP1-A / STEP1-B で得た candidate を受けて、
        /// MaterialTableB → SingleControlProcessTable を MasterKey で結合し、
        /// scp 本体を child node として実体化する。
        /// 
        /// 条件:
        ///   mb.SourceTankLotNumber01 = candidate.ParentNode.LotNumber
        ///   scp.MasterKey            = mb.MasterKey
        /// 
        /// ChildNode に積む:
        ///   ProductionOrderNumber = scp.ForeignKey
        ///   LotNumber             = scp.LotNumber
        ///   ItemCode              = scp.ItemCode
        ///   StartDate             = scp.StartDate
        ///   Weight                = scp.Weight
        ///   ControlMasterKey      = scp.MasterKey
        /// </summary>
        private List<ChildCandidate> ExecuteForwardStep2ForB(
            IEnumerable<ChildCandidate> step1Candidates,
            int depth)
        {
            var result = new List<ChildCandidate>();

            if (step1Candidates == null)
                return result;

            using (var conn = CreateConnection())
            {
                conn.Open();

                foreach (var step1Candidate in step1Candidates)
                {
                    if (step1Candidate == null)
                        continue;

                    if (step1Candidate.PearentNode == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(step1Candidate.PearentNode.LotNumber))
                        continue;

                    // 将来の分岐用フック
                    string parentRouteSystem = step1Candidate.PearentNode.RouteSystem;

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = BuildForwardStep2MaterialTableBSql();
                        cmd.Parameters.AddWithValue("@ParentLot", step1Candidate.PearentNode.LotNumber);
                        cmd.Parameters.AddWithValue("@ChildLot", step1Candidate.ChildLotNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var childNode = new ProductionResultNode();

                                childNode.ProductionOrderNumber =
                                    reader.IsDBNull(0) ? null : reader.GetString(0);

                                childNode.LotNumber =
                                    reader.IsDBNull(1) ? null : reader.GetString(1);

                                childNode.ItemCode =
                                    reader.IsDBNull(2) ? null : reader.GetString(2);

                                childNode.StartDate =
                                    reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);

                                childNode.Weight =
                                    reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                                childNode.ControlMasterKey =
                                    reader.IsDBNull(5) ? null : reader.GetString(5);

                                childNode.ItemName = null;
                                childNode.EndDate = null;
                                childNode.ManufacturingProcessName = null;
                                childNode.ManufacturingTankName = null;

                                childNode.Depth = depth;
                                childNode.NodeType = "Middle";
                                childNode.ParentKey = step1Candidate.PearentNode.LotNumber;
                                childNode.RouteSystem = "B";
                                childNode.InputSourceType = null;
                                childNode.InputSlotNo = 1;
                                childNode.IsTraceTerminal = string.IsNullOrWhiteSpace(childNode.LotNumber);

                                var candidate = BuildForwardChildCandidate(
                                    step1Candidate.PearentNode,
                                    childNode,
                                    "STEP2-B");

                                if (candidate != null)
                                {
                                    result.Add(candidate);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private ProductionResultNode BuildForwardStartDChildMaNode(SqlDataReader reader)
        {
            if (reader == null)
                return null;

            var node = new ProductionResultNode();

            node.ProductionOrderNumber =
                reader.IsDBNull(0) ? null : reader.GetString(0);

            node.LotNumber =
                reader.IsDBNull(1) ? null : reader.GetString(1);

            node.ItemName = null;

            node.ItemCode =
                reader.IsDBNull(2) ? null : reader.GetString(2);

            node.ControlMasterKey =
                reader.IsDBNull(3) ? null : reader.GetString(3);

            node.StartDate = null;
            node.StartDateLabel = null;
            node.EndDate = null;
            node.ManufacturingProcessName = null;
            node.ManufacturingTankName = null;
            node.Weight = null;

            node.Depth = 1;
            node.NodeType = "Middle";
            node.ParentKey = null;
            node.ParentMasterKey = null;

            node.RouteSystem = "A";
            node.InputSlotNo = 0;
            node.InputSourceType = null;

            node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

            return node;
        }

        private ProductionResultNode BuildForwardStartDParentDrumNode(SqlDataReader reader, ProductionResultNode childNode)
        {
            if (reader == null)
                return null;

            string drumLotNumber =
                reader.IsDBNull(4) ? null : reader.GetString(4);

            string drumItemCode =
                reader.IsDBNull(5) ? null : reader.GetString(5);

            float? drumWeight =
                reader.IsDBNull(6) ? (float?)null : Convert.ToSingle(reader.GetValue(6));

            string slotNoText =
                reader.IsDBNull(7) ? null : reader.GetString(7);

            int slotNo = 0;
            int.TryParse(slotNoText, out slotNo);

            var node = new ProductionResultNode();

            node.ProductionOrderNumber = null;
            node.LotNumber = drumLotNumber;
            node.ItemName = null;
            node.ItemCode = drumItemCode;
            node.StartDate = null;
            node.StartDateLabel = null;
            node.EndDate = null;
            node.ManufacturingProcessName = null;
            node.ManufacturingTankName = null;
            node.Weight = drumWeight;

            node.ControlMasterKey = BuildDrumcanSpecialNodeMasterKey(
                        drumItemCode,
                        drumLotNumber,
                        drumWeight,
                        slotNo);

            node.Depth = 0;
            node.NodeType = "Start";
            node.ParentKey = null;
            node.ParentMasterKey = null;

            node.RouteSystem = "A";
            node.InputSlotNo = slotNo;
            node.InputSourceType = "Drumcan";

            node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

            return node;
        }

        
        /// <summary>
        /// Forward BFS STEP1-A:
        /// ManualInputLotNumber01～50 から、親Lot(@ParentLot)を投入している child lot 候補を抽出する。
        /// ここでは child 実体は作らず、ChildLotNumber だけ取る。
        /// Drumcan は対象外。
        /// </summary>
        private string BuildForwardStep1MaterialTableASql()
        {
            var sql = new StringBuilder();
            bool first = true;

            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.LotNumber AS ChildLotNumber   -- 0");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.ManualInputLotNumber" + idx + " = @ParentLot");

                first = false;
            }

            return sql.ToString();
        }

        /// <summary>
        /// Forward BFS STEP1-B:
        /// MaterialTableB.SourceTankLotNumber01 から、親Lot(@ParentLot)を投入している child lot 候補を抽出する。
        /// ここでは child 実体は作らず、ChildLotNumber だけ取る。
        /// scp との JOIN は STEP2-B で行う。
        /// </summary>
        private string BuildForwardStep1MaterialTableBSql()
        {
            return @"
SELECT
    mb.LotNumber AS ChildLotNumber   -- 0
FROM dbo.MaterialTableB mb
WHERE mb.SourceTankLotNumber01 = @ParentLot
";
        }

        /// <summary>
        /// Forward BFS STEP2-A:
        /// 親Lot(@ParentLot) が ManualInputLotNumber01～50 に入っている MaterialTableA レコードを
        /// child node として確定するための SQL を生成する。
        ///
        /// ChildNode は ma レコード本体として扱う。
        /// Weight だけは一致した ManualInputLoadingAmountXX を child の重量として返す。
        /// </summary>
        private string BuildForwardStep2MaterialTableASql()
        {
            var sql = new StringBuilder();
            bool first = true;

            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                     -- 0");
                sql.AppendLine("    ma.LotNumber,                                      -- 1");
                sql.AppendLine("    ma.MasterKey,                                      -- 2");
                sql.AppendLine("    ma.ItemCode,                                       -- 3");
                sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS Weight,  -- 4");
                sql.AppendLine("    'ManualInput' AS SourceType,                       -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                            -- 6");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.ManualInputLotNumber" + idx + " = @ParentLot");
                sql.AppendLine("AND ma.LotNumber = @ChildLot");

                first = false;
            }

            return sql.ToString();
        }

        /// <summary>
        /// Forward BFS STEP2-B:
        /// STEP1-A / STEP1-B で得た candidate に対して、
        /// ParentLot = mb.SourceTankLotNumber01
        /// ChildLot  = scp.LotNumber
        /// で候補を絞り、mb ↔ scp は MasterKey で結合して
        /// B 系 child 実体を取得する。
        ///
        /// ここでの MasterKey は BFS 探索条件ではなく、
        /// MaterialTableB と SingleControlProcessTable の TBL 間結合条件として使う。
        /// supplement は持たない。
        /// </summary>
        private string BuildForwardStep2MaterialTableBSql()
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
  AND scp.LotNumber = @ChildLot
";
        }

        private void AppendForwardStartDCandidatesDeduped(
    List<ChildCandidate> dest,
    HashSet<string> seen,
    List<ChildCandidate> source)
        {
            if (dest == null || seen == null || source == null)
                return;

            foreach (var candidate in source)
            {
                if (candidate == null)
                    continue;

                string key = BuildForwardStartDDedupeKey(candidate);
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (!seen.Add(key))
                    continue;

                dest.Add(candidate);
            }
        }

        private string BuildForwardRelationKey(
    ProductionResultNode parentNode,
    ProductionResultNode childNode)
        {
            if (parentNode == null || childNode == null)
                return "MISSING";

            if (string.IsNullOrWhiteSpace(parentNode.LotNumber) ||
                string.IsNullOrWhiteSpace(childNode.LotNumber))
                return "MISSING";

            return parentNode.LotNumber + "|" + childNode.LotNumber;
        }

        private ChildCandidate BuildForwardChildCandidate(
    ProductionResultNode parentNode,
    ProductionResultNode childNode,
    string debugSource)
        {
            if (parentNode == null || childNode == null)
                return null;

            return new ChildCandidate
            {
                PearentNode = parentNode,
                ChildNode = childNode,
                ChildLotNumber = childNode.LotNumber,
                RelationKey = BuildForwardRelationKey(parentNode, childNode),
                DebugSource = debugSource
            };
        }

        /// <summary>
        /// Forward BFS STEP1 用:
        /// current と childLotNumber から、未完成の ChildCandidate を作る。
        /// STEP1 では ChildNode / RelationKey はまだ作らない。
        /// </summary>
        private ChildCandidate BuildForwardStep1Candidate(
            ProductionResultNode current,
            string childLotNumber,
            string debugSource)
        {
            if (current == null)
                return null;

            if (string.IsNullOrWhiteSpace(childLotNumber))
                return null;

            return new ChildCandidate
            {
                PearentNode = current,
                ChildNode = null,
                ChildLotNumber = childLotNumber,
                RelationKey = null,
                DebugSource = debugSource
            };
        }

        private string BuildForwardStartDDedupeKey(ChildCandidate candidate)
        {
            if (candidate == null ||
                candidate.PearentNode == null ||
                candidate.ChildNode == null)
                return null;

            return candidate.PearentNode.NodeIdentityKey
                + "->"
                + candidate.ChildNode.NodeIdentityKey;
        }

        #endregion



        #region 新トレースバック関連群

        private sealed class BackwardParentLotCandidate
        {
            public string ParentLotNumber { get; set; }

            // Step1(A) で拾える補完用情報
            public string ItemCode { get; set; }
            public float? Weight { get; set; }
            public MaterialAInputType MaterialAInputType { get; set; }
            public int SlotNo { get; set; }

            // A/B のどちら由来の親Lot候補か
            public TraceSourceTable SourceTable { get; set; }
        }

        private sealed class BackwardStep1DrumcanRow
        {
            public string ChildMasterKey { get; set; }
            public string ParentLotNumber { get; set; }
            public string ItemCode { get; set; }
            public float? Weight { get; set; }
            public string SourceType { get; set; }
            public int SlotNo { get; set; }
        }


        private List<BackwardParentCandidate> ExecuteBackwardStep1ForA(
    ProductionResultNode current)
        {
            var parentCandidates = new List<BackwardParentCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return parentCandidates;

            if (!string.Equals(current.RouteSystem, "A", StringComparison.OrdinalIgnoreCase))
                return parentCandidates;

            if (!string.Equals(current.InputSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase))
                return parentCandidates;

            if (!current.InputSlotNo.HasValue || current.InputSlotNo.Value <= 0)
                throw new InvalidOperationException(
                    "Backward Step1-A requires current.InputSlotNo for ManualInput.");

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                var sql = BuildBackwardStep1MaterialTableASql(
                    current.InputSourceType,
                    current.InputSlotNo.Value);

                if (!string.IsNullOrWhiteSpace(current.ControlMasterKey))
                {
                    sql = sql.Replace(
                        "/*CHILD_MASTERKEY_CONDITION_A*/",
                        "  AND ma.MasterKey = @ChildMasterKey");
                }
                else
                {
                    sql = sql.Replace("/*CHILD_MASTERKEY_CONDITION_A*/", string.Empty);
                }

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ChildLotNumber", current.LotNumber);
                cmd.Parameters.AddWithValue("@ChildSourceType", current.InputSourceType);
                cmd.Parameters.AddWithValue("@ChildSlotNoText", current.InputSlotNo.Value.ToString("00"));

                if (!string.IsNullOrWhiteSpace(current.ControlMasterKey))
                {
                    cmd.Parameters.AddWithValue("@ChildMasterKey", current.ControlMasterKey);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string childMasterKey =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        string parentLotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        if (string.IsNullOrWhiteSpace(parentLotNumber))
                            continue;

                        string itemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        float? weight =
                            reader.IsDBNull(3) ? (float?)null : Convert.ToSingle(reader.GetValue(3));

                        string sourceType =
                            reader.IsDBNull(4) ? null : reader.GetString(4);

                        string slotNoText =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

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

                        var childNode = new ProductionResultNode();
                        childNode.ProductionOrderNumber = current.ProductionOrderNumber;
                        childNode.LotNumber = current.LotNumber;
                        childNode.ItemName = current.ItemName;
                        childNode.ItemCode = current.ItemCode;
                        childNode.StartDate = current.StartDate;
                        childNode.EndDate = current.EndDate;
                        childNode.ManufacturingProcessName = current.ManufacturingProcessName;
                        childNode.ManufacturingTankName = current.ManufacturingTankName;
                        childNode.Weight = weight;//重量は子Node側で保持
                        childNode.ControlMasterKey = childMasterKey;
                        childNode.Depth = current.Depth;
                        childNode.NodeType = current.NodeType;
                        childNode.ParentKey = current.ParentKey;
                        childNode.RouteSystem = current.RouteSystem;
                        childNode.InputSlotNo = current.InputSlotNo;
                        childNode.InputSourceType = current.InputSourceType;
                        childNode.IsTraceTerminal = current.IsTraceTerminal;

                        var node = new ProductionResultNode();
                        node.ProductionOrderNumber = null;
                        node.LotNumber = parentLotNumber;
                        node.ItemName = null;
                        node.ItemCode = itemCode;
                        node.StartDate = null;
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;
                        node.Weight = null;//重量は子Node側で保持
                        node.ControlMasterKey = null;
                        node.Depth = 0;
                        node.NodeType = "Middle";
                        node.ParentKey = null;
                        node.RouteSystem = "A";
                        node.InputSlotNo = slotNo;
                        node.InputSourceType = inputType == MaterialAInputType.None
                            ? null
                            : inputType.ToString();
                        node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                        parentCandidates.Add(new BackwardParentCandidate
                        {
                            Node = node,
                            ChildNode = childNode,
                            ParentLotNumber = parentLotNumber,
                            RelationKey = null,
                            DebugSource = "STEP1-A"
                        });
                       
                    }
                }
            }

            return parentCandidates;
        }

        private List<BackwardParentCandidate> ExecuteBackwardStep1ForB(
    ProductionResultNode current)
        {
            var parentCandidates = new List<BackwardParentCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return parentCandidates;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                var sql = BuildBackwardStep1MaterialTableBSql();

                if (!string.IsNullOrWhiteSpace(current.ControlMasterKey))
                {
                    sql = sql.Replace(
                        "/*CHILD_MASTERKEY_CONDITION_B*/",
                        "  AND mb.MasterKey = @ChildMasterKey");
                }
                else
                {
                    sql = sql.Replace("/*CHILD_MASTERKEY_CONDITION_B*/", string.Empty);
                }

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ChildLotNumber", current.LotNumber);

                if (!string.IsNullOrWhiteSpace(current.ControlMasterKey))
                {
                    cmd.Parameters.AddWithValue("@ChildMasterKey", current.ControlMasterKey);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string childMasterKey =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        string parentLotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        if (string.IsNullOrWhiteSpace(parentLotNumber))
                            continue;

                        var childNode = new ProductionResultNode();
                        childNode.ProductionOrderNumber = current.ProductionOrderNumber;
                        childNode.LotNumber = current.LotNumber;
                        childNode.ItemName = current.ItemName;
                        childNode.ItemCode = current.ItemCode;
                        childNode.StartDate = current.StartDate;
                        childNode.EndDate = current.EndDate;
                        childNode.ManufacturingProcessName = current.ManufacturingProcessName;
                        childNode.ManufacturingTankName = current.ManufacturingTankName;
                        childNode.Weight = current.Weight;
                        childNode.ControlMasterKey = childMasterKey;
                        childNode.Depth = current.Depth;
                        childNode.NodeType = current.NodeType;
                        childNode.ParentKey = current.ParentKey;
                        childNode.RouteSystem = current.RouteSystem;
                        childNode.InputSlotNo = current.InputSlotNo;
                        childNode.InputSourceType = current.InputSourceType;
                        childNode.IsTraceTerminal = current.IsTraceTerminal;

                        var node = new ProductionResultNode();
                        node.ProductionOrderNumber = null;
                        node.LotNumber = parentLotNumber;
                        node.ItemName = null;
                        node.ItemCode = null;
                        node.StartDate = null;
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;
                        node.Weight = null;
                        node.ControlMasterKey = null;
                        node.Depth = 0;
                        node.NodeType = "Middle";
                        node.ParentKey = null;
                        node.RouteSystem = "B";
                        node.InputSlotNo = 1;
                        node.InputSourceType = null;
                        node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                        parentCandidates.Add(new BackwardParentCandidate
                        {
                            Node = node,
                            ChildNode = childNode,
                            ParentLotNumber = parentLotNumber,
                            RelationKey = null,
                            DebugSource = "STEP1-B"
                        });
                       
                    }
                }
            }

            return parentCandidates;
        }


        /// <summary>
        /// 当面は使用しない。FALLBACK用に必要であれば復帰
        /// </summary>
        /// <param name="childSourceType"></param>
        /// <param name="childSlotNo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string BuildBackwardStep1MaterialTableASql(
    string childSourceType,
    int childSlotNo)
        {
            if (string.IsNullOrWhiteSpace(childSourceType))
                throw new ArgumentException("childSourceType");

            if (childSlotNo <= 0)
                throw new ArgumentOutOfRangeException("childSlotNo");

            string idx = childSlotNo.ToString("00");

            string lotColumn;
            string itemCodeColumn;
            string amountColumn;

            
            if (string.Equals(childSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase))
            {
                if (childSlotNo > 50)
                    throw new ArgumentOutOfRangeException("childSlotNo");

                lotColumn = "ma.ManualInputLotNumber" + idx;
                itemCodeColumn = "ma.ManualInputItemCode" + idx;
                amountColumn = "ma.ManualInputLoadingAmount" + idx;
            }
            else
            {
                throw new ArgumentException("childSourceType");
            }

            var sql = new StringBuilder();

            sql.AppendLine("SELECT");
            sql.AppendLine("    ma.MasterKey AS ChildMasterKey,                     -- 0");
            sql.AppendLine("    " + lotColumn + " AS ParentLotNumber,               -- 1");
            sql.AppendLine("    " + itemCodeColumn + " AS ItemCode,                 -- 2");
            sql.AppendLine("    " + amountColumn + " AS AmountWeight,               -- 3");
            sql.AppendLine("    @ChildSourceType AS SourceType,                     -- 4");
            sql.AppendLine("    @ChildSlotNoText AS SlotNo                          -- 5");
            sql.AppendLine("FROM dbo.MaterialTableA ma");
            sql.AppendLine("WHERE ma.LotNumber = @ChildLotNumber");
            sql.AppendLine("/*CHILD_MASTERKEY_CONDITION_A*/");
            sql.AppendLine("  AND " + lotColumn + " IS NOT NULL");
            sql.AppendLine("  AND LTRIM(RTRIM(" + lotColumn + ")) <> ''");

            return sql.ToString();
        }

        private string BuildBackwardStep1MaterialTableBSql()
        {
            return @"
SELECT DISTINCT
    mb.MasterKey AS ChildMasterKey,            -- 0
    mb.SourceTankLotNumber01 AS ParentLotNumber -- 1
FROM dbo.MaterialTableB mb
WHERE mb.LotNumber = @ChildLotNumber
/*CHILD_MASTERKEY_CONDITION_B*/
  AND mb.SourceTankLotNumber01 IS NOT NULL
  AND LTRIM(RTRIM(mb.SourceTankLotNumber01)) <> ''
";
        }

        private List<BackwardParentCandidate> ExecuteBackwardStep1ForD(
    ProductionResultNode current,int depth)
        {
            var parentCandidates = new List<BackwardParentCandidate>();
            var aRows = new List<BackwardStep1DrumcanRow>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return parentCandidates;

            if (!string.Equals(current.RouteSystem, "A", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(current.RouteSystem, "B", StringComparison.OrdinalIgnoreCase))
            {
                return parentCandidates;
            }

            if (string.Equals(current.RouteSystem, "A", StringComparison.OrdinalIgnoreCase) &&
                 string.Equals(current.InputSourceType, "Drumcan", StringComparison.OrdinalIgnoreCase))
            {
                return parentCandidates;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildBackwardStep1DrumcanSql();
                cmd.Parameters.AddWithValue("@ChildLotNumber", current.LotNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string childMasterKey =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        string drumcanParentLotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        if (string.IsNullOrWhiteSpace(drumcanParentLotNumber))
                            continue;

                        string drumcanItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        float? drumcanWeight =
                            reader.IsDBNull(3) ? (float?)null : Convert.ToSingle(reader.GetValue(3));

                        string sourceType =
                            reader.IsDBNull(4) ? null : reader.GetString(4);

                        string slotNoText =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

                        int slotNo = 0;
                        int.TryParse(slotNoText, out slotNo);

                        if (string.Equals(current.RouteSystem, "B", StringComparison.OrdinalIgnoreCase))
                        {
                            var bCandidate = BuildBackwardStep1DrumcanCandidateForB(
                                current,
                                childMasterKey,
                                drumcanParentLotNumber,
                                drumcanItemCode,
                                drumcanWeight,
                                sourceType,
                                slotNo,
                                depth);

                            if (bCandidate != null)
                            {
                                parentCandidates.Add(bCandidate);
                            }

                            continue;
                        }

                        aRows.Add(new BackwardStep1DrumcanRow
                        {
                            ChildMasterKey = childMasterKey,
                            ParentLotNumber = drumcanParentLotNumber,
                            ItemCode = drumcanItemCode,
                            Weight = drumcanWeight,
                            SourceType = sourceType,
                            SlotNo = slotNo
                        });
                    }
                }
            }

            if (aRows.Count > 0)
            {
                AppendBackwardStep1DrumcanCandidatesForA(
                    parentCandidates,
                    current,
                    aRows,
                    depth);
            }

            return parentCandidates;
        }

        private void AppendBackwardStep1DrumcanCandidatesForA(
    List<BackwardParentCandidate> parentCandidates,
    ProductionResultNode current,
    List<BackwardStep1DrumcanRow> drumcanRows,
    int depth)
        {
            if (parentCandidates == null || current == null)
                return;

            if (drumcanRows == null || drumcanRows.Count == 0)
                return;

            var firstRow = drumcanRows.FirstOrDefault(x => x != null);
            if (firstRow == null)
                return;

            var parentC = new ProductionResultNode();
            parentC.ProductionOrderNumber = null;
            parentC.LotNumber = current.LotNumber;
            parentC.ItemName = null;
            parentC.ItemCode = current.ItemCode;
            parentC.StartDate = null;
            parentC.StartDateLabel = null;
            parentC.EndDate = null;
            parentC.ManufacturingProcessName = null;
            parentC.ManufacturingTankName = null;
            parentC.Weight = null;
            parentC.ControlMasterKey = BuildDeterministicSpecialNodeMasterKey(
                "A_STEP1_DRUMCAN_PARENT_C",
                firstRow.ChildMasterKey ?? current.ControlMasterKey ?? current.LotNumber,
                parentC.ItemCode,
                parentC.LotNumber,
                parentC.Weight,
                "Drumcan",
                null);
            parentC.Depth = 0;
            parentC.NodeType = "Middle";
            parentC.ParentKey = null;
            parentC.ParentMasterKey = null;
            parentC.RouteSystem = "A";
            parentC.InputSlotNo = null;
            parentC.InputSourceType = "Drumcan";
            parentC.IsTraceTerminal = string.IsNullOrWhiteSpace(parentC.LotNumber);

            var candidate = FinalizeBackwardParentCandidate(new BackwardParentCandidate
            {
                Node = parentC,
                ChildNode = current,
                ParentLotNumber = parentC.LotNumber,
                RelationKey = null,
                DebugSource = "STEP1-D-C"
            });
            if (candidate != null)
            {
                parentCandidates.Add(candidate);
            }

            foreach (var row in drumcanRows)
            {
                if (row == null || string.IsNullOrWhiteSpace(row.ParentLotNumber))
                    continue;

                var drumcanParent = new ProductionResultNode();
                drumcanParent.ProductionOrderNumber = null;
                drumcanParent.LotNumber = row.ParentLotNumber;
                drumcanParent.ItemName = null;
                drumcanParent.ItemCode = row.ItemCode;
                drumcanParent.StartDate = null;
                drumcanParent.StartDateLabel = null;
                drumcanParent.EndDate = null;
                drumcanParent.ManufacturingProcessName = null;
                drumcanParent.ManufacturingTankName = null;
                drumcanParent.Weight = row.Weight;
                drumcanParent.ControlMasterKey = BuildDrumcanSpecialNodeMasterKey(
                    drumcanParent.ItemCode,
                    drumcanParent.LotNumber,
                    drumcanParent.Weight,
                    row.SlotNo);
                drumcanParent.Depth = 0;
                drumcanParent.NodeType = "Middle";
                drumcanParent.ParentKey = null;
                drumcanParent.ParentMasterKey = null;
                drumcanParent.RouteSystem = "A";
                drumcanParent.InputSlotNo = row.SlotNo;
                drumcanParent.InputSourceType = row.SourceType;
                drumcanParent.IsTraceTerminal = string.IsNullOrWhiteSpace(drumcanParent.LotNumber);

                candidate = FinalizeBackwardParentCandidate(new BackwardParentCandidate
                {
                    Node = drumcanParent,
                    ChildNode = parentC,
                    ParentLotNumber = drumcanParent.LotNumber,
                    RelationKey = null,
                    DebugSource = "STEP1-D-P"
                });
                if (candidate != null)
                {
                    parentCandidates.Add(candidate);
                }
            }
        }

        private BackwardParentCandidate BuildBackwardStep1DrumcanCandidateForB(
    ProductionResultNode current,
    string childMasterKey,
    string drumcanParentLotNumber,
    string drumcanItemCode,
    float? drumcanWeight,
    string sourceType,
    int slotNo,
    int depth)
        {
            if (current == null)
                return null;

            if (!string.Equals(current.RouteSystem, "B", StringComparison.OrdinalIgnoreCase))
                return null;

            if (string.IsNullOrWhiteSpace(drumcanParentLotNumber))
                return null;

            var drumcanParent = new ProductionResultNode();
            drumcanParent.ProductionOrderNumber = null;
            drumcanParent.LotNumber = drumcanParentLotNumber;
            drumcanParent.ItemName = null;
            drumcanParent.ItemCode = drumcanItemCode;
            drumcanParent.StartDate = null;
            drumcanParent.StartDateLabel = null;
            drumcanParent.EndDate = null;
            drumcanParent.ManufacturingProcessName = null;
            drumcanParent.ManufacturingTankName = null;
            drumcanParent.Weight = drumcanWeight;
            drumcanParent.ControlMasterKey = BuildDrumcanSpecialNodeMasterKey(
                drumcanParent.ItemCode,
                drumcanParent.LotNumber,
                drumcanParent.Weight,
                slotNo);
            drumcanParent.Depth = 0;
            drumcanParent.NodeType = "Middle";
            drumcanParent.ParentKey = null;
            drumcanParent.ParentMasterKey = null;
            drumcanParent.RouteSystem = "A";
            drumcanParent.InputSlotNo = slotNo;
            drumcanParent.InputSourceType = sourceType;
            drumcanParent.IsTraceTerminal = string.IsNullOrWhiteSpace(drumcanParent.LotNumber);

            return FinalizeBackwardParentCandidate(new BackwardParentCandidate
            {
                Node = drumcanParent,
                ChildNode = current,
                ParentLotNumber = drumcanParent.LotNumber,
                RelationKey = null,
                DebugSource = "STEP1-D-B"
            });
        }

        private string BuildBackwardStep1DrumcanSql()
        {
            var sql = new StringBuilder();
            bool first = true;

            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.MasterKey AS ChildMasterKey,                        -- 0");
                sql.AppendLine("    ma.DrumcanLotNumber" + idx + " AS ParentLotNumber,     -- 1");
                sql.AppendLine("    ma.DrumcanItemCode" + idx + " AS ItemCode,             -- 2");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS AmountWeight,    -- 3");
                sql.AppendLine("    'Drumcan' AS SourceType,                               -- 4");
                sql.AppendLine("    '" + idx + "' AS SlotNo                                -- 5");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.LotNumber = @ChildLotNumber");
                sql.AppendLine("  AND ma.DrumcanLotNumber" + idx + " IS NOT NULL");
                sql.AppendLine("  AND LTRIM(RTRIM(ma.DrumcanLotNumber" + idx + ")) <> ''");

                first = false;
            }

            return sql.ToString();
        }

        private string BuildBackwardStep2MaterialTableASql()
        {
            var sql = new StringBuilder();

            bool first = true;

            // ManualInputLoadingAmount01～50
            for (int i = 1; i <= 50; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                        -- 0");
                sql.AppendLine("    ma.LotNumber,                                         -- 1");
                sql.AppendLine("    ma.MasterKey,                                         -- 2");
                sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS AmountWeight, -- 3");
                sql.AppendLine("    'ManualInput' AS SourceType,                          -- 4");
                sql.AppendLine("    '" + idx + "' AS SlotNo                               -- 5");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE ma.LotNumber = @ParentLotNumber");
                sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " IS NOT NULL");
                sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " <> 0");

                first = false;
            }

            
            return sql.ToString();
        }

        private string BuildBackwardStep2SingleControlProcessTableBSql()
        {
            return @"
SELECT DISTINCT
    scp.ForeignKey,   -- 0
    scp.LotNumber,    -- 1
    scp.ItemCode,     -- 2
    scp.StartDate,    -- 3
    scp.Weight,       -- 4
    scp.MasterKey     -- 5
FROM dbo.SingleControlProcessTable scp
WHERE scp.LotNumber = @ParentLotNumber
  AND scp.MasterKey LIKE '%[_]%'
  -- 種別（フォワードと同じ）
  AND SUBSTRING(
        scp.MasterKey,
        CHARINDEX('_', scp.MasterKey, CHARINDEX('_', scp.MasterKey) + 1) + 2,
        1
      ) IN ('2','3')
  -- 工程（末尾 '_' の1文字前）
  AND SUBSTRING(
        scp.MasterKey,
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),
        1
      ) IN ('4','7')
  
";
        }


        private List<BackwardParentCandidate> ExecuteBackwardStep1ParentLots(
        ProductionResultNode current,int depth)
        {
            var parentCandidates = new List<BackwardParentCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return parentCandidates;

            var aCandidates = ExecuteBackwardStep1ForA(current);
            if (aCandidates != null && aCandidates.Count > 0)
            {
                parentCandidates.AddRange(aCandidates);
            }

            var dCandidates = ExecuteBackwardStep1ForD(current, depth);
            if (dCandidates != null && dCandidates.Count > 0)
            {
                parentCandidates.AddRange(dCandidates);
            }

            var bCandidates = ExecuteBackwardStep1ForB(current);
            if (bCandidates != null && bCandidates.Count > 0)
            {
                parentCandidates.AddRange(bCandidates);
            }

            return parentCandidates;
        }

        



        private List<BackwardParentCandidate> ExecuteBackwardStep2ForA(
    IEnumerable<BackwardParentCandidate> parentCandidates,
    int depth)
        {
            var parents = new List<BackwardParentCandidate>();

            if (parentCandidates == null)
                return parents;

            using (var conn = CreateConnection())
            {
                conn.Open();

                foreach (var parentCandidate in parentCandidates)
                {
                    if (parentCandidate == null)
                        continue;

                    if (parentCandidate.Node == null)
                        continue;

                    bool isStep1A =
                        string.Equals(parentCandidate.Node.RouteSystem, "A", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(parentCandidate.Node.InputSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase);

                    bool isStep1B =
                        string.Equals(parentCandidate.DebugSource, "STEP1-B", StringComparison.OrdinalIgnoreCase);

                    // STEP1-A or STEP1-B のどちらかなら通す
                    if (!isStep1A && !isStep1B)
                    {
                        continue;
                    }



                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = BuildBackwardStep2MaterialTableASql();
                        cmd.Parameters.AddWithValue("@ParentLotNumber", parentCandidate.ParentLotNumber);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                              

                                string foreignKey =
                                    reader.IsDBNull(0) ? null : reader.GetString(0);

                                string lotNumber =
                                    reader.IsDBNull(1) ? null : reader.GetString(1);

                                string masterKey =
                                    reader.IsDBNull(2) ? null : reader.GetString(2);

                                float? amountWeight =
                                    reader.IsDBNull(3) ? (float?)null : Convert.ToSingle(reader.GetValue(3));

                                string sourceType =
                                    reader.IsDBNull(4) ? null : reader.GetString(4);

                                string slotNoText =
                                    reader.IsDBNull(5) ? null : reader.GetString(5);

                                int slotNo = 0;
                                int.TryParse(slotNoText, out slotNo);

                                MaterialAInputType inputType = MaterialAInputType.None;
                                
                                if (string.Equals(sourceType, "ManualInput", StringComparison.OrdinalIgnoreCase))
                                {
                                    inputType = MaterialAInputType.ManualInput;
                                }

                                var step1SeedNode = parentCandidate.Node;

                                var node = new ProductionResultNode();
                                node.ProductionOrderNumber = foreignKey;
                                node.LotNumber = lotNumber;
                                node.ItemName = null;

                                // Step1 で取得済みの ItemCode は維持
                                node.ItemCode = step1SeedNode.ItemCode;

                                node.StartDate = null;
                                node.EndDate = null;
                                node.ManufacturingProcessName = null;
                                node.ManufacturingTankName = null;
                                //node.Weight = null; //重量は子Node側だけ表示
                                parentCandidate.ChildNode.Weight = amountWeight;
                                node.Weight = amountWeight;
                                node.ControlMasterKey = masterKey;
                                node.Depth = depth;
                                node.NodeType = "Middle";
                                node.ParentKey = null;
                                node.RouteSystem = "A";
                                node.InputSlotNo = slotNo;
                                node.InputSourceType = inputType == MaterialAInputType.None
                                    ? null
                                    : inputType.ToString();
                                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);



                                var candidate = FinalizeBackwardParentCandidate(new BackwardParentCandidate
                                {
                                    Node = node,
                                    ChildNode = parentCandidate.ChildNode,
                                    ParentLotNumber = parentCandidate.ParentLotNumber,
                                    RelationKey = null,
                                    DebugSource = "STEP2-A"
                                });
                                if (candidate != null)
                                {
                                    parents.Add(candidate);
                                }

                            }
                        }
                    }

                    
                }
            }

            return parents;
        }

        private List<BackwardParentCandidate> ExecuteBackwardStep2ForB(
    IEnumerable<BackwardParentCandidate> parentCandidates,
    int depth)
        {
            var parents = new List<BackwardParentCandidate>();

            if (parentCandidates == null)
                return parents;

            using (var conn = CreateConnection())
            {
                conn.Open();

                foreach (var parentCandidate in parentCandidates)
                {
                    if (parentCandidate == null)
                        continue;

                    if (parentCandidate.Node == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(parentCandidate.ParentLotNumber))
                        continue;

                    bool isFromStep1A =
               string.Equals(parentCandidate.Node.RouteSystem, "A", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(parentCandidate.Node.InputSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase);

                    bool isFromStep1B =
                        string.Equals(parentCandidate.Node.RouteSystem, "B", StringComparison.OrdinalIgnoreCase) &&
                        string.IsNullOrWhiteSpace(parentCandidate.Node.InputSourceType);

                    if (!isFromStep1A && !isFromStep1B)
                        continue;

                    // STEP2-B は STEP1-A / STEP1-B のどちら由来でも ParentLotNumber があれば照会対象にする
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = BuildBackwardStep2SingleControlProcessTableBSql();
                        cmd.Parameters.AddWithValue("@ParentLotNumber", parentCandidate.ParentLotNumber);

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
                                node.RouteSystem = "B";
                                node.InputSlotNo = 1;
                                node.InputSourceType = null;
                                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);

                                var candidate = FinalizeBackwardParentCandidate(new BackwardParentCandidate
                                {
                                    Node = node,
                                    ChildNode = parentCandidate.ChildNode,
                                    ParentLotNumber = parentCandidate.ParentLotNumber,
                                    RelationKey = null,
                                    DebugSource = "STEP2-B"
                                });
                                if (candidate != null)
                                {
                                    parents.Add(candidate);
                                }
                            }
                        }
                    }
                }
            }

            return parents;
        }

        public List<BackwardParentCandidate> FindBackwardParentsByLotStepFlow(
    ProductionResultNode current,
    int depth)
        {
            var parents = new List<BackwardParentCandidate>();

            if (current == null || string.IsNullOrWhiteSpace(current.LotNumber))
                return parents;

            var parentCandidates = ExecuteBackwardStep1ParentLots(current, depth);
            if (parentCandidates == null || parentCandidates.Count == 0)
                return parents;

            var step2Targets = new List<BackwardParentCandidate>();

            foreach (var parentCandidate in parentCandidates)
            {
                if (parentCandidate == null)
                    continue;

                if (parentCandidate.Node == null)
                    continue;

                bool isDrumcanDirect =
                    string.Equals(parentCandidate.Node.RouteSystem, "A", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(parentCandidate.Node.InputSourceType, "Drumcan", StringComparison.OrdinalIgnoreCase);

                if (isDrumcanDirect)
                {
                    parents.Add(parentCandidate);
                    continue;
                }

                step2Targets.Add(parentCandidate);
            }

            var bParents = ExecuteBackwardStep2ForB(step2Targets, depth);
            var aParents = ExecuteBackwardStep2ForA(step2Targets, depth);

            // STEP2-B で解決済みの ParentLotNumber
            var resolvedByB = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (bParents != null)
            {
                foreach (var bParent in bParents)
                {
                    if (bParent == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(bParent.ParentLotNumber))
                        continue;

                    resolvedByB.Add(bParent.ParentLotNumber);
                    parents.Add(bParent);
                }
            }

            // STEP2-A の実体を追加
            var resolvedByA = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (aParents != null)
            {
                foreach (var aParent in aParents)
                {
                    if (aParent == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(aParent.ParentLotNumber))
                    {
                        resolvedByA.Add(aParent.ParentLotNumber);
                    }

                    parents.Add(aParent);
                }
            }

            foreach (var parentCandidate in step2Targets)
            {
                if (!ShouldCreateBackwardStep2AFallback(parentCandidate, resolvedByA, resolvedByB))
                    continue;

                var fallbackCandidate = BuildBackwardStep2AFallbackCandidate(parentCandidate, depth);
                if (fallbackCandidate != null)
                {
                    parents.Add(fallbackCandidate);
                }
            }

            /// STEP2-A / STEP2-B どちらも解決できなかった ParentLotNumber は、現状のところはバックトレース打ち切り（IsTraceTerminal = true）とする。
            if (parents.Count == 0)
            {
                current.IsTraceTerminal = true;
            }

            return RemoveDuplicateBackwardParentCandidates(parents);
        }


        private BackwardParentCandidate BuildBackwardStep2AFallbackCandidate(
    BackwardParentCandidate parentCandidate,
    int depth)
        {
            if (parentCandidate == null)
                return null;

            if (parentCandidate.Node == null)
                return null;

            var step1SeedNode = parentCandidate.Node;

            var fallbackNode = new ProductionResultNode();
            fallbackNode.ProductionOrderNumber = null;
            fallbackNode.LotNumber = step1SeedNode.LotNumber;
            fallbackNode.ItemName = null;
            fallbackNode.ItemCode = step1SeedNode.ItemCode;
            fallbackNode.StartDate = null;
            fallbackNode.EndDate = null;
            fallbackNode.ManufacturingProcessName = null;
            fallbackNode.ManufacturingTankName = null;
            fallbackNode.Weight = step1SeedNode.Weight;
            fallbackNode.RouteSystem = "A";
            fallbackNode.InputSlotNo = step1SeedNode.InputSlotNo;
            fallbackNode.InputSourceType = step1SeedNode.InputSourceType;
            fallbackNode.ControlMasterKey = BuildDeterministicSpecialNodeMasterKey(
                "A_STEP1_FALLBACK",
                parentCandidate.ParentLotNumber,
                fallbackNode.ItemCode,
                fallbackNode.LotNumber,
                fallbackNode.Weight,
                fallbackNode.InputSourceType,
                fallbackNode.InputSlotNo);
            fallbackNode.Depth = depth;
            fallbackNode.NodeType = "Middle";
            fallbackNode.ParentKey = null;
            fallbackNode.IsTraceTerminal = string.IsNullOrWhiteSpace(fallbackNode.LotNumber);

            return new BackwardParentCandidate
            {
                Node = fallbackNode,
                ChildNode = parentCandidate.ChildNode,
                ParentLotNumber = parentCandidate.ParentLotNumber,
                RelationKey = null,
                DebugSource = "STEP2-A-FALLBACK"
            };
        }

        private bool ShouldCreateBackwardStep2AFallback(
    BackwardParentCandidate parentCandidate,
    ISet<string> resolvedByA,
    ISet<string> resolvedByB)
        {
            if (parentCandidate == null)
                return false;

            if (parentCandidate.Node == null)
                return false;

            if (!string.Equals(
                parentCandidate.Node.RouteSystem,
                "A",
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(
                parentCandidate.Node.InputSourceType,
                "ManualInput",
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(parentCandidate.ParentLotNumber))
                return false;

            if (resolvedByB != null && resolvedByB.Contains(parentCandidate.ParentLotNumber))
                return false;

            if (resolvedByA != null && resolvedByA.Contains(parentCandidate.ParentLotNumber))
                return false;

            return true;
        }


        public List<ProductionResultNode> FindBackwardStartNodes(TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();
            var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var startB = FindStartNodesFromMaterialTableB(p);
            var startAManual = FindStartNodesFromMaterialTableAManualInput(p);

            // 通常のバックトレース入口。
            // ここでは Current として BFS に渡す始点のみを返す。
            // Drumcan(StartD) は始点時点で 1世代分の親子関係を確定できるため、
            // FindBackwardStartDrumcanCandidates(...) で別処理する。
            AppendBackwardStartNodes(result, seenKeys, startB, "B");
            AppendBackwardStartNodes(result, seenKeys, startAManual, "A");

            // 通常Aは fallback 専用。
            // 必要になったらここを復活させる。
            // if (result.Count == 0)
            // {
            //     var startA = FindStartNodesFromMaterialTableA(p);
            //     AppendBackwardStartNodes(result, seenKeys, startA, "A");
            // }

            return result;
        }

        public List<BackwardParentCandidate> FindBackwardStartDrumcanCandidates(TraceSearchParameters p)
        {
            var result = new List<BackwardParentCandidate>();
            var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (p == null)
                return result;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildStartNodesFromMaterialTableADrumcanSql(p, cmd);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var childNode = new ProductionResultNode();

                        childNode.ProductionOrderNumber =
                            reader.IsDBNull(0) ? null : reader.GetString(0);

                        childNode.LotNumber =
                            reader.IsDBNull(1) ? null : reader.GetString(1);

                        childNode.ItemName = null;

                        childNode.ItemCode =
                            reader.IsDBNull(2) ? null : reader.GetString(2);

                        childNode.ControlMasterKey =
                            reader.IsDBNull(3) ? null : reader.GetString(3);

                        childNode.Weight =
                            reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));

                        string sourceType =
                            reader.IsDBNull(5) ? null : reader.GetString(5);

                        string slotNoText =
                            reader.IsDBNull(6) ? null : reader.GetString(6);

                        string parentLotNumber =
                            reader.IsDBNull(7) ? null : reader.GetString(7);

                        string parentItemCode =
                            reader.IsDBNull(8) ? null : reader.GetString(8);

                        float? parentWeight =
                            reader.IsDBNull(9) ? (float?)null : Convert.ToSingle(reader.GetValue(9));

                        int slotNo = 0;
                        int.TryParse(slotNoText, out slotNo);

                        if (string.IsNullOrWhiteSpace(childNode.LotNumber))
                            continue;

                        if (string.IsNullOrWhiteSpace(parentLotNumber))
                            continue;

                        childNode.StartDate = null;
                        childNode.StartDateLabel = null;
                        childNode.EndDate = null;
                        childNode.ManufacturingProcessName = null;
                        childNode.ManufacturingTankName = null;
                        childNode.Weight = null;
                        childNode.Depth = 0;
                        childNode.NodeType = "Start";
                        childNode.ParentKey = null;
                        childNode.ParentMasterKey = null;
                        childNode.RouteSystem = "A";
                        childNode.InputSourceType = sourceType;
                        childNode.InputSlotNo = null;
                        childNode.IsTraceTerminal = string.IsNullOrWhiteSpace(childNode.LotNumber);

                        var parentNode = new ProductionResultNode();
                        parentNode.ProductionOrderNumber = null;
                        parentNode.LotNumber = parentLotNumber;
                        parentNode.ItemName = null;
                        parentNode.ItemCode = parentItemCode;
                        parentNode.StartDate = null;
                        parentNode.StartDateLabel = null;
                        parentNode.EndDate = null;
                        parentNode.ManufacturingProcessName = null;
                        parentNode.ManufacturingTankName = null;
                        parentNode.Weight = parentWeight;
                        parentNode.ControlMasterKey = BuildDrumcanSpecialNodeMasterKey(
                            parentNode.ItemCode,
                            parentNode.LotNumber,
                            parentNode.Weight,
                            slotNo);
                        parentNode.Depth = 0;
                        parentNode.NodeType = "Middle";
                        parentNode.ParentKey = null;
                        parentNode.ParentMasterKey = null;
                        parentNode.RouteSystem = "A";
                        parentNode.InputSourceType = sourceType;
                        parentNode.InputSlotNo = slotNo;
                        parentNode.IsTraceTerminal = string.IsNullOrWhiteSpace(parentNode.LotNumber);

                        string dedupeKey =
                            (childNode.ControlMasterKey ?? childNode.LotNumber ?? string.Empty)
                            + "|START-D|"
                            + (parentNode.ControlMasterKey ?? parentNode.LotNumber ?? string.Empty);

                        if (!seenKeys.Add(dedupeKey))
                            continue;

                        result.Add(new BackwardParentCandidate
                        {
                            Node = parentNode,
                            ChildNode = childNode,
                            ParentLotNumber = parentNode.LotNumber,
                            RelationKey = null,
                            DebugSource = "START-D"
                        });
                    }
                }
            }

            return result;
        }

        private string BuildStartNodesFromMaterialTableADrumcanSql(
    TraceSearchParameters p,
    SqlCommand cmd)
        {
            var sql = new StringBuilder();
            bool first = true;

            bool hasOrder = p != null && !string.IsNullOrWhiteSpace(p.ProductionOrderNumber);
            bool hasLot = p != null && !string.IsNullOrWhiteSpace(p.LotNumber);
            bool hasItemCode = p != null && !string.IsNullOrWhiteSpace(p.ItemCode);

            if (hasOrder)
            {
                cmd.Parameters.AddWithValue("@Order", "%" + p.ProductionOrderNumber + "%");
            }

            if (hasLot)
            {
                cmd.Parameters.AddWithValue("@Lot", "%" + p.LotNumber + "%");
            }

            if (hasItemCode)
            {
                cmd.Parameters.AddWithValue("@ItemCode", "%" + p.ItemCode + "%");
            }

            for (int i = 1; i <= 5; i++)
            {
                string idx = i.ToString("00");

                if (!first)
                {
                    sql.AppendLine("UNION ALL");
                }

                sql.AppendLine("SELECT");
                sql.AppendLine("    ma.ForeignKey,                                 -- 0  ChildOrder");
                sql.AppendLine("    ma.LotNumber,                                  -- 1  ChildLot");
                sql.AppendLine("    ma.ItemCode,                                   -- 2  ChildItemCode");
                sql.AppendLine("    ma.MasterKey,                                  -- 3  ChildMasterKey");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS ChildWeight, -- 4");
                sql.AppendLine("    'Drumcan' AS SourceType,                       -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo,                       -- 6");
                sql.AppendLine("    ma.DrumcanLotNumber" + idx + " AS ParentLotNumber, -- 7");
                sql.AppendLine("    ma.DrumcanItemCode" + idx + " AS ParentItemCode,   -- 8");
                sql.AppendLine("    ma.DrumcanLoadingAmount" + idx + " AS ParentWeight -- 9");
                sql.AppendLine("FROM dbo.MaterialTableA ma");
                sql.AppendLine("WHERE 1 = 1");
                sql.AppendLine("  AND ma.DrumcanLoadingAmount" + idx + " IS NOT NULL");
                sql.AppendLine("  AND ma.DrumcanLoadingAmount" + idx + " <> 0");
                sql.AppendLine("  AND ma.DrumcanLotNumber" + idx + " IS NOT NULL");
                sql.AppendLine("  AND LTRIM(RTRIM(ma.DrumcanLotNumber" + idx + ")) <> ''");

                if (hasOrder)
                {
                    sql.AppendLine("  AND ma.ForeignKey LIKE @Order");
                }

                if (hasLot)
                {
                    sql.AppendLine("  AND ma.LotNumber LIKE @Lot");
                }

                if (hasItemCode)
                {
                    sql.AppendLine("  AND ma.ItemCode LIKE @ItemCode");
                }

                first = false;
            }

            return sql.ToString();
        }

        private void AppendBackwardStartNodes(
            List<ProductionResultNode> target,
            HashSet<string> seenKeys,
            List<ProductionResultNode> source,
            string routeSystem)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (seenKeys == null)
                throw new ArgumentNullException("seenKeys");

            if (source == null || source.Count == 0)
                return;

            foreach (var node in source)
            {
                if (node == null)
                    continue;

                ApplyBackwardStartNodeContext(node, routeSystem);

                string dedupeKey = BuildBackwardStartDedupKey(node);
                if (string.IsNullOrWhiteSpace(dedupeKey))
                {
                    target.Add(node);
                    continue;
                }

                if (seenKeys.Add(dedupeKey))
                {
                    target.Add(node);
                }
            }
        }

        private void ApplyBackwardStartNodeContext(
            ProductionResultNode node,
            string routeSystem)
        {
            if (node == null)
                return;

            node.Depth = 0;
            node.NodeType = "Start";
            node.ParentKey = null;

            if (string.IsNullOrWhiteSpace(node.RouteSystem))
                node.RouteSystem = routeSystem;

            if (string.Equals(routeSystem, "A", StringComparison.OrdinalIgnoreCase))
            {
                if (!node.InputSlotNo.HasValue)
                    node.InputSlotNo = 0;

                if (string.IsNullOrWhiteSpace(node.InputSourceType))
                    node.InputSourceType = null;
            }
            else
            {
                if (!node.InputSlotNo.HasValue)
                    node.InputSlotNo = 1;

                if (string.IsNullOrWhiteSpace(node.InputSourceType))
                    node.InputSourceType = null;
            }

            if (!node.IsTraceTerminal)
                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);
        }

        private string BuildBackwardStartDedupKey(ProductionResultNode node)
        {
            if (node == null)
                return null;

            string routeSystem = string.IsNullOrWhiteSpace(node.RouteSystem)
                ? string.Empty
                : node.RouteSystem.Trim().ToUpperInvariant();

            string masterKey = string.IsNullOrWhiteSpace(node.ControlMasterKey)
                ? string.Empty
                : node.ControlMasterKey.Trim().ToUpperInvariant();

            string lotNumber = string.IsNullOrWhiteSpace(node.LotNumber)
                ? string.Empty
                : node.LotNumber.Trim().ToUpperInvariant();

            string inputSourceType = string.IsNullOrWhiteSpace(node.InputSourceType)
                ? string.Empty
                : node.InputSourceType.Trim().ToUpperInvariant();

            string inputSlotNo = node.InputSlotNo.HasValue
                ? node.InputSlotNo.Value.ToString()
                : string.Empty;

            return string.Join("|",
                "BACKWARD_START",
                routeSystem,
                masterKey,
                lotNumber,
                inputSourceType,
                inputSlotNo);
        }

        private void ApplyBackwardStartNodeContext(ProductionResultNode node)
        {
            if (node == null)
                return;

            node.Depth = 0;
            node.NodeType = "Start";
            node.ParentKey = null;

            // バック始点は現時点では B起点運用
            node.RouteSystem = "B";

            // B系統は現行モデル上、入力情報を固定で持たせる
            node.InputSlotNo = 1;
            node.InputSourceType = null;

            if (!node.IsTraceTerminal)
                node.IsTraceTerminal = string.IsNullOrWhiteSpace(node.LotNumber);
        }

        private string BuildBackwardStartMergeKey(ProductionResultNode node)
        {
            if (node == null)
                return null;

            // サービス側 MergeKey 方針に寄せるため、
            // B始点は MasterKey 優先、無ければ LotNumber を使用
            if (!string.IsNullOrWhiteSpace(node.ControlMasterKey))
                return "BSTART|" + node.ControlMasterKey.Trim().ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(node.LotNumber))
                return "BSTARTLOT|" + node.LotNumber.Trim().ToUpperInvariant();

            return null;
        }

        private string BuildDeterministicSpecialNodeMasterKey(
    string specialNodeType,
    string parentBaseKey,
    string itemCode,
    string lotNumber,
    float? weight,
    string inputSourceType,
    int? inputSlotNo,
    params string[] extraContexts)
        {
            string normalizedWeight = weight.HasValue
                ? weight.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                : "";

            var parts = new List<string>();

            parts.Add("SPNODE");
            parts.Add(NormalizeKeyPart(specialNodeType));
            parts.Add(NormalizeKeyPart(parentBaseKey));
            parts.Add(NormalizeKeyPart(itemCode));
            parts.Add(NormalizeKeyPart(lotNumber));
            parts.Add(normalizedWeight);
            parts.Add(NormalizeKeyPart(inputSourceType));
            parts.Add(inputSlotNo.HasValue ? inputSlotNo.Value.ToString() : "");

            if (extraContexts != null)
            {
                foreach (var context in extraContexts)
                {
                    parts.Add(NormalizeKeyPart(context));
                }
            }

            string raw = string.Join("|", parts);

            return "SPC_" + ComputeSha1Hex(raw);
        }

        private string BuildDrumcanSpecialNodeMasterKey(
    string itemCode,
    string lotNumber,
    float? weight,
    int? inputSlotNo)
        {
            string normalizedWeight = weight.HasValue
                ? weight.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                : "";

            return string.Join("|",
                "SPNODE",
                "DRUMCAN",
                NormalizeKeyPart(itemCode),
                NormalizeKeyPart(lotNumber),
                normalizedWeight,
                inputSlotNo.HasValue ? inputSlotNo.Value.ToString() : "");
        }

        private string NormalizeKeyPart(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? ""
                : value.Trim().ToUpperInvariant();
        }

        private string ComputeSha1Hex(string text)
        {
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text ?? "");
                var hash = sha1.ComputeHash(bytes);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Node補完用共通メソッド（サービスから移管）

        private BackwardParentCandidate FinalizeBackwardParentCandidate(
    BackwardParentCandidate candidate)
        {
            if (candidate == null)
                return null;

            if (candidate.Node == null || candidate.ChildNode == null)
                return null;

            candidate.RelationKey = BuildBackwardRelationKey(candidate);

            if (string.IsNullOrWhiteSpace(candidate.RelationKey))
                return null;

            return candidate;
        }

        private string BuildBackwardRelationKey(BackwardParentCandidate candidate)
        {
            if (candidate == null)
                return string.Empty;

            if (candidate.Node == null || candidate.ChildNode == null)
                return string.Empty;

            string childLotNumber = string.IsNullOrWhiteSpace(candidate.ChildNode.LotNumber)
                ? string.Empty
                : candidate.ChildNode.LotNumber.Trim().ToUpperInvariant();

            string parentMasterKey = string.IsNullOrWhiteSpace(candidate.Node.ControlMasterKey)
                ? string.Empty
                : candidate.Node.ControlMasterKey.Trim();

            if (string.IsNullOrWhiteSpace(childLotNumber))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(parentMasterKey))
                return string.Empty;

            return "R|B|" + childLotNumber + "|" + parentMasterKey;
        }

        #endregion

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



        


        #endregion
    }
}
