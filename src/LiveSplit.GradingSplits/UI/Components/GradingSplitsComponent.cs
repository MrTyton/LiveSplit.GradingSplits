using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.GradingSplits.Model;

namespace LiveSplit.GradingSplits.UI.Components
{
    public class GradingSplitsComponent : IComponent
    {
        protected GradingSplitsSettings Settings { get; set; }
        protected SimpleLabel Label { get; set; }
        protected SimpleLabel GradeLabel { get; set; }

        private int _lastGradedIndex = -1;
        private IRun _lastRun = null;
        private string _lastComparison = null;
        private LiveSplitState _state;

        // Cached statistics for graph rendering
        private List<double> _cachedHistory = new List<double>();
        private double _cachedMean = 0;
        private double _cachedStdDev = 0;
        private double _cachedComparisonTime = 0;
        private double _cachedZScore = 0;

        // Previous split comparison data
        private string _previousAchievedGrade = null;
        private Color _previousAchievedColor = Color.White;
        private string _previousComparisonGrade = null;
        private Color _previousComparisonColor = Color.White;
        private bool _hasPreviousSplitData = false;
        private Image _previousAchievedIcon = null;
        private Image _previousComparisonIcon = null;
        private int _lastPreviousSplitIndex = -1;  // Cache key for previous split data

        // Split name grading
        private Dictionary<int, string> _originalSplitNames = new Dictionary<int, string>();
        private bool _splitNamesModified = false;
        private IRun _gradedRun = null;
        private Dictionary<int, string> _splitNameGradeCache = new Dictionary<int, string>();  // Cache for split name grades
        private int _lastNameUpdateSplitIndex = -1;  // Track when split names need updating

        // Split icon grading
        private Dictionary<int, Image> _originalSplitIcons = new Dictionary<int, Image>();
        private bool _splitIconsModified = false;
        private int _lastIconUpdateSplitIndex = -1;
        private bool _lastShowGradeIconsSetting = false;
        private Dictionary<int, (string Grade, Color Color)> _splitIconCache = new Dictionary<int, (string, Color)>();

        // Grade calculation cache - keyed by (splitIndex, useActualTime)
        private Dictionary<(int Index, bool UseActualTime), (string Grade, Color Color)> _gradeCalculationCache = new Dictionary<(int, bool), (string, Color)>();
        private int _lastGradeCalcSplitIndex = -1;  // Track when grade cache needs invalidation

        // Cached current grade icon for component display
        private Image _currentGradeIcon = null;
        private string _lastIconGrade = null;
        private Color _lastIconColor = Color.White;

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Label.ActualWidth + GradeLabel.ActualWidth + 5;
        public float MinimumHeight => Settings.GradingConfig.ShowGraph ? 25 + Settings.GradingConfig.GraphHeight + 30 : 25;
        public float VerticalHeight
        {
            get
            {
                float height = 0f;

                // Base height for grade label (only if showing current grade)
                if (Settings.GradingConfig.ShowCurrentGrade)
                    height += 25f;

                if (Settings.GradingConfig.ShowGraph)
                {
                    height += Settings.GradingConfig.GraphHeight;

                    if (Settings.GradingConfig.ShowStatistics)
                        height += Settings.GradingConfig.StatisticsFontSize + 10;
                }

                if (Settings.GradingConfig.ShowPreviousSplit)
                    height += Settings.GradingConfig.PreviousSplitFontSize + 10;

                // Minimum height of 1 to prevent issues when everything is hidden
                return Math.Max(1f, height);
            }
        }
        public float MinimumWidth => 100;

        public float PaddingTop => 7f;
        public float PaddingBottom => 7f;
        public float PaddingLeft => 7f;
        public float PaddingRight => 7f;

        public IDictionary<string, Action> ContextMenuControls => null;

        public GradingSplitsComponent(LiveSplitState state)
        {
            _state = state;
            Settings = new GradingSplitsSettings();
            Label = new SimpleLabel()
            {
                Text = "Grade:",
                ForeColor = Color.White,
                VerticalAlignment = StringAlignment.Center,
                HorizontalAlignment = StringAlignment.Near
            };
            GradeLabel = new SimpleLabel()
            {
                Text = "-",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15, FontStyle.Bold, GraphicsUnit.Pixel),
                VerticalAlignment = StringAlignment.Center,
                HorizontalAlignment = StringAlignment.Near
            };

            // Subscribe to state events for split name management
            _state.OnReset += State_OnReset;
            _state.OnStart += State_OnStart;
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            // Only store original names if we haven't already for this run
            // This prevents storing already-modified names as "original"
            if (_originalSplitNames.Count == 0 || _gradedRun != _state.Run)
            {
                StoreOriginalSplitNames();
                StoreOriginalSplitIcons();
            }
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
            // Restore original names when run resets
            RestoreOriginalSplitNames();
            RestoreOriginalSplitIcons();

            // Clear all caches
            _splitNameGradeCache.Clear();
            _splitIconCache.Clear();
            _gradeCalculationCache.Clear();
            _lastNameUpdateSplitIndex = -1;
            _lastIconUpdateSplitIndex = -1;
            _lastPreviousSplitIndex = -1;
            _lastGradeCalcSplitIndex = -1;
            _hasPreviousSplitData = false;
        }

