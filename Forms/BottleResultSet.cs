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


        public List<BottleRowSettings> BottleRowSet = new List<BottleRowSettings>();
        public List<BottleRowSettings> DrumRowSet = new List<BottleRowSettings>();
        public BottleResultSet(List<BottleRowSettings> bottleRowSet, List<BottleRowSettings> drumRowSet)
        {
            BottleRowSet = bottleRowSet.Select(x => new BottleRowSettings
            {
                Visility = x.Visility,
                Index = x.Index,
                SetNo = x.SetNo
            }).ToList();

            DrumRowSet = drumRowSet.Select(x => new BottleRowSettings
            {
                Visility = x.Visility,
                Index = x.Index,
                SetNo = x.SetNo
            }).ToList();


            InitializeComponent();


            InitialCheck();
            
        }

        private void InitialCheck()
        {
            chk_Order_B.Checked = BottleRowSet[0].Visility;
            chk_Lot_B.Checked = BottleRowSet[1].Visility;
            chk_Code_B.Checked = BottleRowSet[2].Visility;
            chk_Middle_Lot_B.Checked = BottleRowSet[3].Visility;
            chk_Middle_Code_B.Checked = BottleRowSet[4].Visility;
            chk_Bottle_Id_B.Checked = BottleRowSet[5].Visility;
            chk_Samp_B.Checked = BottleRowSet[6].Visility;
            chk_Bottle_Code_B.Checked = BottleRowSet[7].Visility;
            chk_Noz_B.Checked = BottleRowSet[8].Visility;
            chk_TorqueValue_B.Checked = BottleRowSet[9].Visility;
            chk_Torque_Jud_B.Checked = BottleRowSet[10].Visility;
            chk_Cap_Jud_B.Checked = BottleRowSet[11].Visility;
            chk_Filling_No_B.Checked = BottleRowSet[12].Visility;
            chk_Total_B.Checked = BottleRowSet[13].Visility;
            chk_Location_B.Checked = BottleRowSet[14].Visility;
            chk_Weight_B.Checked = BottleRowSet[15].Visility;
            chk_time_B.Checked = BottleRowSet[16].Visility;
            chk_StartTime_B.Checked = BottleRowSet[17].Visility;
            chk_EndTime_B.Checked = BottleRowSet[18].Visility;

            chk_Order_D.Checked = DrumRowSet[0].Visility;
            chk_Lot_D.Checked = DrumRowSet[1].Visility;
            chk_Code_D.Checked = DrumRowSet[2].Visility;
            chk_Middle_Lot_D.Checked = DrumRowSet[3].Visility;
            chk_Middle_Code_D.Checked = DrumRowSet[4].Visility;
            chk_DrumNo_D.Checked = DrumRowSet[5].Visility;
            chk_Noz_D.Checked = DrumRowSet[6].Visility;
            chk_TorqueValue_Big_D.Checked = DrumRowSet[7].Visility;
            chk_Torque_Jud_D.Checked = DrumRowSet[8].Visility;
            chk_Cap_Jud_D.Checked = DrumRowSet[9].Visility;
            chk_Total_D.Checked = DrumRowSet[10].Visility;
            chk_TorqueValue_Small_D.Checked = DrumRowSet[11].Visility;
            chk_WeightJudg_D.Checked = DrumRowSet[12].Visility;
            chk_Location_D.Checked = DrumRowSet[13].Visility;
            chk_Weight_D.Checked = DrumRowSet[14].Visility;
            chk_time_D.Checked = DrumRowSet[15].Visility;
            chk_StartTime_D.Checked = DrumRowSet[16].Visility;
            chk_EndTime_D.Checked = DrumRowSet[17].Visility;

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

        private void ApplyRowSet()
        {
            BottleRowSet[0].Visility = chk_Order_B.Checked;
            BottleRowSet[1].Visility = chk_Lot_B.Checked;
            BottleRowSet[2].Visility = chk_Code_B.Checked;
            BottleRowSet[3].Visility = chk_Middle_Lot_B.Checked;
            BottleRowSet[4].Visility = chk_Middle_Code_B.Checked;
            BottleRowSet[5].Visility = chk_Bottle_Id_B.Checked;
            BottleRowSet[6].Visility = chk_Samp_B.Checked;
            BottleRowSet[7].Visility = chk_Bottle_Code_B.Checked;
            BottleRowSet[8].Visility = chk_Noz_B.Checked;
            BottleRowSet[9].Visility = chk_TorqueValue_B.Checked;
            BottleRowSet[10].Visility = chk_Torque_Jud_B.Checked;
            BottleRowSet[11].Visility = chk_Cap_Jud_B.Checked;
            BottleRowSet[12].Visility = chk_Filling_No_B.Checked;
            BottleRowSet[13].Visility = chk_Total_B.Checked;
            BottleRowSet[14].Visility = chk_Location_B.Checked;
            BottleRowSet[15].Visility = chk_Weight_B.Checked;
            BottleRowSet[16].Visility = chk_time_B.Checked;
            BottleRowSet[17].Visility = chk_StartTime_B.Checked;
            BottleRowSet[18].Visility = chk_EndTime_B.Checked;

            DrumRowSet[0].Visility = chk_Order_D.Checked;
            DrumRowSet[1].Visility = chk_Lot_D.Checked;
            DrumRowSet[2].Visility = chk_Code_D.Checked;
            DrumRowSet[3].Visility = chk_Middle_Lot_D.Checked;
            DrumRowSet[4].Visility = chk_Middle_Code_D.Checked;
            DrumRowSet[5].Visility = chk_DrumNo_D.Checked;
            DrumRowSet[6].Visility = chk_Noz_D.Checked;
            DrumRowSet[7].Visility = chk_TorqueValue_Big_D.Checked;
            DrumRowSet[8].Visility = chk_Torque_Jud_D.Checked;
            DrumRowSet[9].Visility = chk_Cap_Jud_D.Checked;
            DrumRowSet[10].Visility = chk_Total_D.Checked;
            DrumRowSet[11].Visility = chk_TorqueValue_Small_D.Checked;
            DrumRowSet[12].Visility = chk_WeightJudg_D.Checked;
            DrumRowSet[13].Visility = chk_Location_D.Checked;
            DrumRowSet[14].Visility = chk_Weight_D.Checked;
            DrumRowSet[15].Visility = chk_time_D.Checked;
            DrumRowSet[16].Visility = chk_StartTime_D.Checked;
            DrumRowSet[17].Visility = chk_EndTime_D.Checked;

        }

        
    }
}
