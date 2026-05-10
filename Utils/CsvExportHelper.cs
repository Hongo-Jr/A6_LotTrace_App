using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LotTraceApp.Utils
{
    public static class CsvExportHelper
    {
        public static void ExportCurrentGridsToCsv(
            string filePath,
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is null or empty.", "filePath");

            var plans = BuildExportColumnPlans(dgvLeft, dgvMiddle, dgvRight);
            var visibleRows = BuildVisibleRowMaps(dgvLeft, dgvMiddle, dgvRight);

            var lines = new List<string>();

            lines.Add(BuildCsvLine(plans.ConvertAll(x => x.HeaderText)));

            for (int rowIndex = 0; rowIndex < visibleRows.MaxVisibleRowCount; rowIndex++)
            {
                var values = new List<string>();

                foreach (var plan in plans)
                {
                    DataGridViewRow row = GetVisibleRow(visibleRows, plan.Area, rowIndex);
                    values.Add(GetCellText(row, plan.GridColumn));
                }

                lines.Add(BuildCsvLine(values));
            }

            System.IO.File.WriteAllLines(filePath, lines, new UTF8Encoding(true));
        }

        private static List<CsvColumnPlan> BuildExportColumnPlans(
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight)
        {
            var result = new List<CsvColumnPlan>();

            AddGridPlans(result, dgvLeft, CsvGridArea.Left);
            AddGridPlans(result, dgvMiddle, CsvGridArea.Middle);
            AddGridPlans(result, dgvRight, CsvGridArea.Right);

            return result;
        }

        private static void AddGridPlans(
            List<CsvColumnPlan> plans,
            DataGridView dgv,
            CsvGridArea area)
        {
            if (plans == null || dgv == null)
                return;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                plans.Add(new CsvColumnPlan
                {
                    Area = area,
                    GridColumn = col,
                    HeaderText = Convert.ToString(col.HeaderText)
                });
            }
        }

        private static CsvVisibleRowMaps BuildVisibleRowMaps(
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight)
        {
            var maps = new CsvVisibleRowMaps();
            maps.LeftRows = GetVisibleRows(dgvLeft);
            maps.MiddleRows = GetVisibleRows(dgvMiddle);
            maps.RightRows = GetVisibleRows(dgvRight);
            maps.MaxVisibleRowCount = Math.Max(
                maps.LeftRows.Count,
                Math.Max(maps.MiddleRows.Count, maps.RightRows.Count));
            return maps;
        }

        private static List<DataGridViewRow> GetVisibleRows(DataGridView dgv)
        {
            var list = new List<DataGridViewRow>();
            if (dgv == null)
                return list;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                list.Add(row);
            }

            return list;
        }

        private static DataGridViewRow GetVisibleRow(
            CsvVisibleRowMaps visibleRows,
            CsvGridArea area,
            int rowIndex)
        {
            if (visibleRows == null || rowIndex < 0)
                return null;

            List<DataGridViewRow> rows;
            switch (area)
            {
                case CsvGridArea.Left:
                    rows = visibleRows.LeftRows;
                    break;
                case CsvGridArea.Middle:
                    rows = visibleRows.MiddleRows;
                    break;
                case CsvGridArea.Right:
                    rows = visibleRows.RightRows;
                    break;
                default:
                    return null;
            }

            if (rows == null || rowIndex >= rows.Count)
                return null;

            return rows[rowIndex];
        }

        private static string GetCellText(DataGridViewRow row, DataGridViewColumn column)
        {
            if (row == null || column == null)
                return string.Empty;

            object value = row.Cells[column.Index].FormattedValue;
            return value == null ? string.Empty : Convert.ToString(value);
        }

        private static string BuildCsvLine(List<string> values)
        {
            if (values == null || values.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');

                sb.Append(EscapeCsv(values[i]));
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string value)
        {
            if (value == null)
                return string.Empty;

            bool shouldQuote =
                value.IndexOf(',') >= 0 ||
                value.IndexOf('"') >= 0 ||
                value.IndexOf('\r') >= 0 ||
                value.IndexOf('\n') >= 0;

            if (!shouldQuote)
                return value;

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private enum CsvGridArea
        {
            Left,
            Middle,
            Right
        }

        private sealed class CsvColumnPlan
        {
            public CsvGridArea Area { get; set; }
            public DataGridViewColumn GridColumn { get; set; }
            public string HeaderText { get; set; }
        }

        private sealed class CsvVisibleRowMaps
        {
            public List<DataGridViewRow> LeftRows { get; set; }
            public List<DataGridViewRow> MiddleRows { get; set; }
            public List<DataGridViewRow> RightRows { get; set; }
            public int MaxVisibleRowCount { get; set; }

            public CsvVisibleRowMaps()
            {
                LeftRows = new List<DataGridViewRow>();
                MiddleRows = new List<DataGridViewRow>();
                RightRows = new List<DataGridViewRow>();
            }
        }
    }
}
