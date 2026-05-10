namespace LotTraceApp.Forms
{
    partial class Result
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.cmbTarget = new System.Windows.Forms.ComboBox();
            this.OrderNo = new System.Windows.Forms.Label();
            this.ProcName = new System.Windows.Forms.Label();
            this.ProcCode = new System.Windows.Forms.Label();
            this.LotNo = new System.Windows.Forms.Label();
            this.OrederNo1 = new System.Windows.Forms.Label();
            this.ProcName1 = new System.Windows.Forms.Label();
            this.ProcCode1 = new System.Windows.Forms.Label();
            this.LotNo1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.grp1 = new System.Windows.Forms.GroupBox();
            this.grp2 = new System.Windows.Forms.GroupBox();
            this.ResultView = new System.Windows.Forms.DataGridView();
            this.rdoBoth = new System.Windows.Forms.RadioButton();
            this.rdoOrderOnly = new System.Windows.Forms.RadioButton();
            this.rdoActualOnly = new System.Windows.Forms.RadioButton();
            this.grp1.SuspendLayout();
            this.grp2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("游ゴシック", 9F);
            this.btnClose.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnClose.Location = new System.Drawing.Point(821, 622);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cmbTarget
            // 
            this.cmbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTarget.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.cmbTarget.ForeColor = System.Drawing.SystemColors.InfoText;
            this.cmbTarget.FormattingEnabled = true;
            this.cmbTarget.Location = new System.Drawing.Point(681, 26);
            this.cmbTarget.Name = "cmbTarget";
            this.cmbTarget.Size = new System.Drawing.Size(180, 25);
            this.cmbTarget.TabIndex = 1;
            // 
            // OrderNo
            // 
            this.OrderNo.AutoSize = true;
            this.OrderNo.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.OrderNo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.OrderNo.Location = new System.Drawing.Point(35, 30);
            this.OrderNo.Name = "OrderNo";
            this.OrderNo.Size = new System.Drawing.Size(86, 18);
            this.OrderNo.TabIndex = 3;
            this.OrderNo.Text = "製造指図No.";
            // 
            // ProcName
            // 
            this.ProcName.AutoSize = true;
            this.ProcName.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.ProcName.ForeColor = System.Drawing.SystemColors.InfoText;
            this.ProcName.Location = new System.Drawing.Point(35, 120);
            this.ProcName.Name = "ProcName";
            this.ProcName.Size = new System.Drawing.Size(50, 18);
            this.ProcName.TabIndex = 3;
            this.ProcName.Text = "品目名";
            // 
            // ProcCode
            // 
            this.ProcCode.AutoSize = true;
            this.ProcCode.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.ProcCode.ForeColor = System.Drawing.SystemColors.InfoText;
            this.ProcCode.Location = new System.Drawing.Point(35, 60);
            this.ProcCode.Name = "ProcCode";
            this.ProcCode.Size = new System.Drawing.Size(78, 18);
            this.ProcCode.TabIndex = 3;
            this.ProcCode.Text = "品目コード";
            // 
            // LotNo
            // 
            this.LotNo.AutoSize = true;
            this.LotNo.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.LotNo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.LotNo.Location = new System.Drawing.Point(35, 90);
            this.LotNo.Name = "LotNo";
            this.LotNo.Size = new System.Drawing.Size(72, 18);
            this.LotNo.TabIndex = 3;
            this.LotNo.Text = "ロットNo.";
            // 
            // OrederNo1
            // 
            this.OrederNo1.AutoSize = true;
            this.OrederNo1.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.OrederNo1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.OrederNo1.Location = new System.Drawing.Point(150, 30);
            this.OrederNo1.Name = "OrederNo1";
            this.OrederNo1.Size = new System.Drawing.Size(65, 18);
            this.OrederNo1.TabIndex = 3;
            this.OrederNo1.Text = "SA01_aa";
            // 
            // ProcName1
            // 
            this.ProcName1.AutoSize = true;
            this.ProcName1.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.ProcName1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.ProcName1.Location = new System.Drawing.Point(150, 120);
            this.ProcName1.Name = "ProcName1";
            this.ProcName1.Size = new System.Drawing.Size(110, 18);
            this.ProcName1.TabIndex = 3;
            this.ProcName1.Text = "SA01_Name_aa";
            // 
            // ProcCode1
            // 
            this.ProcCode1.AutoSize = true;
            this.ProcCode1.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.ProcCode1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.ProcCode1.Location = new System.Drawing.Point(150, 60);
            this.ProcCode1.Name = "ProcCode1";
            this.ProcCode1.Size = new System.Drawing.Size(106, 18);
            this.ProcCode1.TabIndex = 3;
            this.ProcCode1.Text = "SA01_Code_aa";
            // 
            // LotNo1
            // 
            this.LotNo1.AutoSize = true;
            this.LotNo1.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.LotNo1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.LotNo1.Location = new System.Drawing.Point(150, 90);
            this.LotNo1.Name = "LotNo1";
            this.LotNo1.Size = new System.Drawing.Size(93, 18);
            this.LotNo1.TabIndex = 3;
            this.LotNo1.Text = "SA01_Lot_aa";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.label6.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label6.Location = new System.Drawing.Point(614, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "表示対象";
            // 
            // grp1
            // 
            this.grp1.Controls.Add(this.rdoActualOnly);
            this.grp1.Controls.Add(this.rdoOrderOnly);
            this.grp1.Controls.Add(this.rdoBoth);
            this.grp1.Controls.Add(this.label6);
            this.grp1.Controls.Add(this.LotNo);
            this.grp1.Controls.Add(this.ProcCode);
            this.grp1.Controls.Add(this.ProcName);
            this.grp1.Controls.Add(this.OrderNo);
            this.grp1.Controls.Add(this.LotNo1);
            this.grp1.Controls.Add(this.ProcCode1);
            this.grp1.Controls.Add(this.ProcName1);
            this.grp1.Controls.Add(this.OrederNo1);
            this.grp1.Controls.Add(this.cmbTarget);
            this.grp1.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.grp1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.grp1.Location = new System.Drawing.Point(15, 10);
            this.grp1.Name = "grp1";
            this.grp1.Size = new System.Drawing.Size(900, 150);
            this.grp1.TabIndex = 4;
            this.grp1.TabStop = false;
            this.grp1.Text = "指図情報";
            // 
            // grp2
            // 
            this.grp2.BackColor = System.Drawing.SystemColors.Control;
            this.grp2.Controls.Add(this.ResultView);
            this.grp2.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.grp2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.grp2.Location = new System.Drawing.Point(15, 165);
            this.grp2.Name = "grp2";
            this.grp2.Size = new System.Drawing.Size(899, 451);
            this.grp2.TabIndex = 5;
            this.grp2.TabStop = false;
            this.grp2.Text = "履歴詳細データ";
            // 
            // ResultView
            // 
            this.ResultView.AllowUserToAddRows = false;
            this.ResultView.AllowUserToDeleteRows = false;
            this.ResultView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultView.Location = new System.Drawing.Point(6, 27);
            this.ResultView.Name = "ResultView";
            this.ResultView.ReadOnly = true;
            this.ResultView.RowHeadersVisible = false;
            this.ResultView.RowTemplate.Height = 21;
            this.ResultView.Size = new System.Drawing.Size(880, 407);
            this.ResultView.TabIndex = 3;
            // 
            // rdoBoth
            // 
            this.rdoBoth.AccessibleDescription = "";
            this.rdoBoth.AutoSize = true;
            this.rdoBoth.Checked = true;
            this.rdoBoth.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rdoBoth.Location = new System.Drawing.Point(617, 60);
            this.rdoBoth.Name = "rdoBoth";
            this.rdoBoth.Size = new System.Drawing.Size(75, 25);
            this.rdoBoth.TabIndex = 4;
            this.rdoBoth.TabStop = true;
            this.rdoBoth.Text = "すべて";
            this.rdoBoth.UseVisualStyleBackColor = true;
            // 
            // rdoOrderOnly
            // 
            this.rdoOrderOnly.AutoSize = true;
            this.rdoOrderOnly.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rdoOrderOnly.Location = new System.Drawing.Point(699, 60);
            this.rdoOrderOnly.Name = "rdoOrderOnly";
            this.rdoOrderOnly.Size = new System.Drawing.Size(92, 25);
            this.rdoOrderOnly.TabIndex = 4;
            this.rdoOrderOnly.Text = "指図のみ";
            this.rdoOrderOnly.UseVisualStyleBackColor = true;
            // 
            // rdoActualOnly
            // 
            this.rdoActualOnly.AutoSize = true;
            this.rdoActualOnly.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rdoActualOnly.Location = new System.Drawing.Point(797, 60);
            this.rdoActualOnly.Name = "rdoActualOnly";
            this.rdoActualOnly.Size = new System.Drawing.Size(92, 25);
            this.rdoActualOnly.TabIndex = 4;
            this.rdoActualOnly.Text = "実績のみ";
            this.rdoActualOnly.UseVisualStyleBackColor = true;
            // 
            // Result
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 658);
            this.Controls.Add(this.grp2);
            this.Controls.Add(this.grp1);
            this.Controls.Add(this.btnClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Result";
            this.Text = "履歴詳細";
            this.Load += new System.EventHandler(this.Result_Load);
            this.grp1.ResumeLayout(false);
            this.grp1.PerformLayout();
            this.grp2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cmbTarget;
        private System.Windows.Forms.Label OrderNo;
        private System.Windows.Forms.Label ProcName;
        private System.Windows.Forms.Label ProcCode;
        private System.Windows.Forms.Label LotNo;
        private System.Windows.Forms.Label OrederNo1;
        private System.Windows.Forms.Label ProcName1;
        private System.Windows.Forms.Label ProcCode1;
        private System.Windows.Forms.Label LotNo1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grp1;
        private System.Windows.Forms.GroupBox grp2;
        private System.Windows.Forms.DataGridView ResultView;
        private System.Windows.Forms.RadioButton rdoActualOnly;
        private System.Windows.Forms.RadioButton rdoOrderOnly;
        private System.Windows.Forms.RadioButton rdoBoth;
    }
}