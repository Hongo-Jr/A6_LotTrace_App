using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClosedXML.Excel;
using LotTraceApp.Models;

namespace LotTraceApp.Utils
{
    /// <summary>
    /// 現在画面に表示されている3つのDataGridViewを横連結して
    /// 1シートのExcel(.xlsx)として出力するヘルパー。
    ///
    /// 対応内容：
    /// - visible列 / visible行の出力
    /// - 左 + 中 + 右 の横連結
    /// - 2段ヘッダー（1段目：グループ見出し、2段目：列見出し）
    /// - ヘッダー文字、セル文字、背景色、文字色、フォント、列幅、行高さ
    /// - 基本罫線
    /// - 構造線（幹/枝/始点切替）のExcel反映
    /// - 中グリッドLv境界の縦線
    ///
    /// 構造線ルール：
    /// - 左グリッド：始点切替時のみオレンジ線
    /// - 中グリッド：Lv1 NodeKey切替でオレンジ線、Lv2+ NodeKey切替で黒線
    /// - 右グリッド：中グリッドの構造線を引き継ぐ
    ///
    /// 注意：
    /// - DataGridViewRow.DataBoundItem が DataRowView である前提
    /// - MainForm の完全な CellPainting 再現ではなく、表示ルール準拠の Excel 再構成版
    /// </summary>
    public static class ExcelExportHelper
    {
        private static readonly Color DefaultGridBorderColor = Color.LightGray;
        private static readonly Color LevelBoundaryColor = Color.DimGray;

        private static readonly Color StartHeaderBackColor = Color.FromArgb(235, 242, 250);
        private static readonly Color StartHeaderForeColor = Color.FromArgb(40, 40, 40);

        private static readonly Color MiddleHeaderBackColor = Color.FromArgb(232, 245, 236);
        private static readonly Color MiddleHeaderForeColor = Color.FromArgb(40, 40, 40);

        private static readonly Color EndHeaderBackColor = Color.FromArgb(250, 238, 238);
        private static readonly Color EndHeaderForeColor = Color.FromArgb(40, 40, 40);

        private const int GroupHeaderExcelRow = 1;
        private const int ColumnHeaderExcelRow = 2;
        private const int FirstBodyExcelRow = 3;

        public sealed class TraceGridExcelExportRequest
        {
            public string WorksheetName { get; set; }
            public DataGridView LeftGrid { get; set; }
            public DataGridView MiddleGrid { get; set; }
            public DataGridView RightGrid { get; set; }
            public TraceDisplayResult DisplayResult { get; set; }
            public TraceGridDrawContext DrawContext { get; set; }
            public ISet<string> CrossPointNodeKeys { get; set; }
        }

        public static void ExportCurrentGridsToExcel(
    string filePath,
    DataGridView dgvLeft,
    DataGridView dgvMiddle,
    DataGridView dgvRight,
    string worksheetName,
    TraceDisplayResult displayResult,
    TraceGridDrawContext drawContext,
    ISet<string> crossPointNodeKeys = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is null or empty.", nameof(filePath));

            if (dgvLeft == null)
                throw new ArgumentNullException(nameof(dgvLeft));
            if (dgvMiddle == null)
                throw new ArgumentNullException(nameof(dgvMiddle));
            if (dgvRight == null)
                throw new ArgumentNullException(nameof(dgvRight));

            var plans = BuildExportColumnPlans(dgvLeft, dgvMiddle, dgvRight);
            var visibleRows = BuildVisibleRowMaps(dgvLeft, dgvMiddle, dgvRight);

            using (var wb = new XLWorkbook())
            {
                WriteTraceSheet(
                    wb,
                    worksheetName,
                    dgvLeft,
                    dgvMiddle,
                    dgvRight,
                    displayResult,
                    drawContext,
                    crossPointNodeKeys);

                wb.SaveAs(filePath);
            }
        }

        public static void ExportTraceSheetsToExcel(
            string filePath,
            IEnumerable<TraceGridExcelExportRequest> traceSheets,
            DataGridView intersectionGrid,
            string intersectionWorksheetName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is null or empty.", nameof(filePath));

            bool hasAnySheet = false;

            using (var wb = new XLWorkbook())
            {
                if (traceSheets != null)
                {
                    foreach (var request in traceSheets)
                    {
                        if (request == null)
                            continue;

                        WriteTraceSheet(
                            wb,
                            request.WorksheetName,
                            request.LeftGrid,
                            request.MiddleGrid,
                            request.RightGrid,
                            request.DisplayResult,
                            request.DrawContext,
                            request.CrossPointNodeKeys);
                        hasAnySheet = true;
                    }
                }

                if (HasVisibleData(intersectionGrid))
                {
                    WriteDataGridSheet(
                        wb,
                        string.IsNullOrWhiteSpace(intersectionWorksheetName)
                            ? "CrossPoints"
                            : intersectionWorksheetName,
                        intersectionGrid);
                    hasAnySheet = true;
                }

                if (!hasAnySheet)
                    throw new InvalidOperationException("出力対象のシートがありません。");

                wb.SaveAs(filePath);
            }
        }

        private static void WriteTraceSheet(
            XLWorkbook wb,
            string worksheetName,
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight,
            TraceDisplayResult displayResult,
            TraceGridDrawContext drawContext,
            ISet<string> crossPointNodeKeys)
        {
            if (wb == null)
                throw new ArgumentNullException(nameof(wb));
            if (dgvLeft == null)
                throw new ArgumentNullException(nameof(dgvLeft));
            if (dgvMiddle == null)
                throw new ArgumentNullException(nameof(dgvMiddle));
            if (dgvRight == null)
                throw new ArgumentNullException(nameof(dgvRight));

            var plans = BuildExportColumnPlans(dgvLeft, dgvMiddle, dgvRight);
            var visibleRows = BuildVisibleRowMaps(dgvLeft, dgvMiddle, dgvRight);
            var ws = wb.Worksheets.Add(GetUniqueWorksheetName(wb, worksheetName));

            ws.SheetView.FreezeRows(2);

            WriteGroupHeaders(ws, plans, dgvLeft, dgvMiddle, dgvRight);
            WriteColumnHeaders(ws, plans);
            WriteBody(ws, plans, visibleRows, crossPointNodeKeys);

            ApplyColumnWidths(ws, plans);
            ApplyHeaderRowHeights(ws, plans);

            ApplyMiddleLevelVerticalBoundaries(ws, plans, visibleRows.MaxVisibleRowCount);
            ApplyTraceStructureLines(ws, plans, visibleRows, displayResult, drawContext);

            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void WriteDataGridSheet(
            XLWorkbook wb,
            string worksheetName,
            DataGridView grid)
        {
            if (wb == null)
                throw new ArgumentNullException(nameof(wb));
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            var visibleColumns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column != null && column.Visible)
                    visibleColumns.Add(column);
            }

