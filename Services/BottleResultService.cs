using LotTraceApp.Repositories;
using System;
using System.Data;

namespace LotTraceApp.Services
{
    public class BottleResultService
    {
        private readonly BottleResultRepositories _repo;

        public BottleResultService(BottleResultRepositories bottleResultRepositories)
        {
            _repo = bottleResultRepositories ?? throw new ArgumentNullException(nameof(bottleResultRepositories));
        }

       
        public DataTable GetBottleOrder(string order, string lot)
            => _repo.GetBottleOrder(order, lot);

        
        public DataTable GetBottleOrderVerticalAll(string order, string lot)
        {
            var wide = _repo.GetBottleOrder(order, lot);
            return ToVerticalAllColumns(wide);
        }

        private static DataTable ToVerticalAllColumns(DataTable wide)
        {
            var vertical = new DataTable();
            vertical.Columns.Add("Item", typeof(string));
            vertical.Columns["Item"].Caption = "項目";

            vertical.Columns.Add("order", typeof(string));
            vertical.Columns["order"].Caption = "指図";


            if (wide == null || wide.Columns.Count == 0)
                return vertical;

            for (int r = 0; r < wide.Rows.Count; r++)
            {
                var row = wide.Rows[r];

                foreach (DataColumn col in wide.Columns)
                {
                    
                    string value = row.IsNull(col) ? null : Convert.ToString(row[col]);
                    vertical.Rows.Add(col.Caption, value);
                    //vertical.Rows.Add(r, col.ColumnName, value, col.Ordinal);
                }
            }

            return vertical;
        }
    }
}