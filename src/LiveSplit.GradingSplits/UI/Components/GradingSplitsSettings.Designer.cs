namespace LiveSplit.GradingSplits.UI.Components
{
    partial class GradingSplitsSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvThresholds = new System.Windows.Forms.DataGridView();
            this.colZScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddThreshold = new System.Windows.Forms.Button();
            this.btnRemoveThreshold = new System.Windows.Forms.Button();
            this.lblExplanation = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkUseBackground = new System.Windows.Forms.CheckBox();
            this.btnBackgroundColor = new System.Windows.Forms.Button();
            this.chkUseGold = new System.Windows.Forms.CheckBox();
            this.txtGoldLabel = new System.Windows.Forms.TextBox();
            this.btnGoldColor = new System.Windows.Forms.Button();
            this.chkUseWorst = new System.Windows.Forms.CheckBox();
            this.txtWorstLabel = new System.Windows.Forms.TextBox();
            this.btnWorstColor = new System.Windows.Forms.Button();
            this.btnResetDefaults = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThresholds)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnResetDefaults, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 336);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 174);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grade Thresholds (grades are assigned to the first threshold the z-score is less than)";
            // 
            // dgvThresholds
            // 
            this.dgvThresholds.AllowUserToAddRows = false;
            this.dgvThresholds.AllowUserToDeleteRows = false;
            this.dgvThresholds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvThresholds.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colZScore,
            this.colLabel,
            this.colColor});
            this.dgvThresholds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvThresholds.Location = new System.Drawing.Point(3, 3);
            this.dgvThresholds.Name = "dgvThresholds";
            this.dgvThresholds.RowHeadersVisible = false;
            this.dgvThresholds.Size = new System.Drawing.Size(444, 74);
            this.dgvThresholds.TabIndex = 0;
            // 
            // colZScore
            // 
            this.colZScore.HeaderText = "Z-Score Threshold";
            this.colZScore.Name = "colZScore";
            this.colZScore.Width = 120;
            // 
            // colLabel
            // 
            this.colLabel.HeaderText = "Grade Label";
            this.colLabel.Name = "colLabel";
            this.colLabel.Width = 80;
            // 
            // colColor
            // 
            this.colColor.HeaderText = "Color";
            this.colColor.Name = "colColor";
            this.colColor.ReadOnly = true;
            this.colColor.Width = 200;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnAddThreshold);
            this.flowLayoutPanel1.Controls.Add(this.btnRemoveThreshold);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 83);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(444, 29);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnAddThreshold
            // 
            this.btnAddThreshold.Location = new System.Drawing.Point(3, 3);
            this.btnAddThreshold.Name = "btnAddThreshold";
            this.btnAddThreshold.Size = new System.Drawing.Size(100, 23);
            this.btnAddThreshold.TabIndex = 0;
            this.btnAddThreshold.Text = "Add Threshold";
            this.btnAddThreshold.UseVisualStyleBackColor = true;
            // 
            // btnRemoveThreshold
            // 
            this.btnRemoveThreshold.Location = new System.Drawing.Point(109, 3);
            this.btnRemoveThreshold.Name = "btnRemoveThreshold";
            this.btnRemoveThreshold.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveThreshold.TabIndex = 1;
            this.btnRemoveThreshold.Text = "Remove Selected";
            this.btnRemoveThreshold.UseVisualStyleBackColor = true;
            // 
            // lblExplanation
            // 
            this.lblExplanation.AutoSize = true;
            this.lblExplanation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblExplanation.Location = new System.Drawing.Point(3, 115);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblExplanation.Size = new System.Drawing.Size(444, 40);
            this.lblExplanation.TabIndex = 2;
            this.lblExplanation.Text = "Z-Score = (Time - Mean) / StdDev. Lower z-scores are better (faster than average).\r\nClick a color cell to change the color. Thresholds are evaluated in order from top to bottom.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 183);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 114);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Display Options";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.Controls.Add(this.chkUseBackground, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnBackgroundColor, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkUseGold, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtGoldLabel, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnGoldColor, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkUseWorst, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtWorstLabel, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnWorstColor, 2, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(450, 95);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // chkUseBackground
            // 
            this.chkUseBackground.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkUseBackground.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.chkUseBackground, 2);
            this.chkUseBackground.Location = new System.Drawing.Point(3, 6);
            this.chkUseBackground.Name = "chkUseBackground";
            this.chkUseBackground.Size = new System.Drawing.Size(144, 17);
            this.chkUseBackground.TabIndex = 0;
            this.chkUseBackground.Text = "Use Background Color";
            this.chkUseBackground.UseVisualStyleBackColor = true;
            // 
            // btnBackgroundColor
            // 
            this.btnBackgroundColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackgroundColor.Location = new System.Drawing.Point(373, 3);
            this.btnBackgroundColor.Name = "btnBackgroundColor";
            this.btnBackgroundColor.Size = new System.Drawing.Size(74, 23);
            this.btnBackgroundColor.TabIndex = 1;
            this.btnBackgroundColor.UseVisualStyleBackColor = true;
            // 
            // chkUseGold
            // 
            this.chkUseGold.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkUseGold.AutoSize = true;
            this.chkUseGold.Location = new System.Drawing.Point(3, 36);
            this.chkUseGold.Name = "chkUseGold";
            this.chkUseGold.Size = new System.Drawing.Size(143, 17);
            this.chkUseGold.TabIndex = 2;
            this.chkUseGold.Text = "Special Gold Split Grade";
            this.chkUseGold.UseVisualStyleBackColor = true;
            // 
            // txtGoldLabel
            // 
            this.txtGoldLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGoldLabel.Location = new System.Drawing.Point(153, 35);
            this.txtGoldLabel.Name = "txtGoldLabel";
            this.txtGoldLabel.Size = new System.Drawing.Size(214, 20);
            this.txtGoldLabel.TabIndex = 3;
            // 
            // btnGoldColor
            // 
            this.btnGoldColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoldColor.Location = new System.Drawing.Point(373, 33);
            this.btnGoldColor.Name = "btnGoldColor";
            this.btnGoldColor.Size = new System.Drawing.Size(74, 23);
            this.btnGoldColor.TabIndex = 4;
            this.btnGoldColor.UseVisualStyleBackColor = true;
            // 
            // chkUseWorst
            // 
            this.chkUseWorst.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkUseWorst.AutoSize = true;
            this.chkUseWorst.Location = new System.Drawing.Point(3, 66);
            this.chkUseWorst.Name = "chkUseWorst";
            this.chkUseWorst.Size = new System.Drawing.Size(144, 17);
            this.chkUseWorst.TabIndex = 5;
            this.chkUseWorst.Text = "Special Worst Split Grade";
            this.chkUseWorst.UseVisualStyleBackColor = true;
            // 
            // txtWorstLabel
            // 
            this.txtWorstLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWorstLabel.Location = new System.Drawing.Point(153, 65);
            this.txtWorstLabel.Name = "txtWorstLabel";
            this.txtWorstLabel.Size = new System.Drawing.Size(214, 20);
            this.txtWorstLabel.TabIndex = 6;
            // 
            // btnWorstColor
            // 
            this.btnWorstColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWorstColor.Location = new System.Drawing.Point(373, 63);
            this.btnWorstColor.Name = "btnWorstColor";
            this.btnWorstColor.Size = new System.Drawing.Size(74, 23);
            this.btnWorstColor.TabIndex = 7;
            this.btnWorstColor.UseVisualStyleBackColor = true;
            // 
            // btnResetDefaults
            // 
            this.btnResetDefaults.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnResetDefaults.Location = new System.Drawing.Point(351, 306);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new System.Drawing.Size(108, 23);
            this.btnResetDefaults.TabIndex = 2;
            this.btnResetDefaults.Text = "Reset to Defaults";
            this.btnResetDefaults.UseVisualStyleBackColor = true;
            // 
            // GradingSplitsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GradingSplitsSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 350);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThresholds)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvThresholds;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chkUseBackground;
        private System.Windows.Forms.Button btnBackgroundColor;
        private System.Windows.Forms.CheckBox chkUseGold;
        private System.Windows.Forms.TextBox txtGoldLabel;
        private System.Windows.Forms.Button btnGoldColor;
        private System.Windows.Forms.CheckBox chkUseWorst;
        private System.Windows.Forms.TextBox txtWorstLabel;
        private System.Windows.Forms.Button btnWorstColor;
        private System.Windows.Forms.Button btnResetDefaults;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnAddThreshold;
        private System.Windows.Forms.Button btnRemoveThreshold;
        private System.Windows.Forms.Label lblExplanation;
    }
}
