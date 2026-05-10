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
using System.Windows.Forms;


namespace LotTraceApp
{
    public partial class MainForm : Form
    {
        private readonly LotTraceService _liquidService;
        private readonly BottleTraceService _bottleService;
        private readonly ResultService _resultService;


        // タブ番号 → トレース結果（交点検出・EXCEL出力用）
        private readonly Dictionary<int, TraceResult> _tabTraceResults =
            new Dictionary<int, TraceResult>();

        // タブ番号 → EXCEL/交点検出対象フラグ
        private readonly HashSet<int> _excelTargetTabs =
            new HashSet<int>();

        // 交点検出の最終結果
        private List<CrossPointRecord> _lastCrossPoints;

        // ★ 現在タブの全ノード（親子関係を含めたリスト）
        private List<ProductionResultNode> _currentAllNodes =
            new List<ProductionResultNode>();

        private int _currentMaxDepth;
        private bool _syncingScroll = false;
        
        private bool _fixedGridLayoutApplied = false;
        private Panel _middleHeaderInnerPanel;
        private readonly List<Label> _middleHeaderLevelLabels = new List<Label>();

        private Label _startHeaderTitleLabel;
        private Label _endHeaderTitleLabel;

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

            public void Clear()
            {
                StartBottomDividerRows.Clear();
                EndHorizontalLines.Clear();
                MiddleHorizontalLines.Clear();
                ForeColorCaches.Clear();
            }
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
        private readonly GridPaintCache _gridPaintCache = new GridPaintCache();



        private sealed class HeaderVisualStyle
        {
            public Color GroupBackColor { get; set; }
            public Color GroupForeColor { get; set; }
            public Color BorderColor { get; set; }
            public Font GroupFont { get; set; }
            public Font ColumnFont { get; set; }
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

        private DataGridView _hoverGrid = null;
        private int _hoverRowIndex = -1;
        private int _hoverColumnIndex = -1;



        public MainForm(LotTraceService liquidService, BottleTraceService bottleService, ResultService resultService)
        {
            if (liquidService == null) throw new ArgumentNullException("liquidService");
            if (bottleService == null) throw new ArgumentNullException("bottleService");
            if (resultService == null) throw new ArgumentNullException("resulteService");

            _liquidService = liquidService;
            _bottleService = bottleService;
            _resultService = resultService;

            InitializeComponent();
            InitGrids();
            InitializeStartHeaderPanel();
            InitializeMiddleHeaderPanel();
            InitializeEndHeaderPanel();
            InitializeItemNameToolTip();

            // 新罫線描画

            dataGridStart.Paint += DataGridStart_Paint;
            dataGridMiddle.Paint += DataGridMiddle_Paint;
            dataGridEnd.Paint += DataGridEnd_Paint;
            //dataGridStart.CellPainting += DataGridStart_CellPainting;
            //dataGridMiddle.CellPainting += DataGridMiddle_CellPainting;
            //dataGridMiddle.Paint += DataGridMiddle_Vartical_CellPainting;
            //dataGridEnd.CellPainting += DataGridEnd_CellPainting;


            // ★ NodeKeyグループ文字色（後段上書き）
            RegisterNodeKeyGroupForeColorFormatting();


            dataGridStart.Scroll += Grid_ScrollSync;
            dataGridMiddle.Scroll += Grid_ScrollSync;
            dataGridEnd.Scroll += Grid_ScrollSync;

            // カスタムツールチップ
            dataGridStart.CellMouseEnter += Grid_CellMouseEnter_ToolTip;
            dataGridMiddle.CellMouseEnter += Grid_CellMouseEnter_ToolTip;
            dataGridEnd.CellMouseEnter += Grid_CellMouseEnter_ToolTip;

            dataGridStart.CellMouseLeave += Grid_CellMouseLeave_ToolTip;
            dataGridMiddle.CellMouseLeave += Grid_CellMouseLeave_ToolTip;
            dataGridEnd.CellMouseLeave += Grid_CellMouseLeave_ToolTip;

            dataGridStart.Scroll += Grid_ItemNameToolTipHideOnScroll;
            dataGridMiddle.Scroll += Grid_ItemNameToolTipHideOnScroll;
            dataGridEnd.Scroll += Grid_ItemNameToolTipHideOnScroll;

            dataGridStart.MouseLeave += Grid_ItemNameToolTipHideOnMouseLeave;
            dataGridMiddle.MouseLeave += Grid_ItemNameToolTipHideOnMouseLeave;
            dataGridEnd.MouseLeave += Grid_ItemNameToolTipHideOnMouseLeave;

            
        }

        private void RegisterNodeKeyGroupForeColorFormatting()
        {
            dataGridStart.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridMiddle.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridEnd.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridEnd.CellFormatting -= DataGridEnd_CellFormatting;

            dataGridStart.CellFormatting += OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridMiddle.CellFormatting += OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridEnd.CellFormatting += OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridEnd.CellFormatting += DataGridEnd_CellFormatting;
        }

        private void UnregisterNodeKeyGroupForeColorFormatting()
        {
            dataGridStart.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridMiddle.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
            dataGridEnd.CellFormatting -= OnNodeKeyGroupForeColorFormattingFromCache;
        }

        private void InitializeStartHeaderPanel()
        {
            if (panelStartHeader == null)
                return;

            panelStartHeader.Controls.Clear();

            _startHeaderTitleLabel = new Label();
            _startHeaderTitleLabel.Name = "lblStartHeaderTitle";
            _startHeaderTitleLabel.Dock = DockStyle.Fill;
            _startHeaderTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _startHeaderTitleLabel.BackColor = _startHeaderStyle.GroupBackColor;
            _startHeaderTitleLabel.ForeColor = _startHeaderStyle.GroupForeColor;
            _startHeaderTitleLabel.Font = _startHeaderStyle.GroupFont;
            _startHeaderTitleLabel.Margin = Padding.Empty;
            _startHeaderTitleLabel.Padding = Padding.Empty;

            panelStartHeader.Controls.Add(_startHeaderTitleLabel);

            RefreshStartHeaderPanel();
        }

        private void InitializeEndHeaderPanel()
        {
            if (panelEndHeader == null)
                return;

            panelEndHeader.Controls.Clear();

            _endHeaderTitleLabel = new Label();
            _endHeaderTitleLabel.Name = "lblEndHeaderTitle";
            _endHeaderTitleLabel.Dock = DockStyle.Fill;
            _endHeaderTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _endHeaderTitleLabel.BackColor = _endHeaderStyle.GroupBackColor;
            _endHeaderTitleLabel.ForeColor = _endHeaderStyle.GroupForeColor;
            _endHeaderTitleLabel.Font = _endHeaderStyle.GroupFont;
            _endHeaderTitleLabel.Margin = Padding.Empty;
            _endHeaderTitleLabel.Padding = Padding.Empty;

            panelEndHeader.Controls.Add(_endHeaderTitleLabel);

            RefreshEndHeaderPanel();
        }

        private void RefreshStartHeaderPanel()
        {
            if (panelStartHeader == null || dataGridStart == null)
                return;

            if (_startHeaderTitleLabel == null)
            {
                InitializeStartHeaderPanel();
                return;
            }

            _startHeaderTitleLabel.Text = BuildPanelHeaderText("検索始点", dataGridStart);
            panelStartHeader.Invalidate();
        }

        private void RefreshEndHeaderPanel()
        {
            if (panelEndHeader == null || dataGridEnd == null)
                return;

            if (_endHeaderTitleLabel == null)
            {
                InitializeEndHeaderPanel();
                return;
            }

            _endHeaderTitleLabel.Text = BuildPanelHeaderText("検索終点", dataGridEnd);
            panelEndHeader.Invalidate();
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

        private void InitializeMiddleHeaderPanel()
        {
            if (panelMiddleHeader == null)
                return;

            panelMiddleHeader.Controls.Clear();
            _middleHeaderLevelLabels.Clear();

            _middleHeaderInnerPanel = new Panel();
            _middleHeaderInnerPanel.Name = "middleHeaderInnerPanel";
            _middleHeaderInnerPanel.Location = new Point(0, 0);
            _middleHeaderInnerPanel.Height = panelMiddleHeader.Height;
            _middleHeaderInnerPanel.Width = panelMiddleHeader.Width;
            _middleHeaderInnerPanel.BackColor = _middleHeaderStyle.GroupBackColor;
            _middleHeaderInnerPanel.Margin = Padding.Empty;
            _middleHeaderInnerPanel.Padding = Padding.Empty;

            panelMiddleHeader.Controls.Add(_middleHeaderInnerPanel);

            RefreshMiddleHeaderPanel();
        }

        private void RefreshMiddleHeaderPanel()
        {
            if (panelMiddleHeader == null || dataGridMiddle == null)
                return;

            if (_middleHeaderInnerPanel == null)
            {
                InitializeMiddleHeaderPanel();
                return;
            }

            panelMiddleHeader.SuspendLayout();
            _middleHeaderInnerPanel.SuspendLayout();

            try
            {
                _middleHeaderInnerPanel.Controls.Clear();
                _middleHeaderLevelLabels.Clear();

                _middleHeaderInnerPanel.Height = panelMiddleHeader.Height;
                _middleHeaderInnerPanel.Width = GetVisibleColumnsTotalWidth(dataGridMiddle);
                _middleHeaderInnerPanel.Left = -dataGridMiddle.HorizontalScrollingOffset;

                BuildMiddleHeaderLevelLabels();
            }
            finally
            {
                _middleHeaderInnerPanel.ResumeLayout();
                panelMiddleHeader.ResumeLayout();
            }

            _middleHeaderInnerPanel.Invalidate();
            panelMiddleHeader.Invalidate();
        }

        private void BuildMiddleHeaderLevelLabels()
        {
            if (_middleHeaderInnerPanel == null || dataGridMiddle == null)
                return;

            foreach (var levelInfo in GetMiddleHeaderLevelLayoutInfos())
            {
                var label = CreateMiddleHeaderLevelLabel(levelInfo);
                _middleHeaderLevelLabels.Add(label);
                _middleHeaderInnerPanel.Controls.Add(label);
            }
        }

        private Label CreateMiddleHeaderLevelLabel(MiddleHeaderLevelLayoutInfo levelInfo)
        {
            var label = new Label();
            label.Name = "lblMiddleHeaderLv" + levelInfo.Level.ToString();
            label.Text = GetMiddleGridGroupHeaderText(levelInfo.Level);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = _middleHeaderStyle.GroupBackColor;
            label.ForeColor = _middleHeaderStyle.GroupForeColor;
            label.Font = _middleHeaderStyle.GroupFont;
            label.BorderStyle = BorderStyle.FixedSingle;
            label.Margin = Padding.Empty;
            label.Padding = Padding.Empty;
            label.Location = new Point(levelInfo.Left, 0);
            label.Size = new Size(levelInfo.Width, panelMiddleHeader.Height);

            return label;
        }

        private List<MiddleHeaderLevelLayoutInfo> GetMiddleHeaderLevelLayoutInfos()
        {
            var result = new List<MiddleHeaderLevelLayoutInfo>();

            if (dataGridMiddle == null || dataGridMiddle.Columns.Count == 0)
                return result;

            var grouped = new Dictionary<int, List<DataGridViewColumn>>();

            foreach (DataGridViewColumn col in dataGridMiddle.Columns)
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
                        left = GetVisibleColumnsLeftOffset(dataGridMiddle, col.Index);
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
                if (_middleHeaderInnerPanel != null)
                {
                    _middleHeaderInnerPanel.Left = -dataGridMiddle.HorizontalScrollingOffset;
                    _middleHeaderInnerPanel.Invalidate();
                }

                if (panelMiddleHeader != null)
                {
                    panelMiddleHeader.Invalidate();
                }
            }
        }

