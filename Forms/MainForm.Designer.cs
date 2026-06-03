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
            this.components = new System.ComponentModel.Container();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rdoBackward = new System.Windows.Forms.RadioButton();
            this.rdoForward = new System.Windows.Forms.RadioButton();
            this.lblProductionOrderNumber = new System.Windows.Forms.Label();
            this.txtProductionOrderNumber = new System.Windows.Forms.TextBox();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.txtItemCode = new System.Windows.Forms.TextBox();
            this.txtLotNumber = new System.Windows.Forms.TextBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblItemCode = new System.Windows.Forms.Label();
            this.lblLotNumber = new System.Windows.Forms.Label();
            this.lblTargetPeriod = new System.Windows.Forms.Label();
            this.chkUseFrom = new System.Windows.Forms.CheckBox();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTilde = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCsvOutput = new System.Windows.Forms.Button();
            this.btnTraceSearch = new System.Windows.Forms.Button();
            this.panelStartHeader = new System.Windows.Forms.Panel();
            this.dataGridStart = new System.Windows.Forms.DataGridView();
            this.panelMiddleHeader = new System.Windows.Forms.Panel();
            this.dataGridMiddle = new System.Windows.Forms.DataGridView();
            this.panelEndHeader = new System.Windows.Forms.Panel();
            this.dataGridEnd = new System.Windows.Forms.DataGridView();
            this.swichTab = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
            this.label14 = new System.Windows.Forms.Label();
            this.dateTimePicker4 = new System.Windows.Forms.DateTimePicker();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.panel6 = new System.Windows.Forms.Panel();
            this.dataGridView6 = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker5 = new System.Windows.Forms.DateTimePicker();
            this.label20 = new System.Windows.Forms.Label();
            this.dateTimePicker6 = new System.Windows.Forms.DateTimePicker();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.dataGridView7 = new System.Windows.Forms.DataGridView();
            this.panel8 = new System.Windows.Forms.Panel();
            this.dataGridView8 = new System.Windows.Forms.DataGridView();
            this.panel9 = new System.Windows.Forms.Panel();
            this.dataGridView9 = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.radioButton8 = new System.Windows.Forms.RadioButton();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker7 = new System.Windows.Forms.DateTimePicker();
            this.label26 = new System.Windows.Forms.Label();
            this.dateTimePicker8 = new System.Windows.Forms.DateTimePicker();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.panel10 = new System.Windows.Forms.Panel();
            this.dataGridView10 = new System.Windows.Forms.DataGridView();
            this.panel11 = new System.Windows.Forms.Panel();
            this.dataGridView11 = new System.Windows.Forms.DataGridView();
            this.panel12 = new System.Windows.Forms.Panel();
            this.dataGridView12 = new System.Windows.Forms.DataGridView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.label27 = new System.Windows.Forms.Label();
            this.textBox17 = new System.Windows.Forms.TextBox();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.textBox20 = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker9 = new System.Windows.Forms.DateTimePicker();
            this.label32 = new System.Windows.Forms.Label();
            this.dateTimePicker10 = new System.Windows.Forms.DateTimePicker();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.panel13 = new System.Windows.Forms.Panel();
            this.dataGridView13 = new System.Windows.Forms.DataGridView();
            this.panel14 = new System.Windows.Forms.Panel();
            this.dataGridView14 = new System.Windows.Forms.DataGridView();
            this.panel15 = new System.Windows.Forms.Panel();
            this.dataGridView15 = new System.Windows.Forms.DataGridView();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.radioButton12 = new System.Windows.Forms.RadioButton();
            this.label33 = new System.Windows.Forms.Label();
            this.textBox21 = new System.Windows.Forms.TextBox();
            this.textBox22 = new System.Windows.Forms.TextBox();
            this.textBox23 = new System.Windows.Forms.TextBox();
            this.textBox24 = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker11 = new System.Windows.Forms.DateTimePicker();
            this.label38 = new System.Windows.Forms.Label();
            this.dateTimePicker12 = new System.Windows.Forms.DateTimePicker();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.panel16 = new System.Windows.Forms.Panel();
            this.dataGridView16 = new System.Windows.Forms.DataGridView();
            this.panel17 = new System.Windows.Forms.Panel();
            this.dataGridView17 = new System.Windows.Forms.DataGridView();
            this.panel18 = new System.Windows.Forms.Panel();
            this.dataGridView18 = new System.Windows.Forms.DataGridView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.radioButton13 = new System.Windows.Forms.RadioButton();
            this.radioButton14 = new System.Windows.Forms.RadioButton();
            this.label39 = new System.Windows.Forms.Label();
            this.textBox25 = new System.Windows.Forms.TextBox();
            this.textBox26 = new System.Windows.Forms.TextBox();
            this.textBox27 = new System.Windows.Forms.TextBox();
            this.textBox28 = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker13 = new System.Windows.Forms.DateTimePicker();
            this.label44 = new System.Windows.Forms.Label();
            this.dateTimePicker14 = new System.Windows.Forms.DateTimePicker();
            this.button19 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.panel19 = new System.Windows.Forms.Panel();
            this.dataGridView19 = new System.Windows.Forms.DataGridView();
            this.panel20 = new System.Windows.Forms.Panel();
            this.dataGridView20 = new System.Windows.Forms.DataGridView();
            this.panel21 = new System.Windows.Forms.Panel();
            this.dataGridView21 = new System.Windows.Forms.DataGridView();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.radioButton15 = new System.Windows.Forms.RadioButton();
            this.radioButton16 = new System.Windows.Forms.RadioButton();
            this.label45 = new System.Windows.Forms.Label();
            this.textBox29 = new System.Windows.Forms.TextBox();
            this.textBox30 = new System.Windows.Forms.TextBox();
            this.textBox31 = new System.Windows.Forms.TextBox();
            this.textBox32 = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker15 = new System.Windows.Forms.DateTimePicker();
            this.label50 = new System.Windows.Forms.Label();
            this.dateTimePicker16 = new System.Windows.Forms.DateTimePicker();
            this.button22 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.button24 = new System.Windows.Forms.Button();
            this.panel22 = new System.Windows.Forms.Panel();
            this.dataGridView22 = new System.Windows.Forms.DataGridView();
            this.panel23 = new System.Windows.Forms.Panel();
            this.dataGridView23 = new System.Windows.Forms.DataGridView();
            this.panel24 = new System.Windows.Forms.Panel();
            this.dataGridView24 = new System.Windows.Forms.DataGridView();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.radioButton17 = new System.Windows.Forms.RadioButton();
            this.radioButton18 = new System.Windows.Forms.RadioButton();
            this.label51 = new System.Windows.Forms.Label();
            this.textBox33 = new System.Windows.Forms.TextBox();
            this.textBox34 = new System.Windows.Forms.TextBox();
            this.textBox35 = new System.Windows.Forms.TextBox();
            this.textBox36 = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker17 = new System.Windows.Forms.DateTimePicker();
            this.label56 = new System.Windows.Forms.Label();
            this.dateTimePicker18 = new System.Windows.Forms.DateTimePicker();
            this.button25 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button27 = new System.Windows.Forms.Button();
            this.panel25 = new System.Windows.Forms.Panel();
            this.dataGridView25 = new System.Windows.Forms.DataGridView();
            this.panel26 = new System.Windows.Forms.Panel();
            this.dataGridView26 = new System.Windows.Forms.DataGridView();
            this.panel27 = new System.Windows.Forms.Panel();
            this.dataGridView27 = new System.Windows.Forms.DataGridView();
            this.IntersectionTab = new System.Windows.Forms.TabPage();
            this.dataGridIntersection = new System.Windows.Forms.DataGridView();
            this.btnBottleScreen = new System.Windows.Forms.Button();
            this.btnDetectCrossPoints = new System.Windows.Forms.Button();
            this.btnExcelOutput = new System.Windows.Forms.Button();
            this.rdoTabNameItemCode = new System.Windows.Forms.RadioButton();
            this.rdoTabNameOrder = new System.Windows.Forms.RadioButton();
            this.check1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.check2 = new System.Windows.Forms.CheckBox();
            this.check3 = new System.Windows.Forms.CheckBox();
            this.check4 = new System.Windows.Forms.CheckBox();
            this.check5 = new System.Windows.Forms.CheckBox();
            this.check6 = new System.Windows.Forms.CheckBox();
            this.check7 = new System.Windows.Forms.CheckBox();
            this.check8 = new System.Windows.Forms.CheckBox();
            this.check10 = new System.Windows.Forms.CheckBox();
            this.check9 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMiddle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEnd)).BeginInit();
            this.swichTab.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView9)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView12)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView15)).BeginInit();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView18)).BeginInit();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView21)).BeginInit();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView24)).BeginInit();
            this.tabPage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView27)).BeginInit();
            this.IntersectionTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rdoBackward);
            this.tabPage1.Controls.Add(this.rdoForward);
            this.tabPage1.Controls.Add(this.lblProductionOrderNumber);
            this.tabPage1.Controls.Add(this.txtProductionOrderNumber);
            this.tabPage1.Controls.Add(this.txtItemName);
            this.tabPage1.Controls.Add(this.txtItemCode);
            this.tabPage1.Controls.Add(this.txtLotNumber);
            this.tabPage1.Controls.Add(this.lblItemName);
            this.tabPage1.Controls.Add(this.lblItemCode);
            this.tabPage1.Controls.Add(this.lblLotNumber);
            this.tabPage1.Controls.Add(this.lblTargetPeriod);
            this.tabPage1.Controls.Add(this.chkUseFrom);
            this.tabPage1.Controls.Add(this.dtpFrom);
            this.tabPage1.Controls.Add(this.lblTilde);
            this.tabPage1.Controls.Add(this.dtpTo);
            this.tabPage1.Controls.Add(this.btnClear);
            this.tabPage1.Controls.Add(this.btnCsvOutput);
            this.tabPage1.Controls.Add(this.btnTraceSearch);
            this.tabPage1.Controls.Add(this.panelStartHeader);
            this.tabPage1.Controls.Add(this.dataGridStart);
            this.tabPage1.Controls.Add(this.panelMiddleHeader);
            this.tabPage1.Controls.Add(this.dataGridMiddle);
            this.tabPage1.Controls.Add(this.panelEndHeader);
            this.tabPage1.Controls.Add(this.dataGridEnd);
            this.tabPage1.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1896, 902);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "(01)_未設定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rdoBackward
            // 
            this.rdoBackward.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.rdoBackward.Location = new System.Drawing.Point(540, 55);
            this.rdoBackward.Name = "rdoBackward";
            this.rdoBackward.Size = new System.Drawing.Size(220, 30);
            this.rdoBackward.TabIndex = 1;
            this.rdoBackward.Text = "トレースバック(遡及)";
            // 
            // rdoForward
            // 
            this.rdoForward.Checked = true;
            this.rdoForward.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rdoForward.Location = new System.Drawing.Point(540, 20);
            this.rdoForward.Name = "rdoForward";
            this.rdoForward.Size = new System.Drawing.Size(220, 30);
            this.rdoForward.TabIndex = 0;
            this.rdoForward.TabStop = true;
            this.rdoForward.Text = "トレースフォワード(追跡)";
            // 
            // lblProductionOrderNumber
            // 
            this.lblProductionOrderNumber.AutoSize = true;
            this.lblProductionOrderNumber.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblProductionOrderNumber.Location = new System.Drawing.Point(20, 20);
            this.lblProductionOrderNumber.Name = "lblProductionOrderNumber";
            this.lblProductionOrderNumber.Size = new System.Drawing.Size(79, 16);
            this.lblProductionOrderNumber.TabIndex = 1;
            this.lblProductionOrderNumber.Text = "製造指図番号";
            // 
            // txtProductionOrderNumber
            // 
            this.txtProductionOrderNumber.Location = new System.Drawing.Point(110, 15);
            this.txtProductionOrderNumber.Name = "txtProductionOrderNumber";
            this.txtProductionOrderNumber.Size = new System.Drawing.Size(250, 27);
            this.txtProductionOrderNumber.TabIndex = 2;
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(110, 50);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(250, 27);
            this.txtItemName.TabIndex = 4;
            // 
            // txtItemCode
            // 
            this.txtItemCode.Location = new System.Drawing.Point(110, 85);
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new System.Drawing.Size(250, 27);
            this.txtItemCode.TabIndex = 6;
            // 
            // txtLotNumber
            // 
            this.txtLotNumber.Location = new System.Drawing.Point(110, 120);
            this.txtLotNumber.Name = "txtLotNumber";
            this.txtLotNumber.Size = new System.Drawing.Size(250, 27);
            this.txtLotNumber.TabIndex = 8;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblItemName.Location = new System.Drawing.Point(20, 55);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(43, 16);
            this.lblItemName.TabIndex = 3;
            this.lblItemName.Text = "品目名";
            // 
            // lblItemCode
            // 
            this.lblItemCode.AutoSize = true;
            this.lblItemCode.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblItemCode.Location = new System.Drawing.Point(20, 90);
            this.lblItemCode.Name = "lblItemCode";
            this.lblItemCode.Size = new System.Drawing.Size(67, 16);
            this.lblItemCode.TabIndex = 5;
            this.lblItemCode.Text = "品目コード";
            // 
            // lblLotNumber
            // 
            this.lblLotNumber.AutoSize = true;
            this.lblLotNumber.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLotNumber.Location = new System.Drawing.Point(20, 125);
            this.lblLotNumber.Name = "lblLotNumber";
            this.lblLotNumber.Size = new System.Drawing.Size(67, 16);
            this.lblLotNumber.TabIndex = 7;
            this.lblLotNumber.Text = "ロット番号";
            // 
            // lblTargetPeriod
            // 
            this.lblTargetPeriod.AutoSize = true;
            this.lblTargetPeriod.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTargetPeriod.Location = new System.Drawing.Point(20, 160);
            this.lblTargetPeriod.Name = "lblTargetPeriod";
            this.lblTargetPeriod.Size = new System.Drawing.Size(55, 16);
            this.lblTargetPeriod.TabIndex = 9;
            this.lblTargetPeriod.Text = "対象期間";
            // 
            // chkUseFrom
            // 
            this.chkUseFrom.Location = new System.Drawing.Point(367, 162);
            this.chkUseFrom.Name = "chkUseFrom";
            this.chkUseFrom.Size = new System.Drawing.Size(15, 14);
            this.chkUseFrom.TabIndex = 10;
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "yyyy/MM/dd";
            this.dtpFrom.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(110, 155);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 27);
            this.dtpFrom.TabIndex = 11;
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
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "yyyy/MM/dd";
            this.dtpTo.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(251, 155);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 27);
            this.dtpTo.TabIndex = 14;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnClear.Location = new System.Drawing.Point(1377, 15);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(150, 50);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "クリア";
            // 
            // btnCsvOutput
            // 
            this.btnCsvOutput.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnCsvOutput.Location = new System.Drawing.Point(1548, 15);
            this.btnCsvOutput.Name = "btnCsvOutput";
            this.btnCsvOutput.Size = new System.Drawing.Size(150, 50);
            this.btnCsvOutput.TabIndex = 19;
            this.btnCsvOutput.Text = "CSV出力";
            this.btnCsvOutput.Click += new System.EventHandler(this.Csv_FromAnyTab_Click);
            // 
            // btnTraceSearch
            // 
            this.btnTraceSearch.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.btnTraceSearch.Location = new System.Drawing.Point(1720, 15);
            this.btnTraceSearch.Name = "btnTraceSearch";
            this.btnTraceSearch.Size = new System.Drawing.Size(150, 50);
            this.btnTraceSearch.TabIndex = 22;
            this.btnTraceSearch.Text = "トレース検索";
            this.btnTraceSearch.Click += new System.EventHandler(this.TraceSearch_FromAnyTab_Click);
            // 
            // panelStartHeader
            // 
            this.panelStartHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panelStartHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStartHeader.Location = new System.Drawing.Point(26, 194);
            this.panelStartHeader.Name = "panelStartHeader";
            this.panelStartHeader.Size = new System.Drawing.Size(504, 28);
            this.panelStartHeader.TabIndex = 24;
            // 
            // dataGridStart
            // 
            this.dataGridStart.Location = new System.Drawing.Point(26, 222);
            this.dataGridStart.Name = "dataGridStart";
            this.dataGridStart.ReadOnly = true;
            this.dataGridStart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridStart.Size = new System.Drawing.Size(504, 626);
            this.dataGridStart.TabIndex = 25;
            // 
            // panelMiddleHeader
            // 
            this.panelMiddleHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panelMiddleHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMiddleHeader.Location = new System.Drawing.Point(540, 194);
            this.panelMiddleHeader.Name = "panelMiddleHeader";
            this.panelMiddleHeader.Size = new System.Drawing.Size(800, 28);
            this.panelMiddleHeader.TabIndex = 26;
            // 
            // dataGridMiddle
            // 
            this.dataGridMiddle.Location = new System.Drawing.Point(540, 222);
            this.dataGridMiddle.Name = "dataGridMiddle";
            this.dataGridMiddle.ReadOnly = true;
            this.dataGridMiddle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridMiddle.Size = new System.Drawing.Size(800, 626);
            this.dataGridMiddle.TabIndex = 27;
            // 
            // panelEndHeader
            // 
            this.panelEndHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panelEndHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEndHeader.Location = new System.Drawing.Point(1350, 194);
            this.panelEndHeader.Name = "panelEndHeader";
            this.panelEndHeader.Size = new System.Drawing.Size(520, 28);
            this.panelEndHeader.TabIndex = 28;
            // 
            // dataGridEnd
            // 
            this.dataGridEnd.Location = new System.Drawing.Point(1350, 222);
            this.dataGridEnd.Name = "dataGridEnd";
            this.dataGridEnd.ReadOnly = true;
            this.dataGridEnd.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridEnd.Size = new System.Drawing.Size(520, 626);
            this.dataGridEnd.TabIndex = 29;
            // 
            // swichTab
            // 
            this.swichTab.Controls.Add(this.tabPage1);
            this.swichTab.Controls.Add(this.tabPage2);
            this.swichTab.Controls.Add(this.tabPage3);
            this.swichTab.Controls.Add(this.tabPage4);
            this.swichTab.Controls.Add(this.tabPage5);
            this.swichTab.Controls.Add(this.tabPage6);
            this.swichTab.Controls.Add(this.tabPage7);
            this.swichTab.Controls.Add(this.tabPage8);
            this.swichTab.Controls.Add(this.tabPage9);
            this.swichTab.Controls.Add(this.tabPage10);
            this.swichTab.Controls.Add(this.IntersectionTab);
            this.swichTab.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.swichTab.ItemSize = new System.Drawing.Size(172, 30);
            this.swichTab.Location = new System.Drawing.Point(8, 85);
            this.swichTab.Name = "swichTab";
            this.swichTab.SelectedIndex = 0;
            this.swichTab.Size = new System.Drawing.Size(1904, 940);
            this.swichTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.swichTab.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.radioButton1);
            this.tabPage2.Controls.Add(this.radioButton2);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.textBox2);
            this.tabPage2.Controls.Add(this.textBox3);
            this.tabPage2.Controls.Add(this.textBox4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.checkBox2);
            this.tabPage2.Controls.Add(this.dateTimePicker1);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.dateTimePicker2);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Controls.Add(this.dataGridView3);
            this.tabPage2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1896, 902);
            this.tabPage2.TabIndex = 4;
            this.tabPage2.Text = "(02)_未設定";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton1.Location = new System.Drawing.Point(540, 55);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(220, 30);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.Text = "トレースバック(遡及)";
            // 
            // radioButton2
            // 
            this.radioButton2.Checked = true;
            this.radioButton2.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton2.Location = new System.Drawing.Point(540, 20);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(220, 30);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "トレースフォワード(追跡)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(20, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "製造指図番号";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(110, 15);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(250, 27);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(110, 50);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(250, 27);
            this.textBox2.TabIndex = 4;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(110, 85);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(250, 27);
            this.textBox3.TabIndex = 6;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(110, 120);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(250, 27);
            this.textBox4.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(20, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "品目名";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(20, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "品目コード";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(20, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "ロット番号";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(20, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "対象期間";
            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(367, 162);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 10;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker1.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker1.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(226, 162);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 16);
            this.label7.TabIndex = 12;
            this.label7.Text = "～";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker2.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker2.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button1.Location = new System.Drawing.Point(1377, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 50);
            this.button1.TabIndex = 18;
            this.button1.Text = "クリア";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button2.Location = new System.Drawing.Point(1548, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 50);
            this.button2.TabIndex = 19;
            this.button2.Text = "CSV出力";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button3.Location = new System.Drawing.Point(1720, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 50);
            this.button3.TabIndex = 22;
            this.button3.Text = "トレース検索";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(10, 194);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(520, 28);
            this.panel1.TabIndex = 24;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Location = new System.Drawing.Point(10, 222);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(520, 630);
            this.dataGridView1.TabIndex = 25;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(540, 194);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 28);
            this.panel2.TabIndex = 26;
            // 
            // dataGridView2
            // 
            this.dataGridView2.Location = new System.Drawing.Point(540, 222);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(800, 630);
            this.dataGridView2.TabIndex = 27;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(1350, 194);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(520, 28);
            this.panel3.TabIndex = 28;
            // 
            // dataGridView3
            // 
            this.dataGridView3.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView3.Size = new System.Drawing.Size(520, 630);
            this.dataGridView3.TabIndex = 29;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.radioButton3);
            this.tabPage3.Controls.Add(this.radioButton4);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.textBox5);
            this.tabPage3.Controls.Add(this.textBox6);
            this.tabPage3.Controls.Add(this.textBox7);
            this.tabPage3.Controls.Add(this.textBox8);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.checkBox3);
            this.tabPage3.Controls.Add(this.dateTimePicker3);
            this.tabPage3.Controls.Add(this.label14);
            this.tabPage3.Controls.Add(this.dateTimePicker4);
            this.tabPage3.Controls.Add(this.button4);
            this.tabPage3.Controls.Add(this.button5);
            this.tabPage3.Controls.Add(this.button6);
            this.tabPage3.Controls.Add(this.panel4);
            this.tabPage3.Controls.Add(this.dataGridView4);
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Controls.Add(this.dataGridView5);
            this.tabPage3.Controls.Add(this.panel6);
            this.tabPage3.Controls.Add(this.dataGridView6);
            this.tabPage3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage3.Location = new System.Drawing.Point(4, 34);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1896, 902);
            this.tabPage3.TabIndex = 5;
            this.tabPage3.Text = "(03)_未設定";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton3.Location = new System.Drawing.Point(540, 55);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(220, 30);
            this.radioButton3.TabIndex = 1;
            this.radioButton3.Text = "トレースバック(遡及)";
            // 
            // radioButton4
            // 
            this.radioButton4.Checked = true;
            this.radioButton4.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton4.Location = new System.Drawing.Point(540, 20);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(220, 30);
            this.radioButton4.TabIndex = 0;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "トレースフォワード(追跡)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.Location = new System.Drawing.Point(20, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 16);
            this.label9.TabIndex = 1;
            this.label9.Text = "製造指図番号";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(110, 15);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(250, 27);
            this.textBox5.TabIndex = 2;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(110, 50);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(250, 27);
            this.textBox6.TabIndex = 4;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(110, 85);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(250, 27);
            this.textBox7.TabIndex = 6;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(110, 120);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(250, 27);
            this.textBox8.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(20, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 16);
            this.label10.TabIndex = 3;
            this.label10.Text = "品目名";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label11.Location = new System.Drawing.Point(20, 90);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 16);
            this.label11.TabIndex = 5;
            this.label11.Text = "品目コード";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label12.Location = new System.Drawing.Point(20, 125);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 16);
            this.label12.TabIndex = 7;
            this.label12.Text = "ロット番号";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label13.Location = new System.Drawing.Point(20, 160);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 16);
            this.label13.TabIndex = 9;
            this.label13.Text = "対象期間";
            // 
            // checkBox3
            // 
            this.checkBox3.Location = new System.Drawing.Point(367, 162);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 10;
            // 
            // dateTimePicker3
            // 
            this.dateTimePicker3.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker3.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker3.TabIndex = 11;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label14.Location = new System.Drawing.Point(226, 162);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(19, 16);
            this.label14.TabIndex = 12;
            this.label14.Text = "～";
            // 
            // dateTimePicker4
            // 
            this.dateTimePicker4.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker4.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker4.Name = "dateTimePicker4";
            this.dateTimePicker4.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker4.TabIndex = 14;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button4.Location = new System.Drawing.Point(1377, 15);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(150, 50);
            this.button4.TabIndex = 18;
            this.button4.Text = "クリア";
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button5.Location = new System.Drawing.Point(1548, 15);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(150, 50);
            this.button5.TabIndex = 19;
            this.button5.Text = "CSV出力";
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button6.Location = new System.Drawing.Point(1720, 15);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(150, 50);
            this.button6.TabIndex = 22;
            this.button6.Text = "トレース検索";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Location = new System.Drawing.Point(10, 194);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(520, 28);
            this.panel4.TabIndex = 24;
            // 
            // dataGridView4
            // 
            this.dataGridView4.Location = new System.Drawing.Point(10, 222);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.ReadOnly = true;
            this.dataGridView4.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView4.Size = new System.Drawing.Size(520, 626);
            this.dataGridView4.TabIndex = 25;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Location = new System.Drawing.Point(540, 194);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(800, 28);
            this.panel5.TabIndex = 26;
            // 
            // dataGridView5
            // 
            this.dataGridView5.Location = new System.Drawing.Point(540, 222);
            this.dataGridView5.Name = "dataGridView5";
            this.dataGridView5.ReadOnly = true;
            this.dataGridView5.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView5.Size = new System.Drawing.Size(800, 626);
            this.dataGridView5.TabIndex = 27;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Location = new System.Drawing.Point(1350, 194);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(520, 28);
            this.panel6.TabIndex = 28;
            // 
            // dataGridView6
            // 
            this.dataGridView6.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView6.Name = "dataGridView6";
            this.dataGridView6.ReadOnly = true;
            this.dataGridView6.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView6.Size = new System.Drawing.Size(520, 626);
            this.dataGridView6.TabIndex = 29;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.radioButton5);
            this.tabPage4.Controls.Add(this.radioButton6);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.textBox9);
            this.tabPage4.Controls.Add(this.textBox10);
            this.tabPage4.Controls.Add(this.textBox11);
            this.tabPage4.Controls.Add(this.textBox12);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.label17);
            this.tabPage4.Controls.Add(this.label18);
            this.tabPage4.Controls.Add(this.label19);
            this.tabPage4.Controls.Add(this.checkBox4);
            this.tabPage4.Controls.Add(this.dateTimePicker5);
            this.tabPage4.Controls.Add(this.label20);
            this.tabPage4.Controls.Add(this.dateTimePicker6);
            this.tabPage4.Controls.Add(this.button7);
            this.tabPage4.Controls.Add(this.button8);
            this.tabPage4.Controls.Add(this.button9);
            this.tabPage4.Controls.Add(this.panel7);
            this.tabPage4.Controls.Add(this.dataGridView7);
            this.tabPage4.Controls.Add(this.panel8);
            this.tabPage4.Controls.Add(this.dataGridView8);
            this.tabPage4.Controls.Add(this.panel9);
            this.tabPage4.Controls.Add(this.dataGridView9);
            this.tabPage4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1896, 902);
            this.tabPage4.TabIndex = 6;
            this.tabPage4.Text = "(04)_未設定";
            this.tabPage4.UseVisualStyleBackColor = true;
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
            // radioButton6
            // 
            this.radioButton6.Checked = true;
            this.radioButton6.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton6.Location = new System.Drawing.Point(540, 20);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(220, 30);
            this.radioButton6.TabIndex = 0;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "トレースフォワード(追跡)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label15.Location = new System.Drawing.Point(20, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(79, 16);
            this.label15.TabIndex = 1;
            this.label15.Text = "製造指図番号";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(110, 15);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(250, 27);
            this.textBox9.TabIndex = 2;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(110, 50);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(250, 27);
            this.textBox10.TabIndex = 4;
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(110, 85);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(250, 27);
            this.textBox11.TabIndex = 6;
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(110, 120);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(250, 27);
            this.textBox12.TabIndex = 8;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label16.Location = new System.Drawing.Point(20, 55);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 16);
            this.label16.TabIndex = 3;
            this.label16.Text = "品目名";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label17.Location = new System.Drawing.Point(20, 90);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(67, 16);
            this.label17.TabIndex = 5;
            this.label17.Text = "品目コード";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label18.Location = new System.Drawing.Point(20, 125);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(67, 16);
            this.label18.TabIndex = 7;
            this.label18.Text = "ロット番号";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label19.Location = new System.Drawing.Point(20, 160);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(55, 16);
            this.label19.TabIndex = 9;
            this.label19.Text = "対象期間";
            // 
            // checkBox4
            // 
            this.checkBox4.Location = new System.Drawing.Point(367, 162);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 10;
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
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label20.Location = new System.Drawing.Point(226, 162);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(19, 16);
            this.label20.TabIndex = 12;
            this.label20.Text = "～";
            // 
            // dateTimePicker6
            // 
            this.dateTimePicker6.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker6.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker6.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker6.Name = "dateTimePicker6";
            this.dateTimePicker6.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker6.TabIndex = 14;
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button7.Location = new System.Drawing.Point(1377, 15);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(150, 50);
            this.button7.TabIndex = 18;
            this.button7.Text = "クリア";
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button8.Location = new System.Drawing.Point(1548, 15);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(150, 50);
            this.button8.TabIndex = 19;
            this.button8.Text = "CSV出力";
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button9.Location = new System.Drawing.Point(1720, 15);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(150, 50);
            this.button9.TabIndex = 22;
            this.button9.Text = "トレース検索";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Location = new System.Drawing.Point(10, 194);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(520, 28);
            this.panel7.TabIndex = 24;
            // 
            // dataGridView7
            // 
            this.dataGridView7.Location = new System.Drawing.Point(10, 222);
            this.dataGridView7.Name = "dataGridView7";
            this.dataGridView7.ReadOnly = true;
            this.dataGridView7.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView7.Size = new System.Drawing.Size(520, 626);
            this.dataGridView7.TabIndex = 25;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Location = new System.Drawing.Point(540, 194);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(800, 28);
            this.panel8.TabIndex = 26;
            // 
            // dataGridView8
            // 
            this.dataGridView8.Location = new System.Drawing.Point(540, 222);
            this.dataGridView8.Name = "dataGridView8";
            this.dataGridView8.ReadOnly = true;
            this.dataGridView8.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView8.Size = new System.Drawing.Size(800, 626);
            this.dataGridView8.TabIndex = 27;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Location = new System.Drawing.Point(1350, 194);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(520, 28);
            this.panel9.TabIndex = 28;
            // 
            // dataGridView9
            // 
            this.dataGridView9.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView9.Name = "dataGridView9";
            this.dataGridView9.ReadOnly = true;
            this.dataGridView9.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView9.Size = new System.Drawing.Size(520, 626);
            this.dataGridView9.TabIndex = 29;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.radioButton7);
            this.tabPage5.Controls.Add(this.radioButton8);
            this.tabPage5.Controls.Add(this.label21);
            this.tabPage5.Controls.Add(this.textBox13);
            this.tabPage5.Controls.Add(this.textBox14);
            this.tabPage5.Controls.Add(this.textBox15);
            this.tabPage5.Controls.Add(this.textBox16);
            this.tabPage5.Controls.Add(this.label22);
            this.tabPage5.Controls.Add(this.label23);
            this.tabPage5.Controls.Add(this.label24);
            this.tabPage5.Controls.Add(this.label25);
            this.tabPage5.Controls.Add(this.checkBox5);
            this.tabPage5.Controls.Add(this.dateTimePicker7);
            this.tabPage5.Controls.Add(this.label26);
            this.tabPage5.Controls.Add(this.dateTimePicker8);
            this.tabPage5.Controls.Add(this.button10);
            this.tabPage5.Controls.Add(this.button11);
            this.tabPage5.Controls.Add(this.button12);
            this.tabPage5.Controls.Add(this.panel10);
            this.tabPage5.Controls.Add(this.dataGridView10);
            this.tabPage5.Controls.Add(this.panel11);
            this.tabPage5.Controls.Add(this.dataGridView11);
            this.tabPage5.Controls.Add(this.panel12);
            this.tabPage5.Controls.Add(this.dataGridView12);
            this.tabPage5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage5.Location = new System.Drawing.Point(4, 34);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1896, 902);
            this.tabPage5.TabIndex = 7;
            this.tabPage5.Text = "(05)_未設定";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // radioButton7
            // 
            this.radioButton7.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton7.Location = new System.Drawing.Point(540, 55);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(220, 30);
            this.radioButton7.TabIndex = 1;
            this.radioButton7.Text = "トレースバック(遡及)";
            // 
            // radioButton8
            // 
            this.radioButton8.Checked = true;
            this.radioButton8.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton8.Location = new System.Drawing.Point(540, 20);
            this.radioButton8.Name = "radioButton8";
            this.radioButton8.Size = new System.Drawing.Size(220, 30);
            this.radioButton8.TabIndex = 0;
            this.radioButton8.TabStop = true;
            this.radioButton8.Text = "トレースフォワード(追跡)";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label21.Location = new System.Drawing.Point(20, 20);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 16);
            this.label21.TabIndex = 1;
            this.label21.Text = "製造指図番号";
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(110, 15);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(250, 27);
            this.textBox13.TabIndex = 2;
            // 
            // textBox14
            // 
            this.textBox14.Location = new System.Drawing.Point(110, 50);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(250, 27);
            this.textBox14.TabIndex = 4;
            // 
            // textBox15
            // 
            this.textBox15.Location = new System.Drawing.Point(110, 85);
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new System.Drawing.Size(250, 27);
            this.textBox15.TabIndex = 6;
            // 
            // textBox16
            // 
            this.textBox16.Location = new System.Drawing.Point(110, 120);
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new System.Drawing.Size(250, 27);
            this.textBox16.TabIndex = 8;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label22.Location = new System.Drawing.Point(20, 55);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(43, 16);
            this.label22.TabIndex = 3;
            this.label22.Text = "品目名";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label23.Location = new System.Drawing.Point(20, 90);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(67, 16);
            this.label23.TabIndex = 5;
            this.label23.Text = "品目コード";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label24.Location = new System.Drawing.Point(20, 125);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(67, 16);
            this.label24.TabIndex = 7;
            this.label24.Text = "ロット番号";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label25.Location = new System.Drawing.Point(20, 160);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(55, 16);
            this.label25.TabIndex = 9;
            this.label25.Text = "対象期間";
            // 
            // checkBox5
            // 
            this.checkBox5.Location = new System.Drawing.Point(367, 162);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(15, 14);
            this.checkBox5.TabIndex = 10;
            // 
            // dateTimePicker7
            // 
            this.dateTimePicker7.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker7.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker7.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker7.Name = "dateTimePicker7";
            this.dateTimePicker7.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker7.TabIndex = 11;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label26.Location = new System.Drawing.Point(226, 162);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(19, 16);
            this.label26.TabIndex = 12;
            this.label26.Text = "～";
            // 
            // dateTimePicker8
            // 
            this.dateTimePicker8.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker8.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker8.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker8.Name = "dateTimePicker8";
            this.dateTimePicker8.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker8.TabIndex = 14;
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button10.Location = new System.Drawing.Point(1377, 15);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(150, 50);
            this.button10.TabIndex = 18;
            this.button10.Text = "クリア";
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button11.Location = new System.Drawing.Point(1548, 15);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(150, 50);
            this.button11.TabIndex = 19;
            this.button11.Text = "CSV出力";
            // 
            // button12
            // 
            this.button12.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button12.Location = new System.Drawing.Point(1720, 15);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(150, 50);
            this.button12.TabIndex = 22;
            this.button12.Text = "トレース検索";
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Location = new System.Drawing.Point(10, 194);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(520, 28);
            this.panel10.TabIndex = 24;
            // 
            // dataGridView10
            // 
            this.dataGridView10.Location = new System.Drawing.Point(10, 222);
            this.dataGridView10.Name = "dataGridView10";
            this.dataGridView10.ReadOnly = true;
            this.dataGridView10.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView10.Size = new System.Drawing.Size(520, 626);
            this.dataGridView10.TabIndex = 25;
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Location = new System.Drawing.Point(540, 194);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(800, 28);
            this.panel11.TabIndex = 26;
            // 
            // dataGridView11
            // 
            this.dataGridView11.Location = new System.Drawing.Point(540, 222);
            this.dataGridView11.Name = "dataGridView11";
            this.dataGridView11.ReadOnly = true;
            this.dataGridView11.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView11.Size = new System.Drawing.Size(800, 626);
            this.dataGridView11.TabIndex = 27;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Location = new System.Drawing.Point(1350, 194);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(520, 28);
            this.panel12.TabIndex = 28;
            // 
            // dataGridView12
            // 
            this.dataGridView12.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView12.Name = "dataGridView12";
            this.dataGridView12.ReadOnly = true;
            this.dataGridView12.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView12.Size = new System.Drawing.Size(520, 626);
            this.dataGridView12.TabIndex = 29;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.radioButton9);
            this.tabPage6.Controls.Add(this.radioButton10);
            this.tabPage6.Controls.Add(this.label27);
            this.tabPage6.Controls.Add(this.textBox17);
            this.tabPage6.Controls.Add(this.textBox18);
            this.tabPage6.Controls.Add(this.textBox19);
            this.tabPage6.Controls.Add(this.textBox20);
            this.tabPage6.Controls.Add(this.label28);
            this.tabPage6.Controls.Add(this.label29);
            this.tabPage6.Controls.Add(this.label30);
            this.tabPage6.Controls.Add(this.label31);
            this.tabPage6.Controls.Add(this.checkBox6);
            this.tabPage6.Controls.Add(this.dateTimePicker9);
            this.tabPage6.Controls.Add(this.label32);
            this.tabPage6.Controls.Add(this.dateTimePicker10);
            this.tabPage6.Controls.Add(this.button13);
            this.tabPage6.Controls.Add(this.button14);
            this.tabPage6.Controls.Add(this.button15);
            this.tabPage6.Controls.Add(this.panel13);
            this.tabPage6.Controls.Add(this.dataGridView13);
            this.tabPage6.Controls.Add(this.panel14);
            this.tabPage6.Controls.Add(this.dataGridView14);
            this.tabPage6.Controls.Add(this.panel15);
            this.tabPage6.Controls.Add(this.dataGridView15);
            this.tabPage6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage6.Location = new System.Drawing.Point(4, 34);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1896, 902);
            this.tabPage6.TabIndex = 8;
            this.tabPage6.Text = "(06)_未設定";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // radioButton9
            // 
            this.radioButton9.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton9.Location = new System.Drawing.Point(540, 55);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(220, 30);
            this.radioButton9.TabIndex = 1;
            this.radioButton9.Text = "トレースバック(遡及)";
            // 
            // radioButton10
            // 
            this.radioButton10.Checked = true;
            this.radioButton10.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton10.Location = new System.Drawing.Point(540, 20);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(220, 30);
            this.radioButton10.TabIndex = 0;
            this.radioButton10.TabStop = true;
            this.radioButton10.Text = "トレースフォワード(追跡)";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label27.Location = new System.Drawing.Point(20, 20);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(79, 16);
            this.label27.TabIndex = 1;
            this.label27.Text = "製造指図番号";
            // 
            // textBox17
            // 
            this.textBox17.Location = new System.Drawing.Point(110, 15);
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new System.Drawing.Size(250, 27);
            this.textBox17.TabIndex = 2;
            // 
            // textBox18
            // 
            this.textBox18.Location = new System.Drawing.Point(110, 50);
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(250, 27);
            this.textBox18.TabIndex = 4;
            // 
            // textBox19
            // 
            this.textBox19.Location = new System.Drawing.Point(110, 85);
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(250, 27);
            this.textBox19.TabIndex = 6;
            // 
            // textBox20
            // 
            this.textBox20.Location = new System.Drawing.Point(110, 120);
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new System.Drawing.Size(250, 27);
            this.textBox20.TabIndex = 8;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label28.Location = new System.Drawing.Point(20, 55);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(43, 16);
            this.label28.TabIndex = 3;
            this.label28.Text = "品目名";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label29.Location = new System.Drawing.Point(20, 90);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(67, 16);
            this.label29.TabIndex = 5;
            this.label29.Text = "品目コード";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label30.Location = new System.Drawing.Point(20, 125);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(67, 16);
            this.label30.TabIndex = 7;
            this.label30.Text = "ロット番号";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label31.Location = new System.Drawing.Point(20, 160);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(55, 16);
            this.label31.TabIndex = 9;
            this.label31.Text = "対象期間";
            // 
            // checkBox6
            // 
            this.checkBox6.Location = new System.Drawing.Point(367, 162);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(15, 14);
            this.checkBox6.TabIndex = 10;
            // 
            // dateTimePicker9
            // 
            this.dateTimePicker9.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker9.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker9.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker9.Name = "dateTimePicker9";
            this.dateTimePicker9.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker9.TabIndex = 11;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label32.Location = new System.Drawing.Point(226, 162);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(19, 16);
            this.label32.TabIndex = 12;
            this.label32.Text = "～";
            // 
            // dateTimePicker10
            // 
            this.dateTimePicker10.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker10.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker10.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker10.Name = "dateTimePicker10";
            this.dateTimePicker10.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker10.TabIndex = 14;
            // 
            // button13
            // 
            this.button13.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button13.Location = new System.Drawing.Point(1377, 15);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(150, 50);
            this.button13.TabIndex = 18;
            this.button13.Text = "クリア";
            // 
            // button14
            // 
            this.button14.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button14.Location = new System.Drawing.Point(1548, 15);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(150, 50);
            this.button14.TabIndex = 19;
            this.button14.Text = "CSV出力";
            // 
            // button15
            // 
            this.button15.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button15.Location = new System.Drawing.Point(1720, 15);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(150, 50);
            this.button15.TabIndex = 22;
            this.button15.Text = "トレース検索";
            // 
            // panel13
            // 
            this.panel13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel13.Location = new System.Drawing.Point(10, 194);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(520, 28);
            this.panel13.TabIndex = 24;
            // 
            // dataGridView13
            // 
            this.dataGridView13.Location = new System.Drawing.Point(10, 222);
            this.dataGridView13.Name = "dataGridView13";
            this.dataGridView13.ReadOnly = true;
            this.dataGridView13.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView13.Size = new System.Drawing.Size(520, 626);
            this.dataGridView13.TabIndex = 25;
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel14.Location = new System.Drawing.Point(540, 194);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(800, 28);
            this.panel14.TabIndex = 26;
            // 
            // dataGridView14
            // 
            this.dataGridView14.Location = new System.Drawing.Point(540, 222);
            this.dataGridView14.Name = "dataGridView14";
            this.dataGridView14.ReadOnly = true;
            this.dataGridView14.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView14.Size = new System.Drawing.Size(800, 626);
            this.dataGridView14.TabIndex = 27;
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel15.Location = new System.Drawing.Point(1350, 194);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(520, 28);
            this.panel15.TabIndex = 28;
            // 
            // dataGridView15
            // 
            this.dataGridView15.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView15.Name = "dataGridView15";
            this.dataGridView15.ReadOnly = true;
            this.dataGridView15.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView15.Size = new System.Drawing.Size(520, 626);
            this.dataGridView15.TabIndex = 29;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.radioButton11);
            this.tabPage7.Controls.Add(this.radioButton12);
            this.tabPage7.Controls.Add(this.label33);
            this.tabPage7.Controls.Add(this.textBox21);
            this.tabPage7.Controls.Add(this.textBox22);
            this.tabPage7.Controls.Add(this.textBox23);
            this.tabPage7.Controls.Add(this.textBox24);
            this.tabPage7.Controls.Add(this.label34);
            this.tabPage7.Controls.Add(this.label35);
            this.tabPage7.Controls.Add(this.label36);
            this.tabPage7.Controls.Add(this.label37);
            this.tabPage7.Controls.Add(this.checkBox7);
            this.tabPage7.Controls.Add(this.dateTimePicker11);
            this.tabPage7.Controls.Add(this.label38);
            this.tabPage7.Controls.Add(this.dateTimePicker12);
            this.tabPage7.Controls.Add(this.button16);
            this.tabPage7.Controls.Add(this.button17);
            this.tabPage7.Controls.Add(this.button18);
            this.tabPage7.Controls.Add(this.panel16);
            this.tabPage7.Controls.Add(this.dataGridView16);
            this.tabPage7.Controls.Add(this.panel17);
            this.tabPage7.Controls.Add(this.dataGridView17);
            this.tabPage7.Controls.Add(this.panel18);
            this.tabPage7.Controls.Add(this.dataGridView18);
            this.tabPage7.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage7.Location = new System.Drawing.Point(4, 34);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1896, 902);
            this.tabPage7.TabIndex = 9;
            this.tabPage7.Text = "(07)_未設定";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // radioButton11
            // 
            this.radioButton11.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton11.Location = new System.Drawing.Point(540, 55);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(220, 30);
            this.radioButton11.TabIndex = 1;
            this.radioButton11.Text = "トレースバック(遡及)";
            // 
            // radioButton12
            // 
            this.radioButton12.Checked = true;
            this.radioButton12.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton12.Location = new System.Drawing.Point(540, 20);
            this.radioButton12.Name = "radioButton12";
            this.radioButton12.Size = new System.Drawing.Size(220, 30);
            this.radioButton12.TabIndex = 0;
            this.radioButton12.TabStop = true;
            this.radioButton12.Text = "トレースフォワード(追跡)";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label33.Location = new System.Drawing.Point(20, 20);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(79, 16);
            this.label33.TabIndex = 1;
            this.label33.Text = "製造指図番号";
            // 
            // textBox21
            // 
            this.textBox21.Location = new System.Drawing.Point(110, 15);
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new System.Drawing.Size(250, 27);
            this.textBox21.TabIndex = 2;
            // 
            // textBox22
            // 
            this.textBox22.Location = new System.Drawing.Point(110, 50);
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new System.Drawing.Size(250, 27);
            this.textBox22.TabIndex = 4;
            // 
            // textBox23
            // 
            this.textBox23.Location = new System.Drawing.Point(110, 85);
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new System.Drawing.Size(250, 27);
            this.textBox23.TabIndex = 6;
            // 
            // textBox24
            // 
            this.textBox24.Location = new System.Drawing.Point(110, 120);
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new System.Drawing.Size(250, 27);
            this.textBox24.TabIndex = 8;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label34.Location = new System.Drawing.Point(20, 55);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(43, 16);
            this.label34.TabIndex = 3;
            this.label34.Text = "品目名";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label35.Location = new System.Drawing.Point(20, 90);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(67, 16);
            this.label35.TabIndex = 5;
            this.label35.Text = "品目コード";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label36.Location = new System.Drawing.Point(20, 125);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(67, 16);
            this.label36.TabIndex = 7;
            this.label36.Text = "ロット番号";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label37.Location = new System.Drawing.Point(20, 160);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(55, 16);
            this.label37.TabIndex = 9;
            this.label37.Text = "対象期間";
            // 
            // checkBox7
            // 
            this.checkBox7.Location = new System.Drawing.Point(367, 162);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(15, 14);
            this.checkBox7.TabIndex = 10;
            // 
            // dateTimePicker11
            // 
            this.dateTimePicker11.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker11.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker11.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker11.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker11.Name = "dateTimePicker11";
            this.dateTimePicker11.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker11.TabIndex = 11;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label38.Location = new System.Drawing.Point(226, 162);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(19, 16);
            this.label38.TabIndex = 12;
            this.label38.Text = "～";
            // 
            // dateTimePicker12
            // 
            this.dateTimePicker12.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker12.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker12.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker12.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker12.Name = "dateTimePicker12";
            this.dateTimePicker12.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker12.TabIndex = 14;
            // 
            // button16
            // 
            this.button16.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button16.Location = new System.Drawing.Point(1377, 15);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(150, 50);
            this.button16.TabIndex = 18;
            this.button16.Text = "クリア";
            // 
            // button17
            // 
            this.button17.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button17.Location = new System.Drawing.Point(1548, 15);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(150, 50);
            this.button17.TabIndex = 19;
            this.button17.Text = "CSV出力";
            // 
            // button18
            // 
            this.button18.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button18.Location = new System.Drawing.Point(1720, 15);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(150, 50);
            this.button18.TabIndex = 22;
            this.button18.Text = "トレース検索";
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel16.Location = new System.Drawing.Point(10, 194);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(520, 28);
            this.panel16.TabIndex = 24;
            // 
            // dataGridView16
            // 
            this.dataGridView16.Location = new System.Drawing.Point(10, 222);
            this.dataGridView16.Name = "dataGridView16";
            this.dataGridView16.ReadOnly = true;
            this.dataGridView16.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView16.Size = new System.Drawing.Size(520, 626);
            this.dataGridView16.TabIndex = 25;
            // 
            // panel17
            // 
            this.panel17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel17.Location = new System.Drawing.Point(540, 194);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(800, 28);
            this.panel17.TabIndex = 26;
            // 
            // dataGridView17
            // 
            this.dataGridView17.Location = new System.Drawing.Point(540, 222);
            this.dataGridView17.Name = "dataGridView17";
            this.dataGridView17.ReadOnly = true;
            this.dataGridView17.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView17.Size = new System.Drawing.Size(800, 626);
            this.dataGridView17.TabIndex = 27;
            // 
            // panel18
            // 
            this.panel18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel18.Location = new System.Drawing.Point(1350, 194);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(520, 28);
            this.panel18.TabIndex = 28;
            // 
            // dataGridView18
            // 
            this.dataGridView18.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView18.Name = "dataGridView18";
            this.dataGridView18.ReadOnly = true;
            this.dataGridView18.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView18.Size = new System.Drawing.Size(520, 626);
            this.dataGridView18.TabIndex = 29;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.radioButton13);
            this.tabPage8.Controls.Add(this.radioButton14);
            this.tabPage8.Controls.Add(this.label39);
            this.tabPage8.Controls.Add(this.textBox25);
            this.tabPage8.Controls.Add(this.textBox26);
            this.tabPage8.Controls.Add(this.textBox27);
            this.tabPage8.Controls.Add(this.textBox28);
            this.tabPage8.Controls.Add(this.label40);
            this.tabPage8.Controls.Add(this.label41);
            this.tabPage8.Controls.Add(this.label42);
            this.tabPage8.Controls.Add(this.label43);
            this.tabPage8.Controls.Add(this.checkBox8);
            this.tabPage8.Controls.Add(this.dateTimePicker13);
            this.tabPage8.Controls.Add(this.label44);
            this.tabPage8.Controls.Add(this.dateTimePicker14);
            this.tabPage8.Controls.Add(this.button19);
            this.tabPage8.Controls.Add(this.button20);
            this.tabPage8.Controls.Add(this.button21);
            this.tabPage8.Controls.Add(this.panel19);
            this.tabPage8.Controls.Add(this.dataGridView19);
            this.tabPage8.Controls.Add(this.panel20);
            this.tabPage8.Controls.Add(this.dataGridView20);
            this.tabPage8.Controls.Add(this.panel21);
            this.tabPage8.Controls.Add(this.dataGridView21);
            this.tabPage8.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage8.Location = new System.Drawing.Point(4, 34);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1896, 902);
            this.tabPage8.TabIndex = 10;
            this.tabPage8.Text = "(08)_未設定";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // radioButton13
            // 
            this.radioButton13.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton13.Location = new System.Drawing.Point(540, 55);
            this.radioButton13.Name = "radioButton13";
            this.radioButton13.Size = new System.Drawing.Size(220, 30);
            this.radioButton13.TabIndex = 1;
            this.radioButton13.Text = "トレースバック(遡及)";
            // 
            // radioButton14
            // 
            this.radioButton14.Checked = true;
            this.radioButton14.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton14.Location = new System.Drawing.Point(540, 20);
            this.radioButton14.Name = "radioButton14";
            this.radioButton14.Size = new System.Drawing.Size(220, 30);
            this.radioButton14.TabIndex = 0;
            this.radioButton14.TabStop = true;
            this.radioButton14.Text = "トレースフォワード(追跡)";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label39.Location = new System.Drawing.Point(20, 20);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(79, 16);
            this.label39.TabIndex = 1;
            this.label39.Text = "製造指図番号";
            // 
            // textBox25
            // 
            this.textBox25.Location = new System.Drawing.Point(110, 15);
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new System.Drawing.Size(250, 27);
            this.textBox25.TabIndex = 2;
            // 
            // textBox26
            // 
            this.textBox26.Location = new System.Drawing.Point(110, 50);
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new System.Drawing.Size(250, 27);
            this.textBox26.TabIndex = 4;
            // 
            // textBox27
            // 
            this.textBox27.Location = new System.Drawing.Point(110, 85);
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new System.Drawing.Size(250, 27);
            this.textBox27.TabIndex = 6;
            // 
            // textBox28
            // 
            this.textBox28.Location = new System.Drawing.Point(110, 120);
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new System.Drawing.Size(250, 27);
            this.textBox28.TabIndex = 8;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label40.Location = new System.Drawing.Point(20, 55);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(43, 16);
            this.label40.TabIndex = 3;
            this.label40.Text = "品目名";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label41.Location = new System.Drawing.Point(20, 90);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(67, 16);
            this.label41.TabIndex = 5;
            this.label41.Text = "品目コード";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label42.Location = new System.Drawing.Point(20, 125);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(67, 16);
            this.label42.TabIndex = 7;
            this.label42.Text = "ロット番号";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label43.Location = new System.Drawing.Point(20, 160);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(55, 16);
            this.label43.TabIndex = 9;
            this.label43.Text = "対象期間";
            // 
            // checkBox8
            // 
            this.checkBox8.Location = new System.Drawing.Point(367, 162);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(15, 14);
            this.checkBox8.TabIndex = 10;
            // 
            // dateTimePicker13
            // 
            this.dateTimePicker13.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker13.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker13.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker13.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker13.Name = "dateTimePicker13";
            this.dateTimePicker13.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker13.TabIndex = 11;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label44.Location = new System.Drawing.Point(226, 162);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(19, 16);
            this.label44.TabIndex = 12;
            this.label44.Text = "～";
            // 
            // dateTimePicker14
            // 
            this.dateTimePicker14.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker14.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker14.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker14.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker14.Name = "dateTimePicker14";
            this.dateTimePicker14.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker14.TabIndex = 14;
            // 
            // button19
            // 
            this.button19.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button19.Location = new System.Drawing.Point(1377, 15);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(150, 50);
            this.button19.TabIndex = 18;
            this.button19.Text = "クリア";
            // 
            // button20
            // 
            this.button20.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button20.Location = new System.Drawing.Point(1548, 15);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(150, 50);
            this.button20.TabIndex = 19;
            this.button20.Text = "CSV出力";
            // 
            // button21
            // 
            this.button21.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button21.Location = new System.Drawing.Point(1720, 15);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(150, 50);
            this.button21.TabIndex = 22;
            this.button21.Text = "トレース検索";
            // 
            // panel19
            // 
            this.panel19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel19.Location = new System.Drawing.Point(10, 194);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(520, 28);
            this.panel19.TabIndex = 24;
            // 
            // dataGridView19
            // 
            this.dataGridView19.Location = new System.Drawing.Point(10, 222);
            this.dataGridView19.Name = "dataGridView19";
            this.dataGridView19.ReadOnly = true;
            this.dataGridView19.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView19.Size = new System.Drawing.Size(520, 626);
            this.dataGridView19.TabIndex = 25;
            // 
            // panel20
            // 
            this.panel20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel20.Location = new System.Drawing.Point(540, 194);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(800, 28);
            this.panel20.TabIndex = 26;
            // 
            // dataGridView20
            // 
            this.dataGridView20.Location = new System.Drawing.Point(540, 222);
            this.dataGridView20.Name = "dataGridView20";
            this.dataGridView20.ReadOnly = true;
            this.dataGridView20.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView20.Size = new System.Drawing.Size(800, 626);
            this.dataGridView20.TabIndex = 27;
            // 
            // panel21
            // 
            this.panel21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel21.Location = new System.Drawing.Point(1350, 194);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(520, 28);
            this.panel21.TabIndex = 28;
            // 
            // dataGridView21
            // 
            this.dataGridView21.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView21.Name = "dataGridView21";
            this.dataGridView21.ReadOnly = true;
            this.dataGridView21.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView21.Size = new System.Drawing.Size(520, 626);
            this.dataGridView21.TabIndex = 29;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.radioButton15);
            this.tabPage9.Controls.Add(this.radioButton16);
            this.tabPage9.Controls.Add(this.label45);
            this.tabPage9.Controls.Add(this.textBox29);
            this.tabPage9.Controls.Add(this.textBox30);
            this.tabPage9.Controls.Add(this.textBox31);
            this.tabPage9.Controls.Add(this.textBox32);
            this.tabPage9.Controls.Add(this.label46);
            this.tabPage9.Controls.Add(this.label47);
            this.tabPage9.Controls.Add(this.label48);
            this.tabPage9.Controls.Add(this.label49);
            this.tabPage9.Controls.Add(this.checkBox9);
            this.tabPage9.Controls.Add(this.dateTimePicker15);
            this.tabPage9.Controls.Add(this.label50);
            this.tabPage9.Controls.Add(this.dateTimePicker16);
            this.tabPage9.Controls.Add(this.button22);
            this.tabPage9.Controls.Add(this.button23);
            this.tabPage9.Controls.Add(this.button24);
            this.tabPage9.Controls.Add(this.panel22);
            this.tabPage9.Controls.Add(this.dataGridView22);
            this.tabPage9.Controls.Add(this.panel23);
            this.tabPage9.Controls.Add(this.dataGridView23);
            this.tabPage9.Controls.Add(this.panel24);
            this.tabPage9.Controls.Add(this.dataGridView24);
            this.tabPage9.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage9.Location = new System.Drawing.Point(4, 34);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(1896, 902);
            this.tabPage9.TabIndex = 11;
            this.tabPage9.Text = "(09)_未設定";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // radioButton15
            // 
            this.radioButton15.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton15.Location = new System.Drawing.Point(540, 55);
            this.radioButton15.Name = "radioButton15";
            this.radioButton15.Size = new System.Drawing.Size(220, 30);
            this.radioButton15.TabIndex = 1;
            this.radioButton15.Text = "トレースバック(遡及)";
            // 
            // radioButton16
            // 
            this.radioButton16.Checked = true;
            this.radioButton16.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton16.Location = new System.Drawing.Point(540, 20);
            this.radioButton16.Name = "radioButton16";
            this.radioButton16.Size = new System.Drawing.Size(220, 30);
            this.radioButton16.TabIndex = 0;
            this.radioButton16.TabStop = true;
            this.radioButton16.Text = "トレースフォワード(追跡)";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label45.Location = new System.Drawing.Point(20, 20);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(79, 16);
            this.label45.TabIndex = 1;
            this.label45.Text = "製造指図番号";
            // 
            // textBox29
            // 
            this.textBox29.Location = new System.Drawing.Point(110, 15);
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new System.Drawing.Size(250, 27);
            this.textBox29.TabIndex = 2;
            // 
            // textBox30
            // 
            this.textBox30.Location = new System.Drawing.Point(110, 50);
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new System.Drawing.Size(250, 27);
            this.textBox30.TabIndex = 4;
            // 
            // textBox31
            // 
            this.textBox31.Location = new System.Drawing.Point(110, 85);
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new System.Drawing.Size(250, 27);
            this.textBox31.TabIndex = 6;
            // 
            // textBox32
            // 
            this.textBox32.Location = new System.Drawing.Point(110, 120);
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new System.Drawing.Size(250, 27);
            this.textBox32.TabIndex = 8;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label46.Location = new System.Drawing.Point(20, 55);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(43, 16);
            this.label46.TabIndex = 3;
            this.label46.Text = "品目名";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label47.Location = new System.Drawing.Point(20, 90);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(67, 16);
            this.label47.TabIndex = 5;
            this.label47.Text = "品目コード";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label48.Location = new System.Drawing.Point(20, 125);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(67, 16);
            this.label48.TabIndex = 7;
            this.label48.Text = "ロット番号";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label49.Location = new System.Drawing.Point(20, 160);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(55, 16);
            this.label49.TabIndex = 9;
            this.label49.Text = "対象期間";
            // 
            // checkBox9
            // 
            this.checkBox9.Location = new System.Drawing.Point(367, 162);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(15, 14);
            this.checkBox9.TabIndex = 10;
            // 
            // dateTimePicker15
            // 
            this.dateTimePicker15.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker15.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker15.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker15.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker15.Name = "dateTimePicker15";
            this.dateTimePicker15.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker15.TabIndex = 11;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label50.Location = new System.Drawing.Point(226, 162);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(19, 16);
            this.label50.TabIndex = 12;
            this.label50.Text = "～";
            // 
            // dateTimePicker16
            // 
            this.dateTimePicker16.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker16.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker16.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker16.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker16.Name = "dateTimePicker16";
            this.dateTimePicker16.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker16.TabIndex = 14;
            // 
            // button22
            // 
            this.button22.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button22.Location = new System.Drawing.Point(1377, 15);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(150, 50);
            this.button22.TabIndex = 18;
            this.button22.Text = "クリア";
            // 
            // button23
            // 
            this.button23.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button23.Location = new System.Drawing.Point(1548, 15);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(150, 50);
            this.button23.TabIndex = 19;
            this.button23.Text = "CSV出力";
            // 
            // button24
            // 
            this.button24.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button24.Location = new System.Drawing.Point(1720, 15);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(150, 50);
            this.button24.TabIndex = 22;
            this.button24.Text = "トレース検索";
            // 
            // panel22
            // 
            this.panel22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel22.Location = new System.Drawing.Point(10, 194);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(520, 28);
            this.panel22.TabIndex = 24;
            // 
            // dataGridView22
            // 
            this.dataGridView22.Location = new System.Drawing.Point(10, 222);
            this.dataGridView22.Name = "dataGridView22";
            this.dataGridView22.ReadOnly = true;
            this.dataGridView22.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView22.Size = new System.Drawing.Size(520, 626);
            this.dataGridView22.TabIndex = 25;
            // 
            // panel23
            // 
            this.panel23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel23.Location = new System.Drawing.Point(540, 194);
            this.panel23.Name = "panel23";
            this.panel23.Size = new System.Drawing.Size(800, 28);
            this.panel23.TabIndex = 26;
            // 
            // dataGridView23
            // 
            this.dataGridView23.Location = new System.Drawing.Point(540, 222);
            this.dataGridView23.Name = "dataGridView23";
            this.dataGridView23.ReadOnly = true;
            this.dataGridView23.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView23.Size = new System.Drawing.Size(800, 626);
            this.dataGridView23.TabIndex = 27;
            // 
            // panel24
            // 
            this.panel24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel24.Location = new System.Drawing.Point(1350, 194);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(520, 28);
            this.panel24.TabIndex = 28;
            // 
            // dataGridView24
            // 
            this.dataGridView24.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView24.Name = "dataGridView24";
            this.dataGridView24.ReadOnly = true;
            this.dataGridView24.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView24.Size = new System.Drawing.Size(520, 626);
            this.dataGridView24.TabIndex = 29;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.radioButton17);
            this.tabPage10.Controls.Add(this.radioButton18);
            this.tabPage10.Controls.Add(this.label51);
            this.tabPage10.Controls.Add(this.textBox33);
            this.tabPage10.Controls.Add(this.textBox34);
            this.tabPage10.Controls.Add(this.textBox35);
            this.tabPage10.Controls.Add(this.textBox36);
            this.tabPage10.Controls.Add(this.label52);
            this.tabPage10.Controls.Add(this.label53);
            this.tabPage10.Controls.Add(this.label54);
            this.tabPage10.Controls.Add(this.label55);
            this.tabPage10.Controls.Add(this.checkBox10);
            this.tabPage10.Controls.Add(this.dateTimePicker17);
            this.tabPage10.Controls.Add(this.label56);
            this.tabPage10.Controls.Add(this.dateTimePicker18);
            this.tabPage10.Controls.Add(this.button25);
            this.tabPage10.Controls.Add(this.button26);
            this.tabPage10.Controls.Add(this.button27);
            this.tabPage10.Controls.Add(this.panel25);
            this.tabPage10.Controls.Add(this.dataGridView25);
            this.tabPage10.Controls.Add(this.panel26);
            this.tabPage10.Controls.Add(this.dataGridView26);
            this.tabPage10.Controls.Add(this.panel27);
            this.tabPage10.Controls.Add(this.dataGridView27);
            this.tabPage10.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage10.Location = new System.Drawing.Point(4, 34);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.Size = new System.Drawing.Size(1896, 902);
            this.tabPage10.TabIndex = 12;
            this.tabPage10.Text = "(10)_未設定";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // radioButton17
            // 
            this.radioButton17.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.radioButton17.Location = new System.Drawing.Point(540, 55);
            this.radioButton17.Name = "radioButton17";
            this.radioButton17.Size = new System.Drawing.Size(220, 30);
            this.radioButton17.TabIndex = 1;
            this.radioButton17.Text = "トレースバック(遡及)";
            // 
            // radioButton18
            // 
            this.radioButton18.Checked = true;
            this.radioButton18.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton18.Location = new System.Drawing.Point(540, 20);
            this.radioButton18.Name = "radioButton18";
            this.radioButton18.Size = new System.Drawing.Size(220, 30);
            this.radioButton18.TabIndex = 0;
            this.radioButton18.TabStop = true;
            this.radioButton18.Text = "トレースフォワード(追跡)";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label51.Location = new System.Drawing.Point(20, 20);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(79, 16);
            this.label51.TabIndex = 1;
            this.label51.Text = "製造指図番号";
            // 
            // textBox33
            // 
            this.textBox33.Location = new System.Drawing.Point(110, 15);
            this.textBox33.Name = "textBox33";
            this.textBox33.Size = new System.Drawing.Size(250, 27);
            this.textBox33.TabIndex = 2;
            // 
            // textBox34
            // 
            this.textBox34.Location = new System.Drawing.Point(110, 50);
            this.textBox34.Name = "textBox34";
            this.textBox34.Size = new System.Drawing.Size(250, 27);
            this.textBox34.TabIndex = 4;
            // 
            // textBox35
            // 
            this.textBox35.Location = new System.Drawing.Point(110, 85);
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new System.Drawing.Size(250, 27);
            this.textBox35.TabIndex = 6;
            // 
            // textBox36
            // 
            this.textBox36.Location = new System.Drawing.Point(110, 120);
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new System.Drawing.Size(250, 27);
            this.textBox36.TabIndex = 8;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label52.Location = new System.Drawing.Point(20, 55);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(43, 16);
            this.label52.TabIndex = 3;
            this.label52.Text = "品目名";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label53.Location = new System.Drawing.Point(20, 90);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(67, 16);
            this.label53.TabIndex = 5;
            this.label53.Text = "品目コード";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label54.Location = new System.Drawing.Point(20, 125);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(67, 16);
            this.label54.TabIndex = 7;
            this.label54.Text = "ロット番号";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label55.Location = new System.Drawing.Point(20, 160);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(55, 16);
            this.label55.TabIndex = 9;
            this.label55.Text = "対象期間";
            // 
            // checkBox10
            // 
            this.checkBox10.Location = new System.Drawing.Point(367, 162);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(15, 14);
            this.checkBox10.TabIndex = 10;
            // 
            // dateTimePicker17
            // 
            this.dateTimePicker17.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker17.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker17.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker17.Location = new System.Drawing.Point(110, 155);
            this.dateTimePicker17.Name = "dateTimePicker17";
            this.dateTimePicker17.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker17.TabIndex = 11;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label56.Location = new System.Drawing.Point(226, 162);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(19, 16);
            this.label56.TabIndex = 12;
            this.label56.Text = "～";
            // 
            // dateTimePicker18
            // 
            this.dateTimePicker18.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker18.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker18.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker18.Location = new System.Drawing.Point(251, 155);
            this.dateTimePicker18.Name = "dateTimePicker18";
            this.dateTimePicker18.Size = new System.Drawing.Size(110, 27);
            this.dateTimePicker18.TabIndex = 14;
            // 
            // button25
            // 
            this.button25.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button25.Location = new System.Drawing.Point(1377, 15);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(150, 50);
            this.button25.TabIndex = 18;
            this.button25.Text = "クリア";
            // 
            // button26
            // 
            this.button26.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button26.Location = new System.Drawing.Point(1548, 15);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(150, 50);
            this.button26.TabIndex = 19;
            this.button26.Text = "CSV出力";
            // 
            // button27
            // 
            this.button27.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.button27.Location = new System.Drawing.Point(1720, 15);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(150, 50);
            this.button27.TabIndex = 22;
            this.button27.Text = "トレース検索";
            // 
            // panel25
            // 
            this.panel25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(250)))));
            this.panel25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel25.Location = new System.Drawing.Point(10, 194);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(520, 28);
            this.panel25.TabIndex = 24;
            // 
            // dataGridView25
            // 
            this.dataGridView25.Location = new System.Drawing.Point(10, 222);
            this.dataGridView25.Name = "dataGridView25";
            this.dataGridView25.ReadOnly = true;
            this.dataGridView25.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView25.Size = new System.Drawing.Size(520, 626);
            this.dataGridView25.TabIndex = 25;
            // 
            // panel26
            // 
            this.panel26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(236)))));
            this.panel26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel26.Location = new System.Drawing.Point(540, 194);
            this.panel26.Name = "panel26";
            this.panel26.Size = new System.Drawing.Size(800, 28);
            this.panel26.TabIndex = 26;
            // 
            // dataGridView26
            // 
            this.dataGridView26.Location = new System.Drawing.Point(540, 222);
            this.dataGridView26.Name = "dataGridView26";
            this.dataGridView26.ReadOnly = true;
            this.dataGridView26.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView26.Size = new System.Drawing.Size(800, 626);
            this.dataGridView26.TabIndex = 27;
            // 
            // panel27
            // 
            this.panel27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.panel27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel27.Location = new System.Drawing.Point(1350, 194);
            this.panel27.Name = "panel27";
            this.panel27.Size = new System.Drawing.Size(520, 28);
            this.panel27.TabIndex = 28;
            // 
            // dataGridView27
            // 
            this.dataGridView27.Location = new System.Drawing.Point(1350, 222);
            this.dataGridView27.Name = "dataGridView27";
            this.dataGridView27.ReadOnly = true;
            this.dataGridView27.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView27.Size = new System.Drawing.Size(520, 626);
            this.dataGridView27.TabIndex = 29;
            // 
            // IntersectionTab
            // 
            this.IntersectionTab.Controls.Add(this.dataGridIntersection);
            this.IntersectionTab.Location = new System.Drawing.Point(4, 34);
            this.IntersectionTab.Name = "IntersectionTab";
            this.IntersectionTab.Padding = new System.Windows.Forms.Padding(3);
            this.IntersectionTab.Size = new System.Drawing.Size(1896, 902);
            this.IntersectionTab.TabIndex = 3;
            this.IntersectionTab.Text = "交点検出結果";
            this.IntersectionTab.UseVisualStyleBackColor = true;
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
            // btnBottleScreen
            // 
            this.btnBottleScreen.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.btnBottleScreen.Location = new System.Drawing.Point(10, 15);
            this.btnBottleScreen.Name = "btnBottleScreen";
            this.btnBottleScreen.Size = new System.Drawing.Size(150, 40);
            this.btnBottleScreen.TabIndex = 23;
            this.btnBottleScreen.Text = "瓶設備";
            this.btnBottleScreen.Click += new System.EventHandler(this.btnBottleTrace_Click);
            // 
            // btnDetectCrossPoints
            // 
            this.btnDetectCrossPoints.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDetectCrossPoints.Location = new System.Drawing.Point(1732, 15);
            this.btnDetectCrossPoints.Name = "btnDetectCrossPoints";
            this.btnDetectCrossPoints.Size = new System.Drawing.Size(150, 50);
            this.btnDetectCrossPoints.TabIndex = 21;
            this.btnDetectCrossPoints.Text = "交点検出";
            this.btnDetectCrossPoints.Click += new System.EventHandler(this.btnDetectCrossPoints_Click);
            // 
            // btnExcelOutput
            // 
            this.btnExcelOutput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExcelOutput.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnExcelOutput.Location = new System.Drawing.Point(1560, 15);
            this.btnExcelOutput.Name = "btnExcelOutput";
            this.btnExcelOutput.Size = new System.Drawing.Size(150, 50);
            this.btnExcelOutput.TabIndex = 20;
            this.btnExcelOutput.Text = "EXCEL出力";
            this.btnExcelOutput.Click += new System.EventHandler(this.btnExcelOutput_Click);
            // 
            // rdoTabNameItemCode
            // 
            this.rdoTabNameItemCode.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.rdoTabNameItemCode.Location = new System.Drawing.Point(1389, 31);
            this.rdoTabNameItemCode.Name = "rdoTabNameItemCode";
            this.rdoTabNameItemCode.Size = new System.Drawing.Size(110, 24);
            this.rdoTabNameItemCode.TabIndex = 1;
            this.rdoTabNameItemCode.Text = "品目コード";
            // 
            // rdoTabNameOrder
            // 
            this.rdoTabNameOrder.Checked = true;
            this.rdoTabNameOrder.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.rdoTabNameOrder.Location = new System.Drawing.Point(1389, 7);
            this.rdoTabNameOrder.Name = "rdoTabNameOrder";
            this.rdoTabNameOrder.Size = new System.Drawing.Size(110, 24);
            this.rdoTabNameOrder.TabIndex = 0;
            this.rdoTabNameOrder.TabStop = true;
            this.rdoTabNameOrder.Text = "製造指図番号";
            // 
            // check1
            // 
            this.check1.AutoSize = true;
            this.check1.Location = new System.Drawing.Point(12, 71);
            this.check1.Name = "check1";
            this.check1.Size = new System.Drawing.Size(15, 14);
            this.check1.TabIndex = 24;
            this.check1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.label1.Location = new System.Drawing.Point(1265, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "タブ名称選択";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("游ゴシック", 20F);
            this.label8.Location = new System.Drawing.Point(785, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(285, 35);
            this.label8.TabIndex = 1;
            this.label8.Text = "液設備ロットトレース";
            // 
            // check2
            // 
            this.check2.AutoSize = true;
            this.check2.Location = new System.Drawing.Point(184, 71);
            this.check2.Name = "check2";
            this.check2.Size = new System.Drawing.Size(15, 14);
            this.check2.TabIndex = 24;
            this.check2.UseVisualStyleBackColor = true;
            // 
            // check3
            // 
            this.check3.AutoSize = true;
            this.check3.Location = new System.Drawing.Point(358, 71);
            this.check3.Name = "check3";
            this.check3.Size = new System.Drawing.Size(15, 14);
            this.check3.TabIndex = 24;
            this.check3.UseVisualStyleBackColor = true;
            // 
            // check4
            // 
            this.check4.AutoSize = true;
            this.check4.Location = new System.Drawing.Point(527, 71);
            this.check4.Name = "check4";
            this.check4.Size = new System.Drawing.Size(15, 14);
            this.check4.TabIndex = 24;
            this.check4.UseVisualStyleBackColor = true;
            // 
            // check5
            // 
            this.check5.AutoSize = true;
            this.check5.Location = new System.Drawing.Point(699, 71);
            this.check5.Name = "check5";
            this.check5.Size = new System.Drawing.Size(15, 14);
            this.check5.TabIndex = 24;
            this.check5.UseVisualStyleBackColor = true;
            // 
            // check6
            // 
            this.check6.AutoSize = true;
            this.check6.Location = new System.Drawing.Point(871, 71);
            this.check6.Name = "check6";
            this.check6.Size = new System.Drawing.Size(15, 14);
            this.check6.TabIndex = 24;
            this.check6.UseVisualStyleBackColor = true;
            // 
            // check7
            // 
            this.check7.AutoSize = true;
            this.check7.Location = new System.Drawing.Point(1042, 71);
            this.check7.Name = "check7";
            this.check7.Size = new System.Drawing.Size(15, 14);
            this.check7.TabIndex = 24;
            this.check7.UseVisualStyleBackColor = true;
            // 
            // check8
            // 
            this.check8.AutoSize = true;
            this.check8.Location = new System.Drawing.Point(1212, 71);
            this.check8.Name = "check8";
            this.check8.Size = new System.Drawing.Size(15, 14);
            this.check8.TabIndex = 24;
            this.check8.UseVisualStyleBackColor = true;
            // 
            // check10
            // 
            this.check10.AutoSize = true;
            this.check10.Location = new System.Drawing.Point(1560, 71);
            this.check10.Name = "check10";
            this.check10.Size = new System.Drawing.Size(15, 14);
            this.check10.TabIndex = 24;
            this.check10.UseVisualStyleBackColor = true;
            // 
            // check9
            // 
            this.check9.AutoSize = true;
            this.check9.Location = new System.Drawing.Point(1389, 71);
            this.check9.Name = "check9";
            this.check9.Size = new System.Drawing.Size(15, 14);
            this.check9.TabIndex = 24;
            this.check9.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1920, 1024);
            this.Controls.Add(this.swichTab);
            this.Controls.Add(this.check10);
            this.Controls.Add(this.check9);
            this.Controls.Add(this.check8);
            this.Controls.Add(this.check7);
            this.Controls.Add(this.check6);
            this.Controls.Add(this.check5);
            this.Controls.Add(this.check4);
            this.Controls.Add(this.check3);
            this.Controls.Add(this.check2);
            this.Controls.Add(this.check1);
            this.Controls.Add(this.rdoTabNameItemCode);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rdoTabNameOrder);
            this.Controls.Add(this.btnExcelOutput);
            this.Controls.Add(this.btnDetectCrossPoints);
            this.Controls.Add(this.btnBottleScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "液設備ロットトレース";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMiddle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEnd)).EndInit();
            this.swichTab.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView9)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView12)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView15)).EndInit();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView18)).EndInit();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView21)).EndInit();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView24)).EndInit();
            this.tabPage10.ResumeLayout(false);
            this.tabPage10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView27)).EndInit();
            this.IntersectionTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridIntersection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private BindingSource bindingSource1;
        private TabPage tabPage1;
        private Label lblProductionOrderNumber;
        private TextBox txtProductionOrderNumber;
        private TextBox txtItemName;
        private TextBox txtItemCode;
        private TextBox txtLotNumber;
        private Label lblItemName;
        private Label lblItemCode;
        private Label lblLotNumber;
        private Label lblTargetPeriod;
        private CheckBox chkUseFrom;
        private DateTimePicker dtpFrom;
        private Label lblTilde;
        private DateTimePicker dtpTo;
        private RadioButton rdoTabNameOrder;
        private RadioButton rdoTabNameItemCode;
        private Button btnClear;
        private Button btnCsvOutput;
        private Button btnExcelOutput;
        private Button btnDetectCrossPoints;
        private Button btnTraceSearch;
        private Button btnBottleScreen;
        private Panel panelStartHeader;
        private DataGridView dataGridStart;
        private Panel panelMiddleHeader;
        private DataGridView dataGridMiddle;
        private Panel panelEndHeader;
        private DataGridView dataGridEnd;
        private TabControl swichTab;
        private RadioButton rdoBackward;
        private RadioButton rdoForward;
        private CheckBox check1;
        private TabPage IntersectionTab;
        private DataGridView dataGridIntersection;
        private Label label1;
        private TabPage tabPage2;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private CheckBox checkBox2;
        private DateTimePicker dateTimePicker1;
        private Label label7;
        private DateTimePicker dateTimePicker2;
        private Button button1;
        private Button button2;
        private Button button3;
        private Panel panel1;
        private DataGridView dataGridView1;
        private Panel panel2;
        private DataGridView dataGridView2;
        private Panel panel3;
        private DataGridView dataGridView3;
        private Label label8;
        private TabPage tabPage3;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private Label label9;
        private TextBox textBox5;
        private TextBox textBox6;
        private TextBox textBox7;
        private TextBox textBox8;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private CheckBox checkBox3;
        private DateTimePicker dateTimePicker3;
        private Label label14;
        private DateTimePicker dateTimePicker4;
        private Button button4;
        private Button button5;
        private Button button6;
        private Panel panel4;
        private DataGridView dataGridView4;
        private Panel panel5;
        private DataGridView dataGridView5;
        private Panel panel6;
        private DataGridView dataGridView6;
        private TabPage tabPage4;
        private RadioButton radioButton5;
        private RadioButton radioButton6;
        private Label label15;
        private TextBox textBox9;
        private TextBox textBox10;
        private TextBox textBox11;
        private TextBox textBox12;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private CheckBox checkBox4;
        private DateTimePicker dateTimePicker5;
        private Label label20;
        private DateTimePicker dateTimePicker6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Panel panel7;
        private DataGridView dataGridView7;
        private Panel panel8;
        private DataGridView dataGridView8;
        private Panel panel9;
        private DataGridView dataGridView9;
        private TabPage tabPage5;
        private RadioButton radioButton7;
        private RadioButton radioButton8;
        private Label label21;
        private TextBox textBox13;
        private TextBox textBox14;
        private TextBox textBox15;
        private TextBox textBox16;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private CheckBox checkBox5;
        private DateTimePicker dateTimePicker7;
        private Label label26;
        private DateTimePicker dateTimePicker8;
        private Button button10;
        private Button button11;
        private Button button12;
        private Panel panel10;
        private DataGridView dataGridView10;
        private Panel panel11;
        private DataGridView dataGridView11;
        private Panel panel12;
        private DataGridView dataGridView12;
        private TabPage tabPage6;
        private RadioButton radioButton9;
        private RadioButton radioButton10;
        private Label label27;
        private TextBox textBox17;
        private TextBox textBox18;
        private TextBox textBox19;
        private TextBox textBox20;
        private Label label28;
        private Label label29;
        private Label label30;
        private Label label31;
        private CheckBox checkBox6;
        private DateTimePicker dateTimePicker9;
        private Label label32;
        private DateTimePicker dateTimePicker10;
        private Button button13;
        private Button button14;
        private Button button15;
        private Panel panel13;
        private DataGridView dataGridView13;
        private Panel panel14;
        private DataGridView dataGridView14;
        private Panel panel15;
        private DataGridView dataGridView15;
        private TabPage tabPage7;
        private RadioButton radioButton11;
        private RadioButton radioButton12;
        private Label label33;
        private TextBox textBox21;
        private TextBox textBox22;
        private TextBox textBox23;
        private TextBox textBox24;
        private Label label34;
        private Label label35;
        private Label label36;
        private Label label37;
        private CheckBox checkBox7;
        private DateTimePicker dateTimePicker11;
        private Label label38;
        private DateTimePicker dateTimePicker12;
        private Button button16;
        private Button button17;
        private Button button18;
        private Panel panel16;
        private DataGridView dataGridView16;
        private Panel panel17;
        private DataGridView dataGridView17;
        private Panel panel18;
        private DataGridView dataGridView18;
        private TabPage tabPage8;
        private RadioButton radioButton13;
        private RadioButton radioButton14;
        private Label label39;
        private TextBox textBox25;
        private TextBox textBox26;
        private TextBox textBox27;
        private TextBox textBox28;
        private Label label40;
        private Label label41;
        private Label label42;
        private Label label43;
        private CheckBox checkBox8;
        private DateTimePicker dateTimePicker13;
        private Label label44;
        private DateTimePicker dateTimePicker14;
        private Button button19;
        private Button button20;
        private Button button21;
        private Panel panel19;
        private DataGridView dataGridView19;
        private Panel panel20;
        private DataGridView dataGridView20;
        private Panel panel21;
        private DataGridView dataGridView21;
        private TabPage tabPage9;
        private RadioButton radioButton15;
        private RadioButton radioButton16;
        private Label label45;
        private TextBox textBox29;
        private TextBox textBox30;
        private TextBox textBox31;
        private TextBox textBox32;
        private Label label46;
        private Label label47;
        private Label label48;
        private Label label49;
        private CheckBox checkBox9;
        private DateTimePicker dateTimePicker15;
        private Label label50;
        private DateTimePicker dateTimePicker16;
        private Button button22;
        private Button button23;
        private Button button24;
        private Panel panel22;
        private DataGridView dataGridView22;
        private Panel panel23;
        private DataGridView dataGridView23;
        private Panel panel24;
        private DataGridView dataGridView24;
        private TabPage tabPage10;
        private RadioButton radioButton17;
        private RadioButton radioButton18;
        private Label label51;
        private TextBox textBox33;
        private TextBox textBox34;
        private TextBox textBox35;
        private TextBox textBox36;
        private Label label52;
        private Label label53;
        private Label label54;
        private Label label55;
        private CheckBox checkBox10;
        private DateTimePicker dateTimePicker17;
        private Label label56;
        private DateTimePicker dateTimePicker18;
        private Button button25;
        private Button button26;
        private Button button27;
        private Panel panel25;
        private DataGridView dataGridView25;
        private Panel panel26;
        private DataGridView dataGridView26;
        private Panel panel27;
        private DataGridView dataGridView27;
        private CheckBox check2;
        private CheckBox check3;
        private CheckBox check4;
        private CheckBox check5;
        private CheckBox check6;
        private CheckBox check7;
        private CheckBox check8;
        private CheckBox check9;
        private CheckBox check10;
    }
}
