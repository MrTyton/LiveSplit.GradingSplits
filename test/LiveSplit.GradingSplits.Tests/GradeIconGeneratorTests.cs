using System.Drawing;
using Xunit;
using LiveSplit.GradingSplits.Model;

namespace LiveSplit.GradingSplits.Tests
{
    public class GradeIconGeneratorTests
    {
        [Fact]
        public void GenerateIcon_ReturnsCorrectSize()
        {
            var icon = GradeIconGenerator.GenerateIcon("A", Color.Green);

            Assert.NotNull(icon);
            Assert.Equal(GradeIconGenerator.IconSize, icon.Width);
            Assert.Equal(GradeIconGenerator.IconSize, icon.Height);
        }

        [Fact]
        public void GenerateIcon_WithSingleCharacter_ReturnsImage()
        {
            var icon = GradeIconGenerator.GenerateIcon("S", Color.Gold);

            Assert.NotNull(icon);
            Assert.IsType<Bitmap>(icon);
        }

        [Fact]
        public void GenerateIcon_WithMultipleCharacters_ReturnsImage()
        {
            var icon = GradeIconGenerator.GenerateIcon("★", Color.Gold);

            Assert.NotNull(icon);
            Assert.IsType<Bitmap>(icon);
        }

        [Fact]
        public void GenerateIcon_WithEmptyLabel_ReturnsImage()
        {
            var icon = GradeIconGenerator.GenerateIcon("", Color.White);

            Assert.NotNull(icon);
            Assert.IsType<Bitmap>(icon);
        }

        [Fact]
        public void GenerateIcon_WithNullLabel_ReturnsImage()
        {
            var icon = GradeIconGenerator.GenerateIcon(null, Color.White);

            Assert.NotNull(icon);
            Assert.IsType<Bitmap>(icon);
        }

        [Fact]
        public void GenerateIcon_WithCustomBackgroundColor_ReturnsImage()
        {
            var icon = GradeIconGenerator.GenerateIcon("A", Color.Green, Color.DarkGreen);

            Assert.NotNull(icon);
            Assert.IsType<Bitmap>(icon);
        }

        [Fact]
        public void GenerateIcon_WithDifferentColors_ReturnsDistinctImages()
        {
            var iconGold = GradeIconGenerator.GenerateIcon("S", Color.Gold) as Bitmap;
            var iconRed = GradeIconGenerator.GenerateIcon("F", Color.Red) as Bitmap;

            Assert.NotNull(iconGold);
            Assert.NotNull(iconRed);

            // The icons should have different pixel colors (at least some)
            bool hasDifference = false;
            for (int x = 0; x < iconGold.Width && !hasDifference; x++)
            {
                for (int y = 0; y < iconGold.Height && !hasDifference; y++)
                {
                    if (iconGold.GetPixel(x, y) != iconRed.GetPixel(x, y))
                    {
                        hasDifference = true;
                    }
                }
            }

            Assert.True(hasDifference, "Different colored icons should have visual differences");
        }

        [Theory]
        [InlineData("S")]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("C")]
        [InlineData("F")]
        [InlineData("★")]
        [InlineData("✗")]
        public void GenerateIcon_WithAllGradeLabels_ReturnsValidImage(string label)
        {
            var icon = GradeIconGenerator.GenerateIcon(label, Color.White);

            Assert.NotNull(icon);
            Assert.Equal(24, icon.Width);
            Assert.Equal(24, icon.Height);
        }

        [Fact]
        public void IconSize_Is24()
        {
            Assert.Equal(24, GradeIconGenerator.IconSize);
        }

        [Fact]
        public void GenerateIcon_ReturnsBitmap()
        {
            var icon = GradeIconGenerator.GenerateIcon("A", Color.Green);

            Assert.IsType<Bitmap>(icon);
        }

        [Fact]
        public void GenerateIcon_WithLongLabel_ReturnsImage()
        {
            // Test with a longer label to ensure font scaling works
            var icon = GradeIconGenerator.GenerateIcon("SS", Color.Gold);

            Assert.NotNull(icon);
            Assert.Equal(24, icon.Width);
            Assert.Equal(24, icon.Height);
        }
    }
}
