using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    public class GradingSettings
    {
        public List<GradeThreshold> Thresholds { get; set; }
        public bool UseBackgroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public bool UseGoldGrade { get; set; }
        public string GoldLabel { get; set; }
        public Color GoldColor { get; set; }
        public bool UseWorstGrade { get; set; }
        public string WorstLabel { get; set; }
        public Color WorstColor { get; set; }
        public bool ShowGraph { get; set; }
        public int GraphHeight { get; set; }

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
        }

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
                GraphHeight = GraphHeight
            };
            return clone;
        }
    }
}
