using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    /// <summary>
    /// Display style options for the current grade in the component.
    /// </summary>
    public enum GradeDisplayStyle
    {
        /// <summary>
        /// Display grade as text (e.g., "A", "S").
        /// </summary>
        Text,

        /// <summary>
        /// Display grade as an icon.
        /// </summary>
        Icon
    }

    /// <summary>
    /// Configuration settings for the grading system, including thresholds and display options.
    /// </summary>
    public class GradingSettings
    {
        /// <summary>
        /// List of grade thresholds ordered by percentile.
        /// </summary>
        public List<GradeThreshold> Thresholds { get; set; }

        /// <summary>
        /// Whether to show a background color behind the grade label.
        /// </summary>
        public bool UseBackgroundColor { get; set; }

        /// <summary>
        /// The background color to use when UseBackgroundColor is true.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Whether to show a special badge for the best (gold) segment.
        /// </summary>
        public bool UseGoldGrade { get; set; }

        /// <summary>
        /// The label to show for gold segments (default: "★").
        /// </summary>
        public string GoldLabel { get; set; }

        /// <summary>
        /// The color to use for gold segment labels.
        /// </summary>
        public Color GoldColor { get; set; }

        /// <summary>
        /// Whether to show a special badge for the worst segment.
        /// </summary>
        public bool UseWorstGrade { get; set; }

        /// <summary>
        /// The label to show for worst segments (default: "✗").
        /// </summary>
        public string WorstLabel { get; set; }

        /// <summary>
        /// The color to use for worst segment labels.
        /// </summary>
        public Color WorstColor { get; set; }

        /// <summary>
        /// Whether to show the distribution graph below the grade.
        /// </summary>
        public bool ShowGraph { get; set; }

        /// <summary>
        /// The height of the distribution graph in pixels.
        /// </summary>
        public int GraphHeight { get; set; }

        /// <summary>
        /// Whether to show historical points as a histogram (stacked dots) or a scatter line.
        /// </summary>
        public bool UseHistogramGraph { get; set; }

        /// <summary>
        /// Whether to show statistics text below the graph.
        /// </summary>
        public bool ShowStatistics { get; set; }

        /// <summary>
        /// The font size for the statistics text.
        /// </summary>
        public int StatisticsFontSize { get; set; }

        /// <summary>
        /// Whether to show the previous split's comparison result.
        /// </summary>
        public bool ShowPreviousSplit { get; set; }

        /// <summary>
        /// The font size for the previous split comparison text.
        /// </summary>
        public int PreviousSplitFontSize { get; set; }

        /// <summary>
        /// Whether to show the current grade row in the component display.
        /// </summary>
        public bool ShowCurrentGrade { get; set; }

        /// <summary>
        /// The font size for the current grade display.
        /// </summary>
        public int CurrentGradeFontSize { get; set; }

        /// <summary>
        /// The display style for the current grade (Text or Icon).
        /// </summary>
        public GradeDisplayStyle CurrentGradeDisplayStyle { get; set; }

        /// <summary>
        /// Whether to show grades in split names (e.g., "Split Name [A]").
        /// </summary>
        public bool ShowGradeInSplitNames { get; set; }

        /// <summary>
        /// The format string for split names with grades.
        /// Use {Name} for split name and {Grade} for the grade.
        /// </summary>
        public string SplitNameFormat { get; set; }

        /// <summary>
        /// Whether to show grade icons for each split.
        /// </summary>
        public bool ShowGradeIcons { get; set; }

        /// <summary>
        /// Creates a new GradingSettings instance with default values.
        /// Default thresholds use percentiles where 0=fastest and 100=slowest.
        /// </summary>
        public GradingSettings()
        {
            // Default thresholds (using percentiles: 0=fastest, 100=slowest)
            Thresholds = new List<GradeThreshold>
            {
                new GradeThreshold(7, "S", Color.Gold),           // ~7th percentile (z=-1.5)
                new GradeThreshold(31, "A", Color.Green),         // ~31st percentile (z=-0.5)
                new GradeThreshold(69, "B", Color.LightGreen),    // ~69th percentile (z=+0.5)
                new GradeThreshold(93, "C", Color.Yellow),        // ~93rd percentile (z=+1.5)
                new GradeThreshold(100, "F", Color.Red)           // Everything else
            };

            UseBackgroundColor = false;
            BackgroundColor = Color.Black;
            UseGoldGrade = true;
            GoldLabel = "★";
            GoldColor = Color.Gold;
            UseWorstGrade = true;
            WorstLabel = "\u2717";
            WorstColor = Color.DarkRed;
            ShowGraph = false;
            GraphHeight = 80;
            UseHistogramGraph = true;
            ShowStatistics = true;
            StatisticsFontSize = 10;
            ShowPreviousSplit = false;
            PreviousSplitFontSize = 10;
            ShowCurrentGrade = true;
            CurrentGradeFontSize = 15;
            CurrentGradeDisplayStyle = GradeDisplayStyle.Text;
            ShowGradeInSplitNames = false;
            SplitNameFormat = "{Name} [{Grade}]";
            ShowGradeIcons = false;
        }

        /// <summary>
        /// Creates a deep copy of this settings object.
        /// </summary>
        /// <returns>A new GradingSettings instance with the same values.</returns>
        public GradingSettings Clone()
        {
            var clone = new GradingSettings
            {
                Thresholds = Thresholds.Select(t => t.Clone()).ToList(),
                UseBackgroundColor = UseBackgroundColor,
                BackgroundColor = BackgroundColor,
                UseGoldGrade = UseGoldGrade,
                GoldLabel = GoldLabel,
                GoldColor = GoldColor,
                UseWorstGrade = UseWorstGrade,
                WorstLabel = WorstLabel,
                WorstColor = WorstColor,
                ShowGraph = ShowGraph,
                GraphHeight = GraphHeight,
                UseHistogramGraph = UseHistogramGraph,
                ShowStatistics = ShowStatistics,
                StatisticsFontSize = StatisticsFontSize,
                ShowPreviousSplit = ShowPreviousSplit,
                PreviousSplitFontSize = PreviousSplitFontSize,
                ShowCurrentGrade = ShowCurrentGrade,
                CurrentGradeFontSize = CurrentGradeFontSize,
                CurrentGradeDisplayStyle = CurrentGradeDisplayStyle,
                ShowGradeInSplitNames = ShowGradeInSplitNames,
                SplitNameFormat = SplitNameFormat,
                ShowGradeIcons = ShowGradeIcons
            };
            return clone;
        }
    }
}
