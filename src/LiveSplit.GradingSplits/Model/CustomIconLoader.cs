using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LiveSplit.GradingSplits.Model
{
    /// <summary>
    /// Handles loading custom grade icons from files.
    /// Supports both folder-based loading (by grade label) and per-threshold custom paths.
    /// </summary>
    public static class CustomIconLoader
    {
        /// <summary>
        /// The special filename (without extension) for best/gold split icons.
        /// </summary>
        public const string BestIconName = "Best";

        /// <summary>
        /// The special filename (without extension) for worst split icons.
        /// </summary>
        public const string WorstIconName = "Worst";

        /// <summary>
        /// Supported image file extensions.
        /// </summary>
        private static readonly string[] SupportedExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico" };

        /// <summary>
        /// Cache of loaded custom icons to avoid repeated file reads.
        /// Key is the full file path.
        /// </summary>
        private static readonly Dictionary<string, Image> _iconCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The last folder path that was scanned.
        /// </summary>
        private static string _lastFolderPath;

        /// <summary>
        /// Attempts to load a custom icon for a grade.
        /// Priority: 1) Threshold's CustomIconPath, 2) Folder-based icon, 3) null (use generated)
        /// </summary>
        /// <param name="gradeLabel">The grade label (e.g., "S", "A").</param>
        /// <param name="settings">The grading settings containing folder path.</param>
        /// <param name="threshold">Optional threshold with potential custom icon path.</param>
        /// <param name="isGold">Whether this is a gold/best split.</param>
        /// <param name="isWorst">Whether this is a worst split.</param>
        /// <returns>The custom icon image, or null if none found.</returns>
        public static Image GetCustomIcon(string gradeLabel, GradingSettings settings, GradeThreshold threshold = null, bool isGold = false, bool isWorst = false)
        {
            // Priority 1: Per-threshold custom icon path
            if (threshold != null && !string.IsNullOrEmpty(threshold.CustomIconPath))
            {
                var icon = LoadIconFromPath(threshold.CustomIconPath);
                if (icon != null) return icon;
            }

            // Priority 2: Folder-based icons
            if (!string.IsNullOrEmpty(settings.IconFolderPath) && Directory.Exists(settings.IconFolderPath))
            {
                // Check for special Best/Worst icons first
                if (isGold)
                {
                    var bestIcon = LoadIconFromFolder(settings.IconFolderPath, BestIconName);
                    if (bestIcon != null) return bestIcon;
                }

                if (isWorst)
                {
                    var worstIcon = LoadIconFromFolder(settings.IconFolderPath, WorstIconName);
                    if (worstIcon != null) return worstIcon;
                }

                // Fall back to grade label icon
                var gradeIcon = LoadIconFromFolder(settings.IconFolderPath, gradeLabel);
                if (gradeIcon != null) return gradeIcon;
            }

            // Priority 3: No custom icon found
            return null;
        }

        /// <summary>
        /// Loads an icon from a specific file path.
        /// </summary>
        /// <param name="filePath">The full path to the icon file.</param>
        /// <returns>The loaded image, or null if loading failed.</returns>
        public static Image LoadIconFromPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                // Check cache first
                if (_iconCache.TryGetValue(filePath, out var cached))
                    return cached;

                // Load and cache the icon
                var image = Image.FromFile(filePath);
                _iconCache[filePath] = image;
                return image;
            }
            catch
            {
                // Failed to load image
                return null;
            }
        }

        /// <summary>
        /// Loads an icon from a folder by name (tries all supported extensions).
        /// </summary>
        /// <param name="folderPath">The folder containing icons.</param>
        /// <param name="iconName">The icon name without extension.</param>
        /// <returns>The loaded image, or null if not found.</returns>
        public static Image LoadIconFromFolder(string folderPath, string iconName)
        {
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(iconName))
                return null;

            foreach (var ext in SupportedExtensions)
            {
                var filePath = Path.Combine(folderPath, iconName + ext);
                var icon = LoadIconFromPath(filePath);
                if (icon != null) return icon;
            }

            return null;
        }

        /// <summary>
        /// Clears the icon cache. Call when settings change or icons may have been modified.
        /// </summary>
        public static void ClearCache()
        {
            foreach (var icon in _iconCache.Values)
            {
                icon?.Dispose();
            }
            _iconCache.Clear();
            _lastFolderPath = null;
        }

        /// <summary>
        /// Clears the cache if the folder path has changed.
        /// </summary>
        /// <param name="newFolderPath">The current folder path setting.</param>
        public static void ClearCacheIfFolderChanged(string newFolderPath)
        {
            if (_lastFolderPath != newFolderPath)
            {
                ClearCache();
                _lastFolderPath = newFolderPath;
            }
        }

        /// <summary>
        /// Gets a list of icon files found in a folder.
        /// </summary>
        /// <param name="folderPath">The folder to scan.</param>
        /// <returns>A list of found icon names (without extensions).</returns>
        public static List<string> GetAvailableIcons(string folderPath)
        {
            var icons = new List<string>();
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                return icons;

            try
            {
                foreach (var ext in SupportedExtensions)
                {
                    foreach (var file in Directory.GetFiles(folderPath, "*" + ext))
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (!icons.Contains(name))
                            icons.Add(name);
                    }
                }
            }
            catch
            {
                // Ignore folder access errors
            }

            return icons;
        }
    }
}
