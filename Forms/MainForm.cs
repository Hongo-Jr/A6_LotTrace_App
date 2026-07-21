using LotTraceApp.Models;
using LotTraceApp.Services;
using LotTraceApp.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LotTraceApp
{
    public partial class MainForm : Form
    {
        private const string OutputIniSection = "Output";
        private const string DefaultExcelDirectoryIniKey = "DefaultExcelDirectory";
        private const string DefaultCsvDirectoryIniKey = "DefaultCsvDirectory";

        private readonly LotTraceService _liquidService;
        private readonly BottleTraceService _bottleService;
        private readonly ResultService _resultService;
        private readonly BottleResultService _bottelResultService;
        private CommandLineOptions _commandLineStartupOptions;
        private BottleTraceForm _bottleTraceForm;
        private bool _disposingBottleTraceForm;

        
        // タブ番号 → トレース結果（交点検出・EXCEL出力用）
        private readonly Dictionary<int, TraceResult> _tabTraceResults =
            new Dictionary<int, TraceResult>();

        // タブ番号 → 交点検出などの対象タブ
        private readonly HashSet<int> _selectedTraceTargetTabs =
            new HashSet<int>();

        // 交点検出の最終結果
        private List<CrossPointRecord> _lastCrossPoints;
        private List<int> _lastCrossPointTargetTabs = new List<int>();
        private readonly Dictionary<int, HashSet<string>> _crossPointNodeKeysByTab =
            new Dictionary<int, HashSet<string>>();
        private readonly Dictionary<string, Tuple<Color, Color>> _crossPointColorsByNodeKey =
            new Dictionary<string, Tuple<Color, Color>>(StringComparer.Ordinal);
        private Button _btnIntersectionCsv;
        private Button _btnIntersectionClear;

        // ★ 現在タブの全ノード（親子関係を含めたリスト）
        private List<ProductionResultNode> _currentAllNodes =
            new List<ProductionResultNode>();

        private int _currentMaxDepth;
        private bool _syncingScroll = false;
        
        private bool _fixedGridLayoutApplied = false;

        private string _currentItemToolTipText = string.Empty;

        // タブ番号 → 表示結果（サービスが確定した描画根拠）
        private readonly Dictionary<int, TraceDisplayResult> _tabDisplayResults
            = new Dictionary<int, TraceDisplayResult>();

        // タブ番号 → UI描画用の受け皿
        private readonly Dictionary<int, TraceGridDrawContext> _tabDrawContexts
            = new Dictionary<int, TraceGridDrawContext>();


        //Grid描画用キャッシュDTO。スクロールなどのイベントから呼ばれる
        private sealed class GridPaintCache
        {
            public readonly HashSet<int> StartBottomDividerRows
                = new HashSet<int>();

            public readonly List<EndHorizontalLineDrawInfo> EndHorizontalLines
                = new List<EndHorizontalLineDrawInfo>();

            public readonly List<MiddleHorizontalLineDrawInfo> MiddleHorizontalLines
                = new List<MiddleHorizontalLineDrawInfo>();

            public readonly Dictionary<DataGridView, GridForeColorCache> ForeColorCaches
                = new Dictionary<DataGridView, GridForeColorCache>();
            public readonly Dictionary<DataGridView, GridBackColorCache> BackColorCaches
                = new Dictionary<DataGridView, GridBackColorCache>();

            public readonly Dictionary<DataGridView, GridLinePaintCache> LineCaches
                = new Dictionary<DataGridView, GridLinePaintCache>();

            public void Clear()
            {
                StartBottomDividerRows.Clear();
                EndHorizontalLines.Clear();
                MiddleHorizontalLines.Clear();
                ForeColorCaches.Clear();
                BackColorCaches.Clear();
                LineCaches.Clear();
            }
        }

        private sealed class GridLinePaintCache
        {
            public readonly List<CachedHorizontalGridLine> HorizontalLines
                = new List<CachedHorizontalGridLine>();

            public readonly List<CachedVerticalGridLine> VerticalLines
                = new List<CachedVerticalGridLine>();
        }

        private sealed class CachedHorizontalGridLine
        {
            public int RowIndex { get; set; }
            public int StartXOffset { get; set; }
            public int EndXOffset { get; set; }
            public Color Color { get; set; }
            public float Width { get; set; }
        }

        private sealed class CachedVerticalGridLine
        {
            public int XOffset { get; set; }
            public Color Color { get; set; }
            public float Width { get; set; }
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

            public bool TryGetBackColor(int rowIndex, int columnIndex, out Color backColor, out Color selectionBackColor)
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

        private sealed class TraceSearchWorkResult
        {
            public TraceResult Result { get; set; }
            public TraceDisplayResult DisplayResult { get; set; }
            public DataTable DisplayTable { get; set; }
        }
        private readonly GridPaintCache _gridPaintCache = new GridPaintCache();



        private sealed class HeaderVisualStyle
        {
            public Color GroupBackColor { get; set; }
            public Color GroupForeColor { get; set; }
            public Color BorderColor { get; set; }
            public Font GroupFont { get; set; }
            public Font ColumnFont { get; set; }
        }
        // タブごとの表示テーブルも保持（切替時に再表示するため）
        private readonly Dictionary<int, DataTable> _tabDisplayTables = new Dictionary<int, DataTable>();

        // タブ番号 → UIコントロールとヘッダー状態
        private readonly Dictionary<int, TraceTabContext> _traceTabContexts =
            new Dictionary<int, TraceTabContext>();

        private sealed class TraceTabContext
        {
            public int TabNo { get; set; }

            public TextBox TxtOrder { get; set; }
            public TextBox TxtItemName { get; set; }
            public TextBox TxtItemCode { get; set; }
            public TextBox TxtLot { get; set; }

            public CheckBox ChkUseFrom { get; set; }
            public DateTimePicker DtpFrom { get; set; }
            public DateTimePicker DtpTo { get; set; }

            public RadioButton RdoForward { get; set; }
            public RadioButton RdoBackward { get; set; }

            public Button BtnSearch { get; set; }
            public Button BtnClear { get; set; }
            public Button BtnCsv { get; set; }

            public Panel PnlStartHeader { get; set; }
            public Panel PnlMiddleHeader { get; set; }
            public Panel PnlEndHeader { get; set; }

            public DataGridView GridStart { get; set; }
            public DataGridView GridMiddle { get; set; }
            public DataGridView GridEnd { get; set; }

            public Label StartHeaderTitleLabel { get; set; }
            public Label EndHeaderTitleLabel { get; set; }
            public Panel MiddleHeaderInnerPanel { get; set; }
            public List<Label> MiddleHeaderLevelLabels { get; private set; }

            public TraceTabContext()
            {
                MiddleHeaderLevelLabels = new List<Label>();
            }
        }

        private int GetCurrentTraceTabNo()
        {
            int tabNo = this.swichTab.SelectedIndex + 1;   // 0-based -> 1-based
            return (tabNo >= 1 && tabNo <= 10) ? tabNo : -1;
        }

        private TraceTabContext GetTabContext(int tabNo)
        {
            TraceTabContext tab;
            return _traceTabContexts.TryGetValue(tabNo, out tab) ? tab : null;
        }

        private void InitializeTraceTabContexts()
        {
            _traceTabContexts.Clear();

            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = CreateTraceTabContext(tabNo);
                if (tab != null)
                    _traceTabContexts[tabNo] = tab;
            }
        }

        private TraceTabContext CreateTraceTabContext(int tabNo)
        {
            switch (tabNo)
            {
                case 1:
                    return new TraceTabContext
                    {
                        TabNo = 1,
                        TxtOrder = txtProductionOrderNumber,
                        TxtItemName = txtItemName,
                        TxtItemCode = txtItemCode,
                        TxtLot = txtLotNumber,
                        ChkUseFrom = chkUseFrom,
                        DtpFrom = dtpFrom,
                        DtpTo = dtpTo,
                        RdoForward = rdoForward,
                        RdoBackward = rdoBackward,
                        BtnSearch = btnTraceSearch,
                        BtnClear = btnClear,
                        BtnCsv = btnCsvOutput,
                        PnlStartHeader = panelStartHeader,
                        PnlMiddleHeader = panelMiddleHeader,
                        PnlEndHeader = panelEndHeader,
                        GridStart = dataGridStart,
                        GridMiddle = dataGridMiddle,
                        GridEnd = dataGridEnd
                    };

                case 2:
                    return new TraceTabContext
                    {
                        TabNo = 2,
                        TxtOrder = textBox1,
                        TxtItemName = textBox2,
                        TxtItemCode = textBox3,
                        TxtLot = textBox4,
                        ChkUseFrom = checkBox2,
                        DtpFrom = dateTimePicker1,
                        DtpTo = dateTimePicker2,
                        RdoForward = radioButton2,
                        RdoBackward = radioButton1,
                        BtnSearch = button3,
                        BtnClear = button1,
                        BtnCsv = button2,
                        PnlStartHeader = panel1,
                        PnlMiddleHeader = panel2,
                        PnlEndHeader = panel3,
                        GridStart = dataGridView1,
                        GridMiddle = dataGridView2,
                        GridEnd = dataGridView3
                    };

                case 3:
                    return new TraceTabContext
                    {
                        TabNo = 3,
                        TxtOrder = textBox5,
                        TxtItemName = textBox6,
                        TxtItemCode = textBox7,
                        TxtLot = textBox8,
                        ChkUseFrom = checkBox3,
                        DtpFrom = dateTimePicker3,
                        DtpTo = dateTimePicker4,
                        RdoForward = radioButton4,
                        RdoBackward = radioButton3,
                        BtnSearch = button6,
                        BtnClear = button4,
                        BtnCsv = button5,
                        PnlStartHeader = panel4,
                        PnlMiddleHeader = panel5,
                        PnlEndHeader = panel6,
                        GridStart = dataGridView4,
                        GridMiddle = dataGridView5,
                        GridEnd = dataGridView6
                    };

                case 4:
                    return new TraceTabContext
                    {
                        TabNo = 4,
                        TxtOrder = textBox9,
                        TxtItemName = textBox10,
                        TxtItemCode = textBox11,
                        TxtLot = textBox12,
                        ChkUseFrom = checkBox4,
                        DtpFrom = dateTimePicker5,
                        DtpTo = dateTimePicker6,
                        RdoForward = radioButton6,
                        RdoBackward = radioButton5,
                        BtnSearch = button9,
                        BtnClear = button7,
                        BtnCsv = button8,
                        PnlStartHeader = panel7,
                        PnlMiddleHeader = panel8,
                        PnlEndHeader = panel9,
                        GridStart = dataGridView7,
                        GridMiddle = dataGridView8,
                        GridEnd = dataGridView9
                    };

                case 5:
                    return new TraceTabContext
                    {
                        TabNo = 5,
                        TxtOrder = textBox13,
                        TxtItemName = textBox14,
                        TxtItemCode = textBox15,
                        TxtLot = textBox16,
                        ChkUseFrom = checkBox5,
                        DtpFrom = dateTimePicker7,
                        DtpTo = dateTimePicker8,
                        RdoForward = radioButton8,
                        RdoBackward = radioButton7,
                        BtnSearch = button12,
                        BtnClear = button10,
                        BtnCsv = button11,
                        PnlStartHeader = panel10,
                        PnlMiddleHeader = panel11,
                        PnlEndHeader = panel12,
                        GridStart = dataGridView10,
                        GridMiddle = dataGridView11,
                        GridEnd = dataGridView12
                    };

                case 6:
                    return new TraceTabContext
                    {
                        TabNo = 6,
                        TxtOrder = textBox17,
                        TxtItemName = textBox18,
                        TxtItemCode = textBox19,
                        TxtLot = textBox20,
                        ChkUseFrom = checkBox6,
                        DtpFrom = dateTimePicker9,
                        DtpTo = dateTimePicker10,
                        RdoForward = radioButton10,
                        RdoBackward = radioButton9,
                        BtnSearch = button15,
                        BtnClear = button13,
                        BtnCsv = button14,
                        PnlStartHeader = panel13,
                        PnlMiddleHeader = panel14,
                        PnlEndHeader = panel15,
                        GridStart = dataGridView13,
                        GridMiddle = dataGridView14,
                        GridEnd = dataGridView15
                    };

                case 7:
                    return new TraceTabContext
                    {
                        TabNo = 7,
                        TxtOrder = textBox21,
                        TxtItemName = textBox22,
                        TxtItemCode = textBox23,
                        TxtLot = textBox24,
                        ChkUseFrom = checkBox7,
                        DtpFrom = dateTimePicker11,
                        DtpTo = dateTimePicker12,
                        RdoForward = radioButton12,
                        RdoBackward = radioButton11,
                        BtnSearch = button18,
                        BtnClear = button16,
                        BtnCsv = button17,
                        PnlStartHeader = panel16,
                        PnlMiddleHeader = panel17,
                        PnlEndHeader = panel18,
                        GridStart = dataGridView16,
                        GridMiddle = dataGridView17,
                        GridEnd = dataGridView18
                    };

                case 8:
                    return new TraceTabContext
                    {
                        TabNo = 8,
                        TxtOrder = textBox25,
                        TxtItemName = textBox26,
                        TxtItemCode = textBox27,
                        TxtLot = textBox28,
                        ChkUseFrom = checkBox8,
                        DtpFrom = dateTimePicker13,
                        DtpTo = dateTimePicker14,
                        RdoForward = radioButton14,
                        RdoBackward = radioButton13,
                        BtnSearch = button21,
                        BtnClear = button19,
                        BtnCsv = button20,
                        PnlStartHeader = panel19,
                        PnlMiddleHeader = panel20,
                        PnlEndHeader = panel21,
                        GridStart = dataGridView19,
                        GridMiddle = dataGridView20,
                        GridEnd = dataGridView21
                    };

                case 9:
                    return new TraceTabContext
                    {
                        TabNo = 9,
                        TxtOrder = textBox29,
                        TxtItemName = textBox30,
                        TxtItemCode = textBox31,
                        TxtLot = textBox32,
                        ChkUseFrom = checkBox9,
                        DtpFrom = dateTimePicker15,
                        DtpTo = dateTimePicker16,
                        RdoForward = radioButton16,
                        RdoBackward = radioButton15,
                        BtnSearch = button24,
                        BtnClear = button22,
                        BtnCsv = button23,
                        PnlStartHeader = panel22,
                        PnlMiddleHeader = panel23,
                        PnlEndHeader = panel24,
                        GridStart = dataGridView22,
                        GridMiddle = dataGridView23,
                        GridEnd = dataGridView24
                    };

                case 10:
                    return new TraceTabContext
                    {
                        TabNo = 10,
                        TxtOrder = textBox33,
                        TxtItemName = textBox34,
                        TxtItemCode = textBox35,
                        TxtLot = textBox36,
                        ChkUseFrom = checkBox10,
                        DtpFrom = dateTimePicker17,
                        DtpTo = dateTimePicker18,
                        RdoForward = radioButton18,
                        RdoBackward = radioButton17,
                        BtnSearch = button27,
                        BtnClear = button25,
                        BtnCsv = button26,
                        PnlStartHeader = panel25,
                        PnlMiddleHeader = panel26,
                        PnlEndHeader = panel27,
                        GridStart = dataGridView25,
                        GridMiddle = dataGridView26,
                        GridEnd = dataGridView27
                    };
            }

            return null;
        }

        private TraceTabContext GetCurrentTabContext()
        {
            int tabNo = GetCurrentTraceTabNo();
            return tabNo > 0 ? GetTabContext(tabNo) : null;
        }

        private Dictionary<string, NodeRenderRange> _nodeRenderRanges
     = new Dictionary<string, NodeRenderRange>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, MiddleTreeRenderRange> _middleTreeRenderRanges
            = new Dictionary<string, MiddleTreeRenderRange>(StringComparer.OrdinalIgnoreCase);

        private readonly HeaderVisualStyle _startHeaderStyle = new HeaderVisualStyle
        {
            GroupBackColor = Color.FromArgb(235, 242, 250),
            GroupForeColor = Color.FromArgb(40, 40, 40),
            BorderColor = Color.FromArgb(180, 180, 180),
            GroupFont = new Font("Segoe UI", 9F, FontStyle.Bold),
            ColumnFont = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        private readonly HeaderVisualStyle _middleHeaderStyle = new HeaderVisualStyle
        {
            GroupBackColor = Color.FromArgb(232, 245, 236),
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


        private readonly ToolTip _itemNameToolTip = new ToolTip();
        private readonly Font _itemNameToolTipFont = new Font("Segoe UI", 12F, FontStyle.Regular);

        private readonly Color _gridHoverCellBackColor = Color.FromArgb(225, 235, 250);
        private readonly Color _gridSelectedRowBorderColor = Color.FromArgb(150, 56, 112, 190);

        private DataGridView _hoverGrid = null;
        private int _hoverRowIndex = -1;
        private int _hoverColumnIndex = -1;
        private readonly Dictionary<DataGridView, int> _selectedRowIndexByGrid =
            new Dictionary<DataGridView, int>();

        #region 初期化・イベント登録

        public MainForm(LotTraceService liquidService, BottleTraceService bottleService, ResultService resultService, BottleResultService bottleResultService)
        {
            if (liquidService == null) throw new ArgumentNullException("liquidService");
            if (bottleService == null) throw new ArgumentNullException("bottleService");
            if (resultService == null) throw new ArgumentNullException("resulteService");
            if (bottleResultService == null) throw new ArgumentNullException("bottleResultService");


            _bottelResultService = bottleResultService;
            _liquidService = liquidService;
            _bottleService = bottleService;
            _resultService = resultService;

            InitializeComponent();
            ConfigureTraceTabOwnerDraw();
            InitializeTraceTabContexts();
            NormalizeTraceTabGridBounds();
            InitGrids();
            InitializeHeaderPanelsForAllTabs();
            InitializeItemNameToolTip();

            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null) continue;

                RegisterTraceGridEvents(tab);

                tab.BtnSearch.Click -= TraceSearch_FromAnyTab_Click;
                tab.BtnSearch.Click += TraceSearch_FromAnyTab_Click;

                tab.BtnClear.Click -= Clear_FromAnyTab_Click;
                tab.BtnClear.Click += Clear_FromAnyTab_Click;

                tab.BtnCsv.Click -= Csv_FromAnyTab_Click;
                tab.BtnCsv.Click += Csv_FromAnyTab_Click;
                tab.GridStart.CellMouseClick += dataGridStart_CellMouseClick;
                tab.GridMiddle.CellMouseClick += dataGridMiddle_CellMouseClick;
                tab.GridEnd.CellMouseClick += dataGridEnd_CellMouseClick;
            }

            RegisterTraceTargetCheckBoxes();
            RegisterTraceTabNameEvents();
            RefreshTraceTabNames();
            RegisterTracePeriodEvents();
            RefreshTracePeriodControls();

            // タブ切替で「そのタブの結果」を再表示
            swichTab.SelectedIndexChanged -= SwichTab_SelectedIndexChanged;
            swichTab.SelectedIndexChanged += SwichTab_SelectedIndexChanged;

            Shown -= MainForm_Shown;
            Shown += MainForm_Shown;

            FormClosing -= MainForm_FormClosing;
            FormClosing += MainForm_FormClosing;
        }

        private void ConfigureTraceTabOwnerDraw()
        {
            swichTab.DrawMode = TabDrawMode.OwnerDrawFixed;
            swichTab.DrawItem -= SwichTab_DrawItem;
            swichTab.DrawItem += SwichTab_DrawItem;
        }

        private void SwichTab_DrawItem(object sender, DrawItemEventArgs e)
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

       

        public void SetCommandLineStartup(CommandLineOptions options)
        {
            _commandLineStartupOptions = options;
        }

        public string ExecuteCommandLineHeadless(CommandLineOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");

            var tab = GetTabContext(1);
            if (tab == null)
                throw new InvalidOperationException("検索タブを初期化できませんでした。");

            SetSearchParametersToControls(tab, options.SearchParameters);
            WriteCommandLineLog("ロットトレース検索開始");
            TraceSearchWorkResult workResult = ExecuteTraceWork(
                options.SearchParameters,
                null,
                CancellationToken.None);
            WriteCommandLineLog("ロットトレース検索終了");

            WriteCommandLineLog("ロットトレースCSV出力開始");
            string exportedPath = ExportTraceWorkResultToDefaultCsv(workResult);
            WriteCommandLineLog("ロットトレースCSV出力完了");
            WriteCommandLineLog(exportedPath);

            return exportedPath;
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            var options = _commandLineStartupOptions;
            if (options == null)
                return;

            _commandLineStartupOptions = null;

            var tab = GetTabContext(1);
            if (tab == null)
                return;

            SetSearchParametersToControls(tab, options.SearchParameters);

            WriteCommandLineLog("ロットトレース検索開始");
            bool traceSucceeded = await DoTraceAsync(tab, options.SearchParameters);
            if (!traceSucceeded)
                return;
            WriteCommandLineLog("ロットトレース検索終了");

            if (options.ExportsCsv)
            {
                try
                {
                    WriteCommandLineLog("ロットトレースCSV出力開始");
                    string exportedPath = ExportCsvForTabToDefaultFile(tab);
                    WriteCommandLineLog("ロットトレースCSV出力完了");
                    WriteCommandLineLog(exportedPath);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("CSV 出力に失敗しました: " + ex.Message);
                    MessageBox.Show(this,
                        "CSV 出力に失敗しました。\r\n" + ex.Message,
                        "CSV出力",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private static void WriteCommandLineLog(string message)
        {
            Console.WriteLine(message);
            Console.Out.Flush();
        }

        private void RegisterTracePeriodEvents()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null || tab.ChkUseFrom == null)
                    continue;

                tab.ChkUseFrom.CheckedChanged -= TracePeriodCheckChanged;
                tab.ChkUseFrom.CheckedChanged += TracePeriodCheckChanged;
            }
        }

        private void TracePeriodCheckChanged(object sender, EventArgs e)
        {
            RefreshTracePeriodControls();
        }

        private void RefreshTracePeriodControls()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                ApplyTracePeriodControlState(tab);
            }
        }

        private void ApplyTracePeriodControlState(TraceTabContext tab)
        {
            if (tab == null)
                return;

            bool enabled = tab.ChkUseFrom != null && tab.ChkUseFrom.Checked;
            ApplyTraceDateTimePickerState(tab.DtpFrom, enabled);
            ApplyTraceDateTimePickerState(tab.DtpTo, enabled);
        }

        private void ApplyTraceDateTimePickerState(DateTimePicker picker, bool enabled)
        {
            if (picker == null)
                return;

            picker.Enabled = enabled;
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = enabled ? "yyyy/MM/dd" : " ";
        }

        private void RegisterTraceTabNameEvents()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                if (tab.TxtOrder != null)
                {
                    tab.TxtOrder.TextChanged -= TraceTabNameSourceChanged;
                    tab.TxtOrder.TextChanged += TraceTabNameSourceChanged;
                }

                if (tab.TxtItemCode != null)
                {
                    tab.TxtItemCode.TextChanged -= TraceTabNameSourceChanged;
                    tab.TxtItemCode.TextChanged += TraceTabNameSourceChanged;
                }
            }

            rdoTabNameOrder.CheckedChanged -= TraceTabNameModeChanged;
            rdoTabNameOrder.CheckedChanged += TraceTabNameModeChanged;

            rdoTabNameItemCode.CheckedChanged -= TraceTabNameModeChanged;
            rdoTabNameItemCode.CheckedChanged += TraceTabNameModeChanged;
        }

        private void TraceTabNameSourceChanged(object sender, EventArgs e)
        {
            RefreshTraceTabNames();
        }

        private void TraceTabNameModeChanged(object sender, EventArgs e)
        {
            RefreshTraceTabNames();
        }

        private void RefreshTraceTabNames()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                var tabPage = GetTraceTabPage(tabNo);
                if (tabPage == null)
                    continue;

                tabPage.Text = BuildTraceTabName(tab);
            }
        }

        private TabPage GetTraceTabPage(int tabNo)
        {
            if (swichTab == null || tabNo < 1 || tabNo > 10)
                return null;

            int index = tabNo - 1;
            return index < swichTab.TabPages.Count ? swichTab.TabPages[index] : null;
        }

        private string BuildTraceTabName(TraceTabContext tab)
        {
            if (tab == null)
                return string.Empty;

            TextBox source = rdoTabNameItemCode.Checked ? tab.TxtItemCode : tab.TxtOrder;
            string value = source == null ? null : source.Text;
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            return string.Format("({0:00})_未設定", tab.TabNo);
        }

        private void RegisterTraceGridEvents(TraceTabContext tab)
        {
            if (tab == null)
                return;

            RegisterTraceGridEvents(tab.GridStart, DataGridStart_Paint);
            RegisterTraceGridEvents(tab.GridMiddle, DataGridMiddle_Paint);
            RegisterTraceGridEvents(tab.GridEnd, DataGridEnd_Paint);

            if (tab.GridMiddle != null)
            {
                tab.GridMiddle.Scroll -= dataGridMiddle_Scroll;
                tab.GridMiddle.Scroll += dataGridMiddle_Scroll;
            }
        }

        private void NormalizeTraceTabGridBounds()
        {
            var baseTab = GetTabContext(1);
            if (baseTab == null)
                return;

            for (int tabNo = 2; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                CopyControlBounds(baseTab.PnlStartHeader, tab.PnlStartHeader);
                CopyControlBounds(baseTab.GridStart, tab.GridStart);
                CopyControlBounds(baseTab.PnlMiddleHeader, tab.PnlMiddleHeader);
                CopyControlBounds(baseTab.GridMiddle, tab.GridMiddle);
                CopyControlBounds(baseTab.PnlEndHeader, tab.PnlEndHeader);
                CopyControlBounds(baseTab.GridEnd, tab.GridEnd);
            }
        }

        private void CopyControlBounds(Control source, Control target)
        {
            if (source == null || target == null)
                return;

            target.Location = source.Location;
            target.Size = source.Size;
        }

        private void RegisterTraceGridEvents(DataGridView grid, PaintEventHandler paintHandler)
        {
            if (grid == null)
                return;

            grid.Paint -= Grid_SelectedRowOutlinePaint;
            grid.Paint += Grid_SelectedRowOutlinePaint;
            grid.Paint -= paintHandler;
            grid.Paint += paintHandler;

            grid.Scroll -= Grid_ScrollSync;
            grid.Scroll += Grid_ScrollSync;

            grid.MouseWheel -= Grid_MouseWheelScrollSync;
            grid.MouseWheel += Grid_MouseWheelScrollSync;

            grid.CellMouseEnter -= Grid_CellMouseEnter_ToolTip;
            grid.CellMouseEnter += Grid_CellMouseEnter_ToolTip;

            grid.CellMouseLeave -= Grid_CellMouseLeave_ToolTip;
            grid.CellMouseLeave += Grid_CellMouseLeave_ToolTip;

            grid.Scroll -= Grid_ItemNameToolTipHideOnScroll;
            grid.Scroll += Grid_ItemNameToolTipHideOnScroll;

            grid.MouseLeave -= Grid_ItemNameToolTipHideOnMouseLeave;
            grid.MouseLeave += Grid_ItemNameToolTipHideOnMouseLeave;
            grid.MouseLeave -= Grid_MouseLeave_Hover;
            grid.MouseLeave += Grid_MouseLeave_Hover;

            grid.CellFormatting -= OnCrossPointNodeBackColorFormatting;
            grid.CellFormatting += OnCrossPointNodeBackColorFormatting;
            grid.CellFormatting -= Grid_CellFormatting_SelectionAndHover;
            grid.CellFormatting += Grid_CellFormatting_SelectionAndHover;
            grid.CellMouseMove -= Grid_CellMouseMove_Hover;
            grid.CellMouseMove += Grid_CellMouseMove_Hover;
            grid.SelectionChanged -= Grid_SelectionVisualChanged;
            grid.SelectionChanged += Grid_SelectionVisualChanged;
        }

        private void RegisterNodeKeyGroupForeColorFormatting()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                RegisterNodeKeyGroupForeColorFormatting(tab.GridStart, false);
                RegisterNodeKeyGroupForeColorFormatting(tab.GridMiddle, false);
                RegisterNodeKeyGroupForeColorFormatting(tab.GridEnd, true);
            }
        }

        private void RegisterNodeKeyGroupForeColorFormatting(DataGridView grid, bool isEndGrid)
        {
            if (grid == null)
                return;

            grid.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            if (isEndGrid)
                grid.CellFormatting -= DataGridEnd_CellFormatting;

            grid.CellFormatting += OnNodeKeyGroupForeColorFormattingFromCache;
            if (isEndGrid)
                grid.CellFormatting += DataGridEnd_CellFormatting;

            grid.CellFormatting -= Grid_CellFormatting_SelectionAndHover;
            grid.CellFormatting += Grid_CellFormatting_SelectionAndHover;
        }

        private void UnregisterNodeKeyGroupForeColorFormatting()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                UnregisterNodeKeyGroupForeColorFormatting(tab.GridStart, false);
                UnregisterNodeKeyGroupForeColorFormatting(tab.GridMiddle, false);
                UnregisterNodeKeyGroupForeColorFormatting(tab.GridEnd, true);
            }
        }

        private void UnregisterNodeKeyGroupForeColorFormatting(DataGridView grid, bool isEndGrid)
        {
            if (grid == null)
                return;

            grid.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            if (isEndGrid)
                grid.CellFormatting -= DataGridEnd_CellFormatting;
        }

        #endregion

        #region ヘッダー表示

        private void InitializeHeaderPanelsForAllTabs()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                InitializeStartHeaderPanel(tab);
                InitializeMiddleHeaderPanel(tab);
                InitializeEndHeaderPanel(tab);
            }
        }

        private void RefreshHeaderPanels(TraceTabContext tab)
        {
            RefreshStartHeaderPanel(tab);
            RefreshMiddleHeaderPanel(tab);
            RefreshEndHeaderPanel(tab);
        }

        private void InitializeStartHeaderPanel(TraceTabContext tab)
        {
            if (tab == null || tab.PnlStartHeader == null)
                return;

            tab.PnlStartHeader.Controls.Clear();

            tab.StartHeaderTitleLabel = new Label();
            tab.StartHeaderTitleLabel.Name = "lblStartHeaderTitle";
            tab.StartHeaderTitleLabel.Dock = DockStyle.Fill;
            tab.StartHeaderTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            tab.StartHeaderTitleLabel.BackColor = _startHeaderStyle.GroupBackColor;
            tab.StartHeaderTitleLabel.ForeColor = _startHeaderStyle.GroupForeColor;
            tab.StartHeaderTitleLabel.Font = _startHeaderStyle.GroupFont;
            tab.StartHeaderTitleLabel.Margin = Padding.Empty;
            tab.StartHeaderTitleLabel.Padding = Padding.Empty;

            tab.PnlStartHeader.Controls.Add(tab.StartHeaderTitleLabel);

            RefreshStartHeaderPanel(tab);
        }

        private void InitializeEndHeaderPanel(TraceTabContext tab)
        {
            if (tab == null || tab.PnlEndHeader == null)
                return;

            tab.PnlEndHeader.Controls.Clear();

            tab.EndHeaderTitleLabel = new Label();
            tab.EndHeaderTitleLabel.Name = "lblEndHeaderTitle";
            tab.EndHeaderTitleLabel.Dock = DockStyle.Fill;
            tab.EndHeaderTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            tab.EndHeaderTitleLabel.BackColor = _endHeaderStyle.GroupBackColor;
            tab.EndHeaderTitleLabel.ForeColor = _endHeaderStyle.GroupForeColor;
            tab.EndHeaderTitleLabel.Font = _endHeaderStyle.GroupFont;
            tab.EndHeaderTitleLabel.Margin = Padding.Empty;
            tab.EndHeaderTitleLabel.Padding = Padding.Empty;

            tab.PnlEndHeader.Controls.Add(tab.EndHeaderTitleLabel);

            RefreshEndHeaderPanel(tab);
        }

        private void RefreshStartHeaderPanel(TraceTabContext tab)
        {
            if (tab == null || tab.PnlStartHeader == null || tab.GridStart == null)
                return;

            if (tab.StartHeaderTitleLabel == null)
            {
                InitializeStartHeaderPanel(tab);
                return;
            }

            tab.StartHeaderTitleLabel.Text = BuildPanelHeaderText("検索始点", tab.GridStart);
            tab.PnlStartHeader.Invalidate();
        }

        private void RefreshEndHeaderPanel(TraceTabContext tab)
        {
            if (tab == null || tab.PnlEndHeader == null || tab.GridEnd == null)
                return;

            if (tab.EndHeaderTitleLabel == null)
            {
                InitializeEndHeaderPanel(tab);
                return;
            }

            tab.EndHeaderTitleLabel.Text = BuildPanelHeaderText("検索終点", tab.GridEnd);
            tab.PnlEndHeader.Invalidate();
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

        private void InitializeMiddleHeaderPanel(TraceTabContext tab)
        {
            if (tab == null || tab.PnlMiddleHeader == null)
                return;

            tab.PnlMiddleHeader.Controls.Clear();
            tab.MiddleHeaderLevelLabels.Clear();

            tab.MiddleHeaderInnerPanel = new Panel();
            tab.MiddleHeaderInnerPanel.Name = "middleHeaderInnerPanel";
            tab.MiddleHeaderInnerPanel.Location = new Point(0, 0);
            tab.MiddleHeaderInnerPanel.Height = tab.PnlMiddleHeader.Height;
            tab.MiddleHeaderInnerPanel.Width = tab.PnlMiddleHeader.Width;
            tab.MiddleHeaderInnerPanel.BackColor = _middleHeaderStyle.GroupBackColor;
            tab.MiddleHeaderInnerPanel.Margin = Padding.Empty;
            tab.MiddleHeaderInnerPanel.Padding = Padding.Empty;

            tab.PnlMiddleHeader.Controls.Add(tab.MiddleHeaderInnerPanel);

            RefreshMiddleHeaderPanel(tab);
        }

        private void RefreshMiddleHeaderPanel(TraceTabContext tab)
        {
            if (tab == null || tab.PnlMiddleHeader == null || tab.GridMiddle == null)
                return;

            if (tab.MiddleHeaderInnerPanel == null)
            {
                InitializeMiddleHeaderPanel(tab);
                return;
            }

            tab.PnlMiddleHeader.SuspendLayout();
            tab.MiddleHeaderInnerPanel.SuspendLayout();

            try
            {
                tab.MiddleHeaderInnerPanel.Controls.Clear();
                tab.MiddleHeaderLevelLabels.Clear();

                tab.MiddleHeaderInnerPanel.Height = tab.PnlMiddleHeader.Height;
                tab.MiddleHeaderInnerPanel.Width = GetVisibleColumnsTotalWidth(tab.GridMiddle);
                tab.MiddleHeaderInnerPanel.Left = -tab.GridMiddle.HorizontalScrollingOffset;

                BuildMiddleHeaderLevelLabels(tab);
            }
            finally
            {
                tab.MiddleHeaderInnerPanel.ResumeLayout();
                tab.PnlMiddleHeader.ResumeLayout();
            }

            tab.MiddleHeaderInnerPanel.Invalidate();
            tab.PnlMiddleHeader.Invalidate();
        }

        private void BuildMiddleHeaderLevelLabels(TraceTabContext tab)
        {
            if (tab == null || tab.MiddleHeaderInnerPanel == null || tab.GridMiddle == null)
                return;

            foreach (var levelInfo in GetMiddleHeaderLevelLayoutInfos(tab))
            {
                var label = CreateMiddleHeaderLevelLabel(tab, levelInfo);
                tab.MiddleHeaderLevelLabels.Add(label);
                tab.MiddleHeaderInnerPanel.Controls.Add(label);
            }
        }

        private Label CreateMiddleHeaderLevelLabel(TraceTabContext tab, MiddleHeaderLevelLayoutInfo levelInfo)
        {
            var label = new Label();
            label.Name = "lblMiddleHeaderLv" + levelInfo.Level.ToString();
            label.Text = GetMiddleGridGroupHeaderText(tab,levelInfo.Level);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = _middleHeaderStyle.GroupBackColor;
            label.ForeColor = _middleHeaderStyle.GroupForeColor;
            label.Font = _middleHeaderStyle.GroupFont;
            label.BorderStyle = BorderStyle.FixedSingle;
            label.Margin = Padding.Empty;
            label.Padding = Padding.Empty;
            label.Location = new Point(levelInfo.Left, 0);
            label.Size = new Size(levelInfo.Width, tab == null || tab.PnlMiddleHeader == null ? 0 : tab.PnlMiddleHeader.Height);

            return label;
        }

        private List<MiddleHeaderLevelLayoutInfo> GetMiddleHeaderLevelLayoutInfos(TraceTabContext tab)
        {
            var result = new List<MiddleHeaderLevelLayoutInfo>();

            if (tab == null || tab.GridMiddle == null || tab.GridMiddle.Columns.Count == 0)
                return result;

            var grouped = new Dictionary<int, List<DataGridViewColumn>>();

            foreach (DataGridViewColumn col in tab.GridMiddle.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                int? level = GetMiddleHeaderLevelFromColumnName(col.Name);
                if (!level.HasValue)
                    continue;

                if (!grouped.ContainsKey(level.Value))
                {
                    grouped[level.Value] = new List<DataGridViewColumn>();
                }

                grouped[level.Value].Add(col);
            }

            foreach (int level in grouped.Keys.OrderBy(x => x))
            {
                List<DataGridViewColumn> cols = grouped[level];
                if (cols == null || cols.Count == 0)
                    continue;

                int left = 0;
                int width = 0;
                bool first = true;

                foreach (DataGridViewColumn col in cols.OrderBy(c => c.DisplayIndex))
                {
                    if (first)
                    {
                        left = GetVisibleColumnsLeftOffset(tab.GridMiddle, col.Index);
                        first = false;
                    }

                    width += col.Width;
                }

                if (width <= 0)
                    continue;

                result.Add(new MiddleHeaderLevelLayoutInfo
                {
                    Level = level,
                    Left = left,
                    Width = width
                });
            }

            return result;
        }

        private int GetVisibleColumnsLeftOffset(DataGridView grid, int columnIndex)
        {
            if (grid == null || columnIndex < 0 || columnIndex >= grid.Columns.Count)
                return 0;

            int left = 0;

            foreach (DataGridViewColumn col in grid.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex))
            {
                if (col == null || !col.Visible)
                    continue;

                if (col.Index == columnIndex)
                    break;

                left += col.Width;
            }

            return left;
        }

        private sealed class MiddleHeaderLevelLayoutInfo
        {
            public int Level { get; set; }
            public int Left { get; set; }
            public int Width { get; set; }
        }

        private void dataGridMiddle_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                var grid = sender as DataGridView;
                var tab = GetTabContextByGrid(grid);
                if (tab == null)
                    return;

                if (tab.MiddleHeaderInnerPanel != null)
                {
                    tab.MiddleHeaderInnerPanel.Left = -tab.GridMiddle.HorizontalScrollingOffset;
                    tab.MiddleHeaderInnerPanel.Invalidate();
                }

                if (tab.PnlMiddleHeader != null)
                {
                    tab.PnlMiddleHeader.Invalidate();
                }
            }
        }

        #endregion

        #region グリッド初期化・列レイアウト

        private void InitGrids()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                ConfigureGridDefault(tab.GridStart);
                ConfigureGridForMiddle(tab.GridMiddle);
                ConfigureGridDefault(tab.GridEnd);

                ApplyGridColumnHeaderStyle(tab.GridStart, _startHeaderStyle);
                ApplyGridColumnHeaderStyle(tab.GridMiddle, _middleHeaderStyle);
                ApplyGridColumnHeaderStyle(tab.GridEnd, _endHeaderStyle);

                

                if (tab.GridStart != null)
                    tab.GridStart.ScrollBars = ScrollBars.None;
                if (tab.GridMiddle != null)
                    tab.GridMiddle.ScrollBars = ScrollBars.Horizontal;
                if (tab.GridEnd != null)
                    tab.GridEnd.ScrollBars = ScrollBars.Vertical;
            }

            //// 交点グリッド
            ConfigureGridDefault(dataGridIntersection);
            ApplyGridColumnHeaderStyle(dataGridIntersection, _middleHeaderStyle);
            if (dataGridIntersection != null)
            {
                InitializeIntersectionTabCommands();
                IntersectionTab.Resize -= IntersectionTab_Resize;
                IntersectionTab.Resize += IntersectionTab_Resize;
                ApplyIntersectionTabLayout();
                dataGridIntersection.ScrollBars = ScrollBars.Both;
                dataGridIntersection.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGridIntersection.AutoGenerateColumns = true;
                dataGridIntersection.CellFormatting -= OnIntersectionGridCrossPointBackColorFormatting;
                dataGridIntersection.CellFormatting += OnIntersectionGridCrossPointBackColorFormatting;
                dataGridIntersection.DataSource = CreateCrossPointGridTable(null);
                ApplyCrossPointGridColumnWidths();
            }
            //dataGridIntersection.ScrollBars = ScrollBars.Both; // 交点は通常

        }

        private void InitializeIntersectionTabCommands()
        {
            if (IntersectionTab == null)
                return;

            if (_btnIntersectionCsv == null)
            {
                _btnIntersectionCsv = CreateIntersectionCommandButton("btnIntersectionCsv", "CSV出力");
                _btnIntersectionCsv.Click -= btnIntersectionCsv_Click;
                _btnIntersectionCsv.Click += btnIntersectionCsv_Click;
                IntersectionTab.Controls.Add(_btnIntersectionCsv);
            }

            if (_btnIntersectionClear == null)
            {
                _btnIntersectionClear = CreateIntersectionCommandButton("btnIntersectionClear", "クリア");
                _btnIntersectionClear.Click -= btnIntersectionClear_Click;
                _btnIntersectionClear.Click += btnIntersectionClear_Click;
                IntersectionTab.Controls.Add(_btnIntersectionClear);
            }
        }

        private Button CreateIntersectionCommandButton(string name, string text)
        {
            return new Button
            {
                Name = name,
                Text = text,
                Font = new Font("游ゴシック", 12F, FontStyle.Regular, GraphicsUnit.Point, 128),
                Size = new Size(150, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseVisualStyleBackColor = true
            };
        }

        private void IntersectionTab_Resize(object sender, EventArgs e)
        {
            ApplyIntersectionTabLayout();
            ApplyCrossPointGridColumnWidths();
        }

        private void ApplyIntersectionTabLayout()
        {
            if (IntersectionTab == null || dataGridIntersection == null)
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
                    IntersectionTab.ClientSize.Width - margin - buttonWidth,
                    buttonTop);
            }

            if (_btnIntersectionCsv != null)
            {
                _btnIntersectionCsv.Size = new Size(buttonWidth, buttonHeight);
                _btnIntersectionCsv.Location = new Point(
                    IntersectionTab.ClientSize.Width - margin - buttonWidth * 2 - buttonGap,
                    buttonTop);
            }

            int gridTop = 64;
            dataGridIntersection.Location = new Point(margin, gridTop);
            dataGridIntersection.Size = new Size(
                IntersectionTab.ClientSize.Width - margin * 2,
                IntersectionTab.ClientSize.Height - gridTop - margin);
            dataGridIntersection.Anchor = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left;
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

        private DataTable CreateCrossPointGridTable(IEnumerable<int> targetTabs)
        {
            var table = new DataTable();

            table.Columns.Add("NodeKey", typeof(string));
            table.Columns.Add("交点", typeof(int));
            table.Columns.Add("製造指図番号", typeof(string));
            table.Columns.Add("ロットNo.", typeof(string));
            table.Columns.Add("品目名", typeof(string));
            table.Columns.Add("開始日時", typeof(string));
            table.Columns.Add("重量", typeof(float));

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

            return table;
        }

        private DataTable BuildCrossPointGridTable(
            IEnumerable<CrossPointRecord> records,
            IEnumerable<int> targetTabs)
        {
            var tabNos = targetTabs == null
                ? new List<int>()
                : targetTabs
                    .Where(x => x > 0)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

            var table = CreateCrossPointGridTable(tabNos);

            if (records == null)
                return table;

            foreach (var record in records)
            {
                if (record == null)
                    continue;

                var row = table.NewRow();
                row["NodeKey"] = (object)record.NodeKey ?? DBNull.Value;
                row["交点"] = record.CrossPointFlag;
                row["製造指図番号"] = (object)record.ProductionOrderNumber ?? DBNull.Value;
                row["ロットNo."] = (object)record.LotNumber ?? DBNull.Value;
                row["品目名"] = (object)record.ItemName ?? DBNull.Value;
                row["開始日時"] = (object)record.StartDateText ?? DBNull.Value;
                row["重量"] = record.Weight.HasValue ? (object)record.Weight.Value : DBNull.Value;

                foreach (int tabNo in tabNos)
                {
                    row["タブ" + tabNo] = record.GetTabPresence(tabNo);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private void ApplyCrossPointGridColumnWidths()
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
                        col.Width = 500;
                        break;
                    case "開始日時":
                        col.Width = 180;
                        break;
                    case "重量":
                        col.Width = 100;
                        break;
                    default:
                        if (col.Name.StartsWith("タブ", StringComparison.OrdinalIgnoreCase))
                            col.Width = 70;
                        break;
                }

                col.MinimumWidth = col.Width;
                ApplyCrossPointGridCellStyle(col);
            }

            FitIntersectionGridWidthToColumns();
        }

        private int GetIntersectionGridMaxWidth()
        {
            if (IntersectionTab == null)
                return dataGridIntersection == null ? 0 : dataGridIntersection.Width;

            const int margin = 16;
            int width = IntersectionTab.ClientSize.Width - margin * 2;
            return width < 10 ? 10 : width;
        }

        private int GetVisibleIntersectionColumnsWidth()
        {
            if (dataGridIntersection == null)
                return 0;

            int width = 0;
            foreach (DataGridViewColumn col in dataGridIntersection.Columns)
            {
                if (col != null && col.Visible)
                    width += col.Width;
            }

            return width;
        }

        private void FitIntersectionGridWidthToColumns()
        {
            if (dataGridIntersection == null)
                return;

            int columnsWidth = GetVisibleIntersectionColumnsWidth();
            if (columnsWidth <= 0)
                return;

            int targetWidth = columnsWidth + SystemInformation.VerticalScrollBarWidth + 4;
            int maxWidth = GetIntersectionGridMaxWidth();
            if (targetWidth > maxWidth)
                targetWidth = maxWidth;
            if (targetWidth < 10)
                targetWidth = 10;

            dataGridIntersection.Width = targetWidth;
            dataGridIntersection.Invalidate();
        }

        private void ApplyCrossPointGridCellStyle(DataGridViewColumn col)
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

            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            col.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        }

        

      

        private void ApplyFixedGridLayoutOnce(TraceTabContext tab)
        {
            if (_fixedGridLayoutApplied)
                return;

            if (tab == null)
                return;

            ApplyFixedColumnWidths(tab.GridStart);
            ApplyFixedColumnWidths(tab.GridMiddle);
            ApplyFixedColumnWidths(tab.GridEnd);

            ApplyHeaderPanelWidthsFromGrid(tab);

            _fixedGridLayoutApplied = true;
        }

        private void ApplyFixedColumnWidths(DataGridView grid)
        {
            if (grid == null || grid.Columns.Count == 0)
                return;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null)
                    continue;

                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Resizable = DataGridViewTriState.False;
                col.Width = GetFixedColumnWidth(col);
                col.MinimumWidth = col.Width;
            }
        }

        private const int StandardColumnWidth = 95;
        private const int WideColumnWidth = 120;

        private int GetFixedColumnWidth(DataGridViewColumn col)
        {
            if (col == null)
                return StandardColumnWidth;

            string name = col.Name ?? string.Empty;

            if (IsWideColumn(name))
                return WideColumnWidth;

            return StandardColumnWidth;
        }

        private bool IsWideColumn(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return false;

            return columnName.EndsWith("_StartTime", StringComparison.OrdinalIgnoreCase)
                || columnName.EndsWith("_EndTime", StringComparison.OrdinalIgnoreCase)
                || columnName.EndsWith("_Date", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyHeaderPanelWidthsFromGrid(TraceTabContext tab)
        {
            if (tab == null)
                return;

            ApplyHeaderPanelWidth(tab.PnlStartHeader, tab.GridStart);
            ApplyHeaderPanelWidth(tab.PnlMiddleHeader, tab.GridMiddle);
            ApplyHeaderPanelWidth(tab.PnlEndHeader, tab.GridEnd);
            BeginInvoke(new Action(delegate { AlignMiddleGridBottomLineByScrollBar(tab); }));

            if (tab.MiddleHeaderInnerPanel != null && tab.PnlMiddleHeader != null && tab.GridMiddle != null)
            {
                tab.MiddleHeaderInnerPanel.Width = GetVisibleColumnsTotalWidth(tab.GridMiddle);
                tab.MiddleHeaderInnerPanel.Height = tab.PnlMiddleHeader.Height;
                tab.MiddleHeaderInnerPanel.Left = -tab.GridMiddle.HorizontalScrollingOffset;
            }
        }


        private void ApplyHeaderPanelWidth(Panel headerPanel, DataGridView grid)
        {
            if (headerPanel == null || grid == null)
                return;

            int width = GetVisibleColumnsTotalWidth(grid);

            // 枠側は表示領域幅に合わせる
            headerPanel.Width = grid.ClientSize.Width;

            // 0除け
            if (headerPanel.Width <= 0)
                headerPanel.Width = grid.Width;
        }

        private int GetVisibleColumnsTotalWidth(DataGridView grid)
        {
            if (grid == null)
                return 0;

            int total = 0;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                total += col.Width;
            }

            return total;
        }

        #region 中グリッドヘッダー

        private int? GetMiddleHeaderLevelFromColumnName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return null;

            if (!columnName.StartsWith("Lv", StringComparison.OrdinalIgnoreCase))
                return null;

            int underscoreIndex = columnName.IndexOf('_');
            if (underscoreIndex <= 2)
                return null;

            string levelText = columnName.Substring(2, underscoreIndex - 2);

            int level;
            if (!int.TryParse(levelText, out level))
                return null;

            return level;
        }

        

        private int GetMiddleLevelVisibleRowCount(DataGridView grid, int level)
        {
            if (grid == null)
                return 0;

            // このLvに属する「表示対象列」だけを拾う
            var targetColumns = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                int? lv = GetMiddleHeaderLevelFromColumnName(col.Name);
                if (!lv.HasValue || lv.Value != level)
                    continue;

                // 内部列は件数判定から除外したいので、表示列だけ対象
                targetColumns.Add(col);
            }

            if (targetColumns.Count == 0)
                return 0;

            int count = 0;

            for (int rowIndex = 0; rowIndex < grid.Rows.Count; rowIndex++)
            {
                DataGridViewRow row = grid.Rows[rowIndex];
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                bool hasDisplayedValue = false;

                foreach (DataGridViewColumn col in targetColumns)
                {
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

        private string GetMiddleGridGroupHeaderText(TraceTabContext tab, int level)
        {
            return string.Format("中間工程{0}[{1}]", level, GetMiddleLevelVisibleRowCount( tab.GridMiddle, level));
        }

        

        #endregion

        #endregion

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
            if (e == null)
                return;

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
            if (e == null)
                return;

            string text = string.IsNullOrWhiteSpace(_currentItemToolTipText)
                ? e.ToolTipText
                : _currentItemToolTipText;

            e.Graphics.FillRectangle(Brushes.LightYellow, e.Bounds);

            using (var borderPen = new Pen(Color.Gray, 1))
            {
                e.Graphics.DrawRectangle(
                    borderPen,
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
            if (grid == null)
                return;

            _itemNameToolTip.Hide(grid);
            _currentItemToolTipText = string.Empty;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.RowIndex >= grid.Rows.Count || e.ColumnIndex >= grid.Columns.Count)
                return;

            var column = grid.Columns[e.ColumnIndex];
            if (column == null || !column.Visible)
                return;

            var gridRow = grid.Rows[e.RowIndex];
            if (gridRow == null)
                return;

            string text;

            if (column.Name.EndsWith("ItemName", StringComparison.OrdinalIgnoreCase))
            {
                object itemNameValue = gridRow.Cells[e.ColumnIndex].Value;
                if (itemNameValue == null || itemNameValue == DBNull.Value)
                    return;

                string itemName = Convert.ToString(itemNameValue);
                if (string.IsNullOrWhiteSpace(itemName))
                    return;

                string itemCodeColumnName =
                    column.Name.Substring(0, column.Name.Length - "ItemName".Length) + "ItemCode";

                string itemCode = string.Empty;

                var boundItem = gridRow.DataBoundItem as DataRowView;
                if (boundItem != null &&
                    boundItem.Row != null &&
                    boundItem.Row.Table != null &&
                    boundItem.Row.Table.Columns.Contains(itemCodeColumnName))
                {
                    object itemCodeValue = boundItem.Row[itemCodeColumnName];
                    if (itemCodeValue != null && itemCodeValue != DBNull.Value)
                    {
                        itemCode = Convert.ToString(itemCodeValue);
                    }
                }

                if (string.IsNullOrWhiteSpace(itemCode))
                {
                    text = "品目名：" + itemName;
                }
                else
                {
                    text = "品目名：" + itemName + Environment.NewLine
                         + "品目コード：" + itemCode;
                }
            }
            else
            {
                object value = gridRow.Cells[e.ColumnIndex].Value;
                if (value == null || value == DBNull.Value)
                    return;

                text = Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(text))
                    return;

                // 必要なら戻せるが、まずは制限を外して様子見
                // if (text.Length <= 1)
                //     return;
            }

            text = NormalizeToolTipText(text, 32);
            if (string.IsNullOrWhiteSpace(text))
                return;

            _currentItemToolTipText = text;
            //_itemNameToolTip.SetToolTip(grid, text);

            Rectangle cellRect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            if (cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            Point showPoint = new Point(
                Math.Max(0, cellRect.Left + 12),
                Math.Max(0, cellRect.Bottom + 2));

            _itemNameToolTip.Show(_currentItemToolTipText, grid, showPoint, _itemNameToolTip.AutoPopDelay);
        }

        private void Grid_CellMouseLeave_ToolTip(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            _currentItemToolTipText = string.Empty;
            _itemNameToolTip.Hide(grid);
        }

        private void Grid_ItemNameToolTipHideOnScroll(object sender, ScrollEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            _currentItemToolTipText = string.Empty;
            _itemNameToolTip.Hide(grid);
        }

        private void Grid_ItemNameToolTipHideOnMouseLeave(object sender, EventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

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

        #region セル書式・表示補助

        private void DataGridEnd_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            var boundItem = grid.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (boundItem == null)
                return;

            DataRow row = boundItem.Row;

            string columnName = grid.Columns[e.ColumnIndex].Name;

            // 重量列だけ重複表示を差し替える
            if (!string.Equals(columnName, "End_Weight", StringComparison.OrdinalIgnoreCase))
                return;

            if (!IsDuplicateEndRow(row))
                return;

            int? displayGroupIndex = GetDuplicateDisplayGroupIndex(row);
            if (!displayGroupIndex.HasValue)
                return;

            e.Value = "重複Grp" + displayGroupIndex.Value.ToString();
            e.FormattingApplied = true;
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            e.CellStyle.ForeColor = Color.DimGray;
        }

        private int? GetDuplicateDisplayGroupIndex(DataRow row)
        {
            if (row == null)
                return null;

            if (!row.Table.Columns.Contains("End_DuplicateDisplayGroupIndex"))
                return null;

            object value = row["End_DuplicateDisplayGroupIndex"];
            if (value == null || value == DBNull.Value)
                return null;

            int result;
            if (int.TryParse(value.ToString(), out result))
                return result;

            return null;
        }

        private bool IsDuplicateEndRow(DataRow row)
        {
            if (row == null)
                return false;

            if (!row.Table.Columns.Contains("End_IsDuplicate"))
                return false;

            object value = row["End_IsDuplicate"];
            if (value == null || value == DBNull.Value)
                return false;

            bool result;
            if (bool.TryParse(value.ToString(), out result))
                return result;

            return false;
        }

        private void RegisterTraceTargetCheckBoxes()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var checkBox = GetTraceTargetCheckBox(tabNo);
                if (checkBox == null)
                    continue;

                checkBox.Tag = tabNo;
                checkBox.CheckedChanged -= TraceTargetCheckBox_CheckedChanged;
                checkBox.CheckedChanged += TraceTargetCheckBox_CheckedChanged;
            }

            RebuildSelectedTraceTargetTabsFromCheckBoxes();
        }

        private CheckBox GetTraceTargetCheckBox(int tabNo)
        {
            switch (tabNo)
            {
                case 1: return check1;
                case 2: return check2;
                case 3: return check3;
                case 4: return check4;
                case 5: return check5;
                case 6: return check6;
                case 7: return check7;
                case 8: return check8;
                case 9: return check9;
                case 10: return check10;
                default: return null;
            }
        }

        private void TraceTargetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null || checkBox.Tag == null)
                return;

            int tabNo;
            if (!int.TryParse(checkBox.Tag.ToString(), out tabNo) || tabNo <= 0)
                return;

            if (checkBox.Checked)
                _selectedTraceTargetTabs.Add(tabNo);
            else
                _selectedTraceTargetTabs.Remove(tabNo);
        }

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

        



        private Color GetColorFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Color.Black;

            Color[] palette = new Color[]
            {
        Color.DarkBlue,
        Color.DarkGreen,
        Color.DarkRed,
        Color.Purple,
        Color.Brown,
        Color.Teal,
        Color.Maroon
            };

            int hash = Math.Abs(key.GetHashCode());
            return palette[hash % palette.Length];
        }

        #endregion

        #region グリッド設定・列構成

        private void ConfigureGridDefault(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.AutoGenerateColumns = false;
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

            grid.KeyDown -= Grid_KeyDown_CopyCurrentCell;
            grid.KeyDown += Grid_KeyDown_CopyCurrentCell;
        }

        private void Grid_KeyDown_CopyCurrentCell(object sender, KeyEventArgs e)
        {
            if (e == null || !e.Control || e.KeyCode != Keys.C)
                return;

            var grid = sender as DataGridView;
            if (grid == null || grid.CurrentCell == null)
                return;

            object value = grid.CurrentCell.FormattedValue;
            Clipboard.SetText(value == null ? string.Empty : Convert.ToString(value));
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void ConfigureGridForMiddle(DataGridView grid)
        {
            if (grid == null)
                return;

            ConfigureGridDefault(grid);

            // 縦スクロールバーは右グリッドだけに出し、中間は横スクロールのみ表示する
            grid.ScrollBars = ScrollBars.Horizontal;
        }

        private void Grid_ScrollSync(object sender, ScrollEventArgs e)
        {
            if (_syncingScroll)
                return;

            if (e.ScrollOrientation != ScrollOrientation.VerticalScroll)
                return;

            var source = sender as DataGridView;
            if (source == null)
                return;

            var tab = GetTabContextByGrid(source);
            if (tab == null)
                return;

            try
            {
                _syncingScroll = true;

                int rowIndex = source.FirstDisplayedScrollingRowIndex;
                if (rowIndex < 0)
                    return;

                SyncGridScrollRow(tab.GridStart, rowIndex);
                SyncGridScrollRow(tab.GridMiddle, rowIndex);
                SyncGridScrollRow(tab.GridEnd, rowIndex);
            }
            finally
            {
                _syncingScroll = false;
            }
        }

        private void Grid_MouseWheelScrollSync(object sender, MouseEventArgs e)
        {
            if (_syncingScroll)
                return;

            var source = sender as DataGridView;
            if (source == null)
                return;

            var tab = GetTabContextByGrid(source);
            if (tab == null)
                return;

            if (!ReferenceEquals(source, tab.GridEnd))
            {
                int rowIndex = CalculateMouseWheelTargetRowIndex(source, e);
                SyncTraceGridVerticalScroll(tab, rowIndex);
                return;
            }

            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke(new Action(delegate
                {
                    SyncTraceGridVerticalScroll(source);
                }));
            }
            else
            {
                SyncTraceGridVerticalScroll(source);
            }
        }

        private int CalculateMouseWheelTargetRowIndex(DataGridView source, MouseEventArgs e)
        {
            if (source == null || source.RowCount == 0)
                return -1;

            int currentIndex;
            try
            {
                currentIndex = source.FirstDisplayedScrollingRowIndex;
            }
            catch
            {
                currentIndex = 0;
            }

            int lines = SystemInformation.MouseWheelScrollLines;
            if (lines <= 0)
                lines = 3;

            int direction = e.Delta > 0 ? -1 : 1;
            int targetIndex = currentIndex + (direction * lines);

            if (targetIndex < 0)
                targetIndex = 0;
            if (targetIndex >= source.RowCount)
                targetIndex = source.RowCount - 1;

            return targetIndex;
        }

        private void SyncTraceGridVerticalScroll(DataGridView source)
        {
            if (source == null)
                return;

            var tab = GetTabContextByGrid(source);
            if (tab == null)
                return;

            try
            {
                _syncingScroll = true;

                int rowIndex = source.FirstDisplayedScrollingRowIndex;
                if (rowIndex < 0)
                    return;

                SyncTraceGridVerticalScroll(tab, rowIndex);
            }
            finally
            {
                _syncingScroll = false;
            }
        }

        private void SyncTraceGridVerticalScroll(TraceTabContext tab, int rowIndex)
        {
            if (tab == null || rowIndex < 0)
                return;

            try
            {
                _syncingScroll = true;

                SyncGridScrollRow(tab.GridStart, rowIndex);
                SyncGridScrollRow(tab.GridMiddle, rowIndex);
                SyncGridScrollRow(tab.GridEnd, rowIndex);
            }
            finally
            {
                _syncingScroll = false;
            }
        }

        private TraceTabContext GetTabContextByGrid(DataGridView grid)
        {
            if (grid == null)
                return null;

            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                if (ReferenceEquals(grid, tab.GridStart) ||
                    ReferenceEquals(grid, tab.GridMiddle) ||
                    ReferenceEquals(grid, tab.GridEnd))
                {
                    return tab;
                }
            }

            return null;
        }

       
        

        private void SyncGridScrollRow(DataGridView grid, int rowIndex)
        {
            if (grid == null || grid.RowCount == 0)
                return;

            if (rowIndex >= grid.RowCount)
                rowIndex = grid.RowCount - 1;

            if (rowIndex < 0)
                return;

            try
            {
                grid.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch
            {
                // 非表示行や境界条件で例外になる場合があるので握りつぶす
            }
        }

        private void SetupNodeGridColumns(DataGridView grid, string prefix = "")
        {
            if (grid == null)
                return;

            grid.Columns.Clear();

            string p = string.IsNullOrEmpty(prefix) ? "" : prefix + "_";

            // 非表示の内部列
            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "MasterKey",
                DataPropertyName = p + "MasterKey",
                Visible = false
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "NodeKey",
                DataPropertyName = p + "NodeKey",
                Visible = false
            });

            // 表示列
            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "Order",
                DataPropertyName = p + "Order",
                HeaderText = "指図番号"
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "Lot",
                DataPropertyName = p + "Lot",
                HeaderText = "ロットNo."
            });

            // ★ ItemCode は内部データとして保持するが、画面表示列には追加しない

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "ItemName",
                DataPropertyName = p + "ItemName",
                HeaderText = "品目名"
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "StartTime",
                DataPropertyName = p + "StartTime",
                HeaderText = "開始日時"
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "Weight",
                DataPropertyName = p + "Weight",
                HeaderText = "重量",
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            DisableGridColumnSorting(grid);
        }

        private void SetupMiddleGridColumns(DataGridView grid, int maxDepth)
        {
            if (grid == null)
                return;

            grid.Columns.Clear();

            if (maxDepth <= 0)
                return;

            grid.Columns.Clear();

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Start_Order",
                DataPropertyName = "Start_Order",
                Visible = false
            });

            for (int i = 1; i <= maxDepth; i++)
            {
                string prefix = "Lv" + i + "_";

                // 非表示の内部列
                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "MasterKey",
                    DataPropertyName = prefix + "MasterKey",
                    Visible = false
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "NodeKey",
                    DataPropertyName = prefix + "NodeKey",
                    Visible = false
                });

                // 表示列
                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "Order",
                    DataPropertyName = prefix + "Order",
                    HeaderText = " 指図番号"
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "Lot",
                    DataPropertyName = prefix + "Lot",
                    HeaderText = " ロットNo."
                });

                // ★ ItemCode は内部データとして保持するが、画面表示列には追加しない

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "ItemName",
                    DataPropertyName = prefix + "ItemName",
                    HeaderText = " 品目名"
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "StartTime",
                    DataPropertyName = prefix + "StartTime",
                    HeaderText = " 開始日時"
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "Weight",
                    DataPropertyName = prefix + "Weight",
                    HeaderText = " 重量",
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
                });
            }

            DisableGridColumnSorting(grid);
        }

        private void DisableGridColumnSorting(DataGridView grid)
        {
            if (grid == null)
                return;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null)
                    continue;

                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private int GetVisibleColumnsWidth(DataGridView grid)
        {
            if (grid == null) return 0;

            int w = 0;
            foreach (DataGridViewColumn c in grid.Columns)
            {
                if (c != null && c.Visible) w += c.Width;
            }
            return w;
        }

        private int GetVisibleVScrollBarWidth(DataGridView grid)
        {
            if (grid == null) return 0;

            // DataGridView内部のVScrollBarが出ている時だけ加算
            var v = grid.Controls.OfType<VScrollBar>().FirstOrDefault();
            return (v != null && v.Visible) ? SystemInformation.VerticalScrollBarWidth : 0;
        }

        /// <summary>
        /// 指定グリッドの「表示列合計幅」に合わせて、グリッド本体とヘッダーパネルの幅を縮める。
        /// ※列は伸ばさない（AutoSize=Fillは使わない）
        /// </summary>
        private void FitGridAndHeaderToColumns(DataGridView grid, Panel headerPanel, int pad = 4)
        {
            if (grid == null || headerPanel == null) return;

            int columnsWidth = GetVisibleColumnsWidth(grid);
            if (columnsWidth <= 0) return;

            int targetWidth = columnsWidth + GetVisibleVScrollBarWidth(grid) + pad;
            if (targetWidth < 10) targetWidth = 10;

            // Grid幅を縮めて「右の空白」を消す
            grid.Width = targetWidth;

            // ヘッダーも同じ幅に揃える
            headerPanel.Width = targetWidth;

            // 同じ親ならLeftも揃えてズレ防止
            if (headerPanel.Parent == grid.Parent)
                headerPanel.Left = grid.Left;

            headerPanel.Invalidate();
            grid.Invalidate();
        }

        #endregion

        #region 検索条件の取得



        private TraceSearchParameters CollectSearchParametersFromControls(TraceTabContext tab)
        {
            var p = new TraceSearchParameters();
            p.ProductionOrderNumber = tab.TxtOrder.Text.Trim();
            p.ItemName = tab.TxtItemName.Text.Trim();
            p.ItemCode = tab.TxtItemCode.Text.Trim();
            p.LotNumber = tab.TxtLot.Text.Trim();

            if (tab.ChkUseFrom.Checked)
            {
                p.From = tab.DtpFrom.Value.Date;
                // Toは「当日23:59:59.9999999」まで含める（Dateだけだと00:00:00になって漏れやすい）
                p.To = tab.DtpTo.Value.Date.AddDays(1).AddTicks(-1);
            }

            p.Direction = tab.RdoForward.Checked ? TraceDirection.Forward : TraceDirection.Backward;
            return p;
        }

        private void SetSearchParametersToControls(TraceTabContext tab, TraceSearchParameters p)
        {
            if (tab == null || p == null)
                return;

            tab.TxtOrder.Text = p.ProductionOrderNumber ?? string.Empty;
            tab.TxtItemName.Text = p.ItemName ?? string.Empty;
            tab.TxtItemCode.Text = p.ItemCode ?? string.Empty;
            tab.TxtLot.Text = p.LotNumber ?? string.Empty;

            if (p.From.HasValue)
            {
                tab.ChkUseFrom.Checked = true;
                tab.DtpFrom.Value = p.From.Value;
            }
            else
            {
                tab.ChkUseFrom.Checked = false;
            }

            if (p.Direction == TraceDirection.Backward)
                tab.RdoBackward.Checked = true;
            else
                tab.RdoForward.Checked = true;

            RefreshTracePeriodControls();
        }

        #endregion

        #region トレース実行・表示

       
        private async void TraceSearch_FromAnyTab_Click(object sender, EventArgs e)
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

            await DoTraceAsync(tab, p);
        }

        private async Task<bool> DoTraceAsync(
            TraceTabContext tab,
            TraceSearchParameters p,
            bool showProgress = true,
            bool showErrors = true)
        {
            LotTraceApp.Forms.ProgressForm progressForm = null;
            CancellationTokenSource cancellation = null;
            IProgress<TraceProgressState> progress =
                new Progress<TraceProgressState>(state =>
                {
                    if (state == null || progressForm == null)
                        return;

                    progressForm.SetProgress(state.Message, state.Percent);
                });

            try
            {
                if (showProgress)
                {
                    cancellation = new CancellationTokenSource();
                    progressForm = new LotTraceApp.Forms.ProgressForm("トレース検索中");
                    progressForm.CancelRequested += delegate
                    {
                        cancellation.Cancel();
                    };
                    progressForm.SetProgress("検索条件を確認しています...", 5);
                    progressForm.Show(this);
                    Enabled = false;
                    progressForm.BringToFront();
                }

                var workResult = await Task.Run(() =>
                    ExecuteTraceWork(
                        p,
                        showProgress ? progress : null,
                        cancellation == null ? CancellationToken.None : cancellation.Token));

                if (workResult == null || workResult.Result.IsEmpty)
                {
                    MessageBox.Show(
                        "検索結果は0件です。",
                        "検索結果なし",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return false;
                }

                if (showProgress)
                    progress.Report(new TraceProgressState("グリッドへ反映しています...", 90));

                ApplyTraceWorkResult(tab, workResult, showProgress ? progress : null);

                if (showProgress)
                    progress.Report(new TraceProgressState("完了しました。", 100));

                return true;
            }
            catch (OperationCanceledException)
            {
                if (showProgress && progressForm != null && !progressForm.IsDisposed)
                    progressForm.SetMessage("キャンセルしました。");

                return false;
            }
            catch (Exception ex)
            {
                var logMessage = new StringBuilder();
                logMessage.AppendLine("トレース処理中にエラーが発生しました。");
                logMessage.AppendLine(BuildTraceConditionSummary(p));

                WriteAppLog(logMessage.ToString(), ex);

                if (!showErrors)
                    throw;

                MessageBox.Show(
                    "トレース処理中にエラーが発生しました。\r\n" +
                    "詳細は Logs フォルダのログを確認してください。\r\n\r\n" +
                    ex.Message,
                    "トレースエラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
            finally
            {
                if (showProgress)
                {
                    Enabled = true;
                    if (progressForm != null && !progressForm.IsDisposed)
                        progressForm.Close();
                    if (progressForm != null)
                        progressForm.Dispose();
                    if (cancellation != null)
                        cancellation.Dispose();
                    Activate();
                }
            }
        }

        

        private TraceSearchWorkResult ExecuteTraceWork(
            TraceSearchParameters p,
            IProgress<TraceProgressState> progress,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (progress != null)
                progress.Report(new TraceProgressState("製造実績を取得しています...", 10));

            TraceResult result = _liquidService.ExecuteTrace(p, progress, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (progress != null)
                progress.Report(new TraceProgressState("表示行を構築しています...", 66));

            TraceDisplayResult displayResult = _liquidService.BuildDisplayResult(
                result,
                null,
                progress,
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (progress != null)
                progress.Report(new TraceProgressState("グリッド用データを作成しています...", 78));

            DataTable displayTable = _liquidService.BuildDisplayTable(
                displayResult,
                progress,
                cancellationToken);

            return new TraceSearchWorkResult
            {
                Result = result,
                DisplayResult = displayResult,
                DisplayTable = displayTable
            };
        }

        private void ApplyTraceWorkResult(
            TraceTabContext tab,
            TraceSearchWorkResult workResult,
            IProgress<TraceProgressState> progress)
        {
            if (tab == null || workResult == null)
                return;

            if (progress != null)
                progress.Report(new TraceProgressState("グリッドへ反映しています...", 90));

            _currentMaxDepth = workResult.DisplayResult == null ? 0 : workResult.DisplayResult.MaxMiddleDepth;

            int tabNo = tab.TabNo;

            StoreDisplayArtifactsForTab(tabNo, workResult.Result, workResult.DisplayResult);
            _tabDisplayTables[tabNo] = workResult.DisplayTable;
            ClearCrossPointNodeKeysForTab(tabNo);

            ActivateTabDisplay(tabNo);

            RegisterNodeKeyGroupForeColorFormatting();
            FitGridAndHeaderToColumns(tab.GridStart, tab.PnlStartHeader);
        }

        private void RebuildGridPaintCaches()
        {
            _gridPaintCache.Clear();

            BuildStartBottomDividerRowIndexCache();
            BuildMiddleHorizontalLineCache();
            BuildEndHorizontalLineCache();
            BuildGridForeColorCaches();
            BuildGridBackColorCaches();
        }


        private string GetTableString(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
                return null;

            var value = row[columnName];
            return value == null || value == DBNull.Value ? null : value.ToString();
        }

        private int GetTableInt(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
                return 0;

            var value = row[columnName];
            if (value == null || value == DBNull.Value)
                return 0;

            int result;
            return int.TryParse(value.ToString(), out result) ? result : 0;
        }

        


       

        

        

       

        

        private bool HasAnySearchCondition(TraceSearchParameters p)
        {
            if (p == null)
                return false;

            return
                !string.IsNullOrWhiteSpace(p.ProductionOrderNumber) ||
                !string.IsNullOrWhiteSpace(p.ItemName) ||
                !string.IsNullOrWhiteSpace(p.ItemCode) ||
                !string.IsNullOrWhiteSpace(p.LotNumber) ||
                p.From.HasValue ||
                p.To.HasValue;
        }

        #endregion



        #region クリアボタン [09]

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtProductionOrderNumber.Clear();
            txtItemName.Clear();
            txtItemCode.Clear();
            txtLotNumber.Clear();
            chkUseFrom.Checked = false;
            //chkUseTo.Checked = false;

            dataGridStart.DataSource = null;
            dataGridMiddle.DataSource = null;
            dataGridEnd.DataSource = null;

            //int tabNo = tcSearchSlots.SelectedIndex + 1;
            //if (_tabTraceResults.ContainsKey(tabNo))
            //{
            //    _tabTraceResults.Remove(tabNo);
            //}
            //if (_selectedTraceTargetTabs.Contains(tabNo))
            //{
            //    _selectedTraceTargetTabs.Remove(tabNo);
            //}
        }

        #endregion

        #region CSV出力 [10]

        private void ExportCsvForTab(TraceTabContext tab)
        {
            if (tab == null) return;

            if (!HasAnyVisibleData(tab.GridStart) &&
                !HasAnyVisibleData(tab.GridMiddle) &&
                !HasAnyVisibleData(tab.GridEnd))
            {
                MessageBox.Show(this, "出力対象の表示データがありません。", "CSV出力",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "CSV出力";
                dlg.Filter = "CSV ファイル (*.csv)|*.csv";
                dlg.DefaultExt = "csv";
                dlg.AddExtension = true;
                dlg.OverwritePrompt = true;
                dlg.RestoreDirectory = true;
                dlg.FileName = BuildCsvExportFileName();
                ApplyOutputInitialDirectory(dlg, DefaultCsvDirectoryIniKey);

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                CsvExportHelper.ExportCurrentGridsToCsv(
                    dlg.FileName,
                    tab.GridStart,
                    tab.GridMiddle,
                    tab.GridEnd);

                MessageBox.Show(this, "CSV 出力が完了しました。\r\n" + dlg.FileName,
                    "CSV出力", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string ExportCsvForTabToDefaultFile(TraceTabContext tab)
        {
            if (tab == null)
                throw new ArgumentNullException("tab");

            if (!HasAnyVisibleData(tab.GridStart) &&
                !HasAnyVisibleData(tab.GridMiddle) &&
                !HasAnyVisibleData(tab.GridEnd))
            {
                throw new InvalidOperationException("出力対象の表示データがありません。");
            }

            string directory = GetCsvOutputDirectoryOrExecutableDirectory();
            string filePath = Path.Combine(directory, BuildCsvExportFileName());

            CsvExportHelper.ExportCurrentGridsToCsv(
                filePath,
                tab.GridStart,
                tab.GridMiddle,
                tab.GridEnd);

            return filePath;
        }

        private string ExportTraceWorkResultToDefaultCsv(TraceSearchWorkResult workResult)
        {
            if (workResult == null)
                throw new ArgumentNullException("workResult");

            if (workResult.DisplayTable == null || workResult.DisplayTable.Rows.Count == 0)
                throw new InvalidOperationException("出力対象の表示データがありません。");

            string directory = GetCsvOutputDirectoryOrExecutableDirectory();
            string filePath = Path.Combine(directory, BuildCsvExportFileName());

            ExportDisplayTableToCsv(
                filePath,
                workResult.DisplayTable,
                workResult.DisplayResult == null ? 0 : workResult.DisplayResult.MaxMiddleDepth);

            return filePath;
        }

        private void ExportDisplayTableToCsv(string filePath, DataTable table, int maxMiddleDepth)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is null or empty.", "filePath");

            if (table == null)
                throw new ArgumentNullException("table");

            var columns = BuildTraceCsvColumnPlans(table, maxMiddleDepth);
            var lines = new List<string>();

            lines.Add(BuildCsvLine(columns.Select(x => x.HeaderText).ToList()));

            foreach (DataRow row in table.Rows)
            {
                var values = new List<string>();

                foreach (var column in columns)
                {
                    object value = row.Table.Columns.Contains(column.ColumnName)
                        ? row[column.ColumnName]
                        : null;

                    values.Add(value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value));
                }

                lines.Add(BuildCsvLine(values));
            }

            File.WriteAllLines(filePath, lines, new UTF8Encoding(true));
        }

        private List<TraceCsvColumnPlan> BuildTraceCsvColumnPlans(DataTable table, int maxMiddleDepth)
        {
            var columns = new List<TraceCsvColumnPlan>();

            AddTraceCsvNodeColumns(columns, table, "Start_", "指図番号", "ロットNo.", "品目名", "開始日時", "重量");

            for (int level = 1; level <= maxMiddleDepth; level++)
            {
                string prefix = "Lv" + level.ToString() + "_";
                AddTraceCsvNodeColumns(columns, table, prefix, " 指図番号", " ロットNo.", " 品目名", " 開始日時", " 重量");
            }

            AddTraceCsvNodeColumns(columns, table, "End_", "指図番号", "ロットNo.", "品目名", "開始日時", "重量");

            return columns;
        }

        private void AddTraceCsvNodeColumns(
            List<TraceCsvColumnPlan> columns,
            DataTable table,
            string prefix,
            string orderHeader,
            string lotHeader,
            string itemNameHeader,
            string startTimeHeader,
            string weightHeader)
        {
            AddTraceCsvColumn(columns, table, prefix + "Order", orderHeader);
            AddTraceCsvColumn(columns, table, prefix + "Lot", lotHeader);
            AddTraceCsvColumn(columns, table, prefix + "ItemName", itemNameHeader);
            AddTraceCsvColumn(columns, table, prefix + "StartTime", startTimeHeader);
            AddTraceCsvColumn(columns, table, prefix + "Weight", weightHeader);
        }

        private void AddTraceCsvColumn(
            List<TraceCsvColumnPlan> columns,
            DataTable table,
            string columnName,
            string headerText)
        {
            if (columns == null || table == null)
                return;

            if (!table.Columns.Contains(columnName))
                return;

            columns.Add(new TraceCsvColumnPlan
            {
                ColumnName = columnName,
                HeaderText = headerText
            });
        }

        private string BuildCsvLine(List<string> values)
        {
            if (values == null || values.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');

                sb.Append(EscapeCsvValue(values[i]));
            }

            return sb.ToString();
        }

        private string EscapeCsvValue(string value)
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

        private sealed class TraceCsvColumnPlan
        {
            public string ColumnName { get; set; }
            public string HeaderText { get; set; }
        }

        private string GetCsvOutputDirectoryOrExecutableDirectory()
        {
            string directory = GetOutputInitialDirectory(DefaultCsvDirectoryIniKey);
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = AppDomain.CurrentDomain.BaseDirectory;
            }

            return directory;
        }

        private string BuildCsvExportFileName()
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return "LotTrace_Export_" + suffix + ".csv";
        }

        #endregion

        #region EXCEL出力 [13]

        private void btnExcelOutput_Click(object sender, EventArgs e)
        {
            try
            {
                RebuildSelectedTraceTargetTabsFromCheckBoxes();

                var traceSheets = BuildTraceExcelExportRequests();
                bool hasIntersectionData = HasAnyVisibleData(dataGridIntersection);

                if (traceSheets.Count == 0 && !hasIntersectionData)
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
                    sfd.FileName = BuildExcelExportFileName();
                    ApplyOutputInitialDirectory(sfd, DefaultExcelDirectoryIniKey);

                    if (sfd.ShowDialog(this) != DialogResult.OK)
                        return;

                    ExcelExportHelper.ExportTraceSheetsToExcel(
                        sfd.FileName,
                        traceSheets,
                        hasIntersectionData ? dataGridIntersection : null,
                        "CrossPoints");

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

        private List<ExcelExportHelper.TraceGridExcelExportRequest> BuildTraceExcelExportRequests()
        {
            var requests = new List<ExcelExportHelper.TraceGridExcelExportRequest>();

            foreach (int tabNo in _selectedTraceTargetTabs.OrderBy(x => x))
            {
                var tab = GetTabContext(tabNo);
                if (tab == null || !HasAnyVisibleDataForExcelExport(tab))
                    continue;

                TraceDisplayResult displayResult;
                _tabDisplayResults.TryGetValue(tabNo, out displayResult);

                TraceGridDrawContext drawContext;
                _tabDrawContexts.TryGetValue(tabNo, out drawContext);

                HashSet<string> crossPointNodeKeys;
                _crossPointNodeKeysByTab.TryGetValue(tabNo, out crossPointNodeKeys);

                requests.Add(new ExcelExportHelper.TraceGridExcelExportRequest
                {
                    WorksheetName = BuildTraceTabName(tab),
                    LeftGrid = tab.GridStart,
                    MiddleGrid = tab.GridMiddle,
                    RightGrid = tab.GridEnd,
                    DisplayResult = displayResult,
                    DrawContext = drawContext,
                    CrossPointNodeKeys = crossPointNodeKeys
                });
            }

            return requests;
        }

        private string BuildExcelExportFileName()
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return "LotTrace_Export_" + suffix + ".xlsx";
        }

        private bool HasAnyVisibleDataForExcelExport(TraceTabContext tab)
        {
            if (tab == null)
                return false;

            return HasAnyVisibleData(tab.GridStart)
                || HasAnyVisibleData(tab.GridMiddle)
                || HasAnyVisibleData(tab.GridEnd);
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

        

        #endregion

        #region ファイル出力設定

        private void ApplyOutputInitialDirectory(SaveFileDialog dialog, string iniKey)
        {
            if (dialog == null)
                return;

            string directory = GetOutputInitialDirectory(iniKey);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                dialog.InitialDirectory = directory;
            }
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
                {
                    directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
                }

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

        #region 交点検出 [14]

        private void btnDetectCrossPoints_Click(object sender, EventArgs e)
        {
            RebuildSelectedTraceTargetTabsFromCheckBoxes();

            if (_selectedTraceTargetTabs.Count == 0)
            {
                MessageBox.Show("交点検出対象のタブが選択されていません。\r\n" +
                                "チェックボックス [12] を ON にしてください。",
                                "交点検出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var targets = new Dictionary<int, TraceResult>();
            foreach (int tabNo in _selectedTraceTargetTabs)
            {
                TraceResult result;
                if (_tabTraceResults.TryGetValue(tabNo, out result))
                {
                    targets[tabNo] = result;
                }
            }

            if (targets.Count == 0)
            {
                MessageBox.Show("交点検出対象タブにトレース結果がありません。",
                    "交点検出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _lastCrossPoints = DetectCrossPointsByMasterKey(targets);
                _lastCrossPointTargetTabs = targets.Keys.OrderBy(x => x).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("交点検出中にエラーが発生しました。\r\n" + ex.Message,
                    "交点検出エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridIntersection.DataSource = BuildCrossPointGridTable(
                _lastCrossPoints,
                _lastCrossPointTargetTabs);
            ApplyCrossPointGridColumnWidths();
            StoreCrossPointMasterKeysByTab(_lastCrossPoints, _lastCrossPointTargetTabs);
            RebuildGridPaintCaches();
            InvalidateTraceGridsForTabs(_lastCrossPointTargetTabs);
            swichTab.SelectedTab = IntersectionTab;
        }

        private void StoreCrossPointMasterKeysByTab(
     IEnumerable<CrossPointRecord> records,
     IEnumerable<int> targetTabs)
        {
            _crossPointNodeKeysByTab.Clear();

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
                // ★NodeKey欄には MasterKey が入っている前提
                if (record == null || record.CrossPointFlag != 1 ||
                    string.IsNullOrWhiteSpace(record.NodeKey))
                {
                    continue;
                }

                foreach (int tabNo in _crossPointNodeKeysByTab.Keys.ToList())
                {
                    if (record.GetTabPresence(tabNo) != 1)
                        continue;

                    _crossPointNodeKeysByTab[tabNo].Add(record.NodeKey.Trim());
                }
            }
        }

        private void InvalidateTraceGridsForTabs(IEnumerable<int> tabNos)
        {
            if (tabNos == null)
                return;

            foreach (int tabNo in tabNos)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null)
                    continue;

                InvalidateTraceGrids(tab);
            }
        }

        private void InvalidateTraceGrids(TraceTabContext tab)
        {
            if (tab == null)
                return;

            if (tab.GridStart != null) tab.GridStart.Invalidate();
            if (tab.GridMiddle != null) tab.GridMiddle.Invalidate();
            if (tab.GridEnd != null) tab.GridEnd.Invalidate();
        }
        private List<CrossPointRecord> DetectCrossPointsByMasterKey(Dictionary<int, TraceResult> tabResults)
        {
            // key = (MK|xxxx or NK|xxxx)
            var tabsByKey = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
            var repNodeByKey = new Dictionary<string, ProductionResultNode>(StringComparer.OrdinalIgnoreCase);

            if (tabResults == null || tabResults.Count == 0)
                return new List<CrossPointRecord>();

            foreach (var kv in tabResults)
            {
                int tabNo = kv.Key;
                var trace = kv.Value;
                if (trace?.PathRows == null) continue;

                foreach (var row in trace.PathRows)
                {
                    if (row == null) continue;

                    foreach (var node in EnumerateRowNodes(row))
                    {
                        if (node == null) continue;

                        string key = BuildCrossPointUiKey(node); // ★ここが肝
                        if (string.IsNullOrWhiteSpace(key)) continue;

                        if (!tabsByKey.TryGetValue(key, out var set))
                        {
                            set = new HashSet<int>();
                            tabsByKey[key] = set;
                            repNodeByKey[key] = node;
                        }
                        set.Add(tabNo);
                    }
                }
            }

            var allTabs = tabResults.Keys.OrderBy(x => x).ToList();
            var records = new List<CrossPointRecord>();

            foreach (var kv in tabsByKey)
            {
                string key = kv.Key;
                var tabs = kv.Value;
                var node = repNodeByKey[key];

                var r = new CrossPointRecord
                {
                    // NodeKey列は非表示なので「キー格納」に流用（MK| or NK| を入れる）
                    NodeKey = key,
                    CrossPointFlag = tabs.Count >= 2 ? 1 : 0,
                    ProductionOrderNumber = node?.ProductionOrderNumber,
                    LotNumber = node?.LotNumber,
                    ItemName = node?.ItemName,
                    StartDateText = (node != null && node.StartDate.HasValue)
                        ? node.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                        : node?.StartDateLabel,
                    Weight = node?.Weight
                };

                foreach (int t in allTabs)
                    r.TabPresence[t] = tabs.Contains(t) ? 1 : 0;

                records.Add(r);
            }

            return records
                .OrderByDescending(x => x.CrossPointFlag)
                .ThenBy(x => x.ProductionOrderNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LotNumber, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.ItemName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.NodeKey, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private string BuildCrossPointUiKey(ProductionResultNode node)
        {
            if (node == null) return null;

            // 手投入は MasterKey で束ねると粗すぎるので NodeKey 側で分離
            bool isManual = string.Equals(node.StartDateLabel, "手投入", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(node.InputSourceType, "ManualInput", StringComparison.OrdinalIgnoreCase);

            string masterKey = (node.ControlMasterKey ?? "").Trim();
            string nodeKey = (node.NodeIdentityKey ?? "").Trim();

            if (isManual)
                return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey;

            if (!string.IsNullOrWhiteSpace(masterKey))
                return "MK|" + masterKey;

            // MasterKey無い場合のフォールバック
            return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey;
        }

        private IEnumerable<ProductionResultNode> EnumerateRowNodes(TracePathRow row)
        {
            if (row.StartNode != null) yield return row.StartNode;

            if (row.MiddleNodes != null)
            {
                foreach (var n in row.MiddleNodes)
                    if (n != null) yield return n;
            }

            if (row.EndNode != null) yield return row.EndNode;
        }
        private string ResolveCrossPointGroupPrefix(DataGridView grid, int columnIndex)
        {
            if (grid == null) return null;
            if (columnIndex < 0 || columnIndex >= grid.Columns.Count) return null;

            var column = grid.Columns[columnIndex];
            if (column == null) return null;

            string name = column.Name ?? string.Empty;

            if (name.StartsWith("Start_", StringComparison.OrdinalIgnoreCase)) return "Start_";
            if (name.StartsWith("End_", StringComparison.OrdinalIgnoreCase)) return "End_";

            if (name.StartsWith("Lv", StringComparison.OrdinalIgnoreCase))
            {
                int idx = name.IndexOf('_');
                if (idx > 2)
                {
                    string levelText = name.Substring(2, idx - 2);
                    if (int.TryParse(levelText, out int level))
                        return "Lv" + level + "_";
                }
            }

            return null;
        }

        #endregion

        #region 瓶設備画面への遷移 [18]

        private void btnBottleTrace_Click(object sender, EventArgs e)
        {
            var form = GetOrCreateBottleTraceForm();

            Hide();

            try
            {
                form.ShowDialog(this);
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    Show();
                    Activate();
                }
            }
        }

        private BottleTraceForm GetOrCreateBottleTraceForm()
        {
            if (_bottleTraceForm == null || _bottleTraceForm.IsDisposed)
            {
                _bottleTraceForm = new BottleTraceForm(_bottleService,_resultService, _bottelResultService);
                _bottleTraceForm.FormClosing -= BottleTraceForm_FormClosing;
                _bottleTraceForm.FormClosing += BottleTraceForm_FormClosing;
            }

            return _bottleTraceForm;
        }

        private void BottleTraceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_disposingBottleTraceForm)
                return;

            e.Cancel = true;

            var form = sender as Form;
            if (form != null)
                form.Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeBottleTraceForm();
        }

        private void DisposeBottleTraceForm()
        {
            if (_bottleTraceForm == null)
                return;

            if (_bottleTraceForm.IsDisposed)
            {
                _bottleTraceForm = null;
                return;
            }

            _disposingBottleTraceForm = true;
            try
            {
                _bottleTraceForm.FormClosing -= BottleTraceForm_FormClosing;
                _bottleTraceForm.Dispose();
            }
            finally
            {
                _disposingBottleTraceForm = false;
                _bottleTraceForm = null;
            }
        }

        #endregion

        #region 交点タブ クリア [23]

        private void btnIntersectionClear_Click(object sender, EventArgs e)
        {
            _lastCrossPoints = null;
            _lastCrossPointTargetTabs.Clear();
            _crossPointNodeKeysByTab.Clear();
            _crossPointColorsByNodeKey.Clear();
            _gridPaintCache.BackColorCaches.Clear();
            InvalidateTraceGridsForTabs(Enumerable.Range(1, 10));
            if (dataGridIntersection != null)
            {
                dataGridIntersection.DataSource = CreateCrossPointGridTable(null);
                ApplyCrossPointGridColumnWidths();
                dataGridIntersection.Invalidate();
            }
        }

        #endregion

        #region 交点タブ CSV出力 [24]

        private void btnIntersectionCsv_Click(object sender, EventArgs e)
        {
            if (_lastCrossPoints == null || _lastCrossPoints.Count == 0)
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
                dlg.FileName = BuildCrossPointCsvExportFileName();
                ApplyOutputInitialDirectory(dlg, DefaultCsvDirectoryIniKey);

                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var table = BuildCrossPointGridTable(
                    _lastCrossPoints,
                    _lastCrossPointTargetTabs);

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

        private string BuildCrossPointCsvExportFileName()
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return "CrossPoints_" + suffix + ".csv";
        }

        #endregion

        #region ログ出力

        private string GetLogDirectoryPath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string logDir = Path.Combine(baseDir, "Logs");

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            return logDir;
        }

        private void WriteAppLog(string message, Exception ex = null)
        {
            try
            {
                string logDir = GetLogDirectoryPath();
                string filePath = Path.Combine(
                    logDir,
                    "MainForm_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                var sb = new StringBuilder();

                sb.AppendLine("========================================");
                sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine(message);

                if (ex != null)
                {
                    sb.AppendLine("[Exception]");
                    sb.AppendLine(ex.ToString());
                }

                sb.AppendLine();

                File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // ログ出力失敗では画面を落とさない
            }
        }

        private string BuildTraceConditionSummary(TraceSearchParameters p)
        {
            if (p == null)
                return "(TraceSearchParameters = null)";

            var sb = new StringBuilder();

            sb.AppendLine("[SearchParameters]");
            sb.AppendLine("Direction=" + p.Direction);
            sb.AppendLine("ProductionOrderNumber=" + (p.ProductionOrderNumber ?? ""));
            sb.AppendLine("ItemName=" + (p.ItemName ?? ""));
            sb.AppendLine("ItemCode=" + (p.ItemCode ?? ""));
            sb.AppendLine("LotNumber=" + (p.LotNumber ?? ""));

            return sb.ToString();
        }

        #endregion

        #region ホバー表示

        private void Grid_CellFormatting_SelectionAndHover(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            e.CellStyle.SelectionBackColor = ResolveGridSelectionBackColor(e.CellStyle.BackColor);
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;

            bool isHoverCell = ReferenceEquals(_hoverGrid, grid) &&
                e.RowIndex == _hoverRowIndex &&
                e.ColumnIndex == _hoverColumnIndex;

            if (isHoverCell)
            {
                e.CellStyle.BackColor = _gridHoverCellBackColor;
                e.CellStyle.SelectionBackColor = _gridHoverCellBackColor;
            }
        }

        private void Grid_CellMouseMove_Hover(object sender, DataGridViewCellMouseEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                ClearHover(grid);
                return;
            }

            bool gridChanged = !ReferenceEquals(_hoverGrid, grid);
            bool cellChanged = _hoverRowIndex != e.RowIndex || _hoverColumnIndex != e.ColumnIndex;

            if (!gridChanged && !cellChanged)
                return;

            var previousGrid = _hoverGrid;
            int previousRow = _hoverRowIndex;
            int previousCol = _hoverColumnIndex;

            _hoverGrid = grid;
            _hoverRowIndex = e.RowIndex;
            _hoverColumnIndex = e.ColumnIndex;

            if (previousGrid != null)
            {
                if (ReferenceEquals(previousGrid, grid))
                {
                    InvalidateHoverCell(previousGrid, previousRow, previousCol);
                    InvalidateHoverCell(previousGrid, _hoverRowIndex, _hoverColumnIndex);
                }
                else
                {
                    InvalidateHoverCell(previousGrid, previousRow, previousCol);
                    InvalidateHoverCell(grid, _hoverRowIndex, _hoverColumnIndex);
                }
            }
            else
            {
                InvalidateHoverCell(grid, _hoverRowIndex, _hoverColumnIndex);
            }
        }

        private void Grid_MouseLeave_Hover(object sender, EventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            if (!ReferenceEquals(_hoverGrid, grid))
                return;

            ClearHover(grid);
        }

        private void ClearHover(DataGridView grid)
        {
            if (grid == null)
                return;

            int oldRow = _hoverRowIndex;
            int oldCol = _hoverColumnIndex;
            var oldGrid = _hoverGrid;

            _hoverGrid = null;
            _hoverRowIndex = -1;
            _hoverColumnIndex = -1;

            if (oldGrid != null)
            {
                InvalidateHoverCell(oldGrid, oldRow, oldCol);
            }
        }

        private void InvalidateHoverCell(DataGridView grid, int rowIndex, int columnIndex)
        {
            if (grid == null)
                return;

            if (rowIndex < 0 || columnIndex < 0)
                return;

            if (rowIndex >= grid.Rows.Count || columnIndex >= grid.Columns.Count)
                return;

            Rectangle rect = grid.GetCellDisplayRectangle(columnIndex, rowIndex, false);
            if (!rect.IsEmpty)
            {
                grid.Invalidate(rect);
            }
            else
            {
                grid.Invalidate();
            }
        }

        private Color ResolveGridSelectionBackColor(Color backColor)
        {
            return backColor.IsEmpty ? SystemColors.Window : backColor;
        }

        private void Grid_SelectionVisualChanged(object sender, EventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            int previousRowIndex;
            if (!_selectedRowIndexByGrid.TryGetValue(grid, out previousRowIndex))
                previousRowIndex = -1;

            int currentRowIndex = GetSelectedRowIndex(grid);
            _selectedRowIndexByGrid[grid] = currentRowIndex;

            InvalidateDisplayedRow(grid, previousRowIndex);
            InvalidateDisplayedRow(grid, currentRowIndex);
        }

        private int GetSelectedRowIndex(DataGridView grid)
        {
            if (grid == null || grid.SelectedRows.Count == 0)
                return -1;

            return grid.SelectedRows[0].Index;
        }

        private void InvalidateDisplayedRow(DataGridView grid, int rowIndex)
        {
            if (grid == null || rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return;

            Rectangle rect = GetDisplayedRowBounds(grid, rowIndex);
            if (!rect.IsEmpty)
            {
                rect.Inflate(2, 2);
                grid.Invalidate(rect);
            }
        }

        private void Grid_SelectedRowOutlinePaint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            using (var pen = new Pen(_gridSelectedRowBorderColor, 2))
            {
                foreach (DataGridViewRow row in grid.SelectedRows)
                {
                    if (row == null || row.IsNewRow || !row.Visible || !row.Selected)
                        continue;

                    Rectangle rect = GetDisplayedRowBounds(grid, row.Index);
                    if (rect.IsEmpty)
                        continue;

                    int left = rect.Left;
                    int right = rect.Right - 1;
                    int top = rect.Top;
                    int bottom = rect.Bottom - 1;

                    e.Graphics.DrawLine(pen, left, top, right, top);
                    e.Graphics.DrawLine(pen, left, bottom, right, bottom);
                }
            }
        }

        private Rectangle GetDisplayedRowBounds(DataGridView grid, int rowIndex)
        {
            if (grid == null || rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return Rectangle.Empty;

            Rectangle rowRect = grid.GetRowDisplayRectangle(rowIndex, true);
            if (rowRect.Height <= 0)
                return Rectangle.Empty;

            Rectangle bounds = Rectangle.Empty;
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column == null || !column.Visible)
                    continue;

                Rectangle cellRect = grid.GetCellDisplayRectangle(column.Index, rowIndex, true);
                if (cellRect.Width <= 0 || cellRect.Height <= 0)
                    continue;

                bounds = bounds.IsEmpty ? cellRect : Rectangle.Union(bounds, cellRect);
            }

            if (bounds.IsEmpty)
                return Rectangle.Empty;

            bounds.Intersect(grid.DisplayRectangle);
            return bounds;
        }

        #endregion

        #region 罫線描画・キャッシュ
        /// <summary>
        /// 始点グリッド罫線描画メソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridStart_Paint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            if (_gridPaintCache.StartBottomDividerRows.Count == 0)
                return;

            GridLinePaintCache lineCache;
            if (!TryGetGridLinePaintCache(grid, out lineCache))
                return;

            if (lineCache.HorizontalLines.Count == 0)
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

            foreach (var line in lineCache.HorizontalLines)
            {
                if (line == null)
                    continue;

                if (line.RowIndex < firstRowIndex || line.RowIndex > lastRowIndex)
                    continue;

                Rectangle rect = grid.GetRowDisplayRectangle(line.RowIndex, true);
                if (rect.Height <= 0)
                    continue;

                int y = rect.Bottom - 1;
                int left = grid.DisplayRectangle.Left
                    + line.StartXOffset
                    - grid.HorizontalScrollingOffset;
                int right = grid.DisplayRectangle.Left
                    + line.EndXOffset
                    - grid.HorizontalScrollingOffset;

                using (var pen = new Pen(line.Color, line.Width))
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
        private void DrawMiddleHorizontalLinesOnPaint(
    DataGridView grid,
    Graphics graphics)
        {
            if (grid == null || graphics == null)
                return;

            if (_gridPaintCache.MiddleHorizontalLines.Count == 0)
                return;

            GridLinePaintCache lineCache;
            if (!TryGetGridLinePaintCache(grid, out lineCache))
                return;

            if (lineCache.HorizontalLines.Count == 0)
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

            

            foreach (var line in lineCache.HorizontalLines)
            {
                if (line == null)
                    continue;

                if (line.RowIndex < firstRowIndex || line.RowIndex > lastRowIndex)
                    continue;

                Rectangle rowRect = grid.GetRowDisplayRectangle(line.RowIndex, true);
                if (rowRect.Height <= 0)
                    continue;

                int left = grid.DisplayRectangle.Left
                    + line.StartXOffset
                    - grid.HorizontalScrollingOffset;
                int right = grid.DisplayRectangle.Left
                    + line.EndXOffset
                    - grid.HorizontalScrollingOffset;

                if (right <= left)
                    continue;

                using (var pen = new Pen(line.Color, line.Width))
                {
                    int y = rowRect.Bottom - 1;
                    graphics.DrawLine(pen, left, y, right, y);
                }
            }
        }


        private void DataGridMiddle_Paint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            DrawMiddleHorizontalLinesOnPaint(grid, e.Graphics);

            // 既存の縦線描画はそのまま使う
            DataGridMiddle_Vartical_CellPainting(sender, e);
        }

        private void DataGridEnd_Paint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            if (_gridPaintCache.EndHorizontalLines.Count == 0)
                return;

            GridLinePaintCache lineCache;
            if (!TryGetGridLinePaintCache(grid, out lineCache))
                return;

            if (lineCache.HorizontalLines.Count == 0)
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

            foreach (var line in lineCache.HorizontalLines)
            {
                if (line == null)
                    continue;

                if (line.RowIndex < firstRowIndex || line.RowIndex > lastRowIndex)
                    continue;

                Rectangle rect = grid.GetRowDisplayRectangle(line.RowIndex, true);
                if (rect.Height <= 0)
                    continue;

                int y = rect.Bottom - 1;
                int left = grid.DisplayRectangle.Left
                    + line.StartXOffset
                    - grid.HorizontalScrollingOffset;
                int right = grid.DisplayRectangle.Left
                    + line.EndXOffset
                    - grid.HorizontalScrollingOffset;

                using (var pen = new Pen(line.Color, line.Width))
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

        private void BuildStartBottomDividerRowIndexCache()
        {
            var tab = GetCurrentTabContext();
            if (tab == null || tab.GridStart == null)
                return;

            var drawInfo = GetCurrentStartGridDrawInfo();
            if (drawInfo == null || drawInfo.Rows == null)
                return;

            var lineCache = GetOrCreateGridLinePaintCache(tab.GridStart);
            int endXOffset = GetVisibleColumnsTotalWidth(tab.GridStart);
            Color color = Color.FromArgb(120, 72, 32);

            for (int i = 0; i < drawInfo.Rows.Count; i++)
            {
                var row = drawInfo.Rows[i];
                if (row != null && row.DrawBottomDivider)
                {
                    _gridPaintCache.StartBottomDividerRows.Add(i);
                    lineCache.HorizontalLines.Add(new CachedHorizontalGridLine
                    {
                        RowIndex = i,
                        StartXOffset = 0,
                        EndXOffset = endXOffset,
                        Color = color,
                        Width = 2
                    });
                }
            }
        }

        private void BuildMiddleHorizontalLineCache()
        {
            var tab = GetCurrentTabContext();
            if (tab == null || tab.GridMiddle == null)
                return;

            var lineCache = GetOrCreateGridLinePaintCache(tab.GridMiddle);
            var drawInfo = GetCurrentMiddleGridDrawInfo();
            if (drawInfo == null || drawInfo.HorizontalLines == null)
                return;

            int endXOffset = GetVisibleColumnsTotalWidth(tab.GridMiddle);

            var preferredLines = drawInfo.HorizontalLines
                   .Where(x => x != null)
                   .GroupBy(x => x.StartRowIndex)
                   .Select(g => ResolvePreferredMiddleHorizontalLine(g.ToList()))
                   .Where(x => x != null)
                   .ToList();

            foreach (var line in preferredLines)
            {
                if (line == null)
                    continue;

                int fromXLevel = line.FromXLevel;
                if (string.Equals(line.LineKind, "Start", StringComparison.OrdinalIgnoreCase) && line.FromXLevel <= 0)
                {
                    fromXLevel = 1;
                }

                _gridPaintCache.MiddleHorizontalLines.Add(line);
                lineCache.HorizontalLines.Add(new CachedHorizontalGridLine
                {
                    RowIndex = line.StartRowIndex,
                    StartXOffset = GetMiddleLevelLeftOffset(tab.GridMiddle, fromXLevel, true),
                    EndXOffset = endXOffset,
                    Color = ResolveMiddleLineColor(line.LineKind),
                    Width = 2
                });
            }

            BuildMiddleVerticalLinePaintCache(tab.GridMiddle, lineCache, drawInfo);
        }

        private void BuildMiddleVerticalLinePaintCache(
            DataGridView grid,
            GridLinePaintCache lineCache,
            MiddleGridDrawInfo drawInfo)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (lineCache == null)
                throw new ArgumentNullException("lineCache");

            if (drawInfo == null || drawInfo.VerticalLines == null)
                return;

            Color color = ResolveMiddleLineColor("Vartical");

            foreach (var line in drawInfo.VerticalLines)
            {
                if (line == null)
                    continue;

                lineCache.VerticalLines.Add(new CachedVerticalGridLine
                {
                    XOffset = GetMiddleLevelLeftOffset(grid, line.XLevel, false),
                    Color = color,
                    Width = 2
                });
            }
        }

        private int GetMiddleLevelLeftOffset(DataGridView grid, int level, bool addHorizontalLineAdjustment)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (level <= 1)
                return 0;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                int? columnLevel = GetMiddleHeaderLevelFromColumnName(col.Name);
                if (!columnLevel.HasValue)
                    continue;

                if (columnLevel.Value == level)
                {
                    int offset = GetVisibleColumnsLeftOffset(grid, col.Index);
                    return addHorizontalLineAdjustment ? offset + 1 : offset;
                }
            }

            return 0;
        }

        private void BuildEndHorizontalLineCache()
        {
            var tab = GetCurrentTabContext();
            if (tab == null || tab.GridEnd == null)
                return;

            var drawInfo = GetCurrentEndGridDrawInfo();
            if (drawInfo == null || drawInfo.HorizontalLines == null)
                return;

            var lineCache = GetOrCreateGridLinePaintCache(tab.GridEnd);
            int endXOffset = GetVisibleColumnsTotalWidth(tab.GridEnd);

            var preferredLines = drawInfo.HorizontalLines
                .Where(x => x != null)
                .GroupBy(x => x.StartRowIndex)
                .Select(g => ResolvePreferredEndHorizontalLine(g.ToList()))
                .Where(x => x != null)
                .ToList();

            foreach (var line in preferredLines)
            {
                if (line == null)
                    continue;

                _gridPaintCache.EndHorizontalLines.Add(line);
                lineCache.HorizontalLines.Add(new CachedHorizontalGridLine
                {
                    RowIndex = line.StartRowIndex,
                    StartXOffset = 0,
                    EndXOffset = endXOffset,
                    Color = ResolveMiddleLineColor(line.LineKind),
                    Width = 2
                });
            }
        }

        private GridLinePaintCache GetOrCreateGridLinePaintCache(DataGridView grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            GridLinePaintCache cache;
            if (!_gridPaintCache.LineCaches.TryGetValue(grid, out cache))
            {
                cache = new GridLinePaintCache();
                _gridPaintCache.LineCaches[grid] = cache;
            }

            return cache;
        }

        private bool TryGetGridLinePaintCache(DataGridView grid, out GridLinePaintCache cache)
        {
            cache = null;

            if (grid == null)
                return false;

            return _gridPaintCache.LineCaches.TryGetValue(grid, out cache);
        }

        private void PopulateStartDrawInfo(
    StartGridDrawInfo drawInfo,
    TraceDisplayResult displayResult)
        {
            if (drawInfo == null || displayResult == null || displayResult.Rows == null)
                return;

            for (int rowIndex = 0; rowIndex < displayResult.Rows.Count; rowIndex++)
            {
                var row = displayResult.Rows[rowIndex];
                if (row == null)
                    continue;

                bool hasNextRow = rowIndex + 1 < displayResult.Rows.Count;

                drawInfo.Rows.Add(new StartRowDrawInfo
                {
                    RowIndex = rowIndex,
                    DrawBottomDivider = row.IsLastRowOfStartGroup //&& hasNextRow
                });
            }
        }

        private void PopulateMiddleDrawInfo(
    MiddleGridDrawInfo drawInfo,
    TraceDisplayResult displayResult)
        {
            if (drawInfo == null || displayResult == null || displayResult.LineRanges == null)
                return;

            foreach (var line in displayResult.LineRanges)
            {
                if (line == null)
                    continue;

                bool isMiddleLine = string.Equals(line.GridKind, "Middle", StringComparison.OrdinalIgnoreCase);

                bool isStartLine = string.Equals(line.GridKind, "Start", StringComparison.OrdinalIgnoreCase) &&
                                   string.Equals(line.LineKind, "Start", StringComparison.OrdinalIgnoreCase);

                if (!isMiddleLine && !isStartLine)
                    continue;

                drawInfo.HorizontalLines.Add(new MiddleHorizontalLineDrawInfo
                {
                    StartRowIndex = line.StartRowIndex,
                    EndRowIndex = line.EndRowIndex,
                    FromXLevel = line.FromXLevel,
                    ToXLevel = line.ToXLevel,
                    LineKind = line.LineKind
                });

                if (line.FromXLevel > 1)
                {
                    bool alreadyExists = drawInfo.VerticalLines.Any(x =>
                        x != null &&
                        x.XLevel == line.FromXLevel &&
                        string.Equals(x.LineKind, line.LineKind, StringComparison.OrdinalIgnoreCase));

                    if (!alreadyExists)
                    {
                        drawInfo.VerticalLines.Add(new MiddleVerticalLineDrawInfo
                        {
                            XLevel = line.FromXLevel,
                            LineKind = line.LineKind,
                            IncludeHeaderArea = true
                        });
                    }
                }
            }
            AppendStartBottomDividerToMiddleDrawInfo(drawInfo, displayResult);
        }

        private void AppendStartBottomDividerToMiddleDrawInfo(
    MiddleGridDrawInfo drawInfo,
    TraceDisplayResult displayResult)
        {
            if (drawInfo == null || displayResult == null || displayResult.Rows == null)
                return;

            int toXLevel = _currentMaxDepth;
            if (toXLevel <= 0)
                return;

            for (int rowIndex = 0; rowIndex < displayResult.Rows.Count; rowIndex++)
            {
                var row = displayResult.Rows[rowIndex];
                if (row == null)
                    continue;

                if (!row.IsLastRowOfStartGroup)
                    continue;

                drawInfo.HorizontalLines.Add(new MiddleHorizontalLineDrawInfo
                {
                    StartRowIndex = rowIndex,
                    EndRowIndex = rowIndex,
                    FromXLevel = 1,
                    ToXLevel = toXLevel,
                    LineKind = "Trunk"
                });
            }
        }

        private void PopulateEndDrawInfo(
            EndGridDrawInfo drawInfo,
            TraceDisplayResult displayResult)
        {
            if (drawInfo == null || displayResult == null || displayResult.LineRanges == null)
                return;

            foreach (var line in displayResult.LineRanges)
            {
                if (line == null)
                    continue;

                bool isMiddleLine = string.Equals(line.GridKind, "Middle", StringComparison.OrdinalIgnoreCase);

                bool isStartLine = string.Equals(line.GridKind, "Start", StringComparison.OrdinalIgnoreCase) &&
                                   string.Equals(line.LineKind, "Start", StringComparison.OrdinalIgnoreCase);

                if (!isMiddleLine && !isStartLine)
                    continue;

                drawInfo.HorizontalLines.Add(new EndHorizontalLineDrawInfo
                {
                    StartRowIndex = line.StartRowIndex,
                    EndRowIndex = line.EndRowIndex,
                    LineKind = line.LineKind
                });
            }
            AppendStartBottomDividerToEndDrawInfo(drawInfo, displayResult);
        }

        private void AppendStartBottomDividerToEndDrawInfo(
    EndGridDrawInfo drawInfo,
    TraceDisplayResult displayResult)
        {
            if (drawInfo == null || displayResult == null || displayResult.Rows == null)
                return;

            for (int rowIndex = 0; rowIndex < displayResult.Rows.Count; rowIndex++)
            {
                var row = displayResult.Rows[rowIndex];
                if (row == null)
                    continue;

                if (!row.IsLastRowOfStartGroup)
                    continue;

                drawInfo.HorizontalLines.Add(new EndHorizontalLineDrawInfo
                {
                    StartRowIndex = rowIndex,
                    EndRowIndex = rowIndex,
                    LineKind = "Start"
                });
            }
        }



        private void StoreDisplayArtifactsForTab(
    int tabNo,
    TraceResult traceResult,
    TraceDisplayResult displayResult)
        {
            if (tabNo <= 0)
                return;

            if (traceResult != null)
                _tabTraceResults[tabNo] = traceResult;
            else
                _tabTraceResults.Remove(tabNo);

            if (displayResult != null)
                _tabDisplayResults[tabNo] = displayResult;
            else
                _tabDisplayResults.Remove(tabNo);

            var drawContext = BuildTraceGridDrawContext(displayResult);

            if (drawContext != null)
                _tabDrawContexts[tabNo] = drawContext;
            else
                _tabDrawContexts.Remove(tabNo);
        }

        private TraceGridDrawContext BuildTraceGridDrawContext(TraceDisplayResult displayResult)
        {
            if (displayResult == null)
                return null;

            var context = new TraceGridDrawContext();
            context.RowCount = displayResult.Rows == null ? 0 : displayResult.Rows.Count;

            PopulateStartDrawInfo(context.Start, displayResult);
            PopulateMiddleDrawInfo(context.Middle, displayResult);
            PopulateEndDrawInfo(context.End, displayResult);

            return context;
        }

        private TraceGridDrawContext GetCurrentTraceGridDrawContext()
        {
            int tabNo = GetCurrentTraceTabNo();
            if (tabNo <= 0)
                return null;

            TraceGridDrawContext context;
            return _tabDrawContexts.TryGetValue(tabNo, out context) ? context : null;
        }

        private StartGridDrawInfo GetCurrentStartGridDrawInfo()
        {
            var context = GetCurrentTraceGridDrawContext();
            return context == null ? null : context.Start;
        }

       
        private MiddleGridDrawInfo GetCurrentMiddleGridDrawInfo()
        {
            var context = GetCurrentTraceGridDrawContext();
            return context == null ? null : context.Middle;
        }

       
        private MiddleHorizontalLineDrawInfo ResolvePreferredMiddleHorizontalLine(
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

        private EndHorizontalLineDrawInfo ResolvePreferredEndHorizontalLine(
            List<EndHorizontalLineDrawInfo> lines)
        {
            if (lines == null || lines.Count == 0)
                return null;

            EndHorizontalLineDrawInfo firstStart = null;
            EndHorizontalLineDrawInfo firstTrunk = null;
            EndHorizontalLineDrawInfo firstBranch = null;

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                switch (line.LineKind)
                {
                    case "Trunk":
                        if (firstTrunk == null)
                            firstTrunk = line;
                        break;

                    case "Branch":
                        if (firstBranch == null)
                            firstBranch = line;
                        break;

                    case "Start":
                        if (firstStart == null)
                            firstStart = line;
                        break;
                }
            }

            if (firstStart != null)
                return firstStart;

            if (firstTrunk != null)
                return firstTrunk;

            return firstBranch;
        }

        
        private Color ResolveMiddleLineColor(string lineKind)
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

        
        private EndGridDrawInfo GetCurrentEndGridDrawInfo()
        {
            var context = GetCurrentTraceGridDrawContext();
            return context == null ? null : context.End;
        }

       

        
        private void DataGridMiddle_Vartical_CellPainting(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            GridLinePaintCache lineCache;
            if (!TryGetGridLinePaintCache(grid, out lineCache))
                return;

            if (lineCache.VerticalLines.Count == 0)
                return;

            DrawMiddleVerticalLines(grid, e, lineCache.VerticalLines);
        }

        private void DrawMiddleVerticalLines(
        DataGridView grid,
        PaintEventArgs e,
        List<CachedVerticalGridLine> lines)
        {
            if (grid == null || e == null || lines == null || lines.Count == 0)
                return;

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                DrawMiddleVerticalLine(grid, e, line);
            }
        }

        private void DrawMiddleVerticalLine(
        DataGridView grid,
        PaintEventArgs e,
        CachedVerticalGridLine line)
            {
            if (grid == null || e == null || line == null)
                return;

            int x = grid.DisplayRectangle.Left
                + line.XOffset
                - grid.HorizontalScrollingOffset;

            int top = GetMiddleGridTopIncludingHeader(grid);
            int bottom = GetMiddleGridBottom(grid);

            using (var pen = new Pen(line.Color, line.Width))
            {
                e.Graphics.DrawLine(pen, x, top, x, bottom);
            }
        }

       
        private int GetMiddleGridTopIncludingHeader(DataGridView grid)
        {
            return grid.DisplayRectangle.Top;
        }

        private int GetMiddleGridBottom(DataGridView grid)
        {
            if (grid == null)
                return 0;

            int bottom = GetLastDisplayedDataRowBottom(grid);
            if (bottom <= grid.DisplayRectangle.Top)
                return grid.DisplayRectangle.Top;

            return Math.Min(bottom, grid.DisplayRectangle.Bottom);
        }

        private int GetLastDisplayedDataRowBottom(DataGridView grid)
        {
            if (grid == null || grid.Rows == null || grid.Rows.Count == 0)
                return 0;

            for (int rowIndex = grid.Rows.Count - 1; rowIndex >= 0; rowIndex--)
            {
                var row = grid.Rows[rowIndex];
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                var rowRectangle = grid.GetRowDisplayRectangle(row.Index, true);
                if (rowRectangle.Height > 0)
                    return rowRectangle.Bottom;

                return grid.DisplayRectangle.Top + GetVisibleRowsHeightThroughIndex(grid, row.Index);
            }

            return 0;
        }

        private int GetVisibleRowsHeightThroughIndex(DataGridView grid, int endRowIndex)
        {
            if (grid == null || endRowIndex < 0)
                return 0;

            int height = 0;
            for (int rowIndex = 0; rowIndex <= endRowIndex && rowIndex < grid.Rows.Count; rowIndex++)
            {
                var row = grid.Rows[rowIndex];
                if (row == null || row.IsNewRow || !row.Visible)
                    continue;

                height += row.Height;
            }

            return height;
        }

        #endregion

        #region 文字色描画・キャッシュ

        private void BuildGridForeColorCaches()
        {
            var tab = GetCurrentTabContext();
            if (tab == null)
                return;

            BuildGridForeColorCache(tab.GridStart);
            BuildGridForeColorCache(tab.GridMiddle);
            BuildGridForeColorCache(tab.GridEnd);
        }

        private void BuildGridForeColorCache(DataGridView grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            int columnCount = grid.Columns.Count;
            int rowCount = grid.Rows.Count;

            int[] columnGroupIndexes = new int[columnCount];
            var nodeKeyColumnNames = new List<string>();
            var groupIndexByNodeKeyColumnName =
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                string nodeKeyColumnName =
                    ResolveNodeKeyColumnNameForForeColorGrouping(grid, columnIndex);
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
                    for (int groupIndex = 0; groupIndex < nodeKeyColumnNames.Count; groupIndex++)
                    {
                        rowGroupColors[rowIndex, groupIndex] = defaultForeColor;
                    }

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

            _gridPaintCache.ForeColorCaches[grid] =
                new GridForeColorCache(columnGroupIndexes, rowGroupColors);
        }

        private void BuildGridBackColorCaches()
        {
            for (int tabNo = 1; tabNo <= 10; tabNo++)
            {
                var tab = GetTabContext(tabNo);
                if (tab == null) continue;

                BuildGridBackColorCache(tab, tab.GridStart);
                BuildGridBackColorCache(tab, tab.GridMiddle);
                BuildGridBackColorCache(tab, tab.GridEnd);
            }
        }

        private void BuildGridBackColorCache(TraceTabContext tab, DataGridView grid)
        {
            if (tab == null || grid == null)
                return;

            HashSet<string> crossPointNodeKeys;
            if (!_crossPointNodeKeysByTab.TryGetValue(tab.TabNo, out crossPointNodeKeys) ||
    crossPointNodeKeys == null || crossPointNodeKeys.Count == 0)
            {
                // ★このタブは交点ハイライト対象が無いので、過去の塗りを消す
                _gridPaintCache.BackColorCaches.Remove(grid);
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
                string nodeKeyColumnName = ResolveCrossPointGroupPrefix(grid, columnIndex);
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
                    string prefix = nodeKeyColumnNames[groupIndex];
                    if (string.IsNullOrEmpty(prefix))
                        continue;

                    string key = BuildCrossPointUiKeyFromRow(boundItem.Row, prefix);
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

            _gridPaintCache.BackColorCaches[grid] =
                new GridBackColorCache(
                    columnGroupIndexes,
                    rowGroupBackColors,
                    rowGroupSelectionBackColors,
                    rowGroupHasBackColor);
        }

        private string ResolveNodeKeyColumnNameForForeColorGrouping(
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

            if (name.StartsWith("Start_", StringComparison.OrdinalIgnoreCase))
                return "Start_NodeKey";

            if (name.StartsWith("End_", StringComparison.OrdinalIgnoreCase))
                return "End_NodeKey";

            if (name.StartsWith("Lv", StringComparison.OrdinalIgnoreCase))
            {
                int idx = name.IndexOf('_');
                if (idx > 2)
                {
                    string levelText = name.Substring(2, idx - 2);

                    int level;
                    if (int.TryParse(levelText, out level))
                        return "Lv" + level + "_NodeKey";
                }
            }

            return null;
        }

        private Color GetDefaultForeColorForCache(DataGridView grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            Color color = grid.DefaultCellStyle.ForeColor;
            return color.IsEmpty ? Color.Black : color;
        }

        private void OnNodeKeyGroupForeColorFormattingFromCache(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            GridForeColorCache cache;
            if (!_gridPaintCache.ForeColorCaches.TryGetValue(grid, out cache))
                return;

            if (e.RowIndex >= cache.RowGroupColors.GetLength(0) ||
                e.ColumnIndex >= cache.ColumnGroupIndexes.Length)
                return;

            e.CellStyle.ForeColor = cache.GetRequiredColor(e.RowIndex, e.ColumnIndex);
        }

        private void OnCrossPointNodeBackColorFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            GridBackColorCache cache;
            if (!_gridPaintCache.BackColorCaches.TryGetValue(grid, out cache))
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

        private void OnIntersectionGridCrossPointBackColorFormatting(
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

            int crossPointFlag = GetTableInt(boundItem.Row, "交点");
            if (crossPointFlag != 1)
                return;

            string nodeKey = GetTableString(boundItem.Row, "NodeKey");
            if (string.IsNullOrWhiteSpace(nodeKey))
                return;

            var colors = GetCrossPointNodeColors(nodeKey);
            e.CellStyle.BackColor = colors.Item1;
            e.CellStyle.SelectionBackColor = colors.Item2;
        }

        private void OnNodeKeyGroupForeColorFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            var boundItem = grid.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (boundItem == null || boundItem.Row == null)
                return;

            string nodeKey = ResolveNodeKeyForForeColorGrouping(grid, boundItem.Row, e.ColumnIndex);
            if (string.IsNullOrWhiteSpace(nodeKey))
                return;

            e.CellStyle.ForeColor = GetForeColorForNodeKeyGroup(nodeKey);
        }

        private string ResolveNodeKeyForForeColorGrouping(DataGridView grid, DataRow row, int columnIndex)
        {
            if (grid == null || row == null)
                return null;

            if (columnIndex < 0 || columnIndex >= grid.Columns.Count)
                return null;

            var column = grid.Columns[columnIndex];
            if (column == null)
                return null;

            string name = column.Name;

            if (name.StartsWith("Start_", StringComparison.OrdinalIgnoreCase))
            {
                return GetTableString(row, "Start_NodeKey");
            }

            if (name.StartsWith("End_", StringComparison.OrdinalIgnoreCase))
            {
                return GetTableString(row, "End_NodeKey");
            }

            if (name.StartsWith("Lv", StringComparison.OrdinalIgnoreCase))
            {
                int idx = name.IndexOf('_');
                if (idx > 2)
                {
                    string levelText = name.Substring(2, idx - 2);

                    int level;
                    if (int.TryParse(levelText, out level))
                    {
                        return GetTableString(row, "Lv" + level + "_NodeKey");
                    }
                }
            }

            return null;
        }

        private Color GetCrossPointNodeBackColor(string nodeKey)
        {
            int hash = GetStablePositiveHash(nodeKey);
            double hue = hash % 360;
            double saturation = 0.18 + ((hash / 360) % 8) * 0.01;
            double value = 0.98;

            return ConvertHsvToColor(hue, saturation, value);
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
                {
                    hash = hash * 31 + c;
                }

                return hash == int.MinValue ? int.MaxValue : Math.Abs(hash);
            }
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

            // かなり攻めた文字色
            double saturation = 0.94;
            double value = 0.42;

            // 微分散
            saturation += ((hash / 360) % 6) * 0.01;   // 0.94 - 0.99
            value += ((hash / 3600) % 7) * 0.01;       // 0.42 - 0.48

            // 黄色系は白背景で最も弱く見えるのでかなり暗くする
            if (hue >= 40 && hue <= 85)
            {
                value -= 0.16;
            }
            // 黄緑系も少し締める
            else if (hue > 85 && hue <= 135)
            {
                value -= 0.08;
            }
            // シアン系は明るく飛びやすいので補正
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
        private string BuildCrossPointUiKeyFromRow(DataRow row, string prefix)
        {
            if (row == null) return null;

            string masterKey = GetTableString(row, prefix + "MasterKey");
            string nodeKey = GetTableString(row, prefix + "NodeKey");
            string startDateLabel = GetTableString(row, prefix + "StartDateLabel"); // ★DataTableにはある

            bool isManual = string.Equals(startDateLabel, "手投入", StringComparison.OrdinalIgnoreCase);

            if (isManual)
                return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey.Trim();

            if (!string.IsNullOrWhiteSpace(masterKey))
                return "MK|" + masterKey.Trim();

            return string.IsNullOrWhiteSpace(nodeKey) ? null : "NK|" + nodeKey.Trim();
        }

        #endregion



        private static string GetRowString(DataRow r, string colName)
        {
            if (r == null || r.Table == null) return "";
            if (!r.Table.Columns.Contains(colName)) return "";
            return r[colName] == DBNull.Value ? "" : Convert.ToString(r[colName]);
        }
        private void dataGridStart_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // ヘッダ等は無視
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // ★右クリック以外は開かない
            if (e.Button != MouseButtons.Right) return;

            // 右クリックした行を選択

            var tab = GetCurrentTabContext();

            tab.GridStart.ClearSelection();
            tab.GridStart.CurrentCell = tab.GridStart.Rows[e.RowIndex].Cells[e.ColumnIndex];
            tab.GridStart.Rows[e.RowIndex].Selected = true;

            var drv = tab.GridStart.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow r = drv.Row;
            string Get(string col) =>
                r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToString(r[col]) : "";

            string productionOrderNumber = Get("Start_Order");
            string itemCode = Get("Start_ItemCode");
            string itemName = Get("Start_ItemName");
            string lotNumber = Get("Start_Lot");
            string preferredProcessId = Get("Start_MasterKey");

            using (var f = new LotTraceApp.Forms.Result(
                _resultService, productionOrderNumber, itemCode, itemName, lotNumber, preferredProcessId))
            {
                f.ShowDialog(this);
            }
        }
        private void dataGridMiddle_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var tab = GetCurrentTabContext();

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.Button != MouseButtons.Right) return;

            // 右クリックした行を選択（Startと同じ）
            tab.GridMiddle.ClearSelection();
            tab.GridMiddle.CurrentCell = tab.GridMiddle.Rows[e.RowIndex].Cells[e.ColumnIndex];
            tab.GridMiddle.Rows[e.RowIndex].Selected = true;

            var drv = tab.GridMiddle.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow r = drv.Row;

            // クリック列からLvを推定（Lv3_Order など）
            string colName = tab.GridMiddle.Columns[e.ColumnIndex].Name;
            int lv = GetMiddleHeaderLevelFromColumnName(colName) ?? 0;

            string productionOrderNumber = GetRowString(r, $"Lv{lv}_Order");
            string itemCode = GetRowString(r, $"Lv{lv}_ItemCode"); // 表示列に無くてもDataTableにあれば取れる
            string itemName = GetRowString(r, $"Lv{lv}_ItemName");
            string lotNumber = GetRowString(r, $"Lv{lv}_Lot");
            string preferredProcessId = GetRowString(r, $"Lv{lv}_MasterKey");

            // 空なら開かない（Startと同じ安全策）
            if (string.IsNullOrWhiteSpace(productionOrderNumber) &&
                string.IsNullOrWhiteSpace(itemCode) &&
                string.IsNullOrWhiteSpace(lotNumber))
                return;

            using (var f = new LotTraceApp.Forms.Result(
                _resultService, productionOrderNumber, itemCode, itemName, lotNumber, preferredProcessId))
            {
                f.ShowDialog(this);
            }
        }

        private void dataGridEnd_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // ヘッダ等は無視
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // ★右クリック以外は開かない
            if (e.Button != MouseButtons.Right) return;

            var tab = GetCurrentTabContext();

            // 右クリックした行を選択（Startと同じ）
            tab.GridEnd.ClearSelection();
            tab.GridEnd.CurrentCell = tab.GridEnd.Rows[e.RowIndex].Cells[e.ColumnIndex];
            tab.GridEnd.Rows[e.RowIndex].Selected = true;

            var drv = tab.GridEnd.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow r = drv.Row;

            string productionOrderNumber = GetRowString(r, "End_Order");
            string itemCode = GetRowString(r, "End_ItemCode");   // 表示列に無くてもDataTableにあれば取れる
            string itemName = GetRowString(r, "End_ItemName");
            string lotNumber = GetRowString(r, "End_Lot");
            string preferredProcessId = GetRowString(r, "End_MasterKey");

            // 空セルで右クリックされた場合は開かない（Start/Middleと同じ安全策）
            if (string.IsNullOrWhiteSpace(productionOrderNumber) &&
                string.IsNullOrWhiteSpace(itemCode) &&
                string.IsNullOrWhiteSpace(lotNumber))
                return;

            using (var f = new LotTraceApp.Forms.Result(
                _resultService, productionOrderNumber, itemCode, itemName, lotNumber, preferredProcessId))
            {
                f.ShowDialog(this);
            }
        }

        private int GetBottomGap(DataGridView grid)
        {
            if (grid == null) return 0;

            // DisplayRectangle はスクロールバー領域を除いた表示領域
            int gap = grid.ClientRectangle.Bottom - grid.DisplayRectangle.Bottom;
            return Math.Max(0, gap);
        }
        private void AlignMiddleGridBottomLineByScrollBar(TraceTabContext tab)
        {
            if (tab == null || tab.GridStart == null || tab.GridMiddle == null) return;

            int gap = GetBottomGap(tab.GridMiddle);

            // Start と同じ高さを基準にして、Middle だけ gap 分増やす
            int targetHeight = tab.GridStart.Height + gap;

            if (tab.GridMiddle.Height != targetHeight)
            {
                tab.GridMiddle.Height = targetHeight;
                tab.GridMiddle.Invalidate();
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
            tab.ChkUseFrom.Checked = false;

            ClearStoredTraceArtifactsForTab(tab.TabNo);
            ClearTraceTabGrids(tab);
            RefreshHeaderPanels(tab);
            InvalidateTraceGrids(tab);
        }

        private void ClearTraceTabGrids(TraceTabContext tab)
        {
            if (tab == null)
                return;

            ClearTraceGrid(tab.GridStart);
            ClearTraceGrid(tab.GridMiddle);
            ClearTraceGrid(tab.GridEnd);
            RestoreMiddleGridHeight(tab);

            if (tab.MiddleHeaderInnerPanel != null)
            {
                tab.MiddleHeaderInnerPanel.Controls.Clear();
                tab.MiddleHeaderLevelLabels.Clear();
                tab.MiddleHeaderInnerPanel.Width = 0;
                tab.MiddleHeaderInnerPanel.Left = 0;
            }
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

        private void RestoreMiddleGridHeight(TraceTabContext tab)
        {
            if (tab == null || tab.GridStart == null || tab.GridMiddle == null)
                return;

            if (tab.GridStart.Height > 0 && tab.GridMiddle.Height != tab.GridStart.Height)
                tab.GridMiddle.Height = tab.GridStart.Height;
        }

        private void ClearStoredTraceArtifactsForTab(int tabNo)
        {
            if (tabNo <= 0)
                return;

            _tabTraceResults.Remove(tabNo);
            _tabDisplayResults.Remove(tabNo);
            _tabDrawContexts.Remove(tabNo);
            _tabDisplayTables.Remove(tabNo);
            ClearCrossPointNodeKeysForTab(tabNo);
            ClearGridBackColorCachesForTab(tabNo);
            _gridPaintCache.Clear();
        }

        private void ClearCrossPointNodeKeysForTab(int tabNo)
        {
            if (tabNo <= 0)
                return;

            _crossPointNodeKeysByTab.Remove(tabNo);
        }

        private void ClearGridBackColorCachesForTab(int tabNo)
        {
            var tab = GetTabContext(tabNo);
            if (tab == null)
                return;

            if (tab.GridStart != null) _gridPaintCache.BackColorCaches.Remove(tab.GridStart);
            if (tab.GridMiddle != null) _gridPaintCache.BackColorCaches.Remove(tab.GridMiddle);
            if (tab.GridEnd != null) _gridPaintCache.BackColorCaches.Remove(tab.GridEnd);
        }

        private void Csv_FromAnyTab_Click(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            // 既存の btnCsvOutput_Click の中身を「gridをtabから取る」だけに変えるのが最小
            ExportCsvForTab(tab);
        }

        private void SwichTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tab = GetCurrentTabContext();
            if (tab == null) return;

            ActivateTabDisplay(tab.TabNo);
        }
        private void ActivateTabDisplay(int tabNo)
        {
            var tab = GetTabContext(tabNo);
            if (tab == null) return;

            DataTable table;
            if (!_tabDisplayTables.TryGetValue(tabNo, out table))
            {
                // 未検索タブは空表示
                tab.GridStart.DataSource = null;
                tab.GridMiddle.DataSource = null;
                tab.GridEnd.DataSource = null;
                RefreshHeaderPanels(tab);
                return;
            }

            TraceDisplayResult displayResult;
            _tabDisplayResults.TryGetValue(tabNo, out displayResult);

            _currentMaxDepth = displayResult == null ? 0 : displayResult.MaxMiddleDepth;

            tab.GridStart.DataSource = null;
            tab.GridMiddle.DataSource = null;
            tab.GridEnd.DataSource = null;

            SetupNodeGridColumns(tab.GridStart, "Start");
            tab.GridStart.AutoGenerateColumns = false;
            tab.GridStart.DataSource = table;

            SetupMiddleGridColumns(tab.GridMiddle, _currentMaxDepth);
            tab.GridMiddle.AutoGenerateColumns = false;
            tab.GridMiddle.DataSource = table;

            SetupNodeGridColumns(tab.GridEnd, "End");
            tab.GridEnd.AutoGenerateColumns = false;
            tab.GridEnd.DataSource = table;

            // 既存の「グローバルキャッシュ方式」のまま、表示中タブの分を作り直す
            _fixedGridLayoutApplied = false;
            ApplyFixedGridLayoutOnce(tab);
            RefreshHeaderPanels(tab);

            RebuildGridPaintCaches();   // ※グローバル1個のままでも「今見てるタブ」だけなら成立

            tab.GridStart.Invalidate();
            tab.GridMiddle.Invalidate();
            tab.GridEnd.Invalidate();
        }
    }
}
