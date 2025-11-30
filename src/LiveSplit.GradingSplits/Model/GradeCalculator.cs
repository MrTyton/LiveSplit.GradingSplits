using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    public class GradeCalculator
    {
        public static (string Grade, Color Color) CalculateGrade(double zScore, GradingSettings settings, bool isGoldSplit = false, bool isWorstSplit = false)
        {
            // Check for worst split first (takes priority over gold)
            if (isWorstSplit && settings.UseWorstGrade)
            {
                return (settings.WorstLabel, settings.WorstColor);
            }
            
            // Check for gold split
            if (isGoldSplit && settings.UseGoldGrade)
            {
                return (settings.GoldLabel, settings.GoldColor);
            }

            // Find the first threshold that the z-score is less than
            var threshold = settings.Thresholds.FirstOrDefault(t => zScore < t.ZScoreThreshold);
            
            if (threshold != null)
            {
                return (threshold.Label, threshold.ForegroundColor);
            }

            // Fallback to last threshold
            var lastThreshold = settings.Thresholds.LastOrDefault();
            if (lastThreshold != null)
            {
                return (lastThreshold.Label, lastThreshold.ForegroundColor);
            }

            return ("-", Color.White);
        }
    }
}
