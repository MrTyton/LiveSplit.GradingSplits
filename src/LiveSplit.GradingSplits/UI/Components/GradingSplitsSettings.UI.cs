using LiveSplit.GradingSplits.Model;
using System;
using System.Windows.Forms;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Settings control for the Grading Splits component.
    /// This partial class contains UI synchronization methods.
    /// </summary>
    public partial class GradingSplitsSettings
    {
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
