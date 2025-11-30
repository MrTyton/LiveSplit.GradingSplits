using System;
using System.Drawing;

namespace LiveSplit.GradingSplits.Model
{
    public class GradeThreshold
    {
        public double PercentileThreshold { get; set; }
        public string Label { get; set; }
        public Color ForegroundColor { get; set; }

        public GradeThreshold(double percentileThreshold, string label, Color foregroundColor)
        {
            PercentileThreshold = percentileThreshold;
            Label = label;
            ForegroundColor = foregroundColor;
        }

        public GradeThreshold Clone()
        {
            return new GradeThreshold(PercentileThreshold, Label, ForegroundColor);
        }
    }
}
