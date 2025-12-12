using LiveSplit.GradingSplits.Model;
using System;
using System.Drawing;
using System.IO;
using Xunit;

namespace LiveSplit.GradingSplits.Tests
{
    public class CustomIconLoaderTests : IDisposable
    {
        private readonly string _testFolderPath;

        public CustomIconLoaderTests()
        {
            // Create a temp folder for test icons
            _testFolderPath = Path.Combine(Path.GetTempPath(), "GradingSplitsIconTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testFolderPath);
        }

        public void Dispose()
        {
            // Clean up test folder
            CustomIconLoader.ClearCache();
            try
            {
                if (Directory.Exists(_testFolderPath))
                {
                    Directory.Delete(_testFolderPath, true);
                }
            }
            catch { /* Ignore cleanup errors */ }
        }

        private void CreateTestIcon(string filename)
        {
            var filePath = Path.Combine(_testFolderPath, filename);
            using (var bitmap = new Bitmap(24, 24))
            {
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private GradingSettings CreateSettings(string iconFolderPath = null)
        {
            return new GradingSettings
            {
                IconFolderPath = iconFolderPath
            };
        }

        #region Constants Tests

        [Fact]
        public void BestIconName_IsCorrect()
        {
            Assert.Equal("Best", CustomIconLoader.BestIconName);
        }

        [Fact]
        public void WorstIconName_IsCorrect()
        {
            Assert.Equal("Worst", CustomIconLoader.WorstIconName);
        }

        #endregion

        #region GetCustomIcon Tests - No Folder

        [Fact]
        public void GetCustomIcon_NoFolderPath_ReturnsNull()
        {
            var settings = CreateSettings(null);
            var result = CustomIconLoader.GetCustomIcon("S", settings);
            Assert.Null(result);
        }

        [Fact]
        public void GetCustomIcon_EmptyFolderPath_ReturnsNull()
        {
            var settings = CreateSettings("");
            var result = CustomIconLoader.GetCustomIcon("S", settings);
            Assert.Null(result);
        }

        [Fact]
        public void GetCustomIcon_NonExistentFolder_ReturnsNull()
        {
            var settings = CreateSettings(@"C:\NonExistent\Folder\Path");
            var result = CustomIconLoader.GetCustomIcon("S", settings);
            Assert.Null(result);
        }

        #endregion

        #region GetCustomIcon Tests - Folder Based

        [Fact]
        public void GetCustomIcon_WithMatchingIcon_ReturnsImage()
        {
            CreateTestIcon("S.png");
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("S", settings);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<Image>(result);
        }

        [Fact]
        public void GetCustomIcon_NoMatchingIcon_ReturnsNull()
        {
            CreateTestIcon("S.png");
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("Z", settings);

            Assert.Null(result);
        }

        [Fact]
        public void GetCustomIcon_BestSplit_LoadsBestIcon()
        {
            CreateTestIcon("Best.png");
            CreateTestIcon("S.png");
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("S", settings, isGold: true);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustomIcon_BestSplit_NoBestIcon_FallsBackToGradeIcon()
        {
            CreateTestIcon("S.png");
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("S", settings, isGold: true);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustomIcon_WorstSplit_LoadsWorstIcon()
        {
            CreateTestIcon("Worst.png");
            CreateTestIcon("F.png");
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("F", settings, isWorst: true);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustomIcon_WorstSplit_NoWorstIcon_FallsBackToGradeIcon()
        {
            CreateTestIcon("F.png");
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("F", settings, isWorst: true);

            Assert.NotNull(result);
        }

        #endregion

        #region GetCustomIcon Tests - Per-Threshold Custom Path

        [Fact]
        public void GetCustomIcon_ThresholdCustomPath_TakesPriority()
        {
            // Create two different icons
            CreateTestIcon("S.png");
            var customPath = Path.Combine(_testFolderPath, "CustomS.png");
            using (var bitmap = new Bitmap(32, 32)) // Different size to distinguish
            {
                bitmap.Save(customPath, System.Drawing.Imaging.ImageFormat.Png);
            }

            var threshold = new GradeThreshold(7, "S", Color.Gold)
            {
                CustomIconPath = customPath
            };
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("S", settings, threshold);

            Assert.NotNull(result);
            Assert.Equal(32, result.Width); // Should be the custom 32x32 icon
        }

        [Fact]
        public void GetCustomIcon_ThresholdCustomPath_InvalidPath_FallsBackToFolder()
        {
            CreateTestIcon("S.png");
            var threshold = new GradeThreshold(7, "S", Color.Gold)
            {
                CustomIconPath = @"C:\NonExistent\Icon.png"
            };
            var settings = CreateSettings(_testFolderPath);

            var result = CustomIconLoader.GetCustomIcon("S", settings, threshold);

            Assert.NotNull(result);
            Assert.Equal(24, result.Width); // Should be the folder-based 24x24 icon
        }

        #endregion

        #region LoadIconFromPath Tests

        [Fact]
        public void LoadIconFromPath_ValidPath_ReturnsImage()
        {
            CreateTestIcon("test.png");
            var path = Path.Combine(_testFolderPath, "test.png");

            var result = CustomIconLoader.LoadIconFromPath(path);

            Assert.NotNull(result);
        }

        [Fact]
        public void LoadIconFromPath_NullPath_ReturnsNull()
        {
            var result = CustomIconLoader.LoadIconFromPath(null);
            Assert.Null(result);
        }

        [Fact]
        public void LoadIconFromPath_EmptyPath_ReturnsNull()
        {
            var result = CustomIconLoader.LoadIconFromPath("");
            Assert.Null(result);
        }

        [Fact]
        public void LoadIconFromPath_NonExistentPath_ReturnsNull()
        {
            var result = CustomIconLoader.LoadIconFromPath(@"C:\NonExistent\Icon.png");
            Assert.Null(result);
        }

        [Fact]
        public void LoadIconFromPath_SamePath_ReturnsCachedImage()
        {
            CreateTestIcon("cached.png");
            var path = Path.Combine(_testFolderPath, "cached.png");

            var result1 = CustomIconLoader.LoadIconFromPath(path);
            var result2 = CustomIconLoader.LoadIconFromPath(path);

            Assert.Same(result1, result2); // Should be same cached instance
        }

        #endregion

        #region LoadIconFromFolder Tests

        [Fact]
        public void LoadIconFromFolder_PngExists_ReturnsImage()
        {
            CreateTestIcon("test.png");

            var result = CustomIconLoader.LoadIconFromFolder(_testFolderPath, "test");

            Assert.NotNull(result);
        }

        [Fact]
        public void LoadIconFromFolder_NoMatchingFile_ReturnsNull()
        {
            var result = CustomIconLoader.LoadIconFromFolder(_testFolderPath, "nonexistent");

            Assert.Null(result);
        }

        [Fact]
        public void LoadIconFromFolder_NullFolder_ReturnsNull()
        {
            var result = CustomIconLoader.LoadIconFromFolder(null, "test");
            Assert.Null(result);
        }

        [Fact]
        public void LoadIconFromFolder_NullIconName_ReturnsNull()
        {
            var result = CustomIconLoader.LoadIconFromFolder(_testFolderPath, null);
            Assert.Null(result);
        }

        #endregion

        #region GetAvailableIcons Tests

        [Fact]
        public void GetAvailableIcons_WithIcons_ReturnsNames()
        {
            CreateTestIcon("S.png");
            CreateTestIcon("A.png");
            CreateTestIcon("B.png");

            var result = CustomIconLoader.GetAvailableIcons(_testFolderPath);

            Assert.Contains("S", result);
            Assert.Contains("A", result);
            Assert.Contains("B", result);
        }

        [Fact]
        public void GetAvailableIcons_EmptyFolder_ReturnsEmptyList()
        {
            var result = CustomIconLoader.GetAvailableIcons(_testFolderPath);

            Assert.Empty(result);
        }

        [Fact]
        public void GetAvailableIcons_NullPath_ReturnsEmptyList()
        {
            var result = CustomIconLoader.GetAvailableIcons(null);

            Assert.Empty(result);
        }

        [Fact]
        public void GetAvailableIcons_NonExistentPath_ReturnsEmptyList()
        {
            var result = CustomIconLoader.GetAvailableIcons(@"C:\NonExistent\Path");

            Assert.Empty(result);
        }

        #endregion

        #region ClearCache Tests

        [Fact]
        public void ClearCache_RemovesCachedIcons()
        {
            CreateTestIcon("test.png");
            var path = Path.Combine(_testFolderPath, "test.png");

            var result1 = CustomIconLoader.LoadIconFromPath(path);
            CustomIconLoader.ClearCache();
            var result2 = CustomIconLoader.LoadIconFromPath(path);

            Assert.NotSame(result1, result2); // Should be different instances after cache clear
        }

        [Fact]
        public void ClearCacheIfFolderChanged_DifferentFolder_ClearsCache()
        {
            CreateTestIcon("test.png");
            var path = Path.Combine(_testFolderPath, "test.png");

            var result1 = CustomIconLoader.LoadIconFromPath(path);
            CustomIconLoader.ClearCacheIfFolderChanged(_testFolderPath);
            CustomIconLoader.ClearCacheIfFolderChanged(@"C:\Different\Path");
            var result2 = CustomIconLoader.LoadIconFromPath(path);

            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void ClearCacheIfFolderChanged_SameFolder_KeepsCache()
        {
            CreateTestIcon("test.png");
            var path = Path.Combine(_testFolderPath, "test.png");

            CustomIconLoader.ClearCacheIfFolderChanged(_testFolderPath);
            var result1 = CustomIconLoader.LoadIconFromPath(path);
            CustomIconLoader.ClearCacheIfFolderChanged(_testFolderPath);
            var result2 = CustomIconLoader.LoadIconFromPath(path);

            Assert.Same(result1, result2);
        }

        #endregion

        #region GradeThreshold CustomIconPath Tests

        [Fact]
        public void GradeThreshold_CustomIconPath_DefaultsToNull()
        {
            var threshold = new GradeThreshold(50, "B", Color.Green);

            Assert.Null(threshold.CustomIconPath);
        }

        [Fact]
        public void GradeThreshold_Clone_CopiesCustomIconPath()
        {
            var original = new GradeThreshold(50, "B", Color.Green)
            {
                CustomIconPath = @"C:\Icons\B.png"
            };

            var clone = original.Clone();

            Assert.Equal(original.CustomIconPath, clone.CustomIconPath);
        }

        #endregion

        #region GradingSettings IconFolderPath Tests

        [Fact]
        public void GradingSettings_IconFolderPath_DefaultsToNull()
        {
            var settings = new GradingSettings();

            Assert.Null(settings.IconFolderPath);
        }

        [Fact]
        public void GradingSettings_Clone_CopiesIconFolderPath()
        {
            var original = new GradingSettings
            {
                IconFolderPath = @"C:\Icons"
            };

            var clone = original.Clone();

            Assert.Equal(original.IconFolderPath, clone.IconFolderPath);
        }

        #endregion
    }
}
