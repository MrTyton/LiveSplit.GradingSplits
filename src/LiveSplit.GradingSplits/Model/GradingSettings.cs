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

        public GradingSettings()
        {
            // Default thresholds
            Thresholds = new List<GradeThreshold>
            {
                new GradeThreshold(-1.5, "S", Color.Gold),
                new GradeThreshold(-0.5, "A", Color.Green),
                new GradeThreshold(0.5, "B", Color.LightGreen),
                new GradeThreshold(1.5, "C", Color.Yellow),
                new GradeThreshold(2.0, "D", Color.Orange),
                new GradeThreshold(double.MaxValue, "F", Color.Red)
            };

            UseBackgroundColor = false;
            BackgroundColor = Color.Black;
            UseGoldGrade = true;
            GoldLabel = "★";
            GoldColor = Color.Gold;
            UseWorstGrade = true;
            WorstLabel = "✗";
            WorstColor = Color.DarkRed;
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
                WorstColor = WorstColor
            };
            return clone;
        }
    }
}