            var visibleRows = GetVisibleRows(grid);
            var ws = wb.Worksheets.Add(GetUniqueWorksheetName(wb, worksheetName));
            ws.SheetView.FreezeRows(1);

            for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
            {
                var column = visibleColumns[colIndex];
                var cell = ws.Cell(1, colIndex + 1);
                cell.Value = NullToEmpty(column.HeaderText);
                ApplySimpleGridHeaderStyle(cell);
                ws.Column(colIndex + 1).Width = ConvertPixelToExcelColumnWidth(column.Width);
            }

            for (int rowIndex = 0; rowIndex < visibleRows.Count; rowIndex++)
            {
                var gridRow = visibleRows[rowIndex];
                int excelRow = rowIndex + 2;
                ws.Row(excelRow).Height = ConvertPixelToExcelRowHeight(gridRow.Height);

                for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
                {
                    var gridCell = gridRow.Cells[visibleColumns[colIndex].Index];
                    var xlCell = ws.Cell(excelRow, colIndex + 1);
                    WriteCellValue(xlCell, gridCell);
                    ApplySimpleGridCellStyle(xlCell, gridCell);
                    ApplyIntersectionCrossPointBackColor(xlCell, gridRow);
                }
            }

            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static bool HasVisibleData(DataGridView grid)
        {
            if (grid == null)
                return false;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                return true;
            }

            return false;
        }

        private static List<ExportColumnPlan> BuildExportColumnPlans(
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight)
        {
            var result = new List<ExportColumnPlan>();
            int excelCol = 1;

            AddGridPlans(result, dgvLeft, GridArea.Left, ref excelCol);
            AddGridPlans(result, dgvMiddle, GridArea.Middle, ref excelCol);
            AddGridPlans(result, dgvRight, GridArea.Right, ref excelCol);

            return result;
        }