        private void InitGrids()
{
            // 左・右グリッド：標準設定
            ConfigureGridDefault(dataGridStart);
            ConfigureGridDefault(dataGridEnd);

            // 中グリッド
            ConfigureGridForMiddle(dataGridMiddle);

            //// 交点グリッド
            //ConfigureGridDefault(IntersectionTab);

            //// 交点だけ旧ヘッダー
            //ApplyHeaderStyle(dataGridIntersection, Color.FromArgb(90, 90, 90));
            //dataGridIntersection.CellPainting += Grid_HeaderCellPainting;

            ApplyGridColumnHeaderStyle(dataGridStart, _startHeaderStyle);
            ApplyGridColumnHeaderStyle(dataGridMiddle, _middleHeaderStyle);
            ApplyGridColumnHeaderStyle(dataGridEnd, _endHeaderStyle);

            dataGridMiddle.Scroll -= dataGridMiddle_Scroll;
            dataGridMiddle.Scroll += dataGridMiddle_Scroll;
            dataGridStart.ScrollBars = ScrollBars.None;        // 左：なし
            dataGridMiddle.ScrollBars = ScrollBars.Horizontal; // 中：横だけ
            dataGridEnd.ScrollBars = ScrollBars.Vertical;      // 右：縦だけ
            //dataGridIntersection.ScrollBars = ScrollBars.Both; // 交点は通常

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

       

      

        private void ApplyFixedGridLayoutOnce()
        {
            if (_fixedGridLayoutApplied)
                return;

            ApplyFixedColumnWidths(dataGridStart);
            ApplyFixedColumnWidths(dataGridMiddle);
            ApplyFixedColumnWidths(dataGridEnd);

            ApplyHeaderPanelWidthsFromGrid();

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

        private void ApplyHeaderPanelWidthsFromGrid()
        {
            ApplyHeaderPanelWidth(panelStartHeader, dataGridStart);
            ApplyHeaderPanelWidth(panelMiddleHeader, dataGridMiddle);
            ApplyHeaderPanelWidth(panelEndHeader, dataGridEnd);
            BeginInvoke(new Action(AlignMiddleGridBottomLineByScrollBar));

            if (_middleHeaderInnerPanel != null)
            {
                _middleHeaderInnerPanel.Width = GetVisibleColumnsTotalWidth(dataGridMiddle);
                _middleHeaderInnerPanel.Height = panelMiddleHeader.Height;
                _middleHeaderInnerPanel.Left = -dataGridMiddle.HorizontalScrollingOffset;
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
        

      

        #region 2段ヘッダー（中グリッド専用）

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

        private string GetMiddleGridGroupHeaderText(int level)
        {
            return string.Format("中間工程{0}[{1}]", level, GetMiddleLevelVisibleRowCount(dataGridMiddle, level));
        }

        

        #endregion

        

        private void Grid_HeaderCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1) return; // ヘッダーだけ対象

            e.PaintBackground(e.CellBounds, false);
            e.PaintContent(e.CellBounds);

            using (var pen = new Pen(Color.Black, 2)) // ←太さここ
            {
                var rect = e.CellBounds;

                // 下線（太くする）
                e.Graphics.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);

                // 右線（列区切り）
                e.Graphics.DrawLine(pen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
            }

            e.Handled = true;
        }
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

        private bool IsItemNameColumn(DataGridViewColumn column)
        {
            if (column == null)
                return false;

            return column.Name.EndsWith("ItemName", StringComparison.OrdinalIgnoreCase);
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


        private void DataGridMiddle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            string pathKey = GetPathKeyFromBoundRow(grid, e.RowIndex);

            if (string.IsNullOrWhiteSpace(pathKey))
                return;

            e.CellStyle.ForeColor = GetColorFromKey(pathKey);
        }

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

        



        private string GetPathKeyFromBoundRow(DataGridView grid, int rowIndex)
        {
            if (grid == null)
                return null;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return null;

            var boundItem = grid.Rows[rowIndex].DataBoundItem as DataRowView;
            if (boundItem == null)
                return null;

            if (!boundItem.Row.Table.Columns.Contains("PathKey"))
                return null;

            object value = boundItem.Row["PathKey"];
            if (value == null || value == DBNull.Value)
                return null;

            string pathKey = value.ToString();
            return string.IsNullOrWhiteSpace(pathKey) ? null : pathKey;
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

            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ShowCellToolTips = false;
        }

        private void ConfigureGridForMiddle(DataGridView grid)
        {
            if (grid == null)
                return;

            ConfigureGridDefault(grid);

            // 中は横スクロールだけ
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

            try
            {
                _syncingScroll = true;

                int rowIndex = source.FirstDisplayedScrollingRowIndex;
                if (rowIndex < 0)
                    return;

                SyncGridScrollRow(dataGridStart, rowIndex);
                SyncGridScrollRow(dataGridMiddle, rowIndex);
                SyncGridScrollRow(dataGridEnd, rowIndex);
            }
            finally
            {
                _syncingScroll = false;
            }
        }

        private NodeRenderRange GetParentRenderRange(
    TraceDisplayCell cell,
    Dictionary<string, NodeRenderRange> nodeRanges)
        {
            if (cell == null)
                return null;

            if (string.IsNullOrWhiteSpace(cell.DisplayParentNodeKey))
                return null;

            if (nodeRanges == null)
                return null;

            NodeRenderRange parentRange;
            nodeRanges.TryGetValue(cell.DisplayParentNodeKey, out parentRange);
            return parentRange;
        }

        

        private TraceDisplayCell GetMiddleDisplayCellFromGrid(DataGridView grid, int rowIndex)
        {
            if (grid == null)
                return null;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return null;

            var boundItem = grid.Rows[rowIndex].DataBoundItem as DataRowView;
            if (boundItem == null)
                return null;

            string pathKey = GetPathKeyFromBoundRow(grid, rowIndex);
            if (string.IsNullOrWhiteSpace(pathKey))
                return null;

            // 今の行で、最後に値を持っている Middle セルを取る
            for (int level = _currentMaxDepth; level >= 1; level--)
            {
                string nodeKeyCol = $"Lv{level}_NodeKey";

                if (!grid.Columns.Contains(nodeKeyCol))
                    continue;

                var nodeKeyObj = grid.Rows[rowIndex].Cells[nodeKeyCol].Value;
                string nodeKey = nodeKeyObj == null || nodeKeyObj == DBNull.Value
                    ? null
                    : nodeKeyObj.ToString();

                if (string.IsNullOrWhiteSpace(nodeKey))
                    continue;

                string parentKeyCol = $"Lv{level}_ParentKey";
                string displayParentNodeKeyCol = $"Lv{level}_DisplayParentNodeKey";
                string incomingLinkKeyCol = $"Lv{level}_IncomingLinkKey";
                string upstreamPathKeyCol = $"Lv{level}_UpstreamPathKey";
                string downstreamPathKeyCol = $"Lv{level}_DownstreamPathKey";
                string masterKeyCol = $"Lv{level}_MasterKey";
                string orderCol = $"Lv{level}_Order";
                string lotCol = $"Lv{level}_Lot";
                string itemCodeCol = $"Lv{level}_ItemCode";
                string itemNameCol = $"Lv{level}_ItemName";

                return new TraceDisplayCell
                {
                    Level = level,
                    ColumnKind = TraceDisplayColumnKind.Middle,
                    NodeKey = nodeKey,
                    MasterKey = grid.Columns.Contains(masterKeyCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[masterKeyCol].Value)
                        : null,
                    ParentKey = grid.Columns.Contains(parentKeyCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[parentKeyCol].Value)
                        : null,
                    DisplayParentNodeKey = grid.Columns.Contains(displayParentNodeKeyCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[displayParentNodeKeyCol].Value)
                        : null,
                    IncomingLinkKey = grid.Columns.Contains(incomingLinkKeyCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[incomingLinkKeyCol].Value)
                        : null,
                    UpstreamPathKey = grid.Columns.Contains(upstreamPathKeyCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[upstreamPathKeyCol].Value)
                        : null,
                    DownstreamPathKey = grid.Columns.Contains(downstreamPathKeyCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[downstreamPathKeyCol].Value)
                        : null,
                    ProductionOrderNumber = grid.Columns.Contains(orderCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[orderCol].Value)
                        : null,
                    LotNumber = grid.Columns.Contains(lotCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[lotCol].Value)
                        : null,
                    ItemCode = grid.Columns.Contains(itemCodeCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[itemCodeCol].Value)
                        : null,
                    ItemName = grid.Columns.Contains(itemNameCol)
                        ? ConvertCellToString(grid.Rows[rowIndex].Cells[itemNameCol].Value)
                        : null
                };
            }

            return null;
        }

        private string ConvertCellToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            string text = value.ToString();
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }

        private string GetMiddleNodeKeyFromBoundRow(DataGridView grid, int rowIndex, int level)
        {
            if (grid == null)
                return null;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return null;

            if (level <= 0)
                return null;

            var boundItem = grid.Rows[rowIndex].DataBoundItem as DataRowView;
            if (boundItem == null)
                return null;

            string columnName = "Lv" + level + "_NodeKey";
            if (!boundItem.Row.Table.Columns.Contains(columnName))
                return null;

            return ConvertCellToString(boundItem.Row[columnName]);
        }

        private bool TryGetMiddleBottomBoundaryStartLevelByNodeKeyOnly(
    DataGridView grid,
    int rowIndex,
    out int startLevel,
    out bool isOrange)
        {
            startLevel = 0;
            isOrange = false;

            if (grid == null)
                return false;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count - 1)
                return false;

            int firstChangedLevel = 0;

            for (int level = 1; level <= _currentMaxDepth; level++)
            {
                string currentNodeKey = GetMiddleNodeKeyFromBoundRow(grid, rowIndex, level);
                string nextNodeKey = GetMiddleNodeKeyFromBoundRow(grid, rowIndex + 1, level);

                if (string.Equals(currentNodeKey, nextNodeKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                firstChangedLevel = level;
                break;
            }

            if (firstChangedLevel <= 0)
                return false;

            startLevel = firstChangedLevel;
            isOrange = (firstChangedLevel == 1);
            return true;
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

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "ParentKey",
                DataPropertyName = p + "ParentKey",
                Visible = false
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "DisplayParentNodeKey",
                DataPropertyName = p + "DisplayParentNodeKey",
                Visible = false
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "IncomingLinkKey",
                DataPropertyName = p + "IncomingLinkKey",
                Visible = false
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "UpstreamPathKey",
                DataPropertyName = p + "UpstreamPathKey",
                Visible = false
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = p + "DownstreamPathKey",
                DataPropertyName = p + "DownstreamPathKey",
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

            // ★追加：ヘッダークリックの並び替え（ソート）を禁止
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "ParentKey",
                    DataPropertyName = prefix + "ParentKey",
                    Visible = false
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "DisplayParentNodeKey",
                    DataPropertyName = prefix + "DisplayParentNodeKey",
                    Visible = false
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "IncomingLinkKey",
                    DataPropertyName = prefix + "IncomingLinkKey",
                    Visible = false
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "UpstreamPathKey",
                    DataPropertyName = prefix + "UpstreamPathKey",
                    Visible = false
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = prefix + "DownstreamPathKey",
                    DataPropertyName = prefix + "DownstreamPathKey",
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
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
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


        #region 検索条件の取得



        private TraceSearchParameters CollectSearchParametersFromControls()
        {
            var p = new TraceSearchParameters();
            p.ProductionOrderNumber = txtProductionOrderNumber.Text.Trim();
            p.ItemName = txtItemName.Text.Trim();
            p.ItemCode = txtItemCode.Text.Trim();
            p.LotNumber = txtLotNumber.Text.Trim();

            if (chkUseFrom.Checked)
            {
                p.From = dtpFrom.Value.Date;
            }
            //if (chkUseTo.Checked)
            //{
            //    // 日付の終端まで含めるため 23:59:59.999 にする
            //    DateTime to = dtpTo.Value.Date.AddDays(1).AddMilliseconds(-1);
            //    p.To = to;
            //}

            p.Direction = rdoForward.Checked ? TraceDirection.Forward : TraceDirection.Backward;

            return p;
        }

        #endregion

        #region トレース実行・表示

        private void btnTraceSearch_Click(object sender, EventArgs e)
        {
            var p = CollectSearchParametersFromControls();

            if (!HasAnySearchCondition(p))
            {
                MessageBox.Show(
                    "検索条件を1つ以上入力してください。\r\n" +
                    "全項目空欄の検索は負荷が高いため実行できません。",
                    "検索条件不足",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DoTrace(p);
            
        }

        private void DoTrace(TraceSearchParameters p)
        {
            try
            {
                TraceResult result;

                // 通常トレース実行
                result = _liquidService.ExecuteTrace(p);

                var displayResult = _liquidService.BuildDisplayResult(result);
                _currentMaxDepth = displayResult == null ? 0 : displayResult.MaxMiddleDepth;

                // ★ 表示テーブルはサービス側の完成品をそのまま使う
                var displayTable = _liquidService.BuildDisplayTable(result);

               
                // ★ 線レンジはサービス側で確定済みのものをそのまま受け取る
                if (displayResult != null)
                {
                    _nodeRenderRanges = displayResult.NodeRenderRanges != null
                        ? new Dictionary<string, NodeRenderRange>(
                            displayResult.NodeRenderRanges,
                            StringComparer.OrdinalIgnoreCase)
                        : new Dictionary<string, NodeRenderRange>(StringComparer.OrdinalIgnoreCase);

                    _middleTreeRenderRanges = displayResult.MiddleTreeRenderRanges != null
                        ? new Dictionary<string, MiddleTreeRenderRange>(
                            displayResult.MiddleTreeRenderRanges,
                            StringComparer.OrdinalIgnoreCase)
                        : new Dictionary<string, MiddleTreeRenderRange>(StringComparer.OrdinalIgnoreCase);

                }
                else
                {
                    _nodeRenderRanges =
                        new Dictionary<string, NodeRenderRange>(StringComparer.OrdinalIgnoreCase);

                    _middleTreeRenderRanges =
                        new Dictionary<string, MiddleTreeRenderRange>(StringComparer.OrdinalIgnoreCase);
                }

                UnregisterNodeKeyGroupForeColorFormatting();

                dataGridStart.DataSource = null;
                dataGridMiddle.DataSource = null;
                dataGridEnd.DataSource = null;

                SetupNodeGridColumns(dataGridStart, "Start");
                dataGridStart.AutoGenerateColumns = false;
                dataGridStart.DataSource = displayTable;

                SetupMiddleGridColumns(dataGridMiddle, _currentMaxDepth);
                dataGridMiddle.AutoGenerateColumns = false;
                dataGridMiddle.DataSource = displayTable;

                SetupNodeGridColumns(dataGridEnd, "End");
                dataGridEnd.AutoGenerateColumns = false;
                dataGridEnd.DataSource = displayTable;

                _fixedGridLayoutApplied = false;
                ApplyFixedGridLayoutOnce();

                FitGridAndHeaderToColumns(dataGridStart, panelStartHeader);
                RefreshStartHeaderPanel();

                RefreshStartHeaderPanel();
                RefreshMiddleHeaderPanel();
                RefreshEndHeaderPanel();

                int tabNo =1;

                StoreDisplayArtifactsForTab(tabNo, result, displayResult);

                RebuildGridPaintCaches();
                RegisterNodeKeyGroupForeColorFormatting();

                dataGridStart.Invalidate();
                dataGridMiddle.Invalidate();
                dataGridEnd.Invalidate();
            }
            catch (Exception ex)
            {
                var logMessage = new StringBuilder();
                logMessage.AppendLine("トレース処理中にエラーが発生しました。");
                logMessage.AppendLine(BuildTraceConditionSummary(p));

                WriteAppLog(logMessage.ToString(), ex);

                MessageBox.Show(
                    "トレース処理中にエラーが発生しました。\r\n" +
                    "詳細は Logs フォルダのログを確認してください。\r\n\r\n" +
                    ex.Message,
                    "トレースエラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RebuildGridPaintCaches()
        {
            _gridPaintCache.Clear();

            BuildStartBottomDividerRowIndexCache();
            BuildMiddleHorizontalLineCache();
            BuildEndHorizontalLineCache();
            BuildGridForeColorCaches();
        }


        private string GetTableString(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
                return null;

            var value = row[columnName];
            return value == null || value == DBNull.Value ? null : value.ToString();
        }

        


        private object ToDbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
        }


        private int GetLastNonEmptyLevel(DataGridViewRow row, int maxDepth)
        {
            if (row == null)
                return 0;

            for (int level = maxDepth; level >= 1; level--)
            {
                string colName = $"Lv{level}_NodeKey";

                if (!row.DataGridView.Columns.Contains(colName))
                    continue;

                var value = row.Cells[colName].Value;
                if (value != null && value != DBNull.Value)
                {
                    string text = value.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                        return level;
                }
            }

            return 0;
        }

        private int GetCommonNodeLevel(DataGridViewRow row1, DataGridViewRow row2, int maxDepth)
        {
            if (row1 == null || row2 == null)
                return 0;

            int commonLevel = 0;

            for (int level = 1; level <= maxDepth; level++)
            {
                string colName = $"Lv{level}_NodeKey";

                if (!row1.DataGridView.Columns.Contains(colName))
                    break;

                string value1 = row1.Cells[colName].Value == null || row1.Cells[colName].Value == DBNull.Value
                    ? null
                    : row1.Cells[colName].Value.ToString();

                string value2 = row2.Cells[colName].Value == null || row2.Cells[colName].Value == DBNull.Value
                    ? null
                    : row2.Cells[colName].Value.ToString();

                if (string.IsNullOrWhiteSpace(value1) || string.IsNullOrWhiteSpace(value2))
                    break;

                if (!string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase))
                    break;

                commonLevel = level;
            }

            return commonLevel;
        }

       

        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            bool drawStartDivider = false;

            bool drawMiddleBottomLine = false;
            bool middleBottomIsOrange = false;

            bool drawMiddleLevelRightBorder = false;

            const int RouteLineWidth = 3;

            //if (grid == dataGridStart)
            //{
            //    // 左グリッドは始点切替のオレンジ線だけ
            //    drawStartDivider = ShouldDrawStartGroupDivider(dataGridMiddle, e.RowIndex);
            //}
            //else if (grid == dataGridMiddle)
            //{
            //    if (e.ColumnIndex >= 0 &&
            //        e.ColumnIndex < grid.Columns.Count &&
            //        IsMiddleVisibleColumn(grid.Columns[e.ColumnIndex]))
            //    {
            //        if (e.RowIndex >= 0)
            //        {
            //            // ▼ 既存ロジック（横線）
            //            drawMiddleBottomLine =
            //                TryGetMiddleBottomBoundaryStartLevelByNodeKeyOnly(
            //                    grid,
            //                    e.RowIndex,
            //                    out middleBottomStartLevel,
            //                    out middleBottomIsOrange);
            //        }

            //        // ▼ ヘッダーでも判定する
            //        drawMiddleLevelRightBorder =
            //            IsLastVisibleMiddleColumnOfLevel(grid, e.ColumnIndex);
            //    }
            //}
            //else if (grid == dataGridEnd)
            //{
            //    // 右グリッドは中グリッドの線情報をそのまま引き継ぐ
            //    drawMiddleBottomLine =
            //        TryGetMiddleBottomBoundaryStartLevelByNodeKeyOnly(
            //            dataGridMiddle,
            //            e.RowIndex,
            //            out middleBottomStartLevel,
            //            out middleBottomIsOrange);
            //}

            if (!drawStartDivider && !drawMiddleBottomLine && !drawMiddleLevelRightBorder)
                return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            if (grid == dataGridMiddle)
                if (grid == dataGridMiddle)
                {
                    // ▼ まず既存描画
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    // ▼ 横線（データセルだけ）
                    //if (e.RowIndex >= 0 && drawMiddleBottomLine)
                    //{
                    //    DrawMiddleHorizontalLine(
                    //        grid,
                    //        e,
                    //        middleBottomStartLevel,
                    //        false,
                    //        middleBottomIsOrange);
                    //}

                    // ▼ 縦線（ヘッダー含む）
                    if (drawMiddleLevelRightBorder)
                    {
                        using (var pen = new Pen(Color.Black, 3))
                        {
                            e.Graphics.DrawLine(
                                pen,
                                e.CellBounds.Right - 1,
                                e.CellBounds.Top,
                                e.CellBounds.Right - 1,
                                e.CellBounds.Bottom - 1);
                        }
                    }

                    e.Handled = true;
                    return;
                }

            if (grid == dataGridEnd)
            {
                if (drawMiddleBottomLine)
                {
                    Color lineColor = middleBottomIsOrange ? Color.DarkOrange : Color.Black;

                    using (var pen = new Pen(lineColor, RouteLineWidth))
                    {
                        e.Graphics.DrawLine(
                            pen,
                            e.CellBounds.Left,
                            e.CellBounds.Bottom - 1,
                            e.CellBounds.Right,
                            e.CellBounds.Bottom - 1);
                    }
                }

                e.Handled = true;
                return;
            }

            if (drawStartDivider)
            {
                using (var pen = new Pen(Color.DarkOrange, RouteLineWidth))
                {
                    e.Graphics.DrawLine(
                        pen,
                        e.CellBounds.Left,
                        e.CellBounds.Bottom - 1,
                        e.CellBounds.Right,
                        e.CellBounds.Bottom - 1);
                }
            }

            e.Handled = true;
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
            //if (_excelTargetTabs.Contains(tabNo))
            //{
            //    _excelTargetTabs.Remove(tabNo);
            //}
        }

        #endregion

        #region CSV出力 [10]

        //private void btnCsvOutput_Click(object sender, EventArgs e)
        //{
        //    int tabNo = tcSearchSlots.SelectedIndex + 1;

        //    TraceResult result;
        //    if (!_tabTraceResults.TryGetValue(tabNo, out result))
        //    {
        //        MessageBox.Show("現在のタブにトレース結果がありません。", "CSV出力",
        //            MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    using (var dlg = new SaveFileDialog())
        //    {
        //        dlg.Filter = "CSV ファイル (*.csv)|*.csv";
        //        dlg.FileName = "LiquidTrace_Tab" + tabNo + ".csv";

        //        if (dlg.ShowDialog(this) != DialogResult.OK)
        //        {
        //            return;
        //        }

        //        try
        //        {
        //            ExportHelper.ExportTraceResultToCsv(result, dlg.FileName);
        //            MessageBox.Show("CSV 出力が完了しました。", "CSV出力",
        //                MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("CSV 出力に失敗しました。\r\n" + ex.Message,
        //                "CSV出力エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        #endregion

        #region EXCEL出力 [13]

        private void btnExcelOutput_Click(object sender, EventArgs e)
        {
            try
            {
                if (!HasAnyVisibleDataForExcelExport())
                {
                    MessageBox.Show(
                        this,
                        "出力対象の表示データがありません。",
                        "EXCEL出力",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                int tabNo = GetCurrentTraceTabNo();

                TraceDisplayResult displayResult = null;
                if (tabNo > 0)
                {
                    _tabDisplayResults.TryGetValue(tabNo, out displayResult);
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

                    if (sfd.ShowDialog(this) != DialogResult.OK)
                        return;

                    ExcelExportHelper.ExportCurrentGridsToExcel(
                        sfd.FileName,
                        dataGridStart,
                        dataGridMiddle,
                        dataGridEnd,
                        "TraceResult",
                        displayResult);

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

        private string BuildExcelExportFileName()
        {
            string suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return "LotTrace_Export_" + suffix + ".xlsx";
        }

        private bool HasAnyVisibleDataForExcelExport()
        {
            return HasAnyVisibleData(dataGridStart)
                || HasAnyVisibleData(dataGridMiddle)
                || HasAnyVisibleData(dataGridEnd);
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

        #region 交点検出 [14]

        private void btnDetectCrossPoints_Click(object sender, EventArgs e)
        {
            if (_excelTargetTabs.Count == 0)
            {
                MessageBox.Show("交点検出対象のタブが選択されていません。\r\n" +
                                "チェックボックス [12] を ON にしてください。",
                                "交点検出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var targets = new Dictionary<int, TraceResult>();
            foreach (int tabNo in _excelTargetTabs)
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
                _lastCrossPoints = _liquidService.DetectCrossPoints(targets);
            }
            catch (Exception ex)
            {
                MessageBox.Show("交点検出中にエラーが発生しました。\r\n" + ex.Message,
                    "交点検出エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //dataGridIntersection.DataSource = _lastCrossPoints;
            //swichTab.SelectedTab = tpIntersection;
        }

        #endregion

        #region 瓶設備画面への遷移 [18]

        private void btnBottleTrace_Click(object sender, EventArgs e)
        {
            using (var form = new BottleTraceForm(_bottleService))
            {
                form.ShowDialog(this);
            }
        }

        #endregion

        #region 交点タブ クリア [23]

        private void btnIntersectionClear_Click(object sender, EventArgs e)
        {
            _lastCrossPoints = null;
            //dataGridIntersection.DataSource = null;
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
                dlg.Filter = "CSV ファイル (*.csv)|*.csv";
                dlg.FileName = "CrossPoints.csv";

                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                // CrossPointRecord → DataTable 変換
                var table = new DataTable();
                table.Columns.Add("ProductionOrderNumber");
                table.Columns.Add("ItemName");
                table.Columns.Add("ItemCode");
                table.Columns.Add("LotNumber");
                table.Columns.Add("TabNumbers");

                foreach (CrossPointRecord r in _lastCrossPoints)
                {
                    table.Rows.Add(
                        r.ProductionOrderNumber,
                        r.ItemName,
                        r.ItemCode,
                        r.LotNumber,
                        r.TabNumbers);
                }

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

        #endregion

        

        private void ExportDebugDataTableToCsv(DataTable table, string fileNameWithoutExtension)
        {
            if (table == null || string.IsNullOrWhiteSpace(fileNameWithoutExtension))
                return;

            string debugDir = GetDebugDirectoryPath();
            string filePath = BuildUniqueDebugFilePath(
                debugDir,
                fileNameWithoutExtension,
                ".csv");

            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                // ヘッダ
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    if (col > 0)
                        writer.Write(",");

                    writer.Write(EscapeCsv(table.Columns[col].ColumnName));
                }
                writer.WriteLine();

                // データ
                foreach (DataRow row in table.Rows)
                {
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        if (col > 0)
                            writer.Write(",");

                        object value = row[col];
                        string text;

                        if (value == null || value == DBNull.Value)
                        {
                            text = "";
                        }
                        else if (value is DateTime dt)
                        {
                            text = dt.ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        else
                        {
                            text = value.ToString();
                        }

                        writer.Write(EscapeCsv(text));
                    }

                    writer.WriteLine();
                }
            }
        }

        private string GetDebugDirectoryPath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string debugDir = Path.Combine(baseDir, "Debug");

            if (!Directory.Exists(debugDir))
            {
                Directory.CreateDirectory(debugDir);
            }

            return debugDir;
        }

        private string BuildUniqueDebugFilePath(string directoryPath, string baseFileNameWithoutExtension, string extension)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string firstPath = Path.Combine(
                directoryPath,
                baseFileNameWithoutExtension + "_" + timestamp + extension);

            if (!File.Exists(firstPath))
            {
                return firstPath;
            }

            for (int i = 1; i <= 999; i++)
            {
                string retryPath = Path.Combine(
                    directoryPath,
                    baseFileNameWithoutExtension + "_" + timestamp + "_" + i.ToString("D3") + extension);

                if (!File.Exists(retryPath))
                {
                    return retryPath;
                }
            }

            return Path.Combine(
                directoryPath,
                baseFileNameWithoutExtension + "_" + timestamp + "_" + Guid.NewGuid().ToString("N") + extension);
        }

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

        private string EscapeCsv(string value)
        {
            if (value == null)
                return "";

            bool needQuote =
                value.Contains(",") ||
                value.Contains("\"") ||
                value.Contains("\r") ||
                value.Contains("\n");

            if (value.Contains("\""))
            {
                value = value.Replace("\"", "\"\"");
            }

            return needQuote ? $"\"{value}\"" : value;
        }

        private void ExportTraceDebugCsvs(TraceResult result)
        {
            if (result == null)
                return;

            string debugDir = GetDebugDirectoryPath();

            try
            {
                var nodesTable = BuildTraceNodesDebugTable(result);
                string nodesPath = BuildUniqueDebugFilePath(
                    debugDir,
                    "LotTrace_Debug_Nodes",
                    ".csv");
                ExportDataTableToCsv(nodesTable, nodesPath);

                var linksTable = BuildTraceLinksDebugTable(result);
                string linksPath = BuildUniqueDebugFilePath(
                    debugDir,
                    "LotTrace_Debug_Links",
                    ".csv");
                ExportDataTableToCsv(linksTable, linksPath);

                var pathRowsTable = BuildTracePathRowsDebugTable(result);
                string pathRowsPath = BuildUniqueDebugFilePath(
                    debugDir,
                    "LotTrace_Debug_PathRows",
                    ".csv");
                ExportDataTableToCsv(pathRowsTable, pathRowsPath);
            }
            catch (Exception ex)
            {
                WriteAppLog("デバッグCSV出力中にエラーが発生しました。", ex);

                MessageBox.Show(
                    "デバッグCSV出力中にエラーが発生しました。\r\n" +
                    "詳細は Logs フォルダを確認してください。\r\n\r\n" +
                    ex.Message,
                    "デバッグCSV出力エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private DataTable BuildTraceNodesDebugTable(TraceResult result)
        {
            var table = new DataTable();

            table.Columns.Add("NodeKey");
            table.Columns.Add("MasterKey");
            table.Columns.Add("ParentKey");
            table.Columns.Add("ProductionOrderNumber");
            table.Columns.Add("LotNumber");
            table.Columns.Add("ItemCode");
            table.Columns.Add("ItemName");
            table.Columns.Add("StartDate");
            table.Columns.Add("Weight");
            table.Columns.Add("Depth", typeof(int));
            table.Columns.Add("NodeType");
            table.Columns.Add("ParentNodeKeys");
            table.Columns.Add("ChildNodeKeys");
            table.Columns.Add("ParentLinkCount", typeof(int));
            table.Columns.Add("ChildLinkCount", typeof(int));
            table.Columns.Add("IsRoot", typeof(bool));
            table.Columns.Add("IsLeaf", typeof(bool));

            if (result == null || result.AllNodes == null)
                return table;

            foreach (var node in result.AllNodes)
            {
                if (node == null)
                    continue;

                var row = table.NewRow();

                row["NodeKey"] = ToDbValue(GetDebugNodeKey(node));
                row["MasterKey"] = ToDbValue(node.ControlMasterKey);
                row["ParentKey"] = ToDbValue(node.ParentKey);
                row["ProductionOrderNumber"] = ToDbValue(node.ProductionOrderNumber);
                row["LotNumber"] = ToDbValue(node.LotNumber);
                row["ItemCode"] = ToDbValue(node.ItemCode);
                row["ItemName"] = ToDbValue(node.ItemName);
                row["StartDate"] = node.StartDate.HasValue
                    ? node.StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    : (object)DBNull.Value;
                row["Weight"] = node.Weight.HasValue
                    ? (object)node.Weight.Value
                    : DBNull.Value;
                row["Depth"] = node.Depth;
                row["NodeType"] = ToDbValue(node.NodeType);

                row["ParentNodeKeys"] = ToDbValue(string.Join(" | ",
                    node.ParentNodes.Select(GetDebugNodeKey).Where(x => !string.IsNullOrWhiteSpace(x))));

                row["ChildNodeKeys"] = ToDbValue(string.Join(" | ",
                    node.ChildNodes.Select(GetDebugNodeKey).Where(x => !string.IsNullOrWhiteSpace(x))));

                row["ParentLinkCount"] = node.ParentLinks == null ? 0 : node.ParentLinks.Count;
                row["ChildLinkCount"] = node.ChildLinks == null ? 0 : node.ChildLinks.Count;
                row["IsRoot"] = node.ParentNodes == null || node.ParentNodes.Count == 0;
                row["IsLeaf"] = node.ChildNodes == null || node.ChildNodes.Count == 0;

                table.Rows.Add(row);
            }

            return table;
        }

        private DataTable BuildTraceLinksDebugTable(TraceResult result)
        {
            var table = new DataTable();

            table.Columns.Add("LinkIdentityKey");
            table.Columns.Add("SourceTable");
            table.Columns.Add("MaterialAInputType");
            table.Columns.Add("SlotNo", typeof(int));
            table.Columns.Add("ParentLotNumber");
            table.Columns.Add("ParentNodeKey");
            table.Columns.Add("ParentMasterKey");
            table.Columns.Add("ParentOrder");
            table.Columns.Add("ParentLot");
            table.Columns.Add("ChildNodeKey");
            table.Columns.Add("ChildMasterKey");
            table.Columns.Add("ChildOrder");
            table.Columns.Add("ChildLot");

            if (result == null || result.AllLinks == null)
                return table;

            foreach (var link in result.AllLinks)
            {
                if (link == null)
                    continue;

                var row = table.NewRow();

                row["LinkIdentityKey"] = ToDbValue(link.LinkIdentityKey);
                row["SourceTable"] = link.SourceTable.ToString();
                row["MaterialAInputType"] = link.MaterialAInputType.ToString();
                row["SlotNo"] = link.SlotNo;
                row["ParentLotNumber"] = ToDbValue(link.ParentLotNumber);

                row["ParentNodeKey"] = ToDbValue(GetDebugNodeKey(link.ParentNode));
                row["ParentMasterKey"] = ToDbValue(link.ParentNode == null ? null : link.ParentNode.ControlMasterKey);
                row["ParentOrder"] = ToDbValue(link.ParentNode == null ? null : link.ParentNode.ProductionOrderNumber);
                row["ParentLot"] = ToDbValue(link.ParentNode == null ? null : link.ParentNode.LotNumber);

                row["ChildNodeKey"] = ToDbValue(GetDebugNodeKey(link.ChildNode));
                row["ChildMasterKey"] = ToDbValue(link.ChildNode == null ? null : link.ChildNode.ControlMasterKey);
                row["ChildOrder"] = ToDbValue(link.ChildNode == null ? null : link.ChildNode.ProductionOrderNumber);
                row["ChildLot"] = ToDbValue(link.ChildNode == null ? null : link.ChildNode.LotNumber);

                table.Rows.Add(row);
            }

            return table;
        }

        private MiddleLinkContext BuildMiddleLinkContext(DataGridView grid, int rowIndex)
        {
            var currentCell = GetMiddleDisplayCellFromGrid(grid, rowIndex);
            if (currentCell == null)
                return null;

            if (string.IsNullOrWhiteSpace(currentCell.DisplayParentNodeKey))
                return null;

            var context = new MiddleLinkContext
            {
                CurrentCell = currentCell,
                CurrentRowIndex = rowIndex
            };

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (grid.Rows[i].IsNewRow)
                    continue;

                var otherCell = GetMiddleDisplayCellFromGrid(grid, i);
                if (otherCell == null)
                    continue;

                if (!string.Equals(
                        currentCell.DisplayParentNodeKey,
                        otherCell.DisplayParentNodeKey,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // 同じ親でも、IncomingLinkKey が空なら枝として扱わない
                if (string.IsNullOrWhiteSpace(otherCell.IncomingLinkKey))
                    continue;

                if (!context.SiblingRowIndices.Contains(i))
                {
                    context.SiblingRowIndices.Add(i);
                }
            }

            context.SiblingRowIndices.Sort();
            return context;
        }

        private DataTable BuildTracePathRowsDebugTable(TraceResult result)
        {
            var table = new DataTable();

            table.Columns.Add("RowNo", typeof(int));
            table.Columns.Add("RootGroupKey");
            table.Columns.Add("StartNodeKey");
            table.Columns.Add("EndNodeKey");
            table.Columns.Add("NodePath");
            table.Columns.Add("LinkPath");
            table.Columns.Add("MiddleCount", typeof(int));
            table.Columns.Add("LinkCount", typeof(int));

            if (result == null || result.PathRows == null)
                return table;

            for (int i = 0; i < result.PathRows.Count; i++)
            {
                var pathRow = result.PathRows[i];
                if (pathRow == null)
                    continue;

                var row = table.NewRow();

                row["RowNo"] = i + 1;
                row["RootGroupKey"] = ToDbValue(pathRow.RootGroupKey);
                row["StartNodeKey"] = ToDbValue(GetDebugNodeKey(pathRow.StartNode));
                row["EndNodeKey"] = ToDbValue(GetDebugNodeKey(pathRow.EndNode));
                row["NodePath"] = ToDbValue(BuildDebugNodePath(pathRow));
                row["LinkPath"] = ToDbValue(BuildDebugLinkPath(pathRow));
                row["MiddleCount"] = pathRow.MiddleNodes == null ? 0 : pathRow.MiddleNodes.Count;
                row["LinkCount"] = pathRow.PathLinks == null ? 0 : pathRow.PathLinks.Count;

                table.Rows.Add(row);
            }

            return table;
        }

        private class MiddleLinkContext
        {
            public TraceDisplayCell CurrentCell { get; set; }
            public int CurrentRowIndex { get; set; }

            public List<int> SiblingRowIndices { get; } = new List<int>();

            public int SiblingCount
            {
                get { return SiblingRowIndices.Count; }
            }

            public int? FirstSiblingRowIndex
            {
                get
                {
                    return SiblingRowIndices.Count == 0
                        ? (int?)null
                        : SiblingRowIndices.Min();
                }
            }

            public int? LastSiblingRowIndex
            {
                get
                {
                    return SiblingRowIndices.Count == 0
                        ? (int?)null
                        : SiblingRowIndices.Max();
                }
            }
        }

        private string GetDebugNodeKey(ProductionResultNode node)
        {
            if (node == null)
                return null;

            if (!string.IsNullOrWhiteSpace(node.ControlMasterKey))
                return node.ControlMasterKey;

            if (!string.IsNullOrWhiteSpace(node.LotNumber))
                return "LOT|" + node.LotNumber;

            return null;
        }

        private string BuildDebugNodePath(TracePathRow pathRow)
        {
            if (pathRow == null)
                return null;

            var parts = new List<string>();

            if (pathRow.StartNode != null)
                parts.Add(GetDebugNodeKey(pathRow.StartNode));

            if (pathRow.MiddleNodes != null)
            {
                foreach (var node in pathRow.MiddleNodes)
                {
                    parts.Add(GetDebugNodeKey(node));
                }
            }

            if (pathRow.EndNode != null)
                parts.Add(GetDebugNodeKey(pathRow.EndNode));

            return string.Join(" -> ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private string BuildDebugLinkPath(TracePathRow pathRow)
        {
            if (pathRow == null || pathRow.PathLinks == null || pathRow.PathLinks.Count == 0)
                return null;

            var parts = new List<string>();

            foreach (var link in pathRow.PathLinks)
            {
                if (link == null)
                    continue;

                string parentKey = GetDebugNodeKey(link.ParentNode) ?? "";
                string childKey = GetDebugNodeKey(link.ChildNode) ?? "";
                string source = link.SourceTable.ToString();
                string inputType = link.MaterialAInputType.ToString();
                string slotNo = link.SlotNo.ToString();

                parts.Add(
                    parentKey + " -[" + source + "/" + inputType + "/Slot:" + slotNo + "]-> " + childKey);
            }

            return string.Join(" || ", parts);
        }

        private void ExportDataTableToCsv(DataTable table, string filePath)
        {
            if (table == null)
                return;

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var header = string.Join(",",
                    table.Columns.Cast<DataColumn>()
                        .Select(c => EscapeCsv(c.ColumnName)));
                writer.WriteLine(header);

                foreach (DataRow row in table.Rows)
                {
                    var fields = table.Columns.Cast<DataColumn>()
                        .Select(c => EscapeCsv(row[c] == DBNull.Value ? "" : Convert.ToString(row[c])))
                        .ToArray();

                    writer.WriteLine(string.Join(",", fields));
                }
            }
        }

        private sealed class Lv1RenderRange
        {
            public string GroupKey { get; set; }
            public string Lv1NodeKey { get; set; }

            public List<int> RowIndices { get; } = new List<int>();

            public int RowCount
            {
                get { return RowIndices.Count; }
            }

            public int? FirstRowIndex
            {
                get { return RowIndices.Count == 0 ? (int?)null : RowIndices.Min(); }
            }

            public int? LastRowIndex
            {
                get { return RowIndices.Count == 0 ? (int?)null : RowIndices.Max(); }
            }
        }


        private void ExportTraceDisplayKeysDebugCsv(TraceDisplayResult displayResult)
        {
            if (displayResult == null || displayResult.Rows == null || displayResult.Rows.Count == 0)
                return;

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string debugDir = Path.Combine(baseDir, "Debug");

                if (!Directory.Exists(debugDir))
                    Directory.CreateDirectory(debugDir);

                string filePath = Path.Combine(
                    debugDir,
                    "TraceDisplayKeys_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
                );

                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // ヘッダー
                    writer.WriteLine(string.Join(",",
                        "No",
                        "RootGroupKey",
                        "PathKey",
                        "RouteSystem",
                        "IsDisplayTarget",
                        "SuppressReason",
                        "StartLot",
                        "EndLot",
                        "MiddleCount",
                        "LotTraceKey",
                        "UpstreamLotOnlyKey"
                    ));

                    int index = 0;

                    foreach (var row in displayResult.Rows)
                    {
                        index++;

                        string startLot = row.Start != null ? SafeCsv(row.Start.LotNumber) : "";
                        string endLot = row.End != null ? SafeCsv(row.End.LotNumber) : "";

                        int middleCount = row.Middles != null ? row.Middles.Count : 0;

                        writer.WriteLine(string.Join(",",
                            index,
                            SafeCsv(row.RootGroupKey),
                            SafeCsv(row.PathKey),
                            SafeCsv(row.RouteSystem),
                            row.IsDisplayTarget ? "1" : "0",
                            SafeCsv(row.SuppressReason),
                            startLot,
                            endLot,
                            middleCount,
                            SafeCsv(row.LotTraceKey),
                            SafeCsv(row.UpstreamLotOnlyKey)
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                // デバッグ用途なので落とさない
                Console.WriteLine("ExportTraceDisplayKeysDebugCsv Error: " + ex.Message);
            }
        }

        private string SafeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            value = value.Replace("\"", "\"\"");

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return "\"" + value + "\"";

            return value;
        }


        [System.Diagnostics.Conditional("DEBUG")]
        private void DebugMiddleDivider(
    int rowIndex,
    TraceDisplayCell cell,
    NodeRenderRange parentRange,
    bool result)
        {
            string nodeKey = cell == null ? "(null)" : cell.NodeKey ?? "(null)";
            string parentKey = cell == null ? "(null)" : cell.DisplayParentNodeKey ?? "(null)";

            string first = parentRange == null || !parentRange.FirstChildRowIndex.HasValue
                ? "(null)"
                : parentRange.FirstChildRowIndex.Value.ToString();

            string last = parentRange == null || !parentRange.LastChildRowIndex.HasValue
                ? "(null)"
                : parentRange.LastChildRowIndex.Value.ToString();

            string childCount = parentRange == null
                ? "(null)"
                : parentRange.ChildCount.ToString();

            System.Diagnostics.Debug.WriteLine(
                $"[MiddleDivider] row={rowIndex}, node={nodeKey}, parent={parentKey}, first={first}, last={last}, childCount={childCount}, result={result}");
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void DebugNodeRenderRanges(Dictionary<string, NodeRenderRange> ranges)
        {
            if (ranges == null)
                return;

            foreach (var pair in ranges.OrderBy(x => x.Key))
            {
                var range = pair.Value;

                string children = range.ChildRowIndices == null || range.ChildRowIndices.Count == 0
                    ? "(none)"
                    : string.Join(",", range.ChildRowIndices.OrderBy(x => x));

                System.Diagnostics.Debug.WriteLine(
                    $"[NodeRange] node={pair.Key}, row={range.RowIndex}, childCount={range.ChildCount}, first={range.FirstChildRowIndex}, last={range.LastChildRowIndex}, childRows={children}");
            }
        }

        

        

        private string BuildMiddleTreeRenderRangeKey(
    string rootGroupKey,
    int level,
    string nodeKey,
    string displayParentNodeKey,
    string incomingLinkKey,
    string upstreamPathKey,
    string downstreamPathKey)
        {
            string group = string.IsNullOrWhiteSpace(rootGroupKey) ? string.Empty : rootGroupKey;
            string node = string.IsNullOrWhiteSpace(nodeKey) ? string.Empty : nodeKey;
            string parent = string.IsNullOrWhiteSpace(displayParentNodeKey) ? string.Empty : displayParentNodeKey;
            string incoming = string.IsNullOrWhiteSpace(incomingLinkKey) ? string.Empty : incomingLinkKey;
            string upstream = string.IsNullOrWhiteSpace(upstreamPathKey) ? string.Empty : upstreamPathKey;
            string downstream = string.IsNullOrWhiteSpace(downstreamPathKey) ? string.Empty : downstreamPathKey;

            return group
                + "||LV=" + level.ToString()
                + "||NODE=" + node
                + "||PARENT=" + parent
                + "||IN=" + incoming
                + "||UP=" + upstream
                + "||DOWN=" + downstream;
        }

        private string GetTreeGroupKeyForRow(DataRow row)
        {
            if (row == null)
                return string.Empty;

            string trunkKey = GetTableString(row, "StartTrunkGroupKey");
            if (!string.IsNullOrWhiteSpace(trunkKey))
                return trunkKey.Trim();

            string rootGroupKey = GetTableString(row, "RootGroupKey");
            return string.IsNullOrWhiteSpace(rootGroupKey)
                ? string.Empty
                : rootGroupKey.Trim();
        }



        private int GetMiddleLevelFromColumn(DataGridViewColumn column)
        {
            if (column == null)
                return 0;

            string name = column.Name;
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            if (!name.StartsWith("Lv", StringComparison.OrdinalIgnoreCase))
                return 0;

            int underscoreIndex = name.IndexOf('_');
            if (underscoreIndex < 0)
                return 0;

            string levelText = name.Substring(2, underscoreIndex - 2);

            int level;
            if (!int.TryParse(levelText, out level))
                return 0;

            return level;
        }

        
    

        
        private bool TryGetMiddleBottomBoundaryStartLevel(
    DataGridView grid,
    int rowIndex,
    out int startLevel,
    out bool isOrange)
        {
            startLevel = 0;
            isOrange = false;

            if (grid == null || rowIndex < 0)
                return false;

            int foundLevel = 0;

            for (int level = 1; level <= _currentMaxDepth; level++)
            {
                if (!ShouldDrawMiddleLevelLastRowDivider(grid, rowIndex, level))
                    continue;

                foundLevel = level;
            }

            if (foundLevel <= 0)
                return false;

            startLevel = foundLevel;
            isOrange = (foundLevel == 1);
            return true;
        }

        private bool TryGetMiddleBoundaryColorInfoForEndGrid(
    DataGridView middleGrid,
    int rowIndex,
    out bool isOrange)
        {
            isOrange = false;

            if (middleGrid == null || rowIndex < 0)
                return false;

            int startLevel = 0;
            bool boundaryIsOrange = false;

            bool hasBottomBoundary =
                TryGetMiddleBottomBoundaryStartLevel(
                    middleGrid,
                    rowIndex,
                    out startLevel,
                    out boundaryIsOrange);

            if (!hasBottomBoundary)
                return false;

            isOrange = boundaryIsOrange;
            return true;
        }

        private void LogMiddle(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void Grid_CellFormatting_Hover(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // 選択優先
            if (grid.Rows[e.RowIndex].Selected)
                return;

            if (e.RowIndex == _hoverRowIndex && e.ColumnIndex == _hoverColumnIndex)
            {
                e.CellStyle.BackColor = Color.FromArgb(225, 235, 250);
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

        #region 旧・中右グリッド境界描画ロジック（退避中）
        // NodeKey切替ベース正式ロジック採用により、現在は未使用。
        // すぐ消さずに比較用として一時退避しておく。

        

        private bool ShouldDrawMiddleLevelLastRowDivider(DataGridView grid, int rowIndex, int level)
        {
            if (grid == null)
                return false;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return false;

            if (level <= 0)
                return false;

            string currentNodeKey = GetMiddleNodeKeyFromBoundRow(grid, rowIndex, level);
            if (string.IsNullOrWhiteSpace(currentNodeKey))
                return false;

            if (rowIndex >= grid.Rows.Count - 1)
                return true;

            string nextNodeKey = GetMiddleNodeKeyFromBoundRow(grid, rowIndex + 1, level);

            return !string.Equals(
                currentNodeKey,
                nextNodeKey,
                StringComparison.OrdinalIgnoreCase);
        }

        private bool ShouldDrawMiddleLevelFirstRowDivider(DataGridView grid, int rowIndex, int level)
        {
            if (grid == null)
                return false;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return false;

            if (level <= 0)
                return false;

            string currentNodeKey = GetMiddleNodeKeyFromBoundRow(grid, rowIndex, level);
            if (string.IsNullOrWhiteSpace(currentNodeKey))
                return false;

            if (rowIndex == 0)
                return true;

            string prevNodeKey = GetMiddleNodeKeyFromBoundRow(grid, rowIndex - 1, level);

            return !string.Equals(
                currentNodeKey,
                prevNodeKey,
                StringComparison.OrdinalIgnoreCase);
        }

        //private int GetFirstVisibleMiddleColumnIndexByLevel(DataGridView grid, int level)
        //{
        //    if (grid == null || level <= 0)
        //        return -1;

        //    string prefix = "Lv" + level + "_";

        //    for (int i = 0; i < grid.Columns.Count; i++)
        //    {
        //        var column = grid.Columns[i];
        //        if (column == null)
        //            continue;

        //        if (!column.Visible)
        //            continue;

        //        if (!IsMiddleVisibleColumn(column))
        //            continue;

        //        if (column.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        //            return i;
        //    }

        //    return -1;
        //}

        //private int GetLastVisibleMiddleColumnIndex(DataGridView grid)
        //{
        //    if (grid == null)
        //        return -1;

        //    for (int i = grid.Columns.Count - 1; i >= 0; i--)
        //    {
        //        var column = grid.Columns[i];
        //        if (column == null)
        //            continue;

        //        if (!column.Visible)
        //            continue;

        //        if (!IsMiddleVisibleColumn(column))
        //            continue;

        //        return i;
        //    }

        //    return -1;
        //}

        private bool TryGetMiddleBottomBoundaryStartLevelForUnifiedMode(
            DataGridView grid,
            int rowIndex,
            out int startLevel,
            out bool isOrange)
        {
            startLevel = 0;
            isOrange = false;

            if (grid == null || rowIndex < 0)
                return false;

            int deepestLevel = 0;
            bool hasAnyBoundary = false;
            bool hasLv1Boundary = false;

            for (int level = 1; level <= _currentMaxDepth; level++)
            {
                if (!HasUnifiedMiddleBoundaryAtLevel(grid, rowIndex, level))
                    continue;

                hasAnyBoundary = true;

                if (level > deepestLevel)
                    deepestLevel = level;

                if (level == 1)
                    hasLv1Boundary = true;
            }

            if (!hasAnyBoundary || deepestLevel <= 0)
                return false;

            startLevel = deepestLevel;
            isOrange = hasLv1Boundary;
            return true;
        }

        private bool HasUnifiedMiddleBoundaryAtLevel(
            DataGridView grid,
            int rowIndex,
            int level)
        {
            if (grid == null)
                return false;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                return false;

            if (level <= 0)
                return false;

            bool isEndBoundary =
                ShouldDrawMiddleLevelLastRowDivider(grid, rowIndex, level);

            bool isStartBoundaryShiftedToBottom =
                rowIndex + 1 < grid.Rows.Count &&
                ShouldDrawMiddleLevelFirstRowDivider(grid, rowIndex + 1, level);

            return isEndBoundary || isStartBoundaryShiftedToBottom;
        }

        private bool ShouldDrawDividerForTreeGroup(DataGridView grid, int rowIndex, int maxDepth)
        {
            if (grid == null)
                return false;

            if (rowIndex < 0 || rowIndex >= grid.Rows.Count - 1)
                return false;

            var currentRow = grid.Rows[rowIndex];
            var nextRow = grid.Rows[rowIndex + 1];

            if (currentRow.IsNewRow || nextRow.IsNewRow)
                return false;

            int currentDepth = GetLastNonEmptyLevel(currentRow, maxDepth);
            int nextDepth = GetLastNonEmptyLevel(nextRow, maxDepth);

            if (currentDepth <= 0)
                return false;

            int commonLevel = GetCommonNodeLevel(currentRow, nextRow, maxDepth);

            return commonLevel < currentDepth;
        }

        // もし残っていればこれも旧右グリッド色判定側としてここへ移動
        // private bool TryGetMiddleBoundaryColorInfoForEndGrid(...)

        #endregion

        #region 新罫線メソッド群
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

            int firstRowIndex = grid.FirstDisplayedScrollingRowIndex;
            if (firstRowIndex < 0)
                return;

            int displayedRowCount = grid.DisplayedRowCount(true);
            if (displayedRowCount <= 0)
                return;

            int lastRowIndex = firstRowIndex + displayedRowCount - 1;
            if (lastRowIndex >= grid.Rows.Count)
                lastRowIndex = grid.Rows.Count - 1;

            using (var pen = new Pen(Color.FromArgb(120, 72, 32), 2))
            {
                foreach (int rowIndex in _gridPaintCache.StartBottomDividerRows)
                {
                    if (rowIndex < firstRowIndex || rowIndex > lastRowIndex)
                        continue;

                    Rectangle rect = grid.GetRowDisplayRectangle(rowIndex, true);
                    if (rect.Height <= 0)
                        continue;

                    int y = rect.Bottom - 1;

                    e.Graphics.DrawLine(
                        pen,
                        grid.DisplayRectangle.Left,
                        y,
                        grid.DisplayRectangle.Right,
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

            int firstRowIndex = grid.FirstDisplayedScrollingRowIndex;
            if (firstRowIndex < 0)
                return;

            int displayedRowCount = grid.DisplayedRowCount(true);
            if (displayedRowCount <= 0)
                return;

            int lastRowIndex = firstRowIndex + displayedRowCount - 1;
            if (lastRowIndex >= grid.Rows.Count)
                lastRowIndex = grid.Rows.Count - 1;

            

            foreach (var line in _gridPaintCache.MiddleHorizontalLines)
            {
                if (line == null)
                    continue;

                if (line.StartRowIndex < firstRowIndex || line.StartRowIndex > lastRowIndex)
                    continue;

                Rectangle rowRect = grid.GetRowDisplayRectangle(line.StartRowIndex, true);
                if (rowRect.Height <= 0)
                    continue;

                int left = GetMiddleLineLeft(grid, line);
                int right = GetMiddleLineRight(grid, line);

                if (right <= left)
                    continue;

                Color color = ResolveMiddleLineColor(line.LineKind);

                using (var pen = new Pen(color, 2))
                {
                    int y = rowRect.Bottom - 1;
                    graphics.DrawLine(pen, left, y, right, y);
                }
            }
        }

        private int GetMiddleLineLeft(DataGridView grid, MiddleHorizontalLineDrawInfo line)
        {
            if (grid == null || line == null)
                return 0;

            int level = line.FromXLevel;
            if (level <= 1)
                return grid.DisplayRectangle.Left;

            int oneLevelWidth =
                StandardColumnWidth   // Order
                + StandardColumnWidth // Lot
                + StandardColumnWidth // ItemName
                + WideColumnWidth     // StartTime
                + StandardColumnWidth;// Weight

            return grid.DisplayRectangle.Left
                + ((level - 1) * oneLevelWidth)
                - grid.HorizontalScrollingOffset+1;
        }

        private int GetMiddleLineRight(
    DataGridView grid,
    MiddleHorizontalLineDrawInfo line)
        {
            if (grid == null)
                return 0;

            return grid.DisplayRectangle.Right;
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

            int firstRowIndex = grid.FirstDisplayedScrollingRowIndex;
            if (firstRowIndex < 0)
                return;

            int displayedRowCount = grid.DisplayedRowCount(true);
            if (displayedRowCount <= 0)
                return;

            int lastRowIndex = firstRowIndex + displayedRowCount - 1;
            if (lastRowIndex >= grid.Rows.Count)
                lastRowIndex = grid.Rows.Count - 1;

            foreach (var line in _gridPaintCache.EndHorizontalLines)
            {
                if (line == null)
                    continue;

                if (line.StartRowIndex < firstRowIndex || line.StartRowIndex > lastRowIndex)
                    continue;

                Rectangle rect = grid.GetRowDisplayRectangle(line.StartRowIndex, true);
                if (rect.Height <= 0)
                    continue;

                Color color = ResolveMiddleLineColor(line.LineKind);

                using (var pen = new Pen(color, 2))
                {
                    int y = rect.Bottom - 1;

                    e.Graphics.DrawLine(
                        pen,
                        grid.DisplayRectangle.Left,
                        y,
                        grid.DisplayRectangle.Right,
                        y);
                }
            }
        }

        private void BuildStartBottomDividerRowIndexCache()
        {
            

            var drawInfo = GetCurrentStartGridDrawInfo();
            if (drawInfo == null || drawInfo.Rows == null)
                return;

            for (int i = 0; i < drawInfo.Rows.Count; i++)
            {
                var row = drawInfo.Rows[i];
                if (row != null && row.DrawBottomDivider)
                {
                    _gridPaintCache.StartBottomDividerRows.Add(i);
                }
            }
        }

        private void BuildMiddleHorizontalLineCache()
        {
            

            var drawInfo = GetCurrentMiddleGridDrawInfo();
            if (drawInfo == null || drawInfo.HorizontalLines == null)
                return;

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

                if (string.Equals(line.LineKind, "Start", StringComparison.OrdinalIgnoreCase) && line.FromXLevel <= 0)
                {
                    line.FromXLevel = 1;
                }

                _gridPaintCache.MiddleHorizontalLines.Add(line);
            }
        }

        private void BuildEndHorizontalLineCache()
        {
            

            var drawInfo = GetCurrentEndGridDrawInfo();
            if (drawInfo == null || drawInfo.HorizontalLines == null)
                return;

            foreach (var line in drawInfo.HorizontalLines)
            {
                if (line == null)
                    continue;

                _gridPaintCache.EndHorizontalLines.Add(line);
            }
        }

        private sealed class TraceGridDrawContext
        {
            public StartGridDrawInfo Start { get; set; }
            public MiddleGridDrawInfo Middle { get; set; }
            public EndGridDrawInfo End { get; set; }

            public int RowCount { get; set; }

            public TraceGridDrawContext()
            {
                Start = new StartGridDrawInfo();
                Middle = new MiddleGridDrawInfo();
                End = new EndGridDrawInfo();
            }
        }

        private sealed class StartGridDrawInfo
        {
            public List<StartRowDrawInfo> Rows { get; } = new List<StartRowDrawInfo>();
        }

        private sealed class StartRowDrawInfo
        {
            public int RowIndex { get; set; }
            public bool DrawBottomDivider { get; set; }
        }

        private sealed class MiddleGridDrawInfo
        {
            public List<MiddleHorizontalLineDrawInfo> HorizontalLines { get; }
                = new List<MiddleHorizontalLineDrawInfo>();

            public List<MiddleVerticalLineDrawInfo> VerticalLines { get; }
                = new List<MiddleVerticalLineDrawInfo>();
        }

        private sealed class MiddleHorizontalLineDrawInfo
        {
            public int StartRowIndex { get; set; }
            public int EndRowIndex { get; set; }
            public int FromXLevel { get; set; }
            public int ToXLevel { get; set; }
            public string LineKind { get; set; }
        }

        private sealed class MiddleVerticalLineDrawInfo
        {
            public int XLevel { get; set; }
            public string LineKind { get; set; }
            public bool IncludeHeaderArea { get; set; }
        }

        private sealed class EndGridDrawInfo
        {
            public List<EndHorizontalLineDrawInfo> HorizontalLines { get; }
                = new List<EndHorizontalLineDrawInfo>();
        }

        private sealed class EndHorizontalLineDrawInfo
        {
            public int StartRowIndex { get; set; }
            public int EndRowIndex { get; set; }
            public string LineKind { get; set; }
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

        private int GetCurrentTraceTabNo()
        {
            Tag = 1;
            int tabNo;
            if (!int.TryParse(Tag.ToString(), out tabNo))
                return -1;
            return tabNo;
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

        private bool ShouldDrawStartBottomDivider(int rowIndex)
        {
            if (rowIndex < 0)
                return false;

            var drawInfo = GetCurrentStartGridDrawInfo();
            if (drawInfo == null || drawInfo.Rows == null || rowIndex > drawInfo.Rows.Count)
                return false;

            var rowInfo = drawInfo.Rows[rowIndex];
            return rowInfo != null && rowInfo.DrawBottomDivider;
        }

        private void DataGridStart_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e == null)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            // 通常描画はそのまま
            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            if (!ShouldDrawStartBottomDivider(e.RowIndex))
            {
                e.Handled = true;
                return;
            }

            DrawStartGridBottomDivider(e);

            e.Handled = true;
        }

        private void DrawStartGridBottomDivider(DataGridViewCellPaintingEventArgs e)
        {
            if (e == null)
                return;

            var rect = e.CellBounds;

            using (var pen = new Pen(Color.FromArgb(120,72,32), 2))
            {
                e.Graphics.DrawLine(
                    pen,
                    rect.Left,
                    rect.Bottom - 1,
                    rect.Right,
                    rect.Bottom - 1);
            }
        }

        private MiddleGridDrawInfo GetCurrentMiddleGridDrawInfo()
        {
            var context = GetCurrentTraceGridDrawContext();
            return context == null ? null : context.Middle;
        }

        private List<MiddleHorizontalLineDrawInfo> GetCurrentMiddleHorizontalLines()
        {
            var drawInfo = GetCurrentMiddleGridDrawInfo();
            if (drawInfo == null || drawInfo.HorizontalLines == null)
                return new List<MiddleHorizontalLineDrawInfo>();

            return drawInfo.HorizontalLines;
        }

        private List<MiddleVerticalLineDrawInfo> GetCurrentMiddleVerticalLines()
        {
            var drawInfo = GetCurrentMiddleGridDrawInfo();
            if (drawInfo == null || drawInfo.VerticalLines == null)
                return new List<MiddleVerticalLineDrawInfo>();

            return drawInfo.VerticalLines;
        }

        private List<MiddleHorizontalLineDrawInfo> GetMiddleHorizontalLinesForRow(int rowIndex)
        {
            var result = new List<MiddleHorizontalLineDrawInfo>();

            if (rowIndex < 0)
                return result;

            var lines = GetCurrentMiddleHorizontalLines();
            if (lines == null || lines.Count == 0)
                return result;

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                if (rowIndex < line.StartRowIndex)
                    continue;

                if (rowIndex > line.EndRowIndex)
                    continue;

                result.Add(line);
            }

            return result;
        }

  

        private void DataGridMiddle_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e == null)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            var lines = GetMiddleHorizontalLinesForRow(e.RowIndex);
            if (lines == null || lines.Count == 0)
            {
                e.Handled = true;
                return;
            }

            DrawMiddleHorizontalLines(grid, e, lines);

            e.Handled = true;
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

        private void DrawMiddleHorizontalLines(
            DataGridView grid,
            DataGridViewCellPaintingEventArgs e,
            List<MiddleHorizontalLineDrawInfo> lines)
        {
            if (grid == null || e == null || lines == null || lines.Count == 0)
                return;

            var matchedLines = new List<MiddleHorizontalLineDrawInfo>();

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                if (!IsMiddleCellInHorizontalLineRange(grid, e.ColumnIndex, line))
                    continue;

                matchedLines.Add(line);
            }

            if (matchedLines.Count == 0)
                return;

            var preferredLine = ResolvePreferredMiddleHorizontalLine(matchedLines);
            if (preferredLine == null)
                return;

            DrawMiddleHorizontalLine(e, preferredLine);
        }

        private void DrawMiddleHorizontalLine(
            DataGridViewCellPaintingEventArgs e,
            MiddleHorizontalLineDrawInfo line)
        {
            if (e == null || line == null)
                return;

            Color color = ResolveMiddleLineColor(line.LineKind);

            using (var pen = new Pen(color, 2))
            {
                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Left,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right,
                    e.CellBounds.Bottom - 1);
            }
        }

        private bool IsMiddleCellInHorizontalLineRange(
    DataGridView grid,
    int columnIndex,
    MiddleHorizontalLineDrawInfo line)
        {
            if (grid == null || line == null)
                return false;

            if (columnIndex < 0 || columnIndex >= grid.Columns.Count)
                return false;

            var column = grid.Columns[columnIndex];
            if (column == null || !column.Visible)
                return false;

            int? level = GetMiddleHeaderLevelFromColumnName(column.Name);
            if (!level.HasValue)
                return false;

            //int fromXLevel = line.FromXLevel;
            //if (string.Equals(line.LineKind, "Start", StringComparison.OrdinalIgnoreCase) && fromXLevel <= 0)
            //{
            //    fromXLevel = 1;
            //}

            return level.Value >= line.FromXLevel && level.Value <= line.ToXLevel;
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

        

        
        
        private string ResolvePreferredLineKind(IEnumerable<string> lineKinds)
        {
            if (lineKinds == null)
                return null;

            bool hasTrunk = false;
            bool hasBranch = false;

            foreach (var lineKind in lineKinds)
            {
                switch (lineKind)
                {
                    case "Start":
                        return "Start";
                    case "Trunk":
                        hasTrunk = true;
                        break;
                    case "Branch":
                        hasBranch = true;
                        break;
                }
            }

            if (hasTrunk)
                return "Trunk";

            if (hasBranch)
                return "Branch";

            return null;
        }



        private EndGridDrawInfo GetCurrentEndGridDrawInfo()
        {
            var context = GetCurrentTraceGridDrawContext();
            return context == null ? null : context.End;
        }

        private List<EndHorizontalLineDrawInfo> GetCurrentEndHorizontalLines()
        {
            var drawInfo = GetCurrentEndGridDrawInfo();
            if (drawInfo == null || drawInfo.HorizontalLines == null)
                return new List<EndHorizontalLineDrawInfo>();

            return drawInfo.HorizontalLines;
        }

        private List<EndHorizontalLineDrawInfo> GetEndHorizontalLinesForRow(int rowIndex)
        {
            var result = new List<EndHorizontalLineDrawInfo>();

            if (rowIndex < 0)
                return result;

            var lines = GetCurrentEndHorizontalLines();
            if (lines == null || lines.Count == 0)
                return result;

            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                if (rowIndex < line.StartRowIndex)
                    continue;

                if (rowIndex > line.EndRowIndex)
                    continue;

                result.Add(line);
            }

            return result;
        }

        

        private void DataGridEnd_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e == null)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as DataGridView;
            if (grid == null)
                return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            var lines = GetEndHorizontalLinesForRow(e.RowIndex);
            if (lines == null || lines.Count == 0)
            {
                e.Handled = true;
                return;
            }

            DrawEndHorizontalLines(e, lines);

            e.Handled = true;
        }

        private void DrawEndHorizontalLines(
    DataGridViewCellPaintingEventArgs e,
    List<EndHorizontalLineDrawInfo> lines)
        {
            if (e == null || lines == null || lines.Count == 0)
                return;

            string preferredLineKind = ResolvePreferredLineKind(
                lines.Select(x => x == null ? null : x.LineKind));

            if (string.IsNullOrWhiteSpace(preferredLineKind))
                return;

            DrawEndHorizontalLine(e, preferredLineKind);
        }

        private void DrawEndHorizontalLine(
            DataGridViewCellPaintingEventArgs e,
            string lineKind)
        {
            if (e == null)
                return;

            Color lineColor = ResolveMiddleLineColor(lineKind);

            using (var pen = new Pen(lineColor, 2))
            {
                e.Graphics.DrawLine(
                    pen,
                    e.CellBounds.Left,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right,
                    e.CellBounds.Bottom - 1);
            }
        }

        private void DataGridMiddle_Vartical_CellPainting(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e == null)
                return;

            var lines = GetCurrentMiddleVerticalLines();
            if (lines == null || lines.Count == 0)
                return;

            DrawMiddleVerticalLines(grid, e, lines);
        }

        private void DrawMiddleVerticalLines(
        DataGridView grid,
        PaintEventArgs e,
        List<MiddleVerticalLineDrawInfo> lines)
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
        MiddleVerticalLineDrawInfo line)
            {
            if (grid == null || e == null || line == null)
                return;

            int x = ResolveMiddleVerticalLineX(grid, line);
            if (x < 0)
                return;

            int top = GetMiddleGridTopIncludingHeader(grid);
            int bottom = GetMiddleGridBottom(grid);

            Color color = ResolveMiddleLineColor("Vartical");

            using (var pen = new Pen(color, 2))
            {
                e.Graphics.DrawLine(pen, x, top, x, bottom);
            }
        }

        private int ResolveMiddleVerticalLineX(
    DataGridView grid,
    MiddleVerticalLineDrawInfo line)
        {
            if (grid == null || line == null)
                return -1;

            int targetLevel = line.XLevel;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                int? level = GetMiddleHeaderLevelFromColumnName(col.Name);
                if (!level.HasValue)
                    continue;

                if (level.Value == targetLevel)
                {
                    var rect = grid.GetColumnDisplayRectangle(col.Index, true);
                    return rect.Left;
                }
            }

            return -1;
        }

        private int GetMiddleGridTopIncludingHeader(DataGridView grid)
        {
            return grid.DisplayRectangle.Top;
        }

        private int GetMiddleGridBottom(DataGridView grid)
        {
            return grid.DisplayRectangle.Bottom;
        }

        #endregion

        #region 新文字色メソッド

        private void BuildGridForeColorCaches()
        {
            BuildGridForeColorCache(dataGridStart);
            BuildGridForeColorCache(dataGridMiddle);
            BuildGridForeColorCache(dataGridEnd);
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
                    throw new InvalidOperationException("文字色キャッシュ作成時に行データを取得できません。");

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

            GridForeColorCache cache = _gridPaintCache.ForeColorCaches[grid];
            e.CellStyle.ForeColor = cache.GetRequiredColor(e.RowIndex, e.ColumnIndex);
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

        #endregion

        #region 旧2段ヘッダー削除候補

        // ▼ 旧2段ヘッダー（左・右）CellPainting
        private void dataGridStart_TwoRowHeaderCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            PaintSingleGroupTwoRowHeader(
                e,
                grid,
                _startHeaderStyle,
                BuildSingleGroupHeaderText("検索始点", grid));
        }

        private void dataGridEnd_TwoRowHeaderCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            PaintSingleGroupTwoRowHeader(
                e,
                grid,
                _endHeaderStyle,
                BuildSingleGroupHeaderText("検索終点", grid));
        }

        // ▼ 旧2段ヘッダー共通
        private void ApplyTwoRowHeaderStyle(DataGridView grid, HeaderVisualStyle style)
        {
            if (grid == null || style == null)
                return;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 48;

            grid.ColumnHeadersDefaultCellStyle.BackColor = style.GroupBackColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = style.GroupForeColor;
            grid.ColumnHeadersDefaultCellStyle.Font = style.ColumnFont;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = style.GroupBackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = style.GroupForeColor;
            grid.ColumnHeadersDefaultCellStyle.Padding = Padding.Empty;

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        private int GetVisibleRowCount(DataGridView grid)
        {
            if (grid == null)
                return 0;

            var dt = grid.DataSource as DataTable;
            if (dt != null)
                return dt.Rows.Count;

            return grid.Rows
                .Cast<DataGridViewRow>()
                .Count(r => !r.IsNewRow);
        }

        private string BuildSingleGroupHeaderText(string title, DataGridView grid)
        {
            return string.Format("{0}[{1}]", title, GetVisibleRowCount(grid));
        }

        private bool IsVisibleColumn(DataGridView grid, int columnIndex)
        {
            if (grid == null)
                return false;

            if (columnIndex < 0 || columnIndex >= grid.Columns.Count)
                return false;

            return grid.Columns[columnIndex].Visible;
        }

        private int GetFirstVisibleColumnIndex(DataGridView grid)
        {
            if (grid == null)
                return -1;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (grid.Columns[i].Visible)
                    return i;
            }

            return -1;
        }

        private bool IsLeftMostVisibleColumn(DataGridView grid, int columnIndex)
        {
            return GetFirstVisibleColumnIndex(grid) == columnIndex;
        }

        private Rectangle GetMergedHeaderBounds(
            DataGridView grid,
            int topHeaderHeight,
            Func<DataGridViewColumn, bool> includeColumn)
        {
            if (grid == null || includeColumn == null)
                return Rectangle.Empty;

            int left = -1;
            int right = -1;
            int top = 0;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col == null || !col.Visible)
                    continue;

                if (!includeColumn(col))
                    continue;

                Rectangle rect = grid.GetCellDisplayRectangle(col.Index, -1, true);
                if (rect.Width <= 0)
                    continue;

                if (left < 0)
                {
                    left = rect.Left;
                    top = rect.Top;
                }

                right = rect.Right;
            }

            if (left < 0 || right < 0 || right <= left)
                return Rectangle.Empty;

            return new Rectangle(left, top, right - left, topHeaderHeight);
        }

        private void DrawTwoRowHeaderFrame(
            Graphics graphics,
            Rectangle full,
            Rectangle topRect,
            HeaderVisualStyle style,
            bool drawLeftBorder,
            bool drawTopRightBorder)
        {
            using (var borderPen = new Pen(style.BorderColor, 1))
            using (var strongBorderPen = new Pen(Color.FromArgb(150, 150, 150), 2))
            {
                Rectangle bottomRect = new Rectangle(
                    full.X,
                    topRect.Bottom,
                    full.Width,
                    full.Height - topRect.Height);

                graphics.DrawLine(borderPen, full.Left, full.Top, full.Right - 1, full.Top);
                graphics.DrawLine(borderPen, full.Left, topRect.Bottom - 1, full.Right - 1, topRect.Bottom - 1);
                graphics.DrawLine(borderPen, bottomRect.Right - 1, bottomRect.Top, bottomRect.Right - 1, bottomRect.Bottom - 1);

                if (drawTopRightBorder)
                {
                    graphics.DrawLine(borderPen, topRect.Right - 1, topRect.Top, topRect.Right - 1, topRect.Bottom - 1);
                }

                graphics.DrawLine(strongBorderPen, full.Left, full.Bottom - 1, full.Right - 1, full.Bottom - 1);

                if (drawLeftBorder)
                {
                    graphics.DrawLine(borderPen, full.Left, full.Top, full.Left, full.Bottom - 1);
                }
            }
        }

        private void DrawColumnHeaderText(
            Graphics graphics,
            Rectangle bounds,
            string text,
            HeaderVisualStyle style)
        {
            TextRenderer.DrawText(
                graphics,
                text ?? string.Empty,
                style.ColumnFont,
                bounds,
                style.GroupForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.WordBreak |
                TextFormatFlags.EndEllipsis);
        }

        private void PaintSingleGroupTwoRowHeader(
            DataGridViewCellPaintingEventArgs e,
            DataGridView grid,
            HeaderVisualStyle style,
            string groupHeaderText)
        {
            if (e == null || grid == null || style == null)
                return;

            if (e.RowIndex != -1 || e.ColumnIndex < 0)
                return;

            if (!IsVisibleColumn(grid, e.ColumnIndex))
                return;

            e.Handled = true;

            Rectangle full = e.CellBounds;
            int topHeaderHeight = full.Height / 2;
            int bottomHeaderHeight = full.Height - topHeaderHeight;

            Rectangle topRect = new Rectangle(full.X, full.Y, full.Width, topHeaderHeight);
            Rectangle bottomRect = new Rectangle(full.X, full.Y + topHeaderHeight, full.Width, bottomHeaderHeight);

            using (var topBackBrush = new SolidBrush(style.GroupBackColor))
            using (var bottomBackBrush = new SolidBrush(Color.FromArgb(245, 247, 250)))
            {
                e.Graphics.FillRectangle(topBackBrush, topRect);
                e.Graphics.FillRectangle(bottomBackBrush, bottomRect);

                DrawColumnHeaderText(
                    e.Graphics,
                    bottomRect,
                    Convert.ToString(e.FormattedValue),
                    style);

                DrawTwoRowHeaderFrame(
                    e.Graphics,
                    full,
                    topRect,
                    style,
                    IsLeftMostVisibleColumn(grid, e.ColumnIndex),
                    IsRightMostVisibleColumn(grid, e.ColumnIndex));
            }
        }

        private void RegisterTwoRowHeaderPainter(
            DataGridView grid,
            DataGridViewCellPaintingEventHandler handler)
        {
            if (grid == null || handler == null)
                return;

            grid.CellPainting -= Grid_HeaderCellPainting;
            grid.CellPainting -= handler;
            grid.CellPainting += handler;
        }

        // ▼ 旧2段ヘッダー（中グリッド）
        private bool IsRightMostVisibleColumn(DataGridView grid, int columnIndex)
        {
            if (grid == null)
                return false;

            for (int i = grid.Columns.Count - 1; i >= 0; i--)
            {
                if (!grid.Columns[i].Visible)
                    continue;

                return i == columnIndex;
            }

            return false;
        }

        private bool IsLastVisibleColumnOfLevel(DataGridView grid, int columnIndex, int level)
        {
            if (grid == null)
                return false;

            for (int i = grid.Columns.Count - 1; i >= 0; i--)
            {
                DataGridViewColumn col = grid.Columns[i];
                if (col == null || !col.Visible)
                    continue;

                int? lv = GetMiddleHeaderLevelFromColumnName(col.Name);
                if (!lv.HasValue)
                    continue;

                if (lv.Value == level)
                    return i == columnIndex;
            }

            return false;
        }

        private bool IsFirstVisibleColumnOfLevel(DataGridView grid, int columnIndex, int level)
        {
            if (grid == null)
                return false;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                DataGridViewColumn col = grid.Columns[i];
                if (col == null || !col.Visible)
                    continue;

                int? lv = GetMiddleHeaderLevelFromColumnName(col.Name);
                if (!lv.HasValue)
                    continue;

                if (lv.Value == level)
                    return i == columnIndex;
            }

            return false;
        }

        private string NormalizeMiddleColumnHeaderText(string headerText)
        {
            if (string.IsNullOrWhiteSpace(headerText))
                return headerText;

            int spaceIndex = headerText.IndexOf(' ');
            if (spaceIndex > 0)
                return headerText.Substring(spaceIndex + 1);

            return headerText;
        }

        private void dataGridMiddle_TwoRowHeaderCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            if (e.RowIndex != -1 || e.ColumnIndex < 0)
                return;

            if (!IsVisibleColumn(grid, e.ColumnIndex))
                return;

            e.Handled = true;

            Rectangle full = e.CellBounds;
            int topHeaderHeight = full.Height / 2;
            int bottomHeaderHeight = full.Height - topHeaderHeight;

            Rectangle topRect = new Rectangle(full.X, full.Y, full.Width, topHeaderHeight);
            Rectangle bottomRect = new Rectangle(full.X, full.Y + topHeaderHeight, full.Width, bottomHeaderHeight);

            int? level = GetMiddleHeaderLevelFromColumnName(grid.Columns[e.ColumnIndex].Name);

            using (var topBackBrush = new SolidBrush(_middleHeaderStyle.GroupBackColor))
            using (var bottomBackBrush = new SolidBrush(Color.FromArgb(245, 247, 250)))
            {
                e.Graphics.FillRectangle(topBackBrush, topRect);
                e.Graphics.FillRectangle(bottomBackBrush, bottomRect);

                DrawColumnHeaderText(
                    e.Graphics,
                    bottomRect,
                    NormalizeMiddleColumnHeaderText(Convert.ToString(e.FormattedValue)),
                    _middleHeaderStyle);

                DrawTwoRowHeaderFrame(
                    e.Graphics,
                    full,
                    topRect,
                    _middleHeaderStyle,
                    IsLeftMostVisibleColumn(grid, e.ColumnIndex),
                    level.HasValue && IsLastVisibleColumnOfLevel(grid, e.ColumnIndex, level.Value));
            }
        }

        // ▼ 旧2段ヘッダー上段 Paint
        private void dataGridStart_TwoRowHeaderPaint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            DrawSingleGroupHeaderText(
                e.Graphics,
                grid,
                _startHeaderStyle,
                BuildSingleGroupHeaderText("検索始点", grid));
        }

        private void dataGridEnd_TwoRowHeaderPaint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            DrawSingleGroupHeaderText(
                e.Graphics,
                grid,
                _endHeaderStyle,
                BuildSingleGroupHeaderText("検索終点", grid));
        }

        private void dataGridMiddle_TwoRowHeaderPaint(object sender, PaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            DrawMiddleGroupHeaderTexts(e.Graphics, grid, _middleHeaderStyle);
        }

        private void DrawSingleGroupHeaderText(
            Graphics graphics,
            DataGridView grid,
            HeaderVisualStyle style,
            string text)
        {
            if (graphics == null || grid == null || style == null)
                return;

            int topHeaderHeight = grid.ColumnHeadersHeight / 2;

            Rectangle mergedTopRect = GetMergedHeaderBounds(
                grid,
                topHeaderHeight,
                delegate (DataGridViewColumn col) { return col.Visible; });

            if (mergedTopRect.IsEmpty)
                return;

            TextRenderer.DrawText(
                graphics,
                text ?? string.Empty,
                style.GroupFont,
                mergedTopRect,
                style.GroupForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);
        }

        private void DrawMiddleGroupHeaderTexts(
            Graphics graphics,
            DataGridView grid,
            HeaderVisualStyle style)
        {
            if (graphics == null || grid == null || style == null)
                return;

            int topHeaderHeight = grid.ColumnHeadersHeight / 2;

            for (int level = 1; level <= _currentMaxDepth; level++)
            {
                int currentLevel = level;

                Rectangle mergedTopRect = GetMergedHeaderBounds(
                    grid,
                    topHeaderHeight,
                    delegate (DataGridViewColumn col)
                    {
                        int? lv = GetMiddleHeaderLevelFromColumnName(col.Name);
                        return col.Visible && lv.HasValue && lv.Value == currentLevel;
                    });

                if (mergedTopRect.IsEmpty)
                    continue;

                TextRenderer.DrawText(
                    graphics,
                    GetMiddleGridGroupHeaderText(currentLevel),
                    style.GroupFont,
                    mergedTopRect,
                    style.GroupForeColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis);
            }
        }

        // ▼ 旧2段ヘッダー登録・再描画
        private void RegisterTwoRowHeaderPaintHandlers()
        {
            dataGridStart.Paint -= dataGridStart_TwoRowHeaderPaint;
            dataGridStart.Paint += dataGridStart_TwoRowHeaderPaint;

            dataGridMiddle.Paint -= dataGridMiddle_TwoRowHeaderPaint;
            dataGridMiddle.Paint += dataGridMiddle_TwoRowHeaderPaint;

            dataGridEnd.Paint -= dataGridEnd_TwoRowHeaderPaint;
            dataGridEnd.Paint += dataGridEnd_TwoRowHeaderPaint;
        }

        private void Grid_TwoRowHeaderInvalidateOnScroll(object sender, ScrollEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null)
                return;

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                grid.Invalidate();
            }
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
            dataGridStart.ClearSelection();
            dataGridStart.CurrentCell = dataGridStart.Rows[e.RowIndex].Cells[e.ColumnIndex];
            dataGridStart.Rows[e.RowIndex].Selected = true;

            var drv = dataGridStart.Rows[e.RowIndex].DataBoundItem as DataRowView;
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
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.Button != MouseButtons.Right) return;

            // 右クリックした行を選択（Startと同じ）
            dataGridMiddle.ClearSelection();
            dataGridMiddle.CurrentCell = dataGridMiddle.Rows[e.RowIndex].Cells[e.ColumnIndex];
            dataGridMiddle.Rows[e.RowIndex].Selected = true;

            var drv = dataGridMiddle.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow r = drv.Row;

            // クリック列からLvを推定（Lv3_Order など）
            string colName = dataGridMiddle.Columns[e.ColumnIndex].Name;
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

            // 右クリックした行を選択（Startと同じ）
            dataGridEnd.ClearSelection();
            dataGridEnd.CurrentCell = dataGridEnd.Rows[e.RowIndex].Cells[e.ColumnIndex];
            dataGridEnd.Rows[e.RowIndex].Selected = true;

            var drv = dataGridEnd.Rows[e.RowIndex].DataBoundItem as DataRowView;
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
        private void AlignMiddleGridBottomLineByScrollBar()
        {
            if (dataGridStart == null || dataGridMiddle == null) return;

            int gap = GetBottomGap(dataGridMiddle);

            // Start と同じ高さを基準にして、Middle だけ gap 分増やす
            int targetHeight = dataGridStart.Height + gap;

            if (dataGridMiddle.Height != targetHeight)
            {
                dataGridMiddle.Height = targetHeight;
                dataGridMiddle.Invalidate();
            }
        }

    }
}