        private void StoreOriginalSplitNames()
        {
            if (_state.Run == null) return;

            // First restore any previously modified names before storing new originals
            if (_splitNamesModified && _gradedRun != null)
            {
                RestoreOriginalSplitNames();
            }

            _originalSplitNames.Clear();
            _gradedRun = _state.Run;

            for (int i = 0; i < _state.Run.Count; i++)
            {
                // Try to extract the original name in case it already has grade formatting
                string name = _state.Run[i].Name;
                string cleanedName = StripGradeFromName(name);
                _originalSplitNames[i] = cleanedName;

                // If we cleaned the name, also update the run to use the clean name
                if (cleanedName != name)
                {
                    _state.Run[i].Name = cleanedName;
                }
            }

            _splitNamesModified = false;
        }

        /// <summary>
        /// Attempts to strip grade formatting from a split name using the configured format.
        /// </summary>
        private string StripGradeFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            string format = Settings.GradingConfig.SplitNameFormat;
            if (string.IsNullOrEmpty(format) || !format.Contains("{Name}") || !format.Contains("{Grade}"))
                return name;

            // Get all possible grade labels
            var allGrades = new HashSet<string>();
            foreach (var threshold in Settings.GradingConfig.Thresholds)
            {
                if (!string.IsNullOrEmpty(threshold.Label))
                    allGrades.Add(threshold.Label);
            }
            if (Settings.GradingConfig.UseGoldGrade && !string.IsNullOrEmpty(Settings.GradingConfig.GoldLabel))
                allGrades.Add(Settings.GradingConfig.GoldLabel);
            if (Settings.GradingConfig.UseWorstGrade && !string.IsNullOrEmpty(Settings.GradingConfig.WorstLabel))
                allGrades.Add(Settings.GradingConfig.WorstLabel);

            if (allGrades.Count == 0) return name;

            // Try each grade to see if we can extract the original name
            foreach (var grade in allGrades)
            {
                string formatted = format.Replace("{Grade}", grade);
                // Now we have something like "{Name} [A]" or "[A] {Name}"

                // Find where {Name} is in the formatted string
                int nameIndex = formatted.IndexOf("{Name}");
                if (nameIndex < 0) continue;

                string prefix = formatted.Substring(0, nameIndex);
                string suffix = formatted.Substring(nameIndex + 6); // 6 = "{Name}".Length

                // Check if the current name matches this pattern
                if (name.StartsWith(prefix) && name.EndsWith(suffix) && name.Length > prefix.Length + suffix.Length)
                {
                    string extracted = name.Substring(prefix.Length, name.Length - prefix.Length - suffix.Length);
                    // Recursively strip in case multiple grades were appended
                    return StripGradeFromName(extracted);
                }
            }

