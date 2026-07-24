using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using LotTraceApp.Models;
using ClosedXML.Excel;

namespace LotTraceApp.Utils
{
    public static class ExportHelper
    {
        /// <summary>
        /// トレース結果を CSV 1 ファイルとして出力（液設備 / 瓶設備共通想定）
        /// </summary>
        public static void ExportTraceResultToCsv(TraceResult traceResult, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("NodeType,Depth,ProductionOrderNumber,ItemName,ItemCode,LotNumber,StartDate,EndDate,ProcessName,TankName");

                void WriteNodes(IEnumerable<ProductionResultNode> nodes)
                {
                    foreach (var n in nodes)
                    {
                        string line = string.Join(",",
                            Quote(n.NodeType),
                            n.Depth,
                            Quote(n.ProductionOrderNumber),
                            Quote(n.ItemName),
                            Quote(n.ItemCode),
                            Quote(n.LotNumber),
                            Quote(n.StartDate?.ToString("yyyy-MM-dd HH:mm:ss")),
                            Quote(n.EndDate?.ToString("yyyy-MM-dd HH:mm:ss")),
                            Quote(n.ManufacturingProcessName),
                            Quote(n.ManufacturingTankName));
                        writer.WriteLine(line);
                    }
                }

                WriteNodes(traceResult.StartNodes);
                WriteNodes(traceResult.MiddleNodes);
                WriteNodes(traceResult.EndNodes);
            }

            string Quote(string? s) =>
                s == null ? "" :
                "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>
        /// 複数タブの TraceResult を 1 つの XLSX ファイル（シート単位）で出力
        /// </summary>
        public static void ExportTabsToExcel(Dictionary<int, TraceResult> tabResults, string filePath)
        {
            using (var wb = new XLWorkbook())
            {
                foreach (var kv in tabResults)
                {
                    int tabNo = kv.Key;
                    var trace = kv.Value;

                    var ws = wb.Worksheets.Add($"Tab{tabNo}");

                    // ヘッダ
                    ws.Cell(1, 1).Value = "NodeType";
                    ws.Cell(1, 2).Value = "Depth";
                    ws.Cell(1, 3).Value = "ProductionOrderNumber";
                    ws.Cell(1, 4).Value = "ItemName";
                    ws.Cell(1, 5).Value = "ItemCode";
                    ws.Cell(1, 6).Value = "LotNumber";
                    ws.Cell(1, 7).Value = "StartDate";
                    ws.Cell(1, 8).Value = "EndDate";
                    ws.Cell(1, 9).Value = "ProcessName";
                    ws.Cell(1, 10).Value = "TankName";

                    int row = 2;

                    void WriteNodes(IEnumerable<ProductionResultNode> nodes)
                    {
                        foreach (var n in nodes)
                        {
                            ws.Cell(row, 1).Value = n.NodeType;
                            ws.Cell(row, 2).Value = n.Depth;
                            ws.Cell(row, 3).Value = n.ProductionOrderNumber;
                            ws.Cell(row, 4).Value = n.ItemName;
                            ws.Cell(row, 5).Value = n.ItemCode;
                            ws.Cell(row, 6).Value = n.LotNumber;
                            ws.Cell(row, 7).Value = n.StartDate;
                            ws.Cell(row, 8).Value = n.EndDate;
                            ws.Cell(row, 9).Value = n.ManufacturingProcessName;
                            ws.Cell(row, 10).Value = n.ManufacturingTankName;
                            row++;
                        }
                    }

                    WriteNodes(trace.StartNodes);
                    WriteNodes(trace.MiddleNodes);
                    WriteNodes(trace.EndNodes);

                    ws.Columns().AdjustToContents();
                }

                wb.SaveAs(filePath);
            }
        }

        /// <summary>
        /// DataTable を CSV に出力（履歴詳細など）
        /// </summary>
        public static void ExportDataTableToCsv(DataTable table, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // ヘッダ
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i > 0) writer.Write(",");
                    writer.Write(Quote(table.Columns[i].ColumnName));
                }
                writer.WriteLine();

                // データ
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (i > 0) writer.Write(",");
                        writer.Write(Quote(row[i]?.ToString()));
                    }
                    writer.WriteLine();
                }
            }

            string Quote(string? s) =>
                s == null ? "" :
                "\"" + s.Replace("\"", "\"\"") + "\"";
        }
    }
}