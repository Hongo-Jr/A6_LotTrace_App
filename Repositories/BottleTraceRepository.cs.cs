
using LotTraceApp.Forms;
using LotTraceApp.Models;
using LotTraceApp.Services;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace LotTraceApp.Repositories
{
    /// <summary>
    /// 瓶設備（MES33）用 DB アクセス
    /// </summary>
    public class BottleTraceRepository
    {
        private readonly string _connectionString;

        private LotTraceRepository _repo;


        public BottleTraceRepository(string connectionString, LotTraceRepository repo)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (repo == null)
                throw new ArgumentNullException("repo");

            _connectionString = connectionString;
            _repo = repo;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region トレースフォワード 液→瓶

        //削除
        public List<BottleCandidate> B_FindForwardCandidate(TraceSearchParameters p)
        {
            var result = new List<BottleCandidate>();
            //var starts = new List<ProductionResultNode>();

            //var startA = B_GetStartNodesFromA(p);
            //var startB = B_GetStartNodesFromB(p);
            
            //if(startB != null && startB.Count != 0)
            //{
            //    starts.AddRange(startB);
            //}

            //if (startA != null && startA.Count != 0)
            //{
            //    starts.AddRange(startA);
            //}

            //if (starts != null && starts.Count != 0)
            //{
            //    result = B_GetForwardBottleCandidate(starts);
            //}
            
            return result;
        }

        //新規
        public List<Bottle_ProductionResultNode> FindBottleNodeForward(TraceSearchParameters p)
        {
            var result = new List<Bottle_ProductionResultNode>();
            var starts = new List<ProductionResultNode>();

            var startA = B_GetStartNodesFromA(p);
            var startB = B_GetStartNodesFromB(p);

            if (startA != null && startA.Count != 0)
            {
                starts.AddRange(startA);
            }

            if (startB != null && startB.Count != 0)
            {
                starts.AddRange(startB);
            }

            foreach (var node in starts.GroupBy(x => x.LotNumber))
            {
                result.AddRange(GetBottleNodesForward(node.Key));
                result.AddRange(GetDrumNodesForward(node.Key));
            }

            return result;
        }

        public List<ProductionResultNode> B_GetStartNodesFromA(TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();
            if(string.IsNullOrWhiteSpace(p.ProductionOrderNumber) && string.IsNullOrWhiteSpace(p.LotNumber) && string.IsNullOrWhiteSpace(p.ItemName) && string.IsNullOrWhiteSpace(p.ItemCode))
            {
                return result;
            }
            

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = B_BuildForwardStartA_SQL(p, cmd);
                var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                
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
                        node.Weight = reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));
                        node.ControlMasterKey = reader.IsDBNull(3) ? null : reader.GetString(3);
                        node.RouteSystem = reader.IsDBNull(5) ? null : reader.GetString(5);
                        string? SlotNoText = reader.IsDBNull(6) ? null : reader.GetString(6);
                        int slotNo = 0;
                        int.TryParse(SlotNoText, out slotNo);
                        node.InputSlotNo = slotNo;
                            
                        node.Depth = 0;
                        node.NodeType = "Start";
                        node.ParentKey = null;
                        node.StartDateLabel = "手投入";
                            
                        string masterKey = node.ControlMasterKey ?? "";
                        string CheckKey = string.Join("|", masterKey, SlotNoText);

                        if (uniqueKeys.Add(CheckKey))
                        {
                            result.Add(node);
                        }
                    }
                   
                }
            }
            return result;
        }

        public string B_BuildForwardStartA_SQL(TraceSearchParameters p, SqlCommand cmd)
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
                sql.AppendLine("    ma.ForeignKey,                                -- 0");
                sql.AppendLine("    ma.LotNumber,                                 -- 1");
                sql.AppendLine("    ma.ItemCode,                                  -- 2");
                sql.AppendLine("    ma.MasterKey,                                 -- 3");
                sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS LoadingAmount, -- 4");
                sql.AppendLine("    'ManualInput' AS SourceType,                  -- 5");
                sql.AppendLine("    '" + idx + "' AS SlotNo                       -- 6");
                sql.AppendLine("FROM MES31.dbo.MaterialTableA ma");
                sql.AppendLine("WHERE 1 = 1");
                sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " IS NOT NULL");
                sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " <> 0");
                sql.AppendLine("AND SUBSTRING(ma.MasterKey,LEN(ma.MasterKey) - CHARINDEX('_', ma.MasterKey),1) IN('G', 'G')");

                B_AppendStartNodeSearchConditions(p, cmd, sql, "ma", false);

                //B_AppendSearchParameterCondition(p == null ? null : p.ProductionOrderNumber, cmd, sql, "ma", "ForeignKey", "@Order");
                //B_AppendSearchParameterCondition(p == null ? null : p.LotNumber, cmd, sql, "ma", "LotNumber", "@Lot");
                //B_AppendSearchParameterCondition(p == null ? null : p.ItemCode, cmd, sql, "ma", "ItemCode", "@ItemCode");

                first = false;
            }
            return sql.ToString();
        }

        public List<ProductionResultNode> B_GetStartNodesFromB(TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = B_BuildForwardStartB_SQL(p, cmd);
                var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new ProductionResultNode();

                        node.ProductionOrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                        node.LotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
                        node.ItemName = null;
                        node.ItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
                        node.StartDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                        node.EndDate = null;
                        node.ManufacturingProcessName = null;
                        node.ManufacturingTankName = null;
                        node.Weight = reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));
                        node.ControlMasterKey = reader.IsDBNull(5) ? null : reader.GetString(5);
                        node.RouteSystem = "B";
                        node.InputSlotNo = 0;

                        node.Depth = 0;
                        node.NodeType = "Start";
                        node.ParentKey = null;


                        string masterKey = node.ControlMasterKey ?? "";
                        string CheckKey = string.Join("|", masterKey, "0");

                        if (uniqueKeys.Add(CheckKey))
                        {
                            result.Add(node);
                        }
                    }

                }
            }

            return result;
        }

        public string B_BuildForwardStartB_SQL(TraceSearchParameters p, SqlCommand cmd)
        {
            var sql = new StringBuilder();
            
            sql.AppendLine("SELECT");
            sql.AppendLine("    scp.ForeignKey,                                -- 0");
            sql.AppendLine("    scp.LotNumber,                                 -- 1");
            sql.AppendLine("    scp.ItemCode,                                  -- 2");
            sql.AppendLine("    scp.StartDate,                                 -- 3");
            sql.AppendLine("    scp.Weight,                                    -- 4");
            sql.AppendLine("    scp.MasterKey                                  -- 5");
            
            sql.AppendLine("FROM MES31.dbo.SingleControlProcessTable scp");
            sql.AppendLine("WHERE 1 = 1");
            sql.AppendLine("AND SUBSTRING(scp.MasterKey,LEN(scp.MasterKey) - CHARINDEX('_', scp.MasterKey),1)='G'");
            sql.AppendLine("AND SUBSTRING(scp.MasterKey, LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)) - 2,1)='2'");
            sql.AppendLine("AND SUBSTRING(scp.MasterKey, LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),1) IN ('4','7')");


            B_AppendStartNodeSearchConditions(p, cmd, sql, "scp",true);


            return sql.ToString();
        }

        private void B_AppendStartNodeSearchConditions(TraceSearchParameters p, SqlCommand cmd, StringBuilder sql, string alias, bool includeStartDate)
        {
            B_AppendSearchParameterCondition(p.ProductionOrderNumber, cmd, sql, alias, "ForeignKey", "@Order");
           
            
            B_AppendSearchParameterCondition(p.LotNumber, cmd, sql, alias, "LotNumber", "@Lot");

            B_AppendItemCodeCondition(p, cmd, sql, alias, "ItemCode", "@ItemCode");

            if (!includeStartDate || p == null)
                return;


            if (p != null && p.From.HasValue)
            {
                sql.AppendLine("  AND " + alias + ".StartDate >= @From");
                if (!cmd.Parameters.Contains("@From"))
                    cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = p.From.Value;
            }

            if (p != null && p.To.HasValue)
            {
                sql.AppendLine("  AND " + alias + ".StartDate <= @To");
                if (!cmd.Parameters.Contains("@To"))
                    cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = p.To.Value;
            }
        }

        //削除
        //private void B_BottleAppendStartNodeSearchConditions(TraceSearchParameters p, SqlCommand cmd, StringBuilder sql, string alias, bool includeStartDate)
        //{
        //    B_AppendSearchParameterCondition(p.ProductionOrderNumber, cmd, sql, alias, "OrderNumber", "@Order");


        //    B_AppendSearchParameterCondition(p.LotNumber, cmd, sql, alias, "ProductLotNumber", "@Lot");

        //    B_AppendItemCodeCondition(p, cmd, sql, alias, "ProductItemCode", "@ItemCode");

        //    if (!includeStartDate || p == null)
        //        return;


        //    if (p != null && p.From.HasValue)
        //    {
        //        sql.AppendLine("  AND " + alias + ".FillingStartDate >= @From");
        //        if (!cmd.Parameters.Contains("@From"))
        //            cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = p.From.Value;
        //    }

        //    if (p != null && p.To.HasValue)
        //    {
        //        sql.AppendLine("  AND " + alias + ".FillingStartDate <= @To");
        //        if (!cmd.Parameters.Contains("@To"))
        //            cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = p.To.Value;
        //    }
        //}

        //新規
        private void AppendBottleNodeSearchConditions(TraceSearchParameters p, SqlCommand cmd, StringBuilder sql, string orderAlias, string productAlias, bool includeStartDate)
        {
            B_AppendSearchParameterCondition(p.ProductionOrderNumber, cmd, sql, orderAlias, "OrderNumber", "@Order");

            B_AppendSearchParameterCondition(p.LotNumber, cmd, sql, productAlias, "ProductLotNumber", "@Lot");

            B_AppendItemCodeCondition(p, cmd, sql, productAlias, "ProductItemCode", "@ItemCode");

            if (!includeStartDate || p == null)
                return;

            if (p != null && p.From.HasValue)
            {
                sql.AppendLine("  AND " + orderAlias + ".StartDate >= @From");
                if (!cmd.Parameters.Contains("@From"))
                    cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = p.From.Value;
            }

            if (p != null && p.To.HasValue)
            {
                sql.AppendLine("  AND " + orderAlias + ".StartDate <= @To");
                if (!cmd.Parameters.Contains("@To"))
                    cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = p.To.Value;
            }
        }

        public void B_AppendSearchParameterCondition(
            string? rawValue,
            SqlCommand cmd,
            StringBuilder sql,
            string alias,
            string columnName,
            string parameterName)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return;

            bool useLike = _repo.ContainsUserWildcard(rawValue);
            sql.AppendLine( " AND " + alias + "." + columnName + (useLike ? " LIKE " : " = ") + parameterName);
            if (useLike)
                sql.AppendLine(" ESCAPE '\\'");

            if (!cmd.Parameters.Contains(parameterName))
            {
                string value = useLike
                    ? _repo.BuildSqlLikePatternFromUserWildcard(rawValue)
                    : rawValue.Trim();

                cmd.Parameters.AddWithValue(parameterName, value);
            }
        }

        private void B_AppendInCondition( List<string> values, SqlCommand cmd, StringBuilder sql, string alias, string columnName, string parameterName)
        {
            if (values == null || values.Count == 0)
                return;

            var parameterNames = new List<string>();

            for (int i = 0; i < values.Count; i++)
            {
                string value = values[i];

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                string currentParameterName = parameterName + i.ToString();

                parameterNames.Add(currentParameterName);

                if (!cmd.Parameters.Contains(currentParameterName))
                {
                    cmd.Parameters.AddWithValue(
                        currentParameterName,
                        value.Trim());
                }
            }

            if (parameterNames.Count == 0)
                return;

            sql.AppendLine(
                "  AND " + alias + "." + columnName +
                " IN (" + string.Join(", ", parameterNames) + ")");
        }

        private void B_AppendItemCodeCondition(TraceSearchParameters p, SqlCommand cmd, StringBuilder sql, string alias, string columnName, string parameterName)
        {
            if (p == null)
                return;

            if (p.ResolvedItemCodes != null && p.ResolvedItemCodes.Count > 0)
            {
                B_AppendInCondition(p.ResolvedItemCodes, cmd, sql, alias, columnName, parameterName);
                return;
            }

            B_AppendSearchParameterCondition(p.ItemCode, cmd, sql, alias, columnName, parameterName);
        }

        //削除
        //private List<BottleCandidate> B_GetForwardBottleCandidate(List<ProductionResultNode> nodes)
        //{
        //    var result = new List<BottleCandidate>();
            

        //    foreach (var group in nodes.GroupBy(x=> x.LotNumber))
        //    {

        //        var liquidNodes = group.ToList();

                
                

        //        var bottleNodes = B_FindForwardBottleNodes(group.Key);
        //        var DrumNodes = B_FindForwardDrumNodes(group.Key);

        //        var fillNodes = new List<Bottle_ProductionResultNode>();
        //        if( bottleNodes != null && bottleNodes.Count != 0)
        //        {
        //            fillNodes.AddRange(bottleNodes);
        //        }

        //        if (DrumNodes != null && DrumNodes.Count != 0)
        //        {
        //            fillNodes.AddRange(DrumNodes);
        //        }
        //        if(fillNodes.Count > 0)
        //        {
        //            result.Add(B_BuildCandidate(liquidNodes, fillNodes));
        //        }
                
        //    }

        //    return result;
        //}

        //削除
        //private List<Bottle_ProductionResultNode> B_FindForwardBottleNodes(string? midLot)
        //{
        //    var result = new List<Bottle_ProductionResultNode>();

        //    if (midLot == null)
        //    {
        //        return result;
        //    }                

        //    using (var conn = CreateConnection())
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();

        //        cmd.CommandText = B_BuildForwardBottleNodeSQL();
        //        cmd.Parameters.AddWithValue("@lotNo", midLot);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var BottleNode = new Bottle_ProductionResultNode();

        //                BottleNode.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
        //                BottleNode.ProductLotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                BottleNode.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
        //                BottleNode.FillingBottleNum_OK = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
        //                BottleNode.FillingBottleNum_NG = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
        //                BottleNode.StartDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
        //                BottleNode.EndDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);

        //                result.Add(BottleNode);

        //            } 
        //        }
        //        return result;
        //    }
        //}

        //新規
        private List<Bottle_ProductionResultNode> GetBottleNodesForward(string? midLot)
        {
            var result = new List<Bottle_ProductionResultNode>();

            if (midLot == null)
            {
                return result;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildBottleForwardSQL();
                cmd.Parameters.AddWithValue("@lotNo", midLot);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var BottleNode = new Bottle_ProductionResultNode();

                        BottleNode.MasterKey = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        BottleNode.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        BottleNode.OrderNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                        BottleNode.StartDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3);
                        BottleNode.EndDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                        BottleNode.FillingBottleNum_OK = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                        BottleNode.FillingBottleNum_NG = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                        BottleNode.ProductItemCode = reader.IsDBNull(7) ? null : reader.GetString(7);
                        BottleNode.ProductLotNumber = reader.IsDBNull(8) ? null : reader.GetString(8);
                        BottleNode.MiddleProductItemCode = reader.IsDBNull(9) ? null : reader.GetString(9);
                        BottleNode.MiddleProductLotNumber = reader.IsDBNull(10) ? null : reader.GetString(10);

                        result.Add(BottleNode);

                    }
                }
                return result;
            }
        }

        //削除
        //private string B_BuildForwardBottleNodeSQL()
        //{
        //    var sql = new StringBuilder();

        //    sql.AppendLine("SELECT fo.OrderNumber,fb.ProductLotNumber,fb.ProductItemCode,fo.FillingBottleNumberResult_OK,fo.FillingBottleNumberResult_NG,fo.StartDate,fo.EndDate");
        //    sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
        //    sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode");
        //    sql.AppendLine(" FROM FillingBottleTable WHERE MiddleProductLotNumber = @lotNo) fb");
        //    sql.AppendLine(" ON fb.OrderNumber = fo.OrderNumber;");
            
        //    return sql.ToString();
        //}

        //新規
        private string BuildBottleForwardSQL()
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT");
            sql.AppendLine("    fo.MasterKey,                                     -- 0");
            sql.AppendLine("    fo.ProcessType,                                  -- 1");
            sql.AppendLine("    fo.OrderNumber,                                 -- 2");
            sql.AppendLine("    fo.StartDate,                                       -- 3");
            sql.AppendLine("    fo.EndDate,                                        -- 4");
            sql.AppendLine("    fo.FillingBottleNumberResult_OK,       -- 5");
            sql.AppendLine("    fo.FillingBottleNumberResult_NG,       -- 6");

            sql.AppendLine("    fb.ProductItemCode,                          -- 7");
            sql.AppendLine("    fb.ProductLotNumber,                         -- 8");
            sql.AppendLine("    fb.MiddleProductItemCode,                -- 9");
            sql.AppendLine("    fb.MiddleProductLotNumber               -- 10");
    
            sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
            sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode");
            sql.AppendLine(" FROM FillingBottleTable WHERE MiddleProductLotNumber = @lotNo) fb");
            sql.AppendLine(" ON fb.OrderNumber = fo.OrderNumber;");

            return sql.ToString();
        }

        //削除
        //private List<Bottle_ProductionResultNode> B_FindForwardDrumNodes(string? midLot)
        //{
        //    var result = new List<Bottle_ProductionResultNode>();

        //    if (midLot == null)
        //    {
        //        return result;
        //    }   

        //    using (var conn = CreateConnection())
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();

        //        cmd.CommandText = B_BuildForwardDrumNodeSQL();
        //        cmd.Parameters.AddWithValue("@lotNo", midLot);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var BottleNode = new Bottle_ProductionResultNode();

        //                BottleNode.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
        //                BottleNode.ProductLotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                BottleNode.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
        //                BottleNode.FillingBottleNum_OK = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
        //                BottleNode.FillingBottleNum_NG = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
        //                BottleNode.StartDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
        //                BottleNode.EndDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);

        //                result.Add(BottleNode);

        //            }
        //        }
        //        return result;
        //    }
        //}

        //新規
        private List<Bottle_ProductionResultNode> GetDrumNodesForward(string? midLot)
        {
            var result = new List<Bottle_ProductionResultNode>();

            if (midLot == null)
            {
                return result;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildDrumForwardSQL();
                cmd.Parameters.AddWithValue("@lotNo", midLot);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var BottleNode = new Bottle_ProductionResultNode();

                        BottleNode.MasterKey = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        BottleNode.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        BottleNode.OrderNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                        BottleNode.StartDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3);
                        BottleNode.EndDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                        BottleNode.FillingBottleNum_OK = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                        BottleNode.FillingBottleNum_NG = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                        BottleNode.ProductItemCode = reader.IsDBNull(7) ? null : reader.GetString(7);
                        BottleNode.ProductLotNumber = reader.IsDBNull(8) ? null : reader.GetString(8);
                        BottleNode.MiddleProductItemCode = reader.IsDBNull(9) ? null : reader.GetString(9);
                        BottleNode.MiddleProductLotNumber = reader.IsDBNull(10) ? null : reader.GetString(10);

                        result.Add(BottleNode);

                    }
                }
                return result;
            }
        }

        //削除
        //private string B_BuildForwardDrumNodeSQL()
        //{
        //    var sql = new StringBuilder();

        //    sql.AppendLine("SELECT fo.OrderNumber,fd.ProductLotNumber,fd.ProductItemCode,fo.FillingBottleNumberResult_OK,fo.FillingBottleNumberResult_NG,fo.StartDate,fo.EndDate");
        //    sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
        //    sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode");
        //    sql.AppendLine(" FROM FillingDrumcanTable WHERE MiddleProductLotNumber = @lotNo) fd");
        //    sql.AppendLine(" ON fd.OrderNumber = fo.OrderNumber;");

        //    return sql.ToString();
        //}

        //新規
        private string BuildDrumForwardSQL()
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT");
            sql.AppendLine("    fo.MasterKey,                                     -- 0");
            sql.AppendLine("    fo.ProcessType,                                  -- 1");
            sql.AppendLine("    fo.OrderNumber,                                 -- 2");
            sql.AppendLine("    fo.StartDate,                                       -- 3");
            sql.AppendLine("    fo.EndDate,                                        -- 4");
            sql.AppendLine("    fo.FillingBottleNumberResult_OK,       -- 5");
            sql.AppendLine("    fo.FillingBottleNumberResult_NG,       -- 6");

            sql.AppendLine("    fd.ProductItemCode,                          -- 7");
            sql.AppendLine("    fd.ProductLotNumber,                        -- 8");
            sql.AppendLine("    fd.MiddleProductItemCode,                -- 9");
            sql.AppendLine("    fd.MiddleProductLotNumber              -- 10");

            sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
            sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode");
            sql.AppendLine(" FROM FillingDrumcanTable WHERE MiddleProductLotNumber = @lotNo) fd");
            sql.AppendLine(" ON fd.OrderNumber = fo.OrderNumber;");

            return sql.ToString();
        }

        //削除
        //private BottleCandidate B_BuildCandidate(List<ProductionResultNode> liquidNodes, List<Bottle_ProductionResultNode> bottleNodes)
        //{
        //    var result = new BottleCandidate();

        //    result.LiquidNodes.AddRange(liquidNodes);

        //    if (bottleNodes != null)
        //    {
        //        result.BottleNodes.AddRange(bottleNodes);
        //    }

        //    return result;
        //}


        #endregion

        #region トレースバック 瓶→液

        public List<BottleCandidate> B_FindBackwardCandidate(TraceSearchParameters p)
        {
            var result = new List<BottleCandidate>();
            //var starts = new List<Bottle_ProductionResultNode>();

            //var start_Bottle = B_FindBackwardStartBottleNodes(p);
            //var start_Drum = B_FindBackwardStartDrumNodes(p);

            //if (start_Bottle != null && start_Bottle.Count != 0)
            //{
            //    starts.AddRange(start_Bottle);
            //}

            //if (start_Drum != null && start_Drum.Count != 0)
            //{
            //    starts.AddRange(start_Drum);
            //}

            //if (starts != null && starts.Count != 0)
            //{
            //    result = B_GetBackwardBottleCandidate(starts);
            //}


            return result;
        }

        //private List<Bottle_ProductionResultNode> B_FindBackwardStartBottleNodes(TraceSearchParameters p)
        //{
        //    var result = new List<Bottle_ProductionResultNode>();

        //    using (var conn = CreateConnection())
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();

        //        cmd.CommandText = B_BuildBackwardStartBottle_SQL(p,cmd);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var BottleNode = new Bottle_ProductionResultNode();

        //                BottleNode.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
        //                BottleNode.ProductLotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                BottleNode.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
        //                BottleNode.FillingBottleNum_OK = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
        //                BottleNode.FillingBottleNum_NG = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
        //                BottleNode.StartDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
        //                BottleNode.EndDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
        //                BottleNode.MiddleProductLotNumber = reader.IsDBNull(7) ? null : reader.GetString(7);
        //                result.Add(BottleNode);

        //            }
        //        }
        //        return result;
        //    }
        //}

        //public string B_BuildBackwardStartBottle_SQL(TraceSearchParameters p, SqlCommand cmd)
        //{
        //    var sql = new StringBuilder();

        //    sql.AppendLine("SELECT fo.OrderNumber,fb.ProductLotNumber,fb.ProductItemCode,fo.FillingBottleNumberResult_OK,fo.FillingBottleNumberResult_NG,fb.FillingStartDate, fb.FillingEndDate, fb.MiddleProductLotNumber");
        //    sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
        //    sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode,MiddleProductLotNumber,FillingStartDate,FillingEndDate");
        //    sql.AppendLine(" FROM FillingBottleTable) fb");
        //    sql.AppendLine(" ON fb.OrderNumber = fo.OrderNumber");
        //    sql.AppendLine("WHERE 1 = 1");

        //    B_BottleAppendStartNodeSearchConditions(p, cmd, sql, "fb", true);
        //    //B_AppendStartNodeSearchConditions_BottleTable(p, cmd, sql, "fb");

        //    return sql.ToString();
        //}


        //private List<Bottle_ProductionResultNode> B_FindBackwardStartDrumNodes(TraceSearchParameters p)
        //{
        //    var result = new List<Bottle_ProductionResultNode>();

        //    using (var conn = CreateConnection())
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();

        //        cmd.CommandText = B_BuildBackwardStartDrum_SQL(p, cmd);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var BottleNode = new Bottle_ProductionResultNode();

        //                BottleNode.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
        //                BottleNode.ProductLotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                BottleNode.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
        //                BottleNode.FillingBottleNum_OK = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
        //                BottleNode.FillingBottleNum_NG = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
        //                BottleNode.StartDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
        //                BottleNode.EndDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
        //                BottleNode.MiddleProductLotNumber = reader.IsDBNull(7) ? null : reader.GetString(7);
        //                result.Add(BottleNode);

        //            }
        //        }
        //        return result;
        //    }
        //}

        //public string B_BuildBackwardStartDrum_SQL(TraceSearchParameters p, SqlCommand cmd)
        //{
        //    var sql = new StringBuilder();

        //    sql.AppendLine("SELECT fo.OrderNumber,fd.ProductLotNumber,fd.ProductItemCode,fo.FillingBottleNumberResult_OK,fo.FillingBottleNumberResult_NG,fd.FillingStartDate,fd.FillingEndDate, fd.MiddleProductLotNumber");
        //    sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
        //    sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode, MiddleProductLotNumber,FillingStartDate,FillingEndDate");
        //    sql.AppendLine(" FROM FillingDrumcanTable) fd");
        //    sql.AppendLine(" ON fd.OrderNumber = fo.OrderNumber");
        //    sql.AppendLine("WHERE 1 = 1");

        //    B_BottleAppendStartNodeSearchConditions(p, cmd, sql, "fd", true);
        //    //B_AppendStartNodeSearchConditions_BottleTable(p, cmd, sql, "fd");

        //    return sql.ToString();
        //}

        #region 山本さん

        public List<Bottle_ProductionResultNode> FindBottleNodesBackward(TraceSearchParameters p)
        {
            var bottle = GetBottleNodesBackward(p);

            if(bottle != null && bottle.Count != 0)
            {
                return bottle;
            }

            var drum = GetDrumNodesBackward(p);

            if (drum != null && drum.Count != 0)
            {
                return drum;
            }

            return new List<Bottle_ProductionResultNode>();
        }

        /// <summary>
        /// このメソッドはトレースバックで検索条件に従ってList<Node>を作って返します
        /// </summary>
        /// <param name="p"></param>
        /// <returns>result</returns>
        /// 
        private List<Bottle_ProductionResultNode> GetBottleNodesBackward(TraceSearchParameters p)
        {
            var result = new List<Bottle_ProductionResultNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                //中身入れてね
                cmd.CommandText = BuildBottleBackwardSQL(p, cmd);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Nodeの中身入れてね
                        //AddしてListに追加してね
                        var node = new Bottle_ProductionResultNode();

                        node.MasterKey = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        node.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        node.OrderNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                        node.StartDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3);
                        node.EndDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                        node.FillingBottleNum_OK = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                        node.FillingBottleNum_NG = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                        node.ProductItemCode = reader.IsDBNull(7) ? null : reader.GetString(7);
                        node.ProductLotNumber = reader.IsDBNull(8) ? null : reader.GetString(8);
                        node.MiddleProductItemCode = reader.IsDBNull(9) ? null : reader.GetString(9);
                        node.MiddleProductLotNumber = reader.IsDBNull(10) ? null : reader.GetString(10);

                        result.Add(node);
                    }
                }
            }

            return result;
        }

        private string BuildBottleBackwardSQL(TraceSearchParameters p, SqlCommand cmd)
        {
            var sql = new StringBuilder();
            //中身入れてね
            sql.AppendLine("SELECT");
            sql.AppendLine("    fo.MasterKey,                                     -- 0");
            sql.AppendLine("    fo.ProcessType,                                  -- 1");
            sql.AppendLine("    fo.OrderNumber,                                 -- 2");
            sql.AppendLine("    fo.StartDate,                                       -- 3");
            sql.AppendLine("    fo.EndDate,                                        -- 4");
            sql.AppendLine("    fo.FillingBottleNumberResult_OK,       -- 5");
            sql.AppendLine("    fo.FillingBottleNumberResult_NG,       -- 6");

            sql.AppendLine("    fb.ProductItemCode,                          -- 7");
            sql.AppendLine("    fb.ProductLotNumber,                         -- 8");
            sql.AppendLine("    fb.MiddleProductItemCode,                -- 9");
            sql.AppendLine("    fb.MiddleProductLotNumber               -- 10");

            sql.AppendLine("FROM MES33.dbo.FillingOrderResultTable fo");
            sql.AppendLine("INNER JOIN MES33.dbo.FillingBottleTable fb");
            sql.AppendLine("ON fo.OrderNumber = fb.OrderNumber");
            sql.AppendLine("WHERE 1 = 1");

            AppendBottleNodeSearchConditions(p, cmd, sql, "fo", "fb",true);

            return sql.ToString();
        }

        private List<Bottle_ProductionResultNode> GetDrumNodesBackward(TraceSearchParameters p)
        {
            var result = new List<Bottle_ProductionResultNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                //中身入れてね
                cmd.CommandText = BuildDrumBackwardSQL(p, cmd);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Nodeの中身入れてね
                        //AddしてListに追加してね
                        var node = new Bottle_ProductionResultNode();

                        node.MasterKey = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        node.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        node.OrderNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                        node.StartDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3);
                        node.EndDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                        node.FillingBottleNum_OK = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                        node.FillingBottleNum_NG = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                        node.ProductItemCode = reader.IsDBNull(7) ? null : reader.GetString(7);
                        node.ProductLotNumber = reader.IsDBNull(8) ? null : reader.GetString(8);
                        node.MiddleProductItemCode = reader.IsDBNull(9) ? null : reader.GetString(9);
                        node.MiddleProductLotNumber = reader.IsDBNull(10) ? null : reader.GetString(10);

                        result.Add(node);
                    }
                }
            }
            return result;
        }

        private string BuildDrumBackwardSQL(TraceSearchParameters p, SqlCommand cmd)
        {
            var sql = new StringBuilder();
            //中身入れてね
            sql.AppendLine("SELECT");
            sql.AppendLine("    fo.MasterKey,                                     -- 0");
            sql.AppendLine("    fo.ProcessType,                                  -- 1");
            sql.AppendLine("    fo.OrderNumber,                                 -- 2");
            sql.AppendLine("    fo.StartDate,                                       -- 3");
            sql.AppendLine("    fo.EndDate,                                        -- 4");
            sql.AppendLine("    fo.FillingBottleNumberResult_OK,       -- 5");
            sql.AppendLine("    fo.FillingBottleNumberResult_NG,       -- 6");

            sql.AppendLine("    fd.ProductItemCode,                          -- 7");
            sql.AppendLine("    fd.ProductLotNumber,                        -- 8");
            sql.AppendLine("    fd.MiddleProductItemCode,                -- 9");
            sql.AppendLine("    fd.MiddleProductLotNumber              -- 10");

            sql.AppendLine("FROM MES33.dbo.FillingOrderResultTable fo");
            sql.AppendLine("INNER JOIN MES33.dbo.FillingDrumcanTable fd");
            sql.AppendLine("ON fo.OrderNumber = fd.OrderNumber");
            sql.AppendLine("where 1 = 1");

            AppendBottleNodeSearchConditions(p, cmd, sql, "fo", "fd", true);

            return sql.ToString();
        }


        #endregion

        //private List<BottleCandidate> B_GetBackwardBottleCandidate(List<Bottle_ProductionResultNode> nodes)
        //{
        //    var result = new List<BottleCandidate>();


        //    foreach (var group in nodes.GroupBy(x => x.MiddleProductLotNumber))
        //    {

        //        var bottleNodes = group.ToList();
        //        var liquidNodes = new List<ProductionResultNode>();

        //        var liquidB = B_GetBackwardNodesFromB(group.Key);
        //        var liquidA = B_GetBackwardNodesFromA(group.Key);
        //        if (liquidB != null && liquidB.Count != 0 )
        //        {
        //            liquidNodes.AddRange(liquidB);
        //        }

        //        if (liquidA != null && liquidA.Count != 0)
        //        {
        //            liquidNodes.AddRange(liquidA);
        //        }

        //        if(liquidNodes.Count > 0 )
        //        {
        //            result.Add(B_BuildCandidate(liquidNodes,bottleNodes));
        //        }
        //    }

        //    return result;
        //}

        //public List<ProductionResultNode> B_GetBackwardNodesFromA(string? midLot)
        //{
        //    var result = new List<ProductionResultNode>();

        //    if (midLot == null)
        //    {
        //        return result;
        //    }   

        //    using (var conn = CreateConnection())
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();

        //        cmd.CommandText = B_BuildBackwardliquidNodeA_SQL(cmd);
        //        cmd.Parameters.AddWithValue("@lotNo", midLot);
        //        var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var node = new ProductionResultNode();

        //                node.ProductionOrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
        //                node.LotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                node.ItemName = null;
        //                node.ItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
        //                node.StartDate = null;
        //                node.EndDate = null;
        //                node.ManufacturingProcessName = null;
        //                node.ManufacturingTankName = null;
        //                node.Weight = reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));
        //                node.ControlMasterKey = reader.IsDBNull(3) ? null : reader.GetString(3);
        //                node.RouteSystem = reader.IsDBNull(5) ? null : reader.GetString(5);
        //                string? SlotNoText = reader.IsDBNull(6) ? null : reader.GetString(6);
        //                int slotNo = 0;
        //                int.TryParse(SlotNoText, out slotNo);
        //                node.InputSlotNo = slotNo;

        //                node.Depth = 0;
        //                node.NodeType = "Start";
        //                node.ParentKey = null;
        //                node.StartDateLabel = "手投入";

        //                string masterKey = node.ControlMasterKey ?? "";
        //                string CheckKey = string.Join("|", masterKey, SlotNoText);

        //                if (uniqueKeys.Add(CheckKey))
        //                {
        //                    result.Add(node);
        //                }
        //            }

        //        }
        //    }
        //    return result;
        //}

        //public string B_BuildBackwardliquidNodeA_SQL(SqlCommand cmd)
        //{
        //    var sql = new StringBuilder();
        //    bool first = true;

        //    for (int i = 1; i <= 50; i++)
        //    {
        //        string idx = i.ToString("00");

        //        if (!first)
        //        {
        //            sql.AppendLine("UNION ALL");
        //        }

        //        sql.AppendLine("SELECT");
        //        sql.AppendLine("    ma.ForeignKey,                                -- 0");
        //        sql.AppendLine("    ma.LotNumber,                                 -- 1");
        //        sql.AppendLine("    ma.ItemCode,                                  -- 2");
        //        sql.AppendLine("    ma.MasterKey,                                 -- 3");
        //        sql.AppendLine("    ma.ManualInputLoadingAmount" + idx + " AS LoadingAmount, -- 4");
        //        sql.AppendLine("    'ManualInput' AS SourceType,                  -- 5");
        //        sql.AppendLine("    '" + idx + "' AS SlotNo                       -- 6");
        //        sql.AppendLine("FROM MES31.dbo.MaterialTableA ma");
        //        sql.AppendLine("WHERE 1 = 1");
        //        sql.AppendLine("  AND ma.LotNumber = @lotNo");
        //        sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " IS NOT NULL");
        //        sql.AppendLine("  AND ma.ManualInputLoadingAmount" + idx + " <> 0");
        //        sql.AppendLine("  AND SUBSTRING(ma.MasterKey,LEN(ma.MasterKey) - CHARINDEX('_', ma.MasterKey),1) IN('G', 'G')");

        //        first = false;
        //    }
        //    return sql.ToString();
        //}

        //public List<ProductionResultNode> B_GetBackwardNodesFromB(string? midLot)
        //{
        //    var result = new List<ProductionResultNode>();

        //    if (midLot == null)
        //    {
        //        return result;
        //    }

        //    using (var conn = CreateConnection())
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();

        //        cmd.CommandText = B_BuildBackwardliquidNodeB_SQL(cmd);
        //        cmd.Parameters.AddWithValue("@lotNo", midLot);
        //        var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var node = new ProductionResultNode();

        //                node.ProductionOrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
        //                node.LotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                node.ItemName = null;
        //                node.ItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
        //                node.StartDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
        //                node.EndDate = null;
        //                node.ManufacturingProcessName = null;
        //                node.ManufacturingTankName = null;
        //                node.Weight = reader.IsDBNull(4) ? (float?)null : Convert.ToSingle(reader.GetValue(4));
        //                node.ControlMasterKey = reader.IsDBNull(5) ? null : reader.GetString(5);
        //                node.RouteSystem = "B";
        //                node.InputSlotNo = 0;

        //                node.Depth = 0;
        //                node.NodeType = "End";
        //                node.ParentKey = null;


        //                string masterKey = node.ControlMasterKey ?? "";
        //                string CheckKey = string.Join("|", masterKey, "0");

        //                if (uniqueKeys.Add(CheckKey))
        //                {
        //                    result.Add(node);
        //                }
        //            }

        //        }
        //    }

        //    return result;
        //}

        //public string B_BuildBackwardliquidNodeB_SQL(SqlCommand cmd)
        //{
        //    var sql = new StringBuilder();

        //    sql.AppendLine("SELECT");
        //    sql.AppendLine("    scp.ForeignKey,                                -- 0");
        //    sql.AppendLine("    scp.LotNumber,                                 -- 1");
        //    sql.AppendLine("    scp.ItemCode,                                  -- 2");
        //    sql.AppendLine("    scp.StartDate,                                 -- 3");
        //    sql.AppendLine("    scp.Weight,                                    -- 4");
        //    sql.AppendLine("    scp.MasterKey                                  -- 5");

        //    sql.AppendLine("FROM MES31.dbo.SingleControlProcessTable scp");
        //    sql.AppendLine("WHERE 1 = 1");
        //    sql.AppendLine("AND scp.LotNumber = @lotNo");
        //    sql.AppendLine("AND SUBSTRING(scp.MasterKey,LEN(scp.MasterKey) - CHARINDEX('_', scp.MasterKey),1)='G'");
        //    sql.AppendLine("AND SUBSTRING(scp.MasterKey, LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)) - 2,1)='2'");
        //    sql.AppendLine("AND SUBSTRING(scp.MasterKey, LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),1) IN ('4','7')");

        //    return sql.ToString();
        //}

        #endregion




    }
}