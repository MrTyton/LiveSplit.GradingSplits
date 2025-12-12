using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.GradingSplits.Model;
using LiveSplit.UI;

namespace LiveSplit.GradingSplits.UI.Components
{
    public partial class GradingSplitsSettings : UserControl
    {
        public GradingSettings GradingConfig { get; set; }

        public GradingSplitsSettings()
        {
            InitializeComponent();
            GradingConfig = new GradingSettings();

            // Wire up event handlers for threshold grid
            dgvThresholds.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < dgvThresholds.Rows.Count)
                {
                    var row = dgvThresholds.Rows[e.RowIndex];
                    var threshold = row.Tag as GradeThreshold;
                    if (threshold != null)
                    {
                        if (e.ColumnIndex == 0) // Percentile
                        {
                            var cellValue = row.Cells[0].Value?.ToString()?.Trim();
                            if (double.TryParse(cellValue, out double percentile))
                            {
                                // Clamp to valid percentile range
                                percentile = Math.Max(0, Math.Min(100, percentile));
                                threshold.PercentileThreshold = percentile;
                                row.Cells[0].Value = percentile.ToString("F1");
                            }
                            else
                            {
                                // Invalid input, revert to previous value
                                row.Cells[0].Value = threshold.PercentileThreshold.ToString("F1");
                            }
                        }
                        else if (e.ColumnIndex == 1) // Label
                        {
                            threshold.Label = row.Cells[1].Value?.ToString() ?? "?";
                        }
                    }
                }
            };

