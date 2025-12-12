using LiveSplit.GradingSplits.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Settings control for the Grading Splits component.
    /// This partial class contains all event handler wiring.
    /// </summary>
    public partial class GradingSplitsSettings
    {
        /// <summary>
        /// Wires up all event handlers for the settings controls.
        /// </summary>
        private void WireUpEventHandlers()
        {
            WireUpThresholdEventHandlers();
            WireUpDisplayOptionEventHandlers();
            WireUpResetHandler();
        }

        /// <summary>
        /// Wires up event handlers for the threshold data grid.
        /// </summary>
        private void WireUpThresholdEventHandlers()
        {
            dgvThresholds.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < dgvThresholds.Rows.Count)
                {
                    var row = dgvThresholds.Rows[e.RowIndex];
                    if (row.Tag is GradeThreshold threshold)
                    {
                        // Update percentile if that column changed
                        if (e.ColumnIndex == 0)
                        {
                            if (double.TryParse(row.Cells[0].Value?.ToString(), out double percentile))
                            {
                                percentile = Math.Max(0, Math.Min(100, percentile));
                                threshold.PercentileThreshold = percentile;
                                row.Cells[0].Value = percentile.ToString("F1");
                            }
                        }
                        // Update label if that column changed
                        else if (e.ColumnIndex == 1)
                        {
                            threshold.Label = row.Cells[1].Value?.ToString() ?? "?";
                        }
                    }
                }
            };

            dgvThresholds.CellClick += (s, e) =>
            {
                // Handle color cell clicks
                if (e.ColumnIndex == 2 && e.RowIndex >= 0 && e.RowIndex < dgvThresholds.Rows.Count)
                {
                    var row = dgvThresholds.Rows[e.RowIndex];
                    if (row.Tag is GradeThreshold threshold)
                    {
                        var colorDialog = new ColorDialog();
                        colorDialog.Color = threshold.ForegroundColor;
                        if (colorDialog.ShowDialog(this.FindForm()) == DialogResult.OK)
                        {
                            threshold.ForegroundColor = colorDialog.Color;
                            row.Cells[2].Value = colorDialog.Color.Name;
                            row.Cells[2].Style.BackColor = colorDialog.Color;
                        }
                    }
                }
            };

            btnAddThreshold.Click += (s, e) =>
            {
                var newThreshold = new GradeThreshold(50.0, "New", Color.White);
                GradingConfig.Thresholds.Add(newThreshold);
                int rowIndex = dgvThresholds.Rows.Add();
                var row = dgvThresholds.Rows[rowIndex];
                row.Cells[0].Value = newThreshold.PercentileThreshold.ToString("F1");
                row.Cells[1].Value = newThreshold.Label;
                row.Cells[2].Value = newThreshold.ForegroundColor.Name;
                row.Cells[2].Style.BackColor = newThreshold.ForegroundColor;
                row.Tag = newThreshold;
            };

            btnRemoveThreshold.Click += (s, e) =>
            {
                if (dgvThresholds.SelectedRows.Count > 0 && GradingConfig.Thresholds.Count > 1)
                {
                    var row = dgvThresholds.SelectedRows[0];
                    if (row.Tag is GradeThreshold threshold)
                    {
                        GradingConfig.Thresholds.Remove(threshold);
                    }
                    dgvThresholds.Rows.Remove(row);
                }
            };
        }

        /// <summary>
        /// Wires up event handlers for display option controls.
        /// </summary>
        private void WireUpDisplayOptionEventHandlers()
        {
            WireUpBackgroundColorHandlers();
            WireUpGoldGradeHandlers();
            WireUpWorstGradeHandlers();
            WireUpGraphHandlers();
            WireUpStatisticsHandlers();
            WireUpPreviousSplitHandlers();
            WireUpCurrentGradeHandlers();
            WireUpSplitNameHandlers();
            WireUpGradeIconHandlers();
        }

        private void WireUpBackgroundColorHandlers()
        {
            chkUseBackground.CheckedChanged += (s, e) =>
            {
                btnBackgroundColor.Enabled = chkUseBackground.Checked;
                GradingConfig.UseBackgroundColor = chkUseBackground.Checked;
            };

            btnBackgroundColor.Click += (s, e) =>
            {
                var colorDialog = new ColorDialog();
                colorDialog.Color = GradingConfig.BackgroundColor;
                if (colorDialog.ShowDialog(this.FindForm()) == DialogResult.OK)
                {
                    GradingConfig.BackgroundColor = colorDialog.Color;
                    btnBackgroundColor.BackColor = colorDialog.Color;
                }
            };
        }

        private void WireUpGoldGradeHandlers()
        {
            chkUseGold.CheckedChanged += (s, e) =>
            {
                txtGoldLabel.Enabled = chkUseGold.Checked;
                btnGoldColor.Enabled = chkUseGold.Checked;
                GradingConfig.UseGoldGrade = chkUseGold.Checked;
            };

            txtGoldLabel.TextChanged += (s, e) =>
            {
                GradingConfig.GoldLabel = txtGoldLabel.Text;
            };

            btnGoldColor.Click += (s, e) =>
            {
                var colorDialog = new ColorDialog();
                colorDialog.Color = GradingConfig.GoldColor;
                if (colorDialog.ShowDialog(this.FindForm()) == DialogResult.OK)
                {
                    GradingConfig.GoldColor = colorDialog.Color;
                    btnGoldColor.BackColor = colorDialog.Color;
                }
            };
        }

        private void WireUpWorstGradeHandlers()
        {
            chkUseWorst.CheckedChanged += (s, e) =>
            {
                txtWorstLabel.Enabled = chkUseWorst.Checked;
                btnWorstColor.Enabled = chkUseWorst.Checked;
                GradingConfig.UseWorstGrade = chkUseWorst.Checked;
            };

            txtWorstLabel.TextChanged += (s, e) =>
            {
                GradingConfig.WorstLabel = txtWorstLabel.Text;
            };

            btnWorstColor.Click += (s, e) =>
            {
                var colorDialog = new ColorDialog();
                colorDialog.Color = GradingConfig.WorstColor;
                if (colorDialog.ShowDialog(this.FindForm()) == DialogResult.OK)
                {
                    GradingConfig.WorstColor = colorDialog.Color;
                    btnWorstColor.BackColor = colorDialog.Color;
                }
            };
        }

        private void WireUpGraphHandlers()
        {
            chkShowGraph.CheckedChanged += (s, e) =>
            {
                numGraphHeight.Enabled = chkShowGraph.Checked;
                cboGraphStyle.Enabled = chkShowGraph.Checked;
                chkShowStatistics.Enabled = chkShowGraph.Checked;
                numStatsFontSize.Enabled = chkShowGraph.Checked && chkShowStatistics.Checked;
                GradingConfig.ShowGraph = chkShowGraph.Checked;
            };

            numGraphHeight.ValueChanged += (s, e) =>
            {
                GradingConfig.GraphHeight = (int)numGraphHeight.Value;
            };

            cboGraphStyle.SelectedIndexChanged += (s, e) =>
            {
                GradingConfig.UseHistogramGraph = cboGraphStyle.SelectedIndex == 0;
            };
        }

        private void WireUpStatisticsHandlers()
        {
            chkShowStatistics.CheckedChanged += (s, e) =>
            {
                numStatsFontSize.Enabled = chkShowStatistics.Checked;
                GradingConfig.ShowStatistics = chkShowStatistics.Checked;
            };

            numStatsFontSize.ValueChanged += (s, e) =>
            {
                GradingConfig.StatisticsFontSize = (int)numStatsFontSize.Value;
            };
        }

        private void WireUpPreviousSplitHandlers()
        {
            chkShowPreviousSplit.CheckedChanged += (s, e) =>
            {
                numPrevFontSize.Enabled = chkShowPreviousSplit.Checked;
                GradingConfig.ShowPreviousSplit = chkShowPreviousSplit.Checked;
            };

            numPrevFontSize.ValueChanged += (s, e) =>
            {
                GradingConfig.PreviousSplitFontSize = (int)numPrevFontSize.Value;
            };
        }

        private void WireUpCurrentGradeHandlers()
        {
            chkShowCurrentGrade.CheckedChanged += (s, e) =>
            {
                GradingConfig.ShowCurrentGrade = chkShowCurrentGrade.Checked;
                cboCurrentGradeStyle.Enabled = chkShowCurrentGrade.Checked;
                lblCurrentGradeStyle.Enabled = chkShowCurrentGrade.Checked;
                bool textStyleEnabled = chkShowCurrentGrade.Checked && GradingConfig.CurrentGradeDisplayStyle == GradeDisplayStyle.Text;
                numCurrentGradeFontSize.Enabled = textStyleEnabled;
                lblCurrentGradeFontSize.Enabled = textStyleEnabled;
            };

            cboCurrentGradeStyle.SelectedIndexChanged += (s, e) =>
            {
                GradingConfig.CurrentGradeDisplayStyle = (GradeDisplayStyle)cboCurrentGradeStyle.SelectedIndex;
                bool textStyleEnabled = chkShowCurrentGrade.Checked && GradingConfig.CurrentGradeDisplayStyle == GradeDisplayStyle.Text;
                numCurrentGradeFontSize.Enabled = textStyleEnabled;
                lblCurrentGradeFontSize.Enabled = textStyleEnabled;
            };

            numCurrentGradeFontSize.ValueChanged += (s, e) =>
            {
                GradingConfig.CurrentGradeFontSize = (int)numCurrentGradeFontSize.Value;
            };
        }

        private void WireUpSplitNameHandlers()
        {
            chkShowSplitNameGrades.CheckedChanged += (s, e) =>
            {
                GradingConfig.ShowGradeInSplitNames = chkShowSplitNameGrades.Checked;
                txtSplitNameFormat.Enabled = chkShowSplitNameGrades.Checked;
            };

            txtSplitNameFormat.TextChanged += (s, e) =>
            {
                GradingConfig.SplitNameFormat = txtSplitNameFormat.Text;
            };
        }

        private void WireUpGradeIconHandlers()
        {
            chkShowGradeIcons.CheckedChanged += (s, e) =>
            {
                GradingConfig.ShowGradeIcons = chkShowGradeIcons.Checked;
                UpdateIconFolderControlsState();
            };
        }

        private void WireUpResetHandler()
        {
            btnResetDefaults.Click += (s, e) =>
            {
                if (MessageBox.Show("Reset all grade settings to defaults?", "Confirm Reset",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ResetToDefaults();
                }
            };
        }
    }
}
