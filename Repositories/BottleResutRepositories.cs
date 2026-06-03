using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotTraceApp.Repositories
{
    internal class BottleResutRepositories
    {
        private readonly string _connectionString;

        public BottleResutRepositories(string connectionString) 
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

            




            return result;
        }

        public string BuildBottleOrderSQL(string order, string lot)
        {
            string result;

            StringBuilder sql = new StringBuilder();





            result = sql.ToString();

            return result;
        }

    }
}
