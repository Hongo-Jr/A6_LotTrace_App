using LotTraceApp.Models;
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
    public partial class BottleResultSet : Form
    {


        public List<BottleRowSettings> RowSet = new List<BottleRowSettings>();
        public BottleResultSet(List<BottleRowSettings> rowSettings)
        {
            RowSet = rowSettings.Select(x => new BottleRowSettings
            {
                Visility = x.Visility,
                Index = x.Index,
                SetNo = x.SetNo
            }).ToList();

            InitializeComponent();
            InitialChreck();
        }

        private void InitialChreck()
        {
            chk_Order_B.Checked = RowSet[0].Visility;
            chk_Lot_B.Checked = RowSet[1].Visility;
            chk_Code_B.Checked = RowSet[2].Visility;
            chk_Middle_Lot_B.Checked = RowSet[3].Visility;
            chk_Middle_Code_B.Checked = RowSet[4].Visility;
            chk_Bottle_Id_B.Checked = RowSet[5].Visility;
            chk_Samp_B.Checked = RowSet[6].Visility;
            chk_Bottle_Code_B.Checked = RowSet[7].Visility;
            chk_Noz_B.Checked = RowSet[8].Visility;
            chk_TorqueValue_B.Checked = RowSet[9].Visility;
            chk_Torque_Jud_B.Checked = RowSet[10].Visility;
            chk_Cap_Jud_B.Checked = RowSet[11].Visility;
            chk_Filling_No_B.Checked = RowSet[12].Visility;
            chk_Total_B.Checked = RowSet[13].Visility;
            chk_Location_B.Checked = RowSet[14].Visility;
            chk_Weight_B.Checked = RowSet[15].Visility;
            chk_time_B.Checked = RowSet[16].Visility;
            chk_StartTime_B.Checked = RowSet[17].Visility;
            chk_EndTime_B.Checked = RowSet[18].Visility;
        }
        private void ApplyRowSet()
        {
            RowSet[0].Visility = chk_Order_B.Checked;
            RowSet[1].Visility = chk_Lot_B.Checked;
            RowSet[2].Visility = chk_Code_B.Checked;
            RowSet[3].Visility = chk_Middle_Lot_B.Checked;
            RowSet[4].Visility = chk_Middle_Code_B.Checked;
            RowSet[5].Visility = chk_Bottle_Id_B.Checked;
            RowSet[6].Visility = chk_Samp_B.Checked;
            RowSet[7].Visility = chk_Bottle_Code_B.Checked;
            RowSet[8].Visility = chk_Noz_B.Checked;
            RowSet[9].Visility = chk_TorqueValue_B.Checked;
            RowSet[10].Visility = chk_Torque_Jud_B.Checked;
            RowSet[11].Visility = chk_Cap_Jud_B.Checked;
            RowSet[12].Visility = chk_Filling_No_B.Checked;
            RowSet[13].Visility = chk_Total_B.Checked;
            RowSet[14].Visility = chk_Location_B.Checked;
            RowSet[15].Visility = chk_Weight_B.Checked;
            RowSet[16].Visility = chk_time_B.Checked;
            RowSet[17].Visility = chk_StartTime_B.Checked;
            RowSet[18].Visility = chk_EndTime_B.Checked;
        }

        private void btn_cansel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            ApplyRowSet();

            Close();
        }
    }
}
