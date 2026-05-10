using System;
using System.IO;
using System.Windows.Forms;
using LotTraceApp.Models;
using LotTraceApp.Services;
using LotTraceApp.Utils;

namespace LotTraceApp
{
    public partial class BottleTraceForm : Form
    {
        private readonly BottleTraceService _service;
        private BottleTraceResult _lastResult;

        public BottleTraceForm(BottleTraceService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();

            dgvStartBottle.AutoGenerateColumns = true;
            dgvStartBottle.ReadOnly = true;
            dgvStartBottle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvEndBottle.AutoGenerateColumns = true;
            dgvEndBottle.ReadOnly = true;
            dgvEndBottle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private TraceSearchParameters CollectSearchParameters()
        {
            return new TraceSearchParameters
            {
                ProductionOrderNumber = txtOrderNumber.Text.Trim(),
                ItemName = txtItemName.Text.Trim(),
                ItemCode = txtItemCode.Text.Trim(),
                LotNumber = txtLotNumber.Text.Trim(),
                From = chkUseFrom.Checked ? dtpFrom.Value.Date : (DateTime?)null,
                To = chkUseTo.Checked ? dtpTo.Value.Date.AddDays(1).AddSeconds(-1) : (DateTime?)null,
                Direction = rdoForwardBottle.Checked ? TraceDirection.Forward : TraceDirection.Backward
            };
        }

        private void btnTraceSearchBottle_Click(object sender, EventArgs e)
        {
            var p = CollectSearchParameters();
            _lastResult = _service.ExecuteTrace(p);

            dgvStartBottle.DataSource = _lastResult.StartOrders;
            dgvEndBottle.DataSource = _lastResult.Fillings;
        }

        private void btnClearBottle_Click(object sender, EventArgs e)
        {
            txtOrderNumber.Clear();
            txtItemName.Clear();
            txtItemCode.Clear();
            txtLotNumber.Clear();
            chkUseFrom.Checked = false;
            chkUseTo.Checked = false;

            dgvStartBottle.DataSource = null;
            dgvEndBottle.DataSource = null;
            _lastResult = null;
        }

        private void btnCsvOutputBottle_Click(object sender, EventArgs e)
        {
            if (_lastResult == null)
            {
                MessageBox.Show("出力する検索結果がありません。", "瓶設備ロットトレース",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // C# 7.3 用: using var → using (var ... ) { ... }
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV ファイル (*.csv)|*.csv";
                dlg.FileName = "BottleTrace.csv";

                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                // 簡易的に、BottleTraceResult を 1 テーブルにマージして CSV 出力
                var table = new System.Data.DataTable();
                table.Columns.Add("Type"); // Start/End
                table.Columns.Add("OrderNumber");
                table.Columns.Add("ProcessType");
                table.Columns.Add("ProductItemName");
                table.Columns.Add("ProductItemCode");
                table.Columns.Add("ProductLotNumber");
                table.Columns.Add("MiddleProductItemCode");
                table.Columns.Add("MiddleProductLotNumber");
                table.Columns.Add("BottleIdOrDrum");
                table.Columns.Add("FillingType");
                table.Columns.Add("FillingMachineNo", typeof(int));
                table.Columns.Add("FillingNozzleNo", typeof(int));
                table.Columns.Add("FillingWeight", typeof(long));
                table.Columns.Add("StartDate", typeof(DateTime));
                table.Columns.Add("EndDate", typeof(DateTime));

                // 始点
                foreach (var o in _lastResult.StartOrders)
                {
                    table.Rows.Add(
                        "Start",
                        o.OrderNumber, o.ProcessType, o.ProductItemName,
                        o.ProductItemCode, o.ProductLotNumber,
                        o.MiddleProductItemCode, o.MiddleProductLotNumber,
                        null, null, null, null, null,
                        o.StartDate, o.EndDate);
                }

                // 終点
                foreach (var f in _lastResult.Fillings)
                {
                    table.Rows.Add(
                        "End",
                        f.OrderNumber, f.ProcessType, null,
                        f.ProductItemCode, f.ProductLotNumber,
                        f.MiddleProductItemCode, f.MiddleProductLotNumber,
                        f.BottleIdOrDrumcanNumber, f.FillingType,
                        f.FillingMachineNumber, f.FillingNozzleNumber,
                        f.FillingWeight, f.FillingStartDate, f.FillingEndDate);
                }

                ExportHelper.ExportDataTableToCsv(table, dlg.FileName);
            }

            MessageBox.Show("CSV 出力が完了しました。", "瓶設備ロットトレース",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// [37] 液設備ボタン クリック時：液設備画面へ戻る
        /// </summary>
        private void btnBackToLiquid_Click(object sender, EventArgs e)
        {
            // MainForm から ShowDialog(this) などで開かれている前提なら、
            // このフォームを閉じるだけで液設備画面にフォーカスが戻ります。
            this.Close();
        }
    }
}