            dgvThresholds.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 2) // Color column
                {
                    var row = dgvThresholds.Rows[e.RowIndex];
                    var threshold = row.Tag as GradeThreshold;
                    if (threshold != null)
                    {
                        var colorDialog = new ColorDialog();
                        colorDialog.Color = threshold.ForegroundColor;
                        if (colorDialog.ShowDialog() == DialogResult.OK)
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
                var newThreshold = new GradeThreshold(50.0, "X", Color.White);
                GradingConfig.Thresholds.Add(newThreshold);
                int rowIndex = dgvThresholds.Rows.Add();
                var row = dgvThresholds.Rows[rowIndex];
                row.Cells[0].Value = "50.0";
                row.Cells[1].Value = "X";
                row.Cells[2].Value = Color.White.Name;
                row.Cells[2].Style.BackColor = Color.White;
                row.Tag = newThreshold;
            };

            btnRemoveThreshold.Click += (s, e) =>
            {
                if (dgvThresholds.SelectedRows.Count > 0 && GradingConfig.Thresholds.Count > 1)
                {
                    var selectedIndex = dgvThresholds.SelectedRows[0].Index;
                    GradingConfig.Thresholds.RemoveAt(selectedIndex);
                    dgvThresholds.Rows.RemoveAt(selectedIndex);
                }
                else if (GradingConfig.Thresholds.Count <= 1)
                {
                    MessageBox.Show("You must have at least one threshold.", "Cannot Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            chkUseBackground.CheckedChanged += (s, e) =>
            {
                btnBackgroundColor.Enabled = chkUseBackground.Checked;
                GradingConfig.UseBackgroundColor = chkUseBackground.Checked;
            };

            btnBackgroundColor.Click += (s, e) =>
            {
                var colorDialog = new ColorDialog();
                colorDialog.Color = GradingConfig.BackgroundColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    GradingConfig.BackgroundColor = colorDialog.Color;
                    btnBackgroundColor.BackColor = colorDialog.Color;
                }
            };

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
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    GradingConfig.GoldColor = colorDialog.Color;
                    btnGoldColor.BackColor = colorDialog.Color;
                }
            };

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
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    GradingConfig.WorstColor = colorDialog.Color;
                    btnWorstColor.BackColor = colorDialog.Color;
                }
            };

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

            chkShowStatistics.CheckedChanged += (s, e) =>
            {
                numStatsFontSize.Enabled = chkShowStatistics.Checked;
                GradingConfig.ShowStatistics = chkShowStatistics.Checked;
            };

            numStatsFontSize.ValueChanged += (s, e) =>
            {
                GradingConfig.StatisticsFontSize = (int)numStatsFontSize.Value;
            };

            chkShowPreviousSplit.CheckedChanged += (s, e) =>
            {
                numPrevFontSize.Enabled = chkShowPreviousSplit.Checked;
                GradingConfig.ShowPreviousSplit = chkShowPreviousSplit.Checked;
            };

            numPrevFontSize.ValueChanged += (s, e) =>
            {
                GradingConfig.PreviousSplitFontSize = (int)numPrevFontSize.Value;
            };

            chkShowCurrentGrade.CheckedChanged += (s, e) =>
            {
                GradingConfig.ShowCurrentGrade = chkShowCurrentGrade.Checked;
                numCurrentGradeFontSize.Enabled = chkShowCurrentGrade.Checked;
                lblCurrentGradeFontSize.Enabled = chkShowCurrentGrade.Checked;
            };

            numCurrentGradeFontSize.ValueChanged += (s, e) =>
            {
                GradingConfig.CurrentGradeFontSize = (int)numCurrentGradeFontSize.Value;
            };

            chkShowSplitNameGrades.CheckedChanged += (s, e) =>
            {
                GradingConfig.ShowGradeInSplitNames = chkShowSplitNameGrades.Checked;
                txtSplitNameFormat.Enabled = chkShowSplitNameGrades.Checked;
            };

            txtSplitNameFormat.TextChanged += (s, e) =>
            {
                GradingConfig.SplitNameFormat = txtSplitNameFormat.Text;
            };

            chkShowGradeIcons.CheckedChanged += (s, e) =>
            {
                GradingConfig.ShowGradeIcons = chkShowGradeIcons.Checked;
            };

            btnResetDefaults.Click += (s, e) =>
            {
                if (MessageBox.Show("Reset all grade settings to defaults?", "Confirm Reset",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ResetToDefaults();
                }
            };

            LoadSettingsToUI();
        }

        private void ResetToDefaults()
        {
            GradingConfig = new GradingSettings();
            LoadSettingsToUI();
        }

        private void LoadSettingsToUI()
        {
            chkUseBackground.Checked = GradingConfig.UseBackgroundColor;
            btnBackgroundColor.BackColor = GradingConfig.BackgroundColor;
            btnBackgroundColor.Enabled = GradingConfig.UseBackgroundColor;

            chkUseGold.Checked = GradingConfig.UseGoldGrade;
            txtGoldLabel.Text = GradingConfig.GoldLabel;
            txtGoldLabel.Enabled = GradingConfig.UseGoldGrade;
            btnGoldColor.BackColor = GradingConfig.GoldColor;
            btnGoldColor.Enabled = GradingConfig.UseGoldGrade;

            chkUseWorst.Checked = GradingConfig.UseWorstGrade;
            txtWorstLabel.Text = GradingConfig.WorstLabel;
            txtWorstLabel.Enabled = GradingConfig.UseWorstGrade;
            btnWorstColor.BackColor = GradingConfig.WorstColor;
            btnWorstColor.Enabled = GradingConfig.UseWorstGrade;

            chkShowGraph.Checked = GradingConfig.ShowGraph;
            numGraphHeight.Value = GradingConfig.GraphHeight;
            numGraphHeight.Enabled = GradingConfig.ShowGraph;
            cboGraphStyle.SelectedIndex = GradingConfig.UseHistogramGraph ? 0 : 1;
            cboGraphStyle.Enabled = GradingConfig.ShowGraph;
            chkShowStatistics.Checked = GradingConfig.ShowStatistics;
            chkShowStatistics.Enabled = GradingConfig.ShowGraph;
            numStatsFontSize.Value = GradingConfig.StatisticsFontSize;
            numStatsFontSize.Enabled = GradingConfig.ShowGraph && GradingConfig.ShowStatistics;

            chkShowPreviousSplit.Checked = GradingConfig.ShowPreviousSplit;
            numPrevFontSize.Value = GradingConfig.PreviousSplitFontSize;
            numPrevFontSize.Enabled = GradingConfig.ShowPreviousSplit;

            chkShowCurrentGrade.Checked = GradingConfig.ShowCurrentGrade;
            numCurrentGradeFontSize.Value = GradingConfig.CurrentGradeFontSize;
            numCurrentGradeFontSize.Enabled = GradingConfig.ShowCurrentGrade;
            lblCurrentGradeFontSize.Enabled = GradingConfig.ShowCurrentGrade;

            chkShowSplitNameGrades.Checked = GradingConfig.ShowGradeInSplitNames;
            txtSplitNameFormat.Text = GradingConfig.SplitNameFormat;
            txtSplitNameFormat.Enabled = GradingConfig.ShowGradeInSplitNames;

            chkShowGradeIcons.Checked = GradingConfig.ShowGradeIcons;

            // Populate thresholds grid
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

        private void GradingSplitsSettings_Load(object sender, EventArgs e)
        {
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;

            GradingConfig.UseBackgroundColor = SettingsHelper.ParseBool(element["UseBackgroundColor"], false);
            GradingConfig.BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.Black);
            GradingConfig.UseGoldGrade = SettingsHelper.ParseBool(element["UseGoldGrade"], true);
            GradingConfig.GoldLabel = SettingsHelper.ParseString(element["GoldLabel"], "★");
            GradingConfig.GoldColor = SettingsHelper.ParseColor(element["GoldColor"], Color.Gold);
            GradingConfig.UseWorstGrade = SettingsHelper.ParseBool(element["UseWorstGrade"], true);
            GradingConfig.WorstLabel = SettingsHelper.ParseString(element["WorstLabel"], "\u2717");
            GradingConfig.WorstColor = SettingsHelper.ParseColor(element["WorstColor"], Color.DarkRed);
            GradingConfig.ShowGraph = SettingsHelper.ParseBool(element["ShowGraph"], false);
            GradingConfig.GraphHeight = SettingsHelper.ParseInt(element["GraphHeight"], 80);
            GradingConfig.UseHistogramGraph = SettingsHelper.ParseBool(element["UseHistogramGraph"], true);
            GradingConfig.ShowStatistics = SettingsHelper.ParseBool(element["ShowStatistics"], true);
            GradingConfig.StatisticsFontSize = SettingsHelper.ParseInt(element["StatisticsFontSize"], 10);
            GradingConfig.ShowPreviousSplit = SettingsHelper.ParseBool(element["ShowPreviousSplit"], false);
            GradingConfig.PreviousSplitFontSize = SettingsHelper.ParseInt(element["PreviousSplitFontSize"], 10);
            GradingConfig.ShowCurrentGrade = SettingsHelper.ParseBool(element["ShowCurrentGrade"], true);
            GradingConfig.CurrentGradeFontSize = SettingsHelper.ParseInt(element["CurrentGradeFontSize"], 15);
            GradingConfig.ShowGradeInSplitNames = SettingsHelper.ParseBool(element["ShowGradeInSplitNames"], false);
            GradingConfig.SplitNameFormat = SettingsHelper.ParseString(element["SplitNameFormat"], "{Name} [{Grade}]");
            GradingConfig.ShowGradeIcons = SettingsHelper.ParseBool(element["ShowGradeIcons"], false);

            // Parse thresholds
            var thresholdsNode = element["Thresholds"];
            if (thresholdsNode != null && thresholdsNode.HasChildNodes)
            {
                GradingConfig.Thresholds.Clear();
                foreach (XmlElement thresholdNode in thresholdsNode.ChildNodes)
                {
                    double percentile = 50.0;

                    // Try to load as percentile first (new format)
                    var percentileStr = thresholdNode["Percentile"]?.InnerText;
                    if (!string.IsNullOrEmpty(percentileStr) && double.TryParse(percentileStr, out double parsedPercentile))
                    {
                        percentile = Math.Max(0, Math.Min(100, parsedPercentile));
                    }
                    else
                    {
                        // Fallback: try to load as z-score (old format) and convert
                        var zScoreStr = thresholdNode["ZScore"]?.InnerText;
                        if (!string.IsNullOrEmpty(zScoreStr))
                        {
                            double zScore = 0;
                            if (zScoreStr == "∞" || zScoreStr.ToLower() == "infinity" || zScoreStr.ToLower() == "inf")
                            {
                                zScore = 3.0; // Cap at z=3 for conversion
                            }
                            else if (double.TryParse(zScoreStr, out double parsedZScore))
                            {
                                zScore = Math.Max(-3, Math.Min(3, parsedZScore)); // Clamp for conversion
                            }
                            percentile = Statistics.ZScoreToPercentile(zScore);
                        }
                    }

                    var label = SettingsHelper.ParseString(thresholdNode["Label"], "?");
                    var color = SettingsHelper.ParseColor(thresholdNode["Color"], Color.White);

                    GradingConfig.Thresholds.Add(new GradeThreshold(percentile, label, color));
                }
            }

            LoadSettingsToUI();
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            int hash = SettingsHelper.CreateSetting(document, parent, "UseBackgroundColor", GradingConfig.UseBackgroundColor);
            hash ^= SettingsHelper.CreateSetting(document, parent, "BackgroundColor", GradingConfig.BackgroundColor);
            hash ^= SettingsHelper.CreateSetting(document, parent, "UseGoldGrade", GradingConfig.UseGoldGrade);
            hash ^= SettingsHelper.CreateSetting(document, parent, "GoldLabel", GradingConfig.GoldLabel);
            hash ^= SettingsHelper.CreateSetting(document, parent, "GoldColor", GradingConfig.GoldColor);
            hash ^= SettingsHelper.CreateSetting(document, parent, "UseWorstGrade", GradingConfig.UseWorstGrade);
            hash ^= SettingsHelper.CreateSetting(document, parent, "WorstLabel", GradingConfig.WorstLabel);
            hash ^= SettingsHelper.CreateSetting(document, parent, "WorstColor", GradingConfig.WorstColor);
            hash ^= SettingsHelper.CreateSetting(document, parent, "ShowGraph", GradingConfig.ShowGraph);
            hash ^= SettingsHelper.CreateSetting(document, parent, "GraphHeight", GradingConfig.GraphHeight);
            hash ^= SettingsHelper.CreateSetting(document, parent, "UseHistogramGraph", GradingConfig.UseHistogramGraph);
            hash ^= SettingsHelper.CreateSetting(document, parent, "ShowStatistics", GradingConfig.ShowStatistics);
            hash ^= SettingsHelper.CreateSetting(document, parent, "StatisticsFontSize", GradingConfig.StatisticsFontSize);
            hash ^= SettingsHelper.CreateSetting(document, parent, "ShowPreviousSplit", GradingConfig.ShowPreviousSplit);
            hash ^= SettingsHelper.CreateSetting(document, parent, "PreviousSplitFontSize", GradingConfig.PreviousSplitFontSize);
            hash ^= SettingsHelper.CreateSetting(document, parent, "ShowCurrentGrade", GradingConfig.ShowCurrentGrade);
            hash ^= SettingsHelper.CreateSetting(document, parent, "CurrentGradeFontSize", GradingConfig.CurrentGradeFontSize);
            hash ^= SettingsHelper.CreateSetting(document, parent, "ShowGradeInSplitNames", GradingConfig.ShowGradeInSplitNames);
            hash ^= SettingsHelper.CreateSetting(document, parent, "SplitNameFormat", GradingConfig.SplitNameFormat);
            hash ^= SettingsHelper.CreateSetting(document, parent, "ShowGradeIcons", GradingConfig.ShowGradeIcons);

            // Save thresholds
            if (document != null)
            {
                var thresholdsNode = document.CreateElement("Thresholds");
                foreach (var threshold in GradingConfig.Thresholds)
                {
                    var thresholdNode = document.CreateElement("Threshold");
                    SettingsHelper.CreateSetting(document, thresholdNode, "Percentile", threshold.PercentileThreshold);
                    SettingsHelper.CreateSetting(document, thresholdNode, "Label", threshold.Label);
                    SettingsHelper.CreateSetting(document, thresholdNode, "Color", threshold.ForegroundColor);
                    thresholdsNode.AppendChild(thresholdNode);
                }
                parent.AppendChild(thresholdsNode);
            }

            // Hash thresholds
            foreach (var threshold in GradingConfig.Thresholds)
            {
                hash ^= threshold.PercentileThreshold.GetHashCode();
                hash ^= threshold.Label.GetHashCode();
                hash ^= threshold.ForegroundColor.GetHashCode();
            }

            return hash;
        }
    }
}
