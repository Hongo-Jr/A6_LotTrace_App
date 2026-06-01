using DocumentFormat.OpenXml.Office2021.MipLabelMetaData;

using LotTraceApp.Models;
using LotTraceApp.Services;
using LotTraceApp.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows.Forms;

namespace LotTraceApp
{
    public partial class BottleTraceForm : Form
    {
        private readonly BottleTraceService _service;

        // タブ番号 → 検索条件。MainForm と同じく、タブごとの状態を保持する。
        private readonly Dictionary<int, TraceSearchParameters> _tabSearchParameters =
            new Dictionary<int, TraceSearchParameters>();

        // タブ番号 → 表示テーブル。サービス実装接続後、タブ切替時の再表示に使う。
        private readonly Dictionary<int, BottleDisplayTables> _tabDisplayTables =
            new Dictionary<int, BottleDisplayTables>();

        // タブ番号 → UI コントロール一式。
        private readonly Dictionary<int, BottleTraceTabContext> _bottleTraceTabContexts =
            new Dictionary<int, BottleTraceTabContext>();


        private List<LineCache> _lineCache = new List<LineCache>();  

        private sealed class LineCache
        {
            public int RowIndex {  get; set; }
            public Color Color {  get; set; }

        }


        

        private sealed class HeaderVisualStyle
        {
            public Color GroupBackColor { get; set; }
            public Color GroupForeColor { get; set; }
            public Color BorderColor { get; set; }
            public Font GroupFont { get; set; }
            public Font ColumnFont { get; set; }
        }

        private readonly HeaderVisualStyle _startHeaderStyle = new HeaderVisualStyle
        {
            GroupBackColor = Color.FromArgb(235, 242, 250),
            GroupForeColor = Color.FromArgb(40, 40, 40),
            BorderColor = Color.FromArgb(180, 180, 180),
            GroupFont = new Font("Segoe UI", 9F, FontStyle.Bold),
            ColumnFont = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        private readonly HeaderVisualStyle _endHeaderStyle = new HeaderVisualStyle
        {
            GroupBackColor = Color.FromArgb(250, 238, 238),
            GroupForeColor = Color.FromArgb(40, 40, 40),
            BorderColor = Color.FromArgb(180, 180, 180),
            GroupFont = new Font("Segoe UI", 9F, FontStyle.Bold),
            ColumnFont = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        private sealed class BottleTraceTabContext
        {
            public int TabNo { get; set; }

            public TextBox TxtOrder { get; set; }
            public TextBox TxtItemName { get; set; }
            public TextBox TxtItemCode { get; set; }
            public TextBox TxtLot { get; set; }

            public CheckBox ChkUsePeriod { get; set; }
            public DateTimePicker DtpFrom { get; set; }
            public DateTimePicker DtpTo { get; set; }

            public RadioButton RdoForward { get; set; }
            public RadioButton RdoBackward { get; set; }

            public Button BtnSearch { get; set; }
            public Button BtnClear { get; set; }
            public Button BtnCsv { get; set; }

            public DataGridView GridStart { get; set; }
            public DataGridView GridEnd { get; set; }
        }

        public BottleTraceForm(BottleTraceService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();
            ConfigureBottleTraceTabOwnerDraw();

            InitializeBottleTraceTabContexts();
            InitializeBottleTraceTabs();

            swichBottleTab.SelectedIndexChanged -= SwichBottleTab_SelectedIndexChanged;
            swichBottleTab.SelectedIndexChanged += SwichBottleTab_SelectedIndexChanged;
        }

        private void ConfigureBottleTraceTabOwnerDraw()
        {
            swichBottleTab.DrawMode = TabDrawMode.OwnerDrawFixed;
            swichBottleTab.DrawItem -= SwichBottleTab_DrawItem;
            swichBottleTab.DrawItem += SwichBottleTab_DrawItem;
        }

        private void SwichBottleTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null || e.Index < 0 || e.Index >= tabControl.TabPages.Count)
                return;

            var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var fillBounds = e.Bounds;
            fillBounds.Inflate(-1, -1);
            using (var brush = new SolidBrush(selected ? Color.White : Color.FromArgb(242, 242, 242)))
            {
                e.Graphics.FillRectangle(brush, fillBounds);
            }

            var tabPage = tabControl.TabPages[e.Index];
            var textBounds = e.Bounds;
            textBounds.Inflate(-8, 0);

            var foreColor = selected ? tabControl.ForeColor : tabPage.ForeColor;
            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                tabControl.Font,
                textBounds,
                foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);

