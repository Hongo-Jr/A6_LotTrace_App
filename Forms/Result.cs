using LotTraceApp.Services;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LotTraceApp.Forms
{
    public partial class Result : Form
    {
        private readonly ResultService _service;

        private readonly string _productionOrderNumber;
        private readonly string _itemCode;
        private readonly string _itemName;
        private readonly string _lotNumber;
        private readonly string _initialProcessId;
        private System.Drawing.Font _dateSmallFont;

        private bool _loading;

        public Result(
            ResultService resultService,
            string productionOrderNumber,
            string itemCode,
            string itemName,
            string lotNumber,
            string processId)
        {
            InitializeComponent();

            ResultView.RowPrePaint += ResultView_RowPrePaint;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            FixResultViewForeColor();

            float newSize = Math.Max(7.0f, ResultView.Font.Size - 1.0f);
            _dateSmallFont = new System.Drawing.Font(ResultView.Font.FontFamily, newSize, System.Drawing.FontStyle.Regular);

            ResultView.CellFormatting += ResultView_CellFormatting;
            ResultView.DataBindingComplete += ResultView_DataBindingComplete;

            rdoBoth.CheckedChanged += (s, e) =>
            {
                if (rdoBoth.Checked) ApplyOrderActualColumnVisibility(ViewMode.Both);
            };

            rdoOrderOnly.CheckedChanged += (s, e) =>
            {
                if (rdoOrderOnly.Checked) ApplyOrderActualColumnVisibility(ViewMode.OrderOnly);
            };

            rdoActualOnly.CheckedChanged += (s, e) =>
            {
                if (rdoActualOnly.Checked) ApplyOrderActualColumnVisibility(ViewMode.ActualOnly);
            };

            _service = resultService ?? throw new ArgumentNullException(nameof(resultService));
            _productionOrderNumber = productionOrderNumber;
            _itemCode = itemCode;
            _itemName = itemName;
            _lotNumber = lotNumber;
            _initialProcessId = processId;

            Load += Result_Load;
            cmbTarget.SelectedValueChanged += cmbTarget_SelectedValueChanged;
        }
        private void FixResultViewForeColor()
        {
            var c = Color.FromArgb(45, 45, 45);

            // ControlとしてのForeColorも明示（親の継承を受けにくくする）
            ResultView.ForeColor = c;

            // セル文字色（実データ）
            ResultView.DefaultCellStyle.ForeColor = c;
            ResultView.RowsDefaultCellStyle.ForeColor = c;
            ResultView.AlternatingRowsDefaultCellStyle.ForeColor = c;

            // 選択時も読めるように
            ResultView.DefaultCellStyle.SelectionForeColor = c;

            // ヘッダー文字色
            ResultView.EnableHeadersVisualStyles = false;
            ResultView.ColumnHeadersDefaultCellStyle.ForeColor = c;
        }
        private void ResultView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!ResultView.Columns.Contains("項目")) return;

            var row = ResultView.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            // すでに判定済みなら再処理しない（負荷対策）
            if (row.Tag is bool) return;

            // 「項目」名（必要ならここで除外条件を入れられる）
            string itemName = Convert.ToString(row.Cells["項目"].Value) ?? "";

            int numericCount = 0;
            int sampleCount = 0;

            // 指図/実績列のうち、値が入っているものをサンプルして数値っぽさを判定
            for (int i = 0; i < ResultView.Columns.Count; i++)
            {
                var col = ResultView.Columns[i];
                if (col.Name == "項目") continue;

                var header = col.HeaderText ?? col.Name ?? "";
                if (!(header.StartsWith("指図") || header.StartsWith("実績"))) continue;

                string s = Convert.ToString(row.Cells[i].Value);
                if (string.IsNullOrWhiteSpace(s)) continue;

                sampleCount++;

                // 数値判定（小数もOK）
                if (decimal.TryParse(s, out _))
                    numericCount++;

                // サンプルは最大6個くらいで十分
                if (sampleCount >= 6) break;
            }

            // サンプルが無い行は何もしない
            if (sampleCount == 0)
            {
                row.Tag = true;
                return;
            }

            bool isNumericRow = (numericCount >= Math.Max(1, sampleCount * 2 / 3)); // 2/3以上数値なら数値行扱い

            var align = isNumericRow
            ? DataGridViewContentAlignment.MiddleRight
            : DataGridViewContentAlignment.MiddleLeft;

            var padLeft = isNumericRow
            ? new Padding(0, 0, 0, 0)
            : new Padding(6, 0, 0, 0);

            // 行の「値セル」だけ整列を変更（項目列はそのまま）
            for (int i = 0; i < ResultView.Columns.Count; i++)
            {
                var col = ResultView.Columns[i];
                if (col.Name == "項目") continue;

                var header = col.HeaderText ?? col.Name ?? "";
                if (!(header.StartsWith("指図") || header.StartsWith("実績"))) continue;

                row.Cells[i].Style.Alignment = align;
                row.Cells[i].Style.Padding = padLeft;
            }

            row.Tag = true; // 判定済みマーク
        }
        private void Result_Load(object sender, EventArgs e)
        {
            _loading = true;

            OrederNo1.Text = _productionOrderNumber;
            ProcCode1.Text = _itemCode;
            ProcName1.Text = _itemName;
            LotNo1.Text = _lotNumber;

            // 履歴込みドロップダウン（＋フィルター）
            var dropdown = _service.BuildProcessDropdownTable(
                _productionOrderNumber,
                _itemCode,
                _lotNumber);

            cmbTarget.DisplayMember = "Name";
            cmbTarget.ValueMember = "Id";
            cmbTarget.DataSource = dropdown;

            // ★ここに変更
            ApplyInitialProcessSelection();

            _loading = false;

            RefreshGrid();
        }

        private void SelectFirstValidProcess()
        {
            for (int i = 0; i < cmbTarget.Items.Count; i++)
            {
                var drv = cmbTarget.Items[i] as DataRowView;
                if (drv == null) continue;

                string id = Convert.ToString(drv.Row["Id"]);
                if (string.IsNullOrWhiteSpace(id)) continue;

                // 区切り・フィルターは飛ばして「制御工程」を初期選択にする
                if (string.Equals(id, ResultService.FilterId, StringComparison.OrdinalIgnoreCase)) continue;

                cmbTarget.SelectedIndex = i;
                return;
            }

            if (cmbTarget.Items.Count > 0)
                cmbTarget.SelectedIndex = 0;
        }

        private void cmbTarget_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            string id = Convert.ToString(cmbTarget.SelectedValue);
            if (string.IsNullOrWhiteSpace(id)) return;

            // フィルター以外は履歴に記録
            if (!string.Equals(id, ResultService.FilterId, StringComparison.OrdinalIgnoreCase))
            {
                _service.RememberFilterHistory(_productionOrderNumber, _itemCode, _lotNumber, id);
            }

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            string id = Convert.ToString(cmbTarget.SelectedValue);
            if (string.IsNullOrWhiteSpace(id)) return;


            DataTable view;

            if (string.Equals(id, ResultService.FilterId, StringComparison.OrdinalIgnoreCase))
            {
                // ★ FilterTable表示
                view = _service.GetFilterView(_productionOrderNumber, _itemCode, _lotNumber);

                ResultView.DataSource = null;
                ResultView.AutoGenerateColumns = true;
                ResultView.RowHeadersVisible = false;
                ResultView.DataSource = view;

                // FilterTableは列変換CSVが別物の可能性が高いので適用しない（必要なら別途対応）
                return;
            }

            // ★ 制御工程表示
            view = _service.GetProcessView(_productionOrderNumber, _itemCode, _lotNumber, id);

            ResultView.DataSource = null;
            ResultView.AutoGenerateColumns = true;
            ResultView.RowHeadersVisible = false;
            ResultView.DataSource = view;

            // DataSource 設定後（列が生成された後）
            var itemCol = ResultView.Columns.Contains("項目") ? ResultView.Columns["項目"] : null;
            if (itemCol != null)
            {
                // ヘッダーStyleを確実に反映させる（テーマ優先を無効化）
                ResultView.EnableHeadersVisualStyles = false;

                // 項目ヘッダー：左寄せ＋余白（項目名セル側と位置を合わせる）
                itemCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                itemCol.HeaderCell.Style.Padding = new Padding(8, 0, 6, 0);
            }

            _service.ApplyDisplayNamesToGrid(ResultView); // 先にヘッダ文字を確定
            ApplyResultGridStyle();                       // その後に見た目適用
            ApplyOrderActualColumnVisibility(GetViewMode());

        }

        private void ApplyResultGridStyle()
        {
            var g = ResultView;
            // 全体
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;

            // 罫線（薄く）
            g.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            g.GridColor = Color.FromArgb(230, 230, 230);

            // 行の見やすさ（薄い交互色）
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            // 選択色（濃すぎない）
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            g.DefaultCellStyle.SelectionForeColor = g.DefaultCellStyle.ForeColor;

            // ヘッダー（添付のような薄いグレー帯）
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.ColumnHeadersDefaultCellStyle.Font = new Font(g.Font, FontStyle.Bold);
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 28;

            // 並び替え無効（要望）
            foreach (DataGridViewColumn col in g.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            // 「項目」列：固定＋内容に合わせて広げる（要望）
            if (g.Columns.Contains("項目"))
            {
                var itemCol = g.Columns["項目"];
                itemCol.Frozen = true;
                itemCol.MinimumWidth = 180;

                itemCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                // 左余白（項目名が読みやすくなる）
                itemCol.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
                itemCol.DefaultCellStyle.Font = new Font(ResultView.Font, FontStyle.Bold);
            }

            foreach (DataGridViewColumn col in g.Columns)
            {
                var t = col.HeaderText ?? col.Name ?? "";
                if (t.StartsWith("指図"))
                {
                    col.HeaderCell.Style.BackColor = Color.FromArgb(235, 242, 250); // 薄い青
                    col.HeaderCell.Style.ForeColor = Color.Black;
                }
                else if (t.StartsWith("実績"))
                {
                    col.HeaderCell.Style.BackColor = Color.FromArgb(250, 238, 238); // 薄い赤
                    col.HeaderCell.Style.ForeColor = Color.Black;
                }
            }
            // 横スクロール前提
            g.ScrollBars = ScrollBars.Both;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            g.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // 行高さを少し詰める（例：24〜26の間で調整）
            ResultView.RowTemplate.Height = 25;
            ResultView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }
        private void AdjustItemColumnWidth_NoAuto()
        {
            if (!ResultView.Columns.Contains("項目")) return;

            var itemCol = ResultView.Columns["項目"];

            // Autoは使わない
            itemCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ResultView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // 横スクロールしても項目名が見えるように（任意だけどおすすめ）
            itemCol.Frozen = true;

            // 余白（項目列セルに設定しているPaddingと合わせるのが理想）
            int padLR = itemCol.DefaultCellStyle.Padding.Left + itemCol.DefaultCellStyle.Padding.Right;
            int safety = 18; // DPI/罫線ズレ保険（まだ見切れるなら 24/32）

            int max = 0;

            using (var g = ResultView.CreateGraphics())
            {
                // ヘッダー「項目」も考慮
                var headerFont = ResultView.ColumnHeadersDefaultCellStyle.Font ?? ResultView.Font;
                var hsz = TextRenderer.MeasureText(
                    g, itemCol.HeaderText ?? "項目", headerFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine | TextFormatFlags.NoClipping | TextFormatFlags.LeftAndRightPadding);
                max = Math.Max(max, hsz.Width);

                // 各行の項目名を計測
                foreach (DataGridViewRow r in ResultView.Rows)
                {
                    if (r.IsNewRow) continue;

                    string text = Convert.ToString(r.Cells[itemCol.Index].Value) ?? "";
                    if (text.Length == 0) continue;

                    var sz = TextRenderer.MeasureText(
                        g, text, itemCol.DefaultCellStyle.Font ?? ResultView.Font,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine | TextFormatFlags.NoClipping | TextFormatFlags.LeftAndRightPadding);

                    max = Math.Max(max, sz.Width);
                }
            }

            int target = max + padLR + safety;

            // 最低幅（好み）
            if (target < 180) target = 180;

            itemCol.Width = target;
            itemCol.MinimumWidth = target;
        }


        private void ResultView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (!ResultView.Columns.Contains("項目")) return;

            string itemName = Convert.ToString(ResultView.Rows[e.RowIndex].Cells["項目"].Value);

            if (itemName == "開始日時" || itemName == "終了日時")
            {
                if (ResultView.Columns[e.ColumnIndex].Name != "項目")
                {
                    e.CellStyle.Font = _dateSmallFont;
                    e.CellStyle.Padding = new Padding(0, 0, 0, 0); // 任意：詰める
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    e.CellStyle.WrapMode = DataGridViewTriState.False;
                }
            }
        }
        private void ResultView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                AdjustItemColumnWidth_NoAuto();                 // ★追加（項目列）
                AdjustActualColumnsWidth_ByStartEnd_SmallFont_NoAuto(); // 既存（実績列）
            }));
        }
        private void AdjustActualColumnsWidth_ByStartEnd_SmallFont_NoAuto()
        {
            if (GetViewMode() == ViewMode.OrderOnly) return;

            if (!ResultView.Columns.Contains("項目")) return;

            // 開始/終了行を探す（無ければ幅調整しない）
            int startRow = -1, endRow = -1;
            foreach (DataGridViewRow r in ResultView.Rows)
            {
                if (r.IsNewRow) continue;
                var name = Convert.ToString(r.Cells["項目"].Value);
                if (name == "開始日時") startRow = r.Index;
                else if (name == "終了日時") endRow = r.Index;
                if (startRow >= 0 && endRow >= 0) break;
            }
            if (startRow < 0 && endRow < 0) return;

            var dtFont = _dateSmallFont ?? ResultView.Font;
            var headerFont = ResultView.ColumnHeadersDefaultCellStyle.Font ?? ResultView.Font;

            // ★AutoSizeは使わない（手動Widthを効かせる）
            ResultView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            foreach (DataGridViewColumn c in ResultView.Columns)
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            const int safety = 16; // DPI差で切れるなら 24/32 に上げる

            using (var g = ResultView.CreateGraphics())
            {
                foreach (DataGridViewColumn col in ResultView.Columns)
                {
                    if (!col.Visible) continue;
                    if (col.Name == "項目") continue;

                    // ★実績列だけ（指図は触らない）
                    var key = col.DataPropertyName ?? col.Name ?? col.HeaderText ?? "";
                    if (!key.StartsWith("実績")) continue;

                    string s1 = startRow >= 0 ? Convert.ToString(ResultView.Rows[startRow].Cells[col.Index].Value) ?? "" : "";
                    string s2 = endRow >= 0 ? Convert.ToString(ResultView.Rows[endRow].Cells[col.Index].Value) ?? "" : "";

                    // ★この実績列に開始/終了が無いなら幅は変えない（無駄なスペース防止）
                    if (string.IsNullOrWhiteSpace(s1) && string.IsNullOrWhiteSpace(s2))
                        continue;

                    int need = 0;

                    if (!string.IsNullOrWhiteSpace(s1))
                    {
                        var sz = TextRenderer.MeasureText(g, s1, dtFont, new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.SingleLine | TextFormatFlags.NoClipping | TextFormatFlags.LeftAndRightPadding);
                        need = Math.Max(need, sz.Width);
                    }

                    if (!string.IsNullOrWhiteSpace(s2))
                    {
                        var sz = TextRenderer.MeasureText(g, s2, dtFont, new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.SingleLine | TextFormatFlags.NoClipping | TextFormatFlags.LeftAndRightPadding);
                        need = Math.Max(need, sz.Width);
                    }

                    // ヘッダーも最低限考慮（実績10等）
                    var hsz = TextRenderer.MeasureText(g, col.HeaderText ?? "", headerFont, new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine | TextFormatFlags.NoClipping | TextFormatFlags.LeftAndRightPadding);
                    need = Math.Max(need, hsz.Width);

                    int targetWidth = need + safety;

                    // 狭い場合だけ広げる（他行が長い列は維持されやすい）
                    if (col.Width < targetWidth)
                    {
                        col.Width = targetWidth;
                        col.MinimumWidth = targetWidth;
                    }
                }
            }
        }

        private enum ViewMode { Both, OrderOnly, ActualOnly }

        private ViewMode GetViewMode()
        {
            if (rdoOrderOnly.Checked) return ViewMode.OrderOnly;
            if (rdoActualOnly.Checked) return ViewMode.ActualOnly;
            return ViewMode.Both;
        }
        private void ApplyOrderActualColumnVisibility(ViewMode mode)
        {
            foreach (DataGridViewColumn col in ResultView.Columns)
            {
                if (!col.Visible)

                if (col.Name == "項目") continue;

                string key = col.DataPropertyName ?? col.Name ?? col.HeaderText ?? "";

                bool isOrder = key.StartsWith("指図", StringComparison.OrdinalIgnoreCase);
                bool isActual = key.StartsWith("実績", StringComparison.OrdinalIgnoreCase);

                if (!isOrder && !isActual) continue;

                col.Visible =
                    mode == ViewMode.Both ||
                    (mode == ViewMode.OrderOnly && isOrder) ||
                    (mode == ViewMode.ActualOnly && isActual);
            }
        }
        private void ApplyInitialProcessSelection()
        {
            // 1) まず「工程ID（"1"等）」として直接一致するならそれを使う
            if (TrySelectProcessById(_initialProcessId))
                return;

            if (!string.IsNullOrWhiteSpace(_initialProcessId))
            {
                string resolvedId = _service.ResolveProcessIdByMasterKey(
                    _productionOrderNumber, _itemCode, _lotNumber, _initialProcessId);

                if (!string.IsNullOrWhiteSpace(resolvedId) && TrySelectProcessById(resolvedId))
                    return;
            }

            SelectFirstValidProcess();
        }

        private bool TrySelectProcessById(string processId)
        {
            if (string.IsNullOrWhiteSpace(processId))
                return false;

            for (int i = 0; i < cmbTarget.Items.Count; i++)
            {
                var drv = cmbTarget.Items[i] as DataRowView;
                if (drv == null) continue;

                string id = Convert.ToString(drv.Row["Id"]);
                if (string.IsNullOrWhiteSpace(id)) continue;

                // フィルタ行は除外
                if (string.Equals(id, ResultService.FilterId, StringComparison.OrdinalIgnoreCase)) continue;

                if (string.Equals(id, processId, StringComparison.OrdinalIgnoreCase))
                {
                    cmbTarget.SelectedIndex = i;
                    return true;
                }
            }

            return false;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}