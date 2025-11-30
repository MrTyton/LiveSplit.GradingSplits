using System;
using System.Drawing;

namespace LiveSplit.GradingSplits.Model
{
    public class GradeThreshold
    {
        public double ZScoreThreshold { get; set; }
        public string Label { get; set; }
        public Color ForegroundColor { get; set; }

        public GradeThreshold(double threshold, string label, Color color)
        {
            ZScoreThreshold = threshold;
            Label = label;
            ForegroundColor = color;
        }

        public GradeThreshold Clone()
        {
            return new GradeThreshold(ZScoreThreshold, Label, ForegroundColor);
        }
    }
}
