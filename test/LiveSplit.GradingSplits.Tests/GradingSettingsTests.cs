using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Xunit;
using LiveSplit.GradingSplits.Model;

namespace LiveSplit.GradingSplits.Tests
{
    /// <summary>
    /// Comprehensive tests for the GradingSettings class.
    /// Tests default values, cloning, and threshold management.
    /// </summary>
    public class GradingSettingsTests
    {
        #region Default Value Tests

        [Fact]
        public void Constructor_SetsDefaultThresholds()
        {
            var settings = new GradingSettings();

            Assert.NotNull(settings.Thresholds);
            Assert.Equal(5, settings.Thresholds.Count);
        }

        [Fact]
        public void Constructor_DefaultThresholdsAreOrdered()
        {
            var settings = new GradingSettings();

            var percentiles = settings.Thresholds.Select(t => t.PercentileThreshold).ToList();
            var orderedPercentiles = percentiles.OrderBy(p => p).ToList();

            Assert.Equal(orderedPercentiles, percentiles);
        }

        [Fact]
        public void Constructor_DefaultThresholdsHaveCorrectLabels()
        {
            var settings = new GradingSettings();

            var labels = settings.Thresholds.Select(t => t.Label).ToList();

            Assert.Contains("S", labels);
            Assert.Contains("A", labels);
            Assert.Contains("B", labels);
            Assert.Contains("C", labels);
            Assert.Contains("F", labels);
        }

