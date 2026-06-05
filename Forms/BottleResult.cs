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
        private readonly BottleNodeInfo _node;

        private readonly int _pageNo;
        private readonly int _pageSize = 100;



        public BottleResult(BottleResultService service, BottleNodeInfo node)
        {
            if (service == null) throw new ArgumentNullException("bottleResultService");
            _service = service;

            _node = node;
            _pageNo = 1;

            InitializeComponent();
            InitialFrom();
            RegisterEvents();

            SetGridContets();
        }

        private void InitialFrom()
        {
            InitialDispLabel();
            InitialRadio();
            InitialGrid();
        }

        private void InitialDispLabel()
        {
            lbl_Disp_Pro_OrederNo.Text = _node.OrderNumber;
            lbl_Disp_Pro_LotNo.Text = _node.ProductLotNo;
            lbl_Disp_Pro_Code.Text = _node.ProductItemCode;
            lbl_Disp_Pro_Name.Text = _node.ProductItemName;
        }
        
        private void InitialRadio()
        {
            rdo_Order.Checked = true;
            rdo_Result.Checked = false;
        }

        private void InitialGrid()
        {
            BottleResultGridStyle();
        }

        private void RegisterEvents()
        {
            rdo_Order.CheckedChanged += Radio_CheckedChanged;
            rdo_Result.CheckedChanged += Radio_CheckedChanged;
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;

            SetGridContets();
        }

        private void SetGridContets()
        {
            if (rdo_Order.Checked)
            {
                ClearBottleGrid();
                SetBottleOrderGrid();
            }
            if (rdo_Result.Checked)
            {
                ClearBottleGrid();
                SetBottelResultGrid();
            }
        }


        private void ClearBottleGrid()
        {
            var grid = BottleDataGridView;

            grid.DataSource = null;
            grid.Columns.Clear();
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle();

        }

        private void SetBottleOrderGrid()
        {
            SetFromOrderMode();

            DataTable dt = _service.GetBottleOrderVerticalAll(_node.OrderNumber, _node.ProductLotNo);
            BottleDataGridView.DataSource = dt;
            SetBottleOrderGrid(dt);
            BottleOrderGridDesign();
        }

        private void SetFromOrderMode()
        {
            lbl_DispNum.Visible = false;
            btn_PageNext.Visible = false;
            btn_PagePrev.Visible = false;
            lbl_PageNum.Visible = false;
            btn_RowsSetting.Visible = false;
        }

        private void SetBottleOrderGrid(DataTable dt)
        {
            BottleDataGridView.Width = 601;
            BottleDataGridView.AutoGenerateColumns = true;
            BottleDataGridView.DataSource = dt;

            BottleDataGridView.Columns["Item"].Visible = true;
            BottleDataGridView.Columns["Item"].HeaderText = dt.Columns["Item"].Caption;
            BottleDataGridView.Columns["Item"].Width = 300;

            BottleDataGridView.Columns["order"].Visible = true;
            BottleDataGridView.Columns["order"].HeaderText = dt.Columns["order"].Caption;
            BottleDataGridView.Columns["order"].Width = 300;

        }
        private void BottleResultGridStyle()
        {
            var g = BottleDataGridView;
            // 全体
            
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
        private void BottleOrderGridDesign()
        {
            var grid = BottleDataGridView;

            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 32;
            grid.RowTemplate.Height = 32;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                var t = col.HeaderText ?? col.Name ?? "";

                if (t.StartsWith("項目"))
                {
                    col.HeaderCell.Style.ForeColor = Color.Black;
                    col.HeaderCell.Style.Font = new Font(grid.Font.FontFamily, 11F, FontStyle.Bold);
                    col.DefaultCellStyle.Font = new Font(grid.Font.FontFamily, 11F, FontStyle.Bold);
                }
                else if (t.StartsWith("指図"))
                {
                    col.HeaderCell.Style.BackColor = Color.FromArgb(235, 242, 250); // 薄い青
                    col.HeaderCell.Style.ForeColor = Color.Black;
                    col.HeaderCell.Style.Font = new Font(grid.Font.FontFamily, 11F, FontStyle.Bold);
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

        private void SetBottelResultGrid()
        {
            SetFromResultMode();


            DataTable dt = _service.GetFillingResult(_node.OrderNumber, _node.ProductLotNo, _pageNo, _pageSize);

            SetBottleResultGrid(dt);
            SetBottleResultVisibility();
            SetBottleResultDesign();
        }

        private void SetFromResultMode()
        {
            lbl_DispNum.Visible = true;
            btn_PageNext.Visible = true;
            btn_PagePrev.Visible = true;
            lbl_PageNum.Visible = true;
            btn_RowsSetting.Visible = true;

            int displayCount = 0;
            lbl_DispNum.Text = $"表示件数：{displayCount}件";

            int pageNo = 0;
            int maxPage = 0;
            lbl_PageNum.Text = $"{pageNo} / {maxPage}";

           

        }

        private void SetBottleResultVisibility()
        {
            BottleDataGridView.Columns["OrderNumber"].Visible = true;
            BottleDataGridView.Columns["ProductLotNumber"].Visible = true;
            BottleDataGridView.Columns["ProductItemCode"].Visible = true;
            BottleDataGridView.Columns["MiddleProductLotNumber"].Visible = true;
            BottleDataGridView.Columns["MiddleProductItemCode"].Visible = true;
            BottleDataGridView.Columns["BottleID"].Visible = true;
            BottleDataGridView.Columns["SamplingGroup"].Visible = true;
            BottleDataGridView.Columns["BottleINumber"].Visible = true;
            BottleDataGridView.Columns["FillingNozzleNumber"].Visible = true;
            BottleDataGridView.Columns["CapTighteningTorqueValue"].Visible = true;
            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].Visible = true;
            BottleDataGridView.Columns["CapTiltDetectionJudgment"].Visible = true;
            BottleDataGridView.Columns["FillingMachineNumber"].Visible = true;
            BottleDataGridView.Columns["TotalCahckJudgment"].Visible = true;
            BottleDataGridView.Columns["BottleLocation"].Visible = true;
            BottleDataGridView.Columns["FillingWeight"].Visible = true;
            BottleDataGridView.Columns["FillingTime"].Visible = true;
            BottleDataGridView.Columns["FillingStartDate"].Visible = true;
            BottleDataGridView.Columns["FillingEndDate"].Visible = true;
        }

        private void SetBottleResultGrid(DataTable dt)
        {
            int ColumnSize1 = 150;
            int ColumnSize2 = 100;
            int ColumnSize3 = 200;

            
            BottleDataGridView.AutoGenerateColumns = true;
            BottleDataGridView.DataSource = dt;

            

            
            BottleDataGridView.Columns["OrderNumber"].HeaderText = dt.Columns["OrderNumber"].Caption;
            BottleDataGridView.Columns["OrderNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProductLotNumber"].HeaderText = dt.Columns["ProductLotNumber"].Caption;
            BottleDataGridView.Columns["ProductLotNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProductItemCode"].HeaderText = dt.Columns["ProductItemCode"].Caption;
            BottleDataGridView.Columns["ProductItemCode"].Width = ColumnSize1;

            BottleDataGridView.Columns["MiddleProductLotNumber"].HeaderText = dt.Columns["MiddleProductLotNumber"].Caption;
            BottleDataGridView.Columns["MiddleProductLotNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["MiddleProductItemCode"].HeaderText = dt.Columns["MiddleProductItemCode"].Caption;
            BottleDataGridView.Columns["MiddleProductItemCode"].Width = ColumnSize1;

            BottleDataGridView.Columns["BottleID"].HeaderText = dt.Columns["BottleID"].Caption;
            BottleDataGridView.Columns["BottleID"].Width = ColumnSize2;

            BottleDataGridView.Columns["SamplingGroup"].HeaderText = dt.Columns["SamplingGroup"].Caption;
            BottleDataGridView.Columns["SamplingGroup"].Width = ColumnSize2;

            BottleDataGridView.Columns["BottleINumber"].HeaderText = dt.Columns["BottleINumber"].Caption;
            BottleDataGridView.Columns["BottleINumber"].Width = ColumnSize2;

            BottleDataGridView.Columns["FillingNozzleNumber"].HeaderText = dt.Columns["FillingNozzleNumber"].Caption;
            BottleDataGridView.Columns["FillingNozzleNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["CapTighteningTorqueValue"].HeaderText = dt.Columns["CapTighteningTorqueValue"].Caption;
            BottleDataGridView.Columns["CapTighteningTorqueValue"].Width = ColumnSize3;

            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].HeaderText = dt.Columns["CapTighteningTorqueJudgment"].Caption;
            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].Width = ColumnSize3;

            BottleDataGridView.Columns["CapTiltDetectionJudgment"].HeaderText = dt.Columns["CapTiltDetectionJudgment"].Caption;
            BottleDataGridView.Columns["CapTiltDetectionJudgment"].Width = ColumnSize3;

            BottleDataGridView.Columns["FillingMachineNumber"].HeaderText = dt.Columns["FillingMachineNumber"].Caption;
            BottleDataGridView.Columns["FillingMachineNumber"].Width = ColumnSize2;

            BottleDataGridView.Columns["TotalCahckJudgment"].HeaderText = dt.Columns["TotalCahckJudgment"].Caption;
            BottleDataGridView.Columns["TotalCahckJudgment"].Width = ColumnSize2;
         
            BottleDataGridView.Columns["BottleLocation"].HeaderText = dt.Columns["BottleLocation"].Caption;
            BottleDataGridView.Columns["BottleLocation"].Width = ColumnSize1;
            
            BottleDataGridView.Columns["FillingWeight"].HeaderText = dt.Columns["FillingWeight"].Caption;
            BottleDataGridView.Columns["FillingWeight"].Width = ColumnSize2;
            
            BottleDataGridView.Columns["FillingTime"].HeaderText = dt.Columns["FillingTime"].Caption;
            BottleDataGridView.Columns["FillingTime"].Width = ColumnSize2;

            BottleDataGridView.Columns["FillingStartDate"].HeaderText = dt.Columns["FillingStartDate"].Caption;
            BottleDataGridView.Columns["FillingStartDate"].Width = ColumnSize1;
            
            BottleDataGridView.Columns["FillingEndDate"].HeaderText = dt.Columns["FillingEndDate"].Caption;
            BottleDataGridView.Columns["FillingEndDate"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProcessType"].Visible = false;
            BottleDataGridView.Columns["ProcessType"].HeaderText = dt.Columns["ProcessType"].Caption;
            BottleDataGridView.Columns["ProcessType"].Width = 0;

        }

        private void SetBottleResultDesign()
        {
            var grid = BottleDataGridView;

            grid.Width = 860;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black; 
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 242, 250);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.Font.FontFamily,11F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 32;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 0, 0, 0);


            grid.DefaultCellStyle.Font = new Font(grid.Font.FontFamily, 9F, FontStyle.Regular);
            grid.RowTemplate.Height = 28;
        }
    }
}
