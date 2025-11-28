using System.Drawing;

namespace LiveSplit.GradingSplits.Model
{
    public class GradeCalculator
    {
        public static (string Grade, Color Color) CalculateGrade(double zScore)
        {
            if (zScore < -2.0) return ("S", Color.Gold);
            if (zScore < -1.5) return ("A+", Color.SpringGreen);
            if (zScore < -1.0) return ("A", Color.Green);
            if (zScore < -0.5) return ("B", Color.LightGreen);
            if (zScore < 0.5) return ("C", Color.Yellow);
            if (zScore < 1.0) return ("D", Color.Orange);
            if (zScore < 2.0) return ("E", Color.Red);
            return ("F", Color.DarkRed);
        }
    }
}
