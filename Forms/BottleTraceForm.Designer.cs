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
            this.BottleIntersectionTab = new System.Windows.Forms.TabPage();
            this.dataGridIntersection = new System.Windows.Forms.DataGridView();
            this.tabBottlePage2 = new System.Windows.Forms.TabPage();
            this.dgvStartBottle_2 = new System.Windows.Forms.DataGridView();
            this.dgvEndBottle_2 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_2 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_2 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_2 = new System.Windows.Forms.Label();
            this.panelStartBottle_2 = new System.Windows.Forms.Panel();
            this.panelEndBottle_2 = new System.Windows.Forms.Panel();
            this.txtBottleOrderNo_2 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_2 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_2 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_2 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_2 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_2 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_2 = new System.Windows.Forms.Label();
            this.lblTargetBottle_2 = new System.Windows.Forms.Label();
            this.timeCheck_2 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_2 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_2 = new System.Windows.Forms.Label();
            this.endBottleTime_2 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_2 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_2 = new System.Windows.Forms.Button();
            this.btnBottleTraceSearch_2 = new System.Windows.Forms.Button();
            this.swichBottleTab = new System.Windows.Forms.TabControl();
            this.tabBottlePage1 = new System.Windows.Forms.TabPage();
            this.panelStartBottle = new System.Windows.Forms.Panel();
            this.dgvStartBottle = new System.Windows.Forms.DataGridView();
            this.panelEndBottle = new System.Windows.Forms.Panel();
            this.dgvEndBottle = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo = new System.Windows.Forms.Label();
            this.txtBottleOrderNo = new System.Windows.Forms.TextBox();
            this.txtBottleItemName = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo = new System.Windows.Forms.TextBox();
            this.lblBottleItemName = new System.Windows.Forms.Label();
            this.lblBottleItemCode = new System.Windows.Forms.Label();
            this.lblBottleLotNo = new System.Windows.Forms.Label();
            this.lblTargetBottle = new System.Windows.Forms.Label();
            this.timeCheck = new System.Windows.Forms.CheckBox();
            this.startBottleTime = new System.Windows.Forms.DateTimePicker();
            this.lblTilde = new System.Windows.Forms.Label();
            this.endBottleTime = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle = new System.Windows.Forms.Button();
            this.btnBottleTraceSearch = new System.Windows.Forms.Button();
            this.tabBottlePage3 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_3 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_3 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_3 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_3 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_3 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_3 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_3 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_3 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_3 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_3 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_3 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_3 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_3 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_3 = new System.Windows.Forms.Label();
            this.lblTargetBottle_3 = new System.Windows.Forms.Label();
            this.timeCheck_3 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_3 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_3 = new System.Windows.Forms.Label();
            this.endBottleTime_3 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_3 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_3 = new System.Windows.Forms.Button();
            this.btnTraceSearch_3 = new System.Windows.Forms.Button();
            this.tabBottlePage4 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_4 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_4 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_4 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_4 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_4 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_4 = new System.Windows.Forms.RadioButton();
            this.lblOrderNumber_4 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_4 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_4 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_4 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_4 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_4 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_4 = new System.Windows.Forms.Label();
            this.lblTargetBottle_4 = new System.Windows.Forms.Label();
            this.timeCheck_4 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_4 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_4 = new System.Windows.Forms.Label();
            this.endBottleTime_4 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_4 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_4 = new System.Windows.Forms.Button();
            this.btnTraceSearch_4 = new System.Windows.Forms.Button();
            this.tabBottlePage5 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_5 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_5 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_5 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_5 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_5 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_5 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_5 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_5 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_5 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_5 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_5 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_5 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_5 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_5 = new System.Windows.Forms.Label();
            this.lblTargetBottle_5 = new System.Windows.Forms.Label();
            this.timeCheck_5 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_5 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_5 = new System.Windows.Forms.Label();
            this.endBottleTime_5 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_5 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_5 = new System.Windows.Forms.Button();
            this.btnTraceSearch_5 = new System.Windows.Forms.Button();
            this.tabBottlePage6 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_6 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_6 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_6 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_6 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_6 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_6 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_6 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_6 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_6 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_6 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_6 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_6 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_6 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_6 = new System.Windows.Forms.Label();
            this.lblTargetBottle_6 = new System.Windows.Forms.Label();
            this.timeCheck_6 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_6 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_6 = new System.Windows.Forms.Label();
            this.endBottleTime_6 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_6 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_6 = new System.Windows.Forms.Button();
            this.btnTraceSearch_6 = new System.Windows.Forms.Button();
            this.tabBottlePage7 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_7 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_7 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_7 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_7 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_7 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_7 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_7 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_7 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_7 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_7 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_7 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_7 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_7 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_7 = new System.Windows.Forms.Label();
            this.lblTargetBottle_7 = new System.Windows.Forms.Label();
            this.timeCheck_7 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_7 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_7 = new System.Windows.Forms.Label();
            this.endBottleTime_7 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_7 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_7 = new System.Windows.Forms.Button();
            this.btnTraceSearch_7 = new System.Windows.Forms.Button();
            this.tabBottlePage8 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_8 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_8 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_8 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_8 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_8 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_8 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_8 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_8 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_8 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_8 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_8 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_8 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_8 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_8 = new System.Windows.Forms.Label();
            this.lblTargetBottle_8 = new System.Windows.Forms.Label();
            this.timeCheck_8 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_8 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_8 = new System.Windows.Forms.Label();
            this.endBottleTime_8 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_8 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_8 = new System.Windows.Forms.Button();
            this.btnTraceSearch_8 = new System.Windows.Forms.Button();
            this.tabBottlePage9 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_9 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_9 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_9 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_9 = new System.Windows.Forms.DataGridView();
            this.rdoBackwardBottle_9 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_9 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_9 = new System.Windows.Forms.Label();
            this.txtBottleOrderNo_9 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_9 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_9 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_9 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_9 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_9 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_9 = new System.Windows.Forms.Label();
            this.lblTargetBottle_9 = new System.Windows.Forms.Label();
            this.timeCheck_9 = new System.Windows.Forms.CheckBox();
            this.startBottleTime_9 = new System.Windows.Forms.DateTimePicker();
            this.lblBottle_9 = new System.Windows.Forms.Label();
            this.endBottleTime_9 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_9 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_9 = new System.Windows.Forms.Button();
            this.btnTraceSearch_9 = new System.Windows.Forms.Button();
            this.tabBottlePage10 = new System.Windows.Forms.TabPage();
            this.panelStartBottle_10 = new System.Windows.Forms.Panel();
            this.dgvStartBottle_10 = new System.Windows.Forms.DataGridView();
            this.panelEndBottle_10 = new System.Windows.Forms.Panel();
            this.dgvEndBottle_10 = new System.Windows.Forms.DataGridView();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.rdoForwardBottle_10 = new System.Windows.Forms.RadioButton();
            this.lblBottleOrderNo_10 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.txtBottleItemName_10 = new System.Windows.Forms.TextBox();
            this.txtBottleItemCode_10 = new System.Windows.Forms.TextBox();
            this.txtBottleLotNo_10 = new System.Windows.Forms.TextBox();
            this.lblBottleItemName_10 = new System.Windows.Forms.Label();
            this.lblBottleItemCode_10 = new System.Windows.Forms.Label();
            this.lblBottleLotNo_10 = new System.Windows.Forms.Label();
            this.lblTargetBottle_10 = new System.Windows.Forms.Label();
            this.timeCheck_10 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker5 = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.endBottleTime_10 = new System.Windows.Forms.DateTimePicker();
            this.btnClearBottle_10 = new System.Windows.Forms.Button();
            this.btnCsvOutputBottle_10 = new System.Windows.Forms.Button();
            this.btnTraceSearch_10 = new System.Windows.Forms.Button();
            this.BottleTitle = new System.Windows.Forms.Label();
            this.rdoBottleTabNameItemCode = new System.Windows.Forms.RadioButton();
            this.selectBottle = new System.Windows.Forms.Label();
            this.rdoBottleTabNameOrder = new System.Windows.Forms.RadioButton();
            this.btnBottleExcelOutput = new System.Windows.Forms.Button();
            this.btnBottleDetectCrossPoints = new System.Windows.Forms.Button();
            this.checkBottle1 = new System.Windows.Forms.CheckBox();
            this.checkBottle2 = new System.Windows.Forms.CheckBox();
            this.checkBottle3 = new System.Windows.Forms.CheckBox();
            this.checkBottle4 = new System.Windows.Forms.CheckBox();
            this.checkBottle5 = new System.Windows.Forms.CheckBox();
            this.checkBottle6 = new System.Windows.Forms.CheckBox();
            this.checkBottle7 = new System.Windows.Forms.CheckBox();
            this.checkBottle8 = new System.Windows.Forms.CheckBox();
            this.checkBottle9 = new System.Windows.Forms.CheckBox();
            this.checkBottle10 = new System.Windows.Forms.CheckBox();
            this.BottleIntersectionTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).BeginInit();
            this.tabBottlePage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_2)).BeginInit();
            this.swichBottleTab.SuspendLayout();
            this.tabBottlePage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle)).BeginInit();
            this.tabBottlePage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_3)).BeginInit();
            this.tabBottlePage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_4)).BeginInit();
            this.tabBottlePage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_5)).BeginInit();
            this.tabBottlePage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_6)).BeginInit();
            this.tabBottlePage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_7)).BeginInit();
            this.tabBottlePage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_8)).BeginInit();
            this.tabBottlePage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_9)).BeginInit();
            this.tabBottlePage10.SuspendLayout();
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
            this.btnBackToLiquid.Click += new System.EventHandler(this.btnBackToLiquid_Click);
            // 
            // BottleIntersectionTab
            // 
            this.BottleIntersectionTab.Controls.Add(this.dataGridIntersection);
            this.BottleIntersectionTab.Location = new System.Drawing.Point(4, 34);
            this.BottleIntersectionTab.Name = "BottleIntersectionTab";
            this.BottleIntersectionTab.Padding = new System.Windows.Forms.Padding(3);
            this.BottleIntersectionTab.Size = new System.Drawing.Size(1896, 902);
            this.BottleIntersectionTab.TabIndex = 3;
            this.BottleIntersectionTab.Text = "交点検出結果";
            this.BottleIntersectionTab.UseVisualStyleBackColor = true;
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
            // tabBottlePage2
            // 
            this.tabBottlePage2.Controls.Add(this.dgvStartBottle_2);
            this.tabBottlePage2.Controls.Add(this.dgvEndBottle_2);
            this.tabBottlePage2.Controls.Add(this.rdoBackwardBottle_2);
            this.tabBottlePage2.Controls.Add(this.rdoForwardBottle_2);
            this.tabBottlePage2.Controls.Add(this.lblBottleOrderNo_2);
            this.tabBottlePage2.Controls.Add(this.panelStartBottle_2);
            this.tabBottlePage2.Controls.Add(this.panelEndBottle_2);
            this.tabBottlePage2.Controls.Add(this.txtBottleOrderNo_2);
            this.tabBottlePage2.Controls.Add(this.txtBottleItemName_2);
            this.tabBottlePage2.Controls.Add(this.txtBottleItemCode_2);
            this.tabBottlePage2.Controls.Add(this.txtBottleLotNo_2);
            this.tabBottlePage2.Controls.Add(this.lblBottleItemName_2);
            this.tabBottlePage2.Controls.Add(this.lblBottleItemCode_2);
            this.tabBottlePage2.Controls.Add(this.lblBottleLotNo_2);
            this.tabBottlePage2.Controls.Add(this.lblTargetBottle_2);
            this.tabBottlePage2.Controls.Add(this.timeCheck_2);
            this.tabBottlePage2.Controls.Add(this.startBottleTime_2);
            this.tabBottlePage2.Controls.Add(this.lblBottle_2);
            this.tabBottlePage2.Controls.Add(this.endBottleTime_2);
            this.tabBottlePage2.Controls.Add(this.btnClearBottle_2);
            this.tabBottlePage2.Controls.Add(this.btnCsvOutputBottle_2);
            this.tabBottlePage2.Controls.Add(this.btnBottleTraceSearch_2);
            this.tabBottlePage2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage2.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage2.Name = "tabBottlePage2";
            this.tabBottlePage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage2.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage2.TabIndex = 13;
            this.tabBottlePage2.Text = "(02)_未設定";
            this.tabBottlePage2.UseVisualStyleBackColor = true;
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
            // lblBottleOrderNo_2
            // 
            this.lblBottleOrderNo_2.AutoSize = true;
            this.lblBottleOrderNo_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_2.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_2.Name = "lblBottleOrderNo_2";
            this.lblBottleOrderNo_2.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_2.TabIndex = 1;
            this.lblBottleOrderNo_2.Text = "製造指図番号";
            // 
            // panelStartBottle_2
            // 
            this.panelStartBottle_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_2.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_2.Name = "panelStartBottle_2";
            this.panelStartBottle_2.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_2.TabIndex = 28;
            // 
            // panelEndBottle_2
            // 
            this.panelEndBottle_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_2.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_2.Name = "panelEndBottle_2";
            this.panelEndBottle_2.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_2.TabIndex = 30;
            // 
            // txtBottleOrderNo_2
            // 
            this.txtBottleOrderNo_2.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_2.Name = "txtBottleOrderNo_2";
            this.txtBottleOrderNo_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_2.TabIndex = 2;
            // 
            // txtBottleItemName_2
            // 
            this.txtBottleItemName_2.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_2.Name = "txtBottleItemName_2";
            this.txtBottleItemName_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_2.TabIndex = 4;
            // 
            // txtBottleItemCode_2
            // 
            this.txtBottleItemCode_2.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_2.Name = "txtBottleItemCode_2";
            this.txtBottleItemCode_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_2.TabIndex = 6;
            // 
            // txtBottleLotNo_2
            // 
            this.txtBottleLotNo_2.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_2.Name = "txtBottleLotNo_2";
            this.txtBottleLotNo_2.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_2.TabIndex = 8;
            // 
            // lblBottleItemName_2
            // 
            this.lblBottleItemName_2.AutoSize = true;
            this.lblBottleItemName_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_2.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_2.Name = "lblBottleItemName_2";
            this.lblBottleItemName_2.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_2.TabIndex = 3;
            this.lblBottleItemName_2.Text = "品目名";
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
            // lblTargetBottle_2
            // 
            this.lblTargetBottle_2.AutoSize = true;
            this.lblTargetBottle_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_2.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_2.Name = "lblTargetBottle_2";
            this.lblTargetBottle_2.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_2.TabIndex = 9;
            this.lblTargetBottle_2.Text = "対象期間";
            // 
            // timeCheck_2
            // 
            this.timeCheck_2.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_2.Name = "timeCheck_2";
            this.timeCheck_2.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_2.TabIndex = 10;
            // 
            // startBottleTime_2
            // 
            this.startBottleTime_2.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_2.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_2.Name = "startBottleTime_2";
            this.startBottleTime_2.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_2.TabIndex = 11;
            // 
            // lblBottle_2
            // 
            this.lblBottle_2.AutoSize = true;
            this.lblBottle_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_2.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_2.Name = "lblBottle_2";
            this.lblBottle_2.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_2.TabIndex = 12;
            this.lblBottle_2.Text = "～";
            // 
            // endBottleTime_2
            // 
            this.endBottleTime_2.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_2.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_2.Name = "endBottleTime_2";
            this.endBottleTime_2.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_2.TabIndex = 14;
            // 
            // btnClearBottle_2
            // 
            this.btnClearBottle_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClearBottle_2.Location = new System.Drawing.Point(1377, 15);
            this.btnClearBottle_2.Name = "btnClearBottle_2";
            this.btnClearBottle_2.Size = new System.Drawing.Size(150, 50);
            this.btnClearBottle_2.TabIndex = 18;
            this.btnClearBottle_2.Text = "クリア";
            // 
            // btnCsvOutputBottle_2
            // 
            this.btnCsvOutputBottle_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutputBottle_2.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutputBottle_2.Name = "btnCsvOutputBottle_2";
            this.btnCsvOutputBottle_2.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutputBottle_2.TabIndex = 19;
            this.btnCsvOutputBottle_2.Text = "CSV出力";
            // 
            // btnBottleTraceSearch_2
            // 
            this.btnBottleTraceSearch_2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnBottleTraceSearch_2.Location = new System.Drawing.Point(1720, 15);
            this.btnBottleTraceSearch_2.Name = "btnBottleTraceSearch_2";
            this.btnBottleTraceSearch_2.Size = new System.Drawing.Size(150, 50);
            this.btnBottleTraceSearch_2.TabIndex = 22;
            this.btnBottleTraceSearch_2.Text = "トレース検索";
            // 
            // swichBottleTab
            // 
            this.swichBottleTab.Controls.Add(this.tabBottlePage1);
            this.swichBottleTab.Controls.Add(this.tabBottlePage2);
            this.swichBottleTab.Controls.Add(this.tabBottlePage3);
            this.swichBottleTab.Controls.Add(this.tabBottlePage4);
            this.swichBottleTab.Controls.Add(this.tabBottlePage5);
            this.swichBottleTab.Controls.Add(this.tabBottlePage6);
            this.swichBottleTab.Controls.Add(this.tabBottlePage7);
            this.swichBottleTab.Controls.Add(this.tabBottlePage8);
            this.swichBottleTab.Controls.Add(this.tabBottlePage9);
            this.swichBottleTab.Controls.Add(this.tabBottlePage10);
            this.swichBottleTab.Controls.Add(this.BottleIntersectionTab);
            this.swichBottleTab.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.swichBottleTab.ItemSize = new System.Drawing.Size(172, 30);
            this.swichBottleTab.Location = new System.Drawing.Point(8, 85);
            this.swichBottleTab.Name = "swichBottleTab";
            this.swichBottleTab.SelectedIndex = 0;
            this.swichBottleTab.Size = new System.Drawing.Size(1904, 940);
            this.swichBottleTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.swichBottleTab.TabIndex = 49;
            // 
            // tabBottlePage1
            // 
            this.tabBottlePage1.Controls.Add(this.panelStartBottle);
            this.tabBottlePage1.Controls.Add(this.dgvStartBottle);
            this.tabBottlePage1.Controls.Add(this.panelEndBottle);
            this.tabBottlePage1.Controls.Add(this.dgvEndBottle);
            this.tabBottlePage1.Controls.Add(this.rdoBackwardBottle);
            this.tabBottlePage1.Controls.Add(this.rdoForwardBottle);
            this.tabBottlePage1.Controls.Add(this.lblBottleOrderNo);
            this.tabBottlePage1.Controls.Add(this.txtBottleOrderNo);
            this.tabBottlePage1.Controls.Add(this.txtBottleItemName);
            this.tabBottlePage1.Controls.Add(this.txtBottleItemCode);
            this.tabBottlePage1.Controls.Add(this.txtBottleLotNo);
            this.tabBottlePage1.Controls.Add(this.lblBottleItemName);
            this.tabBottlePage1.Controls.Add(this.lblBottleItemCode);
            this.tabBottlePage1.Controls.Add(this.lblBottleLotNo);
            this.tabBottlePage1.Controls.Add(this.lblTargetBottle);
            this.tabBottlePage1.Controls.Add(this.timeCheck);
            this.tabBottlePage1.Controls.Add(this.startBottleTime);
            this.tabBottlePage1.Controls.Add(this.lblTilde);
            this.tabBottlePage1.Controls.Add(this.endBottleTime);
            this.tabBottlePage1.Controls.Add(this.btnClearBottle);
            this.tabBottlePage1.Controls.Add(this.btnCsvOutputBottle);
            this.tabBottlePage1.Controls.Add(this.btnBottleTraceSearch);
            this.tabBottlePage1.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage1.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage1.Name = "tabBottlePage1";
            this.tabBottlePage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage1.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage1.TabIndex = 0;
            this.tabBottlePage1.Text = "(01)_未設定";
            this.tabBottlePage1.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle
            // 
            this.panelStartBottle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle.Name = "panelStartBottle";
            this.panelStartBottle.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle.TabIndex = 28;
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
            // panelEndBottle
            // 
            this.panelEndBottle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle.Name = "panelEndBottle";
            this.panelEndBottle.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle.TabIndex = 30;
            // 
            // dgvEndBottle
            // 
            this.dgvEndBottle.Location = new System.Drawing.Point(966, 222);
            this.dgvEndBottle.Name = "dgvEndBottle";
            this.dgvEndBottle.ReadOnly = true;
            this.dgvEndBottle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle.Size = new System.Drawing.Size(920, 650);
            this.dgvEndBottle.TabIndex = 31;
            this.dgvEndBottle.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvEndBottle_CellMouseClick);
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
            // lblBottleOrderNo
            // 
            this.lblBottleOrderNo.AutoSize = true;
            this.lblBottleOrderNo.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo.Name = "lblBottleOrderNo";
            this.lblBottleOrderNo.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo.TabIndex = 1;
            this.lblBottleOrderNo.Text = "製造指図番号";
            // 
            // txtBottleOrderNo
            // 
            this.txtBottleOrderNo.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo.Name = "txtBottleOrderNo";
            this.txtBottleOrderNo.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo.TabIndex = 2;
            // 
            // txtBottleItemName
            // 
            this.txtBottleItemName.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName.Name = "txtBottleItemName";
            this.txtBottleItemName.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName.TabIndex = 4;
            // 
            // txtBottleItemCode
            // 
            this.txtBottleItemCode.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode.Name = "txtBottleItemCode";
            this.txtBottleItemCode.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode.TabIndex = 6;
            // 
            // txtBottleLotNo
            // 
            this.txtBottleLotNo.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo.Name = "txtBottleLotNo";
            this.txtBottleLotNo.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo.TabIndex = 8;
            // 
            // lblBottleItemName
            // 
            this.lblBottleItemName.AutoSize = true;
            this.lblBottleItemName.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName.Name = "lblBottleItemName";
            this.lblBottleItemName.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName.TabIndex = 3;
            this.lblBottleItemName.Text = "品目名";
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
            // lblTargetBottle
            // 
            this.lblTargetBottle.AutoSize = true;
            this.lblTargetBottle.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle.Name = "lblTargetBottle";
            this.lblTargetBottle.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle.TabIndex = 9;
            this.lblTargetBottle.Text = "対象期間";
            // 
            // timeCheck
            // 
            this.timeCheck.Location = new System.Drawing.Point(367, 162);
            this.timeCheck.Name = "timeCheck";
            this.timeCheck.Size = new System.Drawing.Size(15, 14);
            this.timeCheck.TabIndex = 10;
            // 
            // startBottleTime
            // 
            this.startBottleTime.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime.Name = "startBottleTime";
            this.startBottleTime.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime.TabIndex = 11;
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
            // endBottleTime
            // 
            this.endBottleTime.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime.Name = "endBottleTime";
            this.endBottleTime.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime.TabIndex = 14;
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
            // btnBottleTraceSearch
            // 
            this.btnBottleTraceSearch.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnBottleTraceSearch.Location = new System.Drawing.Point(1720, 15);
            this.btnBottleTraceSearch.Name = "btnBottleTraceSearch";
            this.btnBottleTraceSearch.Size = new System.Drawing.Size(150, 50);
            this.btnBottleTraceSearch.TabIndex = 22;
            this.btnBottleTraceSearch.Text = "トレース検索";
            // 
            // tabBottlePage3
            // 
            this.tabBottlePage3.Controls.Add(this.panelStartBottle_3);
            this.tabBottlePage3.Controls.Add(this.dgvStartBottle_3);
            this.tabBottlePage3.Controls.Add(this.panelEndBottle_3);
            this.tabBottlePage3.Controls.Add(this.dgvEndBottle_3);
            this.tabBottlePage3.Controls.Add(this.rdoBackwardBottle_3);
            this.tabBottlePage3.Controls.Add(this.rdoForwardBottle_3);
            this.tabBottlePage3.Controls.Add(this.lblBottleOrderNo_3);
            this.tabBottlePage3.Controls.Add(this.txtBottleOrderNo_3);
            this.tabBottlePage3.Controls.Add(this.txtBottleItemName_3);
            this.tabBottlePage3.Controls.Add(this.txtBottleItemCode_3);
            this.tabBottlePage3.Controls.Add(this.txtBottleLotNo_3);
            this.tabBottlePage3.Controls.Add(this.lblBottleItemName_3);
            this.tabBottlePage3.Controls.Add(this.lblBottleItemCode_3);
            this.tabBottlePage3.Controls.Add(this.lblBottleLotNo_3);
            this.tabBottlePage3.Controls.Add(this.lblTargetBottle_3);
            this.tabBottlePage3.Controls.Add(this.timeCheck_3);
            this.tabBottlePage3.Controls.Add(this.startBottleTime_3);
            this.tabBottlePage3.Controls.Add(this.lblBottle_3);
            this.tabBottlePage3.Controls.Add(this.endBottleTime_3);
            this.tabBottlePage3.Controls.Add(this.btnClearBottle_3);
            this.tabBottlePage3.Controls.Add(this.btnCsvOutputBottle_3);
            this.tabBottlePage3.Controls.Add(this.btnTraceSearch_3);
            this.tabBottlePage3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage3.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage3.Name = "tabBottlePage3";
            this.tabBottlePage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage3.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage3.TabIndex = 14;
            this.tabBottlePage3.Text = "(03)_未設定";
            this.tabBottlePage3.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_3
            // 
            this.panelStartBottle_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_3.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_3.Name = "panelStartBottle_3";
            this.panelStartBottle_3.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_3.TabIndex = 28;
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
            // panelEndBottle_3
            // 
            this.panelEndBottle_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_3.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_3.Name = "panelEndBottle_3";
            this.panelEndBottle_3.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_3.TabIndex = 30;
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
            // lblBottleOrderNo_3
            // 
            this.lblBottleOrderNo_3.AutoSize = true;
            this.lblBottleOrderNo_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_3.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_3.Name = "lblBottleOrderNo_3";
            this.lblBottleOrderNo_3.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_3.TabIndex = 1;
            this.lblBottleOrderNo_3.Text = "製造指図番号";
            // 
            // txtBottleOrderNo_3
            // 
            this.txtBottleOrderNo_3.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_3.Name = "txtBottleOrderNo_3";
            this.txtBottleOrderNo_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_3.TabIndex = 2;
            // 
            // txtBottleItemName_3
            // 
            this.txtBottleItemName_3.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_3.Name = "txtBottleItemName_3";
            this.txtBottleItemName_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_3.TabIndex = 4;
            // 
            // txtBottleItemCode_3
            // 
            this.txtBottleItemCode_3.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_3.Name = "txtBottleItemCode_3";
            this.txtBottleItemCode_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_3.TabIndex = 6;
            // 
            // txtBottleLotNo_3
            // 
            this.txtBottleLotNo_3.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_3.Name = "txtBottleLotNo_3";
            this.txtBottleLotNo_3.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_3.TabIndex = 8;
            // 
            // lblBottleItemName_3
            // 
            this.lblBottleItemName_3.AutoSize = true;
            this.lblBottleItemName_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_3.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_3.Name = "lblBottleItemName_3";
            this.lblBottleItemName_3.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_3.TabIndex = 3;
            this.lblBottleItemName_3.Text = "品目名";
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
            // lblTargetBottle_3
            // 
            this.lblTargetBottle_3.AutoSize = true;
            this.lblTargetBottle_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_3.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_3.Name = "lblTargetBottle_3";
            this.lblTargetBottle_3.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_3.TabIndex = 9;
            this.lblTargetBottle_3.Text = "対象期間";
            // 
            // timeCheck_3
            // 
            this.timeCheck_3.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_3.Name = "timeCheck_3";
            this.timeCheck_3.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_3.TabIndex = 10;
            // 
            // startBottleTime_3
            // 
            this.startBottleTime_3.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_3.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_3.Name = "startBottleTime_3";
            this.startBottleTime_3.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_3.TabIndex = 11;
            // 
            // lblBottle_3
            // 
            this.lblBottle_3.AutoSize = true;
            this.lblBottle_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_3.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_3.Name = "lblBottle_3";
            this.lblBottle_3.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_3.TabIndex = 12;
            this.lblBottle_3.Text = "～";
            // 
            // endBottleTime_3
            // 
            this.endBottleTime_3.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_3.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_3.Name = "endBottleTime_3";
            this.endBottleTime_3.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_3.TabIndex = 14;
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
            // tabBottlePage4
            // 
            this.tabBottlePage4.Controls.Add(this.panelStartBottle_4);
            this.tabBottlePage4.Controls.Add(this.dgvStartBottle_4);
            this.tabBottlePage4.Controls.Add(this.panelEndBottle_4);
            this.tabBottlePage4.Controls.Add(this.dgvEndBottle_4);
            this.tabBottlePage4.Controls.Add(this.rdoBackwardBottle_4);
            this.tabBottlePage4.Controls.Add(this.rdoForwardBottle_4);
            this.tabBottlePage4.Controls.Add(this.lblOrderNumber_4);
            this.tabBottlePage4.Controls.Add(this.txtBottleOrderNo_4);
            this.tabBottlePage4.Controls.Add(this.textBox7);
            this.tabBottlePage4.Controls.Add(this.txtBottleItemCode_4);
            this.tabBottlePage4.Controls.Add(this.txtBottleLotNo_4);
            this.tabBottlePage4.Controls.Add(this.lblBottleItemName_4);
            this.tabBottlePage4.Controls.Add(this.lblBottleItemCode_4);
            this.tabBottlePage4.Controls.Add(this.lblBottleLotNo_4);
            this.tabBottlePage4.Controls.Add(this.lblTargetBottle_4);
            this.tabBottlePage4.Controls.Add(this.timeCheck_4);
            this.tabBottlePage4.Controls.Add(this.startBottleTime_4);
            this.tabBottlePage4.Controls.Add(this.lblBottle_4);
            this.tabBottlePage4.Controls.Add(this.endBottleTime_4);
            this.tabBottlePage4.Controls.Add(this.btnClearBottle_4);
            this.tabBottlePage4.Controls.Add(this.btnCsvOutputBottle_4);
            this.tabBottlePage4.Controls.Add(this.btnTraceSearch_4);
            this.tabBottlePage4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage4.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage4.Name = "tabBottlePage4";
            this.tabBottlePage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage4.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage4.TabIndex = 15;
            this.tabBottlePage4.Text = "(04)_未設定";
            this.tabBottlePage4.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_4
            // 
            this.panelStartBottle_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_4.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_4.Name = "panelStartBottle_4";
            this.panelStartBottle_4.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_4.TabIndex = 28;
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
            // panelEndBottle_4
            // 
            this.panelEndBottle_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_4.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_4.Name = "panelEndBottle_4";
            this.panelEndBottle_4.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_4.TabIndex = 30;
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
            // txtBottleOrderNo_4
            // 
            this.txtBottleOrderNo_4.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_4.Name = "txtBottleOrderNo_4";
            this.txtBottleOrderNo_4.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_4.TabIndex = 2;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(110, 50);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(250, 27);
            this.textBox7.TabIndex = 4;
            // 
            // txtBottleItemCode_4
            // 
            this.txtBottleItemCode_4.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_4.Name = "txtBottleItemCode_4";
            this.txtBottleItemCode_4.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_4.TabIndex = 6;
            // 
            // txtBottleLotNo_4
            // 
            this.txtBottleLotNo_4.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_4.Name = "txtBottleLotNo_4";
            this.txtBottleLotNo_4.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_4.TabIndex = 8;
            // 
            // lblBottleItemName_4
            // 
            this.lblBottleItemName_4.AutoSize = true;
            this.lblBottleItemName_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_4.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_4.Name = "lblBottleItemName_4";
            this.lblBottleItemName_4.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_4.TabIndex = 3;
            this.lblBottleItemName_4.Text = "品目名";
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
            // lblTargetBottle_4
            // 
            this.lblTargetBottle_4.AutoSize = true;
            this.lblTargetBottle_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_4.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_4.Name = "lblTargetBottle_4";
            this.lblTargetBottle_4.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_4.TabIndex = 9;
            this.lblTargetBottle_4.Text = "対象期間";
            // 
            // timeCheck_4
            // 
            this.timeCheck_4.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_4.Name = "timeCheck_4";
            this.timeCheck_4.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_4.TabIndex = 10;
            // 
            // startBottleTime_4
            // 
            this.startBottleTime_4.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_4.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_4.Name = "startBottleTime_4";
            this.startBottleTime_4.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_4.TabIndex = 11;
            // 
            // lblBottle_4
            // 
            this.lblBottle_4.AutoSize = true;
            this.lblBottle_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_4.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_4.Name = "lblBottle_4";
            this.lblBottle_4.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_4.TabIndex = 12;
            this.lblBottle_4.Text = "～";
            // 
            // endBottleTime_4
            // 
            this.endBottleTime_4.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_4.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_4.Name = "endBottleTime_4";
            this.endBottleTime_4.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_4.TabIndex = 14;
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
            // tabBottlePage5
            // 
            this.tabBottlePage5.Controls.Add(this.panelStartBottle_5);
            this.tabBottlePage5.Controls.Add(this.dgvStartBottle_5);
            this.tabBottlePage5.Controls.Add(this.panelEndBottle_5);
            this.tabBottlePage5.Controls.Add(this.dgvEndBottle_5);
            this.tabBottlePage5.Controls.Add(this.rdoBackwardBottle_5);
            this.tabBottlePage5.Controls.Add(this.rdoForwardBottle_5);
            this.tabBottlePage5.Controls.Add(this.lblBottleOrderNo_5);
            this.tabBottlePage5.Controls.Add(this.txtBottleOrderNo_5);
            this.tabBottlePage5.Controls.Add(this.txtBottleItemName_5);
            this.tabBottlePage5.Controls.Add(this.txtBottleItemCode_5);
            this.tabBottlePage5.Controls.Add(this.txtBottleLotNo_5);
            this.tabBottlePage5.Controls.Add(this.lblBottleItemName_5);
            this.tabBottlePage5.Controls.Add(this.lblBottleItemCode_5);
            this.tabBottlePage5.Controls.Add(this.lblBottleLotNo_5);
            this.tabBottlePage5.Controls.Add(this.lblTargetBottle_5);
            this.tabBottlePage5.Controls.Add(this.timeCheck_5);
            this.tabBottlePage5.Controls.Add(this.startBottleTime_5);
            this.tabBottlePage5.Controls.Add(this.lblBottle_5);
            this.tabBottlePage5.Controls.Add(this.endBottleTime_5);
            this.tabBottlePage5.Controls.Add(this.btnClearBottle_5);
            this.tabBottlePage5.Controls.Add(this.btnCsvOutputBottle_5);
            this.tabBottlePage5.Controls.Add(this.btnTraceSearch_5);
            this.tabBottlePage5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage5.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage5.Name = "tabBottlePage5";
            this.tabBottlePage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage5.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage5.TabIndex = 16;
            this.tabBottlePage5.Text = "(05)_未設定";
            this.tabBottlePage5.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_5
            // 
            this.panelStartBottle_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_5.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_5.Name = "panelStartBottle_5";
            this.panelStartBottle_5.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_5.TabIndex = 28;
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
            // panelEndBottle_5
            // 
            this.panelEndBottle_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_5.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_5.Name = "panelEndBottle_5";
            this.panelEndBottle_5.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_5.TabIndex = 30;
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
            // lblBottleOrderNo_5
            // 
            this.lblBottleOrderNo_5.AutoSize = true;
            this.lblBottleOrderNo_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_5.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_5.Name = "lblBottleOrderNo_5";
            this.lblBottleOrderNo_5.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_5.TabIndex = 1;
            this.lblBottleOrderNo_5.Text = "製造指図番号";
            // 
            // txtBottleOrderNo_5
            // 
            this.txtBottleOrderNo_5.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_5.Name = "txtBottleOrderNo_5";
            this.txtBottleOrderNo_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_5.TabIndex = 2;
            // 
            // txtBottleItemName_5
            // 
            this.txtBottleItemName_5.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_5.Name = "txtBottleItemName_5";
            this.txtBottleItemName_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_5.TabIndex = 4;
            // 
            // txtBottleItemCode_5
            // 
            this.txtBottleItemCode_5.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_5.Name = "txtBottleItemCode_5";
            this.txtBottleItemCode_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_5.TabIndex = 6;
            // 
            // txtBottleLotNo_5
            // 
            this.txtBottleLotNo_5.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_5.Name = "txtBottleLotNo_5";
            this.txtBottleLotNo_5.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_5.TabIndex = 8;
            // 
            // lblBottleItemName_5
            // 
            this.lblBottleItemName_5.AutoSize = true;
            this.lblBottleItemName_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_5.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_5.Name = "lblBottleItemName_5";
            this.lblBottleItemName_5.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_5.TabIndex = 3;
            this.lblBottleItemName_5.Text = "品目名";
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
            // lblTargetBottle_5
            // 
            this.lblTargetBottle_5.AutoSize = true;
            this.lblTargetBottle_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_5.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_5.Name = "lblTargetBottle_5";
            this.lblTargetBottle_5.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_5.TabIndex = 9;
            this.lblTargetBottle_5.Text = "対象期間";
            // 
            // timeCheck_5
            // 
            this.timeCheck_5.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_5.Name = "timeCheck_5";
            this.timeCheck_5.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_5.TabIndex = 10;
            // 
            // startBottleTime_5
            // 
            this.startBottleTime_5.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_5.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_5.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_5.Name = "startBottleTime_5";
            this.startBottleTime_5.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_5.TabIndex = 11;
            // 
            // lblBottle_5
            // 
            this.lblBottle_5.AutoSize = true;
            this.lblBottle_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_5.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_5.Name = "lblBottle_5";
            this.lblBottle_5.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_5.TabIndex = 12;
            this.lblBottle_5.Text = "～";
            // 
            // endBottleTime_5
            // 
            this.endBottleTime_5.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_5.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_5.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_5.Name = "endBottleTime_5";
            this.endBottleTime_5.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_5.TabIndex = 14;
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
            // tabBottlePage6
            // 
            this.tabBottlePage6.Controls.Add(this.panelStartBottle_6);
            this.tabBottlePage6.Controls.Add(this.dgvStartBottle_6);
            this.tabBottlePage6.Controls.Add(this.panelEndBottle_6);
            this.tabBottlePage6.Controls.Add(this.dgvEndBottle_6);
            this.tabBottlePage6.Controls.Add(this.rdoBackwardBottle_6);
            this.tabBottlePage6.Controls.Add(this.rdoForwardBottle_6);
            this.tabBottlePage6.Controls.Add(this.lblBottleOrderNo_6);
            this.tabBottlePage6.Controls.Add(this.txtBottleOrderNo_6);
            this.tabBottlePage6.Controls.Add(this.txtBottleItemName_6);
            this.tabBottlePage6.Controls.Add(this.txtBottleItemCode_6);
            this.tabBottlePage6.Controls.Add(this.txtBottleLotNo_6);
            this.tabBottlePage6.Controls.Add(this.lblBottleItemName_6);
            this.tabBottlePage6.Controls.Add(this.lblBottleItemCode_6);
            this.tabBottlePage6.Controls.Add(this.lblBottleLotNo_6);
            this.tabBottlePage6.Controls.Add(this.lblTargetBottle_6);
            this.tabBottlePage6.Controls.Add(this.timeCheck_6);
            this.tabBottlePage6.Controls.Add(this.startBottleTime_6);
            this.tabBottlePage6.Controls.Add(this.lblBottle_6);
            this.tabBottlePage6.Controls.Add(this.endBottleTime_6);
            this.tabBottlePage6.Controls.Add(this.btnClearBottle_6);
            this.tabBottlePage6.Controls.Add(this.btnCsvOutputBottle_6);
            this.tabBottlePage6.Controls.Add(this.btnTraceSearch_6);
            this.tabBottlePage6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage6.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage6.Name = "tabBottlePage6";
            this.tabBottlePage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage6.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage6.TabIndex = 17;
            this.tabBottlePage6.Text = "(06)_未設定";
            this.tabBottlePage6.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_6
            // 
            this.panelStartBottle_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_6.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_6.Name = "panelStartBottle_6";
            this.panelStartBottle_6.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_6.TabIndex = 28;
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
            // panelEndBottle_6
            // 
            this.panelEndBottle_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_6.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_6.Name = "panelEndBottle_6";
            this.panelEndBottle_6.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_6.TabIndex = 30;
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
            // lblBottleOrderNo_6
            // 
            this.lblBottleOrderNo_6.AutoSize = true;
            this.lblBottleOrderNo_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_6.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_6.Name = "lblBottleOrderNo_6";
            this.lblBottleOrderNo_6.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_6.TabIndex = 1;
            this.lblBottleOrderNo_6.Text = "製造指図番号";
            // 
            // txtBottleOrderNo_6
            // 
            this.txtBottleOrderNo_6.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_6.Name = "txtBottleOrderNo_6";
            this.txtBottleOrderNo_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_6.TabIndex = 2;
            // 
            // txtBottleItemName_6
            // 
            this.txtBottleItemName_6.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_6.Name = "txtBottleItemName_6";
            this.txtBottleItemName_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_6.TabIndex = 4;
            // 
            // txtBottleItemCode_6
            // 
            this.txtBottleItemCode_6.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_6.Name = "txtBottleItemCode_6";
            this.txtBottleItemCode_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_6.TabIndex = 6;
            // 
            // txtBottleLotNo_6
            // 
            this.txtBottleLotNo_6.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_6.Name = "txtBottleLotNo_6";
            this.txtBottleLotNo_6.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_6.TabIndex = 8;
            // 
            // lblBottleItemName_6
            // 
            this.lblBottleItemName_6.AutoSize = true;
            this.lblBottleItemName_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_6.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_6.Name = "lblBottleItemName_6";
            this.lblBottleItemName_6.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_6.TabIndex = 3;
            this.lblBottleItemName_6.Text = "品目名";
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
            // lblTargetBottle_6
            // 
            this.lblTargetBottle_6.AutoSize = true;
            this.lblTargetBottle_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_6.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_6.Name = "lblTargetBottle_6";
            this.lblTargetBottle_6.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_6.TabIndex = 9;
            this.lblTargetBottle_6.Text = "対象期間";
            // 
            // timeCheck_6
            // 
            this.timeCheck_6.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_6.Name = "timeCheck_6";
            this.timeCheck_6.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_6.TabIndex = 10;
            // 
            // startBottleTime_6
            // 
            this.startBottleTime_6.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_6.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_6.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_6.Name = "startBottleTime_6";
            this.startBottleTime_6.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_6.TabIndex = 11;
            // 
            // lblBottle_6
            // 
            this.lblBottle_6.AutoSize = true;
            this.lblBottle_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_6.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_6.Name = "lblBottle_6";
            this.lblBottle_6.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_6.TabIndex = 12;
            this.lblBottle_6.Text = "～";
            // 
            // endBottleTime_6
            // 
            this.endBottleTime_6.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_6.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_6.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_6.Name = "endBottleTime_6";
            this.endBottleTime_6.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_6.TabIndex = 14;
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
            // tabBottlePage7
            // 
            this.tabBottlePage7.Controls.Add(this.panelStartBottle_7);
            this.tabBottlePage7.Controls.Add(this.dgvStartBottle_7);
            this.tabBottlePage7.Controls.Add(this.panelEndBottle_7);
            this.tabBottlePage7.Controls.Add(this.dgvEndBottle_7);
            this.tabBottlePage7.Controls.Add(this.rdoBackwardBottle_7);
            this.tabBottlePage7.Controls.Add(this.rdoForwardBottle_7);
            this.tabBottlePage7.Controls.Add(this.lblBottleOrderNo_7);
            this.tabBottlePage7.Controls.Add(this.txtBottleOrderNo_7);
            this.tabBottlePage7.Controls.Add(this.txtBottleItemName_7);
            this.tabBottlePage7.Controls.Add(this.txtBottleItemCode_7);
            this.tabBottlePage7.Controls.Add(this.txtBottleLotNo_7);
            this.tabBottlePage7.Controls.Add(this.lblBottleItemName_7);
            this.tabBottlePage7.Controls.Add(this.lblBottleItemCode_7);
            this.tabBottlePage7.Controls.Add(this.lblBottleLotNo_7);
            this.tabBottlePage7.Controls.Add(this.lblTargetBottle_7);
            this.tabBottlePage7.Controls.Add(this.timeCheck_7);
            this.tabBottlePage7.Controls.Add(this.startBottleTime_7);
            this.tabBottlePage7.Controls.Add(this.lblBottle_7);
            this.tabBottlePage7.Controls.Add(this.endBottleTime_7);
            this.tabBottlePage7.Controls.Add(this.btnClearBottle_7);
            this.tabBottlePage7.Controls.Add(this.btnCsvOutputBottle_7);
            this.tabBottlePage7.Controls.Add(this.btnTraceSearch_7);
            this.tabBottlePage7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage7.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage7.Name = "tabBottlePage7";
            this.tabBottlePage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage7.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage7.TabIndex = 18;
            this.tabBottlePage7.Text = "(07)_未設定";
            this.tabBottlePage7.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_7
            // 
            this.panelStartBottle_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_7.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_7.Name = "panelStartBottle_7";
            this.panelStartBottle_7.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_7.TabIndex = 28;
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
            // panelEndBottle_7
            // 
            this.panelEndBottle_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_7.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_7.Name = "panelEndBottle_7";
            this.panelEndBottle_7.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_7.TabIndex = 30;
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
            // lblBottleOrderNo_7
            // 
            this.lblBottleOrderNo_7.AutoSize = true;
            this.lblBottleOrderNo_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_7.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_7.Name = "lblBottleOrderNo_7";
            this.lblBottleOrderNo_7.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_7.TabIndex = 1;
            this.lblBottleOrderNo_7.Text = "製造指図番号";
            // 
            // txtBottleOrderNo_7
            // 
            this.txtBottleOrderNo_7.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_7.Name = "txtBottleOrderNo_7";
            this.txtBottleOrderNo_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_7.TabIndex = 2;
            // 
            // txtBottleItemName_7
            // 
            this.txtBottleItemName_7.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_7.Name = "txtBottleItemName_7";
            this.txtBottleItemName_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_7.TabIndex = 4;
            // 
            // txtBottleItemCode_7
            // 
            this.txtBottleItemCode_7.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_7.Name = "txtBottleItemCode_7";
            this.txtBottleItemCode_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_7.TabIndex = 6;
            // 
            // txtBottleLotNo_7
            // 
            this.txtBottleLotNo_7.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_7.Name = "txtBottleLotNo_7";
            this.txtBottleLotNo_7.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_7.TabIndex = 8;
            // 
            // lblBottleItemName_7
            // 
            this.lblBottleItemName_7.AutoSize = true;
            this.lblBottleItemName_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_7.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_7.Name = "lblBottleItemName_7";
            this.lblBottleItemName_7.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_7.TabIndex = 3;
            this.lblBottleItemName_7.Text = "品目名";
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
            // lblTargetBottle_7
            // 
            this.lblTargetBottle_7.AutoSize = true;
            this.lblTargetBottle_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_7.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_7.Name = "lblTargetBottle_7";
            this.lblTargetBottle_7.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_7.TabIndex = 9;
            this.lblTargetBottle_7.Text = "対象期間";
            // 
            // timeCheck_7
            // 
            this.timeCheck_7.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_7.Name = "timeCheck_7";
            this.timeCheck_7.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_7.TabIndex = 10;
            // 
            // startBottleTime_7
            // 
            this.startBottleTime_7.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_7.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_7.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_7.Name = "startBottleTime_7";
            this.startBottleTime_7.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_7.TabIndex = 11;
            // 
            // lblBottle_7
            // 
            this.lblBottle_7.AutoSize = true;
            this.lblBottle_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_7.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_7.Name = "lblBottle_7";
            this.lblBottle_7.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_7.TabIndex = 12;
            this.lblBottle_7.Text = "～";
            // 
            // endBottleTime_7
            // 
            this.endBottleTime_7.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_7.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_7.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_7.Name = "endBottleTime_7";
            this.endBottleTime_7.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_7.TabIndex = 14;
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
            // tabBottlePage8
            // 
            this.tabBottlePage8.Controls.Add(this.panelStartBottle_8);
            this.tabBottlePage8.Controls.Add(this.dgvStartBottle_8);
            this.tabBottlePage8.Controls.Add(this.panelEndBottle_8);
            this.tabBottlePage8.Controls.Add(this.dgvEndBottle_8);
            this.tabBottlePage8.Controls.Add(this.rdoBackwardBottle_8);
            this.tabBottlePage8.Controls.Add(this.rdoForwardBottle_8);
            this.tabBottlePage8.Controls.Add(this.lblBottleOrderNo_8);
            this.tabBottlePage8.Controls.Add(this.txtBottleOrderNo_8);
            this.tabBottlePage8.Controls.Add(this.txtBottleItemName_8);
            this.tabBottlePage8.Controls.Add(this.txtBottleItemCode_8);
            this.tabBottlePage8.Controls.Add(this.txtBottleLotNo_8);
            this.tabBottlePage8.Controls.Add(this.lblBottleItemName_8);
            this.tabBottlePage8.Controls.Add(this.lblBottleItemCode_8);
            this.tabBottlePage8.Controls.Add(this.lblBottleLotNo_8);
            this.tabBottlePage8.Controls.Add(this.lblTargetBottle_8);
            this.tabBottlePage8.Controls.Add(this.timeCheck_8);
            this.tabBottlePage8.Controls.Add(this.startBottleTime_8);
            this.tabBottlePage8.Controls.Add(this.lblBottle_8);
            this.tabBottlePage8.Controls.Add(this.endBottleTime_8);
            this.tabBottlePage8.Controls.Add(this.btnClearBottle_8);
            this.tabBottlePage8.Controls.Add(this.btnCsvOutputBottle_8);
            this.tabBottlePage8.Controls.Add(this.btnTraceSearch_8);
            this.tabBottlePage8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage8.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage8.Name = "tabBottlePage8";
            this.tabBottlePage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage8.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage8.TabIndex = 19;
            this.tabBottlePage8.Text = "(08)_未設定";
            this.tabBottlePage8.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_8
            // 
            this.panelStartBottle_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_8.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_8.Name = "panelStartBottle_8";
            this.panelStartBottle_8.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_8.TabIndex = 28;
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
            // panelEndBottle_8
            // 
            this.panelEndBottle_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_8.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_8.Name = "panelEndBottle_8";
            this.panelEndBottle_8.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_8.TabIndex = 30;
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
            // lblBottleOrderNo_8
            // 
            this.lblBottleOrderNo_8.AutoSize = true;
            this.lblBottleOrderNo_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_8.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_8.Name = "lblBottleOrderNo_8";
            this.lblBottleOrderNo_8.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_8.TabIndex = 1;
            this.lblBottleOrderNo_8.Text = "製造指図番号";
            // 
            // txtBottleOrderNo_8
            // 
            this.txtBottleOrderNo_8.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_8.Name = "txtBottleOrderNo_8";
            this.txtBottleOrderNo_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_8.TabIndex = 2;
            // 
            // txtBottleItemName_8
            // 
            this.txtBottleItemName_8.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_8.Name = "txtBottleItemName_8";
            this.txtBottleItemName_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_8.TabIndex = 4;
            // 
            // txtBottleItemCode_8
            // 
            this.txtBottleItemCode_8.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_8.Name = "txtBottleItemCode_8";
            this.txtBottleItemCode_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_8.TabIndex = 6;
            // 
            // txtBottleLotNo_8
            // 
            this.txtBottleLotNo_8.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_8.Name = "txtBottleLotNo_8";
            this.txtBottleLotNo_8.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_8.TabIndex = 8;
            // 
            // lblBottleItemName_8
            // 
            this.lblBottleItemName_8.AutoSize = true;
            this.lblBottleItemName_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_8.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_8.Name = "lblBottleItemName_8";
            this.lblBottleItemName_8.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_8.TabIndex = 3;
            this.lblBottleItemName_8.Text = "品目名";
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
            // lblTargetBottle_8
            // 
            this.lblTargetBottle_8.AutoSize = true;
            this.lblTargetBottle_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_8.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_8.Name = "lblTargetBottle_8";
            this.lblTargetBottle_8.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_8.TabIndex = 9;
            this.lblTargetBottle_8.Text = "対象期間";
            // 
            // timeCheck_8
            // 
            this.timeCheck_8.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_8.Name = "timeCheck_8";
            this.timeCheck_8.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_8.TabIndex = 10;
            // 
            // startBottleTime_8
            // 
            this.startBottleTime_8.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_8.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_8.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_8.Name = "startBottleTime_8";
            this.startBottleTime_8.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_8.TabIndex = 11;
            // 
            // lblBottle_8
            // 
            this.lblBottle_8.AutoSize = true;
            this.lblBottle_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_8.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_8.Name = "lblBottle_8";
            this.lblBottle_8.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_8.TabIndex = 12;
            this.lblBottle_8.Text = "～";
            // 
            // endBottleTime_8
            // 
            this.endBottleTime_8.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_8.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_8.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_8.Name = "endBottleTime_8";
            this.endBottleTime_8.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_8.TabIndex = 14;
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
            // tabBottlePage9
            // 
            this.tabBottlePage9.Controls.Add(this.panelStartBottle_9);
            this.tabBottlePage9.Controls.Add(this.dgvStartBottle_9);
            this.tabBottlePage9.Controls.Add(this.panelEndBottle_9);
            this.tabBottlePage9.Controls.Add(this.dgvEndBottle_9);
            this.tabBottlePage9.Controls.Add(this.rdoBackwardBottle_9);
            this.tabBottlePage9.Controls.Add(this.rdoForwardBottle_9);
            this.tabBottlePage9.Controls.Add(this.lblBottleOrderNo_9);
            this.tabBottlePage9.Controls.Add(this.txtBottleOrderNo_9);
            this.tabBottlePage9.Controls.Add(this.txtBottleItemName_9);
            this.tabBottlePage9.Controls.Add(this.txtBottleItemCode_9);
            this.tabBottlePage9.Controls.Add(this.txtBottleLotNo_9);
            this.tabBottlePage9.Controls.Add(this.lblBottleItemName_9);
            this.tabBottlePage9.Controls.Add(this.lblBottleItemCode_9);
            this.tabBottlePage9.Controls.Add(this.lblBottleLotNo_9);
            this.tabBottlePage9.Controls.Add(this.lblTargetBottle_9);
            this.tabBottlePage9.Controls.Add(this.timeCheck_9);
            this.tabBottlePage9.Controls.Add(this.startBottleTime_9);
            this.tabBottlePage9.Controls.Add(this.lblBottle_9);
            this.tabBottlePage9.Controls.Add(this.endBottleTime_9);
            this.tabBottlePage9.Controls.Add(this.btnClearBottle_9);
            this.tabBottlePage9.Controls.Add(this.btnCsvOutputBottle_9);
            this.tabBottlePage9.Controls.Add(this.btnTraceSearch_9);
            this.tabBottlePage9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage9.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage9.Name = "tabBottlePage9";
            this.tabBottlePage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage9.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage9.TabIndex = 20;
            this.tabBottlePage9.Text = "(09)_未設定";
            this.tabBottlePage9.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_9
            // 
            this.panelStartBottle_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_9.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_9.Name = "panelStartBottle_9";
            this.panelStartBottle_9.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_9.TabIndex = 28;
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
            // panelEndBottle_9
            // 
            this.panelEndBottle_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_9.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_9.Name = "panelEndBottle_9";
            this.panelEndBottle_9.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_9.TabIndex = 30;
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
            // lblBottleOrderNo_9
            // 
            this.lblBottleOrderNo_9.AutoSize = true;
            this.lblBottleOrderNo_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_9.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_9.Name = "lblBottleOrderNo_9";
            this.lblBottleOrderNo_9.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_9.TabIndex = 1;
            this.lblBottleOrderNo_9.Text = "製造指図番号";
            // 
            // txtBottleOrderNo_9
            // 
            this.txtBottleOrderNo_9.Location = new System.Drawing.Point(110, 15);
            this.txtBottleOrderNo_9.Name = "txtBottleOrderNo_9";
            this.txtBottleOrderNo_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottleOrderNo_9.TabIndex = 2;
            // 
            // txtBottleItemName_9
            // 
            this.txtBottleItemName_9.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_9.Name = "txtBottleItemName_9";
            this.txtBottleItemName_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_9.TabIndex = 4;
            // 
            // txtBottleItemCode_9
            // 
            this.txtBottleItemCode_9.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_9.Name = "txtBottleItemCode_9";
            this.txtBottleItemCode_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_9.TabIndex = 6;
            // 
            // txtBottleLotNo_9
            // 
            this.txtBottleLotNo_9.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_9.Name = "txtBottleLotNo_9";
            this.txtBottleLotNo_9.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_9.TabIndex = 8;
            // 
            // lblBottleItemName_9
            // 
            this.lblBottleItemName_9.AutoSize = true;
            this.lblBottleItemName_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_9.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_9.Name = "lblBottleItemName_9";
            this.lblBottleItemName_9.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_9.TabIndex = 3;
            this.lblBottleItemName_9.Text = "品目名";
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
            // lblTargetBottle_9
            // 
            this.lblTargetBottle_9.AutoSize = true;
            this.lblTargetBottle_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_9.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_9.Name = "lblTargetBottle_9";
            this.lblTargetBottle_9.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_9.TabIndex = 9;
            this.lblTargetBottle_9.Text = "対象期間";
            // 
            // timeCheck_9
            // 
            this.timeCheck_9.Location = new System.Drawing.Point(367, 162);
            this.timeCheck_9.Name = "timeCheck_9";
            this.timeCheck_9.Size = new System.Drawing.Size(15, 14);
            this.timeCheck_9.TabIndex = 10;
            // 
            // startBottleTime_9
            // 
            this.startBottleTime_9.CustomFormat = "yyyy/MM/dd";
            this.startBottleTime_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.startBottleTime_9.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startBottleTime_9.Location = new System.Drawing.Point(110, 155);
            this.startBottleTime_9.Name = "startBottleTime_9";
            this.startBottleTime_9.Size = new System.Drawing.Size(110, 27);
            this.startBottleTime_9.TabIndex = 11;
            // 
            // lblBottle_9
            // 
            this.lblBottle_9.AutoSize = true;
            this.lblBottle_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottle_9.Location = new System.Drawing.Point(226, 162);
            this.lblBottle_9.Name = "lblBottle_9";
            this.lblBottle_9.Size = new System.Drawing.Size(19, 16);
            this.lblBottle_9.TabIndex = 12;
            this.lblBottle_9.Text = "～";
            // 
            // endBottleTime_9
            // 
            this.endBottleTime_9.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_9.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_9.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_9.Name = "endBottleTime_9";
            this.endBottleTime_9.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_9.TabIndex = 14;
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
            // tabBottlePage10
            // 
            this.tabBottlePage10.Controls.Add(this.panelStartBottle_10);
            this.tabBottlePage10.Controls.Add(this.dgvStartBottle_10);
            this.tabBottlePage10.Controls.Add(this.panelEndBottle_10);
            this.tabBottlePage10.Controls.Add(this.dgvEndBottle_10);
            this.tabBottlePage10.Controls.Add(this.radioButton5);
            this.tabBottlePage10.Controls.Add(this.rdoForwardBottle_10);
            this.tabBottlePage10.Controls.Add(this.lblBottleOrderNo_10);
            this.tabBottlePage10.Controls.Add(this.textBox10);
            this.tabBottlePage10.Controls.Add(this.txtBottleItemName_10);
            this.tabBottlePage10.Controls.Add(this.txtBottleItemCode_10);
            this.tabBottlePage10.Controls.Add(this.txtBottleLotNo_10);
            this.tabBottlePage10.Controls.Add(this.lblBottleItemName_10);
            this.tabBottlePage10.Controls.Add(this.lblBottleItemCode_10);
            this.tabBottlePage10.Controls.Add(this.lblBottleLotNo_10);
            this.tabBottlePage10.Controls.Add(this.lblTargetBottle_10);
            this.tabBottlePage10.Controls.Add(this.timeCheck_10);
            this.tabBottlePage10.Controls.Add(this.dateTimePicker5);
            this.tabBottlePage10.Controls.Add(this.label18);
            this.tabBottlePage10.Controls.Add(this.endBottleTime_10);
            this.tabBottlePage10.Controls.Add(this.btnClearBottle_10);
            this.tabBottlePage10.Controls.Add(this.btnCsvOutputBottle_10);
            this.tabBottlePage10.Controls.Add(this.btnTraceSearch_10);
            this.tabBottlePage10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabBottlePage10.Location = new System.Drawing.Point(4, 34);
            this.tabBottlePage10.Name = "tabBottlePage10";
            this.tabBottlePage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabBottlePage10.Size = new System.Drawing.Size(1896, 902);
            this.tabBottlePage10.TabIndex = 21;
            this.tabBottlePage10.Text = "(10)_未設定";
            this.tabBottlePage10.UseVisualStyleBackColor = true;
            // 
            // panelStartBottle_10
            // 
            this.panelStartBottle_10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartBottle_10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartBottle_10.Location = new System.Drawing.Point(10, 194);
            this.panelStartBottle_10.Name = "panelStartBottle_10";
            this.panelStartBottle_10.Size = new System.Drawing.Size(920, 28);
            this.panelStartBottle_10.TabIndex = 28;
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
            // panelEndBottle_10
            // 
            this.panelEndBottle_10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndBottle_10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndBottle_10.Location = new System.Drawing.Point(966, 194);
            this.panelEndBottle_10.Name = "panelEndBottle_10";
            this.panelEndBottle_10.Size = new System.Drawing.Size(920, 28);
            this.panelEndBottle_10.TabIndex = 30;
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
            // lblBottleOrderNo_10
            // 
            this.lblBottleOrderNo_10.AutoSize = true;
            this.lblBottleOrderNo_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleOrderNo_10.Location = new System.Drawing.Point(20, 20);
            this.lblBottleOrderNo_10.Name = "lblBottleOrderNo_10";
            this.lblBottleOrderNo_10.Size = new System.Drawing.Size(79, 16);
            this.lblBottleOrderNo_10.TabIndex = 1;
            this.lblBottleOrderNo_10.Text = "製造指図番号";
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(110, 15);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(250, 27);
            this.textBox10.TabIndex = 2;
            // 
            // txtBottleItemName_10
            // 
            this.txtBottleItemName_10.Location = new System.Drawing.Point(110, 50);
            this.txtBottleItemName_10.Name = "txtBottleItemName_10";
            this.txtBottleItemName_10.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemName_10.TabIndex = 4;
            // 
            // txtBottleItemCode_10
            // 
            this.txtBottleItemCode_10.Location = new System.Drawing.Point(110, 85);
            this.txtBottleItemCode_10.Name = "txtBottleItemCode_10";
            this.txtBottleItemCode_10.Size = new System.Drawing.Size(250, 27);
            this.txtBottleItemCode_10.TabIndex = 6;
            // 
            // txtBottleLotNo_10
            // 
            this.txtBottleLotNo_10.Location = new System.Drawing.Point(110, 120);
            this.txtBottleLotNo_10.Name = "txtBottleLotNo_10";
            this.txtBottleLotNo_10.Size = new System.Drawing.Size(250, 27);
            this.txtBottleLotNo_10.TabIndex = 8;
            // 
            // lblBottleItemName_10
            // 
            this.lblBottleItemName_10.AutoSize = true;
            this.lblBottleItemName_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBottleItemName_10.Location = new System.Drawing.Point(20, 55);
            this.lblBottleItemName_10.Name = "lblBottleItemName_10";
            this.lblBottleItemName_10.Size = new System.Drawing.Size(43, 16);
            this.lblBottleItemName_10.TabIndex = 3;
            this.lblBottleItemName_10.Text = "品目名";
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
            // lblTargetBottle_10
            // 
            this.lblTargetBottle_10.AutoSize = true;
            this.lblTargetBottle_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetBottle_10.Location = new System.Drawing.Point(20, 160);
            this.lblTargetBottle_10.Name = "lblTargetBottle_10";
            this.lblTargetBottle_10.Size = new System.Drawing.Size(55, 16);
            this.lblTargetBottle_10.TabIndex = 9;
            this.lblTargetBottle_10.Text = "対象期間";
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
            // endBottleTime_10
            // 
            this.endBottleTime_10.CustomFormat = "yyyy/MM/dd";
            this.endBottleTime_10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.endBottleTime_10.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endBottleTime_10.Location = new System.Drawing.Point(251, 155);
            this.endBottleTime_10.Name = "endBottleTime_10";
            this.endBottleTime_10.Size = new System.Drawing.Size(110, 27);
            this.endBottleTime_10.TabIndex = 14;
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
            // BottleTitle
            // 
            this.BottleTitle.AutoSize = true;
            this.BottleTitle.Font = new System.Drawing.Font("游ゴシック", 20F);
            this.BottleTitle.Location = new System.Drawing.Point(785, 8);
            this.BottleTitle.Name = "BottleTitle";
            this.BottleTitle.Size = new System.Drawing.Size(285, 35);
            this.BottleTitle.TabIndex = 50;
            this.BottleTitle.Text = "瓶設備ロットトレース";
            // 
            // rdoBottleTabNameItemCode
            // 
            this.rdoBottleTabNameItemCode.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.rdoBottleTabNameItemCode.Location = new System.Drawing.Point(1389, 31);
            this.rdoBottleTabNameItemCode.Name = "rdoBottleTabNameItemCode";
            this.rdoBottleTabNameItemCode.Size = new System.Drawing.Size(110, 24);
            this.rdoBottleTabNameItemCode.TabIndex = 53;
            this.rdoBottleTabNameItemCode.Text = "品目コード";
            // 
            // selectBottle
            // 
            this.selectBottle.AutoSize = true;
            this.selectBottle.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.selectBottle.Location = new System.Drawing.Point(1265, 10);
            this.selectBottle.Name = "selectBottle";
            this.selectBottle.Size = new System.Drawing.Size(106, 21);
            this.selectBottle.TabIndex = 52;
            this.selectBottle.Text = "タブ名称選択";
            // 
            // rdoBottleTabNameOrder
            // 
            this.rdoBottleTabNameOrder.Checked = true;
            this.rdoBottleTabNameOrder.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.rdoBottleTabNameOrder.Location = new System.Drawing.Point(1389, 7);
            this.rdoBottleTabNameOrder.Name = "rdoBottleTabNameOrder";
            this.rdoBottleTabNameOrder.Size = new System.Drawing.Size(110, 24);
            this.rdoBottleTabNameOrder.TabIndex = 51;
            this.rdoBottleTabNameOrder.TabStop = true;
            this.rdoBottleTabNameOrder.Text = "製造指図番号";
            // 
            // btnBottleExcelOutput
            // 
            this.btnBottleExcelOutput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBottleExcelOutput.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBottleExcelOutput.Location = new System.Drawing.Point(1560, 15);
            this.btnBottleExcelOutput.Name = "btnBottleExcelOutput";
            this.btnBottleExcelOutput.Size = new System.Drawing.Size(150, 50);
            this.btnBottleExcelOutput.TabIndex = 54;
            this.btnBottleExcelOutput.Text = "EXCEL出力";
            // 
            // btnBottleDetectCrossPoints
            // 
            this.btnBottleDetectCrossPoints.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBottleDetectCrossPoints.Location = new System.Drawing.Point(1732, 15);
            this.btnBottleDetectCrossPoints.Name = "btnBottleDetectCrossPoints";
            this.btnBottleDetectCrossPoints.Size = new System.Drawing.Size(150, 50);
            this.btnBottleDetectCrossPoints.TabIndex = 55;
            this.btnBottleDetectCrossPoints.Text = "交点検出";
            // 
            // checkBottle1
            // 
            this.checkBottle1.AutoSize = true;
            this.checkBottle1.Location = new System.Drawing.Point(12, 71);
            this.checkBottle1.Name = "checkBottle1";
            this.checkBottle1.Size = new System.Drawing.Size(15, 14);
            this.checkBottle1.TabIndex = 56;
            this.checkBottle1.UseVisualStyleBackColor = true;
            // 
            // checkBottle2
            // 
            this.checkBottle2.AutoSize = true;
            this.checkBottle2.Location = new System.Drawing.Point(184, 71);
            this.checkBottle2.Name = "checkBottle2";
            this.checkBottle2.Size = new System.Drawing.Size(15, 14);
            this.checkBottle2.TabIndex = 56;
            this.checkBottle2.UseVisualStyleBackColor = true;
            // 
            // checkBottle3
            // 
            this.checkBottle3.AutoSize = true;
            this.checkBottle3.Location = new System.Drawing.Point(358, 71);
            this.checkBottle3.Name = "checkBottle3";
            this.checkBottle3.Size = new System.Drawing.Size(15, 14);
            this.checkBottle3.TabIndex = 56;
            this.checkBottle3.UseVisualStyleBackColor = true;
            // 
            // checkBottle4
            // 
            this.checkBottle4.AutoSize = true;
            this.checkBottle4.Location = new System.Drawing.Point(527, 71);
            this.checkBottle4.Name = "checkBottle4";
            this.checkBottle4.Size = new System.Drawing.Size(15, 14);
            this.checkBottle4.TabIndex = 56;
            this.checkBottle4.UseVisualStyleBackColor = true;
            // 
            // checkBottle5
            // 
            this.checkBottle5.AutoSize = true;
            this.checkBottle5.Location = new System.Drawing.Point(699, 71);
            this.checkBottle5.Name = "checkBottle5";
            this.checkBottle5.Size = new System.Drawing.Size(15, 14);
            this.checkBottle5.TabIndex = 56;
            this.checkBottle5.UseVisualStyleBackColor = true;
            // 
            // checkBottle6
            // 
            this.checkBottle6.AutoSize = true;
            this.checkBottle6.Location = new System.Drawing.Point(871, 71);
            this.checkBottle6.Name = "checkBottle6";
            this.checkBottle6.Size = new System.Drawing.Size(15, 14);
            this.checkBottle6.TabIndex = 56;
            this.checkBottle6.UseVisualStyleBackColor = true;
            // 
            // checkBottle7
            // 
            this.checkBottle7.AutoSize = true;
            this.checkBottle7.Location = new System.Drawing.Point(1042, 71);
            this.checkBottle7.Name = "checkBottle7";
            this.checkBottle7.Size = new System.Drawing.Size(15, 14);
            this.checkBottle7.TabIndex = 56;
            this.checkBottle7.UseVisualStyleBackColor = true;
            // 
            // checkBottle8
            // 
            this.checkBottle8.AutoSize = true;
            this.checkBottle8.Location = new System.Drawing.Point(1212, 71);
            this.checkBottle8.Name = "checkBottle8";
            this.checkBottle8.Size = new System.Drawing.Size(15, 14);
            this.checkBottle8.TabIndex = 56;
            this.checkBottle8.UseVisualStyleBackColor = true;
            // 
            // checkBottle9
            // 
            this.checkBottle9.AutoSize = true;
            this.checkBottle9.Location = new System.Drawing.Point(1389, 71);
            this.checkBottle9.Name = "checkBottle9";
            this.checkBottle9.Size = new System.Drawing.Size(15, 14);
            this.checkBottle9.TabIndex = 56;
            this.checkBottle9.UseVisualStyleBackColor = true;
            // 
            // checkBottle10
            // 
            this.checkBottle10.AutoSize = true;
            this.checkBottle10.Location = new System.Drawing.Point(1560, 71);
            this.checkBottle10.Name = "checkBottle10";
            this.checkBottle10.Size = new System.Drawing.Size(15, 14);
            this.checkBottle10.TabIndex = 56;
            this.checkBottle10.UseVisualStyleBackColor = true;
            // 
            // BottleTraceForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1920, 1024);
            this.Controls.Add(this.checkBottle10);
            this.Controls.Add(this.checkBottle9);
            this.Controls.Add(this.checkBottle8);
            this.Controls.Add(this.checkBottle7);
            this.Controls.Add(this.checkBottle6);
            this.Controls.Add(this.checkBottle5);
            this.Controls.Add(this.checkBottle4);
            this.Controls.Add(this.checkBottle3);
            this.Controls.Add(this.checkBottle2);
            this.Controls.Add(this.checkBottle1);
            this.Controls.Add(this.btnBottleExcelOutput);
            this.Controls.Add(this.btnBottleDetectCrossPoints);
            this.Controls.Add(this.rdoBottleTabNameItemCode);
            this.Controls.Add(this.selectBottle);
            this.Controls.Add(this.rdoBottleTabNameOrder);
            this.Controls.Add(this.BottleTitle);
            this.Controls.Add(this.swichBottleTab);
            this.Controls.Add(this.btnBackToLiquid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "BottleTraceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "瓶設備ロットトレース";
            this.BottleIntersectionTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).EndInit();
            this.tabBottlePage2.ResumeLayout(false);
            this.tabBottlePage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_2)).EndInit();
            this.swichBottleTab.ResumeLayout(false);
            this.tabBottlePage1.ResumeLayout(false);
            this.tabBottlePage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle)).EndInit();
            this.tabBottlePage3.ResumeLayout(false);
            this.tabBottlePage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_3)).EndInit();
            this.tabBottlePage4.ResumeLayout(false);
            this.tabBottlePage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_4)).EndInit();
            this.tabBottlePage5.ResumeLayout(false);
            this.tabBottlePage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_5)).EndInit();
            this.tabBottlePage6.ResumeLayout(false);
            this.tabBottlePage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_6)).EndInit();
            this.tabBottlePage7.ResumeLayout(false);
            this.tabBottlePage7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_7)).EndInit();
            this.tabBottlePage8.ResumeLayout(false);
            this.tabBottlePage8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_8)).EndInit();
            this.tabBottlePage9.ResumeLayout(false);
            this.tabBottlePage9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_9)).EndInit();
            this.tabBottlePage10.ResumeLayout(false);
            this.tabBottlePage10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStartBottle_10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndBottle_10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button btnBackToLiquid;
        private TabPage BottleIntersectionTab;
        private DataGridView dataGridIntersection;
        private TabPage tabBottlePage2;
        private DataGridView dgvStartBottle_2;
        private DataGridView dgvEndBottle_2;
        private RadioButton rdoBackwardBottle_2;
        private RadioButton rdoForwardBottle_2;
        private Label lblBottleOrderNo_2;
        private Panel panelStartBottle_2;
        private Panel panelEndBottle_2;
        private TextBox txtBottleOrderNo_2;
        private TextBox txtBottleItemName_2;
        private TextBox txtBottleItemCode_2;
        private TextBox txtBottleLotNo_2;
        private Label lblBottleItemName_2;
        private Label lblBottleItemCode_2;
        private Label lblBottleLotNo_2;
        private Label lblTargetBottle_2;
        private CheckBox timeCheck_2;
        private DateTimePicker startBottleTime_2;
        private Label lblBottle_2;
        private DateTimePicker endBottleTime_2;
        private Button btnClearBottle_2;
        private Button btnCsvOutputBottle_2;
        private Button btnBottleTraceSearch_2;
        private TabControl swichBottleTab;
        private TabPage tabBottlePage1;
        private Panel panelStartBottle;
        private DataGridView dgvStartBottle;
        private Panel panelEndBottle;
        private DataGridView dgvEndBottle;
        private RadioButton rdoBackwardBottle;
        private RadioButton rdoForwardBottle;
        private Label lblBottleOrderNo;
        private TextBox txtBottleOrderNo;
        private TextBox txtBottleItemName;
        private TextBox txtBottleItemCode;
        private TextBox txtBottleLotNo;
        private Label lblBottleItemName;
        private Label lblBottleItemCode;
        private Label lblBottleLotNo;
        private Label lblTargetBottle;
        private CheckBox timeCheck;
        private DateTimePicker startBottleTime;
        private Label lblTilde;
        private DateTimePicker endBottleTime;
        private Button btnClearBottle;
        private Button btnCsvOutputBottle;
        private Button btnBottleTraceSearch;
        private TabPage tabBottlePage3;
        private Panel panelStartBottle_3;
        private DataGridView dgvStartBottle_3;
        private Panel panelEndBottle_3;
        private DataGridView dgvEndBottle_3;
        private RadioButton rdoBackwardBottle_3;
        private RadioButton rdoForwardBottle_3;
        private Label lblBottleOrderNo_3;
        private TextBox txtBottleOrderNo_3;
        private TextBox txtBottleItemName_3;
        private TextBox txtBottleItemCode_3;
        private TextBox txtBottleLotNo_3;
        private Label lblBottleItemName_3;
        private Label lblBottleItemCode_3;
        private Label lblBottleLotNo_3;
        private Label lblTargetBottle_3;
        private CheckBox timeCheck_3;
        private DateTimePicker startBottleTime_3;
        private Label lblBottle_3;
        private DateTimePicker endBottleTime_3;
        private Button btnClearBottle_3;
        private Button btnCsvOutputBottle_3;
        private Button btnTraceSearch_3;
        private TabPage tabBottlePage4;
        private Panel panelStartBottle_4;
        private DataGridView dgvStartBottle_4;
        private Panel panelEndBottle_4;
        private DataGridView dgvEndBottle_4;
        private RadioButton rdoBackwardBottle_4;
        private RadioButton rdoForwardBottle_4;
        private Label lblOrderNumber_4;
        private TextBox txtBottleOrderNo_4;
        private TextBox textBox7;
        private TextBox txtBottleItemCode_4;
        private TextBox txtBottleLotNo_4;
        private Label lblBottleItemName_4;
        private Label lblBottleItemCode_4;
        private Label lblBottleLotNo_4;
        private Label lblTargetBottle_4;
        private CheckBox timeCheck_4;
        private DateTimePicker startBottleTime_4;
        private Label lblBottle_4;
        private DateTimePicker endBottleTime_4;
        private Button btnClearBottle_4;
        private Button btnCsvOutputBottle_4;
        private Button btnTraceSearch_4;
        private TabPage tabBottlePage5;
        private Panel panelStartBottle_5;
        private DataGridView dgvStartBottle_5;
        private Panel panelEndBottle_5;
        private DataGridView dgvEndBottle_5;
        private RadioButton rdoBackwardBottle_5;
        private RadioButton rdoForwardBottle_5;
        private Label lblBottleOrderNo_5;
        private TextBox txtBottleOrderNo_5;
        private TextBox txtBottleItemName_5;
        private TextBox txtBottleItemCode_5;
        private TextBox txtBottleLotNo_5;
        private Label lblBottleItemName_5;
        private Label lblBottleItemCode_5;
        private Label lblBottleLotNo_5;
        private Label lblTargetBottle_5;
        private CheckBox timeCheck_5;
        private DateTimePicker startBottleTime_5;
        private Label lblBottle_5;
        private DateTimePicker endBottleTime_5;
        private Button btnClearBottle_5;
        private Button btnCsvOutputBottle_5;
        private Button btnTraceSearch_5;
        private TabPage tabBottlePage6;
        private Panel panelStartBottle_6;
        private DataGridView dgvStartBottle_6;
        private Panel panelEndBottle_6;
        private DataGridView dgvEndBottle_6;
        private RadioButton rdoBackwardBottle_6;
        private RadioButton rdoForwardBottle_6;
        private Label lblBottleOrderNo_6;
        private TextBox txtBottleOrderNo_6;
        private TextBox txtBottleItemName_6;
        private TextBox txtBottleItemCode_6;
        private TextBox txtBottleLotNo_6;
        private Label lblBottleItemName_6;
        private Label lblBottleItemCode_6;
        private Label lblBottleLotNo_6;
        private Label lblTargetBottle_6;
        private CheckBox timeCheck_6;
        private DateTimePicker startBottleTime_6;
        private Label lblBottle_6;
        private DateTimePicker endBottleTime_6;
        private Button btnClearBottle_6;
        private Button btnCsvOutputBottle_6;
        private Button btnTraceSearch_6;
        private TabPage tabBottlePage7;
        private Panel panelStartBottle_7;
        private DataGridView dgvStartBottle_7;
        private Panel panelEndBottle_7;
        private DataGridView dgvEndBottle_7;
        private RadioButton rdoBackwardBottle_7;
        private RadioButton rdoForwardBottle_7;
        private Label lblBottleOrderNo_7;
        private TextBox txtBottleOrderNo_7;
        private TextBox txtBottleItemName_7;
        private TextBox txtBottleItemCode_7;
        private TextBox txtBottleLotNo_7;
        private Label lblBottleItemName_7;
        private Label lblBottleItemCode_7;
        private Label lblBottleLotNo_7;
        private Label lblTargetBottle_7;
        private CheckBox timeCheck_7;
        private DateTimePicker startBottleTime_7;
        private Label lblBottle_7;
        private DateTimePicker endBottleTime_7;
        private Button btnClearBottle_7;
        private Button btnCsvOutputBottle_7;
        private Button btnTraceSearch_7;
        private TabPage tabBottlePage8;
        private Panel panelStartBottle_8;
        private DataGridView dgvStartBottle_8;
        private Panel panelEndBottle_8;
        private DataGridView dgvEndBottle_8;
        private RadioButton rdoBackwardBottle_8;
        private RadioButton rdoForwardBottle_8;
        private Label lblBottleOrderNo_8;
        private TextBox txtBottleOrderNo_8;
        private TextBox txtBottleItemName_8;
        private TextBox txtBottleItemCode_8;
        private TextBox txtBottleLotNo_8;
        private Label lblBottleItemName_8;
        private Label lblBottleItemCode_8;
        private Label lblBottleLotNo_8;
        private Label lblTargetBottle_8;
        private CheckBox timeCheck_8;
        private DateTimePicker startBottleTime_8;
        private Label lblBottle_8;
        private DateTimePicker endBottleTime_8;
        private Button btnClearBottle_8;
        private Button btnCsvOutputBottle_8;
        private Button btnTraceSearch_8;
        private TabPage tabBottlePage9;
        private Panel panelStartBottle_9;
        private DataGridView dgvStartBottle_9;
        private Panel panelEndBottle_9;
        private DataGridView dgvEndBottle_9;
        private RadioButton rdoBackwardBottle_9;
        private RadioButton rdoForwardBottle_9;
        private Label lblBottleOrderNo_9;
        private TextBox txtBottleOrderNo_9;
        private TextBox txtBottleItemName_9;
        private TextBox txtBottleItemCode_9;
        private TextBox txtBottleLotNo_9;
        private Label lblBottleItemName_9;
        private Label lblBottleItemCode_9;
        private Label lblBottleLotNo_9;
        private Label lblTargetBottle_9;
        private CheckBox timeCheck_9;
        private DateTimePicker startBottleTime_9;
        private Label lblBottle_9;
        private DateTimePicker endBottleTime_9;
        private Button btnClearBottle_9;
        private Button btnCsvOutputBottle_9;
        private Button btnTraceSearch_9;
        private TabPage tabBottlePage10;
        private Panel panelStartBottle_10;
        private DataGridView dgvStartBottle_10;
        private Panel panelEndBottle_10;
        private DataGridView dgvEndBottle_10;
        private RadioButton radioButton5;
        private RadioButton rdoForwardBottle_10;
        private Label lblBottleOrderNo_10;
        private TextBox textBox10;
        private TextBox txtBottleItemName_10;
        private TextBox txtBottleItemCode_10;
        private TextBox txtBottleLotNo_10;
        private Label lblBottleItemName_10;
        private Label lblBottleItemCode_10;
        private Label lblBottleLotNo_10;
        private Label lblTargetBottle_10;
        private CheckBox timeCheck_10;
        private DateTimePicker dateTimePicker5;
        private Label label18;
        private DateTimePicker endBottleTime_10;
        private Button btnClearBottle_10;
        private Button btnCsvOutputBottle_10;
        private Button btnTraceSearch_10;
        private Label BottleTitle;
        private RadioButton rdoBottleTabNameItemCode;
        private Label selectBottle;
        private RadioButton rdoBottleTabNameOrder;
        private Button btnBottleExcelOutput;
        private Button btnBottleDetectCrossPoints;
        private CheckBox checkBottle1;
        private CheckBox checkBottle2;
        private CheckBox checkBottle3;
        private CheckBox checkBottle4;
        private CheckBox checkBottle5;
        private CheckBox checkBottle6;
        private CheckBox checkBottle7;
        private CheckBox checkBottle8;
        private CheckBox checkBottle9;
        private CheckBox checkBottle10;
    }
}