        private static void AddGridPlans(
            List<ExportColumnPlan> plans,
            DataGridView dgv,
            GridArea area,
            ref int excelCol)
        {
            if (dgv == null)
                return;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (!col.Visible)
                    continue;

                plans.Add(new ExportColumnPlan
                {
                    Grid = dgv,
                    Area = area,
                    GridColumn = col,
                    ExcelColumnIndex = excelCol,
                    MiddleLevel = area == GridArea.Middle ? TryGetMiddleLevel(col.Name) : 0
                });

                excelCol++;
            }
        }

        private static VisibleRowMaps BuildVisibleRowMaps(
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight)
        {
            var maps = new VisibleRowMaps();
            maps.LeftRows = GetVisibleRows(dgvLeft);
            maps.MiddleRows = GetVisibleRows(dgvMiddle);
            maps.RightRows = GetVisibleRows(dgvRight);
            maps.MaxVisibleRowCount = Math.Max(maps.LeftRows.Count, Math.Max(maps.MiddleRows.Count, maps.RightRows.Count));
            return maps;
        }

        private static List<DataGridViewRow> GetVisibleRows(DataGridView dgv)
        {
            var list = new List<DataGridViewRow>();
            if (dgv == null)
                return list;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.Visible)
                    continue;
                if (row.IsNewRow)
                    continue;

                list.Add(row);
            }

            return list;
        }

        private static void WriteGroupHeaders(
            IXLWorksheet ws,
            List<ExportColumnPlan> plans,
            DataGridView dgvLeft,
            DataGridView dgvMiddle,
            DataGridView dgvRight)
        {
            WriteAreaGroupHeader(ws, plans, GridArea.Left, BuildLeftGroupHeaderText(dgvLeft));
            WriteMiddleLevelGroupHeaders(ws, plans, dgvMiddle);
            WriteAreaGroupHeader(ws, plans, GridArea.Right, BuildRightGroupHeaderText(dgvRight));
        }

        private static void WriteAreaGroupHeader(
            IXLWorksheet ws,
            List<ExportColumnPlan> plans,
            GridArea area,
            string text)
        {
            int startCol = GetAreaFirstExcelColumn(plans, area);
            int endCol = GetAreaLastExcelColumn(plans, area);

            if (startCol <= 0 || endCol <= 0 || endCol < startCol)
                return;

            var range = ws.Range(GroupHeaderExcelRow, startCol, GroupHeaderExcelRow, endCol);
            range.Merge();

            var cell = ws.Cell(GroupHeaderExcelRow, startCol);
            cell.Value = text ?? string.Empty;

            ApplyGroupHeaderStyle(range, area);
        }

        private static void WriteMiddleLevelGroupHeaders(
            IXLWorksheet ws,
            List<ExportColumnPlan> plans,
            DataGridView dgvMiddle)
        {
            var levels = GetMiddleLevels(plans);
            for (int i = 0; i < levels.Count; i++)
            {
                int level = levels[i];
                int startCol = GetMiddleLevelFirstExcelColumn(plans, level);
                int endCol = GetMiddleLevelLastExcelColumn(plans, level);

                if (startCol <= 0 || endCol <= 0 || endCol < startCol)
                    continue;

                var range = ws.Range(GroupHeaderExcelRow, startCol, GroupHeaderExcelRow, endCol);
                range.Merge();

                var cell = ws.Cell(GroupHeaderExcelRow, startCol);
                cell.Value = BuildMiddleGroupHeaderText(dgvMiddle, level);

                ApplyGroupHeaderStyle(range, GridArea.Middle);
            }
        }

        private static void WriteColumnHeaders(IXLWorksheet ws, List<ExportColumnPlan> plans)
        {
            for (int i = 0; i < plans.Count; i++)
            {
                var plan = plans[i];
                var cell = ws.Cell(ColumnHeaderExcelRow, plan.ExcelColumnIndex);

                cell.Value = NullToEmpty(plan.GridColumn.HeaderText);
                ApplyColumnHeaderStyle(cell, plan.Grid, plan.GridColumn, plan.Area);
            }
        }

        private static void WriteBody(
            IXLWorksheet ws,
            List<ExportColumnPlan> plans,
            VisibleRowMaps visibleRows,
            ISet<string> crossPointNodeKeys)
        {
            for (int visibleRowIndex = 0; visibleRowIndex < visibleRows.MaxVisibleRowCount; visibleRowIndex++)
            {
                int excelRow = FirstBodyExcelRow + visibleRowIndex;

                DataGridViewRow baseRow = GetBaseVisibleRow(visibleRows, visibleRowIndex);
                if (baseRow != null)
                {
                    ws.Row(excelRow).Height = ConvertPixelToExcelRowHeight(baseRow.Height);
                }

                for (int p = 0; p < plans.Count; p++)
                {
                    var plan = plans[p];
                    var gridRow = GetVisibleRowByArea(visibleRows, plan.Area, visibleRowIndex);
                    var xlCell = ws.Cell(excelRow, plan.ExcelColumnIndex);

                    if (gridRow == null)
                    {
                        xlCell.Value = string.Empty;
                        ApplyDefaultThinBorder(xlCell);
                        continue;
                    }

                    var dgvCell = gridRow.Cells[plan.GridColumn.Index];
                    WriteCellValue(xlCell, dgvCell);
                    ApplyCellStyle(xlCell, dgvCell, plan, crossPointNodeKeys);
                }
            }
        }

        private static void ApplyTraceStructureLines(
    IXLWorksheet ws,
    List<ExportColumnPlan> plans,
    VisibleRowMaps visibleRows,
    TraceDisplayResult displayResult,
    TraceGridDrawContext drawContext)
        {
            ApplyLeftStartBoundaryLinesFromDrawContext(ws, plans, drawContext);
            ApplyMiddleAndRightTraceLinesFromDrawContext(ws, plans, drawContext);
        }

        private static void ApplyLeftStartBoundaryLinesFromDrawContext(
    IXLWorksheet ws,
    List<ExportColumnPlan> plans,
    TraceGridDrawContext drawContext)
        {
            int leftStartCol = GetAreaFirstExcelColumn(plans, GridArea.Left);
            int leftEndCol = GetAreaLastExcelColumn(plans, GridArea.Left);

            if (leftStartCol <= 0 || leftEndCol <= 0)
                return;

            if (drawContext == null || drawContext.Start == null || drawContext.Start.Rows == null)
                return;

            foreach (var row in drawContext.Start.Rows)
            {
                if (row == null)
                    continue;

                if (!row.DrawBottomDivider)
                    continue;

                int excelRow = FirstBodyExcelRow + row.RowIndex;
                ApplyBottomBorderRange(
                    ws,
                    excelRow,
                    leftStartCol,
                    leftEndCol,
                    ResolveTraceLineColor("Start"),
                    XLBorderStyleValues.Medium);
            }
        }

        private static void ApplyMiddleAndRightTraceLinesFromDrawContext(
    IXLWorksheet ws,
    List<ExportColumnPlan> plans,
    TraceGridDrawContext drawContext)
        {
            if (drawContext == null)
                return;

            int middleLastCol = GetAreaLastExcelColumn(plans, GridArea.Middle);
            int rightLastCol = GetAreaLastExcelColumn(plans, GridArea.Right);

            if (middleLastCol <= 0)
                return;

            int totalEndCol = rightLastCol > 0 ? rightLastCol : middleLastCol;

            if (drawContext.Middle != null && drawContext.Middle.HorizontalLines != null)
            {
                var preferredLines = ResolvePreferredMiddleHorizontalLines(
                    drawContext.Middle.HorizontalLines);

                foreach (var line in preferredLines)
                {
                    if (line == null)
                        continue;

                    int fromXLevel = line.FromXLevel;
                    if (string.Equals(line.LineKind, "Start", StringComparison.OrdinalIgnoreCase) && fromXLevel <= 0)
                        fromXLevel = 1;

                    int startCol = GetMiddleLevelFirstExcelColumn(plans, fromXLevel);
                    if (startCol <= 0)
                        continue;

                    int startRowIndex = Math.Max(0, line.StartRowIndex);
                    int endRowIndex = Math.Max(startRowIndex, line.EndRowIndex);

                    Color lineColor = ResolveTraceLineColor(line.LineKind);
                    XLBorderStyleValues borderStyle = XLBorderStyleValues.Medium;

                    for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++)
                    {
                        int excelRow = FirstBodyExcelRow + rowIndex;

                        ApplyBottomBorderRange(
                            ws,
                            excelRow,
                            startCol,
                            totalEndCol,
                            lineColor,
                            borderStyle);
                    }
                }
            }

            if (drawContext.End != null && drawContext.End.HorizontalLines != null && rightLastCol > 0)
            {
                int rightStartCol = GetAreaFirstExcelColumn(plans, GridArea.Right);
                if (rightStartCol <= 0)
                    return;

                foreach (var line in drawContext.End.HorizontalLines)
                {
                    if (line == null)
                        continue;

                    int startRowIndex = Math.Max(0, line.StartRowIndex);
                    int endRowIndex = Math.Max(startRowIndex, line.EndRowIndex);

                    Color lineColor = ResolveTraceLineColor(line.LineKind);
                    XLBorderStyleValues borderStyle = XLBorderStyleValues.Medium;

                    for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++)
                    {
                        int excelRow = FirstBodyExcelRow + rowIndex;

                        ApplyBottomBorderRange(
                            ws,
                            excelRow,
                            rightStartCol,
                            rightLastCol,
                            lineColor,
                            borderStyle);
                    }
                }
            }
        }

        private static List<MiddleHorizontalLineDrawInfo> ResolvePreferredMiddleHorizontalLines(
            IList<MiddleHorizontalLineDrawInfo> lines)
        {
            var result = new List<MiddleHorizontalLineDrawInfo>();
            if (lines == null || lines.Count == 0)
                return result;

            var grouped = new Dictionary<int, List<MiddleHorizontalLineDrawInfo>>();

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                List<MiddleHorizontalLineDrawInfo> rowLines;
                if (!grouped.TryGetValue(line.StartRowIndex, out rowLines))
                {
                    rowLines = new List<MiddleHorizontalLineDrawInfo>();
                    grouped[line.StartRowIndex] = rowLines;
                }

                rowLines.Add(line);
            }

            foreach (var rowLines in grouped.Values)
            {
                var preferred = ResolvePreferredMiddleHorizontalLine(rowLines);
                if (preferred != null)
                    result.Add(preferred);
            }

            return result;
        }

        private static MiddleHorizontalLineDrawInfo ResolvePreferredMiddleHorizontalLine(
            List<MiddleHorizontalLineDrawInfo> lines)
        {
            if (lines == null || lines.Count == 0)
                return null;

            MiddleHorizontalLineDrawInfo firstTrunk = null;
            MiddleHorizontalLineDrawInfo firstBranch = null;

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                switch (line.LineKind)
                {
                    case "Start":
                        return line;

                    case "Trunk":
                        if (firstTrunk == null)
                            firstTrunk = line;
                        break;

                    case "Branch":
                        if (firstBranch == null)
                            firstBranch = line;
                        break;
                }
            }

            if (firstTrunk != null)
                return firstTrunk;

            return firstBranch;
        }

        private static Color ResolveTraceLineColor(string lineKind)
        {
            switch (lineKind)
            {
                case "Start":
                    return Color.FromArgb(120, 72, 32);
                case "Trunk":
                    return Color.FromArgb(214, 106, 32);
                case "Branch":
                    return Color.FromArgb(70, 120, 110);
                case "Vartical":
                    return Color.FromArgb(120, 72, 32);
                default:
                    return Color.Black;
            }
        }

        private static bool IsStrongTraceLine(string lineKind)
        {
            if (string.IsNullOrWhiteSpace(lineKind))
                return false;

            return string.Equals(lineKind, "Start", StringComparison.OrdinalIgnoreCase)
                || string.Equals(lineKind, "Trunk", StringComparison.OrdinalIgnoreCase)
                || string.Equals(lineKind, "Orange", StringComparison.OrdinalIgnoreCase)
                || string.Equals(lineKind, "StartGroup", StringComparison.OrdinalIgnoreCase);
        }

        private static void ApplyMiddleLevelVerticalBoundaries(
            IXLWorksheet ws,
            List<ExportColumnPlan> plans,
            int visibleBodyRowCount)
        {
            var levels = GetMiddleLevels(plans);
            if (levels.Count <= 0)
                return;

            int lastExcelRow = Math.Max(ColumnHeaderExcelRow, FirstBodyExcelRow + visibleBodyRowCount - 1);

            for (int i = 0; i < levels.Count; i++)
            {
                int level = levels[i];
                int endCol = GetMiddleLevelLastExcelColumn(plans, level);
                if (endCol <= 0)
                    continue;

                for (int row = GroupHeaderExcelRow; row <= lastExcelRow; row++)
                {
                    var cell = ws.Cell(row, endCol);
                    cell.Style.Border.RightBorder = XLBorderStyleValues.Medium;
                    cell.Style.Border.RightBorderColor = XLColor.FromColor(LevelBoundaryColor);
                }
            }

            int middleStartCol = GetAreaFirstExcelColumn(plans, GridArea.Middle);
            if (middleStartCol > 0)
            {
                for (int row = GroupHeaderExcelRow; row <= lastExcelRow; row++)
                {
                    var cell = ws.Cell(row, middleStartCol);
                    cell.Style.Border.LeftBorder = XLBorderStyleValues.Medium;
                    cell.Style.Border.LeftBorderColor = XLColor.FromColor(LevelBoundaryColor);
                }
            }
        }

        private static void ApplyBottomBorderRange(
    IXLWorksheet ws,
    int excelRow,
    int startCol,
    int endCol,
    Color color,
    XLBorderStyleValues style)
        {
            if (excelRow <= 0 || startCol <= 0 || endCol <= 0)
                return;
            if (endCol < startCol)
                return;

            for (int col = startCol; col <= endCol; col++)
            {
                var cell = ws.Cell(excelRow, col);

                if (!ShouldOverwriteBottomBorder(cell, color, style))
                    continue;

                cell.Style.Border.BottomBorder = style;
                cell.Style.Border.BottomBorderColor = XLColor.FromColor(color);
            }
        }

        private static bool ShouldOverwriteBottomBorder(
            IXLCell cell,
            Color newColor,
            XLBorderStyleValues newStyle)
        {
            if (cell == null)
                return false;

            var border = cell.Style.Border;

            XLBorderStyleValues currentStyle = border.BottomBorder;
            Color currentColor = border.BottomBorderColor.Color;

            // まだ何も構造線が入っていない場合は適用
            if (currentStyle == XLBorderStyleValues.None)
                return true;

            // 通常の既定グリッド線(LightGrayのThin)なら上書きしてよい
            if (currentStyle == XLBorderStyleValues.Thin &&
                currentColor.ToArgb() == DefaultGridBorderColor.ToArgb())
            {
                return true;
            }

            bool currentIsStrongTraceColor = IsStrongTraceLineColor(currentColor);
            bool newIsStrongTraceColor = IsStrongTraceLineColor(newColor);

            // 始点/幹の強調線は枝線で潰さない
            if (currentIsStrongTraceColor && !newIsStrongTraceColor)
                return false;

            // 新規が強調線なら既存の枝線より優先
            if (newIsStrongTraceColor && !currentIsStrongTraceColor)
                return true;

            // 同色系なら太い方を優先
            if ((int)newStyle > (int)currentStyle)
                return true;

            // 同じ強さなら後勝ちにせず現状維持
            return false;
        }

        private static bool IsStrongTraceLineColor(Color color)
        {
            return color.ToArgb() == ResolveTraceLineColor("Start").ToArgb()
                || color.ToArgb() == ResolveTraceLineColor("Trunk").ToArgb();
        }

        private static void ApplyHeaderRowHeights(IXLWorksheet ws, List<ExportColumnPlan> plans)
        {
            ws.Row(GroupHeaderExcelRow).Height = 24d;
            ws.Row(ColumnHeaderExcelRow).Height = GetColumnHeaderRowHeight(plans);
        }

        private static void ApplyColumnWidths(IXLWorksheet ws, List<ExportColumnPlan> plans)
        {
            foreach (var plan in plans)
            {
                double excelWidth = ConvertPixelToExcelColumnWidth(plan.GridColumn.Width);
                ws.Column(plan.ExcelColumnIndex).Width = excelWidth;
            }
        }

        private static void ApplyGroupHeaderStyle(IXLRange range, GridArea area)
        {
            Color backColor;
            Color foreColor;

            switch (area)
            {
                case GridArea.Left:
                    backColor = StartHeaderBackColor;
                    foreColor = StartHeaderForeColor;
                    break;

                case GridArea.Middle:
                    backColor = MiddleHeaderBackColor;
                    foreColor = MiddleHeaderForeColor;
                    break;

                case GridArea.Right:
                    backColor = EndHeaderBackColor;
                    foreColor = EndHeaderForeColor;
                    break;

                default:
                    backColor = Color.Gainsboro;
                    foreColor = Color.Black;
                    break;
            }

            range.Style.Fill.PatternType = XLFillPatternValues.Solid;
            range.Style.Fill.BackgroundColor = XLColor.FromColor(backColor);

            range.Style.Font.Bold = true;
            range.Style.Font.FontName = "Segoe UI";
            range.Style.Font.FontSize = 9;
            range.Style.Font.FontColor = XLColor.FromColor(foreColor);

            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Alignment.WrapText = false;

            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            range.Style.Border.TopBorderColor = XLColor.FromColor(DefaultGridBorderColor);
            range.Style.Border.BottomBorderColor = XLColor.FromColor(LevelBoundaryColor);
            range.Style.Border.LeftBorderColor = XLColor.FromColor(DefaultGridBorderColor);
            range.Style.Border.RightBorderColor = XLColor.FromColor(DefaultGridBorderColor);
        }

        private static void ApplyColumnHeaderStyle(
            IXLCell xlCell,
            DataGridView grid,
            DataGridViewColumn column,
            GridArea area)
        {
            DataGridViewCellStyle style = grid != null
                ? grid.ColumnHeadersDefaultCellStyle
                : null;

            if (style == null)
                style = new DataGridViewCellStyle();

            ApplyCommonStyle(xlCell, style, null, true);

            xlCell.Style.Font.Bold = true;
            xlCell.Style.Alignment.WrapText = true;

            Color foreColor;
            switch (area)
            {
                case GridArea.Left:
                    foreColor = StartHeaderForeColor;
                    break;
                case GridArea.Middle:
                    foreColor = MiddleHeaderForeColor;
                    break;
                case GridArea.Right:
                    foreColor = EndHeaderForeColor;
                    break;
                default:
                    foreColor = Color.Black;
                    break;
            }

            xlCell.Style.Font.FontColor = XLColor.FromColor(foreColor);
            xlCell.Style.Fill.PatternType = XLFillPatternValues.Solid;
            xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(Color.WhiteSmoke);

            ApplyDefaultThinBorder(xlCell);
        }

        private static void ApplyCellStyle(
    IXLCell xlCell,
    DataGridViewCell dgvCell,
    ExportColumnPlan plan,
    ISet<string> crossPointNodeKeys)
        {
            DataGridView grid = plan != null ? plan.Grid : null;

            DataGridViewCellStyle style = dgvCell != null ? dgvCell.InheritedStyle : null;
            if (style == null)
                style = grid != null ? grid.DefaultCellStyle : null;

            ApplyCommonStyle(xlCell, style, grid != null ? grid.DefaultCellStyle : null, false);
            ApplyDefaultThinBorder(xlCell);

            ApplyRuntimeCrossPointBackColor(xlCell, dgvCell, plan, crossPointNodeKeys);
            ApplyRuntimeForeColor(xlCell, dgvCell, plan);
        }

        private static void ApplyRuntimeCrossPointBackColor(
    IXLCell xlCell,
    DataGridViewCell dgvCell,
    ExportColumnPlan plan,
    ISet<string> crossPointNodeKeys)
        {
            if (xlCell == null || dgvCell == null || plan == null ||
                crossPointNodeKeys == null || crossPointNodeKeys.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvCell.OwningRow;
            if (row == null)
                return;

            string columnName = dgvCell.OwningColumn != null
                ? dgvCell.OwningColumn.Name ?? string.Empty
                : string.Empty;

            string nodeKeyColumnName = ResolveNodeKeyColumnNameForForeColorGrouping(columnName);
            if (string.IsNullOrWhiteSpace(nodeKeyColumnName))
                return;

            string nodeKey = GetRowValue(row, nodeKeyColumnName);
            if (string.IsNullOrWhiteSpace(nodeKey) || !crossPointNodeKeys.Contains(nodeKey))
                return;

            xlCell.Style.Fill.PatternType = XLFillPatternValues.Solid;
            xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(GetCrossPointNodeBackColor(nodeKey));
        }

        private static void ApplyIntersectionCrossPointBackColor(
            IXLCell xlCell,
            DataGridViewRow row)
        {
            if (xlCell == null || row == null)
                return;

            if (GetRowInt(row, "交点") != 1)
                return;

            string nodeKey = GetRowValue(row, "NodeKey");
            if (string.IsNullOrWhiteSpace(nodeKey))
                return;

            xlCell.Style.Fill.PatternType = XLFillPatternValues.Solid;
            xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(GetCrossPointNodeBackColor(nodeKey));
        }

        private static void ApplyRuntimeForeColor(
    IXLCell xlCell,
    DataGridViewCell dgvCell,
    ExportColumnPlan plan)
        {
            if (xlCell == null || dgvCell == null || plan == null)
                return;

            DataGridViewRow row = dgvCell.OwningRow;
            if (row == null)
                return;

            string columnName = dgvCell.OwningColumn != null
                ? dgvCell.OwningColumn.Name ?? string.Empty
                : string.Empty;

            string nodeKeyColumnName = ResolveNodeKeyColumnNameForForeColorGrouping(columnName);
            if (!string.IsNullOrWhiteSpace(nodeKeyColumnName))
            {
                string nodeKey = GetRowValue(row, nodeKeyColumnName);
                if (!string.IsNullOrWhiteSpace(nodeKey))
                {
                    Color nodeKeyColor = GetForeColorForNodeKeyGroup(nodeKey);
                    xlCell.Style.Font.FontColor = XLColor.FromColor(nodeKeyColor);
                }
            }

            // 右グリッドの重複重量表示は DimGray 優先
            if (plan.Area == GridArea.Right &&
                string.Equals(columnName, "End_Weight", StringComparison.OrdinalIgnoreCase))
            {
                if (IsDuplicateEndRow(row))
                {
                    xlCell.Style.Font.FontColor = XLColor.FromColor(Color.DimGray);
                }
            }
        }

        private static string ResolveNodeKeyColumnNameForForeColorGrouping(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return null;

            if (columnName.StartsWith("Start_", StringComparison.OrdinalIgnoreCase))
                return "Start_NodeKey";

            if (columnName.StartsWith("End_", StringComparison.OrdinalIgnoreCase))
                return "End_NodeKey";

            if (columnName.StartsWith("Lv", StringComparison.OrdinalIgnoreCase))
            {
                int idx = columnName.IndexOf('_');
                if (idx > 2)
                {
                    string levelText = columnName.Substring(2, idx - 2);

                    int level;
                    if (int.TryParse(levelText, out level))
                        return "Lv" + level + "_NodeKey";
                }
            }

            return null;
        }

        private static bool IsDuplicateEndRow(DataGridViewRow row)
        {
            string value = GetRowValue(row, "End_IsDuplicate");
            if (string.IsNullOrWhiteSpace(value))
                return false;

            bool result;
            if (bool.TryParse(value, out result))
                return result;

            return false;
        }

        private static Color GetForeColorForNodeKeyGroup(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Color.Black;

            int hash = key.GetHashCode();
            if (hash == int.MinValue)
                hash = int.MaxValue;

            hash = Math.Abs(hash);

            double hue = hash % 360;
            double saturation = 0.94;
            double value = 0.42;

            saturation += ((hash / 360) % 6) * 0.01;
            value += ((hash / 3600) % 7) * 0.01;

            if (hue >= 40 && hue <= 85)
            {
                value -= 0.16;
            }
            else if (hue > 85 && hue <= 135)
            {
                value -= 0.08;
            }
            else if (hue >= 170 && hue <= 205)
            {
                value -= 0.10;
            }

            if (value < 0.24) value = 0.24;
            if (value > 0.52) value = 0.52;

            if (saturation < 0.0) saturation = 0.0;
            if (saturation > 1.0) saturation = 1.0;

            return ConvertHsvToColor(hue, saturation, value);
        }

        private static Color GetCrossPointNodeBackColor(string nodeKey)
        {
            int hash = GetStablePositiveHash(nodeKey);
            double hue = hash % 360;
            double saturation = 0.18 + ((hash / 360) % 8) * 0.01;
            double value = 0.98;

            return ConvertHsvToColor(hue, saturation, value);
        }

        private static int GetStablePositiveHash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            unchecked
            {
                int hash = 23;
                foreach (char c in value)
                {
                    hash = hash * 31 + c;
                }

                return hash == int.MinValue ? int.MaxValue : Math.Abs(hash);
            }
        }

        private static Color ConvertHsvToColor(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = (int)value;
            int p = (int)(value * (1 - saturation));
            int q = (int)(value * (1 - f * saturation));
            int t = (int)(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return Color.FromArgb(255, v, t, p);
                case 1: return Color.FromArgb(255, q, v, p);
                case 2: return Color.FromArgb(255, p, v, t);
                case 3: return Color.FromArgb(255, p, q, v);
                case 4: return Color.FromArgb(255, t, p, v);
                default: return Color.FromArgb(255, v, p, q);
            }
        }

        private static void ApplyCommonStyle(
            IXLCell xlCell,
            DataGridViewCellStyle style,
            DataGridViewCellStyle fallbackStyle,
            bool isHeader)
        {
            if (style == null)
                style = fallbackStyle;
            if (style == null)
                style = new DataGridViewCellStyle();

            Color backColor = !IsColorEmptyLike(style.BackColor)
                ? style.BackColor
                : (fallbackStyle != null ? fallbackStyle.BackColor : Color.Empty);

            Color foreColor = !IsColorEmptyLike(style.ForeColor)
                ? style.ForeColor
                : (fallbackStyle != null ? fallbackStyle.ForeColor : Color.Empty);

            Font font = style.Font ?? (fallbackStyle != null ? fallbackStyle.Font : null);

            if (!IsColorEmptyLike(backColor))
            {
                xlCell.Style.Fill.PatternType = XLFillPatternValues.Solid;
                xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(backColor);
            }

            if (!IsColorEmptyLike(foreColor))
            {
                xlCell.Style.Font.FontColor = XLColor.FromColor(foreColor);
            }

            if (font != null)
            {
                xlCell.Style.Font.FontName = font.Name;
                xlCell.Style.Font.FontSize = font.Size;
                xlCell.Style.Font.Bold = font.Bold;
                xlCell.Style.Font.Italic = font.Italic;
                xlCell.Style.Font.Underline = font.Underline ? XLFontUnderlineValues.Single : XLFontUnderlineValues.None;
                xlCell.Style.Font.Strikethrough = font.Strikeout;
            }

            ApplyAlignment(xlCell, style.Alignment);
            xlCell.Style.Alignment.WrapText = style.WrapMode == DataGridViewTriState.True || isHeader;
        }

        private static void ApplyAlignment(IXLCell xlCell, DataGridViewContentAlignment alignment)
        {
            switch (alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    break;
                case DataGridViewContentAlignment.TopCenter:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    break;
                case DataGridViewContentAlignment.TopRight:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    break;
                case DataGridViewContentAlignment.MiddleLeft:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    break;
                case DataGridViewContentAlignment.MiddleCenter:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    break;
                case DataGridViewContentAlignment.MiddleRight:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    break;
                case DataGridViewContentAlignment.BottomLeft:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
                    break;
                case DataGridViewContentAlignment.BottomCenter:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
                    break;
                case DataGridViewContentAlignment.BottomRight:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
                    break;
                case DataGridViewContentAlignment.NotSet:
                default:
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    break;
            }
        }

        private static void ApplySimpleGridHeaderStyle(IXLCell xlCell)
        {
            if (xlCell == null)
                return;

            xlCell.Style.Fill.PatternType = XLFillPatternValues.Solid;
            xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(MiddleHeaderBackColor);
            xlCell.Style.Font.FontColor = XLColor.FromColor(MiddleHeaderForeColor);
            xlCell.Style.Font.Bold = true;
            xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            xlCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            xlCell.Style.Alignment.WrapText = true;
            ApplyDefaultThinBorder(xlCell);
        }

        private static void ApplySimpleGridCellStyle(IXLCell xlCell, DataGridViewCell dgvCell)
        {
            if (xlCell == null)
                return;

            DataGridView grid = dgvCell == null ? null : dgvCell.DataGridView;
            DataGridViewCellStyle style = dgvCell != null ? dgvCell.InheritedStyle : null;
            if (style == null && grid != null)
                style = grid.DefaultCellStyle;

            ApplyCommonStyle(xlCell, style, grid != null ? grid.DefaultCellStyle : null, false);
            ApplyDefaultThinBorder(xlCell);
        }

        private static void ApplyDefaultThinBorder(IXLCell xlCell)
        {
            xlCell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            xlCell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            xlCell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            xlCell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            xlCell.Style.Border.TopBorderColor = XLColor.FromColor(DefaultGridBorderColor);
            xlCell.Style.Border.BottomBorderColor = XLColor.FromColor(DefaultGridBorderColor);
            xlCell.Style.Border.LeftBorderColor = XLColor.FromColor(DefaultGridBorderColor);
            xlCell.Style.Border.RightBorderColor = XLColor.FromColor(DefaultGridBorderColor);
        }

        private static void WriteCellValue(IXLCell xlCell, DataGridViewCell dgvCell)
        {
            if (dgvCell == null)
            {
                xlCell.Value = string.Empty;
                return;
            }

            object value = dgvCell.FormattedValue;
            if (value == null || value == DBNull.Value)
            {
                xlCell.Value = string.Empty;
                return;
            }

            xlCell.Value = value.ToString();
        }

        private static double GetColumnHeaderRowHeight(List<ExportColumnPlan> plans)
        {
            if (plans == null || plans.Count == 0)
                return 22d;

            DataGridView grid = plans[0].Grid;
            if (grid == null)
                return 22d;

            return ConvertPixelToExcelRowHeight(grid.ColumnHeadersHeight);
        }

        private static double ConvertPixelToExcelColumnWidth(int pixelWidth)
        {
            if (pixelWidth <= 0)
                return 8.43d;

            double width = (pixelWidth - 5d) / 7d;
            if (width < 1d)
                width = 1d;

            return width;
        }

        private static double ConvertPixelToExcelRowHeight(int pixelHeight)
        {
            if (pixelHeight <= 0)
                return 15d;

            return Math.Round(pixelHeight * 0.75d, 2, MidpointRounding.AwayFromZero);
        }

        private static DataGridViewRow GetBaseVisibleRow(VisibleRowMaps visibleRows, int visibleRowIndex)
        {
            DataGridViewRow row = GetVisibleRowSafe(visibleRows.LeftRows, visibleRowIndex);
            if (row != null)
                return row;

            row = GetVisibleRowSafe(visibleRows.MiddleRows, visibleRowIndex);
            if (row != null)
                return row;

            return GetVisibleRowSafe(visibleRows.RightRows, visibleRowIndex);
        }

        private static DataGridViewRow GetVisibleRowByArea(VisibleRowMaps visibleRows, GridArea area, int visibleRowIndex)
        {
            switch (area)
            {
                case GridArea.Left:
                    return GetVisibleRowSafe(visibleRows.LeftRows, visibleRowIndex);
                case GridArea.Middle:
                    return GetVisibleRowSafe(visibleRows.MiddleRows, visibleRowIndex);
                case GridArea.Right:
                    return GetVisibleRowSafe(visibleRows.RightRows, visibleRowIndex);
                default:
                    return null;
            }
        }

        private static DataGridViewRow GetVisibleRowSafe(List<DataGridViewRow> rows, int index)
        {
            if (rows == null)
                return null;
            if (index < 0 || index >= rows.Count)
                return null;
            return rows[index];
        }

        private static string GetRowValue(DataGridViewRow row, string columnName)
        {
            if (row == null || string.IsNullOrEmpty(columnName))
                return null;

            try
            {
                var drv = row.DataBoundItem as DataRowView;
                if (drv != null)
                {
                    if (drv.Row != null && drv.Row.Table != null && drv.Row.Table.Columns.Contains(columnName))
                    {
                        object value = drv.Row[columnName];
                        return value == null || value == DBNull.Value
                            ? null
                            : Convert.ToString(value, CultureInfo.InvariantCulture);
                    }
                }

                if (row.DataGridView != null && row.DataGridView.Columns.Contains(columnName))
                {
                    object value = row.Cells[columnName].Value;
                    return value == null || value == DBNull.Value
                        ? null
                        : Convert.ToString(value, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                // 検証用出力なのでここでは null 扱い
            }

            return null;
        }

        private static int GetRowInt(DataGridViewRow row, string columnName)
        {
            string value = GetRowValue(row, columnName);
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            int result;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)
                ? result
                : 0;
        }

        private static bool StringEquals(string a, string b)
        {
            return string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsColorEmptyLike(Color color)
        {
            return color.IsEmpty || color == Color.Empty || color.A == 0;
        }

        private static string NullToEmpty(string s)
        {
            return s ?? string.Empty;
        }

        private static string SanitizeWorksheetName(string worksheetName)
        {
            string name = string.IsNullOrWhiteSpace(worksheetName) ? "TraceResult" : worksheetName.Trim();

            char[] invalid = new[] { ':', '\\', '/', '?', '*', '[', ']' };
            foreach (char c in invalid)
            {
                name = name.Replace(c.ToString(CultureInfo.InvariantCulture), "_");
            }

            if (name.Length > 31)
                name = name.Substring(0, 31);

            if (string.IsNullOrWhiteSpace(name))
                name = "TraceResult";

            return name;
        }

        private static string GetUniqueWorksheetName(XLWorkbook wb, string worksheetName)
        {
            string baseName = SanitizeWorksheetName(worksheetName);
            string name = baseName;
            int suffix = 2;

            while (wb.Worksheets.Contains(name))
            {
                string suffixText = "_" + suffix.ToString(CultureInfo.InvariantCulture);
                int maxBaseLength = 31 - suffixText.Length;
                string trimmedBase = baseName.Length > maxBaseLength
                    ? baseName.Substring(0, maxBaseLength)
                    : baseName;

                name = trimmedBase + suffixText;
                suffix++;
            }

            return name;
        }

        private static int GetAreaFirstExcelColumn(List<ExportColumnPlan> plans, GridArea area)
        {
            int result = 0;
            foreach (var plan in plans)
            {
                if (plan.Area != area)
                    continue;

                if (result == 0 || plan.ExcelColumnIndex < result)
                    result = plan.ExcelColumnIndex;
            }
            return result;
        }

        private static int GetAreaLastExcelColumn(List<ExportColumnPlan> plans, GridArea area)
        {
            int result = 0;
            foreach (var plan in plans)
            {
                if (plan.Area != area)
                    continue;

                if (plan.ExcelColumnIndex > result)
                    result = plan.ExcelColumnIndex;
            }
            return result;
        }

        private static int GetMiddleLevelFirstExcelColumn(List<ExportColumnPlan> plans, int level)
        {
            int result = 0;
            foreach (var plan in plans)
            {
                if (plan.Area != GridArea.Middle)
                    continue;
                if (plan.MiddleLevel != level)
                    continue;

                if (result == 0 || plan.ExcelColumnIndex < result)
                    result = plan.ExcelColumnIndex;
            }
            return result;
        }

        private static int GetMiddleLevelLastExcelColumn(List<ExportColumnPlan> plans, int level)
        {
            int result = 0;
            foreach (var plan in plans)
            {
                if (plan.Area != GridArea.Middle)
                    continue;
                if (plan.MiddleLevel != level)
                    continue;

                if (plan.ExcelColumnIndex > result)
                    result = plan.ExcelColumnIndex;
            }
            return result;
        }

        private static int GetMaxMiddleLevel(List<ExportColumnPlan> plans)
        {
            int max = 0;
            foreach (var plan in plans)
            {
                if (plan.Area != GridArea.Middle)
                    continue;

                if (plan.MiddleLevel > max)
                    max = plan.MiddleLevel;
            }
            return max;
        }

        private static List<int> GetMiddleLevels(List<ExportColumnPlan> plans)
        {
            var result = new List<int>();
            var map = new HashSet<int>();

            foreach (var plan in plans)
            {
                if (plan.Area != GridArea.Middle)
                    continue;
                if (plan.MiddleLevel <= 0)
                    continue;

                if (map.Add(plan.MiddleLevel))
                {
                    result.Add(plan.MiddleLevel);
                }
            }

            result.Sort();
            return result;
        }

        private static int TryGetMiddleLevel(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return 0;

            Match m = Regex.Match(columnName, @"^Lv(\d+)_", RegexOptions.IgnoreCase);
            if (!m.Success)
                return 0;

            int level;
            if (!int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out level))
                return 0;

            return level;
        }

        private static string BuildLeftGroupHeaderText(DataGridView grid)
        {
            return string.Format("検索始点[{0}]", GetDisplayedRowCount(grid));
        }

        private static string BuildRightGroupHeaderText(DataGridView grid)
        {
            return string.Format("検索終点[{0}]", GetDisplayedRowCount(grid));
        }

        private static string BuildMiddleGroupHeaderText(DataGridView grid, int level)
        {
            return string.Format("中間工程{0}[{1}]", level, GetMiddleLevelDisplayedRowCount(grid, level));
        }

        private static int GetDisplayedRowCount(DataGridView grid)
        {
            if (grid == null)
                return 0;

            int count = 0;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                bool hasDisplayedValue = false;

                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (col == null || !col.Visible)
                        continue;

                    object value = row.Cells[col.Index].Value;
                    if (value == null || value == DBNull.Value)
                        continue;

                    string text = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        hasDisplayedValue = true;
                        break;
                    }
                }

                if (hasDisplayedValue)
                    count++;
            }

            return count;
        }

        private static int GetMiddleLevelDisplayedRowCount(DataGridView grid, int level)
        {
            if (grid == null)
                return 0;

            var targetColumns = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                int lv = TryGetMiddleLevel(col.Name);
                if (lv != level)
                    continue;

                targetColumns.Add(col);
            }

            if (targetColumns.Count == 0)
                return 0;

            int count = 0;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                bool hasDisplayedValue = false;

                foreach (DataGridViewColumn col in targetColumns)
                {
                    object value = row.Cells[col.Index].Value;
                    if (value == null || value == DBNull.Value)
                        continue;

                    string text = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        hasDisplayedValue = true;
                        break;
                    }
                }

                if (hasDisplayedValue)
                    count++;
            }

            return count;
        }

        private enum GridArea
        {
            Left,
            Middle,
            Right
        }

        private sealed class ExportColumnPlan
        {
            public DataGridView Grid { get; set; }
            public GridArea Area { get; set; }
            public DataGridViewColumn GridColumn { get; set; }
            public int ExcelColumnIndex { get; set; }
            public int MiddleLevel { get; set; }
        }

        private sealed class VisibleRowMaps
        {
            public List<DataGridViewRow> LeftRows { get; set; }
            public List<DataGridViewRow> MiddleRows { get; set; }
            public List<DataGridViewRow> RightRows { get; set; }
            public int MaxVisibleRowCount { get; set; }
        }
    }
}
