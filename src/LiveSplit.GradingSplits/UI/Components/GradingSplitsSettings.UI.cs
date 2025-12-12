using LiveSplit.GradingSplits.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Settings control for the Grading Splits component.
    /// This partial class contains UI synchronization methods.
    /// </summary>
    public partial class GradingSplitsSettings
    {
        // Custom icon controls (created programmatically)
        private Label lblIconFolder;
        private TextBox txtIconFolderPath;
        private Button btnBrowseIconFolder;
        private Button btnClearIconFolder;
        private Label lblIconFolderHelp;

        /// <summary>
        /// Creates the custom icon folder UI controls programmatically.
        /// </summary>
        private void CreateIconFolderControls()
        {
            // Create label
            lblIconFolder = new Label
            {
                Text = "Custom Icon Folder:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Padding = new Padding(15, 0, 0, 0) // Indent to show it's related to grade icons
            };

            // Create textbox for path
            txtIconFolderPath = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true
            };

            // Create browse button
            btnBrowseIconFolder = new Button
            {
                Text = "...",
                Size = new Size(30, 23),
                Anchor = AnchorStyles.Left
            };
            btnBrowseIconFolder.Click += BtnBrowseIconFolder_Click;

            // Create clear button
            btnClearIconFolder = new Button
            {
                Text = "Clear",
                Size = new Size(45, 23),
                Anchor = AnchorStyles.Left
            };
            btnClearIconFolder.Click += BtnClearIconFolder_Click;

            // Create help label with explanation
            lblIconFolderHelp = new Label
            {
                Text = "Select any icon file in your folder to set the icon folder path.\n" +
                       "Icons should be named: S.png, A.png, etc. Use Best.png / Worst.png for special splits.\n" +
                       "Missing icons will use auto-generated ones. Formats: PNG, JPG, GIF, BMP, ICO",
                AutoSize = true,
                ForeColor = SystemColors.GrayText,
                Padding = new Padding(15, 0, 0, 5)
            };

            // Create a flow panel for browse and clear buttons
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            buttonPanel.Controls.Add(btnBrowseIconFolder);
            buttonPanel.Controls.Add(btnClearIconFolder);

            // Suspend layout while adding rows to prevent flickering
            tableLayoutPanel2.SuspendLayout();

            // Insert rows right after chkShowGradeIcons (row 10)
            // Row 11: folder path controls, Row 12: help label
            tableLayoutPanel2.RowCount = 13;
            
            // Add row styles for the new rows
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Add controls to the new rows
            tableLayoutPanel2.Controls.Add(lblIconFolder, 0, 11);
            tableLayoutPanel2.Controls.Add(txtIconFolderPath, 1, 11);
            tableLayoutPanel2.Controls.Add(buttonPanel, 2, 11);
            tableLayoutPanel2.Controls.Add(lblIconFolderHelp, 0, 12);
            tableLayoutPanel2.SetColumnSpan(lblIconFolderHelp, 3);

            tableLayoutPanel2.ResumeLayout(true);
        }

        private void BtnBrowseIconFolder_Click(object sender, EventArgs e)
        {
            // Use OpenFileDialog in a way that allows folder selection
            // by having user select any file in the desired folder
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select any file in the custom icons folder";
                fileDialog.Filter = "Image files (*.png;*.jpg;*.gif;*.bmp;*.ico)|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.ico|All files (*.*)|*.*";
                fileDialog.CheckFileExists = true;
                
                if (!string.IsNullOrEmpty(GradingConfig.IconFolderPath) && 
                    System.IO.Directory.Exists(GradingConfig.IconFolderPath))
                {
                    fileDialog.InitialDirectory = GradingConfig.IconFolderPath;
                }

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the folder from the selected file
                    string folderPath = System.IO.Path.GetDirectoryName(fileDialog.FileName);
                    GradingConfig.IconFolderPath = folderPath;
                    txtIconFolderPath.Text = folderPath;
                    CustomIconLoader.ClearCache();
                    UpdateIconFolderControlsState();
                }
            }
        }

        private void BtnClearIconFolder_Click(object sender, EventArgs e)
        {
            GradingConfig.IconFolderPath = null;
            txtIconFolderPath.Text = "";
            CustomIconLoader.ClearCache();
            UpdateIconFolderControlsState();
        }

        private void UpdateIconFolderControlsState()
        {
            bool enabled = chkShowGradeIcons.Checked;
            lblIconFolder.Enabled = enabled;
            txtIconFolderPath.Enabled = enabled;
            btnBrowseIconFolder.Enabled = enabled;
            btnClearIconFolder.Enabled = enabled && !string.IsNullOrEmpty(GradingConfig.IconFolderPath);
            lblIconFolderHelp.Enabled = enabled;
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        private void ResetToDefaults()
        {
            GradingConfig = new GradingSettings();
            LoadSettingsToUI();
        }

        /// <summary>
        /// Loads the current GradingConfig values into the UI controls.
        /// </summary>
        private void LoadSettingsToUI()
        {
            LoadBackgroundSettings();
            LoadGoldGradeSettings();
            LoadWorstGradeSettings();
            LoadGraphSettings();
            LoadPreviousSplitSettings();
            LoadCurrentGradeSettings();
            LoadSplitNameSettings();
            LoadThresholdsGrid();
        }

        private void LoadBackgroundSettings()
        {
            chkUseBackground.Checked = GradingConfig.UseBackgroundColor;
            btnBackgroundColor.BackColor = GradingConfig.BackgroundColor;
            btnBackgroundColor.Enabled = GradingConfig.UseBackgroundColor;
        }

        private void LoadGoldGradeSettings()
        {
            chkUseGold.Checked = GradingConfig.UseGoldGrade;
            txtGoldLabel.Text = GradingConfig.GoldLabel;
            txtGoldLabel.Enabled = GradingConfig.UseGoldGrade;
            btnGoldColor.BackColor = GradingConfig.GoldColor;
            btnGoldColor.Enabled = GradingConfig.UseGoldGrade;
        }

        private void LoadWorstGradeSettings()
        {
            chkUseWorst.Checked = GradingConfig.UseWorstGrade;
            txtWorstLabel.Text = GradingConfig.WorstLabel;
            txtWorstLabel.Enabled = GradingConfig.UseWorstGrade;
            btnWorstColor.BackColor = GradingConfig.WorstColor;
            btnWorstColor.Enabled = GradingConfig.UseWorstGrade;
        }

        private void LoadGraphSettings()
        {
            chkShowGraph.Checked = GradingConfig.ShowGraph;
            numGraphHeight.Value = GradingConfig.GraphHeight;
            numGraphHeight.Enabled = GradingConfig.ShowGraph;
            cboGraphStyle.SelectedIndex = GradingConfig.UseHistogramGraph ? 0 : 1;
            cboGraphStyle.Enabled = GradingConfig.ShowGraph;
            chkShowStatistics.Checked = GradingConfig.ShowStatistics;
            chkShowStatistics.Enabled = GradingConfig.ShowGraph;
            numStatsFontSize.Value = GradingConfig.StatisticsFontSize;
            numStatsFontSize.Enabled = GradingConfig.ShowGraph && GradingConfig.ShowStatistics;
        }

        private void LoadPreviousSplitSettings()
        {
            chkShowPreviousSplit.Checked = GradingConfig.ShowPreviousSplit;
            numPrevFontSize.Value = GradingConfig.PreviousSplitFontSize;
            numPrevFontSize.Enabled = GradingConfig.ShowPreviousSplit;
        }

        private void LoadCurrentGradeSettings()
        {
            chkShowCurrentGrade.Checked = GradingConfig.ShowCurrentGrade;
            cboCurrentGradeStyle.SelectedIndex = (int)GradingConfig.CurrentGradeDisplayStyle;
            cboCurrentGradeStyle.Enabled = GradingConfig.ShowCurrentGrade;
            lblCurrentGradeStyle.Enabled = GradingConfig.ShowCurrentGrade;
            numCurrentGradeFontSize.Value = GradingConfig.CurrentGradeFontSize;
            bool textStyleEnabled = GradingConfig.ShowCurrentGrade && GradingConfig.CurrentGradeDisplayStyle == GradeDisplayStyle.Text;
            numCurrentGradeFontSize.Enabled = textStyleEnabled;
            lblCurrentGradeFontSize.Enabled = textStyleEnabled;
        }

        private void LoadSplitNameSettings()
        {
            chkShowSplitNameGrades.Checked = GradingConfig.ShowGradeInSplitNames;
            txtSplitNameFormat.Text = GradingConfig.SplitNameFormat;
            txtSplitNameFormat.Enabled = GradingConfig.ShowGradeInSplitNames;

            chkShowGradeIcons.Checked = GradingConfig.ShowGradeIcons;

            // Load icon folder settings
            txtIconFolderPath.Text = GradingConfig.IconFolderPath ?? "";
            UpdateIconFolderControlsState();
        }

        private void LoadThresholdsGrid()
        {
            dgvThresholds.Rows.Clear();
            foreach (var threshold in GradingConfig.Thresholds)
            {
                int rowIndex = dgvThresholds.Rows.Add();
                var row = dgvThresholds.Rows[rowIndex];
                row.Cells[0].Value = threshold.PercentileThreshold.ToString("F1");
                row.Cells[1].Value = threshold.Label;
                row.Cells[2].Value = threshold.ForegroundColor.Name;
                row.Cells[2].Style.BackColor = threshold.ForegroundColor;
                row.Tag = threshold;
            }
        }

        /// <summary>
        /// Handles the form load event.
        /// </summary>
        private void GradingSplitsSettings_Load(object sender, EventArgs e)
        {
        }
    }
}
