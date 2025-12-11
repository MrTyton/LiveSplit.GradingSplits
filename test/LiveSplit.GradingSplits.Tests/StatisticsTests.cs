using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using LiveSplit.GradingSplits.Model;

namespace LiveSplit.GradingSplits.Tests
{
    /// <summary>
    /// Comprehensive tests for the Statistics class.
    /// Tests statistical calculations including mean, standard deviation, z-score, and percentile conversions.
    /// </summary>
    public class StatisticsTests
    {
        #region CalculateMean Tests

        [Fact]
        public void CalculateMean_EmptyCollection_ReturnsZero()
        {
            var values = new List<double>();
            var result = Statistics.CalculateMean(values);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void CalculateMean_SingleValue_ReturnsThatValue()
        {
            var values = new List<double> { 42.0 };
            var result = Statistics.CalculateMean(values);
            Assert.Equal(42.0, result);
        }

        [Theory]
        [InlineData(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, 3.0)]
        [InlineData(new double[] { 10.0, 20.0, 30.0 }, 20.0)]
        [InlineData(new double[] { 100.0, 100.0, 100.0, 100.0 }, 100.0)]
        [InlineData(new double[] { -5.0, 5.0 }, 0.0)]
        [InlineData(new double[] { 1.5, 2.5, 3.5 }, 2.5)]
        public void CalculateMean_VariousInputs_ReturnsCorrectMean(double[] input, double expected)
        {
            var result = Statistics.CalculateMean(input);
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void CalculateMean_LargeDataset_ReturnsCorrectMean()
        {
            // Generate 1000 values from 1 to 1000
            var values = Enumerable.Range(1, 1000).Select(x => (double)x).ToList();
            var result = Statistics.CalculateMean(values);
            Assert.Equal(500.5, result, 6);
        }

        #endregion

        #region CalculateStandardDeviation Tests

        [Fact]
        public void CalculateStandardDeviation_EmptyCollection_ReturnsZero()
        {
            var values = new List<double>();
            var result = Statistics.CalculateStandardDeviation(values);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void CalculateStandardDeviation_SingleValue_ReturnsZero()
        {
            var values = new List<double> { 42.0 };
            var result = Statistics.CalculateStandardDeviation(values);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void CalculateStandardDeviation_IdenticalValues_ReturnsZero()
        {
            var values = new List<double> { 5.0, 5.0, 5.0, 5.0, 5.0 };
            var result = Statistics.CalculateStandardDeviation(values);
            Assert.Equal(0.0, result);
        }

        [Theory]
        [InlineData(new double[] { 2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0 }, 2.138)]
        [InlineData(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, 1.581)]
        public void CalculateStandardDeviation_KnownValues_ReturnsCorrectStdDev(double[] input, double expected)
        {
            var result = Statistics.CalculateStandardDeviation(input);
            Assert.Equal(expected, result, 2); // Allow for rounding differences
        }

        [Fact]
        public void CalculateStandardDeviation_TwoValues_CalculatesCorrectly()
        {
            // Two values: 0 and 10, mean = 5, variance = ((0-5)^2 + (10-5)^2) / 1 = 50, stddev = sqrt(50) ≈ 7.07
            var values = new List<double> { 0.0, 10.0 };
            var result = Statistics.CalculateStandardDeviation(values);
            Assert.Equal(7.07, result, 2);
        }

        #endregion

        #region CalculateZScore Tests

        [Fact]
        public void CalculateZScore_ZeroStdDev_ReturnsZero()
        {
            var result = Statistics.CalculateZScore(100, 50, 0);
            Assert.Equal(0.0, result);
        }

        [Theory]
        [InlineData(50, 50, 10, 0.0)]      // Value equals mean
        [InlineData(60, 50, 10, 1.0)]      // One stddev above mean
        [InlineData(40, 50, 10, -1.0)]     // One stddev below mean
        [InlineData(70, 50, 10, 2.0)]      // Two stddev above mean
        [InlineData(30, 50, 10, -2.0)]     // Two stddev below mean
        [InlineData(55, 50, 10, 0.5)]      // Half stddev above mean
        public void CalculateZScore_VariousInputs_ReturnsCorrectZScore(double value, double mean, double stdDev, double expected)
        {
            var result = Statistics.CalculateZScore(value, mean, stdDev);
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void CalculateZScore_NegativeValues_WorksCorrectly()
        {
            // mean = -10, stddev = 5, value = -5 (one stddev above mean)
            var result = Statistics.CalculateZScore(-5, -10, 5);
            Assert.Equal(1.0, result, 6);
        }

        #endregion

        #region ZScoreToPercentile Tests

        [Theory]
        [InlineData(0.0, 50.0)]           // Mean = 50th percentile
        [InlineData(-1.0, 15.87)]         // One stddev below ≈ 16th percentile
        [InlineData(1.0, 84.13)]          // One stddev above ≈ 84th percentile
        [InlineData(-2.0, 2.28)]          // Two stddev below ≈ 2nd percentile
        [InlineData(2.0, 97.72)]          // Two stddev above ≈ 98th percentile
        [InlineData(-1.5, 6.68)]          // z=-1.5 ≈ 7th percentile (our S grade threshold)
        [InlineData(1.5, 93.32)]          // z=+1.5 ≈ 93rd percentile (our C grade threshold)
        public void ZScoreToPercentile_StandardValues_ReturnsCorrectPercentile(double zScore, double expected)
        {
            var result = Statistics.ZScoreToPercentile(zScore);
            Assert.Equal(expected, result, 1); // Allow 0.1 tolerance for approximation
        }

        [Theory]
        [InlineData(-10.0)]  // Extreme negative
        [InlineData(10.0)]   // Extreme positive
        public void ZScoreToPercentile_ExtremeValues_ClampedTo0And100(double zScore)
        {
            var result = Statistics.ZScoreToPercentile(zScore);
            Assert.InRange(result, 0, 100);
        }

        [Fact]
        public void ZScoreToPercentile_VeryLargeNegative_ReturnsCloseToZero()
        {
            var result = Statistics.ZScoreToPercentile(-5);
            Assert.True(result < 0.01);
        }

        [Fact]
        public void ZScoreToPercentile_VeryLargePositive_ReturnsCloseToHundred()
        {
            var result = Statistics.ZScoreToPercentile(5);
            Assert.True(result > 99.99);
        }

        #endregion

        #region ZScoreToPercentile Symmetry Tests

        [Theory]
        [InlineData(-2.5)]
        [InlineData(-1.0)]
        [InlineData(0.0)]
        [InlineData(1.0)]
        [InlineData(2.5)]
        public void ZScoreToPercentile_IsSymmetricAroundMean(double zScore)
        {
            var percentilePositive = Statistics.ZScoreToPercentile(Math.Abs(zScore));
            var percentileNegative = Statistics.ZScoreToPercentile(-Math.Abs(zScore));

            // Sum should be close to 100 due to symmetry
            var sum = percentilePositive + percentileNegative;
            Assert.Equal(100.0, sum, 1);
        }

        #endregion

        #region Integration Tests with Real Split-like Data

        [Fact]
        public void Statistics_WithRealisticSplitTimes_CalculatesCorrectly()
        {
            // Simulate a split with times in seconds: 60s average, varying by ~5s
            var splitTimes = new List<double> { 55.2, 58.1, 60.0, 61.3, 64.8, 59.5, 62.1, 57.9 };

            var mean = Statistics.CalculateMean(splitTimes);
            var stdDev = Statistics.CalculateStandardDeviation(splitTimes);

            // Check mean is roughly 60
            Assert.InRange(mean, 58, 62);

            // Check stddev is reasonable (should be around 3)
            Assert.InRange(stdDev, 2, 5);

            // A fast time (55s) should have negative z-score
            var fastZScore = Statistics.CalculateZScore(55.0, mean, stdDev);
            Assert.True(fastZScore < 0);

            // A slow time (65s) should have positive z-score
            var slowZScore = Statistics.CalculateZScore(65.0, mean, stdDev);
            Assert.True(slowZScore > 0);
        }

        [Fact]
        public void Statistics_WithConsistentRunner_ShowsLowVariation()
        {
            // Consistent runner: all times within 1 second
            var splitTimes = new List<double> { 30.0, 30.2, 30.1, 29.9, 30.3, 30.0, 30.1 };

            var stdDev = Statistics.CalculateStandardDeviation(splitTimes);

            // Standard deviation should be very low
            Assert.True(stdDev < 0.5);
        }

        [Fact]
        public void Statistics_WithInconsistentRunner_ShowsHighVariation()
        {
            // Inconsistent runner: times vary by 20+ seconds
            var splitTimes = new List<double> { 30.0, 45.0, 35.0, 50.0, 28.0, 55.0, 40.0 };

            var stdDev = Statistics.CalculateStandardDeviation(splitTimes);

            // Standard deviation should be high
            Assert.True(stdDev > 8);
        }

        #endregion
    }
}
