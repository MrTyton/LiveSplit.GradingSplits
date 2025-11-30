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
                        if (e.ColumnIndex == 0) // Z-Score
                        {
                            var cellValue = row.Cells[0].Value?.ToString()?.Trim();
                            if (cellValue == "∞" || cellValue == "inf" || cellValue == "infinity")
                            {
                                threshold.ZScoreThreshold = double.MaxValue;
                            }
                            else if (double.TryParse(cellValue, out double zScore))
                            {
                                threshold.ZScoreThreshold = zScore;
                            }
                            else
                            {
                                // Invalid input, revert to previous value
                                row.Cells[0].Value = double.IsInfinity(threshold.ZScoreThreshold) || threshold.ZScoreThreshold >= double.MaxValue ? "∞" : threshold.ZScoreThreshold.ToString("F2");
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
                var newThreshold = new GradeThreshold(0.0, "X", Color.White);
                GradingConfig.Thresholds.Add(newThreshold);
                int rowIndex = dgvThresholds.Rows.Add();
                var row = dgvThresholds.Rows[rowIndex];
                row.Cells[0].Value = "0.00";
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

            // Populate thresholds grid
            dgvThresholds.Rows.Clear();
            foreach (var threshold in GradingConfig.Thresholds)
            {
                int rowIndex = dgvThresholds.Rows.Add();
                var row = dgvThresholds.Rows[rowIndex];
                row.Cells[0].Value = double.IsInfinity(threshold.ZScoreThreshold) || threshold.ZScoreThreshold >= double.MaxValue ? "∞" : threshold.ZScoreThreshold.ToString("F2");
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
            GradingConfig.WorstLabel = SettingsHelper.ParseString(element["WorstLabel"], "✗");
            GradingConfig.WorstColor = SettingsHelper.ParseColor(element["WorstColor"], Color.DarkRed);

            // Parse thresholds
            var thresholdsNode = element["Thresholds"];
            if (thresholdsNode != null && thresholdsNode.HasChildNodes)
            {
                GradingConfig.Thresholds.Clear();
                foreach (XmlElement thresholdNode in thresholdsNode.ChildNodes)
                {
                    var zScoreStr = thresholdNode["ZScore"]?.InnerText;
                    double zScore = 0;
                    if (!string.IsNullOrEmpty(zScoreStr))
                    {
                        // Handle special infinity values
                        if (zScoreStr == "∞" || zScoreStr.ToLower() == "infinity" || zScoreStr.ToLower() == "inf")
                        {
                            zScore = double.MaxValue;
                        }
                        else if (double.TryParse(zScoreStr, out double parsedValue))
                        {
                            // Clamp extremely large values to MaxValue
                            zScore = parsedValue >= double.MaxValue ? double.MaxValue : parsedValue;
                        }
                    }
                    var label = SettingsHelper.ParseString(thresholdNode["Label"], "?");
                    var color = SettingsHelper.ParseColor(thresholdNode["Color"], Color.White);
                    
                    GradingConfig.Thresholds.Add(new GradeThreshold(zScore, label, color));
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

            // Save thresholds
            if (document != null)
            {
                var thresholdsNode = document.CreateElement("Thresholds");
                foreach (var threshold in GradingConfig.Thresholds)
                {
                    var thresholdNode = document.CreateElement("Threshold");
                    SettingsHelper.CreateSetting(document, thresholdNode, "ZScore", threshold.ZScoreThreshold);
                    SettingsHelper.CreateSetting(document, thresholdNode, "Label", threshold.Label);
                    SettingsHelper.CreateSetting(document, thresholdNode, "Color", threshold.ForegroundColor);
                    thresholdsNode.AppendChild(thresholdNode);
                }
                parent.AppendChild(thresholdsNode);
            }
            
            // Hash thresholds
            foreach (var threshold in GradingConfig.Thresholds)
            {
                hash ^= threshold.ZScoreThreshold.GetHashCode();
                hash ^= threshold.Label.GetHashCode();
                hash ^= threshold.ForegroundColor.GetHashCode();
            }

            return hash;
        }
    }
}
