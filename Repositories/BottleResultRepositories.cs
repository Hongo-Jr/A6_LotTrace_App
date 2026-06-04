using LotTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotTraceApp.Repositories
{
    public class BottleResultRepositories
    {
        private readonly string _connectionString;

        public BottleResultRepositories(string connectionString) 
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

        public DataTable GetBottleOrder(string order, string lot)
        {
            var result = new DataTable();

            if (string.IsNullOrWhiteSpace(order) || string.IsNullOrWhiteSpace(lot))
            {
                return result;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildBottleOrderSQL(order, lot);
                cmd.Parameters.AddWithValue("@order", order);
                cmd.Parameters.AddWithValue("@lot", lot);


                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Rows.Add(

                            reader.IsDBNull(0) ? null : reader.GetString(0),
                            reader.IsDBNull(1) ? null : reader.GetString(1),
                            reader.IsDBNull(2) ? null : reader.GetString(2),
                            reader.IsDBNull(3) ? null : reader.GetString(3),
                            reader.IsDBNull(4) ? null : reader.GetString(4),
                            reader.IsDBNull(5) ? null : reader.GetString(5),
                            reader.IsDBNull(6) ? null : reader.GetString(6),
                            reader.IsDBNull(7) ? null : reader.GetString(7),
                            reader.IsDBNull(8) ? null : reader.GetString(8),
                            reader.IsDBNull(9) ? null : reader.GetString(9),
                            reader.IsDBNull(10) ? null : reader.GetString(10),
                            reader.IsDBNull(11) ? null : reader.GetString(11),
                            reader.IsDBNull(12) ? null : reader.GetString(12),
                            reader.IsDBNull(13) ? null : reader.GetString(13),
                            reader.IsDBNull(14) ? null : reader.GetString(14),
                            reader.IsDBNull(15) ? null : reader.GetString(15),
                            reader.IsDBNull(16) ? null : reader.GetString(16),
                            reader.IsDBNull(17) ? null : reader.GetString(17),
                            reader.IsDBNull(18) ? null : reader.GetString(18),
                            reader.IsDBNull(19) ? null : reader.GetString(19),
                            reader.IsDBNull(20) ? null : reader.GetString(20),
                            reader.IsDBNull(21) ? null : reader.GetString(21),
                            reader.IsDBNull(22) ? null : reader.GetString(22),
                            reader.IsDBNull(23) ? null : reader.GetString(23),
                            reader.IsDBNull(24) ? null : reader.GetString(24),
                            reader.IsDBNull(25) ? null : reader.GetString(25),
                            reader.IsDBNull(26) ? null : reader.GetString(26),
                            reader.IsDBNull(27) ? null : reader.GetString(27),
                            reader.IsDBNull(28) ? null : reader.GetString(28),
                            reader.IsDBNull(29) ? null : reader.GetString(29)
                        );
                    }
                        return result;
                }
            }
        }

        public string BuildBottleOrderSQL(string order, string lot)
        {
            string result;

            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SELECT");
            sql.AppendLine("    [OrderNumber],");             
            sql.AppendLine("    [ProcessType],");             
            sql.AppendLine("    [ProductItemName],");         
            sql.AppendLine("    [ProductItemCode],");         
            sql.AppendLine("    [MiddleProductItemCode],");   
            sql.AppendLine("    [ProductLotNumber],");        
            sql.AppendLine("    [OutWashBottelLotNumber],");  
            sql.AppendLine("    [MiddleProductLotNumber],");  
            sql.AppendLine("    [AuxiliaryLabelID],");        
            sql.AppendLine("    [NumberOfBottleIndicated],"); 
            sql.AppendLine("    [NumberOfDrumcanSpecified],");
            sql.AppendLine("    [InjectionMode],");
            sql.AppendLine("    [SamplingDesignation1],");
            sql.AppendLine("    [SamplingDesignation2],");
            sql.AppendLine("    [SamplingDesignation3],");
            sql.AppendLine("    [SamplingDesignation4],");
            sql.AppendLine("    [SamplingDesignation5],");
            sql.AppendLine("    [FillingSettingMode],");
            sql.AppendLine("    [NozzleNumber],");
            sql.AppendLine("    [OverEntry],");
            sql.AppendLine("    [SamplingGroup1],");
            sql.AppendLine("    [SamplingGroup2],");
            sql.AppendLine("    [SamplingGroup3],");
            sql.AppendLine("    [SamplingGroup4],");
            sql.AppendLine("    [SamplingGroup5],");
            sql.AppendLine("    [SamplingGroup6],");
            sql.AppendLine("    [SamplingGroup7],");
            sql.AppendLine("    [SamplingGroup8],");
            sql.AppendLine("    [SamplingGroup9],");
            sql.AppendLine("    [SamplingGroup10]");
            sql.AppendLine("FROM [MES33].[dbo].[FillingOrderTable]");
            sql.AppendLine("WHERE 1 = 1");


            if (!string.IsNullOrWhiteSpace(order))
                sql.AppendLine("  AND [OrderNumber] = @order");

            if (!string.IsNullOrWhiteSpace(lot))
                sql.AppendLine("  AND [ProductLotNumber] = @lot");


            result = sql.ToString();

            return result;
        }




        public BottleResultPage GetFillingResultPages(string order, string lot, int pageNo, int pageSize)
        {
            var result = new BottleResultPage();
            if (string.IsNullOrWhiteSpace(order)|| string.IsNullOrWhiteSpace(lot))
            {
                return  result;
            }

            int bottleRowsCount = GetFillingBottleRowsCount(order, lot);
            if (bottleRowsCount > 0)
            {
                result.TotalCount = bottleRowsCount;
                result.BottleRows.AddRange(GetFillingBottleRows(order, lot, pageNo, pageSize));
            }

            int drumRowsCount = GetFillingDrumRowsCount(order, lot);
            if (drumRowsCount > 0)
            {
                result.TotalCount = drumRowsCount;
                result.DrumRows =GetFillingDrumRows(order, lot, pageNo, pageSize);
            }

            return result;

        }

        private int GetFillingBottleRowsCount(string order, string lot)
        {

            if (string.IsNullOrWhiteSpace(order) || string.IsNullOrWhiteSpace(lot))
            {
                return 0;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildFilingBottleCountSQL();
                cmd.Parameters.AddWithValue("@order", order);
                cmd.Parameters.AddWithValue("@lot", lot);

                return Convert.ToInt32(cmd.ExecuteScalar());
                
            }
            
        }

        private int GetFillingDrumRowsCount(string order, string lot)
        {

            if (string.IsNullOrWhiteSpace(order) || string.IsNullOrWhiteSpace(lot))
            {
                return 0;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildFillingDrumRowsCountSQL();
                cmd.Parameters.AddWithValue("@order", order);
                cmd.Parameters.AddWithValue("@lot", lot);

                return Convert.ToInt32(cmd.ExecuteScalar());

            }

        }

        private List<FillingBottleRow> GetFillingBottleRows(string order, string lot,int pageNo, int pageSize)
        {
            var result = new List<FillingBottleRow>();

            if (string.IsNullOrWhiteSpace(order) || string.IsNullOrWhiteSpace(lot))
            {
                return result;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildFillingBottlePageSQL();
                cmd.Parameters.AddWithValue("@order", order);
                cmd.Parameters.AddWithValue("@lot", lot);
                cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = (pageNo - 1) * pageSize;

                cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new FillingBottleRow();
                        row.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                        row.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        row.ProductLotNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                        row.ProductItemCode = reader.IsDBNull(3) ? null : reader.GetString(3);
                        row.MiddleProductLotNumber = reader.IsDBNull(4) ? null : reader.GetString(4);
                        row.MiddleProductItemCode = reader.IsDBNull(5) ? null : reader.GetString(5);
                        row.BottleID = reader.IsDBNull(6) ? null : reader.GetString(6);
                        row.SamplingGroup = reader.IsDBNull(7) ? null : reader.GetString(7);
                        row.BottleINumber = reader.IsDBNull(8) ? (int?) null: reader.GetInt32(8);
                        row.FillingNozzleNumber = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9);
                        row.CapTighteningTorqueValue = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                        row.CapTighteningTorqueJudgment = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11);
                        row.CapTiltDetectionJudgment = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12);
                        row.FillingMachineNumber = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13);
                        row.TotalCahckJudgment = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14);
                        row.BottleLocation = reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15);
                        row.FillingWeight = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16);
                        row.FillingTime = reader.IsDBNull(17)? (TimeSpan?) null : reader.GetTimeSpan(17);
                        row.FillingStartDate = reader.IsDBNull(18) ?(DateTime?) null : reader.GetDateTime(18);
                        row.FillingEndDate = reader.IsDBNull(19) ? (DateTime?)null : reader.GetDateTime(19);

                        result.Add(row);
                    }
                }
            }         
            return result; 
        }

        

        private string BuildFilingBottleCountSQL()
        { 
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SELECT COUNT(*)");
            sql.AppendLine("FROM [MES33].[dbo].[FillingBottleTable]");
            sql.AppendLine("WHERE 1 = 1");
            sql.AppendLine("  AND [OrderNumber] = @order");
            sql.AppendLine("  AND [ProductLotNumber] = @lot");

            return sql.ToString();

        }

        private string BuildFillingBottlePageSQL()
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT");
            sql.AppendLine("    [OrderNumber],");
            sql.AppendLine("    [ProcessType],");
            sql.AppendLine("    [ProductLotNumber],");
            sql.AppendLine("    [ProductItemCode],");
            sql.AppendLine("    [MiddleProductLotNumber],");
            sql.AppendLine("    [MiddleProductItemCode],");
            sql.AppendLine("    [BottleID],");
            sql.AppendLine("    [SamplingGroup],");
            sql.AppendLine("    [BottleINumber],");
            sql.AppendLine("    [FillingNozzleNumber],");
            sql.AppendLine("    [CapTighteningTorqueValue],");
            sql.AppendLine("    [CapTighteningTorqueJudgment],");
            sql.AppendLine("    [CapTiltDetectionJudgment],");
            sql.AppendLine("    [FillingMachineNumber],");
            sql.AppendLine("    [TotalCahckJudgment],");
            sql.AppendLine("    [BottleLocation],");
            sql.AppendLine("    [FillingWeight],");
            sql.AppendLine("    [FillingTime],");
            sql.AppendLine("    [FillingStartDate],");
            sql.AppendLine("    [FillingEndDate]");
            sql.AppendLine("FROM [MES33].[dbo].[FillingBottleTable]");
            sql.AppendLine("WHERE 1 = 1");
            sql.AppendLine("  AND [OrderNumber] = @order");
            sql.AppendLine("  AND [ProductLotNumber] = @lot");
            
            sql.AppendLine("ORDER BY [FillingStartDate] DESC, [BottleID]");
            sql.AppendLine("OFFSET @Offset ROWS");
            sql.AppendLine("FETCH NEXT @PageSize ROWS ONLY");

            return sql.ToString();
        }
        




        private List<FillingDrumcanRow> GetFillingDrumRows(string order, string lot, int pageNo, int pageSize)
        {
            var result = new List<FillingDrumcanRow>();

            if (string.IsNullOrWhiteSpace(order) || string.IsNullOrWhiteSpace(lot))
            {
                return result;
            }

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = BuildFillingDrumRowsSQL();
                cmd.Parameters.AddWithValue("@order", order);
                cmd.Parameters.AddWithValue("@lot", lot);
                cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = (pageNo - 1) * pageSize;

                cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new FillingDrumcanRow();
                        row.OrderNumber = reader.IsDBNull(0) ? null : reader.GetString(0);
                        row.ProcessType = reader.IsDBNull(1) ? null : reader.GetString(1);
                        row.ProductLotNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                        row.ProductItemCode = reader.IsDBNull(3) ? null : reader.GetString(3);
                        row.MiddleProductLotNumber = reader.IsDBNull(4) ? null : reader.GetString(4);
                        row.MiddleProductItemCode = reader.IsDBNull(5) ? null : reader.GetString(5);
                        row.DrumcanNumber = reader.GetInt32(6);
                        row.FillingNozzleNumber = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7);
                        row.CapTighteningTorqueValue_Big = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8);
                        row.CapTighteningTorqueJudgment = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9);
                        row.CapTiltDetectionJudgment = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                        row.TotalCahckJudgment = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11);
                        row.CapTighteningTorqueValue_Small = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12);
                        row.FillingWeightJudgment = reader.IsDBNull(13) ? (int?)null : reader.GetInt32(13);
                        row.BottleLocation = reader.IsDBNull(14) ? (int?)null : reader.GetInt32(14);
                        row.FillingWeight = reader.IsDBNull(15) ? (int?)null : reader.GetInt32(15);
                        row.FillingTime = reader.IsDBNull(16) ? (TimeSpan?)null : reader.GetTimeSpan(16);
                        row.FillingStartDate = reader.IsDBNull(17) ? (DateTime?)null : reader.GetDateTime(17);
                        row.FillingEndDate = reader.IsDBNull(18) ? (DateTime?)null : reader.GetDateTime(18);

                        result.Add(row);
                    }
                }
            }

                return result;
        }


        private string BuildFillingDrumRowsSQL()
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT");
            sql.AppendLine("    [OrderNumber],");
            sql.AppendLine("    [ProcessType],");
            sql.AppendLine("    [ProductLotNumber],");
            sql.AppendLine("    [ProductItemCode],");
            sql.AppendLine("    [MiddleProductLotNumber],");
            sql.AppendLine("    [MiddleProductItemCode],");
            sql.AppendLine("    [DrumcanNumber],");
            sql.AppendLine("    [FillingNozzleNumber],");
            sql.AppendLine("    [CapTighteningTorqueValue_Big],");
            sql.AppendLine("    [CapTighteningTorqueJudgment],");
            sql.AppendLine("    [CapTiltDetectionJudgment],");
            sql.AppendLine("    [TotalCahckJudgment],");
            sql.AppendLine("    [CapTighteningTorqueValue_Small],");
            sql.AppendLine("    [FillingWeightJudgment],");
            sql.AppendLine("    [BottleLocation],");
            sql.AppendLine("    [FillingWeight],");
            sql.AppendLine("    [FillingTime],");
            sql.AppendLine("    [FillingStartDate],");
            sql.AppendLine("    [FillingEndDate]");
            sql.AppendLine("FROM [MES33].[dbo].[FillingDrumcanTable]");
            sql.AppendLine("WHERE [OrderNumber] = @order");
            sql.AppendLine("  AND [ProductLotNumber] = @lot");
            sql.AppendLine("ORDER BY [FillingStartDate] DESC");
            sql.AppendLine("OFFSET @Offset ROWS");
            sql.AppendLine("FETCH NEXT @PageSize ROWS ONLY");

            return sql.ToString();
        }

        private static string BuildFillingDrumRowsCountSQL()
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT COUNT(*)");
            sql.AppendLine("FROM [MES33].[dbo].[FillingDrumcanTable]");
            sql.AppendLine("WHERE [OrderNumber] = @order");
            sql.AppendLine("  AND [ProductLotNumber] = @lot");

            return sql.ToString();
        }

    }
}
