using System.Windows.Forms;

namespace LotTraceApp
{
    partial class MainForm
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
            this.tcMainTabs = new System.Windows.Forms.TabControl();
            this.tpLotTrace = new System.Windows.Forms.TabPage();
            this.tcSearchSlots = new System.Windows.Forms.TabControl();
            this.tpSlot01 = new System.Windows.Forms.TabPage();
            this.tpSlot02 = new System.Windows.Forms.TabPage();
            this.tpSlot03 = new System.Windows.Forms.TabPage();
            this.tpSlot04 = new System.Windows.Forms.TabPage();
            this.tpSlot05 = new System.Windows.Forms.TabPage();
            this.tpSlot06 = new System.Windows.Forms.TabPage();
            this.tpSlot07 = new System.Windows.Forms.TabPage();
            this.tpSlot08 = new System.Windows.Forms.TabPage();
            this.tpSlot09 = new System.Windows.Forms.TabPage();
            this.tpSlot10 = new System.Windows.Forms.TabPage();
            this.lblProductionOrderNumber = new System.Windows.Forms.Label();
            this.txtProductionOrderNumber = new System.Windows.Forms.TextBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.lblItemCode = new System.Windows.Forms.Label();
            this.txtItemCode = new System.Windows.Forms.TextBox();
            this.lblLotNumber = new System.Windows.Forms.Label();
            this.txtLotNumber = new System.Windows.Forms.TextBox();
            this.lblTargetPeriod = new System.Windows.Forms.Label();
            this.chkUseFrom = new System.Windows.Forms.CheckBox();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTilde = new System.Windows.Forms.Label();
            this.chkUseTo = new System.Windows.Forms.CheckBox();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.grpDirection = new System.Windows.Forms.GroupBox();
            this.rdoForward = new System.Windows.Forms.RadioButton();
            this.rdoBackward = new System.Windows.Forms.RadioButton();
            this.grpTabName = new System.Windows.Forms.GroupBox();
            this.rdoTabNameOrder = new System.Windows.Forms.RadioButton();
            this.rdoTabNameItemCode = new System.Windows.Forms.RadioButton();
            this.chkExcelTarget = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCsvOutput = new System.Windows.Forms.Button();
            this.btnExcelOutput = new System.Windows.Forms.Button();
            this.btnDetectCrossPoints = new System.Windows.Forms.Button();
            this.btnTraceSearch = new System.Windows.Forms.Button();
            this.btnBottleScreen = new System.Windows.Forms.Button();
            this.panelStartHeader = new System.Windows.Forms.Panel();
            this.dataGridStart = new System.Windows.Forms.DataGridView();
            this.panelMiddleHeader = new System.Windows.Forms.Panel();
            this.dataGridMiddle = new System.Windows.Forms.DataGridView();
            this.panelEndHeader = new System.Windows.Forms.Panel();
            this.dataGridEnd = new System.Windows.Forms.DataGridView();
            this.tpIntersection = new System.Windows.Forms.TabPage();
            this.btnIntersectionClear = new System.Windows.Forms.Button();
            this.btnIntersectionCsv = new System.Windows.Forms.Button();
            this.lblIntersectionGrid = new System.Windows.Forms.Label();
            this.dataGridIntersection = new System.Windows.Forms.DataGridView();
            this.tcMainTabs.SuspendLayout();
            this.tpLotTrace.SuspendLayout();
            this.tcSearchSlots.SuspendLayout();
            this.grpDirection.SuspendLayout();
            this.grpTabName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMiddle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEnd)).BeginInit();
            this.tpIntersection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).BeginInit();
            this.SuspendLayout();
            // 
            // tcMainTabs
            // 
            this.tcMainTabs.Controls.Add(this.tpLotTrace);
            this.tcMainTabs.Controls.Add(this.tpIntersection);
            this.tcMainTabs.Location = new System.Drawing.Point(8, 8);
            this.tcMainTabs.Name = "tcMainTabs";
            this.tcMainTabs.SelectedIndex = 0;
            this.tcMainTabs.Size = new System.Drawing.Size(1904, 1008);
            this.tcMainTabs.TabIndex = 0;
            // 
            // tpLotTrace
            // 
            this.tpLotTrace.Controls.Add(this.tcSearchSlots);
            this.tpLotTrace.Controls.Add(this.lblProductionOrderNumber);
            this.tpLotTrace.Controls.Add(this.txtProductionOrderNumber);
            this.tpLotTrace.Controls.Add(this.lblItemName);
            this.tpLotTrace.Controls.Add(this.txtItemName);
            this.tpLotTrace.Controls.Add(this.lblItemCode);
            this.tpLotTrace.Controls.Add(this.txtItemCode);
            this.tpLotTrace.Controls.Add(this.lblLotNumber);
            this.tpLotTrace.Controls.Add(this.txtLotNumber);
            this.tpLotTrace.Controls.Add(this.lblTargetPeriod);
            this.tpLotTrace.Controls.Add(this.chkUseFrom);
            this.tpLotTrace.Controls.Add(this.dtpFrom);
            this.tpLotTrace.Controls.Add(this.lblTilde);
            this.tpLotTrace.Controls.Add(this.chkUseTo);
            this.tpLotTrace.Controls.Add(this.dtpTo);
            this.tpLotTrace.Controls.Add(this.grpDirection);
            this.tpLotTrace.Controls.Add(this.grpTabName);
            this.tpLotTrace.Controls.Add(this.chkExcelTarget);
            this.tpLotTrace.Controls.Add(this.btnClear);
            this.tpLotTrace.Controls.Add(this.btnCsvOutput);
            this.tpLotTrace.Controls.Add(this.btnExcelOutput);
            this.tpLotTrace.Controls.Add(this.btnDetectCrossPoints);
            this.tpLotTrace.Controls.Add(this.btnTraceSearch);
            this.tpLotTrace.Controls.Add(this.btnBottleScreen);
            this.tpLotTrace.Controls.Add(this.panelStartHeader);
            this.tpLotTrace.Controls.Add(this.dataGridStart);
            this.tpLotTrace.Controls.Add(this.panelMiddleHeader);
            this.tpLotTrace.Controls.Add(this.dataGridMiddle);
            this.tpLotTrace.Controls.Add(this.panelEndHeader);
            this.tpLotTrace.Controls.Add(this.dataGridEnd);
            this.tpLotTrace.Location = new System.Drawing.Point(4, 22);
            this.tpLotTrace.Name = "tpLotTrace";
            this.tpLotTrace.Padding = new System.Windows.Forms.Padding(3);
            this.tpLotTrace.Size = new System.Drawing.Size(1896, 982);
            this.tpLotTrace.TabIndex = 0;
            this.tpLotTrace.Text = "液設備ロットトレース";
            this.tpLotTrace.UseVisualStyleBackColor = true;
            // 
            // tcSearchSlots
            // 
            this.tcSearchSlots.Controls.Add(this.tpSlot01);
            this.tcSearchSlots.Controls.Add(this.tpSlot02);
            this.tcSearchSlots.Controls.Add(this.tpSlot03);
            this.tcSearchSlots.Controls.Add(this.tpSlot04);
            this.tcSearchSlots.Controls.Add(this.tpSlot05);
            this.tcSearchSlots.Controls.Add(this.tpSlot06);
            this.tcSearchSlots.Controls.Add(this.tpSlot07);
            this.tcSearchSlots.Controls.Add(this.tpSlot08);
            this.tcSearchSlots.Controls.Add(this.tpSlot09);
            this.tcSearchSlots.Controls.Add(this.tpSlot10);
            this.tcSearchSlots.Location = new System.Drawing.Point(10, 10);
            this.tcSearchSlots.Multiline = true;
            this.tcSearchSlots.Name = "tcSearchSlots";
            this.tcSearchSlots.SelectedIndex = 0;
            this.tcSearchSlots.Size = new System.Drawing.Size(480, 80);
            this.tcSearchSlots.TabIndex = 0;
            // 
            // tpSlot01
            // 
            this.tpSlot01.Location = new System.Drawing.Point(4, 40);
            this.tpSlot01.Name = "tpSlot01";
            this.tpSlot01.Size = new System.Drawing.Size(472, 36);
            this.tpSlot01.TabIndex = 0;
            this.tpSlot01.Text = "(01)_未設定";
            // 
            // tpSlot02
            // 
            this.tpSlot02.Location = new System.Drawing.Point(4, 40);
            this.tpSlot02.Name = "tpSlot02";
            this.tpSlot02.Size = new System.Drawing.Size(472, 36);
            this.tpSlot02.TabIndex = 1;
            this.tpSlot02.Text = "(02)_未設定";
            // 
            // tpSlot03
            // 
            this.tpSlot03.Location = new System.Drawing.Point(4, 40);
            this.tpSlot03.Name = "tpSlot03";
            this.tpSlot03.Size = new System.Drawing.Size(472, 36);
            this.tpSlot03.TabIndex = 2;
            this.tpSlot03.Text = "(03)_未設定";
            // 
            // tpSlot04
            // 
            this.tpSlot04.Location = new System.Drawing.Point(4, 40);
            this.tpSlot04.Name = "tpSlot04";
            this.tpSlot04.Size = new System.Drawing.Size(472, 36);
            this.tpSlot04.TabIndex = 3;
            this.tpSlot04.Text = "(04)_未設定";
            // 
            // tpSlot05
            // 
            this.tpSlot05.Location = new System.Drawing.Point(4, 40);
            this.tpSlot05.Name = "tpSlot05";
            this.tpSlot05.Size = new System.Drawing.Size(472, 36);
            this.tpSlot05.TabIndex = 4;
            this.tpSlot05.Text = "(05)_未設定";
            // 
            // tpSlot06
            // 
            this.tpSlot06.Location = new System.Drawing.Point(4, 40);
            this.tpSlot06.Name = "tpSlot06";
            this.tpSlot06.Size = new System.Drawing.Size(472, 36);
            this.tpSlot06.TabIndex = 5;
            this.tpSlot06.Text = "(06)_未設定";
            // 
            // tpSlot07
            // 
            this.tpSlot07.Location = new System.Drawing.Point(4, 40);
            this.tpSlot07.Name = "tpSlot07";
            this.tpSlot07.Size = new System.Drawing.Size(472, 36);
            this.tpSlot07.TabIndex = 6;
            this.tpSlot07.Text = "(07)_未設定";
            // 
            // tpSlot08
            // 
            this.tpSlot08.Location = new System.Drawing.Point(4, 40);
            this.tpSlot08.Name = "tpSlot08";
            this.tpSlot08.Size = new System.Drawing.Size(472, 36);
            this.tpSlot08.TabIndex = 7;
            this.tpSlot08.Text = "(08)_未設定";
            // 
            // tpSlot09
            // 
            this.tpSlot09.Location = new System.Drawing.Point(4, 40);
            this.tpSlot09.Name = "tpSlot09";
            this.tpSlot09.Size = new System.Drawing.Size(472, 36);
            this.tpSlot09.TabIndex = 8;
            this.tpSlot09.Text = "(09)_未設定";
            // 
            // tpSlot10
            // 
            this.tpSlot10.Location = new System.Drawing.Point(4, 40);
            this.tpSlot10.Name = "tpSlot10";
            this.tpSlot10.Size = new System.Drawing.Size(472, 36);
            this.tpSlot10.TabIndex = 9;
            this.tpSlot10.Text = "(10)_未設定";
            // 
            // lblProductionOrderNumber
            // 
            this.lblProductionOrderNumber.AutoSize = true;
            this.lblProductionOrderNumber.Location = new System.Drawing.Point(510, 18);
            this.lblProductionOrderNumber.Name = "lblProductionOrderNumber";
            this.lblProductionOrderNumber.Size = new System.Drawing.Size(101, 12);
            this.lblProductionOrderNumber.TabIndex = 1;
            this.lblProductionOrderNumber.Text = "製造指図番号 [02]";
            // 
            // txtProductionOrderNumber
            // 
            this.txtProductionOrderNumber.Location = new System.Drawing.Point(600, 15);
            this.txtProductionOrderNumber.Name = "txtProductionOrderNumber";
            this.txtProductionOrderNumber.Size = new System.Drawing.Size(150, 19);
            this.txtProductionOrderNumber.TabIndex = 2;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Location = new System.Drawing.Point(770, 18);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(65, 12);
            this.lblItemName.TabIndex = 3;
            this.lblItemName.Text = "品目名 [03]";
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(820, 15);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(220, 19);
            this.txtItemName.TabIndex = 4;
            // 
            // lblItemCode
            // 
            this.lblItemCode.AutoSize = true;
            this.lblItemCode.Location = new System.Drawing.Point(510, 47);
            this.lblItemCode.Name = "lblItemCode";
            this.lblItemCode.Size = new System.Drawing.Size(80, 12);
            this.lblItemCode.TabIndex = 5;
            this.lblItemCode.Text = "品目コード [04]";
            // 
            // txtItemCode
            // 
            this.txtItemCode.Location = new System.Drawing.Point(600, 44);
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new System.Drawing.Size(150, 19);
            this.txtItemCode.TabIndex = 6;
            // 
            // lblLotNumber
            // 
            this.lblLotNumber.AutoSize = true;
            this.lblLotNumber.Location = new System.Drawing.Point(770, 47);
            this.lblLotNumber.Name = "lblLotNumber";
            this.lblLotNumber.Size = new System.Drawing.Size(77, 12);
            this.lblLotNumber.TabIndex = 7;
            this.lblLotNumber.Text = "ロット番号 [05]";
            // 
            // txtLotNumber
            // 
            this.txtLotNumber.Location = new System.Drawing.Point(820, 44);
            this.txtLotNumber.Name = "txtLotNumber";
            this.txtLotNumber.Size = new System.Drawing.Size(150, 19);
            this.txtLotNumber.TabIndex = 8;
            // 
            // lblTargetPeriod
            // 
            this.lblTargetPeriod.AutoSize = true;
            this.lblTargetPeriod.Location = new System.Drawing.Point(510, 76);
            this.lblTargetPeriod.Name = "lblTargetPeriod";
            this.lblTargetPeriod.Size = new System.Drawing.Size(77, 12);
            this.lblTargetPeriod.TabIndex = 9;
            this.lblTargetPeriod.Text = "対象期間 [06]";
            // 
            // chkUseFrom
            // 
            this.chkUseFrom.Location = new System.Drawing.Point(600, 76);
            this.chkUseFrom.Name = "chkUseFrom";
            this.chkUseFrom.Size = new System.Drawing.Size(15, 14);
            this.chkUseFrom.TabIndex = 10;
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "yyyy/MM/dd";
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(620, 71);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 19);
            this.dtpFrom.TabIndex = 11;
            // 
            // lblTilde
            // 
            this.lblTilde.AutoSize = true;
            this.lblTilde.Location = new System.Drawing.Point(735, 76);
            this.lblTilde.Name = "lblTilde";
            this.lblTilde.Size = new System.Drawing.Size(17, 12);
            this.lblTilde.TabIndex = 12;
            this.lblTilde.Text = "～";
            // 
            // chkUseTo
            // 
            this.chkUseTo.Location = new System.Drawing.Point(755, 76);
            this.chkUseTo.Name = "chkUseTo";
            this.chkUseTo.Size = new System.Drawing.Size(15, 14);
            this.chkUseTo.TabIndex = 13;
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "yyyy/MM/dd";
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(775, 71);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 19);
            this.dtpTo.TabIndex = 14;
            // 
            // grpDirection
            // 
            this.grpDirection.Controls.Add(this.rdoForward);
            this.grpDirection.Controls.Add(this.rdoBackward);
            this.grpDirection.Location = new System.Drawing.Point(940, 71);
            this.grpDirection.Name = "grpDirection";
            this.grpDirection.Size = new System.Drawing.Size(210, 40);
            this.grpDirection.TabIndex = 15;
            this.grpDirection.TabStop = false;
            this.grpDirection.Text = "トレース方向 [07]";
            // 
            // rdoForward
            // 
            this.rdoForward.Checked = true;
            this.rdoForward.Location = new System.Drawing.Point(10, 16);
            this.rdoForward.Name = "rdoForward";
            this.rdoForward.Size = new System.Drawing.Size(104, 24);
            this.rdoForward.TabIndex = 0;
            this.rdoForward.TabStop = true;
            this.rdoForward.Text = "フォワード(追跡)";
            // 
            // rdoBackward
            // 
            this.rdoBackward.Location = new System.Drawing.Point(110, 16);
            this.rdoBackward.Name = "rdoBackward";
            this.rdoBackward.Size = new System.Drawing.Size(104, 24);
            this.rdoBackward.TabIndex = 1;
            this.rdoBackward.Text = "バック(遡及)";
            // 
            // grpTabName
            // 
            this.grpTabName.Controls.Add(this.rdoTabNameOrder);
            this.grpTabName.Controls.Add(this.rdoTabNameItemCode);
            this.grpTabName.Location = new System.Drawing.Point(1160, 10);
            this.grpTabName.Name = "grpTabName";
            this.grpTabName.Size = new System.Drawing.Size(210, 50);
            this.grpTabName.TabIndex = 16;
            this.grpTabName.TabStop = false;
            this.grpTabName.Text = "タブ名称選択 [08]";
            // 
            // rdoTabNameOrder
            // 
            this.rdoTabNameOrder.Checked = true;
            this.rdoTabNameOrder.Location = new System.Drawing.Point(10, 20);
            this.rdoTabNameOrder.Name = "rdoTabNameOrder";
            this.rdoTabNameOrder.Size = new System.Drawing.Size(104, 24);
            this.rdoTabNameOrder.TabIndex = 0;
            this.rdoTabNameOrder.TabStop = true;
            this.rdoTabNameOrder.Text = "製造指図番号";
            // 
            // rdoTabNameItemCode
            // 
            this.rdoTabNameItemCode.Location = new System.Drawing.Point(110, 20);
            this.rdoTabNameItemCode.Name = "rdoTabNameItemCode";
            this.rdoTabNameItemCode.Size = new System.Drawing.Size(104, 24);
            this.rdoTabNameItemCode.TabIndex = 1;
            this.rdoTabNameItemCode.Text = "品目コード";
            // 
            // chkExcelTarget
            // 
            this.chkExcelTarget.Location = new System.Drawing.Point(1160, 71);
            this.chkExcelTarget.Name = "chkExcelTarget";
            this.chkExcelTarget.Size = new System.Drawing.Size(230, 19);
            this.chkExcelTarget.TabIndex = 17;
            this.chkExcelTarget.Text = "このタブをEXCEL出力 / 交点検出対象にする [12]";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(1400, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 30);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "クリア [09]";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCsvOutput
            // 
            this.btnCsvOutput.Location = new System.Drawing.Point(1486, 10);
            this.btnCsvOutput.Name = "btnCsvOutput";
            this.btnCsvOutput.Size = new System.Drawing.Size(90, 30);
            this.btnCsvOutput.TabIndex = 19;
            this.btnCsvOutput.Text = "CSV出力 [10]";
            this.btnCsvOutput.Click += new System.EventHandler(this.btnCsvOutput_Click);
            // 
            // btnExcelOutput
            // 
            this.btnExcelOutput.Location = new System.Drawing.Point(1582, 10);
            this.btnExcelOutput.Name = "btnExcelOutput";
            this.btnExcelOutput.Size = new System.Drawing.Size(100, 30);
            this.btnExcelOutput.TabIndex = 20;
            this.btnExcelOutput.Text = "EXCEL出力 [13]";
            this.btnExcelOutput.Click += new System.EventHandler(this.btnExcelOutput_Click);
            // 
            // btnDetectCrossPoints
            // 
            this.btnDetectCrossPoints.Location = new System.Drawing.Point(1688, 10);
            this.btnDetectCrossPoints.Name = "btnDetectCrossPoints";
            this.btnDetectCrossPoints.Size = new System.Drawing.Size(100, 30);
            this.btnDetectCrossPoints.TabIndex = 21;
            this.btnDetectCrossPoints.Text = "交点検出 [14]";
            this.btnDetectCrossPoints.Click += new System.EventHandler(this.btnDetectCrossPoints_Click);
            // 
            // btnTraceSearch
            // 
            this.btnTraceSearch.Location = new System.Drawing.Point(1400, 50);
            this.btnTraceSearch.Name = "btnTraceSearch";
            this.btnTraceSearch.Size = new System.Drawing.Size(150, 35);
            this.btnTraceSearch.TabIndex = 22;
            this.btnTraceSearch.Text = "トレース検索 [11]";
            this.btnTraceSearch.Click += new System.EventHandler(this.btnTraceSearch_Click);
            // 
            // btnBottleScreen
            // 
            this.btnBottleScreen.Location = new System.Drawing.Point(1556, 50);
            this.btnBottleScreen.Name = "btnBottleScreen";
            this.btnBottleScreen.Size = new System.Drawing.Size(150, 35);
            this.btnBottleScreen.TabIndex = 23;
            this.btnBottleScreen.Text = "瓶設備 [18]";
            this.btnBottleScreen.Click += new System.EventHandler(this.btnBottleTrace_Click);
            // 
            // panelStartHeader
            // 
            this.panelStartHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartHeader.Location = new System.Drawing.Point(10, 120);
            this.panelStartHeader.Name = "panelStartHeader";
            this.panelStartHeader.Size = new System.Drawing.Size(520, 28);
            this.panelStartHeader.TabIndex = 24;
            // 
            // dataGridStart
            // 
            this.dataGridStart.Location = new System.Drawing.Point(10, 148);
            this.dataGridStart.Name = "dataGridStart";
            this.dataGridStart.ReadOnly = true;
            this.dataGridStart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridStart.Size = new System.Drawing.Size(520, 802);
            this.dataGridStart.TabIndex = 25;
            // 
            // panelMiddleHeader
            // 
            this.panelMiddleHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panelMiddleHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMiddleHeader.Location = new System.Drawing.Point(540, 120);
            this.panelMiddleHeader.Name = "panelMiddleHeader";
            this.panelMiddleHeader.Size = new System.Drawing.Size(800, 28);
            this.panelMiddleHeader.TabIndex = 26;
            // 
            // dataGridMiddle
            // 
            this.dataGridMiddle.Location = new System.Drawing.Point(540, 148);
            this.dataGridMiddle.Name = "dataGridMiddle";
            this.dataGridMiddle.ReadOnly = true;
            this.dataGridMiddle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridMiddle.Size = new System.Drawing.Size(800, 802);
            this.dataGridMiddle.TabIndex = 27;
            // 
            // panelEndHeader
            // 
            this.panelEndHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndHeader.Location = new System.Drawing.Point(1350, 120);
            this.panelEndHeader.Name = "panelEndHeader";
            this.panelEndHeader.Size = new System.Drawing.Size(520, 28);
            this.panelEndHeader.TabIndex = 28;
            // 
            // dataGridEnd
            // 
            this.dataGridEnd.Location = new System.Drawing.Point(1350, 148);
            this.dataGridEnd.Name = "dataGridEnd";
            this.dataGridEnd.ReadOnly = true;
            this.dataGridEnd.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridEnd.Size = new System.Drawing.Size(520, 802);
            this.dataGridEnd.TabIndex = 29;
            // 
            // tpIntersection
            // 
            this.tpIntersection.Controls.Add(this.btnIntersectionClear);
            this.tpIntersection.Controls.Add(this.btnIntersectionCsv);
            this.tpIntersection.Controls.Add(this.lblIntersectionGrid);
            this.tpIntersection.Controls.Add(this.dataGridIntersection);
            this.tpIntersection.Location = new System.Drawing.Point(4, 22);
            this.tpIntersection.Name = "tpIntersection";
            this.tpIntersection.Padding = new System.Windows.Forms.Padding(3);
            this.tpIntersection.Size = new System.Drawing.Size(1896, 982);
            this.tpIntersection.TabIndex = 1;
            this.tpIntersection.Text = "交点検出結果";
            this.tpIntersection.UseVisualStyleBackColor = true;
            // 
            // btnIntersectionClear
            // 
            this.btnIntersectionClear.Location = new System.Drawing.Point(10, 10);
            this.btnIntersectionClear.Name = "btnIntersectionClear";
            this.btnIntersectionClear.Size = new System.Drawing.Size(90, 30);
            this.btnIntersectionClear.TabIndex = 0;
            this.btnIntersectionClear.Text = "クリア [23]";
            this.btnIntersectionClear.Click += new System.EventHandler(this.btnIntersectionClear_Click);
            // 
            // btnIntersectionCsv
            // 
            this.btnIntersectionCsv.Location = new System.Drawing.Point(110, 10);
            this.btnIntersectionCsv.Name = "btnIntersectionCsv";
            this.btnIntersectionCsv.Size = new System.Drawing.Size(90, 30);
            this.btnIntersectionCsv.TabIndex = 1;
            this.btnIntersectionCsv.Text = "CSV出力 [24]";
            this.btnIntersectionCsv.Click += new System.EventHandler(this.btnIntersectionCsv_Click);
            // 
            // lblIntersectionGrid
            // 
            this.lblIntersectionGrid.AutoSize = true;
            this.lblIntersectionGrid.Location = new System.Drawing.Point(10, 50);
            this.lblIntersectionGrid.Name = "lblIntersectionGrid";
            this.lblIntersectionGrid.Size = new System.Drawing.Size(101, 12);
            this.lblIntersectionGrid.TabIndex = 2;
            this.lblIntersectionGrid.Text = "交点検出結果 [25]";
            // 
            // dataGridIntersection
            // 
            this.dataGridIntersection.Location = new System.Drawing.Point(10, 70);
            this.dataGridIntersection.Name = "dataGridIntersection";
            this.dataGridIntersection.ReadOnly = true;
            this.dataGridIntersection.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridIntersection.Size = new System.Drawing.Size(1860, 880);
            this.dataGridIntersection.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1920, 1024);
            this.Controls.Add(this.tcMainTabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "液設備ロットトレース";
            this.tcMainTabs.ResumeLayout(false);
            this.tpLotTrace.ResumeLayout(false);
            this.tpLotTrace.PerformLayout();
            this.tcSearchSlots.ResumeLayout(false);
            this.grpDirection.ResumeLayout(false);
            this.grpTabName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMiddle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEnd)).EndInit();
            this.tpIntersection.ResumeLayout(false);
            this.tpIntersection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // === コントロールフィールド ===
        private TabControl tcMainTabs;
        private TabPage tpLotTrace;
        private TabPage tpIntersection;

        // [01]
        private TabControl tcSearchSlots;
        private TabPage tpSlot01;
        private TabPage tpSlot02;
        private TabPage tpSlot03;
        private TabPage tpSlot04;
        private TabPage tpSlot05;
        private TabPage tpSlot06;
        private TabPage tpSlot07;
        private TabPage tpSlot08;
        private TabPage tpSlot09;
        private TabPage tpSlot10;

        // [02]〜[06]
        private Label lblProductionOrderNumber;
        private TextBox txtProductionOrderNumber;
        private Label lblItemName;
        private TextBox txtItemName;
        private Label lblItemCode;
        private TextBox txtItemCode;
        private Label lblLotNumber;
        private TextBox txtLotNumber;
        private Label lblTargetPeriod;
        private CheckBox chkUseFrom;
        private DateTimePicker dtpFrom;
        private Label lblTilde;
        private CheckBox chkUseTo;
        private DateTimePicker dtpTo;

        // [07]
        private GroupBox grpDirection;
        private RadioButton rdoForward;
        private RadioButton rdoBackward;

        // [08]
        private GroupBox grpTabName;
        private RadioButton rdoTabNameOrder;
        private RadioButton rdoTabNameItemCode;

        // [09]〜[14], [18]
        private CheckBox chkExcelTarget; // [12]
        private Button btnClear;         // [09]
        private Button btnCsvOutput;     // [10]
        private Button btnExcelOutput;   // [13]
        private Button btnDetectCrossPoints; // [14]
        private Button btnTraceSearch;   // [11]
        private Button btnBottleScreen;  // [18]

        // [15]〜[17]
        
        private DataGridView dataGridStart;
        
        private DataGridView dataGridMiddle;
        
        private DataGridView dataGridEnd;

        // [23]〜[25]
        private Button btnIntersectionClear;
        private Button btnIntersectionCsv;
        private Label lblIntersectionGrid;
        private DataGridView dataGridIntersection;

        private System.Windows.Forms.Panel panelStartHeader;
        private System.Windows.Forms.Panel panelMiddleHeader;
        private System.Windows.Forms.Panel panelMiddleHeaderInner;
        private System.Windows.Forms.Panel panelEndHeader;
    }
}