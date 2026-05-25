using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using LotTraceApp.Forms;
using LotTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public List<BottleCandidate> B_FindForwardCandidate(TraceSearchParameters p)
        {
            var result = new List<BottleCandidate>();
            var starts = new List<ProductionResultNode>();

            var startA = B_GetStartNodesFromA(p);

            starts.AddRange(startA);



            result = B_GetForwardBottleCandidate(starts);


            return result;
        }

        public List<ProductionResultNode> B_GetStartNodesFromA(TraceSearchParameters p)
        {
            var result = new List<ProductionResultNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = B_BuildForwardStartA_SQL(p, cmd);
                var uniqueMasterKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                
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
                            node.Depth = 0;
                            node.NodeType = "Start";
                            node.ParentKey = null;

                            string masterKey = node.ControlMasterKey ?? "";

                            if (uniqueMasterKeys.Add(masterKey))
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


                B_AppendSearchParameterCondition(p == null ? null : p.ProductionOrderNumber, cmd, sql, "ma", "ForeignKey", "@Order");
                B_AppendSearchParameterCondition(p == null ? null : p.LotNumber, cmd, sql, "ma", "LotNumber", "@Lot");
                B_AppendSearchParameterCondition(p == null ? null : p.ItemCode, cmd, sql, "ma", "ItemCode", "@ItemCode");

                first = false;
            }
            return sql.ToString();
        }

        public void B_AppendSearchParameterCondition(
            string rawValue,
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

        

        private List<BottleCandidate> B_GetForwardBottleCandidate(List<ProductionResultNode> nodes)
        {
            var result = new List<BottleCandidate>();
            

            foreach (var group in nodes.GroupBy(x=> x.LotNumber))
            {

                var liquidNodes = group.ToList();
                var BottleNodes = B_FindForwardBottleNodes(group.Key);

                result.Add(B_BuildForwardCandidate(liquidNodes, BottleNodes));
            }

            return result;
        }

        private List<Bottle_ProductionResultNode> B_FindForwardBottleNodes(string midLot)
        {
            var result = new List<Bottle_ProductionResultNode>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = B_BuildForwardBottleNodeSQL();
                cmd.Parameters.AddWithValue("@lotNo", midLot);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var BottleNode = new Bottle_ProductionResultNode();

                        BottleNode.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                        BottleNode.ProductLotNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
                        BottleNode.ProductItemCode = reader.IsDBNull(2) ? null : reader.GetString(2);
                        BottleNode.FillingBottleNum_OK = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        BottleNode.FillingBottleNum_NG = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                        BottleNode.StartDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                        BottleNode.EndDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);

                        result.Add(BottleNode);

                    } 
                }
                return result;
            }
        }

        private string B_BuildForwardBottleNodeSQL()
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT fo.OrderNumber,fb.ProductLotNumber,fb.ProductItemCode,fo.FillingBottleNumberResult_OK,fo.FillingBottleNumberResult_NG,fo.StartDate,fo.EndDate");
            sql.AppendLine(" FROM [MES33].[dbo].[FillingOrderResultTable] fo");
            sql.AppendLine(" INNER JOIN ( SELECT DISTINCT OrderNumber,ProductLotNumber,ProductItemCode");
            sql.AppendLine(" FROM FillingBottleTable WHERE MiddleProductLotNumber = @lotNo) fb");
            sql.AppendLine(" ON fb.OrderNumber = fo.OrderNumber;");
            
            return sql.ToString();
        }

        private BottleCandidate B_BuildForwardCandidate(List<ProductionResultNode> liquidNodes, List<Bottle_ProductionResultNode> bottleNodes)
        {
            var result = new BottleCandidate();

            result.LiquidNodes.AddRange(liquidNodes);

            if (bottleNodes != null)
            {
                result.BottleNodes.AddRange(bottleNodes);
            }

            return result;
        }

        



        #endregion

        #region トレースバック 瓶→液



        #endregion

        

        
    }
}