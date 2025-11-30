using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    public static class GradeCalculator
    {
        public static (string Grade, Color Color) CalculateGrade(double zScore, GradingSettings settings, bool isGold = false, bool isWorst = false)
        {
            // Convert z-score to percentile
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