            e.DrawFocusRectangle();
        }

        private int GetCurrentBottleTraceTabNo()
        {
            int tabNo = swichBottleTab.SelectedIndex + 1;
            return (tabNo >= 1 && tabNo <= 10) ? tabNo : -1;
        }

        private BottleTraceTabContext GetTabContext(int tabNo)
        {
            BottleTraceTabContext tab;
            return _bottleTraceTabContexts.TryGetValue(tabNo, out tab) ? tab : null;
        }

        private BottleTraceTabContext GetCurrentTabContext()
        {
            int tabNo = GetCurrentBottleTraceTabNo();
            return tabNo > 0 ? GetTabContext(tabNo) : null;
        }

        private void InitializeBottleTraceTabContexts()
        {
            _bottleTraceTabContexts.Clear();

            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = CreateBottleTraceTabContext(tabNo);
                if (tab != null)
                    _bottleTraceTabContexts[tabNo] = tab;
            }
        }

        private BottleTraceTabContext CreateBottleTraceTabContext(int tabNo)
        {
            switch (tabNo)
            {
                case 1:
                    return new BottleTraceTabContext
                    {
                        TabNo = 1,
                        TxtOrder = txtBottoleOrderNo,
                        TxtItemName = txtBottoleItemName,
                        TxtItemCode = txtBottoleItemCode,
                        TxtLot = txtBottoleLotNo,
                        ChkUsePeriod = timeCheck,
                        DtpFrom = startBottoleTime,
                        DtpTo = endBottoleTime,
                        RdoForward = rdoForwardBottle,
                        RdoBackward = rdoBackwardBottle,
                        BtnSearch = btnBottoleTraceSearch,
                        BtnClear = btnClearBottle,
                        BtnCsv = btnCsvOutputBottle,
                        GridStart = dgvStartBottle,
                        GridEnd = dgvEndBottle
                    };

                case 2:
                    return new BottleTraceTabContext
                    {
                        TabNo = 2,
                        TxtOrder = txtBottoleOrderNo_2,
                        TxtItemName = txtBottoleItemName_2,
                        TxtItemCode = txtBottoleItemCode_2,
                        TxtLot = txtBottoleLotNo_2,
                        ChkUsePeriod = timeCheck_2,
                        DtpFrom = startBottoleTime_2,
                        DtpTo = endBottoleTime_2,
                        RdoForward = rdoForwardBottle_2,
                        RdoBackward = rdoBackwardBottle_2,
                        BtnSearch = btnBottoleTraceSearch_2,
                        BtnClear = btnClearBottole_2,
                        BtnCsv = btnCsvOutputBottole_2,
                        GridStart = dgvStartBottle_2,
                        GridEnd = dgvEndBottle_2
                    };

                case 3:
                    return new BottleTraceTabContext
                    {
                        TabNo = 3,
                        TxtOrder = txtBottoleOrderNo_3,
                        TxtItemName = txtBottoleItemName_3,
                        TxtItemCode = txtBottoleItemCode_3,
                        TxtLot = txtBottoleLotNo_3,
                        ChkUsePeriod = timeCheck_3,
                        DtpFrom = startBottoleTime_3,
                        DtpTo = endBottoleTime_3,
                        RdoForward = rdoForwardBottle_3,
                        RdoBackward = rdoBackwardBottle_3,
                        BtnSearch = btnTraceSearch_3,
                        BtnClear = btnClearBottle_3,
                        BtnCsv = btnCsvOutputBottle_3,
                        GridStart = dgvStartBottle_3,
                        GridEnd = dgvEndBottle_3
                    };

                case 4:
                    return new BottleTraceTabContext
                    {
                        TabNo = 4,
                        TxtOrder = txtBottoleOrderNo_4,
                        TxtItemName = textBox7,
                        TxtItemCode = txtBottoleItemCode_4,
                        TxtLot = txtBottoleLotNo_4,
                        ChkUsePeriod = timeCheck_4,
                        DtpFrom = startBottoleTime_4,
                        DtpTo = endBottoleTime_4,
                        RdoForward = rdoForwardBottle_4,
                        RdoBackward = rdoBackwardBottle_4,
                        BtnSearch = btnTraceSearch_4,
                        BtnClear = btnClearBottle_4,
                        BtnCsv = btnCsvOutputBottle_4,
                        GridStart = dgvStartBottle_4,
                        GridEnd = dgvEndBottle_4
                    };

                case 5:
                    return new BottleTraceTabContext
                    {
                        TabNo = 5,
                        TxtOrder = txtBottoleOrderNo_5,
                        TxtItemName = txtBottoleItemName_5,
                        TxtItemCode = txtBottoleItemCode_5,
                        TxtLot = txtBottoleLotNo_5,
                        ChkUsePeriod = timeCheck_5,
                        DtpFrom = startBottoleTime_5,
                        DtpTo = endBottoleTime_5,
                        RdoForward = rdoForwardBottle_5,
                        RdoBackward = rdoBackwardBottle_5,
                        BtnSearch = btnTraceSearch_5,
                        BtnClear = btnClearBottle_5,
                        BtnCsv = btnCsvOutputBottle_5,
                        GridStart = dgvStartBottle_5,
                        GridEnd = dgvEndBottle_5
                    };

                case 6:
                    return new BottleTraceTabContext
                    {
                        TabNo = 6,
                        TxtOrder = txtBottoleOrderNo_6,
                        TxtItemName = txtBottoleItemName_6,
                        TxtItemCode = txtBottoleItemCode_6,
                        TxtLot = txtBottoleLotNo_6,
                        ChkUsePeriod = timeCheck_6,
                        DtpFrom = startBottoleTime_6,
                        DtpTo = endBottoleTime_6,
                        RdoForward = rdoForwardBottle_6,
                        RdoBackward = rdoBackwardBottle_6,
                        BtnSearch = btnTraceSearch_6,
                        BtnClear = btnClearBottle_6,
                        BtnCsv = btnCsvOutputBottle_6,
                        GridStart = dgvStartBottle_6,
                        GridEnd = dgvEndBottle_6
                    };

                case 7:
                    return new BottleTraceTabContext
                    {
                        TabNo = 7,
                        TxtOrder = txtBottoleOrderNo_7,
                        TxtItemName = txtBottoleItemName_7,
                        TxtItemCode = txtBottoleItemCode_7,
                        TxtLot = txtBottoleLotNo_7,
                        ChkUsePeriod = timeCheck_7,
                        DtpFrom = startBottoleTime_7,
                        DtpTo = endBottoleTime_7,
                        RdoForward = rdoForwardBottle_7,
                        RdoBackward = rdoBackwardBottle_7,
                        BtnSearch = btnTraceSearch_7,
                        BtnClear = btnClearBottle_7,
                        BtnCsv = btnCsvOutputBottle_7,
                        GridStart = dgvStartBottle_7,
                        GridEnd = dgvEndBottle_7
                    };

                case 8:
                    return new BottleTraceTabContext
                    {
                        TabNo = 8,
                        TxtOrder = txtBottoleOrderNo_8,
                        TxtItemName = txtBottoleItemName_8,
                        TxtItemCode = txtBottoleItemCode_8,
                        TxtLot = txtBottoleLotNo_8,
                        ChkUsePeriod = timeCheck_8,
                        DtpFrom = startBottoleTime_8,
                        DtpTo = endBottoleTime_8,
                        RdoForward = rdoForwardBottle_8,
                        RdoBackward = rdoBackwardBottle_8,
                        BtnSearch = btnTraceSearch_8,
                        BtnClear = btnClearBottle_8,
                        BtnCsv = btnCsvOutputBottle_8,
                        GridStart = dgvStartBottle_8,
                        GridEnd = dgvEndBottle_8
                    };

                case 9:
                    return new BottleTraceTabContext
                    {
                        TabNo = 9,
                        TxtOrder = txtBottoleOrderNo_9,
                        TxtItemName = txtBottoleItemName_9,
                        TxtItemCode = txtBottoleItemCode_9,
                        TxtLot = txtBottoleLotNo_9,
                        ChkUsePeriod = timeCheck_9,
                        DtpFrom = startBottoleTime_9,
                        DtpTo = endBottoleTime_9,
                        RdoForward = rdoForwardBottle_9,
                        RdoBackward = rdoBackwardBottle_9,
                        BtnSearch = btnTraceSearch_9,
                        BtnClear = btnClearBottle_9,
                        BtnCsv = btnCsvOutputBottle_9,
                        GridStart = dgvStartBottle_9,
                        GridEnd = dgvEndBottle_9
                    };

                case 10:
                    return new BottleTraceTabContext
                    {
                        TabNo = 10,
                        TxtOrder = textBox10,
                        TxtItemName = txtBottoleItemName_10,
                        TxtItemCode = txtBottoleItemCode_10,
                        TxtLot = txtBottoleLotNo_10,
                        ChkUsePeriod = timeCheck_10,
                        DtpFrom = dateTimePicker5,
                        DtpTo = endBottoleTime_10,
                        RdoForward = rdoForwardBottle_10,
                        RdoBackward = radioButton5,
                        BtnSearch = btnTraceSearch_10,
                        BtnClear = btnClearBottle_10,
                        BtnCsv = btnCsvOutputBottle_10,
                        GridStart = dgvStartBottle_10,
                        GridEnd = dgvEndBottle_10
                    };

                default:
                    return null;
            }
        }

        private void InitializeBottleTraceTabs()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null) continue;

                InitializeGrid(tab.GridStart);
                InitializeGrid(tab.GridEnd);
                ApplyGridColumnHeaderStyle(tab.GridStart, _startHeaderStyle);
                ApplyGridColumnHeaderStyle(tab.GridEnd, _endHeaderStyle);
                tab.GridStart.ScrollBars = ScrollBars.None;
                tab.GridEnd.ScrollBars = ScrollBars.Vertical;
                InitializeBottleHeaderPanel(tab.GridStart, "検索始点", _startHeaderStyle);
                InitializeBottleHeaderPanel(tab.GridEnd, "検索終点", _endHeaderStyle);

                RegisterTraceGridEvents(tab);

                tab.BtnSearch.Click -= TraceSearch_FromAnyTab_Click;
                tab.BtnSearch.Click += TraceSearch_FromAnyTab_Click;

                tab.BtnClear.Click -= Clear_FromAnyTab_Click;
                tab.BtnClear.Click += Clear_FromAnyTab_Click;

                tab.BtnCsv.Click -= Csv_FromAnyTab_Click;
                tab.BtnCsv.Click += Csv_FromAnyTab_Click;

                tab.RdoForward.CheckedChanged -= TraceDirectionChanged;
                tab.RdoForward.CheckedChanged += TraceDirectionChanged;

                tab.RdoBackward.CheckedChanged -= TraceDirectionChanged;
                tab.RdoBackward.CheckedChanged += TraceDirectionChanged;
            }
        }

        private void InitializeGrid(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.AutoGenerateColumns = true;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToOrderColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToResizeColumns = false;

            grid.RowHeadersVisible = false;   // ← 左の矢印列を消す
            grid.ColumnHeadersVisible = true;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            grid.ScrollBars = ScrollBars.None; // ← 既定は出さない
            grid.RowTemplate.Height = 22;

            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            //grid.BackgroundColor = Color.FromArgb(96, 100, 105);
            grid.GridColor = Color.FromArgb(176, 180, 184);
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ShowCellToolTips = false;
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            //grid.KeyDown -= Grid_KeyDown_CopyCurrentCell;
            //grid.KeyDown += Grid_KeyDown_CopyCurrentCell;
        }

        private void ApplyGridColumnHeaderStyle(DataGridView grid, HeaderVisualStyle style)
        {
            if (grid == null || style == null)
                return;

            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = style.GroupBackColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = style.GroupForeColor;
            grid.ColumnHeadersDefaultCellStyle.Font = style.ColumnFont;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = style.GroupBackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = style.GroupForeColor;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(2, 4, 2, 4);

            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 30;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        private void InitializeBottleHeaderPanel(DataGridView grid, string title, HeaderVisualStyle style)
        {
            Panel panel = FindHeaderPanelForGrid(grid);
            if (panel == null || style == null)
                return;

            panel.Controls.Clear();
            panel.BackColor = style.GroupBackColor;
            panel.BorderStyle = BorderStyle.FixedSingle;

            var label = new Label();
            label.Name = "lbl" + grid.Name + "HeaderTitle";
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = style.GroupBackColor;
            label.ForeColor = style.GroupForeColor;
            label.Font = style.GroupFont;
            label.Margin = Padding.Empty;
            label.Padding = Padding.Empty;

            panel.Controls.Add(label);
            RefreshBottleHeaderPanel(grid, title);
        }

        private void RefreshBottleHeaderPanels(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            RefreshBottleHeaderPanel(tab.GridStart, "検索始点");
            RefreshBottleHeaderPanel(tab.GridEnd, "検索終点");
        }

        private void RefreshBottleHeaderPanel(DataGridView grid, string title)
        {
            Panel panel = FindHeaderPanelForGrid(grid);
            if (panel == null)
                return;

            if (panel.Controls.Count == 0 || !(panel.Controls[0] is Label))
            {
                InitializeBottleHeaderPanel(
                    grid,
                    title,
                    title == "検索始点" ? _startHeaderStyle : _endHeaderStyle);
                return;
            }

            panel.Controls[0].Text = BuildPanelHeaderText(title, grid);
            panel.Width = grid.Width;
            panel.Invalidate();
        }

        private Panel FindHeaderPanelForGrid(DataGridView grid)
        {
            if (grid == null || grid.Parent == null)
                return null;

            foreach (Control control in grid.Parent.Controls)
            {
                Panel panel = control as Panel;
                if (panel == null)
                    continue;

                if (panel.Left == grid.Left && panel.Top + panel.Height == grid.Top)
                    return panel;
            }

            return null;
        }

        private string BuildPanelHeaderText(string title, DataGridView grid)
        {
            return string.Format("{0}[{1}]", title, GetDisplayedRowCount(grid));
        }

        private int GetDisplayedRowCount(DataGridView grid)
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

                    string text = Convert.ToString(value);
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

        private void ApplyBottleGridWidthsFromColumns(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            FitGridAndHeaderToColumns(tab.GridStart, false);
            AlignEndGridToStartGrid(tab, 50);
            FitGridAndHeaderToColumns(tab.GridEnd, true);
        }

        private void AlignEndGridToStartGrid(BottleTraceTabContext tab, int gap)
        {
            if (tab == null || tab.GridStart == null || tab.GridEnd == null)
                return;

            int left = tab.GridStart.Right + gap;
            Panel endHeaderPanel = FindHeaderPanelForGrid(tab.GridEnd);

            tab.GridEnd.Left = left;

            if (endHeaderPanel != null)
            {
                endHeaderPanel.Left = left;
                endHeaderPanel.Invalidate();
            }
        }

        private void FitGridAndHeaderToColumns(DataGridView grid, bool includeVerticalScrollBarWhenNeeded, int pad = 4)
        {
            if (grid == null)
                return;

            int columnsWidth = GetVisibleColumnsWidth(grid);
            if (columnsWidth <= 0)
                return;

            int targetWidth = columnsWidth + pad;
            if (includeVerticalScrollBarWhenNeeded && NeedsVerticalScrollBar(grid))
                targetWidth += SystemInformation.VerticalScrollBarWidth;

            if (targetWidth < 10)
                targetWidth = 10;

            grid.Width = targetWidth;

            Panel panel = FindHeaderPanelForGrid(grid);
            if (panel != null)
            {
                panel.Left = grid.Left;
                panel.Width = targetWidth;
                panel.Invalidate();
            }

            grid.Invalidate();
        }

        private int GetVisibleColumnsWidth(DataGridView grid)
        {
            if (grid == null)
                return 0;

            int width = 0;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                width += col.Width;
            }

            return width;
        }

        private bool NeedsVerticalScrollBar(DataGridView grid)
        {
            if (grid == null)
                return false;

            int rowsHeight = grid.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            int headerHeight = grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight : 0;
            int availableHeight = grid.ClientSize.Height;

            return rowsHeight + headerHeight > availableHeight;
        }

        private void RegisterTraceGridEvents(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            UnregisterTraceGridBorderPaint(tab.GridStart);
            UnregisterTraceGridBorderPaint(tab.GridEnd);

            if (tab.RdoForward != null && tab.RdoForward.Checked)
            {
                RegisterTraceGridEvents(tab.GridStart, LiqidTableBorderPaint);
                RegisterTraceGridEvents(tab.GridEnd, BottleTableBorderPaint);
            }
        }

        private void RegisterTraceGridEvents(DataGridView grid, PaintEventHandler paintHandler)
        {
            if (grid == null || paintHandler == null)
                return;

            grid.Paint -= paintHandler;
            grid.Paint += paintHandler;
        }

        private void UnregisterTraceGridBorderPaint(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.Paint -= LiqidTableBorderPaint;
            grid.Paint -= BottleTableBorderPaint;
        }

        private void TraceDirectionChanged(object sender, EventArgs e)
        {
            RefreshTraceGridEventAssignments();
        }

        private void RefreshTraceGridEventAssignments()
        {
            foreach (var tab in _bottleTraceTabContexts.Values)
            {
                RegisterTraceGridEvents(tab);
            }
        }



        private void SetForwardGrid(BottleTraceTabContext tab, BottleDisplayTables tables)
        {
            //液

            tab.GridStart.DataSource = tables.LiquidTable;

            tab.GridStart.Columns["OrderNumber"].Visible = true;
            tab.GridStart.Columns["OrderNumber"].HeaderText = tables.LiquidTable.Columns["OrderNumber"].Caption;
            tab.GridStart.Columns["OrderNumber"].Width = 120;

            tab.GridStart.Columns["Lot"].Visible = true;
            tab.GridStart.Columns["Lot"].HeaderText = tables.LiquidTable.Columns["Lot"].Caption;
            tab.GridStart.Columns["Lot"].Width = 120;

            tab.GridStart.Columns["ItemName"].Visible = true;
            tab.GridStart.Columns["ItemName"].HeaderText = tables.LiquidTable.Columns["ItemName"].Caption;
            tab.GridStart.Columns["ItemName"].Width = 120;

            tab.GridStart.Columns["StartDate"].Visible = true;
            tab.GridStart.Columns["StartDate"].HeaderText = tables.LiquidTable.Columns["StartDate"].Caption;
            tab.GridStart.Columns["StartDate"].Width = 150;

            tab.GridStart.Columns["Weight"].Visible = true;
            tab.GridStart.Columns["Weight"].HeaderText = tables.LiquidTable.Columns["Weight"].Caption;
            tab.GridStart.Columns["Weight"].Width = 120;

            tab.GridStart.Columns["NodeKey"].Visible = false;
            tab.GridStart.Columns["NodeKey"].Width = 0;

            tab.GridStart.Columns["DisplayKey"].Visible = false;
            tab.GridStart.Columns["DisplayKey"].Width = 0;

            //瓶

            tab.GridEnd.DataSource = tables.BottleTable;

            tab.GridEnd.Columns["OrderNumber"].Visible = true;
            tab.GridEnd.Columns["OrderNumber"].HeaderText = tables.BottleTable.Columns["OrderNumber"].Caption;
            tab.GridEnd.Columns["OrderNumber"].Width = 120;

            tab.GridEnd.Columns["Lot"].Visible = true;
            tab.GridEnd.Columns["Lot"].HeaderText = tables.BottleTable.Columns["Lot"].Caption;
            tab.GridEnd.Columns["Lot"].Width = 120;

            tab.GridEnd.Columns["ItemName"].Visible = true;
            tab.GridEnd.Columns["ItemName"].HeaderText = tables.BottleTable.Columns["ItemName"].Caption;
            tab.GridEnd.Columns["ItemName"].Width = 120;

            tab.GridEnd.Columns["StartDate"].Visible = true;
            tab.GridEnd.Columns["StartDate"].HeaderText = tables.BottleTable.Columns["StartDate"].Caption;
            tab.GridEnd.Columns["StartDate"].Width = 150;

            tab.GridEnd.Columns["OK_Num"].Visible = true;
            tab.GridEnd.Columns["OK_Num"].HeaderText = tables.BottleTable.Columns["OK_Num"].Caption;
            tab.GridEnd.Columns["OK_Num"].Width = 120;

            tab.GridEnd.Columns["NG_Num"].Visible = true;
            tab.GridEnd.Columns["NG_Num"].HeaderText = tables.BottleTable.Columns["NG_Num"].Caption;
            tab.GridEnd.Columns["NG_Num"].Width = 120;

            tab.GridEnd.Columns["Total_Num"].Visible = true;
            tab.GridEnd.Columns["Total_Num"].HeaderText = tables.BottleTable.Columns["Total_Num"].Caption;
            tab.GridEnd.Columns["Total_Num"].Width = 120;

           

            tab.GridEnd.Columns["NodeKey"].Visible = false;
            tab.GridEnd.Columns["NodeKey"].Width = 0;

            tab.GridEnd.Columns["DisplayKey"].Visible = false;
            tab.GridEnd.Columns["DisplayKey"].Width = 0;

            ApplyBottleGridWidthsFromColumns(tab);
            RefreshBottleHeaderPanels(tab);
        }

        private TraceSearchParameters CollectSearchParametersFromControls(BottleTraceTabContext tab)
        {
            var p = new TraceSearchParameters();
            p.ProductionOrderNumber = tab.TxtOrder.Text.Trim();
            p.ItemName = tab.TxtItemName.Text.Trim();
            p.ItemCode = tab.TxtItemCode.Text.Trim();
            p.LotNumber = tab.TxtLot.Text.Trim();

            if (tab.ChkUsePeriod.Checked)
            {
                p.From = tab.DtpFrom.Value.Date;
                p.To = tab.DtpTo.Value.Date.AddDays(1).AddTicks(-1);
            }

            p.Direction = tab.RdoForward.Checked ? TraceDirection.Forward : TraceDirection.Backward;
            return p;
        }

        private bool HasAnySearchCondition(TraceSearchParameters p)
        {
            if (p == null)
                return false;

            return !string.IsNullOrWhiteSpace(p.ProductionOrderNumber)
                || !string.IsNullOrWhiteSpace(p.ItemName)
                || !string.IsNullOrWhiteSpace(p.ItemCode)
                || !string.IsNullOrWhiteSpace(p.LotNumber)
                || p.From.HasValue
                || p.To.HasValue;
        }

        private void TraceSearch_FromAnyTab_Click(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            var p = CollectSearchParametersFromControls(tab);

            if (!HasAnySearchCondition(p))
            {
                MessageBox.Show(
                    "検索条件を1つ以上入力してください。\r\n全項目空欄の検索は負荷が高いため実行できません。",
                    "検索条件不足",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _tabSearchParameters[tab.TabNo] = p;

            RegisterTraceGridEvents(tab);

           if (tab.RdoForward.Checked)
            {
                var tableSources = _service.B_TraceForward(p);
                tab.GridStart.DataSource = tableSources.LiquidTable;
                SetForwardGrid(tab, tableSources);
                _tabDisplayTables[tab.TabNo] = tableSources;
                BuildLineColorCash(tableSources);

            }

        }

        private void BuildLineColorCash(BottleDisplayTables tableSource)
        {
            if(tableSource == null|| tableSource.LineRanges.Count == 0) return;

            var Lines = tableSource.LineRanges;


            foreach (var Line in Lines)
            {
                var cache = new LineCache();

                cache.RowIndex = Line.BorderIndex;
                cache.Color = Color.FromArgb(120, 72, 32);

                _lineCache.Add(cache);
            }
        }


        private void LiqidTableBorderPaint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            int firstRowIndex = grid.FirstDisplayedScrollingRowIndex;
            if (firstRowIndex < 0)
                return;

            int displayedRowCount = grid.DisplayedRowCount(true);
            if (displayedRowCount <= 0)
                return;

            int lastRowIndex = firstRowIndex + displayedRowCount - 1;
            if (lastRowIndex >= grid.Rows.Count)
                lastRowIndex = grid.Rows.Count - 1;

            foreach(var cache in _lineCache)
            {
                if (cache == null)
                    continue;

                //if (cache.RowIndex < firstRowIndex || cache.RowIndex > lastRowIndex)
                //    continue;

                Rectangle rect = grid.GetRowDisplayRectangle(cache.RowIndex-1, true);
                if (rect.Height <= 0)
                    continue;


                int y = rect.Bottom - 1;
                int left = grid.DisplayRectangle.Left;
                int right = grid.DisplayRectangle.Right;

                using (var pen = new Pen(cache.Color, 2))
                {
                    e.Graphics.DrawLine(
                        pen,
                        left,
                        y,
                        right,
                        y);
                }
            }

        }

        private void BottleTableBorderPaint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            int firstRowIndex = grid.FirstDisplayedScrollingRowIndex;
            if (firstRowIndex < 0)
                return;

            int displayedRowCount = grid.DisplayedRowCount(true);
            if (displayedRowCount <= 0)
                return;

            int lastRowIndex = firstRowIndex + displayedRowCount - 1;
            if (lastRowIndex >= grid.Rows.Count)
                lastRowIndex = grid.Rows.Count - 1;

            foreach (var cache in _lineCache)
            {
                if (cache == null)
                    continue;

                //if (cache.RowIndex < firstRowIndex || cache.RowIndex > lastRowIndex)
                //    continue;

                Rectangle rect = grid.GetRowDisplayRectangle(cache.RowIndex-1, true);
                if (rect.Height <= 0)
                    continue;


                int y = rect.Bottom - 1;
                int left = grid.DisplayRectangle.Left;
                int right = grid.DisplayRectangle.Right;


                using (var pen = new Pen(cache.Color, 2))
                {
                    e.Graphics.DrawLine(
                        pen,
                        left,
                        y,
                        right,
                        y);
                }
            }

        }








        private void Clear_FromAnyTab_Click(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            tab.TxtOrder.Clear();
            tab.TxtItemName.Clear();
            tab.TxtItemCode.Clear();
            tab.TxtLot.Clear();
            tab.ChkUsePeriod.Checked = false;

            _tabSearchParameters.Remove(tab.TabNo);
            _tabDisplayTables.Remove(tab.TabNo);
            ClearBottleTraceTabGrids(tab);
        }





















        private void Csv_FromAnyTab_Click(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            BottleDisplayTables tables;
            if (!_tabDisplayTables.TryGetValue(tab.TabNo, out tables) || tables == null)
            {
                MessageBox.Show("出力する検索結果がありません。", "瓶設備ロットトレース",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportCsvForTab(tab, tables);
        }

        private void SwichBottleTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            ActivateTabDisplay(tab.TabNo);
        }

        private void ActivateTabDisplay(int tabNo)
        {
            var tab = GetTabContext(tabNo);
            if (tab == null) return;

            BottleDisplayTables tables;
            if (!_tabDisplayTables.TryGetValue(tabNo, out tables) || tables == null)
            {
                tab.GridStart.DataSource = null;
                tab.GridEnd.DataSource = null;
                RefreshBottleHeaderPanels(tab);
                return;
            }

            tab.GridStart.DataSource = null;
            tab.GridEnd.DataSource = null;

            tab.GridStart.DataSource = tables.LiquidTable;
            tab.GridEnd.DataSource = tables.BottleTable;
            SetForwardGrid(tab, tables);
            RefreshBottleHeaderPanels(tab);
        }

        private void ClearBottleTraceTabGrids(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            ClearTraceGrid(tab.GridStart);
            ClearTraceGrid(tab.GridEnd);
            RefreshBottleHeaderPanels(tab);
        }

        private void ClearTraceGrid(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.DataSource = null;
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.ClearSelection();
        }

        private void ExportCsvForTab(BottleTraceTabContext tab, BottleDisplayTables tables)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV ファイル (*.csv)|*.csv";
                dlg.FileName = "BottleTrace_Tab" + tab.TabNo + ".csv";

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                ExportHelper.ExportDataTableToCsv(MergeBottleDisplayTables(tables), dlg.FileName);
            }

            MessageBox.Show("CSV 出力が完了しました。", "瓶設備ロットトレース",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DataTable MergeBottleDisplayTables(BottleDisplayTables tables)
        {
            var result = new DataTable();
            result.Columns.Add("Type", typeof(string));

            AddColumns(result, tables.LiquidTable);
            AddColumns(result, tables.BottleTable);

            AddRows(result, "Liquid", tables.LiquidTable);
            AddRows(result, "Bottle", tables.BottleTable);

            return result;
        }

        private void AddColumns(DataTable target, DataTable source)
        {
            if (target == null || source == null)
                return;

            foreach (DataColumn sourceColumn in source.Columns)
            {
                if (!target.Columns.Contains(sourceColumn.ColumnName))
                    target.Columns.Add(sourceColumn.ColumnName, typeof(string));
            }
        }

        private void AddRows(DataTable target, string type, DataTable source)
        {
            if (target == null || source == null)
                return;

            foreach (DataRow sourceRow in source.Rows)
            {
                var row = target.NewRow();
                row["Type"] = type;

                foreach (DataColumn sourceColumn in source.Columns)
                {
                    object value = sourceRow[sourceColumn];
                    row[sourceColumn.ColumnName] = value == DBNull.Value ? "" : Convert.ToString(value);
                }

                target.Rows.Add(row);
            }
        }

        private void btnBackToLiquid_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
