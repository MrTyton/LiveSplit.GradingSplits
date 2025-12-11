using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.GradingSplits.Model
{
    /// <summary>
    /// Provides statistical calculation methods for grading analysis.
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Calculates the arithmetic mean (average) of a collection of values.
        /// </summary>
        /// <param name="values">The collection of values to average.</param>
        /// <returns>The mean value, or 0.0 if the collection is empty.</returns>
        public static double CalculateMean(IEnumerable<double> values)
        {
            if (!values.Any())
                return 0.0;
            return values.Average();
        }

        /// <summary>
        /// Calculates the sample standard deviation of a collection of values.
        /// </summary>
        /// <param name="values">The collection of values to analyze.</param>
        /// <returns>The standard deviation, or 0.0 if there are 1 or fewer values.</returns>
        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            var count = values.Count();
            if (count <= 1)
                return 0.0;

            var avg = values.Average();
            var sum = values.Sum(d => Math.Pow(d - avg, 2));
            return Math.Sqrt(sum / (count - 1));
        }

        /// <summary>
        /// Calculates the z-score (number of standard deviations from the mean) for a value.
        /// </summary>
        /// <param name="value">The value to analyze.</param>
        /// <param name="mean">The mean of the distribution.</param>
        /// <param name="stdDev">The standard deviation of the distribution.</param>
        /// <returns>The z-score, or 0.0 if standard deviation is 0.</returns>
        public static double CalculateZScore(double value, double mean, double stdDev)
        {
            if (stdDev == 0.0)
                return 0.0;
            return (value - mean) / stdDev;
        }

        /// <summary>
        /// Converts a z-score to a percentile rank (0-100 scale).
        /// Uses the cumulative distribution function for a normal distribution.
        /// </summary>
        /// <param name="zScore">The z-score to convert.</param>
        /// <returns>The percentile rank, clamped to [0, 100].</returns>
        public static double ZScoreToPercentile(double zScore)
        {
            // Using the error function approximation for the normal CDF
            // Percentile = 100 * (1 + erf(z/sqrt(2))) / 2
            double percentile = 100.0 * (1.0 + Erf(zScore / Math.Sqrt(2.0))) / 2.0;
            return Math.Max(0, Math.Min(100, percentile));
        }



        /// <summary>
        /// Error function approximation using Abramowitz and Stegun formula 7.1.26.
        /// Used for converting z-scores to percentiles.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The error function of x.</returns>
        private static double Erf(double x)
        {
            // Constants for A&S formula 7.1.26
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x (erf is an odd function)
            int sign = x < 0 ? -1 : 1;
            x = Math.Abs(x);

            // A&S formula 7.1.26 computation
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }


    }
}
