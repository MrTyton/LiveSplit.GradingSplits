using LiveSplit.GradingSplits.Model;
using LiveSplit.UI;
using System;
using System.Drawing;
using System.Xml;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Settings control for the Grading Splits component.
    /// This partial class contains XML serialization methods.
    /// </summary>
    public partial class GradingSplitsSettings
    {
        /// <summary>
        /// Deserializes settings from an XML node.
        /// </summary>
        /// <param name="node">The XML node containing the settings.</param>
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
            var displayStyleStr = SettingsHelper.ParseString(element["CurrentGradeDisplayStyle"], "Text");
            GradingConfig.CurrentGradeDisplayStyle = displayStyleStr == "Icon" ? GradeDisplayStyle.Icon : GradeDisplayStyle.Text;
            GradingConfig.ShowGradeInSplitNames = SettingsHelper.ParseBool(element["ShowGradeInSplitNames"], false);
            GradingConfig.SplitNameFormat = SettingsHelper.ParseString(element["SplitNameFormat"], "{Name} [{Grade}]");
            GradingConfig.ShowGradeIcons = SettingsHelper.ParseBool(element["ShowGradeIcons"], false);
            GradingConfig.IconFolderPath = SettingsHelper.ParseString(element["IconFolderPath"], null);

            ParseThresholds(element);

            LoadSettingsToUI();
        }

        /// <summary>
        /// Parses threshold settings from XML.
        /// </summary>
        private void ParseThresholds(XmlElement element)
        {
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
                    var customIconPath = SettingsHelper.ParseString(thresholdNode["CustomIconPath"], null);

                    GradingConfig.Thresholds.Add(new GradeThreshold(percentile, label, color)
                    {
                        CustomIconPath = customIconPath
                    });
                }
            }
        }

        /// <summary>
        /// Serializes settings to an XML node.
        /// </summary>
        /// <param name="document">The XML document to create elements in.</param>
        /// <returns>An XML node containing the serialized settings.</returns>
        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        /// <summary>
        /// Gets a hash code representing the current settings state.
        /// </summary>
        /// <returns>A hash code for the current settings.</returns>
        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        /// <summary>
        /// Creates the settings XML node and/or computes the settings hash.
        /// </summary>
        /// <param name="document">The XML document (null if only computing hash).</param>
        /// <param name="parent">The parent element (null if only computing hash).</param>
        /// <returns>A hash code for the settings.</returns>
        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            int hash = CreateBasicSettingsHash(document, parent);
            hash ^= CreateThresholdsSettings(document, parent);
            return hash;
        }

        private int CreateBasicSettingsHash(XmlDocument document, XmlElement parent)
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
            hash ^= SettingsHelper.CreateSetting(document, parent, "CurrentGradeDisplayStyle", GradingConfig.CurrentGradeDisplayStyle.ToString());
            hash ^= SettingsHelper.CreateSetting(document, parent, "IconFolderPath", GradingConfig.IconFolderPath ?? "");
            return hash;
        }

        private int CreateThresholdsSettings(XmlDocument document, XmlElement parent)
        {
            int hash = 0;

            // Save thresholds to XML
            if (document != null)
            {
                var thresholdsNode = document.CreateElement("Thresholds");
                foreach (var threshold in GradingConfig.Thresholds)
                {
                    var thresholdNode = document.CreateElement("Threshold");
                    SettingsHelper.CreateSetting(document, thresholdNode, "Percentile", threshold.PercentileThreshold);
                    SettingsHelper.CreateSetting(document, thresholdNode, "Label", threshold.Label);
                    SettingsHelper.CreateSetting(document, thresholdNode, "Color", threshold.ForegroundColor);
                    if (!string.IsNullOrEmpty(threshold.CustomIconPath))
                    {
                        SettingsHelper.CreateSetting(document, thresholdNode, "CustomIconPath", threshold.CustomIconPath);
                    }
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
                if (!string.IsNullOrEmpty(threshold.CustomIconPath))
                {
                    hash ^= threshold.CustomIconPath.GetHashCode();
                }
            }

            return hash;
        }
    }
}
