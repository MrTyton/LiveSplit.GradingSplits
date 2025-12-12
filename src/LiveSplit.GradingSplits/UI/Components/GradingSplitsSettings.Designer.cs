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
            this.chkShowGraph = new System.Windows.Forms.CheckBox();
            this.lblGraphHeight = new System.Windows.Forms.Label();
            this.numGraphHeight = new System.Windows.Forms.NumericUpDown();
            this.lblGraphStyle = new System.Windows.Forms.Label();
            this.cboGraphStyle = new System.Windows.Forms.ComboBox();
            this.chkShowStatistics = new System.Windows.Forms.CheckBox();
            this.lblStatsFontSize = new System.Windows.Forms.Label();
            this.numStatsFontSize = new System.Windows.Forms.NumericUpDown();
            this.chkShowPreviousSplit = new System.Windows.Forms.CheckBox();
            this.lblPrevFontSize = new System.Windows.Forms.Label();
            this.numPrevFontSize = new System.Windows.Forms.NumericUpDown();
            this.chkShowCurrentGrade = new System.Windows.Forms.CheckBox();
            this.lblCurrentGradeFontSize = new System.Windows.Forms.Label();
            this.numCurrentGradeFontSize = new System.Windows.Forms.NumericUpDown();
            this.chkShowSplitNameGrades = new System.Windows.Forms.CheckBox();
            this.txtSplitNameFormat = new System.Windows.Forms.TextBox();
            this.chkShowGradeIcons = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.btnResetDefaults = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThresholds)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGraphHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStatsFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrevFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrentGradeFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnResetDefaults, 0, 2);
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 330F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 680);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 319);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grade Thresholds (grades are assigned to the first threshold the percentile is l" +
    "ess than or equal to)";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.dgvThresholds, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblExplanation, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(450, 300);
            this.tableLayoutPanel3.TabIndex = 0;
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
            this.dgvThresholds.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvThresholds.MultiSelect = false;
            this.dgvThresholds.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvThresholds.RowTemplate.Height = 24;
            this.dgvThresholds.Size = new System.Drawing.Size(444, 127);
            this.dgvThresholds.TabIndex = 0;
            // 
            // colZScore
            // 
            this.colZScore.HeaderText = "Percentile (0-100)";
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 136);
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
            this.lblExplanation.Location = new System.Drawing.Point(3, 168);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblExplanation.Size = new System.Drawing.Size(444, 132);
            this.lblExplanation.TabIndex = 2;
            this.lblExplanation.Text = "Percentile shows where your time ranks (0=fastest, 100=slowest). Lower percentil" +
                "es are better.\r\nClick a color cell to change the color. Thresholds are evaluated in" +
                " order from top to bottom.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 333);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 310);
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
            this.tableLayoutPanel2.Controls.Add(this.chkShowGraph, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblGraphHeight, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.numGraphHeight, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblGraphStyle, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.cboGraphStyle, 1, 4);
            this.tableLayoutPanel2.SetColumnSpan(this.cboGraphStyle, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkShowStatistics, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.lblStatsFontSize, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.numStatsFontSize, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.chkShowPreviousSplit, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.lblPrevFontSize, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.numPrevFontSize, 2, 6);
            this.tableLayoutPanel2.Controls.Add(this.chkShowCurrentGrade, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.lblCurrentGradeFontSize, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.numCurrentGradeFontSize, 2, 7);
            this.tableLayoutPanel2.Controls.Add(this.chkShowSplitNameGrades, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.txtSplitNameFormat, 1, 8);
            this.tableLayoutPanel2.SetColumnSpan(this.txtSplitNameFormat, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkShowGradeIcons, 0, 9);
            this.tableLayoutPanel2.SetColumnSpan(this.chkShowGradeIcons, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 10;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.Size = new System.Drawing.Size(450, 300);
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
            // chkShowGraph
            // 
            this.chkShowGraph.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowGraph.AutoSize = true;
            this.chkShowGraph.Location = new System.Drawing.Point(3, 96);
            this.chkShowGraph.Name = "chkShowGraph";
            this.chkShowGraph.Size = new System.Drawing.Size(129, 17);
            this.chkShowGraph.TabIndex = 8;
            this.chkShowGraph.Text = "Show Distribution Graph";
            this.chkShowGraph.UseVisualStyleBackColor = true;
            // 
            // lblGraphHeight
            // 
            this.lblGraphHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGraphHeight.AutoSize = true;
            this.lblGraphHeight.Location = new System.Drawing.Point(153, 98);
            this.lblGraphHeight.Name = "lblGraphHeight";
            this.lblGraphHeight.Size = new System.Drawing.Size(75, 13);
            this.lblGraphHeight.TabIndex = 9;
            this.lblGraphHeight.Text = "Graph Height:";
            // 
            // numGraphHeight
            // 
            this.numGraphHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numGraphHeight.Location = new System.Drawing.Point(373, 95);
            this.numGraphHeight.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            this.numGraphHeight.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            this.numGraphHeight.Name = "numGraphHeight";
            this.numGraphHeight.Size = new System.Drawing.Size(74, 20);
            this.numGraphHeight.TabIndex = 10;
            this.numGraphHeight.Value = new decimal(new int[] { 80, 0, 0, 0 });
            // 
            // lblGraphStyle
            // 
            this.lblGraphStyle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGraphStyle.AutoSize = true;
            this.lblGraphStyle.Location = new System.Drawing.Point(3, 128);
            this.lblGraphStyle.Name = "lblGraphStyle";
            this.lblGraphStyle.Size = new System.Drawing.Size(65, 13);
            this.lblGraphStyle.TabIndex = 11;
            this.lblGraphStyle.Text = "Graph Style:";
            // 
            // cboGraphStyle
            // 
            this.cboGraphStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboGraphStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGraphStyle.FormattingEnabled = true;
            this.cboGraphStyle.Items.AddRange(new object[] { "Histogram (stacked dots)", "Scatter (line)" });
            this.cboGraphStyle.Location = new System.Drawing.Point(153, 124);
            this.cboGraphStyle.Name = "cboGraphStyle";
            this.cboGraphStyle.Size = new System.Drawing.Size(294, 21);
            this.cboGraphStyle.TabIndex = 12;
            // 
            // chkShowStatistics
            // 
            this.chkShowStatistics.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowStatistics.AutoSize = true;
            this.chkShowStatistics.Location = new System.Drawing.Point(3, 156);
            this.chkShowStatistics.Name = "chkShowStatistics";
            this.chkShowStatistics.Size = new System.Drawing.Size(98, 17);
            this.chkShowStatistics.TabIndex = 13;
            this.chkShowStatistics.Text = "Show Statistics";
            this.chkShowStatistics.UseVisualStyleBackColor = true;
            // 
            // lblStatsFontSize
            // 
            this.lblStatsFontSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatsFontSize.AutoSize = true;
            this.lblStatsFontSize.Location = new System.Drawing.Point(153, 158);
            this.lblStatsFontSize.Name = "lblStatsFontSize";
            this.lblStatsFontSize.Size = new System.Drawing.Size(55, 13);
            this.lblStatsFontSize.TabIndex = 14;
            this.lblStatsFontSize.Text = "Font Size:";
            // 
            // numStatsFontSize
            // 
            this.numStatsFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numStatsFontSize.Location = new System.Drawing.Point(373, 155);
            this.numStatsFontSize.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            this.numStatsFontSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            this.numStatsFontSize.Name = "numStatsFontSize";
            this.numStatsFontSize.Size = new System.Drawing.Size(74, 20);
            this.numStatsFontSize.TabIndex = 15;
            this.numStatsFontSize.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // chkShowPreviousSplit
            // 
            this.chkShowPreviousSplit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowPreviousSplit.AutoSize = true;
            this.chkShowPreviousSplit.Location = new System.Drawing.Point(3, 186);
            this.chkShowPreviousSplit.Name = "chkShowPreviousSplit";
            this.chkShowPreviousSplit.Size = new System.Drawing.Size(132, 17);
            this.chkShowPreviousSplit.TabIndex = 16;
            this.chkShowPreviousSplit.Text = "Show Previous Split";
            this.chkShowPreviousSplit.UseVisualStyleBackColor = true;
            // 
            // lblPrevFontSize
            // 
            this.lblPrevFontSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPrevFontSize.AutoSize = true;
            this.lblPrevFontSize.Location = new System.Drawing.Point(153, 188);
            this.lblPrevFontSize.Name = "lblPrevFontSize";
            this.lblPrevFontSize.Size = new System.Drawing.Size(55, 13);
            this.lblPrevFontSize.TabIndex = 17;
            this.lblPrevFontSize.Text = "Font Size:";
            // 
            // numPrevFontSize
            // 
            this.numPrevFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numPrevFontSize.Location = new System.Drawing.Point(373, 185);
            this.numPrevFontSize.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            this.numPrevFontSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            this.numPrevFontSize.Name = "numPrevFontSize";
            this.numPrevFontSize.Size = new System.Drawing.Size(74, 20);
            this.numPrevFontSize.TabIndex = 18;
            this.numPrevFontSize.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // chkShowCurrentGrade
            // 
            this.chkShowCurrentGrade.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowCurrentGrade.AutoSize = true;
            this.chkShowCurrentGrade.Location = new System.Drawing.Point(3, 216);
            this.chkShowCurrentGrade.Name = "chkShowCurrentGrade";
            this.chkShowCurrentGrade.Size = new System.Drawing.Size(124, 17);
            this.chkShowCurrentGrade.TabIndex = 19;
            this.chkShowCurrentGrade.Text = "Show Current Grade";
            this.chkShowCurrentGrade.UseVisualStyleBackColor = true;
            // 
            // lblCurrentGradeFontSize
            // 
            this.lblCurrentGradeFontSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentGradeFontSize.AutoSize = true;
            this.lblCurrentGradeFontSize.Location = new System.Drawing.Point(153, 218);
            this.lblCurrentGradeFontSize.Name = "lblCurrentGradeFontSize";
            this.lblCurrentGradeFontSize.Size = new System.Drawing.Size(55, 13);
            this.lblCurrentGradeFontSize.TabIndex = 22;
            this.lblCurrentGradeFontSize.Text = "Font Size:";
            // 
            // numCurrentGradeFontSize
            // 
            this.numCurrentGradeFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numCurrentGradeFontSize.Location = new System.Drawing.Point(373, 215);
            this.numCurrentGradeFontSize.Maximum = new decimal(new int[] { 48, 0, 0, 0 });
            this.numCurrentGradeFontSize.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            this.numCurrentGradeFontSize.Name = "numCurrentGradeFontSize";
            this.numCurrentGradeFontSize.Size = new System.Drawing.Size(74, 20);
            this.numCurrentGradeFontSize.TabIndex = 23;
            this.numCurrentGradeFontSize.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // chkShowSplitNameGrades
            // 
            this.chkShowSplitNameGrades.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowSplitNameGrades.AutoSize = true;
            this.chkShowSplitNameGrades.Location = new System.Drawing.Point(3, 246);
            this.chkShowSplitNameGrades.Name = "chkShowSplitNameGrades";
            this.chkShowSplitNameGrades.Size = new System.Drawing.Size(154, 17);
            this.chkShowSplitNameGrades.TabIndex = 20;
            this.chkShowSplitNameGrades.Text = "Split Name Grades:";
            this.chkShowSplitNameGrades.UseVisualStyleBackColor = true;
            // 
            // txtSplitNameFormat
            // 
            this.txtSplitNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSplitNameFormat.Location = new System.Drawing.Point(153, 245);
            this.txtSplitNameFormat.Name = "txtSplitNameFormat";
            this.txtSplitNameFormat.Size = new System.Drawing.Size(294, 20);
            this.txtSplitNameFormat.TabIndex = 21;
            this.toolTip1.SetToolTip(this.txtSplitNameFormat, "Format string for split names.\nUse {Name} for the original split name.\nUse {Grade} for the grade.\n\nExamples:\n  {Name} [{Grade}]\n  [{Grade}] {Name}\n  {Name} ({Grade})");
            // 
            // chkShowGradeIcons
            // 
            this.chkShowGradeIcons.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowGradeIcons.AutoSize = true;
            this.chkShowGradeIcons.Location = new System.Drawing.Point(3, 276);
            this.chkShowGradeIcons.Name = "chkShowGradeIcons";
            this.chkShowGradeIcons.Size = new System.Drawing.Size(130, 17);
            this.chkShowGradeIcons.TabIndex = 24;
            this.chkShowGradeIcons.Text = "Show Grade Icons";
            this.chkShowGradeIcons.UseVisualStyleBackColor = true;
            this.toolTip1.SetToolTip(this.chkShowGradeIcons, "Display grade icons next to each split.\nIcons are generated based on grade colors.");
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
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GradingSplitsSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 550);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThresholds)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGraphHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStatsFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrevFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrentGradeFontSize)).EndInit();
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
        private System.Windows.Forms.CheckBox chkShowGraph;
        private System.Windows.Forms.Label lblGraphHeight;
        private System.Windows.Forms.NumericUpDown numGraphHeight;
        private System.Windows.Forms.Label lblGraphStyle;
        private System.Windows.Forms.ComboBox cboGraphStyle;
        private System.Windows.Forms.CheckBox chkShowStatistics;
        private System.Windows.Forms.Label lblStatsFontSize;
        private System.Windows.Forms.NumericUpDown numStatsFontSize;
        private System.Windows.Forms.CheckBox chkShowPreviousSplit;
        private System.Windows.Forms.Label lblPrevFontSize;
        private System.Windows.Forms.NumericUpDown numPrevFontSize;
        private System.Windows.Forms.CheckBox chkShowCurrentGrade;
        private System.Windows.Forms.Label lblCurrentGradeFontSize;
        private System.Windows.Forms.NumericUpDown numCurrentGradeFontSize;
        private System.Windows.Forms.CheckBox chkShowSplitNameGrades;
        private System.Windows.Forms.TextBox txtSplitNameFormat;
        private System.Windows.Forms.CheckBox chkShowGradeIcons;
        private System.Windows.Forms.ToolTip toolTip1;
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
