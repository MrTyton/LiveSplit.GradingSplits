using System;
using System.Drawing;

namespace LiveSplit.GradingSplits.Model
{
    /// <summary>
    /// Represents a single grading threshold with a percentile cutoff, label, and color.
    /// </summary>
    public class GradeThreshold
    {
        /// <summary>
        /// The percentile threshold (0-100). Times at or below this percentile receive this grade.
        /// </summary>
        public double PercentileThreshold { get; set; }
        
        /// <summary>
        /// The label to display for this grade (e.g., "S", "A", "B", "C", "F").
        /// </summary>
        public string Label { get; set; }
        
        /// <summary>
        /// The color to use when displaying this grade.
        /// </summary>
        public Color ForegroundColor { get; set; }

        /// <summary>
        /// Creates a new grade threshold.
        /// </summary>
        /// <param name="percentileThreshold">The percentile cutoff (0-100).</param>
        /// <param name="label">The grade label.</param>
        /// <param name="foregroundColor">The grade color.</param>
        public GradeThreshold(double percentileThreshold, string label, Color foregroundColor)
        {
            PercentileThreshold = percentileThreshold;
            Label = label;
            ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// Creates a deep copy of this threshold.
        /// </summary>
        /// <returns>A new GradeThreshold with the same values.</returns>
        public GradeThreshold Clone()
        {
            return new GradeThreshold(PercentileThreshold, Label, ForegroundColor);
        }
    }
}
