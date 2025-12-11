using System.Collections.Generic;
using System.Drawing;
using Xunit;
using LiveSplit.GradingSplits.Model;

namespace LiveSplit.GradingSplits.Tests
{
    /// <summary>
    /// Comprehensive tests for the GradeCalculator class.
    /// Tests grade calculation based on z-scores, percentiles, and special badges.
    /// </summary>
    public class GradeCalculatorTests
    {
        #region Helper Methods

        private static GradingSettings CreateDefaultSettings()
        {
            return new GradingSettings();
        }

        private static GradingSettings CreateCustomSettings(List<GradeThreshold> thresholds)
        {
            var settings = new GradingSettings
            {
                Thresholds = thresholds
            };
            return settings;
        }

        #endregion

        #region Default Threshold Tests

        [Theory]
        [InlineData(-2.0, "S")]    // z=-2.0 → ~2.28 percentile → S grade (≤7)
        [InlineData(-1.5, "S")]    // z=-1.5 → ~6.68 percentile → S grade (≤7)
        [InlineData(-1.0, "A")]    // z=-1.0 → ~15.87 percentile → A grade (≤31)
        [InlineData(-0.5, "A")]    // z=-0.5 → ~30.85 percentile → A grade (≤31)
        [InlineData(0.0, "B")]     // z=0.0 → 50 percentile → B grade (≤69)
        [InlineData(0.5, "C")]     // z=0.5 → ~69.15 percentile → C grade (>69, ≤93)
        [InlineData(1.0, "C")]     // z=1.0 → ~84.13 percentile → C grade (≤93)
        [InlineData(1.5, "F")]     // z=1.5 → ~93.32 percentile → F grade (>93)
        [InlineData(2.0, "F")]     // z=2.0 → ~97.72 percentile → F grade
        [InlineData(3.0, "F")]     // z=3.0 → >99 percentile → F grade
        public void CalculateGrade_DefaultThresholds_ReturnsCorrectGrade(double zScore, string expectedGrade)
        {
            var settings = CreateDefaultSettings();
            var result = GradeCalculator.CalculateGrade(zScore, settings);
            Assert.Equal(expectedGrade, result.Grade);
        }

        [Theory]
        [InlineData(-2.0)]    // S grade
        [InlineData(-1.0)]    // A grade  
        [InlineData(0.0)]     // B grade
        [InlineData(1.0)]     // C grade
        [InlineData(2.0)]     // F grade
        public void CalculateGrade_DefaultThresholds_ReturnsNonDefaultColor(double zScore)
        {
            var settings = CreateDefaultSettings();
            var result = GradeCalculator.CalculateGrade(zScore, settings);
            
            // Should return a color from the thresholds, not the fallback white
            Assert.NotEqual(Color.White, result.Color);
        }

        #endregion

        #region Gold Badge Tests

        [Fact]
        public void CalculateGrade_GoldSegment_ReturnsGoldBadge()
        {
            var settings = CreateDefaultSettings();
            settings.UseGoldGrade = true;
            settings.GoldLabel = "★";
            settings.GoldColor = Color.Gold;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isGold: true);
            
            Assert.Equal("★", result.Grade);
            Assert.Equal(Color.Gold, result.Color);
        }

        [Fact]
        public void CalculateGrade_GoldSegment_DisabledGoldBadge_ReturnsNormalGrade()
        {
            var settings = CreateDefaultSettings();
            settings.UseGoldGrade = false;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isGold: true);
            
