using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotTraceApp.Repositories
{
    internal class BottleResultRepositories
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

    }
}
