using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    public static class Statistics
    {
        public static double CalculateMean(IEnumerable<double> values)
        {
            if (!values.Any())
                return 0.0;
            return values.Average();
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            var count = values.Count();
            if (count <= 1)
                return 0.0;

            var avg = values.Average();
            var sum = values.Sum(d => Math.Pow(d - avg, 2));
            return Math.Sqrt(sum / (count - 1));
        }

        public static double CalculateZScore(double value, double mean, double stdDev)
        {
            if (stdDev == 0.0)
                return 0.0;
            return (value - mean) / stdDev;
        }

        // Convert z-score to percentile (0-100)
        // Using approximation of cumulative distribution function for normal distribution
        public static double ZScoreToPercentile(double zScore)
        {
            // Using the error function approximation
            // Percentile = 100 * (1 + erf(z/sqrt(2))) / 2
            double percentile = 100.0 * (1.0 + Erf(zScore / Math.Sqrt(2.0))) / 2.0;
            return Math.Max(0, Math.Min(100, percentile));
        }

        // Convert percentile (0-100) to z-score
        public static double PercentileToZScore(double percentile)
        {
            // Clamp percentile to valid range
            percentile = Math.Max(0.01, Math.Min(99.99, percentile));
            
            // Convert to probability (0-1)
            double p = percentile / 100.0;
            
            // Inverse normal CDF approximation
            return InverseNormalCDF(p);
        }

        // Error function approximation (Abramowitz and Stegun)
        private static double Erf(double x)
        {
            // Constants
            double a1 =  0.254829592;
            double a2 = -0.284496736;
            double a3 =  1.421413741;
            double a4 = -1.453152027;
            double a5 =  1.061405429;
            double p  =  0.3275911;

            // Save the sign of x
            int sign = x < 0 ? -1 : 1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        // Inverse normal CDF using Beasley-Springer-Moro algorithm
        private static double InverseNormalCDF(double p)
        {
            double[] c = { 0.3374754822726147, 0.9761690190917186, 0.1607979714918209,
                          0.0276438810333863, 0.0038405729373609, 0.0003951896511919,
                          0.0000321767881768, 0.0000002888167364, 0.0000003960315187 };
            double[] b = { -8.47351093090, 23.08336743743, -21.06224101826, 3.13082909833 };

            if (p <= 0.5)
            {
                double t = Math.Sqrt(-2.0 * Math.Log(p));
                double numerator = ((((((((c[8] * t + c[7]) * t + c[6]) * t + c[5]) * t + c[4]) * t + c[3]) * t + c[2]) * t + c[1]) * t + c[0]);
                double denominator = ((((b[3] * t + b[2]) * t + b[1]) * t + b[0]) * t + 1.0);
                return -numerator / denominator;
            }
            else
            {
                double t = Math.Sqrt(-2.0 * Math.Log(1.0 - p));
                double numerator = ((((((((c[8] * t + c[7]) * t + c[6]) * t + c[5]) * t + c[4]) * t + c[3]) * t + c[2]) * t + c[1]) * t + c[0]);
                double denominator = ((((b[3] * t + b[2]) * t + b[1]) * t + b[0]) * t + 1.0);
                return numerator / denominator;
            }
        }
    }
}
