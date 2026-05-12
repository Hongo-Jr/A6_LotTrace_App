using System.Windows.Forms;

namespace LotTraceApp
{
    partial class BottleTraceForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnBackToLiquid = new System.Windows.Forms.Button();
            this.btnBottleScreen = new System.Windows.Forms.Button();
            this.bottoleIntersectionTab = new System.Windows.Forms.TabPage();
            this.dataGridIntersection = new System.Windows.Forms.DataGridView();
            this.tabBottolePage2 = new System.Windows.Forms.TabPage();
            this.dgvStartBottle_2 = new System.Windows.Forms.DataGridView();
            this.dgvEndBottle_2 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_2 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_2 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_2 = new System.Windows.Forms.Label();
            this.panelStartBottole_2 = new System.Windows.Forms.Panel();
            this.panelEndBottole_2 = new System.Windows.Forms.Panel();
            this.txtBottoleOrderNo_2 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_2 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_2 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_2 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_2 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_2 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_2 = new System.Windows.Forms.Label();
            this.lblTargetBottole_2 = new System.Windows.Forms.Label();
            this.timeCheck_2 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_2 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_2 = new System.Windows.Forms.Label();
            this.endBottoleTime_2 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottole_2 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottole_2 = new System.Windows.Forms.Button();
            this.btnBottoleTraceSearch_2 = new System.Windows.Forms.Button();
            this.swichBottleTab = new System.Windows.Forms.TabControl();
            this.tabBottolePage1 = new System.Windows.Forms.TabPage();
            this.panelStartBottole = new System.Windows.Forms.Panel();
            this.dgvStartBottle = new System.Windows.Forms.DataGridView();
            this.panelEndBottole = new System.Windows.Forms.Panel();
            this.dgvEndBottle = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName = new System.Windows.Forms.Label();
            this.lblBottleItemCode = new System.Windows.Forms.Label();
            this.lblBottleLotNo = new System.Windows.Forms.Label();
            this.lblTargetBottole = new System.Windows.Forms.Label();
            this.timeCheck = new System.Windows.Forms.CheckBox();
            this.startBottoleTime = new System.Windows.Forms.DateTimePicker();
            this.lblTilde = new System.Windows.Forms.Label();
            this.endBottoleTime = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle = new System.Windows.Forms.Button();
            this.btnBottoleTraceSearch = new System.Windows.Forms.Button();
            this.tabBottolePage3 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_3 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_3 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_3 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_3 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_3 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_3 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_3 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_3 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_3 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_3 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_3 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_3 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_3 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_3 = new System.Windows.Forms.Label();
            this.lblTargetBottole_3 = new System.Windows.Forms.Label();
            this.timeCheck_3 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_3 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_3 = new System.Windows.Forms.Label();
            this.endBottoleTime_3 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_3 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_3 = new System.Windows.Forms.Button();
            this.btnTraceSearch_3 = new System.Windows.Forms.Button();
            this.tabBottolePage4 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_4 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_4 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_4 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_4 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_4 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_4 = new System.Windows.Forms.RadioButton();
            this.lblOrderNumber_4 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_4 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_4 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_4 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_4 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_4 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_4 = new System.Windows.Forms.Label();
            this.lblTargetBottole_4 = new System.Windows.Forms.Label();
            this.timeCheck_4 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_4 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_4 = new System.Windows.Forms.Label();
            this.endBottoleTime_4 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_4 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_4 = new System.Windows.Forms.Button();
            this.btnTraceSearch_4 = new System.Windows.Forms.Button();
            this.tabBottolePage5 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_5 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_5 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_5 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_5 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_5 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_5 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_5 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_5 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_5 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_5 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_5 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_5 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_5 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_5 = new System.Windows.Forms.Label();
            this.lblTargetBottole_5 = new System.Windows.Forms.Label();
            this.timeCheck_5 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_5 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_5 = new System.Windows.Forms.Label();
            this.endBottoleTime_5 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_5 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_5 = new System.Windows.Forms.Button();
            this.btnTraceSearch_5 = new System.Windows.Forms.Button();
            this.tabBottolePage6 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_6 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_6 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_6 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_6 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_6 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_6 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_6 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_6 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_6 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_6 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_6 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_6 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_6 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_6 = new System.Windows.Forms.Label();
            this.lblTargetBottole_6 = new System.Windows.Forms.Label();
            this.timeCheck_6 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_6 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_6 = new System.Windows.Forms.Label();
            this.endBottoleTime_6 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_6 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_6 = new System.Windows.Forms.Button();
            this.btnTraceSearch_6 = new System.Windows.Forms.Button();
            this.tabBottolePage7 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_7 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_7 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_7 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_7 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_7 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_7 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_7 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_7 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_7 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_7 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_7 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_7 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_7 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_7 = new System.Windows.Forms.Label();
            this.lblTargetBottole_7 = new System.Windows.Forms.Label();
            this.timeCheck_7 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_7 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_7 = new System.Windows.Forms.Label();
            this.endBottoleTime_7 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_7 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_7 = new System.Windows.Forms.Button();
            this.btnTraceSearch_7 = new System.Windows.Forms.Button();
            this.tabBottolePage8 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_8 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_8 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_8 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_8 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_8 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_8 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_8 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_8 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_8 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_8 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_8 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_8 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_8 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_8 = new System.Windows.Forms.Label();
            this.lblTargetBottole_8 = new System.Windows.Forms.Label();
            this.timeCheck_8 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_8 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_8 = new System.Windows.Forms.Label();
            this.endBottoleTime_8 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_8 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_8 = new System.Windows.Forms.Button();
            this.btnTraceSearch_8 = new System.Windows.Forms.Button();
            this.tabBottolePage9 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_9 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_9 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_9 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_9 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_9 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_9 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_9 = new System.Windows.Forms.Label();
            this.txtBottoleOrderNo_9 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_9 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_9 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_9 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_9 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_9 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_9 = new System.Windows.Forms.Label();
            this.lblTargetBottole_9 = new System.Windows.Forms.Label();
            this.timeCheck_9 = new System.Windows.Forms.CheckBox();
            this.startBottoleTime_9 = new System.Windows.Forms.DateTimePicker();
            this.lblBottole_9 = new System.Windows.Forms.Label();
            this.endBottoleTime_9 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_9 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_9 = new System.Windows.Forms.Button();
            this.btnTraceSearch_9 = new System.Windows.Forms.Button();
            this.tabBottolePage10 = new System.Windows.Forms.TabPage();
            this.panelStartBottole_10 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_10 = new System.Windows.Forms.DataGridView();
            this.panelEndBottole_10 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_10 = new System.Windows.Forms.DataGridView();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_10 = new System.Windows.Forms.RadioButton();
            this.lblBottoleOrderNo_10 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemName_10 = new System.Windows.Forms.TextBox();
            this.txtBottoleItemCode_10 = new System.Windows.Forms.TextBox();
            this.txtBottoleLotNo_10 = new System.Windows.Forms.TextBox();
            this.lblBottoleItemName_10 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_10 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_10 = new System.Windows.Forms.Label();
            this.lblTargetBottole_10 = new System.Windows.Forms.Label();
            this.timeCheck_10 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker5 = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.endBottoleTime_10 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_10 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_10 = new System.Windows.Forms.Button();
            this.btnTraceSearch_10 = new System.Windows.Forms.Button();
            this.bottoleTitle = new System.Windows.Forms.Label();
            this.rdoBottoleTabNameItemCode = new System.Windows.Forms.RadioButton();
            this.selectBottole = new System.Windows.Forms.Label();
            this.rdoBottoleTabNameOrder = new System.Windows.Forms.RadioButton();
            this.btnBottoleExcelOutput = new System.Windows.Forms.Button();
            this.btnBottoleDetectCrossPoints = new System.Windows.Forms.Button();
            this.checkBottole1 = new System.Windows.Forms.CheckBox();
            this.checkBottole2 = new System.Windows.Forms.CheckBox();
            this.checkBottole3 = new System.Windows.Forms.CheckBox();
            this.checkBottole4 = new System.Windows.Forms.CheckBox();
            this.checkBottole5 = new System.Windows.Forms.CheckBox();
            this.checkBottole6 = new System.Windows.Forms.CheckBox();
            this.checkBottole7 = new System.Windows.Forms.CheckBox();
            this.checkBottole8 = new System.Windows.Forms.CheckBox();
            this.checkBottole9 = new System.Windows.Forms.CheckBox();
            this.checkBottole10 = new System.Windows.Forms.CheckBox();
            this.bottoleIntersectionTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).BeginInit();
            this.tabBottolePage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_2)).BeginInit();
            this.swichBottleTab.SuspendLayout();
            this.tabBottolePage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle)).BeginInit();
            this.tabBottolePage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_3)).BeginInit();
            this.tabBottolePage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_4)).BeginInit();
            this.tabBottolePage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_5)).BeginInit();
            this.tabBottolePage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_6)).BeginInit();
            this.tabBottolePage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_7)).BeginInit();
            this.tabBottolePage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_8)).BeginInit();
            this.tabBottolePage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_9)).BeginInit();
            this.tabBottolePage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_10)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBackToLiquid
            // 
            this.btnBackToLiquid.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.btnBackToLiquid.Location = new System.Drawing.Point(10, 15);
            this.btnBackToLiquid.Name = "btnBackToLiquid";
            this.btnBackToLiquid.Size = new System.Drawing.Size(150, 40);
            this.btnBackToLiquid.TabIndex = 24;
            this.btnBackToLiquid.Text = "液設備";
            // 
            // btnBottleScreen
            // 
            this.btnBottleScreen.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.btnBottleScreen.Location = new System.Drawing.Point(170, 15);
            this.btnBottleScreen.Name = "btnBottleScreen";
            this.btnBottleScreen.Size = new System.Drawing.Size(150, 40);
            this.btnBottleScreen.TabIndex = 25;
            this.btnBottleScreen.Text = "瓶設備";
            // 
            // bottoleIntersectionTab
            // 
            this.bottoleIntersectionTab.Controls.Add(this.dataGridIntersection);
            this.bottoleIntersectionTab.Location = new System.Drawing.Point(4, 34);
            this.bottoleIntersectionTab.Name = "bottoleIntersectionTab";
            this.bottoleIntersectionTab.Padding = new System.Windows.Forms.Padding(3);
            this.bottoleIntersectionTab.Size = new System.Drawing.Size(1896, 902);
            this.bottoleIntersectionTab.TabIndex = 3;
            this.bottoleIntersectionTab.Text = "交点検出結果";
            this.bottoleIntersectionTab.UseVisualStyleBackColor = true;
            // 
            // dataGridIntersection
            // 
            this.dataGridIntersection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridIntersection.Location = new System.Drawing.Point(16, 16);
            this.dataGridIntersection.Name = "dataGridIntersection";
            this.dataGridIntersection.ReadOnly = true;
            this.dataGridIntersection.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridIntersection.Size = new System.Drawing.Size(1864, 870);
            this.dataGridIntersection.TabIndex = 0;
            // 
            // tabBottolePage2
            // 
            this.tabBottolePage2.Controls.Add(this.dgvStartBottle_2);
            this.tabBottolePage2.Controls.Add(this.dgvEndBottle_2);
            this.tabBottolePage2.Controls.Add(this.rdoBackwardBottle_2);
            this.tabBottolePage2.Controls.Add(this.rdoForwardBottle_2);
            this.tabBottolePage2.Controls.Add(this.lblBottoleOrderNo_2);
            this.tabBottolePage2.Controls.Add(this.panelStartBottole_2);
            this.tabBottolePage2.Controls.Add(this.panelEndBottole_2);
            this.tabBottolePage2.Controls.Add(this.txtBottoleOrderNo_2);
            this.tabBottolePage2.Controls.Add(this.txtBottoleItemName_2);
            this.tabBottolePage2.Controls.Add(this.txtBottoleItemCode_2);
            this.tabBottolePage2.Controls.Add(this.txtBottoleLotNo_2);
            this.tabBottolePage2.Controls.Add(this.lblBottoleItemName_2);
            this.tabBottolePage2.Controls.Add(this.lblBottleItemCode_2);
            this.tabBottolePage2.Controls.Add(this.lblBottleLotNo_2);
            this.tabBottolePage2.Controls.Add(this.lblTargetBottole_2);
            this.tabBottolePage2.Controls.Add(this.timeCheck_2);
            this.tabBottolePage2.Controls.Add(this.startBottoleTime_2);
            this.tabBottolePage2.Controls.Add(this.lblBottole_2);
            this.tabBottolePage2.Controls.Add(this.endBottoleTime_2);
            this.tabBottolePage2.Controls.Add(this.btnClearBottole_2);
            this.tabBottolePage2.Controls.Add(this.btnCsvOutputBottole_2);
            this.tabBottolePage2.Controls.Add(this.btnBottoleTraceSearch_2);
            this.tabBottolePage2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage2.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage2.Name = "tabBottolePage2";
            this.tabBottolePage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage2.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage2.TabIndex = 13;
            this.tabBottolePage2.Text = "(02)_未設定";
            this.tabBottolePage2.UseVisualStyleBackColor = true;
            // 
            // dgvStartBottle_2
            // 
            this.dgvStartBottle_2.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_2.Name = "dgvStartBottle_2";
            this.dgvStartBottle_2.ReadOnly = true;
            this.dgvStartBottle_2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_2.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_2.TabIndex = 29;
            // 
            // dgvEndBottle_2
            // 
            this.dgvEndBottle_2.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_2.Name = "dgvEndBottle_2";
            this.dgvEndBottle_2.ReadOnly = true;
            this.dgvEndBottle_2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_2.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_2.TabIndex = 31;
            // 
            // rdoBackwardBottle_2
            // 
            this.rdoBackwardBottle_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_2.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_2.Name = "rdoBackwardBottle_2";
            this.rdoBackwardBottle_2.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_2.TabIndex = 1;
            this.rdoBackwardBottle_2.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_2
            // 
            this.rdoForwardBottle_2.Checked = true;
            this.rdoForwardBottle_2.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_2.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_2.Name = "rdoForwardBottle_2";
            this.rdoForwardBottle_2.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_2.TabIndex = 0;
            this.rdoForwardBottle_2.TabStop = true;
            this.rdoForwardBottle_2.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_2
            // 
            this.lblBottoleOrderNo_2.AutoSize = true;
            this.lblBottoleOrderNo_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_2.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_2.Name = "lblBottoleOrderNo_2";
            this.lblBottoleOrderNo_2.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_2.TabIndex = 1;
            this.lblBottoleOrderNo_2.Text = "製造指図番号";
            // 
            // panelStartBottole_2
            // 
            this.panelStartBottole_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_2.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_2.Name = "panelStartBottole_2";
            this.panelStartBottole_2.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_2.TabIndex = 28;
            // 
            // panelEndBottole_2
            // 
            this.panelEndBottole_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottole_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_2.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_2.Name = "panelEndBottole_2";
            this.panelEndBottole_2.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_2.TabIndex = 30;
            // 
            // txtBottoleOrderNo_2
            // 
            this.txtBottoleOrderNo_2.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_2.Name = "txtBottoleOrderNo_2";
            this.txtBottoleOrderNo_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_2.TabIndex = 2;
            // 
            // txtBottoleItemName_2
            // 
            this.txtBottoleItemName_2.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_2.Name = "txtBottoleItemName_2";
            this.txtBottoleItemName_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_2.TabIndex = 4;
            // 
            // txtBottoleItemCode_2
            // 
            this.txtBottoleItemCode_2.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_2.Name = "txtBottoleItemCode_2";
            this.txtBottoleItemCode_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_2.TabIndex = 6;
            // 
            // txtBottoleLotNo_2
            // 
            this.txtBottoleLotNo_2.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_2.Name = "txtBottoleLotNo_2";
            this.txtBottoleLotNo_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_2.TabIndex = 8;
            // 
            // lblBottoleItemName_2
            // 
            this.lblBottoleItemName_2.AutoSize = true;
            this.lblBottoleItemName_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_2.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_2.Name = "lblBottoleItemName_2";
            this.lblBottoleItemName_2.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_2.TabIndex = 3;
            this.lblBottoleItemName_2.Text = "品目名";
            // 
            // lblBottleItemCode_2
            // 
            this.lblBottleItemCode_2.AutoSize = true;
            this.lblBottleItemCode_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_2.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_2.Name = "lblBottleItemCode_2";
            this.lblBottleItemCode_2.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_2.TabIndex = 5;
            this.lblBottleItemCode_2.Text = "品目コード";
            // 
            // lblBottleLotNo_2
            // 
            this.lblBottleLotNo_2.AutoSize = true;
            this.lblBottleLotNo_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_2.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_2.Name = "lblBottleLotNo_2";
            this.lblBottleLotNo_2.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_2.TabIndex = 7;
            this.lblBottleLotNo_2.Text = "ロット番号";
            // 
            // lblTargetBottole_2
            // 
            this.lblTargetBottole_2.AutoSize = true;
            this.lblTargetBottole_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_2.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_2.Name = "lblTargetBottole_2";
            this.lblTargetBottole_2.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_2.TabIndex = 9;
            this.lblTargetBottole_2.Text = "対象期間";
            // 
            // timeCheck_2
            // 
            this.timeCheck_2.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_2.Name = "timeCheck_2";
            this.timeCheck_2.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_2.TabIndex = 10;
            // 
            // startBottoleTime_2
            // 
            this.startBottoleTime_2.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_2.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_2.Name = "startBottoleTime_2";
            this.startBottoleTime_2.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_2.TabIndex = 11;
            // 
            // lblBottole_2
            // 
            this.lblBottole_2.AutoSize = true;
            this.lblBottole_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_2.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_2.Name = "lblBottole_2";
            this.lblBottole_2.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_2.TabIndex = 12;
            this.lblBottole_2.Text = "～";
            // 
            // endBottoleTime_2
            // 
            this.endBottoleTime_2.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_2.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_2.Name = "endBottoleTime_2";
            this.endBottoleTime_2.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_2.TabIndex = 14;
            // 
            // btnClearBottole_2
            // 
            this.btnClearBottole_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottole_2.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottole_2.Name = "btnClearBottole_2";
            this.btnClearBottole_2.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottole_2.TabIndex = 18;
            this.btnClearBottole_2.Text = "クリア";
            // 
            // btnCsvOutputBottole_2
            // 
            this.btnCsvOutputBottole_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottole_2.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottole_2.Name = "btnCsvOutputBottole_2";
            this.btnCsvOutputBottole_2.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottole_2.TabIndex = 19;
            this.btnCsvOutputBottole_2.Text = "CSV出力";
            // 
            // btnBottoleTraceSearch_2
            // 
            this.btnBottoleTraceSearch_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnBottoleTraceSearch_2.Location = new System.Drawing.Point(1720, 15);
            this.btnBottoleTraceSearch_2.Name = "btnBottoleTraceSearch_2";
            this.btnBottoleTraceSearch_2.Size = new System.Drawing.Size(150, 50);
            this.btnBottoleTraceSearch_2.TabIndex = 22;
            this.btnBottoleTraceSearch_2.Text = "トレース検索";
            // 
            // swichBottleTab
            // 
            this.swichBottleTab.Controls.Add(this.tabBottolePage1);
            this.swichBottleTab.Controls.Add(this.tabBottolePage2);
            this.swichBottleTab.Controls.Add(this.tabBottolePage3);
            this.swichBottleTab.Controls.Add(this.tabBottolePage4);
            this.swichBottleTab.Controls.Add(this.tabBottolePage5);
            this.swichBottleTab.Controls.Add(this.tabBottolePage6);
            this.swichBottleTab.Controls.Add(this.tabBottolePage7);
            this.swichBottleTab.Controls.Add(this.tabBottolePage8);
            this.swichBottleTab.Controls.Add(this.tabBottolePage9);
            this.swichBottleTab.Controls.Add(this.tabBottolePage10);
            this.swichBottleTab.Controls.Add(this.bottoleIntersectionTab);
            this.swichBottleTab.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.swichBottleTab.ItemSize = new System.Drawing.Size(172, 30);
            this.swichBottleTab.Location = new System.Drawing.Point(8, 85);
            this.swichBottleTab.Name = "swichBottleTab";
            this.swichBottleTab.SelectedIndex = 0;
            this.swichBottleTab.Size = new System.Drawing.Size(1904, 940);
            this.swichBottleTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.swichBottleTab.TabIndex = 49;
            // 
            // tabBottolePage1
            // 
            this.tabBottolePage1.Controls.Add(this.panelStartBottole);
            this.tabBottolePage1.Controls.Add(this.dgvStartBottle);
            this.tabBottolePage1.Controls.Add(this.panelEndBottole);
            this.tabBottolePage1.Controls.Add(this.dgvEndBottle);
            this.tabBottolePage1.Controls.Add(this.rdoBackwardBottle);
            this.tabBottolePage1.Controls.Add(this.rdoForwardBottle);
            this.tabBottolePage1.Controls.Add(this.lblBottoleOrderNo);
            this.tabBottolePage1.Controls.Add(this.txtBottoleOrderNo);
            this.tabBottolePage1.Controls.Add(this.txtBottoleItemName);
            this.tabBottolePage1.Controls.Add(this.txtBottoleItemCode);
            this.tabBottolePage1.Controls.Add(this.txtBottoleLotNo);
            this.tabBottolePage1.Controls.Add(this.lblBottoleItemName);
            this.tabBottolePage1.Controls.Add(this.lblBottleItemCode);
            this.tabBottolePage1.Controls.Add(this.lblBottleLotNo);
            this.tabBottolePage1.Controls.Add(this.lblTargetBottole);
            this.tabBottolePage1.Controls.Add(this.timeCheck);
            this.tabBottolePage1.Controls.Add(this.startBottoleTime);
            this.tabBottolePage1.Controls.Add(this.lblTilde);
            this.tabBottolePage1.Controls.Add(this.endBottoleTime);
            this.tabBottolePage1.Controls.Add(this.btnClearBottle);
            this.tabBottolePage1.Controls.Add(this.btnCsvOutputBottle);
            this.tabBottolePage1.Controls.Add(this.btnBottoleTraceSearch);
            this.tabBottolePage1.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage1.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage1.Name = "tabBottolePage1";
            this.tabBottolePage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage1.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage1.TabIndex = 0;
            this.tabBottolePage1.Text = "(01)_未設定";
            this.tabBottolePage1.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole
            // 
            this.panelStartBottole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole.Name = "panelStartBottole";
            this.panelStartBottole.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole.TabIndex = 28;
            // 
            // dgvStartBottle
            // 
            this.dgvStartBottle.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle.Name = "dgvStartBottle";
            this.dgvStartBottle.ReadOnly = true;
            this.dgvStartBottle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle.TabIndex = 29;
            // 
            // panelEndBottole
            // 
            this.panelEndBottole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panelEndBottole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole.Name = "panelEndBottole";
            this.panelEndBottole.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole.TabIndex = 30;
            // 
            // dgvEndBottle
            // 
            this.dgvEndBottle.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle.Name = "dgvEndBottle";
            this.dgvEndBottle.ReadOnly = true;
            this.dgvEndBottle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle.TabIndex = 31;
            // 
            // rdoBackwardBottle
            // 
            this.rdoBackwardBottle.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle.Name = "rdoBackwardBottle";
            this.rdoBackwardBottle.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle.TabIndex = 1;
            this.rdoBackwardBottle.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle
            // 
            this.rdoForwardBottle.Checked = true;
            this.rdoForwardBottle.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle.Name = "rdoForwardBottle";
            this.rdoForwardBottle.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle.TabIndex = 0;
            this.rdoForwardBottle.TabStop = true;
            this.rdoForwardBottle.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo
            // 
            this.lblBottoleOrderNo.AutoSize = true;
            this.lblBottoleOrderNo.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo.Name = "lblBottoleOrderNo";
            this.lblBottoleOrderNo.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo.TabIndex = 1;
            this.lblBottoleOrderNo.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo
            // 
            this.txtBottoleOrderNo.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo.Name = "txtBottoleOrderNo";
            this.txtBottoleOrderNo.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo.TabIndex = 2;
            // 
            // txtBottoleItemName
            // 
            this.txtBottoleItemName.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName.Name = "txtBottoleItemName";
            this.txtBottoleItemName.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName.TabIndex = 4;
            // 
            // txtBottoleItemCode
            // 
            this.txtBottoleItemCode.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode.Name = "txtBottoleItemCode";
            this.txtBottoleItemCode.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode.TabIndex = 6;
            // 
            // txtBottoleLotNo
            // 
            this.txtBottoleLotNo.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo.Name = "txtBottoleLotNo";
            this.txtBottoleLotNo.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo.TabIndex = 8;
            // 
            // lblBottoleItemName
            // 
            this.lblBottoleItemName.AutoSize = true;
            this.lblBottoleItemName.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName.Name = "lblBottoleItemName";
            this.lblBottoleItemName.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName.TabIndex = 3;
            this.lblBottoleItemName.Text = "品目名";
            // 
            // lblBottleItemCode
            // 
            this.lblBottleItemCode.AutoSize = true;
            this.lblBottleItemCode.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode.Name = "lblBottleItemCode";
            this.lblBottleItemCode.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode.TabIndex = 5;
            this.lblBottleItemCode.Text = "品目コード";
            // 
            // lblBottleLotNo
            // 
            this.lblBottleLotNo.AutoSize = true;
            this.lblBottleLotNo.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo.Name = "lblBottleLotNo";
            this.lblBottleLotNo.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo.TabIndex = 7;
            this.lblBottleLotNo.Text = "ロット番号";
            // 
            // lblTargetBottole
            // 
            this.lblTargetBottole.AutoSize = true;
            this.lblTargetBottole.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole.Name = "lblTargetBottole";
            this.lblTargetBottole.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole.TabIndex = 9;
            this.lblTargetBottole.Text = "対象期間";
            // 
            // timeCheck
            // 
            this.timeCheck.Location = new System.Drawing.Point(367, 162);
            this.timeCheck.Name = "timeCheck";
            this.timeCheck.Size = new System.Drawing.Size(15, 14);
            this.timeCheck.TabIndex = 10;
            // 
            // startBottoleTime
            // 
            this.startBottoleTime.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime.Name = "startBottoleTime";
            this.startBottoleTime.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime.TabIndex = 11;
            // 
            // lblTilde
            // 
            this.lblTilde.AutoSize = true;
            this.lblTilde.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTilde.Location = new System.Drawing.Point(226, 162);
            this.lblTilde.Name = "lblTilde";
            this.lblTilde.Size = new System.Drawing.Size(19, 16);
            this.lblTilde.TabIndex = 12;
            this.lblTilde.Text = "～";
            // 
            // endBottoleTime
            // 
            this.endBottoleTime.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime.Name = "endBottoleTime";
            this.endBottoleTime.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime.TabIndex = 14;
            // 
            // btnClearBottle
            // 
            this.btnClearBottle.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle.Name = "btnClearBottle";
            this.btnClearBottle.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle.TabIndex = 18;
            this.btnClearBottle.Text = "クリア";
            // 
            // btnCsvOutputBottle
            // 
            this.btnCsvOutputBottle.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle.Name = "btnCsvOutputBottle";
            this.btnCsvOutputBottle.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle.TabIndex = 19;
            this.btnCsvOutputBottle.Text = "CSV出力";
            // 
            // btnBottoleTraceSearch
            // 
            this.btnBottoleTraceSearch.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnBottoleTraceSearch.Location = new System.Drawing.Point(1720, 15);
            this.btnBottoleTraceSearch.Name = "btnBottoleTraceSearch";
            this.btnBottoleTraceSearch.Size = new System.Drawing.Size(150, 50);
            this.btnBottoleTraceSearch.TabIndex = 22;
            this.btnBottoleTraceSearch.Text = "トレース検索";
            // 
            // tabBottolePage3
            // 
            this.tabBottolePage3.Controls.Add(this.panelStartBottole_3);
            this.tabBottolePage3.Controls.Add(this.dgvStartBottle_3);
            this.tabBottolePage3.Controls.Add(this.panelEndBottole_3);
            this.tabBottolePage3.Controls.Add(this.dgvEndBottle_3);
            this.tabBottolePage3.Controls.Add(this.rdoBackwardBottle_3);
            this.tabBottolePage3.Controls.Add(this.rdoForwardBottle_3);
            this.tabBottolePage3.Controls.Add(this.lblBottoleOrderNo_3);
            this.tabBottolePage3.Controls.Add(this.txtBottoleOrderNo_3);
            this.tabBottolePage3.Controls.Add(this.txtBottoleItemName_3);
            this.tabBottolePage3.Controls.Add(this.txtBottoleItemCode_3);
            this.tabBottolePage3.Controls.Add(this.txtBottoleLotNo_3);
            this.tabBottolePage3.Controls.Add(this.lblBottoleItemName_3);
            this.tabBottolePage3.Controls.Add(this.lblBottleItemCode_3);
            this.tabBottolePage3.Controls.Add(this.lblBottleLotNo_3);
            this.tabBottolePage3.Controls.Add(this.lblTargetBottole_3);
            this.tabBottolePage3.Controls.Add(this.timeCheck_3);
            this.tabBottolePage3.Controls.Add(this.startBottoleTime_3);
            this.tabBottolePage3.Controls.Add(this.lblBottole_3);
            this.tabBottolePage3.Controls.Add(this.endBottoleTime_3);
            this.tabBottolePage3.Controls.Add(this.btnClearBottle_3);
            this.tabBottolePage3.Controls.Add(this.btnCsvOutputBottle_3);
            this.tabBottolePage3.Controls.Add(this.btnTraceSearch_3);
            this.tabBottolePage3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage3.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage3.Name = "tabBottolePage3";
            this.tabBottolePage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage3.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage3.TabIndex = 14;
            this.tabBottolePage3.Text = "(03)_未設定";
            this.tabBottolePage3.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_3
            // 
            this.panelStartBottole_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_3.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_3.Name = "panelStartBottole_3";
            this.panelStartBottole_3.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_3.TabIndex = 28;
            // 
            // dgvStartBottle_3
            // 
            this.dgvStartBottle_3.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_3.Name = "dgvStartBottle_3";
            this.dgvStartBottle_3.ReadOnly = true;
            this.dgvStartBottle_3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_3.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_3.TabIndex = 29;
            // 
            // panelEndBottole_3
            // 
            this.panelEndBottole_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottole_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_3.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_3.Name = "panelEndBottole_3";
            this.panelEndBottole_3.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_3.TabIndex = 30;
            // 
            // dgvEndBottle_3
            // 
            this.dgvEndBottle_3.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_3.Name = "dgvEndBottle_3";
            this.dgvEndBottle_3.ReadOnly = true;
            this.dgvEndBottle_3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_3.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_3.TabIndex = 31;
            // 
            // rdoBackwardBottle_3
            // 
            this.rdoBackwardBottle_3.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_3.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_3.Name = "rdoBackwardBottle_3";
            this.rdoBackwardBottle_3.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_3.TabIndex = 1;
            this.rdoBackwardBottle_3.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_3
            // 
            this.rdoForwardBottle_3.Checked = true;
            this.rdoForwardBottle_3.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_3.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_3.Name = "rdoForwardBottle_3";
            this.rdoForwardBottle_3.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_3.TabIndex = 0;
            this.rdoForwardBottle_3.TabStop = true;
            this.rdoForwardBottle_3.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_3
            // 
            this.lblBottoleOrderNo_3.AutoSize = true;
            this.lblBottoleOrderNo_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_3.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_3.Name = "lblBottoleOrderNo_3";
            this.lblBottoleOrderNo_3.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_3.TabIndex = 1;
            this.lblBottoleOrderNo_3.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_3
            // 
            this.txtBottoleOrderNo_3.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_3.Name = "txtBottoleOrderNo_3";
            this.txtBottoleOrderNo_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_3.TabIndex = 2;
            // 
            // txtBottoleItemName_3
            // 
            this.txtBottoleItemName_3.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_3.Name = "txtBottoleItemName_3";
            this.txtBottoleItemName_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_3.TabIndex = 4;
            // 
            // txtBottoleItemCode_3
            // 
            this.txtBottoleItemCode_3.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_3.Name = "txtBottoleItemCode_3";
            this.txtBottoleItemCode_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_3.TabIndex = 6;
            // 
            // txtBottoleLotNo_3
            // 
            this.txtBottoleLotNo_3.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_3.Name = "txtBottoleLotNo_3";
            this.txtBottoleLotNo_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_3.TabIndex = 8;
            // 
            // lblBottoleItemName_3
            // 
            this.lblBottoleItemName_3.AutoSize = true;
            this.lblBottoleItemName_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_3.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_3.Name = "lblBottoleItemName_3";
            this.lblBottoleItemName_3.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_3.TabIndex = 3;
            this.lblBottoleItemName_3.Text = "品目名";
            // 
            // lblBottleItemCode_3
            // 
            this.lblBottleItemCode_3.AutoSize = true;
            this.lblBottleItemCode_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_3.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_3.Name = "lblBottleItemCode_3";
            this.lblBottleItemCode_3.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_3.TabIndex = 5;
            this.lblBottleItemCode_3.Text = "品目コード";
            // 
            // lblBottleLotNo_3
            // 
            this.lblBottleLotNo_3.AutoSize = true;
            this.lblBottleLotNo_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_3.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_3.Name = "lblBottleLotNo_3";
            this.lblBottleLotNo_3.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_3.TabIndex = 7;
            this.lblBottleLotNo_3.Text = "ロット番号";
            // 
            // lblTargetBottole_3
            // 
            this.lblTargetBottole_3.AutoSize = true;
            this.lblTargetBottole_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_3.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_3.Name = "lblTargetBottole_3";
            this.lblTargetBottole_3.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_3.TabIndex = 9;
            this.lblTargetBottole_3.Text = "対象期間";
            // 
            // timeCheck_3
            // 
            this.timeCheck_3.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_3.Name = "timeCheck_3";
            this.timeCheck_3.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_3.TabIndex = 10;
            // 
            // startBottoleTime_3
            // 
            this.startBottoleTime_3.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_3.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_3.Name = "startBottoleTime_3";
            this.startBottoleTime_3.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_3.TabIndex = 11;
            // 
            // lblBottole_3
            // 
            this.lblBottole_3.AutoSize = true;
            this.lblBottole_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_3.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_3.Name = "lblBottole_3";
            this.lblBottole_3.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_3.TabIndex = 12;
            this.lblBottole_3.Text = "～";
            // 
            // endBottoleTime_3
            // 
            this.endBottoleTime_3.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_3.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_3.Name = "endBottoleTime_3";
            this.endBottoleTime_3.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_3.TabIndex = 14;
            // 
            // btnClearBottle_3
            // 
            this.btnClearBottle_3.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_3.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_3.Name = "btnClearBottle_3";
            this.btnClearBottle_3.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_3.TabIndex = 18;
            this.btnClearBottle_3.Text = "クリア";
            // 
            // btnCsvOutputBottle_3
            // 
            this.btnCsvOutputBottle_3.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_3.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_3.Name = "btnCsvOutputBottle_3";
            this.btnCsvOutputBottle_3.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_3.TabIndex = 19;
            this.btnCsvOutputBottle_3.Text = "CSV出力";
            // 
            // btnTraceSearch_3
            // 
            this.btnTraceSearch_3.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_3.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_3.Name = "btnTraceSearch_3";
            this.btnTraceSearch_3.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_3.TabIndex = 22;
            this.btnTraceSearch_3.Text = "トレース検索";
            // 
            // tabBottolePage4
            // 
            this.tabBottolePage4.Controls.Add(this.panelStartBottole_4);
            this.tabBottolePage4.Controls.Add(this.dgvStartBottle_4);
            this.tabBottolePage4.Controls.Add(this.panelEndBottole_4);
            this.tabBottolePage4.Controls.Add(this.dgvEndBottle_4);
            this.tabBottolePage4.Controls.Add(this.rdoBackwardBottle_4);
            this.tabBottolePage4.Controls.Add(this.rdoForwardBottle_4);
            this.tabBottolePage4.Controls.Add(this.lblOrderNumber_4);
            this.tabBottolePage4.Controls.Add(this.txtBottoleOrderNo_4);
            this.tabBottolePage4.Controls.Add(this.textBox7);
            this.tabBottolePage4.Controls.Add(this.txtBottoleItemCode_4);
            this.tabBottolePage4.Controls.Add(this.txtBottoleLotNo_4);
            this.tabBottolePage4.Controls.Add(this.lblBottoleItemName_4);
            this.tabBottolePage4.Controls.Add(this.lblBottleItemCode_4);
            this.tabBottolePage4.Controls.Add(this.lblBottleLotNo_4);
            this.tabBottolePage4.Controls.Add(this.lblTargetBottole_4);
            this.tabBottolePage4.Controls.Add(this.timeCheck_4);
            this.tabBottolePage4.Controls.Add(this.startBottoleTime_4);
            this.tabBottolePage4.Controls.Add(this.lblBottole_4);
            this.tabBottolePage4.Controls.Add(this.endBottoleTime_4);
            this.tabBottolePage4.Controls.Add(this.btnClearBottle_4);
            this.tabBottolePage4.Controls.Add(this.btnCsvOutputBottle_4);
            this.tabBottolePage4.Controls.Add(this.btnTraceSearch_4);
            this.tabBottolePage4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage4.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage4.Name = "tabBottolePage4";
            this.tabBottolePage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage4.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage4.TabIndex = 15;
            this.tabBottolePage4.Text = "(04)_未設定";
            this.tabBottolePage4.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_4
            // 
            this.panelStartBottole_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_4.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_4.Name = "panelStartBottole_4";
            this.panelStartBottole_4.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_4.TabIndex = 28;
            // 
            // dgvStartBottle_4
            // 
            this.dgvStartBottle_4.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_4.Name = "dgvStartBottle_4";
            this.dgvStartBottle_4.ReadOnly = true;
            this.dgvStartBottle_4.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_4.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_4.TabIndex = 29;
            // 
            // panelEndBottole_4
            // 
            this.panelEndBottole_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottole_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_4.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_4.Name = "panelEndBottole_4";
            this.panelEndBottole_4.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_4.TabIndex = 30;
            // 
            // dgvEndBottle_4
            // 
            this.dgvEndBottle_4.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_4.Name = "dgvEndBottle_4";
            this.dgvEndBottle_4.ReadOnly = true;
            this.dgvEndBottle_4.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_4.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_4.TabIndex = 31;
            // 
            // rdoBackwardBottle_4
            // 
            this.rdoBackwardBottle_4.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_4.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_4.Name = "rdoBackwardBottle_4";
            this.rdoBackwardBottle_4.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_4.TabIndex = 1;
            this.rdoBackwardBottle_4.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_4
            // 
            this.rdoForwardBottle_4.Checked = true;
            this.rdoForwardBottle_4.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_4.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_4.Name = "rdoForwardBottle_4";
            this.rdoForwardBottle_4.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_4.TabIndex = 0;
            this.rdoForwardBottle_4.TabStop = true;
            this.rdoForwardBottle_4.Text = "トレースフォワード(追跡)";
            // 
            // lblOrderNumber_4
            // 
            this.lblOrderNumber_4.AutoSize = true;
            this.lblOrderNumber_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblOrderNumber_4.Location = new System.Drawing.Point(20, 20);
            this.lblOrderNumber_4.Name = "lblOrderNumber_4";
            this.lblOrderNumber_4.Size = new System.Drawing.Size(79, 16);
            this.lblOrderNumber_4.TabIndex = 1;
            this.lblOrderNumber_4.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_4
            // 
            this.txtBottoleOrderNo_4.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_4.Name = "txtBottoleOrderNo_4";
            this.txtBottoleOrderNo_4.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_4.TabIndex = 2;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(110, 50);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(250, 27);
            this.textBox7.TabIndex = 4;
            // 
            // txtBottoleItemCode_4
            // 
            this.txtBottoleItemCode_4.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_4.Name = "txtBottoleItemCode_4";
            this.txtBottoleItemCode_4.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_4.TabIndex = 6;
            // 
            // txtBottoleLotNo_4
            // 
            this.txtBottoleLotNo_4.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_4.Name = "txtBottoleLotNo_4";
            this.txtBottoleLotNo_4.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_4.TabIndex = 8;
            // 
            // lblBottoleItemName_4
            // 
            this.lblBottoleItemName_4.AutoSize = true;
            this.lblBottoleItemName_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_4.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_4.Name = "lblBottoleItemName_4";
            this.lblBottoleItemName_4.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_4.TabIndex = 3;
            this.lblBottoleItemName_4.Text = "品目名";
            // 
            // lblBottleItemCode_4
            // 
            this.lblBottleItemCode_4.AutoSize = true;
            this.lblBottleItemCode_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_4.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_4.Name = "lblBottleItemCode_4";
            this.lblBottleItemCode_4.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_4.TabIndex = 5;
            this.lblBottleItemCode_4.Text = "品目コード";
            // 
            // lblBottleLotNo_4
            // 
            this.lblBottleLotNo_4.AutoSize = true;
            this.lblBottleLotNo_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_4.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_4.Name = "lblBottleLotNo_4";
            this.lblBottleLotNo_4.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_4.TabIndex = 7;
            this.lblBottleLotNo_4.Text = "ロット番号";
            // 
            // lblTargetBottole_4
            // 
            this.lblTargetBottole_4.AutoSize = true;
            this.lblTargetBottole_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_4.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_4.Name = "lblTargetBottole_4";
            this.lblTargetBottole_4.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_4.TabIndex = 9;
            this.lblTargetBottole_4.Text = "対象期間";
            // 
            // timeCheck_4
            // 
            this.timeCheck_4.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_4.Name = "timeCheck_4";
            this.timeCheck_4.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_4.TabIndex = 10;
            // 
            // startBottoleTime_4
            // 
            this.startBottoleTime_4.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_4.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_4.Name = "startBottoleTime_4";
            this.startBottoleTime_4.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_4.TabIndex = 11;
            // 
            // lblBottole_4
            // 
            this.lblBottole_4.AutoSize = true;
            this.lblBottole_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_4.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_4.Name = "lblBottole_4";
            this.lblBottole_4.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_4.TabIndex = 12;
            this.lblBottole_4.Text = "～";
            // 
            // endBottoleTime_4
            // 
            this.endBottoleTime_4.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_4.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_4.Name = "endBottoleTime_4";
            this.endBottoleTime_4.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_4.TabIndex = 14;
            // 
            // btnClearBottle_4
            // 
            this.btnClearBottle_4.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_4.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_4.Name = "btnClearBottle_4";
            this.btnClearBottle_4.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_4.TabIndex = 18;
            this.btnClearBottle_4.Text = "クリア";
            // 
            // btnCsvOutputBottle_4
            // 
            this.btnCsvOutputBottle_4.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_4.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_4.Name = "btnCsvOutputBottle_4";
            this.btnCsvOutputBottle_4.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_4.TabIndex = 19;
            this.btnCsvOutputBottle_4.Text = "CSV出力";
            // 
            // btnTraceSearch_4
            // 
            this.btnTraceSearch_4.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_4.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_4.Name = "btnTraceSearch_4";
            this.btnTraceSearch_4.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_4.TabIndex = 22;
            this.btnTraceSearch_4.Text = "トレース検索";
            // 
            // tabBottolePage5
            // 
            this.tabBottolePage5.Controls.Add(this.panelStartBottole_5);
            this.tabBottolePage5.Controls.Add(this.dgvStartBottle_5);
            this.tabBottolePage5.Controls.Add(this.panelEndBottole_5);
            this.tabBottolePage5.Controls.Add(this.dgvEndBottle_5);
            this.tabBottolePage5.Controls.Add(this.rdoBackwardBottle_5);
            this.tabBottolePage5.Controls.Add(this.rdoForwardBottle_5);
            this.tabBottolePage5.Controls.Add(this.lblBottoleOrderNo_5);
            this.tabBottolePage5.Controls.Add(this.txtBottoleOrderNo_5);
            this.tabBottolePage5.Controls.Add(this.txtBottoleItemName_5);
            this.tabBottolePage5.Controls.Add(this.txtBottoleItemCode_5);
            this.tabBottolePage5.Controls.Add(this.txtBottoleLotNo_5);
            this.tabBottolePage5.Controls.Add(this.lblBottoleItemName_5);
            this.tabBottolePage5.Controls.Add(this.lblBottleItemCode_5);
            this.tabBottolePage5.Controls.Add(this.lblBottleLotNo_5);
            this.tabBottolePage5.Controls.Add(this.lblTargetBottole_5);
            this.tabBottolePage5.Controls.Add(this.timeCheck_5);
            this.tabBottolePage5.Controls.Add(this.startBottoleTime_5);
            this.tabBottolePage5.Controls.Add(this.lblBottole_5);
            this.tabBottolePage5.Controls.Add(this.endBottoleTime_5);
            this.tabBottolePage5.Controls.Add(this.btnClearBottle_5);
            this.tabBottolePage5.Controls.Add(this.btnCsvOutputBottle_5);
            this.tabBottolePage5.Controls.Add(this.btnTraceSearch_5);
            this.tabBottolePage5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage5.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage5.Name = "tabBottolePage5";
            this.tabBottolePage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage5.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage5.TabIndex = 16;
            this.tabBottolePage5.Text = "(05)_未設定";
            this.tabBottolePage5.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_5
            // 
            this.panelStartBottole_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_5.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_5.Name = "panelStartBottole_5";
            this.panelStartBottole_5.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_5.TabIndex = 28;
            // 
            // dgvStartBottle_5
            // 
            this.dgvStartBottle_5.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_5.Name = "dgvStartBottle_5";
            this.dgvStartBottle_5.ReadOnly = true;
            this.dgvStartBottle_5.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_5.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_5.TabIndex = 29;
            // 
            // panelEndBottole_5
            // 
            this.panelEndBottole_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottole_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_5.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_5.Name = "panelEndBottole_5";
            this.panelEndBottole_5.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_5.TabIndex = 30;
            // 
            // dgvEndBottle_5
            // 
            this.dgvEndBottle_5.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_5.Name = "dgvEndBottle_5";
            this.dgvEndBottle_5.ReadOnly = true;
            this.dgvEndBottle_5.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_5.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_5.TabIndex = 31;
            // 
            // rdoBackwardBottle_5
            // 
            this.rdoBackwardBottle_5.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_5.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_5.Name = "rdoBackwardBottle_5";
            this.rdoBackwardBottle_5.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_5.TabIndex = 1;
            this.rdoBackwardBottle_5.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_5
            // 
            this.rdoForwardBottle_5.Checked = true;
            this.rdoForwardBottle_5.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_5.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_5.Name = "rdoForwardBottle_5";
            this.rdoForwardBottle_5.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_5.TabIndex = 0;
            this.rdoForwardBottle_5.TabStop = true;
            this.rdoForwardBottle_5.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_5
            // 
            this.lblBottoleOrderNo_5.AutoSize = true;
            this.lblBottoleOrderNo_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_5.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_5.Name = "lblBottoleOrderNo_5";
            this.lblBottoleOrderNo_5.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_5.TabIndex = 1;
            this.lblBottoleOrderNo_5.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_5
            // 
            this.txtBottoleOrderNo_5.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_5.Name = "txtBottoleOrderNo_5";
            this.txtBottoleOrderNo_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_5.TabIndex = 2;
            // 
            // txtBottoleItemName_5
            // 
            this.txtBottoleItemName_5.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_5.Name = "txtBottoleItemName_5";
            this.txtBottoleItemName_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_5.TabIndex = 4;
            // 
            // txtBottoleItemCode_5
            // 
            this.txtBottoleItemCode_5.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_5.Name = "txtBottoleItemCode_5";
            this.txtBottoleItemCode_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_5.TabIndex = 6;
            // 
            // txtBottoleLotNo_5
            // 
            this.txtBottoleLotNo_5.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_5.Name = "txtBottoleLotNo_5";
            this.txtBottoleLotNo_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_5.TabIndex = 8;
            // 
            // lblBottoleItemName_5
            // 
            this.lblBottoleItemName_5.AutoSize = true;
            this.lblBottoleItemName_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_5.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_5.Name = "lblBottoleItemName_5";
            this.lblBottoleItemName_5.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_5.TabIndex = 3;
            this.lblBottoleItemName_5.Text = "品目名";
            // 
            // lblBottleItemCode_5
            // 
            this.lblBottleItemCode_5.AutoSize = true;
            this.lblBottleItemCode_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_5.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_5.Name = "lblBottleItemCode_5";
            this.lblBottleItemCode_5.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_5.TabIndex = 5;
            this.lblBottleItemCode_5.Text = "品目コード";
            // 
            // lblBottleLotNo_5
            // 
            this.lblBottleLotNo_5.AutoSize = true;
            this.lblBottleLotNo_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_5.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_5.Name = "lblBottleLotNo_5";
            this.lblBottleLotNo_5.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_5.TabIndex = 7;
            this.lblBottleLotNo_5.Text = "ロット番号";
            // 
            // lblTargetBottole_5
            // 
            this.lblTargetBottole_5.AutoSize = true;
            this.lblTargetBottole_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_5.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_5.Name = "lblTargetBottole_5";
            this.lblTargetBottole_5.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_5.TabIndex = 9;
            this.lblTargetBottole_5.Text = "対象期間";
            // 
            // timeCheck_5
            // 
            this.timeCheck_5.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_5.Name = "timeCheck_5";
            this.timeCheck_5.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_5.TabIndex = 10;
            // 
            // startBottoleTime_5
            // 
            this.startBottoleTime_5.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_5.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_5.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_5.Name = "startBottoleTime_5";
            this.startBottoleTime_5.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_5.TabIndex = 11;
            // 
            // lblBottole_5
            // 
            this.lblBottole_5.AutoSize = true;
            this.lblBottole_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_5.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_5.Name = "lblBottole_5";
            this.lblBottole_5.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_5.TabIndex = 12;
            this.lblBottole_5.Text = "～";
            // 
            // endBottoleTime_5
            // 
            this.endBottoleTime_5.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_5.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_5.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_5.Name = "endBottoleTime_5";
            this.endBottoleTime_5.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_5.TabIndex = 14;
            // 
            // btnClearBottle_5
            // 
            this.btnClearBottle_5.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_5.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_5.Name = "btnClearBottle_5";
            this.btnClearBottle_5.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_5.TabIndex = 18;
            this.btnClearBottle_5.Text = "クリア";
            // 
            // btnCsvOutputBottle_5
            // 
            this.btnCsvOutputBottle_5.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_5.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_5.Name = "btnCsvOutputBottle_5";
            this.btnCsvOutputBottle_5.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_5.TabIndex = 19;
            this.btnCsvOutputBottle_5.Text = "CSV出力";
            // 
            // btnTraceSearch_5
            // 
            this.btnTraceSearch_5.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_5.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_5.Name = "btnTraceSearch_5";
            this.btnTraceSearch_5.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_5.TabIndex = 22;
            this.btnTraceSearch_5.Text = "トレース検索";
            // 
            // tabBottolePage6
            // 
            this.tabBottolePage6.Controls.Add(this.panelStartBottole_6);
            this.tabBottolePage6.Controls.Add(this.dgvStartBottle_6);
            this.tabBottolePage6.Controls.Add(this.panelEndBottole_6);
            this.tabBottolePage6.Controls.Add(this.dgvEndBottle_6);
            this.tabBottolePage6.Controls.Add(this.rdoBackwardBottle_6);
            this.tabBottolePage6.Controls.Add(this.rdoForwardBottle_6);
            this.tabBottolePage6.Controls.Add(this.lblBottoleOrderNo_6);
            this.tabBottolePage6.Controls.Add(this.txtBottoleOrderNo_6);
            this.tabBottolePage6.Controls.Add(this.txtBottoleItemName_6);
            this.tabBottolePage6.Controls.Add(this.txtBottoleItemCode_6);
            this.tabBottolePage6.Controls.Add(this.txtBottoleLotNo_6);
            this.tabBottolePage6.Controls.Add(this.lblBottoleItemName_6);
            this.tabBottolePage6.Controls.Add(this.lblBottleItemCode_6);
            this.tabBottolePage6.Controls.Add(this.lblBottleLotNo_6);
            this.tabBottolePage6.Controls.Add(this.lblTargetBottole_6);
            this.tabBottolePage6.Controls.Add(this.timeCheck_6);
            this.tabBottolePage6.Controls.Add(this.startBottoleTime_6);
            this.tabBottolePage6.Controls.Add(this.lblBottole_6);
            this.tabBottolePage6.Controls.Add(this.endBottoleTime_6);
            this.tabBottolePage6.Controls.Add(this.btnClearBottle_6);
            this.tabBottolePage6.Controls.Add(this.btnCsvOutputBottle_6);
            this.tabBottolePage6.Controls.Add(this.btnTraceSearch_6);
            this.tabBottolePage6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage6.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage6.Name = "tabBottolePage6";
            this.tabBottolePage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage6.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage6.TabIndex = 17;
            this.tabBottolePage6.Text = "(06)_未設定";
            this.tabBottolePage6.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_6
            // 
            this.panelStartBottole_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_6.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_6.Name = "panelStartBottole_6";
            this.panelStartBottole_6.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_6.TabIndex = 28;
            // 
            // dgvStartBottle_6
            // 
            this.dgvStartBottle_6.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_6.Name = "dgvStartBottle_6";
            this.dgvStartBottle_6.ReadOnly = true;
            this.dgvStartBottle_6.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_6.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_6.TabIndex = 29;
            // 
            // panelEndBottole_6
            // 
            this.panelEndBottole_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottole_6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_6.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_6.Name = "panelEndBottole_6";
            this.panelEndBottole_6.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_6.TabIndex = 30;
            // 
            // dgvEndBottle_6
            // 
            this.dgvEndBottle_6.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_6.Name = "dgvEndBottle_6";
            this.dgvEndBottle_6.ReadOnly = true;
            this.dgvEndBottle_6.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_6.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_6.TabIndex = 31;
            // 
            // rdoBackwardBottle_6
            // 
            this.rdoBackwardBottle_6.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_6.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_6.Name = "rdoBackwardBottle_6";
            this.rdoBackwardBottle_6.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_6.TabIndex = 1;
            this.rdoBackwardBottle_6.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_6
            // 
            this.rdoForwardBottle_6.Checked = true;
            this.rdoForwardBottle_6.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_6.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_6.Name = "rdoForwardBottle_6";
            this.rdoForwardBottle_6.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_6.TabIndex = 0;
            this.rdoForwardBottle_6.TabStop = true;
            this.rdoForwardBottle_6.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_6
            // 
            this.lblBottoleOrderNo_6.AutoSize = true;
            this.lblBottoleOrderNo_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_6.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_6.Name = "lblBottoleOrderNo_6";
            this.lblBottoleOrderNo_6.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_6.TabIndex = 1;
            this.lblBottoleOrderNo_6.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_6
            // 
            this.txtBottoleOrderNo_6.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_6.Name = "txtBottoleOrderNo_6";
            this.txtBottoleOrderNo_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_6.TabIndex = 2;
            // 
            // txtBottoleItemName_6
            // 
            this.txtBottoleItemName_6.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_6.Name = "txtBottoleItemName_6";
            this.txtBottoleItemName_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_6.TabIndex = 4;
            // 
            // txtBottoleItemCode_6
            // 
            this.txtBottoleItemCode_6.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_6.Name = "txtBottoleItemCode_6";
            this.txtBottoleItemCode_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_6.TabIndex = 6;
            // 
            // txtBottoleLotNo_6
            // 
            this.txtBottoleLotNo_6.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_6.Name = "txtBottoleLotNo_6";
            this.txtBottoleLotNo_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_6.TabIndex = 8;
            // 
            // lblBottoleItemName_6
            // 
            this.lblBottoleItemName_6.AutoSize = true;
            this.lblBottoleItemName_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_6.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_6.Name = "lblBottoleItemName_6";
            this.lblBottoleItemName_6.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_6.TabIndex = 3;
            this.lblBottoleItemName_6.Text = "品目名";
            // 
            // lblBottleItemCode_6
            // 
            this.lblBottleItemCode_6.AutoSize = true;
            this.lblBottleItemCode_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_6.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_6.Name = "lblBottleItemCode_6";
            this.lblBottleItemCode_6.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_6.TabIndex = 5;
            this.lblBottleItemCode_6.Text = "品目コード";
            // 
            // lblBottleLotNo_6
            // 
            this.lblBottleLotNo_6.AutoSize = true;
            this.lblBottleLotNo_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_6.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_6.Name = "lblBottleLotNo_6";
            this.lblBottleLotNo_6.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_6.TabIndex = 7;
            this.lblBottleLotNo_6.Text = "ロット番号";
            // 
            // lblTargetBottole_6
            // 
            this.lblTargetBottole_6.AutoSize = true;
            this.lblTargetBottole_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_6.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_6.Name = "lblTargetBottole_6";
            this.lblTargetBottole_6.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_6.TabIndex = 9;
            this.lblTargetBottole_6.Text = "対象期間";
            // 
            // timeCheck_6
            // 
            this.timeCheck_6.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_6.Name = "timeCheck_6";
            this.timeCheck_6.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_6.TabIndex = 10;
            // 
            // startBottoleTime_6
            // 
            this.startBottoleTime_6.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_6.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_6.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_6.Name = "startBottoleTime_6";
            this.startBottoleTime_6.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_6.TabIndex = 11;
            // 
            // lblBottole_6
            // 
            this.lblBottole_6.AutoSize = true;
            this.lblBottole_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_6.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_6.Name = "lblBottole_6";
            this.lblBottole_6.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_6.TabIndex = 12;
            this.lblBottole_6.Text = "～";
            // 
            // endBottoleTime_6
            // 
            this.endBottoleTime_6.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_6.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_6.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_6.Name = "endBottoleTime_6";
            this.endBottoleTime_6.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_6.TabIndex = 14;
            // 
            // btnClearBottle_6
            // 
            this.btnClearBottle_6.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_6.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_6.Name = "btnClearBottle_6";
            this.btnClearBottle_6.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_6.TabIndex = 18;
            this.btnClearBottle_6.Text = "クリア";
            // 
            // btnCsvOutputBottle_6
            // 
            this.btnCsvOutputBottle_6.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_6.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_6.Name = "btnCsvOutputBottle_6";
            this.btnCsvOutputBottle_6.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_6.TabIndex = 19;
            this.btnCsvOutputBottle_6.Text = "CSV出力";
            // 
            // btnTraceSearch_6
            // 
            this.btnTraceSearch_6.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_6.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_6.Name = "btnTraceSearch_6";
            this.btnTraceSearch_6.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_6.TabIndex = 22;
            this.btnTraceSearch_6.Text = "トレース検索";
            // 
            // tabBottolePage7
            // 
            this.tabBottolePage7.Controls.Add(this.panelStartBottole_7);
            this.tabBottolePage7.Controls.Add(this.dgvStartBottle_7);
            this.tabBottolePage7.Controls.Add(this.panelEndBottole_7);
            this.tabBottolePage7.Controls.Add(this.dgvEndBottle_7);
            this.tabBottolePage7.Controls.Add(this.rdoBackwardBottle_7);
            this.tabBottolePage7.Controls.Add(this.rdoForwardBottle_7);
            this.tabBottolePage7.Controls.Add(this.lblBottoleOrderNo_7);
            this.tabBottolePage7.Controls.Add(this.txtBottoleOrderNo_7);
            this.tabBottolePage7.Controls.Add(this.txtBottoleItemName_7);
            this.tabBottolePage7.Controls.Add(this.txtBottoleItemCode_7);
            this.tabBottolePage7.Controls.Add(this.txtBottoleLotNo_7);
            this.tabBottolePage7.Controls.Add(this.lblBottoleItemName_7);
            this.tabBottolePage7.Controls.Add(this.lblBottleItemCode_7);
            this.tabBottolePage7.Controls.Add(this.lblBottleLotNo_7);
            this.tabBottolePage7.Controls.Add(this.lblTargetBottole_7);
            this.tabBottolePage7.Controls.Add(this.timeCheck_7);
            this.tabBottolePage7.Controls.Add(this.startBottoleTime_7);
            this.tabBottolePage7.Controls.Add(this.lblBottole_7);
            this.tabBottolePage7.Controls.Add(this.endBottoleTime_7);
            this.tabBottolePage7.Controls.Add(this.btnClearBottle_7);
            this.tabBottolePage7.Controls.Add(this.btnCsvOutputBottle_7);
            this.tabBottolePage7.Controls.Add(this.btnTraceSearch_7);
            this.tabBottolePage7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage7.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage7.Name = "tabBottolePage7";
            this.tabBottolePage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage7.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage7.TabIndex = 18;
            this.tabBottolePage7.Text = "(07)_未設定";
            this.tabBottolePage7.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_7
            // 
            this.panelStartBottole_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_7.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_7.Name = "panelStartBottole_7";
            this.panelStartBottole_7.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_7.TabIndex = 28;
            // 
            // dgvStartBottle_7
            // 
            this.dgvStartBottle_7.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_7.Name = "dgvStartBottle_7";
            this.dgvStartBottle_7.ReadOnly = true;
            this.dgvStartBottle_7.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_7.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_7.TabIndex = 29;
            // 
            // panelEndBottole_7
            // 
            this.panelEndBottole_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panelEndBottole_7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_7.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_7.Name = "panelEndBottole_7";
            this.panelEndBottole_7.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_7.TabIndex = 30;
            // 
            // dgvEndBottle_7
            // 
            this.dgvEndBottle_7.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_7.Name = "dgvEndBottle_7";
            this.dgvEndBottle_7.ReadOnly = true;
            this.dgvEndBottle_7.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_7.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_7.TabIndex = 31;
            // 
            // rdoBackwardBottle_7
            // 
            this.rdoBackwardBottle_7.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_7.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_7.Name = "rdoBackwardBottle_7";
            this.rdoBackwardBottle_7.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_7.TabIndex = 1;
            this.rdoBackwardBottle_7.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_7
            // 
            this.rdoForwardBottle_7.Checked = true;
            this.rdoForwardBottle_7.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_7.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_7.Name = "rdoForwardBottle_7";
            this.rdoForwardBottle_7.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_7.TabIndex = 0;
            this.rdoForwardBottle_7.TabStop = true;
            this.rdoForwardBottle_7.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_7
            // 
            this.lblBottoleOrderNo_7.AutoSize = true;
            this.lblBottoleOrderNo_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_7.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_7.Name = "lblBottoleOrderNo_7";
            this.lblBottoleOrderNo_7.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_7.TabIndex = 1;
            this.lblBottoleOrderNo_7.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_7
            // 
            this.txtBottoleOrderNo_7.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_7.Name = "txtBottoleOrderNo_7";
            this.txtBottoleOrderNo_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_7.TabIndex = 2;
            // 
            // txtBottoleItemName_7
            // 
            this.txtBottoleItemName_7.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_7.Name = "txtBottoleItemName_7";
            this.txtBottoleItemName_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_7.TabIndex = 4;
            // 
            // txtBottoleItemCode_7
            // 
            this.txtBottoleItemCode_7.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_7.Name = "txtBottoleItemCode_7";
            this.txtBottoleItemCode_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_7.TabIndex = 6;
            // 
            // txtBottoleLotNo_7
            // 
            this.txtBottoleLotNo_7.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_7.Name = "txtBottoleLotNo_7";
            this.txtBottoleLotNo_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_7.TabIndex = 8;
            // 
            // lblBottoleItemName_7
            // 
            this.lblBottoleItemName_7.AutoSize = true;
            this.lblBottoleItemName_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_7.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_7.Name = "lblBottoleItemName_7";
            this.lblBottoleItemName_7.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_7.TabIndex = 3;
            this.lblBottoleItemName_7.Text = "品目名";
            // 
            // lblBottleItemCode_7
            // 
            this.lblBottleItemCode_7.AutoSize = true;
            this.lblBottleItemCode_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_7.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_7.Name = "lblBottleItemCode_7";
            this.lblBottleItemCode_7.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_7.TabIndex = 5;
            this.lblBottleItemCode_7.Text = "品目コード";
            // 
            // lblBottleLotNo_7
            // 
            this.lblBottleLotNo_7.AutoSize = true;
            this.lblBottleLotNo_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_7.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_7.Name = "lblBottleLotNo_7";
            this.lblBottleLotNo_7.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_7.TabIndex = 7;
            this.lblBottleLotNo_7.Text = "ロット番号";
            // 
            // lblTargetBottole_7
            // 
            this.lblTargetBottole_7.AutoSize = true;
            this.lblTargetBottole_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_7.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_7.Name = "lblTargetBottole_7";
            this.lblTargetBottole_7.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_7.TabIndex = 9;
            this.lblTargetBottole_7.Text = "対象期間";
            // 
            // timeCheck_7
            // 
            this.timeCheck_7.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_7.Name = "timeCheck_7";
            this.timeCheck_7.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_7.TabIndex = 10;
            // 
            // startBottoleTime_7
            // 
            this.startBottoleTime_7.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_7.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_7.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_7.Name = "startBottoleTime_7";
            this.startBottoleTime_7.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_7.TabIndex = 11;
            // 
            // lblBottole_7
            // 
            this.lblBottole_7.AutoSize = true;
            this.lblBottole_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_7.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_7.Name = "lblBottole_7";
            this.lblBottole_7.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_7.TabIndex = 12;
            this.lblBottole_7.Text = "～";
            // 
            // endBottoleTime_7
            // 
            this.endBottoleTime_7.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_7.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_7.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_7.Name = "endBottoleTime_7";
            this.endBottoleTime_7.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_7.TabIndex = 14;
            // 
            // btnClearBottle_7
            // 
            this.btnClearBottle_7.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_7.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_7.Name = "btnClearBottle_7";
            this.btnClearBottle_7.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_7.TabIndex = 18;
            this.btnClearBottle_7.Text = "クリア";
            // 
            // btnCsvOutputBottle_7
            // 
            this.btnCsvOutputBottle_7.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_7.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_7.Name = "btnCsvOutputBottle_7";
            this.btnCsvOutputBottle_7.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_7.TabIndex = 19;
            this.btnCsvOutputBottle_7.Text = "CSV出力";
            // 
            // btnTraceSearch_7
            // 
            this.btnTraceSearch_7.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_7.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_7.Name = "btnTraceSearch_7";
            this.btnTraceSearch_7.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_7.TabIndex = 22;
            this.btnTraceSearch_7.Text = "トレース検索";
            // 
            // tabBottolePage8
            // 
            this.tabBottolePage8.Controls.Add(this.panelStartBottole_8);
            this.tabBottolePage8.Controls.Add(this.dgvStartBottle_8);
            this.tabBottolePage8.Controls.Add(this.panelEndBottole_8);
            this.tabBottolePage8.Controls.Add(this.dgvEndBottle_8);
            this.tabBottolePage8.Controls.Add(this.rdoBackwardBottle_8);
            this.tabBottolePage8.Controls.Add(this.rdoForwardBottle_8);
            this.tabBottolePage8.Controls.Add(this.lblBottoleOrderNo_8);
            this.tabBottolePage8.Controls.Add(this.txtBottoleOrderNo_8);
            this.tabBottolePage8.Controls.Add(this.txtBottoleItemName_8);
            this.tabBottolePage8.Controls.Add(this.txtBottoleItemCode_8);
            this.tabBottolePage8.Controls.Add(this.txtBottoleLotNo_8);
            this.tabBottolePage8.Controls.Add(this.lblBottoleItemName_8);
            this.tabBottolePage8.Controls.Add(this.lblBottleItemCode_8);
            this.tabBottolePage8.Controls.Add(this.lblBottleLotNo_8);
            this.tabBottolePage8.Controls.Add(this.lblTargetBottole_8);
            this.tabBottolePage8.Controls.Add(this.timeCheck_8);
            this.tabBottolePage8.Controls.Add(this.startBottoleTime_8);
            this.tabBottolePage8.Controls.Add(this.lblBottole_8);
            this.tabBottolePage8.Controls.Add(this.endBottoleTime_8);
            this.tabBottolePage8.Controls.Add(this.btnClearBottle_8);
            this.tabBottolePage8.Controls.Add(this.btnCsvOutputBottle_8);
            this.tabBottolePage8.Controls.Add(this.btnTraceSearch_8);
            this.tabBottolePage8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage8.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage8.Name = "tabBottolePage8";
            this.tabBottolePage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage8.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage8.TabIndex = 19;
            this.tabBottolePage8.Text = "(08)_未設定";
            this.tabBottolePage8.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_8
            // 
            this.panelStartBottole_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_8.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_8.Name = "panelStartBottole_8";
            this.panelStartBottole_8.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_8.TabIndex = 28;
            // 
            // dgvStartBottle_8
            // 
            this.dgvStartBottle_8.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_8.Name = "dgvStartBottle_8";
            this.dgvStartBottle_8.ReadOnly = true;
            this.dgvStartBottle_8.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_8.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_8.TabIndex = 29;
            // 
            // panelEndBottole_8
            // 
            this.panelEndBottole_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panelEndBottole_8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_8.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_8.Name = "panelEndBottole_8";
            this.panelEndBottole_8.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_8.TabIndex = 30;
            // 
            // dgvEndBottle_8
            // 
            this.dgvEndBottle_8.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_8.Name = "dgvEndBottle_8";
            this.dgvEndBottle_8.ReadOnly = true;
            this.dgvEndBottle_8.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_8.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_8.TabIndex = 31;
            // 
            // rdoBackwardBottle_8
            // 
            this.rdoBackwardBottle_8.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_8.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_8.Name = "rdoBackwardBottle_8";
            this.rdoBackwardBottle_8.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_8.TabIndex = 1;
            this.rdoBackwardBottle_8.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_8
            // 
            this.rdoForwardBottle_8.Checked = true;
            this.rdoForwardBottle_8.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_8.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_8.Name = "rdoForwardBottle_8";
            this.rdoForwardBottle_8.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_8.TabIndex = 0;
            this.rdoForwardBottle_8.TabStop = true;
            this.rdoForwardBottle_8.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_8
            // 
            this.lblBottoleOrderNo_8.AutoSize = true;
            this.lblBottoleOrderNo_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_8.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_8.Name = "lblBottoleOrderNo_8";
            this.lblBottoleOrderNo_8.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_8.TabIndex = 1;
            this.lblBottoleOrderNo_8.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_8
            // 
            this.txtBottoleOrderNo_8.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_8.Name = "txtBottoleOrderNo_8";
            this.txtBottoleOrderNo_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_8.TabIndex = 2;
            // 
            // txtBottoleItemName_8
            // 
            this.txtBottoleItemName_8.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_8.Name = "txtBottoleItemName_8";
            this.txtBottoleItemName_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_8.TabIndex = 4;
            // 
            // txtBottoleItemCode_8
            // 
            this.txtBottoleItemCode_8.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_8.Name = "txtBottoleItemCode_8";
            this.txtBottoleItemCode_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_8.TabIndex = 6;
            // 
            // txtBottoleLotNo_8
            // 
            this.txtBottoleLotNo_8.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_8.Name = "txtBottoleLotNo_8";
            this.txtBottoleLotNo_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_8.TabIndex = 8;
            // 
            // lblBottoleItemName_8
            // 
            this.lblBottoleItemName_8.AutoSize = true;
            this.lblBottoleItemName_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_8.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_8.Name = "lblBottoleItemName_8";
            this.lblBottoleItemName_8.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_8.TabIndex = 3;
            this.lblBottoleItemName_8.Text = "品目名";
            // 
            // lblBottleItemCode_8
            // 
            this.lblBottleItemCode_8.AutoSize = true;
            this.lblBottleItemCode_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_8.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_8.Name = "lblBottleItemCode_8";
            this.lblBottleItemCode_8.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_8.TabIndex = 5;
            this.lblBottleItemCode_8.Text = "品目コード";
            // 
            // lblBottleLotNo_8
            // 
            this.lblBottleLotNo_8.AutoSize = true;
            this.lblBottleLotNo_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_8.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_8.Name = "lblBottleLotNo_8";
            this.lblBottleLotNo_8.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_8.TabIndex = 7;
            this.lblBottleLotNo_8.Text = "ロット番号";
            // 
            // lblTargetBottole_8
            // 
            this.lblTargetBottole_8.AutoSize = true;
            this.lblTargetBottole_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_8.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_8.Name = "lblTargetBottole_8";
            this.lblTargetBottole_8.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_8.TabIndex = 9;
            this.lblTargetBottole_8.Text = "対象期間";
            // 
            // timeCheck_8
            // 
            this.timeCheck_8.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_8.Name = "timeCheck_8";
            this.timeCheck_8.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_8.TabIndex = 10;
            // 
            // startBottoleTime_8
            // 
            this.startBottoleTime_8.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_8.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_8.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_8.Name = "startBottoleTime_8";
            this.startBottoleTime_8.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_8.TabIndex = 11;
            // 
            // lblBottole_8
            // 
            this.lblBottole_8.AutoSize = true;
            this.lblBottole_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_8.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_8.Name = "lblBottole_8";
            this.lblBottole_8.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_8.TabIndex = 12;
            this.lblBottole_8.Text = "～";
            // 
            // endBottoleTime_8
            // 
            this.endBottoleTime_8.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_8.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_8.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_8.Name = "endBottoleTime_8";
            this.endBottoleTime_8.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_8.TabIndex = 14;
            // 
            // btnClearBottle_8
            // 
            this.btnClearBottle_8.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_8.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_8.Name = "btnClearBottle_8";
            this.btnClearBottle_8.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_8.TabIndex = 18;
            this.btnClearBottle_8.Text = "クリア";
            // 
            // btnCsvOutputBottle_8
            // 
            this.btnCsvOutputBottle_8.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_8.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_8.Name = "btnCsvOutputBottle_8";
            this.btnCsvOutputBottle_8.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_8.TabIndex = 19;
            this.btnCsvOutputBottle_8.Text = "CSV出力";
            // 
            // btnTraceSearch_8
            // 
            this.btnTraceSearch_8.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_8.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_8.Name = "btnTraceSearch_8";
            this.btnTraceSearch_8.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_8.TabIndex = 22;
            this.btnTraceSearch_8.Text = "トレース検索";
            // 
            // tabBottolePage9
            // 
            this.tabBottolePage9.Controls.Add(this.panelStartBottole_9);
            this.tabBottolePage9.Controls.Add(this.dgvStartBottle_9);
            this.tabBottolePage9.Controls.Add(this.panelEndBottole_9);
            this.tabBottolePage9.Controls.Add(this.dgvEndBottle_9);
            this.tabBottolePage9.Controls.Add(this.rdoBackwardBottle_9);
            this.tabBottolePage9.Controls.Add(this.rdoForwardBottle_9);
            this.tabBottolePage9.Controls.Add(this.lblBottoleOrderNo_9);
            this.tabBottolePage9.Controls.Add(this.txtBottoleOrderNo_9);
            this.tabBottolePage9.Controls.Add(this.txtBottoleItemName_9);
            this.tabBottolePage9.Controls.Add(this.txtBottoleItemCode_9);
            this.tabBottolePage9.Controls.Add(this.txtBottoleLotNo_9);
            this.tabBottolePage9.Controls.Add(this.lblBottoleItemName_9);
            this.tabBottolePage9.Controls.Add(this.lblBottleItemCode_9);
            this.tabBottolePage9.Controls.Add(this.lblBottleLotNo_9);
            this.tabBottolePage9.Controls.Add(this.lblTargetBottole_9);
            this.tabBottolePage9.Controls.Add(this.timeCheck_9);
            this.tabBottolePage9.Controls.Add(this.startBottoleTime_9);
            this.tabBottolePage9.Controls.Add(this.lblBottole_9);
            this.tabBottolePage9.Controls.Add(this.endBottoleTime_9);
            this.tabBottolePage9.Controls.Add(this.btnClearBottle_9);
            this.tabBottolePage9.Controls.Add(this.btnCsvOutputBottle_9);
            this.tabBottolePage9.Controls.Add(this.btnTraceSearch_9);
            this.tabBottolePage9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage9.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage9.Name = "tabBottolePage9";
            this.tabBottolePage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage9.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage9.TabIndex = 20;
            this.tabBottolePage9.Text = "(09)_未設定";
            this.tabBottolePage9.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_9
            // 
            this.panelStartBottole_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_9.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_9.Name = "panelStartBottole_9";
            this.panelStartBottole_9.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_9.TabIndex = 28;
            // 
            // dgvStartBottle_9
            // 
            this.dgvStartBottle_9.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_9.Name = "dgvStartBottle_9";
            this.dgvStartBottle_9.ReadOnly = true;
            this.dgvStartBottle_9.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_9.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_9.TabIndex = 29;
            // 
            // panelEndBottole_9
            // 
            this.panelEndBottole_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panelEndBottole_9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_9.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_9.Name = "panelEndBottole_9";
            this.panelEndBottole_9.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_9.TabIndex = 30;
            // 
            // dgvEndBottle_9
            // 
            this.dgvEndBottle_9.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_9.Name = "dgvEndBottle_9";
            this.dgvEndBottle_9.ReadOnly = true;
            this.dgvEndBottle_9.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_9.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_9.TabIndex = 31;
            // 
            // rdoBackwardBottle_9
            // 
            this.rdoBackwardBottle_9.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackwardBottle_9.Location = new System.Drawing.Point(540, 55);
            this.rdoBackwardBottle_9.Name = "rdoBackwardBottle_9";
            this.rdoBackwardBottle_9.Size = new System.Drawing.Size(220, 30);
            this.rdoBackwardBottle_9.TabIndex = 1;
            this.rdoBackwardBottle_9.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_9
            // 
            this.rdoForwardBottle_9.Checked = true;
            this.rdoForwardBottle_9.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_9.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_9.Name = "rdoForwardBottle_9";
            this.rdoForwardBottle_9.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_9.TabIndex = 0;
            this.rdoForwardBottle_9.TabStop = true;
            this.rdoForwardBottle_9.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_9
            // 
            this.lblBottoleOrderNo_9.AutoSize = true;
            this.lblBottoleOrderNo_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_9.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_9.Name = "lblBottoleOrderNo_9";
            this.lblBottoleOrderNo_9.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_9.TabIndex = 1;
            this.lblBottoleOrderNo_9.Text = "製造指図番号";
            // 
            // txtBottoleOrderNo_9
            // 
            this.txtBottoleOrderNo_9.Location = new System.Drawing.Point(110, 15);
            this.txtBottoleOrderNo_9.Name = "txtBottoleOrderNo_9";
            this.txtBottoleOrderNo_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleOrderNo_9.TabIndex = 2;
            // 
            // txtBottoleItemName_9
            // 
            this.txtBottoleItemName_9.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_9.Name = "txtBottoleItemName_9";
            this.txtBottoleItemName_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_9.TabIndex = 4;
            // 
            // txtBottoleItemCode_9
            // 
            this.txtBottoleItemCode_9.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_9.Name = "txtBottoleItemCode_9";
            this.txtBottoleItemCode_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_9.TabIndex = 6;
            // 
            // txtBottoleLotNo_9
            // 
            this.txtBottoleLotNo_9.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_9.Name = "txtBottoleLotNo_9";
            this.txtBottoleLotNo_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_9.TabIndex = 8;
            // 
            // lblBottoleItemName_9
            // 
            this.lblBottoleItemName_9.AutoSize = true;
            this.lblBottoleItemName_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_9.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_9.Name = "lblBottoleItemName_9";
            this.lblBottoleItemName_9.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_9.TabIndex = 3;
            this.lblBottoleItemName_9.Text = "品目名";
            // 
            // lblBottleItemCode_9
            // 
            this.lblBottleItemCode_9.AutoSize = true;
            this.lblBottleItemCode_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_9.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_9.Name = "lblBottleItemCode_9";
            this.lblBottleItemCode_9.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_9.TabIndex = 5;
            this.lblBottleItemCode_9.Text = "品目コード";
            // 
            // lblBottleLotNo_9
            // 
            this.lblBottleLotNo_9.AutoSize = true;
            this.lblBottleLotNo_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_9.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_9.Name = "lblBottleLotNo_9";
            this.lblBottleLotNo_9.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_9.TabIndex = 7;
            this.lblBottleLotNo_9.Text = "ロット番号";
            // 
            // lblTargetBottole_9
            // 
            this.lblTargetBottole_9.AutoSize = true;
            this.lblTargetBottole_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_9.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_9.Name = "lblTargetBottole_9";
            this.lblTargetBottole_9.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_9.TabIndex = 9;
            this.lblTargetBottole_9.Text = "対象期間";
            // 
            // timeCheck_9
            // 
            this.timeCheck_9.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_9.Name = "timeCheck_9";
            this.timeCheck_9.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_9.TabIndex = 10;
            // 
            // startBottoleTime_9
            // 
            this.startBottoleTime_9.CustomFormat = "yyyy/MM/dd";
            this.startBottoleTime_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottoleTime_9.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottoleTime_9.Location = new System.Drawing.Point(110, 155);
            this.startBottoleTime_9.Name = "startBottoleTime_9";
            this.startBottoleTime_9.Size = new System.Drawing.Size(110, 27);
            this.startBottoleTime_9.TabIndex = 11;
            // 
            // lblBottole_9
            // 
            this.lblBottole_9.AutoSize = true;
            this.lblBottole_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottole_9.Location = new System.Drawing.Point(226, 162);
            this.lblBottole_9.Name = "lblBottole_9";
            this.lblBottole_9.Size = new System.Drawing.Size(19, 16);
            this.lblBottole_9.TabIndex = 12;
            this.lblBottole_9.Text = "～";
            // 
            // endBottoleTime_9
            // 
            this.endBottoleTime_9.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_9.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_9.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_9.Name = "endBottoleTime_9";
            this.endBottoleTime_9.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_9.TabIndex = 14;
            // 
            // btnClearBottle_9
            // 
            this.btnClearBottle_9.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_9.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_9.Name = "btnClearBottle_9";
            this.btnClearBottle_9.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_9.TabIndex = 18;
            this.btnClearBottle_9.Text = "クリア";
            // 
            // btnCsvOutputBottle_9
            // 
            this.btnCsvOutputBottle_9.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_9.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_9.Name = "btnCsvOutputBottle_9";
            this.btnCsvOutputBottle_9.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_9.TabIndex = 19;
            this.btnCsvOutputBottle_9.Text = "CSV出力";
            // 
            // btnTraceSearch_9
            // 
            this.btnTraceSearch_9.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_9.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_9.Name = "btnTraceSearch_9";
            this.btnTraceSearch_9.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_9.TabIndex = 22;
            this.btnTraceSearch_9.Text = "トレース検索";
            // 
            // tabBottolePage10
            // 
            this.tabBottolePage10.Controls.Add(this.panelStartBottole_10);
            this.tabBottolePage10.Controls.Add(this.dgvStartBottle_10);
            this.tabBottolePage10.Controls.Add(this.panelEndBottole_10);
            this.tabBottolePage10.Controls.Add(this.dgvEndBottle_10);
            this.tabBottolePage10.Controls.Add(this.radioButton5);
            this.tabBottolePage10.Controls.Add(this.rdoForwardBottle_10);
            this.tabBottolePage10.Controls.Add(this.lblBottoleOrderNo_10);
            this.tabBottolePage10.Controls.Add(this.textBox10);
            this.tabBottolePage10.Controls.Add(this.txtBottoleItemName_10);
            this.tabBottolePage10.Controls.Add(this.txtBottoleItemCode_10);
            this.tabBottolePage10.Controls.Add(this.txtBottoleLotNo_10);
            this.tabBottolePage10.Controls.Add(this.lblBottoleItemName_10);
            this.tabBottolePage10.Controls.Add(this.lblBottleItemCode_10);
            this.tabBottolePage10.Controls.Add(this.lblBottleLotNo_10);
            this.tabBottolePage10.Controls.Add(this.lblTargetBottole_10);
            this.tabBottolePage10.Controls.Add(this.timeCheck_10);
            this.tabBottolePage10.Controls.Add(this.dateTimePicker5);
            this.tabBottolePage10.Controls.Add(this.label18);
            this.tabBottolePage10.Controls.Add(this.endBottoleTime_10);
            this.tabBottolePage10.Controls.Add(this.btnClearBottle_10);
            this.tabBottolePage10.Controls.Add(this.btnCsvOutputBottle_10);
            this.tabBottolePage10.Controls.Add(this.btnTraceSearch_10);
            this.tabBottolePage10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottolePage10.Location = new System.Drawing.Point(4, 34);
            this.tabBottolePage10.Name = "tabBottolePage10";
            this.tabBottolePage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottolePage10.Size = new System.Drawing.Size(1896, 902);
            this.tabBottolePage10.TabIndex = 21;
            this.tabBottolePage10.Text = "(10)_未設定";
            this.tabBottolePage10.UseVisualStyleBackColor = true;
            // 
            // panelStartBottole_10
            // 
            this.panelStartBottole_10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottole_10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottole_10.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottole_10.Name = "panelStartBottole_10";
            this.panelStartBottole_10.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottole_10.TabIndex = 28;
            // 
            // dgvStartBottle_10
            // 
            this.dgvStartBottle_10.Location = new System.Drawing.Point(10, 222);
            this.dgvStartBottle_10.Name = "dgvStartBottle_10";
            this.dgvStartBottle_10.ReadOnly = true;
            this.dgvStartBottle_10.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle_10.Size = new System.Drawing.Size(920, 650);
            this.dgvStartBottle_10.TabIndex = 29;
            // 
            // panelEndBottole_10
            // 
            this.panelEndBottole_10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottole_10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottole_10.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottole_10.Name = "panelEndBottole_10";
            this.panelEndBottole_10.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottole_10.TabIndex = 30;
            // 
            // dgvEndBottle_10
            // 
            this.dgvEndBottle_10.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle_10.Name = "dgvEndBottle_10";
            this.dgvEndBottle_10.ReadOnly = true;
            this.dgvEndBottle_10.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle_10.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle_10.TabIndex = 31;
            // 
            // radioButton5
            // 
            this.radioButton5.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton5.Location = new System.Drawing.Point(540, 55);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(220, 30);
            this.radioButton5.TabIndex = 1;
            this.radioButton5.Text = "トレースバック(遡及)";
            // 
            // rdoForwardBottle_10
            // 
            this.rdoForwardBottle_10.Checked = true;
            this.rdoForwardBottle_10.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForwardBottle_10.Location = new System.Drawing.Point(540, 20);
            this.rdoForwardBottle_10.Name = "rdoForwardBottle_10";
            this.rdoForwardBottle_10.Size = new System.Drawing.Size(220, 30);
            this.rdoForwardBottle_10.TabIndex = 0;
            this.rdoForwardBottle_10.TabStop = true;
            this.rdoForwardBottle_10.Text = "トレースフォワード(追跡)";
            // 
            // lblBottoleOrderNo_10
            // 
            this.lblBottoleOrderNo_10.AutoSize = true;
            this.lblBottoleOrderNo_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleOrderNo_10.Location = new System.Drawing.Point(20, 20);
            this.lblBottoleOrderNo_10.Name = "lblBottoleOrderNo_10";
            this.lblBottoleOrderNo_10.Size = new System.Drawing.Size(79, 16);
            this.lblBottoleOrderNo_10.TabIndex = 1;
            this.lblBottoleOrderNo_10.Text = "製造指図番号";
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(110, 15);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(250, 27);
            this.textBox10.TabIndex = 2;
            // 
            // txtBottoleItemName_10
            // 
            this.txtBottoleItemName_10.Location = new System.Drawing.Point(110, 50);
            this.txtBottoleItemName_10.Name = "txtBottoleItemName_10";
            this.txtBottoleItemName_10.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemName_10.TabIndex = 4;
            // 
            // txtBottoleItemCode_10
            // 
            this.txtBottoleItemCode_10.Location = new System.Drawing.Point(110, 85);
            this.txtBottoleItemCode_10.Name = "txtBottoleItemCode_10";
            this.txtBottoleItemCode_10.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleItemCode_10.TabIndex = 6;
            // 
            // txtBottoleLotNo_10
            // 
            this.txtBottoleLotNo_10.Location = new System.Drawing.Point(110, 120);
            this.txtBottoleLotNo_10.Name = "txtBottoleLotNo_10";
            this.txtBottoleLotNo_10.Size = new System.Drawing.Size(250, 27);
            this.txtBottoleLotNo_10.TabIndex = 8;
            // 
            // lblBottoleItemName_10
            // 
            this.lblBottoleItemName_10.AutoSize = true;
            this.lblBottoleItemName_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottoleItemName_10.Location = new System.Drawing.Point(20, 55);
            this.lblBottoleItemName_10.Name = "lblBottoleItemName_10";
            this.lblBottoleItemName_10.Size = new System.Drawing.Size(43, 16);
            this.lblBottoleItemName_10.TabIndex = 3;
            this.lblBottoleItemName_10.Text = "品目名";
            // 
            // lblBottleItemCode_10
            // 
            this.lblBottleItemCode_10.AutoSize = true;
            this.lblBottleItemCode_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemCode_10.Location = new System.Drawing.Point(20, 90);
            this.lblBottleItemCode_10.Name = "lblBottleItemCode_10";
            this.lblBottleItemCode_10.Size = new System.Drawing.Size(67, 16);
            this.lblBottleItemCode_10.TabIndex = 5;
            this.lblBottleItemCode_10.Text = "品目コード";
            // 
            // lblBottleLotNo_10
            // 
            this.lblBottleLotNo_10.AutoSize = true;
            this.lblBottleLotNo_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleLotNo_10.Location = new System.Drawing.Point(20, 125);
            this.lblBottleLotNo_10.Name = "lblBottleLotNo_10";
            this.lblBottleLotNo_10.Size = new System.Drawing.Size(67, 16);
            this.lblBottleLotNo_10.TabIndex = 7;
            this.lblBottleLotNo_10.Text = "ロット番号";
            // 
            // lblTargetBottole_10
            // 
            this.lblTargetBottole_10.AutoSize = true;
            this.lblTargetBottole_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottole_10.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottole_10.Name = "lblTargetBottole_10";
            this.lblTargetBottole_10.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottole_10.TabIndex = 9;
            this.lblTargetBottole_10.Text = "対象期間";
            // 
            // timeCheck_10
            // 
            this.timeCheck_10.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_10.Name = "timeCheck_10";
            this.timeCheck_10.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_10.TabIndex = 10;
            // 
            // dateTimePicker5
            // 
            this.dateTimePicker5.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker5.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker5.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker5.Name = "dateTimePicker5";
            this.dateTimePicker5.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker5.TabIndex = 11;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label18.Location = new System.Drawing.Point(226, 162);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(19, 16);
            this.label18.TabIndex = 12;
            this.label18.Text = "～";
            // 
            // endBottoleTime_10
            // 
            this.endBottoleTime_10.CustomFormat = "yyyy/MM/dd";
            this.endBottoleTime_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottoleTime_10.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottoleTime_10.Location = new System.Drawing.Point(251, 155);
            this.endBottoleTime_10.Name = "endBottoleTime_10";
            this.endBottoleTime_10.Size = new System.Drawing.Size(110, 27);
            this.endBottoleTime_10.TabIndex = 14;
            // 
            // btnClearBottle_10
            // 
            this.btnClearBottle_10.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_10.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_10.Name = "btnClearBottle_10";
            this.btnClearBottle_10.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_10.TabIndex = 18;
            this.btnClearBottle_10.Text = "クリア";
            // 
            // btnCsvOutputBottle_10
            // 
            this.btnCsvOutputBottle_10.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_10.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_10.Name = "btnCsvOutputBottle_10";
            this.btnCsvOutputBottle_10.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_10.TabIndex = 19;
            this.btnCsvOutputBottle_10.Text = "CSV出力";
            // 
            // btnTraceSearch_10
            // 
            this.btnTraceSearch_10.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch_10.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch_10.Name = "btnTraceSearch_10";
            this.btnTraceSearch_10.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch_10.TabIndex = 22;
            this.btnTraceSearch_10.Text = "トレース検索";
            // 
            // bottoleTitle
            // 
            this.bottoleTitle.AutoSize = true;
            this.bottoleTitle.Font = new System.Drawing.Font("游ゴシック", 20F);
            this.bottoleTitle.Location = new System.Drawing.Point(785, 8);
            this.bottoleTitle.Name = "bottoleTitle";
            this.bottoleTitle.Size = new System.Drawing.Size(285, 35);
            this.bottoleTitle.TabIndex = 50;
            this.bottoleTitle.Text = "瓶設備ロットトレース";
            // 
            // rdoBottoleTabNameItemCode
            // 
            this.rdoBottoleTabNameItemCode.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.rdoBottoleTabNameItemCode.Location = new System.Drawing.Point(1389, 31);
            this.rdoBottoleTabNameItemCode.Name = "rdoBottoleTabNameItemCode";
            this.rdoBottoleTabNameItemCode.Size = new System.Drawing.Size(110, 24);
            this.rdoBottoleTabNameItemCode.TabIndex = 53;
            this.rdoBottoleTabNameItemCode.Text = "品目コード";
            // 
            // selectBottole
            // 
            this.selectBottole.AutoSize = true;
            this.selectBottole.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.selectBottole.Location = new System.Drawing.Point(1265, 10);
            this.selectBottole.Name = "selectBottole";
            this.selectBottole.Size = new System.Drawing.Size(106, 21);
            this.selectBottole.TabIndex = 52;
            this.selectBottole.Text = "タブ名称選択";
            // 
            // rdoBottoleTabNameOrder
            // 
            this.rdoBottoleTabNameOrder.Checked = true;
            this.rdoBottoleTabNameOrder.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.rdoBottoleTabNameOrder.Location = new System.Drawing.Point(1389, 7);
            this.rdoBottoleTabNameOrder.Name = "rdoBottoleTabNameOrder";
            this.rdoBottoleTabNameOrder.Size = new System.Drawing.Size(110, 24);
            this.rdoBottoleTabNameOrder.TabIndex = 51;
            this.rdoBottoleTabNameOrder.TabStop = true;
            this.rdoBottoleTabNameOrder.Text = "製造指図番号";
            // 
            // btnBottoleExcelOutput
            // 
            this.btnBottoleExcelOutput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBottoleExcelOutput.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBottoleExcelOutput.Location = new System.Drawing.Point(1560, 15);
            this.btnBottoleExcelOutput.Name = "btnBottoleExcelOutput";
            this.btnBottoleExcelOutput.Size = new System.Drawing.Size(150, 50);
            this.btnBottoleExcelOutput.TabIndex = 54;
            this.btnBottoleExcelOutput.Text = "EXCEL出力";
            // 
            // btnBottoleDetectCrossPoints
            // 
            this.btnBottoleDetectCrossPoints.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBottoleDetectCrossPoints.Location = new System.Drawing.Point(1732, 15);
            this.btnBottoleDetectCrossPoints.Name = "btnBottoleDetectCrossPoints";
            this.btnBottoleDetectCrossPoints.Size = new System.Drawing.Size(150, 50);
            this.btnBottoleDetectCrossPoints.TabIndex = 55;
            this.btnBottoleDetectCrossPoints.Text = "交点検出";
            // 
            // checkBottole1
            // 
            this.checkBottole1.AutoSize = true;
            this.checkBottole1.Location = new System.Drawing.Point(12, 71);
            this.checkBottole1.Name = "checkBottole1";
            this.checkBottole1.Size = new System.Drawing.Size(15, 14);
            this.checkBottole1.TabIndex = 56;
            this.checkBottole1.UseVisualStyleBackColor = true;
            // 
            // checkBottole2
            // 
            this.checkBottole2.AutoSize = true;
            this.checkBottole2.Location = new System.Drawing.Point(184, 71);
            this.checkBottole2.Name = "checkBottole2";
            this.checkBottole2.Size = new System.Drawing.Size(15, 14);
            this.checkBottole2.TabIndex = 56;
            this.checkBottole2.UseVisualStyleBackColor = true;
            // 
            // checkBottole3
            // 
            this.checkBottole3.AutoSize = true;
            this.checkBottole3.Location = new System.Drawing.Point(358, 71);
            this.checkBottole3.Name = "checkBottole3";
            this.checkBottole3.Size = new System.Drawing.Size(15, 14);
            this.checkBottole3.TabIndex = 56;
            this.checkBottole3.UseVisualStyleBackColor = true;
            // 
            // checkBottole4
            // 
            this.checkBottole4.AutoSize = true;
            this.checkBottole4.Location = new System.Drawing.Point(527, 71);
            this.checkBottole4.Name = "checkBottole4";
            this.checkBottole4.Size = new System.Drawing.Size(15, 14);
            this.checkBottole4.TabIndex = 56;
            this.checkBottole4.UseVisualStyleBackColor = true;
            // 
            // checkBottole5
            // 
            this.checkBottole5.AutoSize = true;
            this.checkBottole5.Location = new System.Drawing.Point(699, 71);
            this.checkBottole5.Name = "checkBottole5";
            this.checkBottole5.Size = new System.Drawing.Size(15, 14);
            this.checkBottole5.TabIndex = 56;
            this.checkBottole5.UseVisualStyleBackColor = true;
            // 
            // checkBottole6
            // 
            this.checkBottole6.AutoSize = true;
            this.checkBottole6.Location = new System.Drawing.Point(871, 71);
            this.checkBottole6.Name = "checkBottole6";
            this.checkBottole6.Size = new System.Drawing.Size(15, 14);
            this.checkBottole6.TabIndex = 56;
            this.checkBottole6.UseVisualStyleBackColor = true;
            // 
            // checkBottole7
            // 
            this.checkBottole7.AutoSize = true;
            this.checkBottole7.Location = new System.Drawing.Point(1042, 71);
            this.checkBottole7.Name = "checkBottole7";
            this.checkBottole7.Size = new System.Drawing.Size(15, 14);
            this.checkBottole7.TabIndex = 56;
            this.checkBottole7.UseVisualStyleBackColor = true;
            // 
            // checkBottole8
            // 
            this.checkBottole8.AutoSize = true;
            this.checkBottole8.Location = new System.Drawing.Point(1212, 71);
            this.checkBottole8.Name = "checkBottole8";
            this.checkBottole8.Size = new System.Drawing.Size(15, 14);
            this.checkBottole8.TabIndex = 56;
            this.checkBottole8.UseVisualStyleBackColor = true;
            // 
            // checkBottole9
            // 
            this.checkBottole9.AutoSize = true;
            this.checkBottole9.Location = new System.Drawing.Point(1389, 71);
            this.checkBottole9.Name = "checkBottole9";
            this.checkBottole9.Size = new System.Drawing.Size(15, 14);
            this.checkBottole9.TabIndex = 56;
            this.checkBottole9.UseVisualStyleBackColor = true;
            // 
            // checkBottole10
            // 
            this.checkBottole10.AutoSize = true;
            this.checkBottole10.Location = new System.Drawing.Point(1560, 71);
            this.checkBottole10.Name = "checkBottole10";
            this.checkBottole10.Size = new System.Drawing.Size(15, 14);
            this.checkBottole10.TabIndex = 56;
            this.checkBottole10.UseVisualStyleBackColor = true;
            // 
            // BottleTraceForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1920, 1024);
            this.Controls.Add(this.checkBottole10);
            this.Controls.Add(this.checkBottole9);
            this.Controls.Add(this.checkBottole8);
            this.Controls.Add(this.checkBottole7);
            this.Controls.Add(this.checkBottole6);
            this.Controls.Add(this.checkBottole5);
            this.Controls.Add(this.checkBottole4);
            this.Controls.Add(this.checkBottole3);
            this.Controls.Add(this.checkBottole2);
            this.Controls.Add(this.checkBottole1);
            this.Controls.Add(this.btnBottoleExcelOutput);
            this.Controls.Add(this.btnBottoleDetectCrossPoints);
            this.Controls.Add(this.rdoBottoleTabNameItemCode);
            this.Controls.Add(this.selectBottole);
            this.Controls.Add(this.rdoBottoleTabNameOrder);
            this.Controls.Add(this.bottoleTitle);
            this.Controls.Add(this.swichBottleTab);
            this.Controls.Add(this.btnBackToLiquid);
            this.Controls.Add(this.btnBottleScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "BottleTraceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "瓶設備ロットトレース";
            this.bottoleIntersectionTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).EndInit();
            this.tabBottolePage2.ResumeLayout(false);
            this.tabBottolePage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_2)).EndInit();
            this.swichBottleTab.ResumeLayout(false);
            this.tabBottolePage1.ResumeLayout(false);
            this.tabBottolePage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle)).EndInit();
            this.tabBottolePage3.ResumeLayout(false);
            this.tabBottolePage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_3)).EndInit();
            this.tabBottolePage4.ResumeLayout(false);
            this.tabBottolePage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_4)).EndInit();
            this.tabBottolePage5.ResumeLayout(false);
            this.tabBottolePage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_5)).EndInit();
            this.tabBottolePage6.ResumeLayout(false);
            this.tabBottolePage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_6)).EndInit();
            this.tabBottolePage7.ResumeLayout(false);
            this.tabBottolePage7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_7)).EndInit();
            this.tabBottolePage8.ResumeLayout(false);
            this.tabBottolePage8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_8)).EndInit();
            this.tabBottolePage9.ResumeLayout(false);
            this.tabBottolePage9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_9)).EndInit();
            this.tabBottolePage10.ResumeLayout(false);
            this.tabBottolePage10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button btnBackToLiquid;
        private Button btnBottleScreen;
        private TabPage bottoleIntersectionTab;
        private DataGridView dataGridIntersection;
        private TabPage tabBottolePage2;
        private DataGridView dgvStartBottle_2;
        private DataGridView dgvEndBottle_2;
        private RadioButton rdoBackwardBottle_2;
        private RadioButton rdoForwardBottle_2;
        private Label lblBottoleOrderNo_2;
        private Panel panelStartBottole_2;
        private Panel panelEndBottole_2;
        private TextBox txtBottoleOrderNo_2;
        private TextBox txtBottoleItemName_2;
        private TextBox txtBottoleItemCode_2;
        private TextBox txtBottoleLotNo_2;
        private Label lblBottoleItemName_2;
        private Label lblBottleItemCode_2;
        private Label lblBottleLotNo_2;
        private Label lblTargetBottole_2;
        private CheckBox timeCheck_2;
        private DateTimePicker startBottoleTime_2;
        private Label lblBottole_2;
        private DateTimePicker endBottoleTime_2;
        private Button btnClearBottole_2;
        private Button btnCsvOutputBottole_2;
        private Button btnBottoleTraceSearch_2;
        private TabControl swichBottleTab;
        private TabPage tabBottolePage1;
        private Panel panelStartBottole;
        private DataGridView dgvStartBottle;
        private Panel panelEndBottole;
        private DataGridView dgvEndBottle;
        private RadioButton rdoBackwardBottle;
        private RadioButton rdoForwardBottle;
        private Label lblBottoleOrderNo;
        private TextBox txtBottoleOrderNo;
        private TextBox txtBottoleItemName;
        private TextBox txtBottoleItemCode;
        private TextBox txtBottoleLotNo;
        private Label lblBottoleItemName;
        private Label lblBottleItemCode;
        private Label lblBottleLotNo;
        private Label lblTargetBottole;
        private CheckBox timeCheck;
        private DateTimePicker startBottoleTime;
        private Label lblTilde;
        private DateTimePicker endBottoleTime;
        private Button btnClearBottle;
        private Button btnCsvOutputBottle;
        private Button btnBottoleTraceSearch;
        private TabPage tabBottolePage3;
        private Panel panelStartBottole_3;
        private DataGridView dgvStartBottle_3;
        private Panel panelEndBottole_3;
        private DataGridView dgvEndBottle_3;
        private RadioButton rdoBackwardBottle_3;
        private RadioButton rdoForwardBottle_3;
        private Label lblBottoleOrderNo_3;
        private TextBox txtBottoleOrderNo_3;
        private TextBox txtBottoleItemName_3;
        private TextBox txtBottoleItemCode_3;
        private TextBox txtBottoleLotNo_3;
        private Label lblBottoleItemName_3;
        private Label lblBottleItemCode_3;
        private Label lblBottleLotNo_3;
        private Label lblTargetBottole_3;
        private CheckBox timeCheck_3;
        private DateTimePicker startBottoleTime_3;
        private Label lblBottole_3;
        private DateTimePicker endBottoleTime_3;
        private Button btnClearBottle_3;
        private Button btnCsvOutputBottle_3;
        private Button btnTraceSearch_3;
        private TabPage tabBottolePage4;
        private Panel panelStartBottole_4;
        private DataGridView dgvStartBottle_4;
        private Panel panelEndBottole_4;
        private DataGridView dgvEndBottle_4;
        private RadioButton rdoBackwardBottle_4;
        private RadioButton rdoForwardBottle_4;
        private Label lblOrderNumber_4;
        private TextBox txtBottoleOrderNo_4;
        private TextBox textBox7;
        private TextBox txtBottoleItemCode_4;
        private TextBox txtBottoleLotNo_4;
        private Label lblBottoleItemName_4;
        private Label lblBottleItemCode_4;
        private Label lblBottleLotNo_4;
        private Label lblTargetBottole_4;
        private CheckBox timeCheck_4;
        private DateTimePicker startBottoleTime_4;
        private Label lblBottole_4;
        private DateTimePicker endBottoleTime_4;
        private Button btnClearBottle_4;
        private Button btnCsvOutputBottle_4;
        private Button btnTraceSearch_4;
        private TabPage tabBottolePage5;
        private Panel panelStartBottole_5;
        private DataGridView dgvStartBottle_5;
        private Panel panelEndBottole_5;
        private DataGridView dgvEndBottle_5;
        private RadioButton rdoBackwardBottle_5;
        private RadioButton rdoForwardBottle_5;
        private Label lblBottoleOrderNo_5;
        private TextBox txtBottoleOrderNo_5;
        private TextBox txtBottoleItemName_5;
        private TextBox txtBottoleItemCode_5;
        private TextBox txtBottoleLotNo_5;
        private Label lblBottoleItemName_5;
        private Label lblBottleItemCode_5;
        private Label lblBottleLotNo_5;
        private Label lblTargetBottole_5;
        private CheckBox timeCheck_5;
        private DateTimePicker startBottoleTime_5;
        private Label lblBottole_5;
        private DateTimePicker endBottoleTime_5;
        private Button btnClearBottle_5;
        private Button btnCsvOutputBottle_5;
        private Button btnTraceSearch_5;
        private TabPage tabBottolePage6;
        private Panel panelStartBottole_6;
        private DataGridView dgvStartBottle_6;
        private Panel panelEndBottole_6;
        private DataGridView dgvEndBottle_6;
        private RadioButton rdoBackwardBottle_6;
        private RadioButton rdoForwardBottle_6;
        private Label lblBottoleOrderNo_6;
        private TextBox txtBottoleOrderNo_6;
        private TextBox txtBottoleItemName_6;
        private TextBox txtBottoleItemCode_6;
        private TextBox txtBottoleLotNo_6;
        private Label lblBottoleItemName_6;
        private Label lblBottleItemCode_6;
        private Label lblBottleLotNo_6;
        private Label lblTargetBottole_6;
        private CheckBox timeCheck_6;
        private DateTimePicker startBottoleTime_6;
        private Label lblBottole_6;
        private DateTimePicker endBottoleTime_6;
        private Button btnClearBottle_6;
        private Button btnCsvOutputBottle_6;
        private Button btnTraceSearch_6;
        private TabPage tabBottolePage7;
        private Panel panelStartBottole_7;
        private DataGridView dgvStartBottle_7;
        private Panel panelEndBottole_7;
        private DataGridView dgvEndBottle_7;
        private RadioButton rdoBackwardBottle_7;
        private RadioButton rdoForwardBottle_7;
        private Label lblBottoleOrderNo_7;
        private TextBox txtBottoleOrderNo_7;
        private TextBox txtBottoleItemName_7;
        private TextBox txtBottoleItemCode_7;
        private TextBox txtBottoleLotNo_7;
        private Label lblBottoleItemName_7;
        private Label lblBottleItemCode_7;
        private Label lblBottleLotNo_7;
        private Label lblTargetBottole_7;
        private CheckBox timeCheck_7;
        private DateTimePicker startBottoleTime_7;
        private Label lblBottole_7;
        private DateTimePicker endBottoleTime_7;
        private Button btnClearBottle_7;
        private Button btnCsvOutputBottle_7;
        private Button btnTraceSearch_7;
        private TabPage tabBottolePage8;
        private Panel panelStartBottole_8;
        private DataGridView dgvStartBottle_8;
        private Panel panelEndBottole_8;
        private DataGridView dgvEndBottle_8;
        private RadioButton rdoBackwardBottle_8;
        private RadioButton rdoForwardBottle_8;
        private Label lblBottoleOrderNo_8;
        private TextBox txtBottoleOrderNo_8;
        private TextBox txtBottoleItemName_8;
        private TextBox txtBottoleItemCode_8;
        private TextBox txtBottoleLotNo_8;
        private Label lblBottoleItemName_8;
        private Label lblBottleItemCode_8;
        private Label lblBottleLotNo_8;
        private Label lblTargetBottole_8;
        private CheckBox timeCheck_8;
        private DateTimePicker startBottoleTime_8;
        private Label lblBottole_8;
        private DateTimePicker endBottoleTime_8;
        private Button btnClearBottle_8;
        private Button btnCsvOutputBottle_8;
        private Button btnTraceSearch_8;
        private TabPage tabBottolePage9;
        private Panel panelStartBottole_9;
        private DataGridView dgvStartBottle_9;
        private Panel panelEndBottole_9;
        private DataGridView dgvEndBottle_9;
        private RadioButton rdoBackwardBottle_9;
        private RadioButton rdoForwardBottle_9;
        private Label lblBottoleOrderNo_9;
        private TextBox txtBottoleOrderNo_9;
        private TextBox txtBottoleItemName_9;
        private TextBox txtBottoleItemCode_9;
        private TextBox txtBottoleLotNo_9;
        private Label lblBottoleItemName_9;
        private Label lblBottleItemCode_9;
        private Label lblBottleLotNo_9;
        private Label lblTargetBottole_9;
        private CheckBox timeCheck_9;
        private DateTimePicker startBottoleTime_9;
        private Label lblBottole_9;
        private DateTimePicker endBottoleTime_9;
        private Button btnClearBottle_9;
        private Button btnCsvOutputBottle_9;
        private Button btnTraceSearch_9;
        private TabPage tabBottolePage10;
        private Panel panelStartBottole_10;
        private DataGridView dgvStartBottle_10;
        private Panel panelEndBottole_10;
        private DataGridView dgvEndBottle_10;
        private RadioButton radioButton5;
        private RadioButton rdoForwardBottle_10;
        private Label lblBottoleOrderNo_10;
        private TextBox textBox10;
        private TextBox txtBottoleItemName_10;
        private TextBox txtBottoleItemCode_10;
        private TextBox txtBottoleLotNo_10;
        private Label lblBottoleItemName_10;
        private Label lblBottleItemCode_10;
        private Label lblBottleLotNo_10;
        private Label lblTargetBottole_10;
        private CheckBox timeCheck_10;
        private DateTimePicker dateTimePicker5;
        private Label label18;
        private DateTimePicker endBottoleTime_10;
        private Button btnClearBottle_10;
        private Button btnCsvOutputBottle_10;
        private Button btnTraceSearch_10;
        private Label bottoleTitle;
        private RadioButton rdoBottoleTabNameItemCode;
        private Label selectBottole;
        private RadioButton rdoBottoleTabNameOrder;
        private Button btnBottoleExcelOutput;
        private Button btnBottoleDetectCrossPoints;
        private CheckBox checkBottole1;
        private CheckBox checkBottole2;
        private CheckBox checkBottole3;
        private CheckBox checkBottole4;
        private CheckBox checkBottole5;
        private CheckBox checkBottole6;
        private CheckBox checkBottole7;
        private CheckBox checkBottole8;
        private CheckBox checkBottole9;
        private CheckBox checkBottole10;
    }
}