        [Theory]
        [InlineData(nameof(GradingSettings.UseBackgroundColor), false)]
        [InlineData(nameof(GradingSettings.UseGoldGrade), true)]
        [InlineData(nameof(GradingSettings.UseWorstGrade), true)]
        [InlineData(nameof(GradingSettings.ShowGraph), false)]
        [InlineData(nameof(GradingSettings.UseHistogramGraph), true)]
        [InlineData(nameof(GradingSettings.ShowStatistics), true)]
        [InlineData(nameof(GradingSettings.ShowPreviousSplit), false)]
        [InlineData(nameof(GradingSettings.ShowCurrentGrade), true)]
        [InlineData(nameof(GradingSettings.ShowGradeInSplitNames), false)]
        public void Constructor_BooleanDefaults_AreCorrect(string propertyName, bool expectedValue)
        {
            var settings = new GradingSettings();
            var property = typeof(GradingSettings).GetProperty(propertyName);
            var actualValue = (bool)property.GetValue(settings);

            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Constructor_DefaultGoldLabel_IsStar()
        {
            var settings = new GradingSettings();
            Assert.Equal("★", settings.GoldLabel);
        }

        [Fact]
        public void Constructor_DefaultWorstLabel_IsX()
        {
            var settings = new GradingSettings();
            Assert.Equal("\u2717", settings.WorstLabel); // ✗
        }

        [Fact]
        public void Constructor_DefaultGoldColor_IsGold()
        {
            var settings = new GradingSettings();
            Assert.Equal(Color.Gold, settings.GoldColor);
        }

        [Fact]
        public void Constructor_DefaultWorstColor_IsDarkRed()
        {
            var settings = new GradingSettings();
            Assert.Equal(Color.DarkRed, settings.WorstColor);
        }

        [Fact]
        public void Constructor_DefaultBackgroundColor_IsBlack()
        {
            var settings = new GradingSettings();
            Assert.Equal(Color.Black, settings.BackgroundColor);
        }

        [Fact]
        public void Constructor_DefaultGraphHeight_Is80()
        {
            var settings = new GradingSettings();
            Assert.Equal(80, settings.GraphHeight);
        }

        [Fact]
        public void Constructor_DefaultStatisticsFontSize_Is10()
        {
            var settings = new GradingSettings();
            Assert.Equal(10, settings.StatisticsFontSize);
        }

        [Fact]
        public void Constructor_DefaultPreviousSplitFontSize_Is10()
        {
            var settings = new GradingSettings();
            Assert.Equal(10, settings.PreviousSplitFontSize);
        }

        [Fact]
        public void Constructor_DefaultSplitNameFormat_IsCorrect()
        {
            var settings = new GradingSettings();
            Assert.Equal("{Name} [{Grade}]", settings.SplitNameFormat);
        }

        [Fact]
        public void Constructor_DefaultCurrentGradeFontSize_Is15()
        {
            var settings = new GradingSettings();
            Assert.Equal(15, settings.CurrentGradeFontSize);
        }

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_CreatesNewInstance()
        {
            var original = new GradingSettings();
            var clone = original.Clone();

            Assert.NotSame(original, clone);
        }

        [Fact]
        public void Clone_CopiesAllBooleanProperties()
        {
            var original = new GradingSettings
            {
                UseBackgroundColor = true,
                UseGoldGrade = false,
                UseWorstGrade = false,
                ShowGraph = true,
                UseHistogramGraph = false,
                ShowStatistics = false,
                ShowPreviousSplit = true,
                ShowCurrentGrade = false,
                ShowGradeInSplitNames = true
            };

            var clone = original.Clone();

            Assert.Equal(original.UseBackgroundColor, clone.UseBackgroundColor);
            Assert.Equal(original.UseGoldGrade, clone.UseGoldGrade);
            Assert.Equal(original.UseWorstGrade, clone.UseWorstGrade);
            Assert.Equal(original.ShowGraph, clone.ShowGraph);
            Assert.Equal(original.UseHistogramGraph, clone.UseHistogramGraph);
            Assert.Equal(original.ShowStatistics, clone.ShowStatistics);
            Assert.Equal(original.ShowPreviousSplit, clone.ShowPreviousSplit);
            Assert.Equal(original.ShowCurrentGrade, clone.ShowCurrentGrade);
            Assert.Equal(original.ShowGradeInSplitNames, clone.ShowGradeInSplitNames);
        }

        [Fact]
        public void Clone_CopiesAllColorProperties()
        {
            var original = new GradingSettings
            {
                BackgroundColor = Color.Blue,
                GoldColor = Color.Magenta,
                WorstColor = Color.Cyan
            };

            var clone = original.Clone();

            Assert.Equal(original.BackgroundColor, clone.BackgroundColor);
            Assert.Equal(original.GoldColor, clone.GoldColor);
            Assert.Equal(original.WorstColor, clone.WorstColor);
        }

        [Fact]
        public void Clone_CopiesAllStringProperties()
        {
            var original = new GradingSettings
            {
                GoldLabel = "GOLD!",
                WorstLabel = "BAD!",
                SplitNameFormat = "[{Grade}] {Name}"
            };

            var clone = original.Clone();

            Assert.Equal(original.GoldLabel, clone.GoldLabel);
            Assert.Equal(original.WorstLabel, clone.WorstLabel);
            Assert.Equal(original.SplitNameFormat, clone.SplitNameFormat);
        }

        [Fact]
        public void Clone_CopiesAllIntProperties()
        {
            var original = new GradingSettings
            {
                GraphHeight = 150,
                StatisticsFontSize = 14,
                PreviousSplitFontSize = 12,
                CurrentGradeFontSize = 20
            };

            var clone = original.Clone();

            Assert.Equal(original.GraphHeight, clone.GraphHeight);
            Assert.Equal(original.StatisticsFontSize, clone.StatisticsFontSize);
            Assert.Equal(original.PreviousSplitFontSize, clone.PreviousSplitFontSize);
            Assert.Equal(original.CurrentGradeFontSize, clone.CurrentGradeFontSize);
        }

        [Fact]
        public void Clone_CreatesDeepCopyOfThresholds()
        {
            var original = new GradingSettings();
            var clone = original.Clone();

            // Thresholds list should be different instance
            Assert.NotSame(original.Thresholds, clone.Thresholds);

            // Each threshold should be different instance
            for (int i = 0; i < original.Thresholds.Count; i++)
            {
                Assert.NotSame(original.Thresholds[i], clone.Thresholds[i]);
            }
        }

        [Fact]
        public void Clone_ThresholdsHaveSameValues()
        {
            var original = new GradingSettings();
            var clone = original.Clone();

            Assert.Equal(original.Thresholds.Count, clone.Thresholds.Count);

            for (int i = 0; i < original.Thresholds.Count; i++)
            {
                Assert.Equal(original.Thresholds[i].PercentileThreshold, clone.Thresholds[i].PercentileThreshold);
                Assert.Equal(original.Thresholds[i].Label, clone.Thresholds[i].Label);
                Assert.Equal(original.Thresholds[i].ForegroundColor, clone.Thresholds[i].ForegroundColor);
            }
        }

        [Fact]
        public void Clone_ModifyingClone_DoesNotAffectOriginal()
        {
            var original = new GradingSettings();
            var clone = original.Clone();

            // Modify clone
            clone.UseGoldGrade = false;
            clone.GoldLabel = "MODIFIED";
            clone.GraphHeight = 200;
            clone.Thresholds.Clear();

            // Original should be unchanged
            Assert.True(original.UseGoldGrade);
            Assert.Equal("★", original.GoldLabel);
            Assert.Equal(80, original.GraphHeight);
            Assert.Equal(5, original.Thresholds.Count);
        }

        #endregion

        #region Threshold Validation Tests

        [Fact]
        public void DefaultThresholds_CoverEntirePercentileRange()
        {
            var settings = new GradingSettings();
            var maxThreshold = settings.Thresholds.Max(t => t.PercentileThreshold);

            Assert.Equal(100, maxThreshold);
        }

        [Fact]
        public void DefaultThresholds_AllHaveNonEmptyLabels()
        {
            var settings = new GradingSettings();

            foreach (var threshold in settings.Thresholds)
            {
                Assert.False(string.IsNullOrEmpty(threshold.Label));
            }
        }

        [Fact]
        public void DefaultThresholds_AllHaveNonDefaultColors()
        {
            var settings = new GradingSettings();

            foreach (var threshold in settings.Thresholds)
            {
                Assert.NotEqual(Color.Empty, threshold.ForegroundColor);
            }
        }

        #endregion
    }
}