            // Should return B grade (z=0 is average)
            Assert.Equal("B", result.Grade);
        }

        [Theory]
        [InlineData("GOLD")]
        [InlineData("🥇")]
        [InlineData("PB!")]
        [InlineData("*")]
        public void CalculateGrade_GoldSegment_CustomLabel_ReturnsCustomLabel(string customLabel)
        {
            var settings = CreateDefaultSettings();
            settings.UseGoldGrade = true;
            settings.GoldLabel = customLabel;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isGold: true);
            
            Assert.Equal(customLabel, result.Grade);
        }

        [Fact]
        public void CalculateGrade_GoldSegment_CustomColor_ReturnsCustomColor()
        {
            var settings = CreateDefaultSettings();
            settings.UseGoldGrade = true;
            settings.GoldColor = Color.Magenta;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isGold: true);
            
            Assert.Equal(Color.Magenta, result.Color);
        }

        #endregion

        #region Worst Badge Tests

        [Fact]
        public void CalculateGrade_WorstSegment_ReturnsWorstBadge()
        {
            var settings = CreateDefaultSettings();
            settings.UseWorstGrade = true;
            settings.WorstLabel = "✗";
            settings.WorstColor = Color.DarkRed;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isWorst: true);
            
            Assert.Equal("✗", result.Grade);
            Assert.Equal(Color.DarkRed, result.Color);
        }

        [Fact]
        public void CalculateGrade_WorstSegment_DisabledWorstBadge_ReturnsNormalGrade()
        {
            var settings = CreateDefaultSettings();
            settings.UseWorstGrade = false;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isWorst: true);
            
            // Should return B grade (z=0 is average)
            Assert.Equal("B", result.Grade);
        }

        [Theory]
        [InlineData("WORST")]
        [InlineData("💀")]
        [InlineData("X")]
        [InlineData("---")]
        public void CalculateGrade_WorstSegment_CustomLabel_ReturnsCustomLabel(string customLabel)
        {
            var settings = CreateDefaultSettings();
            settings.UseWorstGrade = true;
            settings.WorstLabel = customLabel;
            
            var result = GradeCalculator.CalculateGrade(0.0, settings, isWorst: true);
            
            Assert.Equal(customLabel, result.Grade);
        }

        #endregion

        #region Badge Priority Tests

        [Fact]
        public void CalculateGrade_BothGoldAndWorst_GoldTakesPriority()
        {
            var settings = CreateDefaultSettings();
            settings.UseGoldGrade = true;
            settings.UseWorstGrade = true;
            settings.GoldLabel = "★";
            settings.WorstLabel = "✗";
            
            // If somehow both flags are true, gold should take priority
            var result = GradeCalculator.CalculateGrade(0.0, settings, isGold: true, isWorst: true);
            
            Assert.Equal("★", result.Grade);
        }

        #endregion

        #region Custom Threshold Tests

        [Fact]
        public void CalculateGrade_CustomThresholds_TwoTierSystem()
        {
            var thresholds = new List<GradeThreshold>
            {
                new GradeThreshold(50, "GOOD", Color.Green),
                new GradeThreshold(100, "BAD", Color.Red)
            };
            var settings = CreateCustomSettings(thresholds);
            
            // z = -0.1 → ~46th percentile → GOOD
            var goodResult = GradeCalculator.CalculateGrade(-0.1, settings);
            Assert.Equal("GOOD", goodResult.Grade);
            Assert.Equal(Color.Green, goodResult.Color);
            
            // z = 0.1 → ~54th percentile → BAD
            var badResult = GradeCalculator.CalculateGrade(0.1, settings);
            Assert.Equal("BAD", badResult.Grade);
            Assert.Equal(Color.Red, badResult.Color);
        }

        [Fact]
        public void CalculateGrade_CustomThresholds_SingleGrade()
        {
            var thresholds = new List<GradeThreshold>
            {
                new GradeThreshold(100, "PASS", Color.Green)
            };
            var settings = CreateCustomSettings(thresholds);
            
            // Any z-score should return PASS
            Assert.Equal("PASS", GradeCalculator.CalculateGrade(-3.0, settings).Grade);
            Assert.Equal("PASS", GradeCalculator.CalculateGrade(0.0, settings).Grade);
            Assert.Equal("PASS", GradeCalculator.CalculateGrade(3.0, settings).Grade);
        }

        [Fact]
        public void CalculateGrade_CustomThresholds_ManyTiers()
        {
            var thresholds = new List<GradeThreshold>
            {
                new GradeThreshold(10, "S+", Color.Gold),
                new GradeThreshold(20, "S", Color.Yellow),
                new GradeThreshold(30, "A+", Color.LimeGreen),
                new GradeThreshold(40, "A", Color.Green),
                new GradeThreshold(50, "B+", Color.Cyan),
                new GradeThreshold(60, "B", Color.Blue),
                new GradeThreshold(70, "C+", Color.Orange),
                new GradeThreshold(80, "C", Color.OrangeRed),
                new GradeThreshold(90, "D", Color.Red),
                new GradeThreshold(100, "F", Color.DarkRed)
            };
            var settings = CreateCustomSettings(thresholds);
            
            // Test various percentiles
            var s_plus = GradeCalculator.CalculateGrade(-2.5, settings); // ~0.6 percentile → S+ (≤10)
            Assert.Equal("S+", s_plus.Grade);
            
            // z=-0.01 → ~49.6th percentile → B+ (≤50)
            var b_plus = GradeCalculator.CalculateGrade(-0.01, settings);
            Assert.Equal("B+", b_plus.Grade);
            
            // z=0.15 → ~56th percentile → B (>50, ≤60)
            var b = GradeCalculator.CalculateGrade(0.15, settings);
            Assert.Equal("B", b.Grade);
            
            // z=-0.26 → ~39.7th percentile → A (≤40)
            var a = GradeCalculator.CalculateGrade(-0.26, settings);
            Assert.Equal("A", a.Grade);
            
            var f = GradeCalculator.CalculateGrade(3.0, settings); // ~99.9 percentile → F (≤100)
            Assert.Equal("F", f.Grade);
        }

        [Fact]
        public void CalculateGrade_EmptyThresholds_ReturnsFallback()
        {
            var settings = CreateCustomSettings(new List<GradeThreshold>());
            
            var result = GradeCalculator.CalculateGrade(0.0, settings);
            
            Assert.Equal("?", result.Grade);
            Assert.Equal(Color.White, result.Color);
        }

        #endregion

        #region Edge Case Tests

        [Theory]
        [InlineData(-10.0)]
        [InlineData(10.0)]
        public void CalculateGrade_ExtremeZScores_DoesNotCrash(double extremeZScore)
        {
            var settings = CreateDefaultSettings();
            
            // Should not throw
            var result = GradeCalculator.CalculateGrade(extremeZScore, settings);
            
            // Should return a valid grade
            Assert.False(string.IsNullOrEmpty(result.Grade));
        }

        [Fact]
        public void CalculateGrade_ZeroZScore_ReturnsAverageGrade()
        {
            var settings = CreateDefaultSettings();
            
            var result = GradeCalculator.CalculateGrade(0.0, settings);
            
            // z=0 means exactly average, which is B with default thresholds
            Assert.Equal("B", result.Grade);
        }

        #endregion

        #region Color Verification Tests

        [Fact]
        public void CalculateGrade_DefaultThresholds_ReturnsExpectedColors()
        {
            var settings = CreateDefaultSettings();
            
            var sGrade = GradeCalculator.CalculateGrade(-2.0, settings);
            Assert.Equal(Color.Gold, sGrade.Color);
            
            var aGrade = GradeCalculator.CalculateGrade(-1.0, settings);
            Assert.Equal(Color.Green, aGrade.Color);
            
            var bGrade = GradeCalculator.CalculateGrade(0.0, settings);
            Assert.Equal(Color.LightGreen, bGrade.Color);
            
            var cGrade = GradeCalculator.CalculateGrade(1.0, settings);
            Assert.Equal(Color.Yellow, cGrade.Color);
            
            var fGrade = GradeCalculator.CalculateGrade(2.0, settings);
            Assert.Equal(Color.Red, fGrade.Color);
        }

        #endregion
    }
}
