using System.Drawing;
using Xunit;
using LiveSplit.GradingSplits.Model;

namespace LiveSplit.GradingSplits.Tests
{
    /// <summary>
    /// Comprehensive tests for the GradeThreshold class.
    /// Tests construction, properties, and cloning behavior.
    /// </summary>
    public class GradeThresholdTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var threshold = new GradeThreshold(50.0, "A", Color.Green);
            
            Assert.Equal(50.0, threshold.PercentileThreshold);
            Assert.Equal("A", threshold.Label);
            Assert.Equal(Color.Green, threshold.ForegroundColor);
        }

        [Theory]
        [InlineData(0.0, "Best", "Green")]
        [InlineData(100.0, "Worst", "Red")]
        [InlineData(50.0, "Average", "Yellow")]
        [InlineData(7.0, "S", "Gold")]
        [InlineData(99.9, "F-", "DarkRed")]
        public void Constructor_AcceptsVariousValidInputs(double percentile, string label, string colorName)
        {
            var color = Color.FromName(colorName);
            var threshold = new GradeThreshold(percentile, label, color);
            
            Assert.Equal(percentile, threshold.PercentileThreshold);
            Assert.Equal(label, threshold.Label);
            Assert.Equal(color, threshold.ForegroundColor);
        }

        [Fact]
        public void Constructor_AcceptsEmptyLabel()
        {
            var threshold = new GradeThreshold(50.0, "", Color.White);
            Assert.Equal("", threshold.Label);
        }

        [Fact]
        public void Constructor_AcceptsNullLabel()
        {
            var threshold = new GradeThreshold(50.0, null, Color.White);
            Assert.Null(threshold.Label);
        }

        [Fact]
        public void Constructor_AcceptsUnicodeLabel()
        {
            var threshold = new GradeThreshold(50.0, "★🎮✓", Color.Gold);
            Assert.Equal("★🎮✓", threshold.Label);
        }

        [Fact]
        public void Constructor_AcceptsNegativePercentile()
        {
            // While not semantically correct, the class doesn't validate
            var threshold = new GradeThreshold(-10.0, "Invalid", Color.Red);
            Assert.Equal(-10.0, threshold.PercentileThreshold);
        }

        [Fact]
        public void Constructor_AcceptsPercentileOver100()
        {
            // While not semantically correct, the class doesn't validate
            var threshold = new GradeThreshold(150.0, "Invalid", Color.Red);
            Assert.Equal(150.0, threshold.PercentileThreshold);
        }

        #endregion

        #region Property Modification Tests

        [Fact]
        public void PercentileThreshold_CanBeModified()
        {
            var threshold = new GradeThreshold(50.0, "A", Color.Green);
            threshold.PercentileThreshold = 75.0;
            
            Assert.Equal(75.0, threshold.PercentileThreshold);
        }

        [Fact]
        public void Label_CanBeModified()
        {
            var threshold = new GradeThreshold(50.0, "A", Color.Green);
            threshold.Label = "B";
            
            Assert.Equal("B", threshold.Label);
        }

        [Fact]
        public void ForegroundColor_CanBeModified()
        {
            var threshold = new GradeThreshold(50.0, "A", Color.Green);
            threshold.ForegroundColor = Color.Red;
            
            Assert.Equal(Color.Red, threshold.ForegroundColor);
        }

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_CreatesNewInstance()
        {
            var original = new GradeThreshold(50.0, "A", Color.Green);
            var clone = original.Clone();
            
            Assert.NotSame(original, clone);
        }

        [Fact]
        public void Clone_CopiesAllProperties()
        {
            var original = new GradeThreshold(75.5, "Custom", Color.Magenta);
            var clone = original.Clone();
            
            Assert.Equal(original.PercentileThreshold, clone.PercentileThreshold);
            Assert.Equal(original.Label, clone.Label);
            Assert.Equal(original.ForegroundColor, clone.ForegroundColor);
        }

        [Fact]
        public void Clone_ModifyingClone_DoesNotAffectOriginal()
        {
            var original = new GradeThreshold(50.0, "A", Color.Green);
            var clone = original.Clone();
            
            clone.PercentileThreshold = 100.0;
            clone.Label = "F";
            clone.ForegroundColor = Color.Red;
            
            Assert.Equal(50.0, original.PercentileThreshold);
            Assert.Equal("A", original.Label);
            Assert.Equal(Color.Green, original.ForegroundColor);
        }

        [Fact]
        public void Clone_PreservesUnicodeLabel()
        {
            var original = new GradeThreshold(7.0, "★", Color.Gold);
            var clone = original.Clone();
            
            Assert.Equal("★", clone.Label);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(50.0)]
        [InlineData(100.0)]
        [InlineData(0.001)]
        [InlineData(99.999)]
        public void Clone_PreservesPercentilePrecision(double percentile)
        {
            var original = new GradeThreshold(percentile, "Test", Color.White);
            var clone = original.Clone();
            
            Assert.Equal(percentile, clone.PercentileThreshold);
        }

        #endregion

        #region Color Handling Tests

        [Fact]
        public void ForegroundColor_AcceptsNamedColor()
        {
            var threshold = new GradeThreshold(50.0, "A", Color.DarkOliveGreen);
            Assert.Equal(Color.DarkOliveGreen, threshold.ForegroundColor);
        }

        [Fact]
        public void ForegroundColor_AcceptsCustomRgbColor()
        {
            var customColor = Color.FromArgb(128, 64, 192);
            var threshold = new GradeThreshold(50.0, "A", customColor);
            
            Assert.Equal(128, threshold.ForegroundColor.R);
            Assert.Equal(64, threshold.ForegroundColor.G);
            Assert.Equal(192, threshold.ForegroundColor.B);
        }

        [Fact]
        public void ForegroundColor_AcceptsTransparentColor()
        {
            var transparentColor = Color.FromArgb(128, 255, 0, 0); // Semi-transparent red
            var threshold = new GradeThreshold(50.0, "A", transparentColor);
            
            Assert.Equal(128, threshold.ForegroundColor.A);
        }

        #endregion
    }
}
