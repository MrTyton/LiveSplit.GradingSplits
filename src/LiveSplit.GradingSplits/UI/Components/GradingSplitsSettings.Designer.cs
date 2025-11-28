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
            this.lblMode = new System.Windows.Forms.Label();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblMode, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbMode, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 136);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblMode
            // 
            this.lblMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(3, 8);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(37, 13);
            this.lblMode.TabIndex = 0;
            this.lblMode.Text = "Mode:";
            // 
            // cmbMode
            // 
            this.cmbMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Items.AddRange(new object[] {
            "Single Component",
            "List Component"});
            this.cmbMode.Location = new System.Drawing.Point(103, 4);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(356, 21);
            this.cmbMode.TabIndex = 1;
            // 
            // GradingSplitsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GradingSplitsSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 150);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cmbMode;
    }
}
