namespace LotTraceApp.Forms
{
    partial class BottleResult
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
            this.BottleDataGridView = new System.Windows.Forms.DataGridView();
            this.grp2 = new System.Windows.Forms.GroupBox();
            this.lbl_PageNum = new System.Windows.Forms.Label();
            this.btn_PagePrev = new System.Windows.Forms.Button();
            this.btn_RowsSetting = new System.Windows.Forms.Button();
            this.btn_PageNext = new System.Windows.Forms.Button();
            this.lbl_DispNum = new System.Windows.Forms.Label();
            this.grp1 = new System.Windows.Forms.GroupBox();
            this.rdo_Result = new System.Windows.Forms.RadioButton();
            this.rdo_Order = new System.Windows.Forms.RadioButton();
            this.lbl_Pro_LotNo = new System.Windows.Forms.Label();
            this.lbl_Pro_Code = new System.Windows.Forms.Label();
            this.lbl_Pro_Name = new System.Windows.Forms.Label();
            this.lbl_Pro_OrderNo = new System.Windows.Forms.Label();
            this.lbl_Disp_Pro_LotNo = new System.Windows.Forms.Label();
            this.lbl_Disp_Pro_Code = new System.Windows.Forms.Label();
            this.lbl_Disp_Pro_Name = new System.Windows.Forms.Label();
            this.lbl_Disp_Pro_OrederNo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.BottleDataGridView)).BeginInit();
            this.grp2.SuspendLayout();
            this.grp1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottleDataGridView
            // 
            this.BottleDataGridView.AllowUserToAddRows = false;
            this.BottleDataGridView.AllowUserToDeleteRows = false;
            this.BottleDataGridView.AllowUserToOrderColumns = true;
            this.BottleDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BottleDataGridView.Location = new System.Drawing.Point(20, 50);
            this.BottleDataGridView.Name = "BottleDataGridView";
            this.BottleDataGridView.ReadOnly = true;
            this.BottleDataGridView.RowTemplate.Height = 21;
            this.BottleDataGridView.Size = new System.Drawing.Size(860, 430);
            this.BottleDataGridView.TabIndex = 0;
            // 
            // grp2
            // 
            this.grp2.BackColor = System.Drawing.SystemColors.Control;
            this.grp2.Controls.Add(this.lbl_PageNum);
            this.grp2.Controls.Add(this.btn_PagePrev);
            this.grp2.Controls.Add(this.btn_RowsSetting);
            this.grp2.Controls.Add(this.btn_PageNext);
            this.grp2.Controls.Add(this.BottleDataGridView);
            this.grp2.Controls.Add(this.lbl_DispNum);
            this.grp2.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.grp2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.grp2.Location = new System.Drawing.Point(15, 120);
            this.grp2.Name = "grp2";
            this.grp2.Size = new System.Drawing.Size(900, 520);
            this.grp2.TabIndex = 6;
            this.grp2.TabStop = false;
            this.grp2.Text = "履歴詳細データ";
            // 
            // lbl_PageNum
            // 
            this.lbl_PageNum.BackColor = System.Drawing.Color.White;
            this.lbl_PageNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_PageNum.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PageNum.Location = new System.Drawing.Point(694, 485);
            this.lbl_PageNum.Name = "lbl_PageNum";
            this.lbl_PageNum.Size = new System.Drawing.Size(120, 30);
            this.lbl_PageNum.TabIndex = 2;
            this.lbl_PageNum.Text = "9999 / 9999";
            this.lbl_PageNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_PagePrev
            // 
            this.btn_PagePrev.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_PagePrev.FlatAppearance.BorderSize = 0;
            this.btn_PagePrev.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_PagePrev.ForeColor = System.Drawing.Color.Black;
            this.btn_PagePrev.Location = new System.Drawing.Point(628, 485);
            this.btn_PagePrev.Name = "btn_PagePrev";
            this.btn_PagePrev.Size = new System.Drawing.Size(60, 30);
            this.btn_PagePrev.TabIndex = 1;
            this.btn_PagePrev.Text = "◀◀";
            this.btn_PagePrev.UseCompatibleTextRendering = true;
            this.btn_PagePrev.UseVisualStyleBackColor = true;
            this.btn_PagePrev.Click += new System.EventHandler(this.btn_PagePrev_Click);
            // 
            // btn_RowsSetting
            // 
            this.btn_RowsSetting.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btn_RowsSetting.ForeColor = System.Drawing.Color.Black;
            this.btn_RowsSetting.Location = new System.Drawing.Point(805, 19);
            this.btn_RowsSetting.Name = "btn_RowsSetting";
            this.btn_RowsSetting.Size = new System.Drawing.Size(75, 25);
            this.btn_RowsSetting.TabIndex = 1;
            this.btn_RowsSetting.Text = "列設定";
            this.btn_RowsSetting.UseVisualStyleBackColor = true;
            this.btn_RowsSetting.Click += new System.EventHandler(this.btn_RowsSetting_Click);
            // 
            // btn_PageNext
            // 
            this.btn_PageNext.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_PageNext.ForeColor = System.Drawing.Color.Black;
            this.btn_PageNext.Location = new System.Drawing.Point(820, 485);
            this.btn_PageNext.Name = "btn_PageNext";
            this.btn_PageNext.Size = new System.Drawing.Size(60, 30);
            this.btn_PageNext.TabIndex = 1;
            this.btn_PageNext.Text = "▶▶";
            this.btn_PageNext.UseCompatibleTextRendering = true;
            this.btn_PageNext.UseVisualStyleBackColor = true;
            this.btn_PageNext.Click += new System.EventHandler(this.btn_PageNext_Click);
            // 
            // lbl_DispNum
            // 
            this.lbl_DispNum.AutoSize = true;
            this.lbl_DispNum.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_DispNum.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_DispNum.Location = new System.Drawing.Point(17, 485);
            this.lbl_DispNum.Name = "lbl_DispNum";
            this.lbl_DispNum.Size = new System.Drawing.Size(118, 17);
            this.lbl_DispNum.TabIndex = 3;
            this.lbl_DispNum.Text = "表示件数：9999 件";
            // 
            // grp1
            // 
            this.grp1.Controls.Add(this.rdo_Result);
            this.grp1.Controls.Add(this.rdo_Order);
            this.grp1.Controls.Add(this.lbl_Pro_LotNo);
            this.grp1.Controls.Add(this.lbl_Pro_Code);
            this.grp1.Controls.Add(this.lbl_Pro_Name);
            this.grp1.Controls.Add(this.lbl_Pro_OrderNo);
            this.grp1.Controls.Add(this.lbl_Disp_Pro_LotNo);
            this.grp1.Controls.Add(this.lbl_Disp_Pro_Code);
            this.grp1.Controls.Add(this.lbl_Disp_Pro_Name);
            this.grp1.Controls.Add(this.lbl_Disp_Pro_OrederNo);
            this.grp1.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.grp1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.grp1.Location = new System.Drawing.Point(15, 10);
            this.grp1.Name = "grp1";
            this.grp1.Size = new System.Drawing.Size(900, 100);
            this.grp1.TabIndex = 7;
            this.grp1.TabStop = false;
            this.grp1.Text = "指図情報";
            // 
            // rdo_Result
            // 
            this.rdo_Result.AutoSize = true;
            this.rdo_Result.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rdo_Result.Location = new System.Drawing.Point(714, 60);
            this.rdo_Result.Name = "rdo_Result";
            this.rdo_Result.Size = new System.Drawing.Size(157, 25);
            this.rdo_Result.TabIndex = 4;
            this.rdo_Result.Text = "瓶 / ドラム缶実績";
            this.rdo_Result.UseVisualStyleBackColor = true;
            // 
            // rdo_Order
            // 
            this.rdo_Order.AutoSize = true;
            this.rdo_Order.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rdo_Order.Location = new System.Drawing.Point(714, 32);
            this.rdo_Order.Name = "rdo_Order";
            this.rdo_Order.Size = new System.Drawing.Size(60, 25);
            this.rdo_Order.TabIndex = 4;
            this.rdo_Order.Text = "指図";
            this.rdo_Order.UseVisualStyleBackColor = true;
            // 
            // lbl_Pro_LotNo
            // 
            this.lbl_Pro_LotNo.AutoSize = true;
            this.lbl_Pro_LotNo.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_Pro_LotNo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Pro_LotNo.Location = new System.Drawing.Point(287, 31);
            this.lbl_Pro_LotNo.Name = "lbl_Pro_LotNo";
            this.lbl_Pro_LotNo.Size = new System.Drawing.Size(95, 17);
            this.lbl_Pro_LotNo.TabIndex = 3;
            this.lbl_Pro_LotNo.Text = "製品ロットNo.";
            // 
            // lbl_Pro_Code
            // 
            this.lbl_Pro_Code.AutoSize = true;
            this.lbl_Pro_Code.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_Pro_Code.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Pro_Code.Location = new System.Drawing.Point(35, 60);
            this.lbl_Pro_Code.Name = "lbl_Pro_Code";
            this.lbl_Pro_Code.Size = new System.Drawing.Size(99, 17);
            this.lbl_Pro_Code.TabIndex = 3;
            this.lbl_Pro_Code.Text = "製品品目コード";
            // 
            // lbl_Pro_Name
            // 
            this.lbl_Pro_Name.AutoSize = true;
            this.lbl_Pro_Name.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_Pro_Name.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Pro_Name.Location = new System.Drawing.Point(287, 60);
            this.lbl_Pro_Name.Name = "lbl_Pro_Name";
            this.lbl_Pro_Name.Size = new System.Drawing.Size(73, 17);
            this.lbl_Pro_Name.TabIndex = 3;
            this.lbl_Pro_Name.Text = "製品品目名";
            // 
            // lbl_Pro_OrderNo
            // 
            this.lbl_Pro_OrderNo.AutoSize = true;
            this.lbl_Pro_OrderNo.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_Pro_OrderNo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Pro_OrderNo.Location = new System.Drawing.Point(35, 30);
            this.lbl_Pro_OrderNo.Name = "lbl_Pro_OrderNo";
            this.lbl_Pro_OrderNo.Size = new System.Drawing.Size(82, 17);
            this.lbl_Pro_OrderNo.TabIndex = 3;
            this.lbl_Pro_OrderNo.Text = "製造指図No.";
            // 
            // lbl_Disp_Pro_LotNo
            // 
            this.lbl_Disp_Pro_LotNo.AutoSize = true;
            this.lbl_Disp_Pro_LotNo.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.lbl_Disp_Pro_LotNo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Disp_Pro_LotNo.Location = new System.Drawing.Point(402, 31);
            this.lbl_Disp_Pro_LotNo.Name = "lbl_Disp_Pro_LotNo";
            this.lbl_Disp_Pro_LotNo.Size = new System.Drawing.Size(93, 18);
            this.lbl_Disp_Pro_LotNo.TabIndex = 3;
            this.lbl_Disp_Pro_LotNo.Text = "SA01_Lot_aa";
            // 
            // lbl_Disp_Pro_Code
            // 
            this.lbl_Disp_Pro_Code.AutoSize = true;
            this.lbl_Disp_Pro_Code.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.lbl_Disp_Pro_Code.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Disp_Pro_Code.Location = new System.Drawing.Point(150, 60);
            this.lbl_Disp_Pro_Code.Name = "lbl_Disp_Pro_Code";
            this.lbl_Disp_Pro_Code.Size = new System.Drawing.Size(106, 18);
            this.lbl_Disp_Pro_Code.TabIndex = 3;
            this.lbl_Disp_Pro_Code.Text = "SA01_Code_aa";
            // 
            // lbl_Disp_Pro_Name
            // 
            this.lbl_Disp_Pro_Name.AutoSize = true;
            this.lbl_Disp_Pro_Name.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.lbl_Disp_Pro_Name.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Disp_Pro_Name.Location = new System.Drawing.Point(402, 60);
            this.lbl_Disp_Pro_Name.Name = "lbl_Disp_Pro_Name";
            this.lbl_Disp_Pro_Name.Size = new System.Drawing.Size(110, 18);
            this.lbl_Disp_Pro_Name.TabIndex = 3;
            this.lbl_Disp_Pro_Name.Text = "SA01_Name_aa";
            // 
            // lbl_Disp_Pro_OrederNo
            // 
            this.lbl_Disp_Pro_OrederNo.AutoSize = true;
            this.lbl_Disp_Pro_OrederNo.Font = new System.Drawing.Font("游ゴシック", 10F);
            this.lbl_Disp_Pro_OrederNo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbl_Disp_Pro_OrederNo.Location = new System.Drawing.Point(150, 30);
            this.lbl_Disp_Pro_OrederNo.Name = "lbl_Disp_Pro_OrederNo";
            this.lbl_Disp_Pro_OrederNo.Size = new System.Drawing.Size(65, 18);
            this.lbl_Disp_Pro_OrederNo.TabIndex = 3;
            this.lbl_Disp_Pro_OrederNo.Text = "SA01_aa";
            // 
            // BottleResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 661);
            this.Controls.Add(this.grp1);
            this.Controls.Add(this.grp2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BottleResult";
            this.Text = "瓶履歴詳細";
            ((System.ComponentModel.ISupportInitialize)(this.BottleDataGridView)).EndInit();
            this.grp2.ResumeLayout(false);
            this.grp2.PerformLayout();
            this.grp1.ResumeLayout(false);
            this.grp1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView BottleDataGridView;
        private System.Windows.Forms.GroupBox grp2;
        private System.Windows.Forms.GroupBox grp1;
        private System.Windows.Forms.RadioButton rdo_Result;
        private System.Windows.Forms.RadioButton rdo_Order;
        private System.Windows.Forms.Label lbl_Pro_LotNo;
        private System.Windows.Forms.Label lbl_Pro_Code;
        private System.Windows.Forms.Label lbl_Pro_Name;
        private System.Windows.Forms.Label lbl_Pro_OrderNo;
        private System.Windows.Forms.Label lbl_Disp_Pro_LotNo;
        private System.Windows.Forms.Label lbl_Disp_Pro_Code;
        private System.Windows.Forms.Label lbl_Disp_Pro_Name;
        private System.Windows.Forms.Label lbl_Disp_Pro_OrederNo;
        private System.Windows.Forms.Button btn_PageNext;
        private System.Windows.Forms.Button btn_PagePrev;
        private System.Windows.Forms.Label lbl_PageNum;
        private System.Windows.Forms.Button btn_RowsSetting;
        private System.Windows.Forms.Label lbl_DispNum;
    }
}