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

        private  int _pageNo;
        private readonly int _pageSize = 100;

        private   List<BottleRowSettings> _bottleRowSettings = new List<BottleRowSettings>();
        private List<BottleRowSettings> _drumRowSettings = new List<BottleRowSettings>();

        private  FillingResultTable _pages = new FillingResultTable();


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
            InitialRowSet();
        }

        private void InitialRowSet()
        {
            //Bottle

            int bottleColumns = 19;
            int drumColumns = 18;

            for (int i = 0; i < bottleColumns; i++)
            {
                var set = new BottleRowSettings();
                set.SetNo = i + 1;
                set.Visility = true;
                set.Index = i + 1;

                _bottleRowSettings.Add(set);
            }

            for (int i = 0; i < drumColumns; i++)
            {
                var set = new BottleRowSettings();
                set.SetNo = i + 1;
                set.Visility = true;
                set.Index = i + 1;

                _drumRowSettings.Add(set);
            }



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

        private void Radio_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is not RadioButton { Checked: true } radioButton)
            {
                return;
            }

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
            BottleDataGridView.Columns["Item"].HeaderText = dt.Columns["Item"]!.Caption;
            BottleDataGridView.Columns["Item"].Width = 300;

            BottleDataGridView.Columns["order"].Visible = true;
            BottleDataGridView.Columns["order"].HeaderText = dt.Columns["order"]!.Caption;
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
            RefreshResultView();
        }
        private void RefreshResultView()
        {
            ClearBottleGrid();

            _pages.Clear();
            _pages = _service.GetFillingResultPage(_node.OrderNumber, _node.ProductLotNo, _pageNo, _pageSize);

            lbl_DispNum.Text = $"総件数：{_pages.TotalCount}件";
            lbl_PageNum.Text = $"{_pageNo} / {_pages.MaxPageNo}";

            btn_PagePrev.Enabled = (_pageNo > 1);
            btn_PageNext.Enabled = (_pageNo < _pages.MaxPageNo);

            if (_pages.BottleTable.Rows.Count != 0)
            {
                SetBottleResultGrid(_pages.BottleTable);
                SetBottleResultVisibility();
                SetBottleResultDesign();
                return;
            }

            else if(_pages.DrumTable.Rows.Count != 0)
            {
                SetDrumResultGrid(_pages.DrumTable);
                SetDrumResultVisibility();
                SetBottleResultDesign();
                return;
            }
            else
            {
                //何もしない

                //BottleDataGridView.DataSource = null;
                //BottleDataGridView.Columns.Clear();
            }

            SetBottleResultDesign();
        }

        private void SetFromResultMode()
        {
            lbl_DispNum.Visible = true;
            btn_PageNext.Visible = true;
            btn_PagePrev.Visible = true;
            lbl_PageNum.Visible = true;
            btn_RowsSetting.Visible = true;
        }

        private void SetBottleResultVisibility()
        {
            BottleDataGridView.Columns["OrderNumber"].Visible = _bottleRowSettings[0].Visility;
            BottleDataGridView.Columns["ProductLotNumber"].Visible = _bottleRowSettings[1].Visility;
            BottleDataGridView.Columns["ProductItemCode"].Visible = _bottleRowSettings[2].Visility;
            BottleDataGridView.Columns["MiddleProductLotNumber"].Visible = _bottleRowSettings[3].Visility;
            BottleDataGridView.Columns["MiddleProductItemCode"].Visible = _bottleRowSettings[4].Visility;
            BottleDataGridView.Columns["BottleID"].Visible = _bottleRowSettings[5].Visility;
            BottleDataGridView.Columns["SamplingGroup"].Visible = _bottleRowSettings[6].Visility;
            BottleDataGridView.Columns["BottleINumber"].Visible = _bottleRowSettings[7].Visility;
            BottleDataGridView.Columns["FillingNozzleNumber"].Visible = _bottleRowSettings[8].Visility;
            BottleDataGridView.Columns["CapTighteningTorqueValue"].Visible = _bottleRowSettings[9].Visility;
            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].Visible = _bottleRowSettings[10].Visility;
            BottleDataGridView.Columns["CapTiltDetectionJudgment"].Visible = _bottleRowSettings[11].Visility;
            BottleDataGridView.Columns["FillingMachineNumber"].Visible = _bottleRowSettings[12].Visility;
            BottleDataGridView.Columns["TotalCahckJudgment"].Visible = _bottleRowSettings[13].Visility;
            BottleDataGridView.Columns["BottleLocation"].Visible = _bottleRowSettings[14].Visility;
            BottleDataGridView.Columns["FillingWeight"].Visible = _bottleRowSettings[15].Visility;
            BottleDataGridView.Columns["FillingTime"].Visible = _bottleRowSettings[16].Visility;
            BottleDataGridView.Columns["FillingStartDate"].Visible = _bottleRowSettings[17].Visility;
            BottleDataGridView.Columns["FillingEndDate"].Visible = _bottleRowSettings[18].Visility;
        }
        private void SetDrumResultVisibility()
        {
            BottleDataGridView.Columns["OrderNumber"].Visible = _drumRowSettings[0].Visility;
            BottleDataGridView.Columns["ProductLotNumber"].Visible = _drumRowSettings[1].Visility;
            BottleDataGridView.Columns["ProductItemCode"].Visible = _drumRowSettings[2].Visility;
            BottleDataGridView.Columns["MiddleProductLotNumber"].Visible = _drumRowSettings[3].Visility;
            BottleDataGridView.Columns["MiddleProductItemCode"].Visible = _drumRowSettings[4].Visility;
            BottleDataGridView.Columns["DrumcanNumber"].Visible = _drumRowSettings[5].Visility;
            BottleDataGridView.Columns["FillingNozzleNumber"].Visible = _drumRowSettings[6].Visility;
            BottleDataGridView.Columns["CapTighteningTorqueValue_Big"].Visible = _drumRowSettings[7].Visility;
            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].Visible = _drumRowSettings[8].Visility;
            BottleDataGridView.Columns["CapTiltDetectionJudgment"].Visible = _drumRowSettings[9].Visility;
            BottleDataGridView.Columns["TotalCahckJudgment"].Visible = _drumRowSettings[10].Visility;
            BottleDataGridView.Columns["CapTighteningTorqueValue_Small"].Visible = _drumRowSettings[11].Visility;
            BottleDataGridView.Columns["FillingWeightJudgment"].Visible = _drumRowSettings[12].Visility;
            BottleDataGridView.Columns["BottleLocation"].Visible = _drumRowSettings[13].Visility;
            BottleDataGridView.Columns["FillingWeight"].Visible = _drumRowSettings[14].Visility;
            BottleDataGridView.Columns["FillingTime"].Visible = _drumRowSettings[15].Visility;
            BottleDataGridView.Columns["FillingStartDate"].Visible = _drumRowSettings[16].Visility;
            BottleDataGridView.Columns["FillingEndDate"].Visible = _drumRowSettings[17].Visility;
        }

        private void SetBottleResultGrid(DataTable dt)
        {
            int ColumnSize1 = 150;
            int ColumnSize2 = 100;
            int ColumnSize3 = 200;

            BottleDataGridView.RowTemplate.Height = 28;
            BottleDataGridView.AutoGenerateColumns = true;
            BottleDataGridView.DataSource = dt;

            

            
            BottleDataGridView.Columns["OrderNumber"].HeaderText = dt.Columns["OrderNumber"]!.Caption;
            BottleDataGridView.Columns["OrderNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProductLotNumber"].HeaderText = dt.Columns["ProductLotNumber"]!.Caption;
            BottleDataGridView.Columns["ProductLotNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProductItemCode"].HeaderText = dt.Columns["ProductItemCode"]!.Caption;
            BottleDataGridView.Columns["ProductItemCode"].Width = ColumnSize1;

            BottleDataGridView.Columns["MiddleProductLotNumber"].HeaderText = dt.Columns["MiddleProductLotNumber"]!.Caption;
            BottleDataGridView.Columns["MiddleProductLotNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["MiddleProductItemCode"].HeaderText = dt.Columns["MiddleProductItemCode"]!.Caption;
            BottleDataGridView.Columns["MiddleProductItemCode"].Width = ColumnSize1;

            BottleDataGridView.Columns["BottleID"].HeaderText = dt.Columns["BottleID"]!.Caption;
            BottleDataGridView.Columns["BottleID"].Width = ColumnSize2;

            BottleDataGridView.Columns["SamplingGroup"].HeaderText = dt.Columns["SamplingGroup"]!.Caption;
            BottleDataGridView.Columns["SamplingGroup"].Width = ColumnSize3;

            BottleDataGridView.Columns["BottleINumber"].HeaderText = dt.Columns["BottleINumber"]!.Caption;
            BottleDataGridView.Columns["BottleINumber"].Width = ColumnSize2;

            BottleDataGridView.Columns["FillingNozzleNumber"].HeaderText = dt.Columns["FillingNozzleNumber"]!.Caption;
            BottleDataGridView.Columns["FillingNozzleNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["CapTighteningTorqueValue"].HeaderText = dt.Columns["CapTighteningTorqueValue"]!.Caption;
            BottleDataGridView.Columns["CapTighteningTorqueValue"].Width = ColumnSize3;

            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].HeaderText = dt.Columns["CapTighteningTorqueJudgment"]!.Caption;
            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].Width = ColumnSize3;

            BottleDataGridView.Columns["CapTiltDetectionJudgment"].HeaderText = dt.Columns["CapTiltDetectionJudgment"]!.Caption;
            BottleDataGridView.Columns["CapTiltDetectionJudgment"].Width = ColumnSize3;

            BottleDataGridView.Columns["FillingMachineNumber"].HeaderText = dt.Columns["FillingMachineNumber"]!.Caption;
            BottleDataGridView.Columns["FillingMachineNumber"].Width = ColumnSize2;

            BottleDataGridView.Columns["TotalCahckJudgment"].HeaderText = dt.Columns["TotalCahckJudgment"]!.Caption;
            BottleDataGridView.Columns["TotalCahckJudgment"].Width = ColumnSize2;
         
            BottleDataGridView.Columns["BottleLocation"].HeaderText = dt.Columns["BottleLocation"]!.Caption;
            BottleDataGridView.Columns["BottleLocation"].Width = ColumnSize1;
            
            BottleDataGridView.Columns["FillingWeight"].HeaderText = dt.Columns["FillingWeight"]!.Caption;
            BottleDataGridView.Columns["FillingWeight"].Width = ColumnSize2;
            
            BottleDataGridView.Columns["FillingTime"].HeaderText = dt.Columns["FillingTime"]!.Caption;
            BottleDataGridView.Columns["FillingTime"].Width = ColumnSize2;

            BottleDataGridView.Columns["FillingStartDate"].HeaderText = dt.Columns["FillingStartDate"]!.Caption;
            BottleDataGridView.Columns["FillingStartDate"].Width = ColumnSize1;
            
            BottleDataGridView.Columns["FillingEndDate"].HeaderText = dt.Columns["FillingEndDate"]!.Caption;
            BottleDataGridView.Columns["FillingEndDate"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProcessType"].Visible = false;
            BottleDataGridView.Columns["ProcessType"].HeaderText = dt.Columns["ProcessType"]!.Caption;
            BottleDataGridView.Columns["ProcessType"].Width = 0;

        }

        private void SetDrumResultGrid(DataTable dt)
        {
            int ColumnSize1 = 150;
            int ColumnSize2 = 100;
            int ColumnSize3 = 200;

            BottleDataGridView.AutoGenerateColumns = true;
            BottleDataGridView.RowTemplate.Height = 28;
            BottleDataGridView.DataSource = dt;

            BottleDataGridView.Columns["OrderNumber"].HeaderText = dt.Columns["OrderNumber"]!.Caption;
            BottleDataGridView.Columns["OrderNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProductLotNumber"].HeaderText = dt.Columns["ProductLotNumber"]!.Caption;
            BottleDataGridView.Columns["ProductLotNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProductItemCode"].HeaderText = dt.Columns["ProductItemCode"]!.Caption;
            BottleDataGridView.Columns["ProductItemCode"].Width = ColumnSize1;

            BottleDataGridView.Columns["MiddleProductLotNumber"].HeaderText = dt.Columns["MiddleProductLotNumber"]!.Caption;
            BottleDataGridView.Columns["MiddleProductLotNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["MiddleProductItemCode"].HeaderText = dt.Columns["MiddleProductItemCode"]!.Caption;
            BottleDataGridView.Columns["MiddleProductItemCode"].Width = ColumnSize1;

            BottleDataGridView.Columns["DrumcanNumber"].HeaderText = dt.Columns["DrumcanNumber"]!.Caption;
            BottleDataGridView.Columns["DrumcanNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["FillingNozzleNumber"].HeaderText = dt.Columns["FillingNozzleNumber"]!.Caption;
            BottleDataGridView.Columns["FillingNozzleNumber"].Width = ColumnSize1;

            BottleDataGridView.Columns["CapTighteningTorqueValue_Big"].HeaderText = dt.Columns["CapTighteningTorqueValue_Big"]!.Caption;
            BottleDataGridView.Columns["CapTighteningTorqueValue_Big"].Width = ColumnSize3;

            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].HeaderText = dt.Columns["CapTighteningTorqueJudgment"]!.Caption;
            BottleDataGridView.Columns["CapTighteningTorqueJudgment"].Width = ColumnSize3;

            BottleDataGridView.Columns["CapTiltDetectionJudgment"].HeaderText = dt.Columns["CapTiltDetectionJudgment"]!.Caption;
            BottleDataGridView.Columns["CapTiltDetectionJudgment"].Width = ColumnSize3;

            BottleDataGridView.Columns["TotalCahckJudgment"].HeaderText = dt.Columns["TotalCahckJudgment"]!.Caption;
            BottleDataGridView.Columns["TotalCahckJudgment"].Width = ColumnSize2;

            BottleDataGridView.Columns["CapTighteningTorqueValue_Small"].HeaderText = dt.Columns["CapTighteningTorqueValue_Small"]!.Caption;
            BottleDataGridView.Columns["CapTighteningTorqueValue_Small"].Width = ColumnSize3;

            BottleDataGridView.Columns["FillingWeightJudgment"].HeaderText = dt.Columns["FillingWeightJudgment"]!.Caption;
            BottleDataGridView.Columns["FillingWeightJudgment"].Width = ColumnSize2;

            BottleDataGridView.Columns["BottleLocation"].HeaderText = dt.Columns["BottleLocation"]!.Caption;
            BottleDataGridView.Columns["BottleLocation"].Width = ColumnSize1;

            BottleDataGridView.Columns["FillingWeight"].HeaderText = dt.Columns["FillingWeight"]!.Caption;
            BottleDataGridView.Columns["FillingWeight"].Width = ColumnSize2;

            BottleDataGridView.Columns["FillingTime"].HeaderText = dt.Columns["FillingTime"]!.Caption;
            BottleDataGridView.Columns["FillingTime"].Width = ColumnSize2;

            BottleDataGridView.Columns["FillingStartDate"].HeaderText = dt.Columns["FillingStartDate"]!.Caption;
            BottleDataGridView.Columns["FillingStartDate"].Width = ColumnSize1;

            BottleDataGridView.Columns["FillingEndDate"].HeaderText = dt.Columns["FillingEndDate"]!.Caption;
            BottleDataGridView.Columns["FillingEndDate"].Width = ColumnSize1;

            BottleDataGridView.Columns["ProcessType"].Visible = false;
            BottleDataGridView.Columns["ProcessType"].HeaderText = dt.Columns["ProcessType"]!.Caption;
            BottleDataGridView.Columns["ProcessType"].Width = 0;
        }

        private void SetBottleResultDesign()
        {
            var grid = BottleDataGridView;

            grid.Width = 860;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black; 
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 238, 238);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.Font.FontFamily,11F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 32;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 0, 0, 0);


            grid.DefaultCellStyle.Font = new Font(grid.Font.FontFamily, 9F, FontStyle.Bold);
            grid.DefaultCellStyle.Padding = new Padding(5, 0, 0, 0);
            
        }

        private void btn_PagePrev_Click(object sender, EventArgs e)
        {
            if(_pageNo > 1) _pageNo--;
            RefreshResultView();
        }

        private void btn_PageNext_Click(object sender, EventArgs e)
        {
            _pageNo++;
            RefreshResultView();
        }

        private void btn_RowsSetting_Click(object sender, EventArgs e)
        {
            var screenPos = btn_RowsSetting.PointToScreen(Point.Empty);

            using (var f = new BottleResultSet(_bottleRowSettings,_drumRowSettings))
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(screenPos.X + (btn_RowsSetting.Width - f.Width) / 2, screenPos.Y );
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    _bottleRowSettings = f.BottleRowSet;
                    _drumRowSettings = f.DrumRowSet;


                    if (_pages.BottleTable.Rows.Count > 0)
                    {
                        SetBottleResultVisibility();
                    }

                    if (_pages.DrumTable.Rows.Count > 0)
                    {
                        SetDrumResultVisibility();
                    }
                    
                }

            }
        }
    }
}
