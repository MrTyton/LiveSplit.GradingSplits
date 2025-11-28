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
    }
}
