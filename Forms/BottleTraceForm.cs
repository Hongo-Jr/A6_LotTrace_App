using DocumentFormat.OpenXml.Office2021.MipLabelMetaData;

using LotTraceApp.Models;
using LotTraceApp.Services;
using LotTraceApp.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
        private readonly Dictionary<int, BottleTraceResult> _tabBottleTraceResults =
            new Dictionary<int, BottleTraceResult>();

        // タブ番号 → UI コントロール一式。
        private readonly Dictionary<int, BottleTraceTabContext> _bottleTraceTabContexts =
            new Dictionary<int, BottleTraceTabContext>();


        private readonly Dictionary<int, List<LineCache>> _lineCache = new Dictionary<int, List<LineCache>>();
        private readonly Dictionary<DataGridView, GridForeColorCache> _gridForeColorCaches =
            new Dictionary<DataGridView, GridForeColorCache>();
        private readonly Dictionary<DataGridView, GridBackColorCache> _gridBackColorCaches =
            new Dictionary<DataGridView, GridBackColorCache>();
        private const string OutputIniSection = "Output";
        private const string DefaultExcelDirectoryIniKey = "DefaultExcelDirectory";
        private const string DefaultCsvDirectoryIniKey = "DefaultCsvDirectory";

        


        private sealed class LineCache
        {
            public int RowIndex {  get; set; }
            public Color Color {  get; set; }

        }

        private sealed class GridForeColorCache
        {
            public int[] ColumnGroupIndexes { get; private set; }
            public Color[,] RowGroupColors { get; private set; }

            public GridForeColorCache(int[] columnGroupIndexes, Color[,] rowGroupColors)
            {
                if (columnGroupIndexes == null) throw new ArgumentNullException("columnGroupIndexes");
                if (rowGroupColors == null) throw new ArgumentNullException("rowGroupColors");

                ColumnGroupIndexes = columnGroupIndexes;
                RowGroupColors = rowGroupColors;
            }

            public Color GetRequiredColor(int rowIndex, int columnIndex)
            {
                int groupIndex = ColumnGroupIndexes[columnIndex];
                return RowGroupColors[rowIndex, groupIndex];
            }
        }

        private sealed class GridBackColorCache
        {
            public int[] ColumnGroupIndexes { get; private set; }
            public Color[,] RowGroupBackColors { get; private set; }
            public Color[,] RowGroupSelectionBackColors { get; private set; }
            public bool[,] RowGroupHasBackColor { get; private set; }

            public GridBackColorCache(
                int[] columnGroupIndexes,
                Color[,] rowGroupBackColors,
                Color[,] rowGroupSelectionBackColors,
                bool[,] rowGroupHasBackColor)
            {
                if (columnGroupIndexes == null) throw new ArgumentNullException("columnGroupIndexes");
                if (rowGroupBackColors == null) throw new ArgumentNullException("rowGroupBackColors");
                if (rowGroupSelectionBackColors == null) throw new ArgumentNullException("rowGroupSelectionBackColors");
                if (rowGroupHasBackColor == null) throw new ArgumentNullException("rowGroupHasBackColor");

                ColumnGroupIndexes = columnGroupIndexes;
                RowGroupBackColors = rowGroupBackColors;
                RowGroupSelectionBackColors = rowGroupSelectionBackColors;
                RowGroupHasBackColor = rowGroupHasBackColor;
            }

            public bool TryGetBackColor(
                int rowIndex,
                int columnIndex,
                out Color backColor,
                out Color selectionBackColor)
            {
                backColor = Color.Empty;
                selectionBackColor = Color.Empty;

                int groupIndex = ColumnGroupIndexes[columnIndex];
                if (!RowGroupHasBackColor[rowIndex, groupIndex])
                    return false;

                backColor = RowGroupBackColors[rowIndex, groupIndex];
                selectionBackColor = RowGroupSelectionBackColors[rowIndex, groupIndex];
                return true;
            }
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

        private readonly ToolTip _itemNameToolTip = new ToolTip();
        private readonly Font _itemNameToolTipFont = new Font("Segoe UI", 12F, FontStyle.Regular);
        private string _currentItemToolTipText = string.Empty;


        private Button _btnIntersectionCsv;
        private Button _btnIntersectionClear;
        // タブ番号 → 交点検出などの対象タブ
        private readonly HashSet<int> _selectedTraceTargetTabs =
            new HashSet<int>();
        private List<B_CrossPointRecord> _lastBottleCrossPoints;
        private readonly List<int> _lastBottleCrossPointTargetTabs = new List<int>();
        private readonly Dictionary<int, HashSet<string>> _crossPointNodeKeysByTab =
            new Dictionary<int, HashSet<string>>();
        private readonly Dictionary<string, Tuple<Color, Color>> _crossPointColorsByNodeKey =
            new Dictionary<string, Tuple<Color, Color>>(StringComparer.Ordinal);


        #region コンストラクタ・初期化

        public BottleTraceForm(BottleTraceService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();
            ConfigureBottleTraceTabOwnerDraw();

            InitializeBottleTraceTabContexts();
            InitializeBottleTraceTabs();
            InitializeItemNameToolTip();

            RegisterBottleTraceTabNameEvents();
            RefreshBottleTraceTabNames();

            RegisterBottleTracePeriodEvents();
            RefreshBottleTracePeriodControls();
            InitializeBottleIntersectionTab();

            swichBottleTab.SelectedIndexChanged -= SwichBottleTab_SelectedIndexChanged;
            swichBottleTab.SelectedIndexChanged += SwichBottleTab_SelectedIndexChanged;
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


            }

            btnBottleExcelOutput.Click -= btnBottleExcelOutput_Click;
            btnBottleExcelOutput.Click += btnBottleExcelOutput_Click;

            btnBottleDetectCrossPoints.Click -= btnBottleDetectCrossPoints_Click;
            btnBottleDetectCrossPoints.Click += btnBottleDetectCrossPoints_Click;
        }

        private void InitializeBottleIntersectionTab()
        {
            if (dataGridIntersection != null)
            {
                dataGridIntersection.AutoGenerateColumns = true;
                dataGridIntersection.ReadOnly = true;
                dataGridIntersection.AllowUserToAddRows = false;
                dataGridIntersection.AllowUserToDeleteRows = false;
                dataGridIntersection.AllowUserToOrderColumns = false;
                dataGridIntersection.RowHeadersVisible = false;
                dataGridIntersection.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridIntersection.ScrollBars = ScrollBars.Both;
                dataGridIntersection.CellFormatting -= OnBottleIntersectionGridCrossPointBackColorFormatting;
                dataGridIntersection.CellFormatting += OnBottleIntersectionGridCrossPointBackColorFormatting;
                dataGridIntersection.DataSource = B_CreateCrossPointGridTable(null, null);
            }

            if (_btnIntersectionCsv == null)
            {
                _btnIntersectionCsv = CreateBottleIntersectionCommandButton("btnBottleIntersectionCsv", "CSV出力");
                _btnIntersectionCsv.Click -= btnBottleIntersectionCsv_Click;
                _btnIntersectionCsv.Click += btnBottleIntersectionCsv_Click;
                BottleIntersectionTab.Controls.Add(_btnIntersectionCsv);
            }

            if (_btnIntersectionClear == null)
            {
                _btnIntersectionClear = CreateBottleIntersectionCommandButton("btnBottleIntersectionClear", "クリア");
                _btnIntersectionClear.Click -= btnBottleIntersectionClear_Click;
                _btnIntersectionClear.Click += btnBottleIntersectionClear_Click;
                BottleIntersectionTab.Controls.Add(_btnIntersectionClear);
            }

            BottleIntersectionTab.Resize -= BottleIntersectionTab_Resize;
            BottleIntersectionTab.Resize += BottleIntersectionTab_Resize;
            B_ApplyIntersectionTabLayout();
            B_ApplyCrossPointGridColumnWidths();
        }

        private Button CreateBottleIntersectionCommandButton(string name, string text)
        {
            return new Button
            {
                Name = name,
                Text = text,
                Font = new Font("游ゴシック", 12F, FontStyle.Regular),
                UseVisualStyleBackColor = true
            };
        }

        private void BottleIntersectionTab_Resize(object sender, EventArgs e)
        {
            B_ApplyIntersectionTabLayout();
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

        #endregion

        #region イベント

        private void RegisterTraceGridEvents(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            UnregisterTraceGridBorderPaint(tab.GridStart);
            UnregisterTraceGridBorderPaint(tab.GridEnd);

            TraceSearchParameters p;
            if (!_tabSearchParameters.TryGetValue(tab.TabNo, out p))
                return;

            if (p.Direction == TraceDirection.Forward)
            {
                RegisterTraceGridEvents(tab.GridStart, LiquidTableBorderPaint);
                RegisterTraceGridEvents(tab.GridEnd, BottleTableBorderPaint);
            }
            if (p.Direction == TraceDirection.Backward)
            {
                RegisterTraceGridEvents(tab.GridEnd, LiquidTableBorderPaint);
                RegisterTraceGridEvents(tab.GridStart, BottleTableBorderPaint);
            }

        }

        private void RegisterTraceGridEvents(DataGridView grid, PaintEventHandler paintHandler)
        {
            if (grid == null || paintHandler == null)
                return;

            grid.Paint -= paintHandler;
            grid.Paint += paintHandler;

            grid.CellFormatting -= OnBottleNodeKeyGroupForeColorFormattingFromCache;
            grid.CellFormatting += OnBottleNodeKeyGroupForeColorFormattingFromCache;
            grid.CellFormatting -= OnBottleCrossPointNodeBackColorFormatting;
            grid.CellFormatting += OnBottleCrossPointNodeBackColorFormatting;

            grid.CellMouseEnter -= Grid_CellMouseEnter_ToolTip;
            grid.CellMouseEnter += Grid_CellMouseEnter_ToolTip;

            grid.CellMouseLeave -= Grid_CellMouseLeave_ToolTip;
            grid.CellMouseLeave += Grid_CellMouseLeave_ToolTip;

            grid.Scroll -= Grid_ItemNameToolTipHideOnScroll;
            grid.Scroll += Grid_ItemNameToolTipHideOnScroll;

            grid.MouseLeave -= Grid_ItemNameToolTipHideOnMouseLeave;
            grid.MouseLeave += Grid_ItemNameToolTipHideOnMouseLeave;
        }

        private void UnregisterTraceGridBorderPaint(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.Paint -= LiquidTableBorderPaint;
            grid.Paint -= BottleTableBorderPaint;
        }

        private void RegisterBottleTraceTabNameEvents()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                if (tab.TxtOrder != null)
                {
                    tab.TxtOrder.TextChanged -= BottleTraceTabNameSourceChanged;
                    tab.TxtOrder.TextChanged += BottleTraceTabNameSourceChanged;
                }

                if (tab.TxtItemCode != null)
                {
                    tab.TxtItemCode.TextChanged -= BottleTraceTabNameSourceChanged;
                    tab.TxtItemCode.TextChanged += BottleTraceTabNameSourceChanged;
                }
            }

            rdoBottleTabNameOrder.CheckedChanged -= BottleTraceTabNameModeChanged;
            rdoBottleTabNameOrder.CheckedChanged += BottleTraceTabNameModeChanged;

            rdoBottleTabNameItemCode.CheckedChanged -= BottleTraceTabNameModeChanged;
            rdoBottleTabNameItemCode.CheckedChanged += BottleTraceTabNameModeChanged;
        }

        private void RegisterBottleTracePeriodEvents()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null || tab.ChkUsePeriod == null)
                    continue;

                tab.ChkUsePeriod.CheckedChanged -= BottleTracePeriodCheckChanged;
                tab.ChkUsePeriod.CheckedChanged += BottleTracePeriodCheckChanged;
            }
        }

        private void BottleTracePeriodCheckChanged(object sender, EventArgs e)
        {
            RefreshBottleTracePeriodControls();
        }

        private void RefreshBottleTracePeriodControls()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                ApplyBottleTracePeriodControlState(tab);
            }
        }

        private void ApplyBottleTracePeriodControlState(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            bool enabled = tab.ChkUsePeriod != null && tab.ChkUsePeriod.Checked;

            ApplyBottleTraceDateTimePickerState(tab.DtpFrom, enabled);
            ApplyBottleTraceDateTimePickerState(tab.DtpTo, enabled);
        }

        private void ApplyBottleTraceDateTimePickerState(DateTimePicker picker, bool enabled)
        {
            if (picker == null)
                return;

            picker.Enabled = enabled;
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = enabled ? "yyyy/MM/dd" : " ";
        }

        #endregion

        #region タブコントロール関連

        private void ActivateTabDisplay(int tabNo)
        {
            var tab = GetTabContext(tabNo);
            if (tab == null) return;

            BottleDisplayTables tables;
            if (!_tabDisplayTables.TryGetValue(tabNo, out tables) || tables == null)
            {
                _gridForeColorCaches.Remove(tab.GridStart);
                _gridForeColorCaches.Remove(tab.GridEnd);
                _gridBackColorCaches.Remove(tab.GridStart);
                _gridBackColorCaches.Remove(tab.GridEnd);
                tab.GridStart.DataSource = null;
                tab.GridEnd.DataSource = null;
                RefreshBottleHeaderPanels(tab);
                return;
            }

            tab.GridStart.DataSource = null;
            tab.GridEnd.DataSource = null;

            tab.GridStart.DataSource = tables.LiquidTable;
            tab.GridEnd.DataSource = tables.BottleTable;

            TraceSearchParameters p;
            if (!_tabSearchParameters.TryGetValue(tabNo, out p))
                return;

            if (p.Direction == TraceDirection.Forward) { SetForwardGrid(tab, tables); }

            if (p.Direction == TraceDirection.Backward) { SetBackwardGrid(tab, tables); }

            BuildBottleGridForeColorCaches(tab);
            BuildBottleGridBackColorCache(tab, tab.GridStart);
            BuildBottleGridBackColorCache(tab, tab.GridEnd);
            RegisterTraceGridEvents(tab);
            RefreshBottleHeaderPanels(tab);
        }

        private void SwichBottleTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            ActivateTabDisplay(tab.TabNo);
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

        private BottleTraceTabContext CreateBottleTraceTabContext(int tabNo)
        {
            switch (tabNo)
            {
                case 1:
                    return new BottleTraceTabContext
                    {
                        TabNo = 1,
                        TxtOrder = txtBottleOrderNo,
                        TxtItemName = txtBottleItemName,
                        TxtItemCode = txtBottleItemCode,
                        TxtLot = txtBottleLotNo,
                        ChkUsePeriod = timeCheck,
                        DtpFrom = startBottleTime,
                        DtpTo = endBottleTime,
                        RdoForward = rdoForwardBottle,
                        RdoBackward = rdoBackwardBottle,
                        BtnSearch = btnBottleTraceSearch,
                        BtnClear = btnClearBottle,
                        BtnCsv = btnCsvOutputBottle,
                        GridStart = dgvStartBottle,
                        GridEnd = dgvEndBottle
                    };

                case 2:
                    return new BottleTraceTabContext
                    {
                        TabNo = 2,
                        TxtOrder = txtBottleOrderNo_2,
                        TxtItemName = txtBottleItemName_2,
                        TxtItemCode = txtBottleItemCode_2,
                        TxtLot = txtBottleLotNo_2,
                        ChkUsePeriod = timeCheck_2,
                        DtpFrom = startBottleTime_2,
                        DtpTo = endBottleTime_2,
                        RdoForward = rdoForwardBottle_2,
                        RdoBackward = rdoBackwardBottle_2,
                        BtnSearch = btnBottleTraceSearch_2,
                        BtnClear = btnClearBottle_2,
                        BtnCsv = btnCsvOutputBottle_2,
                        GridStart = dgvStartBottle_2,
                        GridEnd = dgvEndBottle_2
                    };

                case 3:
                    return new BottleTraceTabContext
                    {
                        TabNo = 3,
                        TxtOrder = txtBottleOrderNo_3,
                        TxtItemName = txtBottleItemName_3,
                        TxtItemCode = txtBottleItemCode_3,
                        TxtLot = txtBottleLotNo_3,
                        ChkUsePeriod = timeCheck_3,
                        DtpFrom = startBottleTime_3,
                        DtpTo = endBottleTime_3,
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
                        TxtOrder = txtBottleOrderNo_4,
                        TxtItemName = textBox7,
                        TxtItemCode = txtBottleItemCode_4,
                        TxtLot = txtBottleLotNo_4,
                        ChkUsePeriod = timeCheck_4,
                        DtpFrom = startBottleTime_4,
                        DtpTo = endBottleTime_4,
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
                        TxtOrder = txtBottleOrderNo_5,
                        TxtItemName = txtBottleItemName_5,
                        TxtItemCode = txtBottleItemCode_5,
                        TxtLot = txtBottleLotNo_5,
                        ChkUsePeriod = timeCheck_5,
                        DtpFrom = startBottleTime_5,
                        DtpTo = endBottleTime_5,
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
                        TxtOrder = txtBottleOrderNo_6,
                        TxtItemName = txtBottleItemName_6,
                        TxtItemCode = txtBottleItemCode_6,
                        TxtLot = txtBottleLotNo_6,
                        ChkUsePeriod = timeCheck_6,
                        DtpFrom = startBottleTime_6,
                        DtpTo = endBottleTime_6,
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
                        TxtOrder = txtBottleOrderNo_7,
                        TxtItemName = txtBottleItemName_7,
                        TxtItemCode = txtBottleItemCode_7,
                        TxtLot = txtBottleLotNo_7,
                        ChkUsePeriod = timeCheck_7,
                        DtpFrom = startBottleTime_7,
                        DtpTo = endBottleTime_7,
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
                        TxtOrder = txtBottleOrderNo_8,
                        TxtItemName = txtBottleItemName_8,
                        TxtItemCode = txtBottleItemCode_8,
                        TxtLot = txtBottleLotNo_8,
                        ChkUsePeriod = timeCheck_8,
                        DtpFrom = startBottleTime_8,
                        DtpTo = endBottleTime_8,
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
                        TxtOrder = txtBottleOrderNo_9,
                        TxtItemName = txtBottleItemName_9,
                        TxtItemCode = txtBottleItemCode_9,
                        TxtLot = txtBottleLotNo_9,
                        ChkUsePeriod = timeCheck_9,
                        DtpFrom = startBottleTime_9,
                        DtpTo = endBottleTime_9,
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
                        TxtItemName = txtBottleItemName_10,
                        TxtItemCode = txtBottleItemCode_10,
                        TxtLot = txtBottleLotNo_10,
                        ChkUsePeriod = timeCheck_10,
                        DtpFrom = dateTimePicker5,
                        DtpTo = endBottleTime_10,
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

        

        private void BottleTraceTabNameSourceChanged(object sender, EventArgs e)
        {
            RefreshBottleTraceTabNames();
        }

        private void BottleTraceTabNameModeChanged(object sender, EventArgs e)
        {
            RefreshBottleTraceTabNames();
        }

        private void RefreshBottleTraceTabNames()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                var tabPage = GetBottleTraceTabPage(tabNo);

                if (tab == null || tabPage == null)
                    continue;

                tabPage.Text = BuildBottleTraceTabName(tab);
            }
        }

        private TabPage GetBottleTraceTabPage(int tabNo)
        {
            if (swichBottleTab == null || tabNo < 1 || tabNo > 10)
                return null;

            int index = tabNo - 1;
            return index < swichBottleTab.TabPages.Count
                ? swichBottleTab.TabPages[index]
                : null;
        }

        private string BuildBottleTraceTabName(BottleTraceTabContext tab)
        {
            if (tab == null)
                return string.Empty;

            TextBox source = rdoBottleTabNameItemCode.Checked
                ? tab.TxtItemCode
                : tab.TxtOrder;

            string value = source == null ? null : source.Text;

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            return string.Format("({0:00})_未設定", tab.TabNo);
        }


        private CheckBox GetTraceTargetCheckBox(int tabNo)
        {
            switch (tabNo)
            {
                case 1: return checkBottle1;
                case 2: return checkBottle2;
                case 3: return checkBottle3;
                case 4: return checkBottle4;
                case 5: return checkBottle5;
                case 6: return checkBottle6;
                case 7: return checkBottle7;
                case 8: return checkBottle8;
                case 9: return checkBottle9;
                case 10: return checkBottle10;
                default: return null;
            }
        }

        #endregion

        #region 画面描画系


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
            tab.GridStart.Columns["DisplayKey"].Visible = false;
            tab.GridStart.Columns["ItemCode"].Visible = false;

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
            tab.GridEnd.Columns["DisplayKey"].Visible = false;
            tab.GridEnd.Columns["ItemCode"].Visible = false;

            HideBottleInternalColumns(tab.GridStart);
            HideBottleInternalColumns(tab.GridEnd);
            ApplyBottleGridWidthsFromColumns(tab);
            RefreshBottleHeaderPanels(tab);
        }

        private void SetBackwardGrid(BottleTraceTabContext tab, BottleDisplayTables tables)
        {
            //液

            tab.GridEnd.DataSource = tables.LiquidTable;

            tab.GridEnd.Columns["OrderNumber"].Visible = true;
            tab.GridEnd.Columns["OrderNumber"].HeaderText = tables.LiquidTable.Columns["OrderNumber"].Caption;
            tab.GridEnd.Columns["OrderNumber"].Width = 120;

            tab.GridEnd.Columns["Lot"].Visible = true;
            tab.GridEnd.Columns["Lot"].HeaderText = tables.LiquidTable.Columns["Lot"].Caption;
            tab.GridEnd.Columns["Lot"].Width = 120;

            tab.GridEnd.Columns["ItemName"].Visible = true;
            tab.GridEnd.Columns["ItemName"].HeaderText = tables.LiquidTable.Columns["ItemName"].Caption;
            tab.GridEnd.Columns["ItemName"].Width = 120;

            tab.GridEnd.Columns["StartDate"].Visible = true;
            tab.GridEnd.Columns["StartDate"].HeaderText = tables.LiquidTable.Columns["StartDate"].Caption;
            tab.GridEnd.Columns["StartDate"].Width = 150;

            tab.GridEnd.Columns["Weight"].Visible = true;
            tab.GridEnd.Columns["Weight"].HeaderText = tables.LiquidTable.Columns["Weight"].Caption;
            tab.GridEnd.Columns["Weight"].Width = 120;

            tab.GridEnd.Columns["NodeKey"].Visible = false;
            tab.GridEnd.Columns["DisplayKey"].Visible = false;
            tab.GridEnd.Columns["ItemCode"].Visible = false;
            
            //瓶

            tab.GridStart.DataSource = tables.BottleTable;

            tab.GridStart.Columns["OrderNumber"].Visible = true;
            tab.GridStart.Columns["OrderNumber"].HeaderText = tables.BottleTable.Columns["OrderNumber"].Caption;
            tab.GridStart.Columns["OrderNumber"].Width = 120;

            tab.GridStart.Columns["Lot"].Visible = true;
            tab.GridStart.Columns["Lot"].HeaderText = tables.BottleTable.Columns["Lot"].Caption;
            tab.GridStart.Columns["Lot"].Width = 120;

            tab.GridStart.Columns["ItemName"].Visible = true;
            tab.GridStart.Columns["ItemName"].HeaderText = tables.BottleTable.Columns["ItemName"].Caption;
            tab.GridStart.Columns["ItemName"].Width = 120;

            tab.GridStart.Columns["StartDate"].Visible = true;
            tab.GridStart.Columns["StartDate"].HeaderText = tables.BottleTable.Columns["StartDate"].Caption;
            tab.GridStart.Columns["StartDate"].Width = 150;

            tab.GridStart.Columns["OK_Num"].Visible = true;
            tab.GridStart.Columns["OK_Num"].HeaderText = tables.BottleTable.Columns["OK_Num"].Caption;
            tab.GridStart.Columns["OK_Num"].Width = 120;

            tab.GridStart.Columns["NG_Num"].Visible = true;
            tab.GridStart.Columns["NG_Num"].HeaderText = tables.BottleTable.Columns["NG_Num"].Caption;
            tab.GridStart.Columns["NG_Num"].Width = 120;

            tab.GridStart.Columns["Total_Num"].Visible = true;
            tab.GridStart.Columns["Total_Num"].HeaderText = tables.BottleTable.Columns["Total_Num"].Caption;
            tab.GridStart.Columns["Total_Num"].Width = 120;



            tab.GridStart.Columns["NodeKey"].Visible = false;
            tab.GridStart.Columns["DisplayKey"].Visible = false;
            tab.GridStart.Columns["ItemCode"].Visible = false;


            HideBottleInternalColumns(tab.GridStart);
            HideBottleInternalColumns(tab.GridEnd);
            ApplyBottleGridWidthsFromColumns(tab);
            RefreshBottleHeaderPanels(tab);
        }

        private void HideBottleInternalColumns(DataGridView grid)
        {
            if (grid == null)
                return;

            HideGridColumnIfExists(grid, "NodeKey");
            HideGridColumnIfExists(grid, "DisplayKey");
            HideGridColumnIfExists(grid, "ItemCode");
            HideGridColumnIfExists(grid, "MasterKey");
            HideGridColumnIfExists(grid, "StartDateLabel");
            HideGridColumnIfExists(grid, "InputSourceType");
        }

        private void HideGridColumnIfExists(DataGridView grid, string columnName)
        {
            if (grid == null || string.IsNullOrEmpty(columnName) || !grid.Columns.Contains(columnName))
                return;

            grid.Columns[columnName].Visible = false;
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

        #endregion

        #region ツールチップ

        #region ツールチップ

        private void InitializeItemNameToolTip()
        {
            _itemNameToolTip.OwnerDraw = true;
            _itemNameToolTip.UseAnimation = true;
            _itemNameToolTip.UseFading = true;
            _itemNameToolTip.InitialDelay = 250;
            _itemNameToolTip.ReshowDelay = 100;
            _itemNameToolTip.AutoPopDelay = 8000;
            _itemNameToolTip.ShowAlways = true;

            _itemNameToolTip.Popup -= ItemNameToolTip_Popup;
            _itemNameToolTip.Draw -= ItemNameToolTip_Draw;

            _itemNameToolTip.Popup += ItemNameToolTip_Popup;
            _itemNameToolTip.Draw += ItemNameToolTip_Draw;
        }

        private void ItemNameToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (e == null) return;

            string text = _currentItemToolTipText;
            if (string.IsNullOrWhiteSpace(text))
            {
                e.ToolTipSize = new Size(120, 32);
                return;
            }

            Size measured = TextRenderer.MeasureText(
                text,
                _itemNameToolTipFont,
                new Size(600, 0),
                TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.NoPrefix);

            e.ToolTipSize = new Size(
                Math.Max(120, measured.Width + 16),
                Math.Max(32, measured.Height + 12));
        }

        private void ItemNameToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            if (e == null) return;

            string text = string.IsNullOrWhiteSpace(_currentItemToolTipText)
                ? e.ToolTipText
                : _currentItemToolTipText;

            e.Graphics.FillRectangle(Brushes.LightYellow, e.Bounds);

            using (var borderPen = new Pen(Color.Gray, 1))
            {
                e.Graphics.DrawRectangle(borderPen,
                    new Rectangle(0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1));
            }

            TextRenderer.DrawText(
                e.Graphics,
                text ?? string.Empty,
                _itemNameToolTipFont,
                new Rectangle(8, 6, Math.Max(1, e.Bounds.Width - 16), Math.Max(1, e.Bounds.Height - 12)),
                Color.Black,
                TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.NoPrefix);
        }

        private void Grid_CellMouseEnter_ToolTip(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;

            _itemNameToolTip.Hide(grid);
            _currentItemToolTipText = string.Empty;

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.RowIndex >= grid.Rows.Count || e.ColumnIndex >= grid.Columns.Count) return;

            var column = grid.Columns[e.ColumnIndex];
            if (column == null || !column.Visible) return;

            var row = grid.Rows[e.RowIndex];
            if (row == null) return;

            object value = row.Cells[e.ColumnIndex].Value;
            if (value == null || value == DBNull.Value) return;

            string cellText = Convert.ToString(value);
            if (string.IsNullOrWhiteSpace(cellText)) return;

            string text;

            if (string.Equals(column.Name, "ItemName", StringComparison.OrdinalIgnoreCase))
            {
                string itemName = cellText;
                string itemCode = GetGridCellString(row, "ItemCode");

                text = string.IsNullOrWhiteSpace(itemCode)
                    ? "品目名：" + itemName
                    : "品目名：" + itemName + Environment.NewLine
                      + "品目コード：" + itemCode;
            }
            else
            {
                string headerText = string.IsNullOrWhiteSpace(column.HeaderText)
                    ? column.Name
                    : column.HeaderText;

                text = headerText + "：" + cellText;
            }

            text = NormalizeToolTipText(text, 32);
            if (string.IsNullOrWhiteSpace(text)) return;

            _currentItemToolTipText = text;

            Rectangle cellRect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            if (cellRect.Width <= 0 || cellRect.Height <= 0) return;

            Point showPoint = new Point(
                Math.Max(0, cellRect.Left + 12),
                Math.Max(0, cellRect.Bottom + 2));

            _itemNameToolTip.Show(_currentItemToolTipText, grid, showPoint, _itemNameToolTip.AutoPopDelay);
        }

        private string GetGridCellString(DataGridViewRow row, string columnName)
        {
            if (row == null || row.DataGridView == null)
                return string.Empty;

            var grid = row.DataGridView;
            if (!grid.Columns.Contains(columnName))
                return string.Empty;

            object value = row.Cells[columnName].Value;
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        private void Grid_CellMouseLeave_ToolTip(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;

            _currentItemToolTipText = string.Empty;
            _itemNameToolTip.Hide(grid);
        }

        private void Grid_ItemNameToolTipHideOnScroll(object sender, ScrollEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;

            _currentItemToolTipText = string.Empty;
            _itemNameToolTip.Hide(grid);
        }

        private void Grid_ItemNameToolTipHideOnMouseLeave(object sender, EventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;

            _currentItemToolTipText = string.Empty;
            _itemNameToolTip.Hide(grid);
        }

        private string NormalizeToolTipText(string text, int lineLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Replace("\r\n", "\n").Replace('\r', '\n');

            var lines = text.Split('\n');
            var normalizedLines = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    normalizedLines.Add(string.Empty);
                    continue;
                }

                int index = 0;
                while (index < line.Length)
                {
                    int length = Math.Min(lineLength, line.Length - index);
                    normalizedLines.Add(line.Substring(index, length));
                    index += length;
                }
            }

            return string.Join(Environment.NewLine, normalizedLines);
        }

        #endregion

        #endregion

        #region トレース実行

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
                var traceResult = _service.B_TraceForwardResult(p);
                var tableSources = traceResult == null ? null : traceResult.DisplayTables;

                SetForwardGrid(tab, tableSources);
                _tabDisplayTables[tab.TabNo] = tableSources;
                _tabBottleTraceResults[tab.TabNo] = traceResult;
                BuildLineColorCache(tab.TabNo, tableSources);
                BuildBottleGridForeColorCaches(tab);
                ClearBottleCrossPointNodeKeysForTab(tab.TabNo);
                ClearBottleGridBackColorCachesForTab(tab.TabNo);

            }

            if (tab.RdoBackward.Checked)
            {
                var traceResult = _service.B_TraceBackwardResult(p);
                var tableSources = traceResult == null ? null : traceResult.DisplayTables;

                SetBackwardGrid(tab, tableSources);
                _tabDisplayTables[tab.TabNo] = tableSources;
                _tabBottleTraceResults[tab.TabNo] = traceResult;
                BuildLineColorCache(tab.TabNo, tableSources);
                BuildBottleGridForeColorCaches(tab);
                ClearBottleCrossPointNodeKeysForTab(tab.TabNo);
                ClearBottleGridBackColorCachesForTab(tab.TabNo);
            }

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

        #endregion

        #region 文字色

        private void BuildBottleGridForeColorCaches(BottleTraceTabContext tab)
        {
            if (tab == null)
                return;

            BuildBottleGridForeColorCache(tab.GridStart);
            BuildBottleGridForeColorCache(tab.GridEnd);
        }

        private void BuildBottleGridForeColorCache(DataGridView grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            int columnCount = grid.Columns.Count;
            int rowCount = grid.Rows.Count;

            if (columnCount == 0)
            {
                _gridForeColorCaches.Remove(grid);
                return;
            }

            int[] columnGroupIndexes = new int[columnCount];
            var nodeKeyColumnNames = new List<string>();
            var groupIndexByNodeKeyColumnName =
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                string nodeKeyColumnName =
                    ResolveBottleNodeKeyColumnNameForForeColorGrouping(grid, columnIndex);
                string groupKey = nodeKeyColumnName ?? string.Empty;

                int groupIndex;
                if (!groupIndexByNodeKeyColumnName.TryGetValue(groupKey, out groupIndex))
                {
                    groupIndex = nodeKeyColumnNames.Count;
                    groupIndexByNodeKeyColumnName[groupKey] = groupIndex;
                    nodeKeyColumnNames.Add(nodeKeyColumnName);
                }

                columnGroupIndexes[columnIndex] = groupIndex;
            }

            Color[,] rowGroupColors = new Color[rowCount, nodeKeyColumnNames.Count];
            Color defaultForeColor = GetDefaultForeColorForCache(grid);
            var colorByNodeKey = new Dictionary<string, Color>(StringComparer.Ordinal);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var boundItem = grid.Rows[rowIndex].DataBoundItem as DataRowView;
                if (boundItem == null || boundItem.Row == null)
                {
                    FillBottleForeColorCacheRow(rowGroupColors, rowIndex, nodeKeyColumnNames.Count, defaultForeColor);
                    continue;
                }

                for (int groupIndex = 0; groupIndex < nodeKeyColumnNames.Count; groupIndex++)
                {
                    string nodeKeyColumnName = nodeKeyColumnNames[groupIndex];
                    if (string.IsNullOrEmpty(nodeKeyColumnName))
                    {
                        rowGroupColors[rowIndex, groupIndex] = defaultForeColor;
                        continue;
                    }

                    string nodeKey = GetTableString(boundItem.Row, nodeKeyColumnName);
                    if (string.IsNullOrWhiteSpace(nodeKey))
                    {
                        rowGroupColors[rowIndex, groupIndex] = defaultForeColor;
                        continue;
                    }

                    Color color;
                    if (!colorByNodeKey.TryGetValue(nodeKey, out color))
                    {
                        color = GetForeColorForNodeKeyGroup(nodeKey);
                        colorByNodeKey[nodeKey] = color;
                    }

                    rowGroupColors[rowIndex, groupIndex] = color;
                }
            }

            _gridForeColorCaches[grid] =
                new GridForeColorCache(columnGroupIndexes, rowGroupColors);
        }

        private void FillBottleForeColorCacheRow(
            Color[,] rowGroupColors,
            int rowIndex,
            int groupCount,
            Color defaultForeColor)
        {
            for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
            {
                rowGroupColors[rowIndex, groupIndex] = defaultForeColor;
            }
        }

        private string ResolveBottleNodeKeyColumnNameForForeColorGrouping(
            DataGridView grid,
            int columnIndex)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (columnIndex < 0 || columnIndex >= grid.Columns.Count)
                throw new ArgumentOutOfRangeException("columnIndex");

            var column = grid.Columns[columnIndex];
            if (column == null)
                throw new InvalidOperationException("文字色キャッシュ作成時に列情報を取得できません。");

            string name = column.Name;
            if (string.Equals(name, "NodeKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "DisplayKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "ItemCode", StringComparison.OrdinalIgnoreCase))
                return null;

            DataTable table = ResolveGridDataTable(grid);
            if (table == null || !table.Columns.Contains("NodeKey"))
                return null;

            return "NodeKey";
        }

        private DataTable ResolveGridDataTable(DataGridView grid)
        {
            if (grid == null)
                return null;

            var table = grid.DataSource as DataTable;
            if (table != null)
                return table;

            var view = grid.DataSource as DataView;
            return view == null ? null : view.Table;
        }

        private Color GetDefaultForeColorForCache(DataGridView grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            Color color = grid.DefaultCellStyle.ForeColor;
            return color.IsEmpty ? Color.Black : color;
        }

        private void OnBottleNodeKeyGroupForeColorFormattingFromCache(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            GridForeColorCache cache;
            if (!_gridForeColorCaches.TryGetValue(grid, out cache))
                return;

            if (e.RowIndex >= cache.RowGroupColors.GetLength(0) ||
                e.ColumnIndex >= cache.ColumnGroupIndexes.Length)
                return;

            e.CellStyle.ForeColor = cache.GetRequiredColor(e.RowIndex, e.ColumnIndex);
        }

        private Color GetForeColorForNodeKeyGroup(string key)
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

        private Color ConvertHsvToColor(double hue, double saturation, double value)
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

        private string GetTableString(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || string.IsNullOrEmpty(columnName))
                return string.Empty;

            if (!row.Table.Columns.Contains(columnName))
                return string.Empty;

            object value = row[columnName];
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        private int GetTableInt(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || string.IsNullOrEmpty(columnName) ||
                !row.Table.Columns.Contains(columnName))
                return 0;

            object value = row[columnName];
            if (value == null || value == DBNull.Value)
                return 0;

            int result;
            return int.TryParse(Convert.ToString(value), out result) ? result : 0;
        }

        private void BuildBottleGridBackColorCaches()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null) continue;

                BuildBottleGridBackColorCache(tab, tab.GridStart);
                BuildBottleGridBackColorCache(tab, tab.GridEnd);
            }
        }

        private void BuildBottleGridBackColorCache(BottleTraceTabContext tab, DataGridView grid)
        {
            if (tab == null || grid == null)
                return;

            HashSet<string> crossPointNodeKeys;
            if (!_crossPointNodeKeysByTab.TryGetValue(tab.TabNo, out crossPointNodeKeys) ||
                crossPointNodeKeys == null || crossPointNodeKeys.Count == 0)
            {
                _gridBackColorCaches.Remove(grid);
                return;
            }

            int columnCount = grid.Columns.Count;
            int rowCount = grid.Rows.Count;

            int[] columnGroupIndexes = new int[columnCount];
            var nodeKeyColumnNames = new List<string>();
            var groupIndexByNodeKeyColumnName =
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                string nodeKeyColumnName = ResolveBottleCrossPointNodeKeyColumnName(grid, columnIndex);
                string groupKey = nodeKeyColumnName ?? string.Empty;

                int groupIndex;
                if (!groupIndexByNodeKeyColumnName.TryGetValue(groupKey, out groupIndex))
                {
                    groupIndex = nodeKeyColumnNames.Count;
                    groupIndexByNodeKeyColumnName[groupKey] = groupIndex;
                    nodeKeyColumnNames.Add(nodeKeyColumnName);
                }

                columnGroupIndexes[columnIndex] = groupIndex;
            }

            var rowGroupBackColors = new Color[rowCount, nodeKeyColumnNames.Count];
            var rowGroupSelectionBackColors = new Color[rowCount, nodeKeyColumnNames.Count];
            var rowGroupHasBackColor = new bool[rowCount, nodeKeyColumnNames.Count];
            var colorByNodeKey = new Dictionary<string, Tuple<Color, Color>>(StringComparer.Ordinal);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var boundItem = grid.Rows[rowIndex].DataBoundItem as DataRowView;
                if (boundItem == null || boundItem.Row == null)
                    continue;

                for (int groupIndex = 0; groupIndex < nodeKeyColumnNames.Count; groupIndex++)
                {
                    string nodeKeyColumnName = nodeKeyColumnNames[groupIndex];
                    if (string.IsNullOrEmpty(nodeKeyColumnName))
                        continue;

                    string key = BuildBottleCrossPointUiKeyFromRow(boundItem.Row);
                    if (string.IsNullOrWhiteSpace(key) || !crossPointNodeKeys.Contains(key))
                        continue;

                    Tuple<Color, Color> colors;
                    if (!colorByNodeKey.TryGetValue(key, out colors))
                    {
                        colors = GetCrossPointNodeColors(key);
                        colorByNodeKey[key] = colors;
                    }

                    rowGroupBackColors[rowIndex, groupIndex] = colors.Item1;
                    rowGroupSelectionBackColors[rowIndex, groupIndex] = colors.Item2;
                    rowGroupHasBackColor[rowIndex, groupIndex] = true;
                }
            }

            _gridBackColorCaches[grid] =
                new GridBackColorCache(
                    columnGroupIndexes,
                    rowGroupBackColors,
                    rowGroupSelectionBackColors,
                    rowGroupHasBackColor);
        }

        private string ResolveBottleCrossPointNodeKeyColumnName(DataGridView grid, int columnIndex)
        {
            if (grid == null || columnIndex < 0 || columnIndex >= grid.Columns.Count)
                return null;

            var column = grid.Columns[columnIndex];
            if (column == null)
                return null;

            string name = column.Name;
            if (string.Equals(name, "NodeKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "DisplayKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "ItemCode", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "MasterKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "StartDateLabel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "InputSourceType", StringComparison.OrdinalIgnoreCase))
                return null;

            DataTable table = ResolveGridDataTable(grid);
            if (table == null || !table.Columns.Contains("NodeKey"))
                return null;

            return "NodeKey";
        }

        private void OnBottleCrossPointNodeBackColorFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            GridBackColorCache cache;
            if (!_gridBackColorCaches.TryGetValue(grid, out cache))
                return;

            if (e.RowIndex >= cache.RowGroupHasBackColor.GetLength(0) ||
                e.ColumnIndex >= cache.ColumnGroupIndexes.Length)
                return;

            Color backColor;
            Color selectionBackColor;
            if (!cache.TryGetBackColor(e.RowIndex, e.ColumnIndex, out backColor, out selectionBackColor))
                return;

            e.CellStyle.BackColor = backColor;
            e.CellStyle.SelectionBackColor = selectionBackColor;
        }

        private void OnBottleIntersectionGridCrossPointBackColorFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            var column = grid.Columns[e.ColumnIndex];
            if (column == null || !column.Visible)
                return;

            var boundItem = grid.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (boundItem == null || boundItem.Row == null)
                return;

            if (GetTableInt(boundItem.Row, "交点") != 1)
                return;

            string nodeKey = GetTableString(boundItem.Row, "NodeKey");
            if (string.IsNullOrWhiteSpace(nodeKey))
                return;

            var colors = GetCrossPointNodeColors(nodeKey);
            e.CellStyle.BackColor = colors.Item1;
            e.CellStyle.SelectionBackColor = colors.Item2;
        }

        private string BuildBottleCrossPointUiKeyFromRow(DataRow row)
        {
            if (row == null)
                return null;

            string masterKey = GetTableString(row, "MasterKey");
            string nodeKey = GetTableString(row, "NodeKey");
            string startDateLabel = GetTableString(row, "StartDateLabel");
            string inputSourceType = GetTableString(row, "InputSourceType");

            bool isManual =
                string.Equals(startDateLabel, "手投入", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(inputSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase);

            if (isManual)
                return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey.Trim();

            if (!string.IsNullOrWhiteSpace(masterKey))
                return "MK|" + masterKey.Trim();

            return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey.Trim();
        }

        private Tuple<Color, Color> GetCrossPointNodeColors(string nodeKey)
        {
            if (string.IsNullOrWhiteSpace(nodeKey))
                return Tuple.Create(Color.Empty, Color.Empty);

            Tuple<Color, Color> colors;
            if (_crossPointColorsByNodeKey.TryGetValue(nodeKey, out colors))
                return colors;

            colors = Tuple.Create(
                GetCrossPointNodeBackColor(nodeKey),
                GetCrossPointNodeSelectionBackColor(nodeKey));
            _crossPointColorsByNodeKey[nodeKey] = colors;

            return colors;
        }

        private Color GetCrossPointNodeBackColor(string nodeKey)
        {
            int hash = GetStablePositiveHash(nodeKey);
            double hue = hash % 360;
            double saturation = 0.18 + ((hash / 360) % 8) * 0.01;
            double value = 0.98;

            return ConvertHsvToColor(hue, saturation, value);
        }

        private Color GetCrossPointNodeSelectionBackColor(string nodeKey)
        {
            int hash = GetStablePositiveHash(nodeKey);
            double hue = hash % 360;
            double saturation = 0.38 + ((hash / 360) % 8) * 0.01;
            double value = 0.74;

            return ConvertHsvToColor(hue, saturation, value);
        }

        private int GetStablePositiveHash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            unchecked
            {
                int hash = 23;
                foreach (char c in value)
                    hash = hash * 31 + c;

                return hash == int.MinValue ? int.MaxValue : Math.Abs(hash);
            }
        }

        #endregion

        #region 罫線描画系

        private void BuildLineColorCache(int tabNo, BottleDisplayTables tableSource)
        {
            var cacheList = new List<LineCache>();

            if (tableSource != null && tableSource.LineRanges != null)
            {
                foreach (var line in tableSource.LineRanges)
                {
                    cacheList.Add(new LineCache
                    {
                        RowIndex = line.BorderIndex,
                        Color = Color.FromArgb(120, 72, 32)
                    });
                }
            }

            _lineCache[tabNo] = cacheList;
        }

        private List<LineCache> GetCurrentLineCache()
        {
            int tabNo = GetCurrentBottleTraceTabNo();

            List<LineCache> cache;
            if (_lineCache.TryGetValue(tabNo, out cache))
                return cache;

            return null;
        }


        private void LiquidTableBorderPaint(object sender, PaintEventArgs e)
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

            var caches = GetCurrentLineCache();
            if (caches == null) return;

            foreach (var cache in caches)
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

            var caches = GetCurrentLineCache();
            if (caches == null) return;

            foreach (var cache in caches)
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


        #endregion

        #region CSV処理系

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

        private void ExportCsvForTab(BottleTraceTabContext tab, BottleDisplayTables tables)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "CSV出力";
                dlg.Filter = "CSV ファイル (*.csv)|*.csv";
                dlg.DefaultExt = "csv";
                dlg.AddExtension = true;
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.FileName = BuildBottleCsvExportFileName(tab);
                ApplyOutputInitialDirectory(dlg, DefaultCsvDirectoryIniKey);

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                CsvExportHelper.ExportCurrentGridsToCsv(
                    dlg.FileName,
                    tab.GridStart,
                    null,
                    tab.GridEnd);
            }

            MessageBox.Show("CSV 出力が完了しました。", "瓶設備ロットトレース",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string BuildBottleCsvExportFileName(BottleTraceTabContext tab)
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            int tabNo = tab == null ? 0 : tab.TabNo;
            return "BottleTrace_Tab" + tabNo + "_" + suffix + ".csv";
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

        #endregion

        #region EXCEL出力

        private void btnBottleExcelOutput_Click(object sender, EventArgs e)
        {
            try
            {
                RebuildSelectedTraceTargetTabsFromCheckBoxes();

                var sheets = BuildBottleExcelExportRequests();
                bool hasIntersectionData = HasAnyVisibleData(dataGridIntersection);

                if (sheets.Count == 0 && !hasIntersectionData)
                {
                    MessageBox.Show(
                        this,
                        "出力対象の表示データがありません。\r\n" +
                        "通常タブはチェックボックスで出力対象を選択してください。",
                        "EXCEL出力",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = "EXCEL出力";
                    sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                    sfd.DefaultExt = "xlsx";
                    sfd.AddExtension = true;
                    sfd.OverwritePrompt = true;
                    sfd.RestoreDirectory = true;
                    sfd.FileName = BuildBottleExcelExportFileName();
                    ApplyOutputInitialDirectory(sfd, DefaultExcelDirectoryIniKey);

                    if (sfd.ShowDialog(this) != DialogResult.OK)
                        return;

                    ExcelExportHelper.ExportBottleTraceSheetsToExcel(
                        sfd.FileName,
                        sheets,
                        hasIntersectionData ? dataGridIntersection : null,
                        "BottleCrossPoints");

                    MessageBox.Show(
                        this,
                        "EXCEL出力が完了しました。\r\n" + sfd.FileName,
                        "EXCEL出力",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "EXCEL出力に失敗しました。\r\n" + ex.Message,
                    "EXCEL出力エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private List<ExcelExportHelper.BottleTraceGridExcelExportRequest> BuildBottleExcelExportRequests()
        {
            var requests = new List<ExcelExportHelper.BottleTraceGridExcelExportRequest>();

            foreach (int tabNo in _selectedTraceTargetTabs.OrderBy(x => x))
            {
                BottleDisplayTables tables;
                if (!_tabDisplayTables.TryGetValue(tabNo, out tables) || tables == null)
                    continue;

                var tab = GetTabContext(tabNo);
                if (tab == null || !HasAnyVisibleDataForBottleExcelExport(tab))
                    continue;

                HashSet<string> crossPointNodeKeys;
                _crossPointNodeKeysByTab.TryGetValue(tabNo, out crossPointNodeKeys);

                requests.Add(new ExcelExportHelper.BottleTraceGridExcelExportRequest
                {
                    WorksheetName = BuildBottleTraceTabName(tab),
                    LeftGrid = tab.GridStart,
                    RightGrid = tab.GridEnd,
                    LineRanges = tables.LineRanges,
                    CrossPointNodeKeys = crossPointNodeKeys
                });
            }

            return requests;
        }

        private bool HasAnyVisibleDataForBottleExcelExport(BottleTraceTabContext tab)
        {
            if (tab == null)
                return false;

            return HasAnyVisibleData(tab.GridStart)
                || HasAnyVisibleData(tab.GridEnd);
        }

        private string BuildBottleExcelExportFileName()
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return "BottleTrace_Export_" + suffix + ".xlsx";
        }

        private bool HasAnyVisibleData(DataGridView grid)
        {
            if (grid == null)
                return false;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (col == null || !col.Visible)
                        continue;

                    object value = row.Cells[col.Index].Value;
                    if (value == null || value == DBNull.Value)
                        continue;

                    string text = Convert.ToString(value);
                    if (!string.IsNullOrWhiteSpace(text))
                        return true;
                }
            }

            return false;
        }

        private void ApplyOutputInitialDirectory(SaveFileDialog dialog, string iniKey)
        {
            if (dialog == null)
                return;

            string directory = GetOutputInitialDirectory(iniKey);
            if (!string.IsNullOrWhiteSpace(directory))
                dialog.InitialDirectory = directory;
        }

        private string GetOutputInitialDirectory(string iniKey)
        {
            if (string.IsNullOrWhiteSpace(iniKey))
                return null;

            try
            {
                string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LotTraceApp.ini");
                if (!File.Exists(iniPath))
                    return null;

                var ini = new IniFile(iniPath);
                string directory = ini.GetString(OutputIniSection, iniKey, null);
                if (string.IsNullOrWhiteSpace(directory))
                    return null;

                directory = Environment.ExpandEnvironmentVariables(directory.Trim());
                if (directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return null;

                if (!Path.IsPathRooted(directory))
                    directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);

                directory = Path.GetFullPath(directory);
                Directory.CreateDirectory(directory);
                return directory;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region クリアボタン系

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
            _tabBottleTraceResults.Remove(tab.TabNo);
            _lineCache.Remove(tab.TabNo);
            _gridForeColorCaches.Remove(tab.GridStart);
            _gridForeColorCaches.Remove(tab.GridEnd);
            ClearBottleCrossPointNodeKeysForTab(tab.TabNo);
            ClearBottleGridBackColorCachesForTab(tab.TabNo);
            ClearBottleTraceTabGrids(tab);
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
            _gridForeColorCaches.Remove(grid);
            _gridBackColorCaches.Remove(grid);
        }

        private void ClearBottleCrossPointNodeKeysForTab(int tabNo)
        {
            if (tabNo <= 0)
                return;

            _crossPointNodeKeysByTab.Remove(tabNo);
        }

        private void ClearBottleGridBackColorCachesForTab(int tabNo)
        {
            var tab = GetTabContext(tabNo);
            if (tab == null)
                return;

            if (tab.GridStart != null) _gridBackColorCaches.Remove(tab.GridStart);
            if (tab.GridEnd != null) _gridBackColorCaches.Remove(tab.GridEnd);
        }

        #endregion

        #region 画面遷移

        private void btnBackToLiquid_Click(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion


        #region 交点検出

        private void RebuildSelectedTraceTargetTabsFromCheckBoxes()
        {
            _selectedTraceTargetTabs.Clear();

            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var checkBox = GetTraceTargetCheckBox(tabNo);
                if (checkBox != null && checkBox.Checked)
                    _selectedTraceTargetTabs.Add(tabNo);
            }
        }

        private void btnBottleDetectCrossPoints_Click(object sender, EventArgs e)
        {
            RebuildSelectedTraceTargetTabsFromCheckBoxes();

            if (_selectedTraceTargetTabs.Count == 0)
            {
                MessageBox.Show("交点検出対象のタブが選択されていません。\r\n" +
                                "チェックボックスを ON にしてください。",
                                "交点検出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var targets = new Dictionary<int, BottleTraceResult>();
            foreach (int tabNo in _selectedTraceTargetTabs)
            {
                BottleTraceResult result;
                if (_tabBottleTraceResults.TryGetValue(tabNo, out result) && result != null)
                    targets[tabNo] = result;
            }

            if (targets.Count == 0)
            {
                MessageBox.Show("交点検出対象タブにトレース結果がありません。",
                    "交点検出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _lastBottleCrossPoints = B_DetectCrossPointsByNodeKey(targets);
                _lastBottleCrossPointTargetTabs.Clear();
                _lastBottleCrossPointTargetTabs.AddRange(targets.Keys.OrderBy(x => x));

                dataGridIntersection.DataSource = B_CreateCrossPointGridTable(
                    _lastBottleCrossPoints,
                    _lastBottleCrossPointTargetTabs);
                B_ApplyCrossPointGridColumnWidths();
                StoreBottleCrossPointKeysByTab(_lastBottleCrossPoints, _lastBottleCrossPointTargetTabs);
                BuildBottleGridBackColorCaches();
                InvalidateBottleTraceGridsForTabs(_lastBottleCrossPointTargetTabs);
                swichBottleTab.SelectedTab = BottleIntersectionTab;
            }
            catch (Exception ex)
            {
                MessageBox.Show("交点検出中にエラーが発生しました。\r\n" + ex.Message,
                    "交点検出エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<B_CrossPointRecord> B_DetectCrossPointsByNodeKey(
            Dictionary<int, BottleTraceResult> tabResults)
        {
            var tabsByKey = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
            var repByKey = new Dictionary<string, B_CrossPointRecord>(StringComparer.OrdinalIgnoreCase);

            if (tabResults == null || tabResults.Count == 0)
                return new List<B_CrossPointRecord>();

            foreach (var kv in tabResults)
            {
                int tabNo = kv.Key;
                var result = kv.Value;
                B_CollectCrossPointNodes(tabNo, result, tabsByKey, repByKey);
            }

            var allTabs = tabResults.Keys.OrderBy(x => x).ToList();
            var records = new List<B_CrossPointRecord>();

            foreach (var kv in tabsByKey)
            {
                string key = kv.Key;
                var tabs = kv.Value;
                B_CrossPointRecord record;
                if (!repByKey.TryGetValue(key, out record) || record == null)
                    continue;

                record.CrossPointFlag = tabs.Count >= 2 ? 1 : 0;
                foreach (int tabNo in allTabs)
                    record.TabPresence[tabNo] = tabs.Contains(tabNo) ? 1 : 0;

                records.Add(record);
            }

            return records
                .OrderByDescending(x => x.CrossPointFlag)
                .ThenBy(x => x.ProductionOrderNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LotNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.ItemName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.NodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void B_CollectCrossPointNodes(
            int tabNo,
            BottleTraceResult traceResult,
            Dictionary<string, HashSet<int>> tabsByKey,
            Dictionary<string, B_CrossPointRecord> repByKey)
        {
            if (traceResult == null || traceResult.DisplayGroups == null ||
                tabsByKey == null || repByKey == null)
                return;

            foreach (var group in traceResult.DisplayGroups)
            {
                if (group == null)
                    continue;

                if (group.LiquidNodes != null)
                {
                    foreach (var laneNode in group.LiquidNodes)
                        B_AddCrossPointNode(tabNo, laneNode, tabsByKey, repByKey);
                }

                if (group.BottleNodes != null)
                {
                    foreach (var laneNode in group.BottleNodes)
                        B_AddCrossPointNode(tabNo, laneNode, tabsByKey, repByKey);
                }
            }
        }

        private void B_AddCrossPointNode(
            int tabNo,
            BottleDisplayLaneNode laneNode,
            Dictionary<string, HashSet<int>> tabsByKey,
            Dictionary<string, B_CrossPointRecord> repByKey)
        {
            if (laneNode == null || tabsByKey == null || repByKey == null)
                return;

            string key = B_BuildCrossPointUiKey(laneNode);
            if (string.IsNullOrWhiteSpace(key))
                return;

            HashSet<int> tabs;
            if (!tabsByKey.TryGetValue(key, out tabs))
            {
                tabs = new HashSet<int>();
                tabsByKey[key] = tabs;
                repByKey[key] = B_CreateCrossPointRecordFromNode(key, laneNode);
            }

            tabs.Add(tabNo);
        }

        private B_CrossPointRecord B_CreateCrossPointRecordFromNode(
            string key,
            BottleDisplayLaneNode laneNode)
        {
            var record = new B_CrossPointRecord
            {
                NodeKey = key,
                NodeType = laneNode == null ? 0 : laneNode.NodeType
            };

            if (laneNode != null && laneNode.SourceLiquidNode != null)
            {
                var node = laneNode.SourceLiquidNode;
                record.ProductionOrderNumber = node.ProductionOrderNumber;
                record.LotNumber = node.LotNumber;
                record.ItemName = node.ItemName;
                record.StartDateText = node.StartDate.HasValue
                    ? node.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    : node.StartDateLabel;
                record.Weight = node.Weight.HasValue ? (float?)Convert.ToSingle(node.Weight.Value) : null;
            }

            if (laneNode != null && laneNode.SourceBottleNode != null)
            {
                var node = laneNode.SourceBottleNode;
                record.ProductionOrderNumber = node.OrderNumber;
                record.LotNumber = node.ProductLotNumber;
                record.ItemName = node.ProductItemName;
                record.StartDateText = node.StartDate.HasValue
                    ? node.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    : string.Empty;
                record.FillingBottleNum_Total = node.FillingBottleNum_Total;
            }

            return record;
        }

        private string B_BuildCrossPointUiKey(BottleDisplayLaneNode laneNode)
        {
            if (laneNode == null)
                return null;

            if (laneNode.SourceLiquidNode != null)
                return B_BuildCrossPointUiKey(laneNode.SourceLiquidNode);

            if (laneNode.SourceBottleNode != null)
            {
                string nodeKey = (laneNode.SourceBottleNode.NodeIdentifyKey ?? string.Empty).Trim();
                return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey;
            }

            return null;
        }

        private string B_BuildCrossPointUiKey(ProductionResultNode node)
        {
            if (node == null)
                return null;

            bool isManual =
                string.Equals(node.StartDateLabel, "手投入", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(node.InputSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase);

            string masterKey = (node.ControlMasterKey ?? string.Empty).Trim();
            string nodeKey = (node.NodeIdentityKey ?? string.Empty).Trim();

            if (isManual)
                return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey;

            if (!string.IsNullOrWhiteSpace(masterKey))
                return "MK|" + masterKey;

            return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey;
        }

        private void StoreBottleCrossPointKeysByTab(
            IEnumerable<B_CrossPointRecord> records,
            IEnumerable<int> targetTabs)
        {
            _crossPointNodeKeysByTab.Clear();
            _crossPointColorsByNodeKey.Clear();

            if (targetTabs != null)
            {
                foreach (int tabNo in targetTabs)
                {
                    if (!_crossPointNodeKeysByTab.ContainsKey(tabNo))
                    {
                        _crossPointNodeKeysByTab[tabNo] =
                            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }
                }
            }

            if (records == null)
                return;

            foreach (var record in records)
            {
                if (record == null || record.CrossPointFlag != 1 ||
                    string.IsNullOrWhiteSpace(record.NodeKey))
                    continue;

                foreach (int tabNo in _crossPointNodeKeysByTab.Keys.ToList())
                {
                    if (record.GetTabPresence(tabNo) != 1)
                        continue;

                    _crossPointNodeKeysByTab[tabNo].Add(record.NodeKey.Trim());
                }
            }
        }

        private void InvalidateBottleTraceGridsForTabs(IEnumerable<int> tabNos)
        {
            if (tabNos == null)
                return;

            foreach (int tabNo in tabNos)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                if (tab.GridStart != null) tab.GridStart.Invalidate();
                if (tab.GridEnd != null) tab.GridEnd.Invalidate();
            }
        }

        private float? B_GetNullableFloat(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || string.IsNullOrEmpty(columnName) ||
                !row.Table.Columns.Contains(columnName))
                return null;

            object value = row[columnName];
            if (value == null || value == DBNull.Value)
                return null;

            float result;
            return float.TryParse(Convert.ToString(value), out result) ? (float?)result : null;
        }

        private void btnBottleIntersectionClear_Click(object sender, EventArgs e)
        {
            _lastBottleCrossPoints = null;
            _lastBottleCrossPointTargetTabs.Clear();
            _crossPointNodeKeysByTab.Clear();
            _crossPointColorsByNodeKey.Clear();
            _gridBackColorCaches.Clear();
            InvalidateBottleTraceGridsForTabs(Enumerable.Range(1, 10));

            if (dataGridIntersection != null)
            {
                dataGridIntersection.DataSource = B_CreateCrossPointGridTable(null, null);
                B_ApplyCrossPointGridColumnWidths();
                dataGridIntersection.Invalidate();
            }
        }

        private void btnBottleIntersectionCsv_Click(object sender, EventArgs e)
        {
            if (_lastBottleCrossPoints == null || _lastBottleCrossPoints.Count == 0)
            {
                MessageBox.Show("交点検出結果がありません。", "CSV出力",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "交点CSV出力";
                dlg.Filter = "CSV ファイル (*.csv)|*.csv";
                dlg.DefaultExt = "csv";
                dlg.AddExtension = true;
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.FileName = B_BuildCrossPointCsvExportFileName();
                ApplyOutputInitialDirectory(dlg, DefaultCsvDirectoryIniKey);

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                var table = B_CreateCrossPointGridTable(
                    _lastBottleCrossPoints,
                    _lastBottleCrossPointTargetTabs);

                if (table.Columns.Contains("NodeKey"))
                    table.Columns.Remove("NodeKey");

                try
                {
                    ExportHelper.ExportDataTableToCsv(table, dlg.FileName);
                    MessageBox.Show("CSV 出力が完了しました。", "CSV出力",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CSV 出力に失敗しました。\r\n" + ex.Message,
                        "CSV出力エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string B_BuildCrossPointCsvExportFileName()
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return "BottleCrossPoints_" + suffix + ".csv";
        }

        /// <summary>
        /// 交点検出結果 1 行分
        /// </summary>
        public class B_CrossPointRecord
        {
            public B_CrossPointRecord()
            {
                TabPresence = new Dictionary<int, int>();
            }

            public int NodeType {  get; set; }

            /// <summary>
            /// NodeIdentityKey 由来のNode識別子。
            /// 交点判定、通常グリッドのセル強調に使用する。
            /// </summary>
            public string NodeKey { get; set; }

            /// <summary>
            /// 複数の対象タブに存在する場合は 1、それ以外は 0。
            /// CSV/Excel出力で扱いやすいよう数値で保持する。
            /// </summary>
            public int CrossPointFlag { get; set; }

            public string ProductionOrderNumber { get; set; }
            public string LotNumber { get; set; }
            public string ItemName { get; set; }
            public string StartDateText { get; set; }
            public float? Weight { get; set; }
            public int FillingBottleNum_Total {  get; set; }

            /// <summary>
            /// 内部保持用。画面/CSV/Excelでは対象タブだけを列化し、
            /// 値は必ず 1 または 0 とする。
            /// </summary>
            public Dictionary<int, int> TabPresence { get; private set; }

            public int GetTabPresence(int tabNo)
            {
                int value;
                return TabPresence != null && TabPresence.TryGetValue(tabNo, out value)
                    ? value
                    : 0;
            }
        }


        private DataTable B_CreateCrossPointGridTable(
            IEnumerable<B_CrossPointRecord> records,
            IEnumerable<int> targetTabs)
        {
            var table = new DataTable();

            table.Columns.Add("NodeKey", typeof(string));
            table.Columns.Add("交点", typeof(int));
            table.Columns.Add("製造指図番号", typeof(string));
            table.Columns.Add("ロットNo.", typeof(string));
            table.Columns.Add("品目名", typeof(string));
            table.Columns.Add("開始日時", typeof(string));
            table.Columns.Add("重量", typeof(float));
            table.Columns.Add("充填本数", typeof(float));

            if (targetTabs != null)
            {
                foreach (int tabNo in targetTabs
                    .Where(x => x > 0)
                    .Distinct()
                    .OrderBy(x => x))
                {
                    table.Columns.Add("タブ" + tabNo, typeof(int));
                }
            }

            if (records != null)
            {
                var tabs = targetTabs == null
                    ? new List<int>()
                    : targetTabs.Where(x => x > 0).Distinct().OrderBy(x => x).ToList();

                foreach (var record in records)
                {
                    if (record == null)
                        continue;

                    var row = table.NewRow();
                    row["NodeKey"] = record.NodeKey ?? string.Empty;
                    row["交点"] = record.CrossPointFlag;
                    row["製造指図番号"] = record.ProductionOrderNumber ?? string.Empty;
                    row["ロットNo."] = record.LotNumber ?? string.Empty;
                    row["品目名"] = record.ItemName ?? string.Empty;
                    row["開始日時"] = record.StartDateText ?? string.Empty;
                    row["重量"] = record.Weight.HasValue ? (object)record.Weight.Value : DBNull.Value;
                    row["充填本数"] = record.FillingBottleNum_Total;

                    foreach (int tabNo in tabs)
                        row["タブ" + tabNo] = record.GetTabPresence(tabNo);

                    table.Rows.Add(row);
                }
            }

            return table;
        }

        private void B_ApplyCrossPointGridColumnWidths()
        {
            if (dataGridIntersection == null || dataGridIntersection.Columns.Count == 0)
                return;

            dataGridIntersection.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            foreach (DataGridViewColumn col in dataGridIntersection.Columns)
            {
                if (col == null)
                    continue;

                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Resizable = DataGridViewTriState.True;

                switch (col.Name)
                {
                    case "NodeKey":
                        col.Visible = false;
                        col.Width = 5;
                        break;
                    case "交点":
                        col.Width = 60;
                        break;
                    case "製造指図番号":
                    case "ロットNo.":
                        col.Width = 150;
                        break;
                    case "品目名":
                        col.Width = 400;
                        break;
                    case "開始日時":
                        col.Width = 180;
                        break;
                    case "重量":
                        col.Width = 100;
                        break;
                    case "充填本数":
                        col.Width = 100;
                        break;
                    default:
                        if (col.Name.StartsWith("タブ", StringComparison.OrdinalIgnoreCase))
                            col.Width = 70;
                        break;
                }

                col.MinimumWidth = col.Width;
                B_ApplyCrossPointGridCellStyle(col);
            }

            B_ApplyIntersectionTabLayout();
        }

        private void B_ApplyCrossPointGridCellStyle(DataGridViewColumn col)
        {
            if (col == null)
                return;

            if (string.Equals(col.Name, "交点", StringComparison.OrdinalIgnoreCase) ||
                col.Name.StartsWith("タブ", StringComparison.OrdinalIgnoreCase))
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Padding = Padding.Empty;
                return;
            }

            if (string.Equals(col.Name, "重量", StringComparison.OrdinalIgnoreCase))
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.DefaultCellStyle.Padding = new Padding(0, 0, 8, 0);
                return;
            }

            if (string.Equals(col.Name, "充填本数", StringComparison.OrdinalIgnoreCase))
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.DefaultCellStyle.Padding = new Padding(0, 0, 8, 0);
                return;
            }

            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            col.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        }

        private void B_ApplyIntersectionTabLayout()
        {
            if (BottleIntersectionTab == null || dataGridIntersection == null)
                return;

            int margin = 16;
            int buttonTop = 14;
            int buttonWidth = 150;
            int buttonHeight = 40;
            int buttonGap = 16;

            if (_btnIntersectionClear != null)
            {
                _btnIntersectionClear.Size = new Size(buttonWidth, buttonHeight);
                _btnIntersectionClear.Location = new Point(
                    BottleIntersectionTab.ClientSize.Width - margin - buttonWidth,
                    buttonTop);
            }

            if (_btnIntersectionCsv != null)
            {
                _btnIntersectionCsv.Size = new Size(buttonWidth, buttonHeight);
                _btnIntersectionCsv.Location = new Point(
                    BottleIntersectionTab.ClientSize.Width - margin - buttonWidth * 2 - buttonGap,
                    buttonTop);
            }

            int gridTop = 64;
            dataGridIntersection.Location = new Point(margin, gridTop);
            dataGridIntersection.Size = new Size(
                BottleIntersectionTab.ClientSize.Width - margin * 2,
                BottleIntersectionTab.ClientSize.Height - gridTop - margin);
            dataGridIntersection.Anchor = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;
        }

        #endregion
    }
}
