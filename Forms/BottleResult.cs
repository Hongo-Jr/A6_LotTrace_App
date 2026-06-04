using LotTraceApp.Models;
using LotTraceApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotTraceApp.Forms
{
    public partial class BottleResult : Form
    {
        private readonly BottleResultService _service;
        private readonly string _orderNumber;
        private readonly string _lotNo;



        public BottleResult(BottleResultService service, string orderNumber, string lotNo)
        {
            _service = service;
            _orderNumber = orderNumber;
            _lotNo = lotNo;

            InitializeComponent();
            this.Load += BottleResult_Load;
        }


        private void BottleResult_Load(object sender, EventArgs e)
        {
            DataTable dt = _service.GetBottleOrderVerticalAll(_orderNumber, _lotNo);

            BottleDataGridView.DataSource = dt;

            SetBottleResultGrid(dt);

            BottleResultGridStyle();
            BottleGridDesign();
        }
        private void SetBottleResultGrid(DataTable dt)
        {
            BottleDataGridView.AutoGenerateColumns = true;
            BottleDataGridView.DataSource = dt;

            BottleDataGridView.Columns["Item"].Visible = true;
            BottleDataGridView.Columns["Item"].HeaderText = dt.Columns["Item"].Caption;
            BottleDataGridView.Columns["Item"].Width = 220;

            BottleDataGridView.Columns["order"].Visible = true;
            BottleDataGridView.Columns["order"].HeaderText = dt.Columns["order"].Caption;
            BottleDataGridView.Columns["order"].Width = 180;

        }
        private void BottleResultGridStyle()
        {
            var g = BottleDataGridView;
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

        }
        private void BottleGridDesign()
        {
            var grid = BottleDataGridView;
            // ヘッダーのスタイルを個別適用できるようにする（必須）
            grid.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                var t = col.HeaderText ?? col.Name ?? "";

                if (t.StartsWith("項目"))
                {
                    col.HeaderCell.Style.ForeColor = Color.Black;
                }
                else if (t.StartsWith("指図"))
                {
                    col.HeaderCell.Style.BackColor = Color.FromArgb(235, 242, 250); // 薄い青
                    col.HeaderCell.Style.ForeColor = Color.Black;
                    col.DefaultCellStyle.Font = new Font(grid.DefaultCellStyle.Font, FontStyle.Regular);
                }
                else
                {
                    // その他（任意：デフォルトに戻す）
                    col.HeaderCell.Style.BackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
                    col.HeaderCell.Style.ForeColor = grid.ColumnHeadersDefaultCellStyle.ForeColor;
                }
                var c = Color.FromArgb(45, 45, 45);

                // ControlとしてのForeColorも明示（親の継承を受けにくくする）
                grid.ForeColor = c;

                // セル文字色（実データ）
                grid.DefaultCellStyle.ForeColor = c;
                grid.RowsDefaultCellStyle.ForeColor = c;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = c;

                // 選択時も読めるように
                grid.DefaultCellStyle.SelectionForeColor = c;
                

            }
        }
    }
}