            return name;
        }

        private void RestoreOriginalSplitNames()
        {
            if (!_splitNamesModified || _gradedRun == null) return;

            foreach (var kvp in _originalSplitNames)
            {
                if (kvp.Key < _gradedRun.Count)
                {
                    _gradedRun[kvp.Key].Name = kvp.Value;
                }
            }

            _originalSplitNames.Clear();
            _splitNamesModified = false;
            _gradedRun = null;
        }

        private void StoreOriginalSplitIcons()
        {
            if (_state.Run == null) return;

            // First restore any previously modified icons before storing new originals
            if (_splitIconsModified && _gradedRun != null)
            {
                RestoreOriginalSplitIcons();
            }

            _originalSplitIcons.Clear();

            for (int i = 0; i < _state.Run.Count; i++)
            {
                // Store the current icon (may be null)
                _originalSplitIcons[i] = _state.Run[i].Icon;
            }

            _splitIconsModified = false;
        }

        private void RestoreOriginalSplitIcons()
        {
            if (!_splitIconsModified || _gradedRun == null) return;

            foreach (var kvp in _originalSplitIcons)
            {
                if (kvp.Key < _gradedRun.Count)
                {
                    _gradedRun[kvp.Key].Icon = kvp.Value;
                }
            }

            _originalSplitIcons.Clear();
            _splitIconsModified = false;
        }

        private void UpdateSplitIconsWithGrades(LiveSplitState state)
        {
            if (state.Run == null) return;

            bool settingEnabled = Settings.GradingConfig.ShowGradeIcons;

            // If the feature is disabled, restore original icons if modified
            if (!settingEnabled)
            {
                if (_splitIconsModified)
                {
                    RestoreOriginalSplitIcons();
                    _splitIconCache.Clear();
                }
                _lastShowGradeIconsSetting = false;
                return;
            }

            // Check if we need to update icons
            bool needsFullUpdate = false;

            // Setting was just enabled
            if (!_lastShowGradeIconsSetting && settingEnabled)
            {
                needsFullUpdate = true;
            }
            // Run changed
            else if (_gradedRun != state.Run)
            {
                needsFullUpdate = true;
                _splitIconCache.Clear();
            }
            // Split index changed (a split was completed)
            else if (_lastIconUpdateSplitIndex != state.CurrentSplitIndex)
            {
                needsFullUpdate = true;
            }

            _lastShowGradeIconsSetting = settingEnabled;
            _lastIconUpdateSplitIndex = state.CurrentSplitIndex;

            if (!needsFullUpdate)
            {
                return; // Icons are up to date
            }

            // Make sure we have original icons stored
            if (_originalSplitIcons.Count == 0 || _gradedRun != state.Run)
            {
                StoreOriginalSplitIcons();
            }

            for (int i = 0; i < state.Run.Count; i++)
            {
                string grade;
                Color color;

                if (i < state.CurrentSplitIndex)
                {
                    // Completed split - show achieved grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: true);
                    grade = result.Grade;
                    color = result.Color;
                }
                else
                {
                    // Upcoming split - show comparison grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: false);
                    grade = result.Grade;
                    color = result.Color;
                }

                // Check if this split's grade changed from cached value
                bool gradeChanged = true;
                if (_splitIconCache.TryGetValue(i, out var cached))
                {
                    gradeChanged = cached.Grade != grade || cached.Color.ToArgb() != color.ToArgb();
                }

                if (gradeChanged)
                {
                    _splitIconCache[i] = (grade, color);

                    if (grade != "-" && !string.IsNullOrEmpty(grade))
                    {
                        // Generate and set the grade icon
                        state.Run[i].Icon = GradeIconGenerator.GenerateIcon(grade, color);
                        _splitIconsModified = true;
                    }
                    else
                    {
                        // Restore original icon for splits without grades
                        if (_originalSplitIcons.ContainsKey(i))
                        {
                            state.Run[i].Icon = _originalSplitIcons[i];
                        }
                    }
                }
            }
        }

        private void UpdateSplitNamesWithGrades(LiveSplitState state)
        {
            if (state.Run == null) return;

            // If the feature is disabled, restore original names if modified
            if (!Settings.GradingConfig.ShowGradeInSplitNames)
            {
                if (_splitNamesModified)
                {
                    RestoreOriginalSplitNames();
                    _splitNameGradeCache.Clear();
                }
                _lastNameUpdateSplitIndex = -1;
                return;
            }

            // Check if we need to update names
            bool needsUpdate = false;

            // Run changed
            if (_gradedRun != state.Run)
            {
                needsUpdate = true;
                _splitNameGradeCache.Clear();
            }
            // Split index changed (a split was completed)
            else if (_lastNameUpdateSplitIndex != state.CurrentSplitIndex)
            {
                needsUpdate = true;
            }

            _lastNameUpdateSplitIndex = state.CurrentSplitIndex;

            if (!needsUpdate)
            {
                return;  // Names are up to date
            }

            // Make sure we have original names stored
            if (_originalSplitNames.Count == 0 || _gradedRun != state.Run)
            {
                StoreOriginalSplitNames();
            }

            for (int i = 0; i < state.Run.Count; i++)
            {
                string originalName = _originalSplitNames.ContainsKey(i) ? _originalSplitNames[i] : state.Run[i].Name;
                string grade;

                if (i < state.CurrentSplitIndex)
                {
                    // Completed split - show achieved grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: true);
                    grade = result.Grade;
                }
                else
                {
                    // Upcoming split - show comparison grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: false);
                    grade = result.Grade;
                }

                // Check if grade changed from cached value
                bool gradeChanged = true;
                if (_splitNameGradeCache.TryGetValue(i, out var cachedGrade))
                {
                    gradeChanged = cachedGrade != grade;
                }

                if (gradeChanged)
                {
                    _splitNameGradeCache[i] = grade;

                    if (grade != "-" && !string.IsNullOrEmpty(grade))
                    {
                        // Use the customizable format string
                        string format = Settings.GradingConfig.SplitNameFormat;
                        if (string.IsNullOrEmpty(format))
                            format = "{Name} [{Grade}]";

                        state.Run[i].Name = format
                            .Replace("{Name}", originalName)
                            .Replace("{Grade}", grade);
                        _splitNamesModified = true;
                    }
                    else
                    {
                        state.Run[i].Name = originalName;
                    }
                }
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawSingle(g, state, HorizontalWidth, height, clipRegion, true);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawSingle(g, state, width, VerticalHeight, clipRegion, false);
        }

        private void DrawSingle(Graphics g, LiveSplitState state, float width, float height, Region clipRegion, bool horizontal)
        {
            var oldMatrix = g.Transform;

            // Update label text with current comparison
            Label.Text = state.CurrentComparison + "'s Grade:";

            // Basic layout
            Label.ForeColor = state.LayoutSettings.TextColor;
            Label.HasShadow = state.LayoutSettings.DropShadows;
            Label.ShadowColor = state.LayoutSettings.ShadowsColor;
            Label.Font = state.LayoutSettings.TextFont;

            // Determine if we're using icon or text display
            bool useIconDisplay = Settings.GradingConfig.CurrentGradeDisplayStyle == GradeDisplayStyle.Icon;
            float gradeDisplayWidth = useIconDisplay ? GradeIconGenerator.SmallIconSize : 0;

            // Use configurable font size for grade label (only needed for text display)
            using (var gradeFont = new Font(state.LayoutSettings.TextFont.FontFamily, Settings.GradingConfig.CurrentGradeFontSize, FontStyle.Bold))
            {
                GradeLabel.Font = gradeFont;
                GradeLabel.HasShadow = state.LayoutSettings.DropShadows;
                GradeLabel.ShadowColor = state.LayoutSettings.ShadowsColor;

                // Calculate sizes
                Label.SetActualWidth(g);
                if (!useIconDisplay)
                {
                    GradeLabel.SetActualWidth(g);
                    gradeDisplayWidth = GradeLabel.ActualWidth;
                }

                if (Settings.GradingConfig.ShowCurrentGrade)
                {
                    if (horizontal)
                    {
                        Label.X = 0;
                        Label.Y = 0;
                        Label.Width = Label.ActualWidth;
                        Label.Height = 25;
                        Label.Draw(g);

                        float gradeX = Label.ActualWidth + 5;
                        float gradeY = 0;

                        if (useIconDisplay && _currentGradeIcon != null)
                        {
                            // Draw small icon centered vertically in 25px row
                            float iconY = (25 - GradeIconGenerator.SmallIconSize) / 2f;
                            g.DrawImage(_currentGradeIcon, gradeX, iconY, GradeIconGenerator.SmallIconSize, GradeIconGenerator.SmallIconSize);
                        }
                        else
                        {
                            GradeLabel.X = gradeX;
                            GradeLabel.Y = gradeY;
                            GradeLabel.Width = gradeDisplayWidth;
                            GradeLabel.Height = 25;

                            if (Settings.GradingConfig.UseBackgroundColor)
                            {
                                using (var backgroundBrush = new SolidBrush(Settings.GradingConfig.BackgroundColor))
                                {
                                    g.FillRectangle(backgroundBrush, GradeLabel.X, GradeLabel.Y, GradeLabel.Width, GradeLabel.Height);
                                }
                            }

                            GradeLabel.Draw(g);
                        }
                    }
                    else
                    {
                        Label.X = 5;
                        Label.Y = 0;
                        Label.Width = width - gradeDisplayWidth - 10;
                        Label.Height = 25;
                        Label.Draw(g);

                        float gradeX = width - gradeDisplayWidth - 5;

                        if (useIconDisplay && _currentGradeIcon != null)
                        {
                            // Draw small icon centered vertically in 25px row
                            float iconY = (25 - GradeIconGenerator.SmallIconSize) / 2f;
                            g.DrawImage(_currentGradeIcon, gradeX, iconY, GradeIconGenerator.SmallIconSize, GradeIconGenerator.SmallIconSize);
                        }
                        else
                        {
                            GradeLabel.X = gradeX;
                            GradeLabel.Y = 0;
                            GradeLabel.Width = gradeDisplayWidth;
                            GradeLabel.Height = 25;

                            if (Settings.GradingConfig.UseBackgroundColor)
                            {
                                using (var backgroundBrush = new SolidBrush(Settings.GradingConfig.BackgroundColor))
                                {
                                    g.FillRectangle(backgroundBrush, GradeLabel.X, GradeLabel.Y, GradeLabel.Width, GradeLabel.Height);
                                }
                            }

                            GradeLabel.Draw(g);
                        }
                    }
                }
            }

            // Calculate starting Y position for elements below grade row
            float baseY = Settings.GradingConfig.ShowCurrentGrade ? 25f : 0f;

            // Draw graph if enabled
            if (Settings.GradingConfig.ShowGraph && _cachedHistory.Count >= 2)
            {
                float graphY = baseY; // Position graph below the grade label (or at top if grade hidden)
                DrawDistributionGraph(g, state, width, Settings.GradingConfig.GraphHeight, graphY);

                // Position for elements below the graph
                float belowGraphY = graphY + Settings.GradingConfig.GraphHeight + 2;

                // Statistics first, then previous split
                if (Settings.GradingConfig.ShowStatistics)
                {
                    DrawStatistics(g, state, width, belowGraphY);
                    belowGraphY += Settings.GradingConfig.StatisticsFontSize + 10;
                }

                if (Settings.GradingConfig.ShowPreviousSplit)
                {
                    DrawPreviousSplitComparison(g, state, width, belowGraphY);
                }
            }
            else
            {
                // No graph - just draw previous split if enabled
                if (Settings.GradingConfig.ShowPreviousSplit)
                {
                    float prevY = baseY;
                    DrawPreviousSplitComparison(g, state, width, prevY);
                }
            }

            g.Transform = oldMatrix;
        }

        private void DrawPreviousSplitComparison(Graphics g, LiveSplitState state, float width, float yOffset)
        {
            using (var font = new Font(state.LayoutSettings.TextFont.FontFamily, Settings.GradingConfig.PreviousSplitFontSize, FontStyle.Regular))
            using (var textBrush = new SolidBrush(state.LayoutSettings.TextColor))
            {
                // If no previous split data, show default text
                if (!_hasPreviousSplitData)
                {
                    string defaultText = "Previous: N/A";
                    var defaultSize = g.MeasureString(defaultText, font);
                    float defaultX = (width - defaultSize.Width) / 2;
                    g.DrawString(defaultText, font, textBrush, defaultX, yOffset);
                    return;
                }

                // Build the text parts
                string prefix = "Previous: Achieved ";
                string vs = " vs " + state.CurrentComparison + "'s ";

                // Trim grades for proper spacing
                string achievedDisplay = _previousAchievedGrade?.Trim() ?? "-";
                string comparisonDisplay = _previousComparisonGrade?.Trim() ?? "-";

                // Determine if we're using icons
                bool useIcons = Settings.GradingConfig.ShowGradeIcons && _previousAchievedIcon != null && _previousComparisonIcon != null;
                int iconSize = GradeIconGenerator.SmallIconSize;
                float iconPadding = 2f;

                // Measure each part to position them
                var prefixSize = g.MeasureString(prefix, font);
                var achievedSize = useIcons ? new SizeF(iconSize + iconPadding, font.Height) : g.MeasureString(achievedDisplay, font);
                var vsSize = g.MeasureString(vs, font);
                var comparisonSize = useIcons ? new SizeF(iconSize + iconPadding, font.Height) : g.MeasureString(comparisonDisplay, font);

                float totalWidth = prefixSize.Width + achievedSize.Width + vsSize.Width + comparisonSize.Width;
                float startX = (width - totalWidth) / 2;
                float currentX = startX;

                // Calculate vertical centering for icons
                float iconY = yOffset + (font.Height - iconSize) / 2;

                // Draw prefix
                g.DrawString(prefix, font, textBrush, currentX, yOffset);
                currentX += prefixSize.Width;

                // Draw achieved grade (icon or text)
                if (useIcons)
                {
                    g.DrawImage(_previousAchievedIcon, currentX, iconY, iconSize, iconSize);
                    currentX += iconSize + iconPadding;
                }
                else
                {
                    using (var achievedBrush = new SolidBrush(_previousAchievedColor))
                    {
                        g.DrawString(achievedDisplay, font, achievedBrush, currentX, yOffset);
                    }
                    currentX += achievedSize.Width;
                }

                // Draw "vs comparison's"
                g.DrawString(vs, font, textBrush, currentX, yOffset);
                currentX += vsSize.Width;

                // Draw comparison grade (icon or text)
                if (useIcons)
                {
                    g.DrawImage(_previousComparisonIcon, currentX, iconY, iconSize, iconSize);
                }
                else
                {
                    using (var comparisonBrush = new SolidBrush(_previousComparisonColor))
                    {
                        g.DrawString(comparisonDisplay, font, comparisonBrush, currentX, yOffset);
                    }
                }
            }
        }

        private void DrawDistributionGraph(Graphics g, LiveSplitState state, float width, float height, float yOffset)
        {
            if (_cachedHistory.Count < 2 || _cachedStdDev == 0)
                return;

            var graphRect = new RectangleF(5, yOffset, width - 10, height);

            // Draw background
            using (var bgBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            {
                g.FillRectangle(bgBrush, graphRect);
            }

            // Calculate range for x-axis (mean ± 3 standard deviations)
            double minValue = _cachedMean - 3 * _cachedStdDev;
            double maxValue = _cachedMean + 3 * _cachedStdDev;
            double range = maxValue - minValue;

            if (range <= 0)
                return;

            // Draw normal distribution curve
            var curvePoints = new List<PointF>();
            int numPoints = (int)graphRect.Width;
            for (int i = 0; i < numPoints; i++)
            {
                float x = graphRect.X + i;
                double value = minValue + (i / (double)numPoints) * range;
                double probability = NormalDistribution(value, _cachedMean, _cachedStdDev);

                // Scale to graph height
                double maxProbability = NormalDistribution(_cachedMean, _cachedMean, _cachedStdDev);
                float y = graphRect.Y + graphRect.Height - (float)(probability / maxProbability * graphRect.Height * 0.9);

                curvePoints.Add(new PointF(x, y));
            }

            if (curvePoints.Count > 1)
            {
                using (var curvePen = new Pen(Color.LightBlue, 2))
                {
                    g.DrawLines(curvePen, curvePoints.ToArray());
                }
            }

            // Draw historical points
            float dotSize = 4;

            if (Settings.GradingConfig.UseHistogramGraph)
            {
                // Histogram mode: dots stacked vertically in bins
                int numBins = Math.Min(30, (int)(graphRect.Width / 6)); // Each bin ~6 pixels wide
                var bins = new int[numBins];
                float binWidth = graphRect.Width / numBins;

                // Count values in each bin
                foreach (var histValue in _cachedHistory)
                {
                    if (histValue >= minValue && histValue <= maxValue)
                    {
                        int binIndex = (int)((histValue - minValue) / range * (numBins - 1));
                        binIndex = Math.Max(0, Math.Min(numBins - 1, binIndex));
                        bins[binIndex]++;
                    }
                }

                // Find max bin count for scaling
                int maxBinCount = bins.Max();
                if (maxBinCount == 0) maxBinCount = 1;

                // Calculate available height for dots (leave some margin at top and bottom)
                float availableHeight = graphRect.Height - 18; // 10px top margin, 8px bottom margin
                float dotSpacing = dotSize + 1;

                // Calculate how many dots can fit without scaling
                int maxDotsUnscaled = (int)(availableHeight / dotSpacing);

                // Determine if we need to scale
                float scaleFactor = 1.0f;
                if (maxBinCount > maxDotsUnscaled)
                {
                    // Scale the spacing so all dots fit
                    scaleFactor = availableHeight / (maxBinCount * dotSpacing);
                    dotSpacing *= scaleFactor;
                    dotSize *= scaleFactor;
                    // Ensure minimum visibility
                    if (dotSize < 2) dotSize = 2;
                    if (dotSpacing < dotSize + 0.5f) dotSpacing = dotSize + 0.5f;
                }

                // Draw dots for each bin, stacked vertically
                for (int bin = 0; bin < numBins; bin++)
                {
                    if (bins[bin] > 0)
                    {
                        float binCenterX = graphRect.X + bin * binWidth + binWidth / 2;

                        // Stack dots from bottom up
                        using (var dotBrush = new SolidBrush(Color.Yellow))
                        {
                            for (int dotIndex = 0; dotIndex < bins[bin]; dotIndex++)
                            {
                                float dotY = graphRect.Y + graphRect.Height - 8 - (dotIndex * dotSpacing);

                                // Safety check - don't draw if we'd go above the graph area
                                if (dotY < graphRect.Y + 5)
                                    break;

                                g.FillEllipse(dotBrush,
                                    binCenterX - dotSize / 2,
                                    dotY - dotSize / 2,
                                    dotSize,
                                    dotSize);
                            }
                        }
                    }
                }
            }
            else
            {
                // Scatter mode: dots on a single line
                using (var dotBrush = new SolidBrush(Color.Yellow))
                {
                    foreach (var histValue in _cachedHistory)
                    {
                        if (histValue >= minValue && histValue <= maxValue)
                        {
                            float x = graphRect.X + (float)((histValue - minValue) / range * graphRect.Width);
                            g.FillEllipse(dotBrush, x - dotSize / 2, graphRect.Y + graphRect.Height - 10, dotSize, dotSize);
                        }
                    }
                }
            }

            // Draw current comparison line
            if (_cachedComparisonTime >= minValue && _cachedComparisonTime <= maxValue)
            {
                float x = graphRect.X + (float)((_cachedComparisonTime - minValue) / range * graphRect.Width);
                using (var comparisonPen = new Pen(GradeLabel.ForeColor, 2))
                {
                    g.DrawLine(comparisonPen, x, graphRect.Y, x, graphRect.Y + graphRect.Height);
                }
            }

            // Draw mean line
            float meanX = graphRect.X + (float)((_cachedMean - minValue) / range * graphRect.Width);
            using (var meanPen = new Pen(Color.White, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawLine(meanPen, meanX, graphRect.Y, meanX, graphRect.Y + graphRect.Height);
            }
        }

        private double NormalDistribution(double x, double mean, double stdDev)
        {
            double exponent = -Math.Pow(x - mean, 2) / (2 * Math.Pow(stdDev, 2));
            return (1 / (stdDev * Math.Sqrt(2 * Math.PI))) * Math.Exp(exponent);
        }

        private void DrawStatistics(Graphics g, LiveSplitState state, float width, float yOffset)
        {
            using (var font = new Font(state.LayoutSettings.TextFont.FontFamily, Settings.GradingConfig.StatisticsFontSize, FontStyle.Regular))
            using (var brush = new SolidBrush(state.LayoutSettings.TextColor))
            {
                // Calculate percentile from z-score
                double percentile = Statistics.ZScoreToPercentile(_cachedZScore);

                string statsText = $"Average: {TimeSpan.FromSeconds(_cachedMean):mm\\:ss\\.ff}  Percentile: {percentile:F1}%  n={_cachedHistory.Count}";

                var textSize = g.MeasureString(statsText, font);
                float x = (width - textSize.Width) / 2;

                g.DrawString(statsText, font, brush, x, yOffset);
            }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            UpdateGrade(state);
            UpdateSplitNamesWithGrades(state);
            UpdateSplitIconsWithGrades(state);

            if (invalidator != null)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        private void UpdateGrade(LiveSplitState state)
        {
            int index = state.CurrentSplitIndex;

            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                index = 0;
            }

            if (index < 0 || index >= state.Run.Count)
            {
                GradeLabel.Text = "-";
                GradeLabel.ForeColor = state.LayoutSettings.TextColor;
                return;
            }

            // Check if we need to recalculate
            // We should recalculate if the index changed, or if the run changed (e.g. reset/loaded new splits)
            // Or if the comparison changed.
            if (index == _lastGradedIndex && state.Run == _lastRun && state.CurrentComparison == _lastComparison)
            {
                return;
            }

            _lastGradedIndex = index;
            _lastRun = state.Run;
            _lastComparison = state.CurrentComparison;

            var gradeResult = CalculateGradeForSplit(state, index);
            GradeLabel.Text = gradeResult.Grade;
            GradeLabel.ForeColor = gradeResult.Color;

            // Update cached icon if grade or color changed
            if (_lastIconGrade != gradeResult.Grade || _lastIconColor != gradeResult.Color)
            {
                _lastIconGrade = gradeResult.Grade;
                _lastIconColor = gradeResult.Color;
                _currentGradeIcon?.Dispose();
                // Use small icon for the current grade display row (better fit in 25px row height)
                _currentGradeIcon = gradeResult.Grade != "-" 
                    ? GradeIconGenerator.GenerateSmallIcon(gradeResult.Grade, gradeResult.Color)
                    : null;
            }

            // Calculate previous split comparison data
            UpdatePreviousSplitData(state, index);
        }

        private void UpdatePreviousSplitData(LiveSplitState state, int currentIndex)
        {
            // Need at least one previous split
            if (currentIndex <= 0)
            {
                _hasPreviousSplitData = false;
                _lastPreviousSplitIndex = -1;
                return;
            }

            // Check if we already have cached data for this split
            if (_lastPreviousSplitIndex == currentIndex - 1 && _hasPreviousSplitData)
            {
                return;  // Use cached data
            }

            _hasPreviousSplitData = false;

            int prevIndex = currentIndex - 1;
            _lastPreviousSplitIndex = prevIndex;
            var prevSegment = state.Run[prevIndex];
            var method = state.CurrentTimingMethod;

            // Get the actual achieved time for the previous split
            var prevSplitTime = prevSegment.SplitTime[method];
            var prevPrevSplitTime = prevIndex > 0 ? state.Run[prevIndex - 1].SplitTime[method] : TimeSpan.Zero;

            TimeSpan? achievedSegmentTime = null;
            if (prevSplitTime.HasValue)
            {
                achievedSegmentTime = prevSplitTime.Value - (prevPrevSplitTime ?? TimeSpan.Zero);
            }

            // Get the comparison time for the previous split
            var comparison = state.CurrentComparison;
            var comparisonSplitTime = prevSegment.Comparisons[comparison][method];
            var comparisonPrevSplitTime = prevIndex > 0 ? state.Run[prevIndex - 1].Comparisons[comparison][method] : TimeSpan.Zero;

            TimeSpan? comparisonSegmentTime = null;
            if (comparisonSplitTime.HasValue)
            {
                comparisonSegmentTime = comparisonSplitTime.Value - (comparisonPrevSplitTime ?? TimeSpan.Zero);
            }

            // Calculate statistics for the previous segment
            var history = new List<double>();
            foreach (var historyItem in prevSegment.SegmentHistory)
            {
                var time = historyItem.Value[method];
                if (time.HasValue)
                {
                    history.Add(time.Value.TotalSeconds);
                }
            }

            if (history.Count < 2)
                return;

            var mean = Statistics.CalculateMean(history);
            var stdDev = Statistics.CalculateStandardDeviation(history);

            if (stdDev == 0)
                return;

            // Calculate achieved grade if we have the time
            if (achievedSegmentTime.HasValue)
            {
                double achievedSeconds = achievedSegmentTime.Value.TotalSeconds;
                double achievedZScore = Statistics.CalculateZScore(achievedSeconds, mean, stdDev);

                // Check for gold/worst using helper methods
                var bestSegment = prevSegment.BestSegmentTime[method];
                bool isGold = GradeCalculator.IsGoldSplit(achievedSeconds, bestSegment?.TotalSeconds);
                bool isWorst = GradeCalculator.IsWorstSplit(achievedSeconds, history);

                var achievedResult = GradeCalculator.CalculateGrade(achievedZScore, Settings.GradingConfig, isGold, isWorst);
                _previousAchievedGrade = achievedResult.Grade;
                _previousAchievedColor = achievedResult.Color;

                // Generate small icon for the achieved grade
                _previousAchievedIcon?.Dispose();
                _previousAchievedIcon = GradeIconGenerator.GenerateSmallIcon(achievedResult.Grade.Trim(), achievedResult.Color);
            }
            else
            {
                _previousAchievedGrade = "-";
                _previousAchievedColor = state.LayoutSettings.TextColor;
                _previousAchievedIcon?.Dispose();
                _previousAchievedIcon = null;
            }

            // Calculate comparison grade if we have the time
            if (comparisonSegmentTime.HasValue)
            {
                double comparisonSeconds = comparisonSegmentTime.Value.TotalSeconds;
                double comparisonZScore = Statistics.CalculateZScore(comparisonSeconds, mean, stdDev);

                // Check for gold/worst using helper methods
                var bestSegment = prevSegment.BestSegmentTime[method];
                bool isGold = GradeCalculator.IsGoldSplit(comparisonSeconds, bestSegment?.TotalSeconds);
                bool isWorst = GradeCalculator.IsWorstSplit(comparisonSeconds, history);

                var comparisonResult = GradeCalculator.CalculateGrade(comparisonZScore, Settings.GradingConfig, isGold, isWorst);
                _previousComparisonGrade = comparisonResult.Grade;
                _previousComparisonColor = comparisonResult.Color;

                // Generate small icon for the comparison grade
                _previousComparisonIcon?.Dispose();
                _previousComparisonIcon = GradeIconGenerator.GenerateSmallIcon(comparisonResult.Grade.Trim(), comparisonResult.Color);
            }
            else
            {
                _previousComparisonGrade = "-";
                _previousComparisonColor = state.LayoutSettings.TextColor;
                _previousComparisonIcon?.Dispose();
                _previousComparisonIcon = null;
            }

            _hasPreviousSplitData = true;
        }

        private (string Grade, Color Color) CalculateGradeForSplit(LiveSplitState state, int index)
        {
            return CalculateGradeForSplit(state, index, useActualTime: false);
        }

        private (string Grade, Color Color) CalculateGradeForSplit(LiveSplitState state, int index, bool useActualTime)
        {
            if (index < 0 || index >= state.Run.Count)
            {
                _cachedHistory.Clear();
                return ("-", state.LayoutSettings.TextColor);
            }

            // Check if cache needs to be invalidated (split completed or run changed)
            if (_lastGradeCalcSplitIndex != state.CurrentSplitIndex || _gradedRun != state.Run)
            {
                _gradeCalculationCache.Clear();
                _lastGradeCalcSplitIndex = state.CurrentSplitIndex;
            }

            // Check cache first
            var cacheKey = (index, useActualTime);
            if (_gradeCalculationCache.TryGetValue(cacheKey, out var cachedResult))
            {
                // For the current split being viewed (used by graph), we still need to populate the cached stats
                // Only return cached result if this isn't the primary display split
                if (index != _lastGradedIndex)
                {
                    return cachedResult;
                }
            }

            var segment = state.Run[index];
            var method = state.CurrentTimingMethod;

            // Calculate stats for this segment based on history
            _cachedHistory = new List<double>();
            foreach (var historyItem in segment.SegmentHistory)
            {
                var time = historyItem.Value[method];
                if (time.HasValue)
                {
                    _cachedHistory.Add(time.Value.TotalSeconds);
                }
            }

            if (_cachedHistory.Count < 2)
            {
                _cachedHistory.Clear();
                var noDataResult = ("-", state.LayoutSettings.TextColor);
                _gradeCalculationCache[cacheKey] = noDataResult;
                return noDataResult;
            }

            _cachedMean = Statistics.CalculateMean(_cachedHistory);
            _cachedStdDev = Statistics.CalculateStandardDeviation(_cachedHistory);

            if (_cachedStdDev == 0)
            {
                _cachedHistory.Clear();
                var noStdDevResult = ("-", state.LayoutSettings.TextColor);
                _gradeCalculationCache[cacheKey] = noStdDevResult;
                return noStdDevResult;
            }

            // Get the segment time - either actual or comparison based on parameter
            TimeSpan? segmentTime = null;

            if (useActualTime)
            {
                // Use the actual segment time from the current run
                var splitTime = segment.SplitTime[method];
                var prevSplitTime = index > 0 ? state.Run[index - 1].SplitTime[method] : TimeSpan.Zero;

                if (splitTime != null && prevSplitTime != null)
                {
                    segmentTime = splitTime - prevSplitTime;
                }
            }
            else
            {
                // Use comparison segment time
                var comparison = state.CurrentComparison;
                var splitTime = segment.Comparisons[comparison][method];
                var prevSplitTime = index > 0 ? state.Run[index - 1].Comparisons[comparison][method] : TimeSpan.Zero;

                if (splitTime != null && prevSplitTime != null)
                {
                    segmentTime = splitTime - prevSplitTime;
                }
            }

            if (segmentTime != null)
            {
                _cachedComparisonTime = segmentTime.Value.TotalSeconds;
                _cachedZScore = Statistics.CalculateZScore(_cachedComparisonTime, _cachedMean, _cachedStdDev);

                // Check if this is a gold or worst split using helper methods
                var bestSegment = segment.BestSegmentTime[method];
                bool isGoldSplit = GradeCalculator.IsGoldSplit(_cachedComparisonTime, bestSegment?.TotalSeconds);
                bool isWorstSplit = GradeCalculator.IsWorstSplit(_cachedComparisonTime, _cachedHistory);

                var gradeResult = GradeCalculator.CalculateGrade(_cachedZScore, Settings.GradingConfig, isGoldSplit, isWorstSplit);
                _gradeCalculationCache[cacheKey] = gradeResult;
                return gradeResult;
            }

            var noTimeResult = ("-", state.LayoutSettings.TextColor);
            _gradeCalculationCache[cacheKey] = noTimeResult;
            return noTimeResult;
        }

        public void Dispose()
        {
            // Restore original split names and icons before disposing
            RestoreOriginalSplitNames();
            RestoreOriginalSplitIcons();

            // Dispose cached icons
            _currentGradeIcon?.Dispose();
            _currentGradeIcon = null;
            _previousAchievedIcon?.Dispose();
            _previousAchievedIcon = null;
            _previousComparisonIcon?.Dispose();
            _previousComparisonIcon = null;

            // Unsubscribe from events
            if (_state != null)
            {
                _state.OnReset -= State_OnReset;
                _state.OnStart -= State_OnStart;
            }

            Settings.Dispose();
        }
    }
}
