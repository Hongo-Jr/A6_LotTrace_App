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
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            // フォーム共通設定
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1920, 1024);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "BottleTraceForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "瓶設備ロットトレース";

            // ==============================
            // 検索条件 [26]〜[30]
            // ==============================
            this.lblOrderNumber = new Label();
            this.lblOrderNumber.AutoSize = true;
            this.lblOrderNumber.Location = new System.Drawing.Point(20, 20);
            this.lblOrderNumber.Text = "製造指図番号 [26]";

            this.txtOrderNumber = new TextBox();
            this.txtOrderNumber.Location = new System.Drawing.Point(120, 17);
            this.txtOrderNumber.Size = new System.Drawing.Size(170, 23);

            this.lblBottleItemName = new Label();
            this.lblBottleItemName.AutoSize = true;
            this.lblBottleItemName.Location = new System.Drawing.Point(310, 20);
            this.lblBottleItemName.Text = "品目名 [27]";

            this.txtItemName = new TextBox();
            this.txtItemName.Location = new System.Drawing.Point(360, 17);
            this.txtItemName.Size = new System.Drawing.Size(220, 23);

            this.lblBottleItemCode = new Label();
            this.lblBottleItemCode.AutoSize = true;
            this.lblBottleItemCode.Location = new System.Drawing.Point(600, 20);
            this.lblBottleItemCode.Text = "品目コード [28]";

            this.txtItemCode = new TextBox();
            this.txtItemCode.Location = new System.Drawing.Point(680, 17);
            this.txtItemCode.Size = new System.Drawing.Size(150, 23);

            this.lblBottleLotNo = new Label();
            this.lblBottleLotNo.AutoSize = true;
            this.lblBottleLotNo.Location = new System.Drawing.Point(850, 20);
            this.lblBottleLotNo.Text = "ロット番号 [29]";

            this.txtLotNumber = new TextBox();
            this.txtLotNumber.Location = new System.Drawing.Point(910, 17);
            this.txtLotNumber.Size = new System.Drawing.Size(150, 23);

            // [30] 対象期間
            this.lblBottlePeriod = new Label();
            this.lblBottlePeriod.AutoSize = true;
            this.lblBottlePeriod.Location = new System.Drawing.Point(20, 50);
            this.lblBottlePeriod.Text = "対象期間 [30]";

            this.chkUseFrom = new CheckBox();
            this.chkUseFrom.Location = new System.Drawing.Point(120, 50);
            this.chkUseFrom.Size = new System.Drawing.Size(15, 14);

            this.dtpFrom = new DateTimePicker();
            this.dtpFrom.CustomFormat = "yyyy/MM/dd";
            this.dtpFrom.Format = DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(140, 45);
            this.dtpFrom.Size = new System.Drawing.Size(120, 23);

            this.lblBottleTilde = new Label();
            this.lblBottleTilde.AutoSize = true;
            this.lblBottleTilde.Location = new System.Drawing.Point(265, 50);
            this.lblBottleTilde.Text = "～";

            this.chkUseTo = new CheckBox();
            this.chkUseTo.Location = new System.Drawing.Point(285, 50);
            this.chkUseTo.Size = new System.Drawing.Size(15, 14);

            this.dtpTo = new DateTimePicker();
            this.dtpTo.CustomFormat = "yyyy/MM/dd";
            this.dtpTo.Format = DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(305, 45);
            this.dtpTo.Size = new System.Drawing.Size(120, 23);

            // [31] トレース方向選択
            this.grpDirectionBottle = new GroupBox();
            this.grpDirectionBottle.Text = "トレース方向 [31]";
            this.grpDirectionBottle.Location = new System.Drawing.Point(450, 45);
            this.grpDirectionBottle.Size = new System.Drawing.Size(260, 40);

            this.rdoForwardBottle = new RadioButton();
            this.rdoForwardBottle.Text = "トレースフォワード(追跡)";
            this.rdoForwardBottle.Location = new System.Drawing.Point(10, 16);
            this.rdoForwardBottle.Checked = true;

            this.rdoBackwardBottle = new RadioButton();
            this.rdoBackwardBottle.Text = "トレースバック(遡及)";
            this.rdoBackwardBottle.Location = new System.Drawing.Point(140, 16);

            this.grpDirectionBottle.Controls.Add(this.rdoForwardBottle);
            this.grpDirectionBottle.Controls.Add(this.rdoBackwardBottle);

            // [32] クリア
            this.btnClearBottle = new Button();
            this.btnClearBottle.Location = new System.Drawing.Point(730, 50);
            this.btnClearBottle.Size = new System.Drawing.Size(80, 30);
            this.btnClearBottle.Text = "クリア [32]";
            this.btnClearBottle.Click += new System.EventHandler(this.btnClearBottle_Click);

            // [33] CSV 出力
            this.btnCsvOutputBottle = new Button();
            this.btnCsvOutputBottle.Location = new System.Drawing.Point(816, 50);
            this.btnCsvOutputBottle.Size = new System.Drawing.Size(90, 30);
            this.btnCsvOutputBottle.Text = "CSV出力 [33]";
            this.btnCsvOutputBottle.Click += new System.EventHandler(this.btnCsvOutputBottle_Click);

            // [34] トレース検索
            this.btnTraceSearchBottle = new Button();
            this.btnTraceSearchBottle.Location = new System.Drawing.Point(912, 50);
            this.btnTraceSearchBottle.Size = new System.Drawing.Size(120, 30);
            this.btnTraceSearchBottle.Text = "トレース検索 [34]";
            this.btnTraceSearchBottle.Click += new System.EventHandler(this.btnTraceSearchBottle_Click);

            // [37] 液設備ボタン
            this.btnBackToLiquid = new Button();
            this.btnBackToLiquid.Location = new System.Drawing.Point(1038, 50);
            this.btnBackToLiquid.Size = new System.Drawing.Size(120, 30);
            this.btnBackToLiquid.Text = "液設備 [37]";
            this.btnBackToLiquid.Click += new System.EventHandler(this.btnBackToLiquid_Click);

            // [35] 検索始点データ
            this.lblBottleStart = new Label();
            this.lblBottleStart.AutoSize = true;
            this.lblBottleStart.Location = new System.Drawing.Point(20, 90);
            this.lblBottleStart.Text = "検索始点データ [35]";

            this.dgvStartBottle = new DataGridView();
            this.dgvStartBottle.Location = new System.Drawing.Point(20, 110);
            this.dgvStartBottle.Size = new System.Drawing.Size(700, 860);
            this.dgvStartBottle.ReadOnly = true;
            this.dgvStartBottle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStartBottle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // [36] 検索終点データ
            this.lblBottleEnd = new Label();
            this.lblBottleEnd.AutoSize = true;
            this.lblBottleEnd.Location = new System.Drawing.Point(740, 90);
            this.lblBottleEnd.Text = "検索終点データ [36]";

            this.dgvEndBottle = new DataGridView();
            this.dgvEndBottle.Location = new System.Drawing.Point(740, 110);
            this.dgvEndBottle.Size = new System.Drawing.Size(1160, 860);
            this.dgvEndBottle.ReadOnly = true;
            this.dgvEndBottle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndBottle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // フォームに追加
            this.Controls.Add(this.lblOrderNumber);
            this.Controls.Add(this.txtOrderNumber);
            this.Controls.Add(this.lblBottleItemName);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.lblBottleItemCode);
            this.Controls.Add(this.txtItemCode);
            this.Controls.Add(this.lblBottleLotNo);
            this.Controls.Add(this.txtLotNumber);
            this.Controls.Add(this.lblBottlePeriod);
            this.Controls.Add(this.chkUseFrom);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.lblBottleTilde);
            this.Controls.Add(this.chkUseTo);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.grpDirectionBottle);
            this.Controls.Add(this.btnClearBottle);
            this.Controls.Add(this.btnCsvOutputBottle);
            this.Controls.Add(this.btnTraceSearchBottle);
            this.Controls.Add(this.btnBackToLiquid);
            this.Controls.Add(this.lblBottleStart);
            this.Controls.Add(this.dgvStartBottle);
            this.Controls.Add(this.lblBottleEnd);
            this.Controls.Add(this.dgvEndBottle);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // コントロールフィールド [26]〜[37]
        private Label lblOrderNumber;   // [26]
        private TextBox txtOrderNumber;
        private Label lblBottleItemName; // [27]
        private TextBox txtItemName;
        private Label lblBottleItemCode; // [28]
        private TextBox txtItemCode;
        private Label lblBottleLotNo;    // [29]
        private TextBox txtLotNumber;
        private Label lblBottlePeriod;   // [30]
        private CheckBox chkUseFrom;
        private DateTimePicker dtpFrom;
        private Label lblBottleTilde;
        private CheckBox chkUseTo;
        private DateTimePicker dtpTo;
        private GroupBox grpDirectionBottle;  // [31]
        private RadioButton rdoForwardBottle;
        private RadioButton rdoBackwardBottle;
        private Button btnClearBottle;       // [32]
        private Button btnCsvOutputBottle;   // [33]
        private Button btnTraceSearchBottle; // [34]
        private Button btnBackToLiquid;      // [37]
        private Label lblBottleStart;        // [35]
        private DataGridView dgvStartBottle;
        private Label lblBottleEnd;          // [36]
        private DataGridView dgvEndBottle;
    }
}