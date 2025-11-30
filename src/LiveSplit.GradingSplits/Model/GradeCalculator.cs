using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    /// <summary>
    /// Provides static methods for calculating grades based on statistical performance.
    /// </summary>
    public static class GradeCalculator
    {
        /// <summary>
        /// Calculates a grade and color based on a z-score and grading settings.
        /// </summary>
        /// <param name="zScore">The z-score representing how many standard deviations from the mean.</param>
        /// <param name="settings">The grading settings containing thresholds and special badge configurations.</param>
        /// <param name="isGold">Whether this is the best (gold) segment.</param>
        /// <param name="isWorst">Whether this is the worst segment.</param>
        /// <returns>A tuple containing the grade label and its associated color.</returns>
        public static (string Grade, Color Color) CalculateGrade(double zScore, GradingSettings settings, bool isGold = false, bool isWorst = false)
        {
            // Convert z-score to percentile (0-100 scale)
            double percentile = Statistics.ZScoreToPercentile(zScore);
            
            // Check for special badges first
            if (isGold && settings.UseGoldGrade)
            {
                return (settings.GoldLabel, settings.GoldColor);
            }
            
            if (isWorst && settings.UseWorstGrade)
            {
                return (settings.WorstLabel, settings.WorstColor);
            }
            
            // Find the appropriate grade based on percentile thresholds
            foreach (var threshold in settings.Thresholds.OrderBy(t => t.PercentileThreshold))
            {
                if (percentile <= threshold.PercentileThreshold)
                {
                    return (threshold.Label, threshold.ForegroundColor);
                }
            }
            
            // Fallback to last threshold
            var lastThreshold = settings.Thresholds.LastOrDefault();
            if (lastThreshold != null)
            {
                return (lastThreshold.Label, lastThreshold.ForegroundColor);
            }
            
            return ("?", Color.White);
        }
    }
}
