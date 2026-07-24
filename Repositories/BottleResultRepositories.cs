using LotTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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

        public DataTable GetBottleOrder(string? order, string? lot)
        {
            var result = SettWideTabel();

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
                            reader.IsDBNull(9) ? null : reader.GetInt32(9).ToString(),
                            reader.IsDBNull(10) ? null : reader.GetInt32(10).ToString(),
                            reader.IsDBNull(11) ? null : reader.GetInt32(11).ToString(),
                            reader.IsDBNull(12) ? null : reader.GetInt32(12).ToString(),
                            reader.IsDBNull(13) ? null : reader.GetInt32(13).ToString(),
                            reader.IsDBNull(14) ? null : reader.GetInt32(14).ToString(),
                            reader.IsDBNull(15) ? null : reader.GetInt32(15).ToString(),
                            reader.IsDBNull(16) ? null : reader.GetInt32(16).ToString(),
                            reader.IsDBNull(17) ? null : reader.GetInt32(17).ToString(),
                            reader.IsDBNull(18) ? null : reader.GetInt32(18).ToString(),
                            reader.IsDBNull(19) ? null : reader.GetInt32(19).ToString(),
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

                    //result.Load(reader);
                    return result;
                }
            }
        }

        private DataTable SettWideTabel()
        {
            var result = new DataTable();

            result.Columns.Add("OrderNumber", typeof(string));
            result.Columns["OrderNumber"]!.Caption = "指図番号";
            result.Columns.Add("ProcessType", typeof(string));
            result.Columns["ProcessType"]!.Caption = "工程種別";
            result.Columns.Add("ProductItemName", typeof(string));
            result.Columns["ProductItemName"]!.Caption = "製品品目名";
            result.Columns.Add("ProductItemCode", typeof(string));
            result.Columns["ProductItemCode"]!.Caption = "製品品目コード";
            result.Columns.Add("MiddleProductItemCode", typeof(string));
            result.Columns["MiddleProductItemCode"]!.Caption = "中間品品目コード";
            result.Columns.Add("ProductLotNumber", typeof(string));
            result.Columns["ProductLotNumber"]!.Caption = "製品ロットNo.";
            result.Columns.Add("OutWashBottelLotNumber", typeof(string));
            result.Columns["OutWashBottelLotNumber"]!.Caption = "出庫洗瓶ロットNo.";
            result.Columns.Add("MiddleProductLotNumber", typeof(string));
            result.Columns["MiddleProductLotNumber"]!.Caption = "中間品ロットNo.";
            result.Columns.Add("AuxiliaryLabelID", typeof(string));
            result.Columns["AuxiliaryLabelID"]!.Caption = "補助ラベルID";
            result.Columns.Add("NumberOfBottleIndicated", typeof(int));
            result.Columns["NumberOfBottleIndicated"]!.Caption = "指示瓶本数";
            result.Columns.Add("NumberOfDrumcanSpecified", typeof(int));
            result.Columns["NumberOfDrumcanSpecified"]!.Caption = "指示ドラム缶本数";
            result.Columns.Add("InjectionMode", typeof(string));
            result.Columns["InjectionMode"]!.Caption = "投入モード";
            result.Columns.Add("SamplingDesignation1", typeof(string));
            result.Columns["SamplingDesignation1"]!.Caption = "サンプリング指定1";
            result.Columns.Add("SamplingDesignation2", typeof(string));
            result.Columns["SamplingDesignation2"]!.Caption = "サンプリング指定2";
            result.Columns.Add("SamplingDesignation3", typeof(string));
            result.Columns["SamplingDesignation3"]!.Caption = "サンプリング指定3";
            result.Columns.Add("SamplingDesignation4", typeof(string));
            result.Columns["SamplingDesignation4"]!.Caption = "サンプリング指定4";
            result.Columns.Add("SamplingDesignation5", typeof(string));
            result.Columns["SamplingDesignation5"]!.Caption = "サンプリング指定5";
            result.Columns.Add("FillingSettingMode", typeof(string));
            result.Columns["FillingSettingMode"]!.Caption = "充填設定モード";
            result.Columns.Add("NozzleNumber", typeof(int));
            result.Columns["NozzleNumber"]!.Caption = "ノズル番号";
            result.Columns.Add("OverEntry", typeof(string));
            result.Columns["OverEntry"]!.Caption = "追越入庫";
            result.Columns.Add("SamplingGroup1", typeof(string));
            result.Columns["SamplingGroup1"]!.Caption = "サンプリンググループ１";
            result.Columns.Add("SamplingGroup2", typeof(string));
            result.Columns["SamplingGroup2"]!.Caption = "サンプリンググループ２";
            result.Columns.Add("SamplingGroup3", typeof(string));
            result.Columns["SamplingGroup3"]!.Caption = "サンプリンググループ３";
            result.Columns.Add("SamplingGroup4", typeof(string));
            result.Columns["SamplingGroup4"]!.Caption = "サンプリンググループ４";
            result.Columns.Add("SamplingGroup5", typeof(string));
            result.Columns["SamplingGroup5"]!.Caption = "サンプリンググループ５";
            result.Columns.Add("SamplingGroup6", typeof(string));
            result.Columns["SamplingGroup6"]!.Caption = "サンプリンググループ６";
            result.Columns.Add("SamplingGroup7", typeof(string));
            result.Columns["SamplingGroup7"]!.Caption = "サンプリンググループ７";
            result.Columns.Add("SamplingGroup8", typeof(string));
            result.Columns["SamplingGroup8"]!.Caption = "サンプリンググループ８";
            result.Columns.Add("SamplingGroup9", typeof(string));
            result.Columns["SamplingGroup9"]!.Caption = "サンプリンググループ９";
            result.Columns.Add("SamplingGroup10", typeof(string));
            result.Columns["SamplingGroup10"]!.Caption = "サンプリンググループ１０";


            return result;
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




        public BottleResultPage GetFillingResultPages(string? order, string? lot, int pageNo, int pageSize)
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
                        row.FillingWeight = reader.IsDBNull(16) ? (long?)null : reader.GetInt64(16);
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
            
            sql.AppendLine("ORDER BY [FillingStartDate] ASC, [BottleID]");
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
                        row.FillingWeight = reader.IsDBNull(15) ? (long?)null : reader.GetInt64(15);
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
