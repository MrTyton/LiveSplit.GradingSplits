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

        // Split name grading
        private Dictionary<int, string> _originalSplitNames = new Dictionary<int, string>();
        private bool _splitNamesModified = false;
        private IRun _gradedRun = null;

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Label.ActualWidth + GradeLabel.ActualWidth + 5;
        public float MinimumHeight => Settings.GradingConfig.ShowGraph ? 25 + Settings.GradingConfig.GraphHeight + 30 : 25;
        public float VerticalHeight
        {
            get
            {
                float height = 25f; // Base height for grade label

                if (Settings.GradingConfig.ShowGraph)
                {
                    height += Settings.GradingConfig.GraphHeight;

                    if (Settings.GradingConfig.ShowStatistics)
                        height += Settings.GradingConfig.StatisticsFontSize + 10;
                }

                if (Settings.GradingConfig.ShowPreviousSplit)
                    height += Settings.GradingConfig.PreviousSplitFontSize + 10;

                return height;
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
            }
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
            // Restore original names when run resets
            RestoreOriginalSplitNames();
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
                _originalSplitNames[i] = _state.Run[i].Name;
            }

            _splitNamesModified = false;
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

        private void UpdateSplitNamesWithGrades(LiveSplitState state)
        {
            if (state.Run == null) return;

            // If the feature is disabled, restore original names if modified
            if (!Settings.GradingConfig.ShowGradeInSplitNames)
            {
                if (_splitNamesModified)
                {
                    RestoreOriginalSplitNames();
                }
                return;
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
            GradeLabel.Font = state.LayoutSettings.TextFont;

            GradeLabel.HasShadow = state.LayoutSettings.DropShadows;
            GradeLabel.ShadowColor = state.LayoutSettings.ShadowsColor;

            // Calculate sizes
            Label.SetActualWidth(g);
            GradeLabel.SetActualWidth(g);

            if (horizontal)
            {
                Label.X = 0;
                Label.Y = 0;
                Label.Width = Label.ActualWidth;
                Label.Height = 25; // Fixed height for label at top
                Label.Draw(g);

                GradeLabel.X = Label.ActualWidth + 5;
                GradeLabel.Y = 0;
                GradeLabel.Width = GradeLabel.ActualWidth;
                GradeLabel.Height = 25; // Fixed height for grade at top

                // Draw background for grade label only if enabled
                if (Settings.GradingConfig.UseBackgroundColor)
                {
                    var backgroundBrush = new SolidBrush(Settings.GradingConfig.BackgroundColor);
                    g.FillRectangle(backgroundBrush, GradeLabel.X, GradeLabel.Y, GradeLabel.Width, GradeLabel.Height);
                    backgroundBrush.Dispose();
                }

                GradeLabel.Draw(g);
            }
            else
            {
                Label.X = 5;
                Label.Y = 0;
                Label.Width = width - GradeLabel.ActualWidth - 10;
                Label.Height = 25; // Fixed height for label at top
                Label.Draw(g);

                GradeLabel.X = width - GradeLabel.ActualWidth - 5;
                GradeLabel.Y = 0;
                GradeLabel.Width = GradeLabel.ActualWidth;
                GradeLabel.Height = 25; // Fixed height for grade at top

                // Draw background for grade label only if enabled
                if (Settings.GradingConfig.UseBackgroundColor)
                {
                    var backgroundBrush = new SolidBrush(Settings.GradingConfig.BackgroundColor);
                    g.FillRectangle(backgroundBrush, GradeLabel.X, GradeLabel.Y, GradeLabel.Width, GradeLabel.Height);
                    backgroundBrush.Dispose();
                }

                GradeLabel.Draw(g);
            }

            // Draw graph if enabled
            if (Settings.GradingConfig.ShowGraph && _cachedHistory.Count >= 2)
            {
                float graphY = 25; // Position graph below the grade label
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
                    float prevY = 25;
                    DrawPreviousSplitComparison(g, state, width, prevY);
                }
            }

            g.Transform = oldMatrix;
        }

        private void DrawPreviousSplitComparison(Graphics g, LiveSplitState state, float width, float yOffset)
        {
            var font = new Font(state.LayoutSettings.TextFont.FontFamily, Settings.GradingConfig.PreviousSplitFontSize, FontStyle.Regular);
            var textBrush = new SolidBrush(state.LayoutSettings.TextColor);

            // If no previous split data, show default text
            if (!_hasPreviousSplitData)
            {
                string defaultText = "Previous: N/A";
                var defaultSize = g.MeasureString(defaultText, font);
                float defaultX = (width - defaultSize.Width) / 2;
                g.DrawString(defaultText, font, textBrush, defaultX, yOffset);
                font.Dispose();
                textBrush.Dispose();
                return;
            }

            // Build the text parts - no extra space before "vs"
            string prefix = "Previous: Achieved ";
            string vs = " vs " + state.CurrentComparison + "'s ";

            // Trim grades for proper spacing
            string achievedDisplay = _previousAchievedGrade?.Trim() ?? "-";
            string comparisonDisplay = _previousComparisonGrade?.Trim() ?? "-";

            // Measure each part to position them
            var prefixSize = g.MeasureString(prefix, font);
            var achievedSize = g.MeasureString(achievedDisplay, font);
            var vsSize = g.MeasureString(vs, font);
            var comparisonSize = g.MeasureString(comparisonDisplay, font);

            float totalWidth = prefixSize.Width + achievedSize.Width + vsSize.Width + comparisonSize.Width;
            float startX = (width - totalWidth) / 2;
            float currentX = startX;

            // Draw prefix
            g.DrawString(prefix, font, textBrush, currentX, yOffset);
            currentX += prefixSize.Width;

            // Draw achieved grade with its color
            using (var achievedBrush = new SolidBrush(_previousAchievedColor))
            {
                g.DrawString(achievedDisplay, font, achievedBrush, currentX, yOffset);
            }
            currentX += achievedSize.Width;

            // Draw "vs comparison's"
            g.DrawString(vs, font, textBrush, currentX, yOffset);
            currentX += vsSize.Width;

            // Draw comparison grade with its color
            using (var comparisonBrush = new SolidBrush(_previousComparisonColor))
            {
                g.DrawString(comparisonDisplay, font, comparisonBrush, currentX, yOffset);
            }

            font.Dispose();
            textBrush.Dispose();
        }

        private void DrawDistributionGraph(Graphics g, LiveSplitState state, float width, float height, float yOffset)
        {
            if (_cachedHistory.Count < 2 || _cachedStdDev == 0)
                return;

            var graphRect = new RectangleF(5, yOffset, width - 10, height);

            // Draw background
            g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), graphRect);

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
                g.DrawLines(new Pen(Color.LightBlue, 2), curvePoints.ToArray());
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
                        for (int dotIndex = 0; dotIndex < bins[bin]; dotIndex++)
                        {
                            float dotY = graphRect.Y + graphRect.Height - 8 - (dotIndex * dotSpacing);

                            // Safety check - don't draw if we'd go above the graph area
                            if (dotY < graphRect.Y + 5)
                                break;

                            g.FillEllipse(new SolidBrush(Color.Yellow),
                                binCenterX - dotSize / 2,
                                dotY - dotSize / 2,
                                dotSize,
                                dotSize);
                        }
                    }
                }
            }
            else
            {
                // Scatter mode: dots on a single line
                foreach (var histValue in _cachedHistory)
                {
                    if (histValue >= minValue && histValue <= maxValue)
                    {
                        float x = graphRect.X + (float)((histValue - minValue) / range * graphRect.Width);
                        g.FillEllipse(new SolidBrush(Color.Yellow), x - dotSize / 2, graphRect.Y + graphRect.Height - 10, dotSize, dotSize);
                    }
                }
            }

            // Draw current comparison line
            if (_cachedComparisonTime >= minValue && _cachedComparisonTime <= maxValue)
            {
                float x = graphRect.X + (float)((_cachedComparisonTime - minValue) / range * graphRect.Width);
                g.DrawLine(new Pen(GradeLabel.ForeColor, 2), x, graphRect.Y, x, graphRect.Y + graphRect.Height);
            }

            // Draw mean line
            float meanX = graphRect.X + (float)((_cachedMean - minValue) / range * graphRect.Width);
            g.DrawLine(new Pen(Color.White, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }, meanX, graphRect.Y, meanX, graphRect.Y + graphRect.Height);
        }

        private double NormalDistribution(double x, double mean, double stdDev)
        {
            double exponent = -Math.Pow(x - mean, 2) / (2 * Math.Pow(stdDev, 2));
            return (1 / (stdDev * Math.Sqrt(2 * Math.PI))) * Math.Exp(exponent);
        }

        private void DrawStatistics(Graphics g, LiveSplitState state, float width, float yOffset)
        {
            var font = new Font(state.LayoutSettings.TextFont.FontFamily, Settings.GradingConfig.StatisticsFontSize, FontStyle.Regular);
            var brush = new SolidBrush(state.LayoutSettings.TextColor);

            // Calculate percentile from z-score
            double percentile = Statistics.ZScoreToPercentile(_cachedZScore);

            string statsText = $"Average: {TimeSpan.FromSeconds(_cachedMean):mm\\:ss\\.ff}  Percentile: {percentile:F1}%  n={_cachedHistory.Count}";

            var textSize = g.MeasureString(statsText, font);
            float x = (width - textSize.Width) / 2;

            g.DrawString(statsText, font, brush, x, yOffset);
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

            // Calculate previous split comparison data
            UpdatePreviousSplitData(state, index);
        }

        private void UpdatePreviousSplitData(LiveSplitState state, int currentIndex)
        {
            _hasPreviousSplitData = false;

            // Need at least one previous split
            if (currentIndex <= 0)
                return;

            int prevIndex = currentIndex - 1;
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
                double achievedZScore = Statistics.CalculateZScore(achievedSegmentTime.Value.TotalSeconds, mean, stdDev);

                // Check for gold/worst
                bool isGold = false;
                var bestSegment = prevSegment.BestSegmentTime[method];
                if (bestSegment.HasValue && Math.Abs(achievedSegmentTime.Value.TotalSeconds - bestSegment.Value.TotalSeconds) < 0.001)
                    isGold = true;

                bool isWorst = false;
                var maxTime = history.Max();
                if (Math.Abs(achievedSegmentTime.Value.TotalSeconds - maxTime) < 0.001)
                    isWorst = true;

                var achievedResult = GradeCalculator.CalculateGrade(achievedZScore, Settings.GradingConfig, isGold, isWorst);
                _previousAchievedGrade = achievedResult.Grade;
                _previousAchievedColor = achievedResult.Color;
            }
            else
            {
                _previousAchievedGrade = "-";
                _previousAchievedColor = state.LayoutSettings.TextColor;
            }

            // Calculate comparison grade if we have the time
            if (comparisonSegmentTime.HasValue)
            {
                double comparisonZScore = Statistics.CalculateZScore(comparisonSegmentTime.Value.TotalSeconds, mean, stdDev);

                // Check for gold/worst for comparison
                bool isGold = false;
                var bestSegment = prevSegment.BestSegmentTime[method];
                if (bestSegment.HasValue && Math.Abs(comparisonSegmentTime.Value.TotalSeconds - bestSegment.Value.TotalSeconds) < 0.001)
                    isGold = true;

                bool isWorst = false;
                var maxTime = history.Max();
                if (Math.Abs(comparisonSegmentTime.Value.TotalSeconds - maxTime) < 0.001)
                    isWorst = true;

                var comparisonResult = GradeCalculator.CalculateGrade(comparisonZScore, Settings.GradingConfig, isGold, isWorst);
                _previousComparisonGrade = comparisonResult.Grade;
                _previousComparisonColor = comparisonResult.Color;
            }
            else
            {
                _previousComparisonGrade = "-";
                _previousComparisonColor = state.LayoutSettings.TextColor;
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
                return ("-", state.LayoutSettings.TextColor);
            }

            _cachedMean = Statistics.CalculateMean(_cachedHistory);
            _cachedStdDev = Statistics.CalculateStandardDeviation(_cachedHistory);

            if (_cachedStdDev == 0)
            {
                _cachedHistory.Clear();
                return ("-", state.LayoutSettings.TextColor);
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

                // Check if this is a gold split (best segment)
                bool isGoldSplit = false;
                var bestSegment = segment.BestSegmentTime[method];
                if (bestSegment != null)
                {
                    // Use tolerance for comparison - consider gold if within 1ms or equal/better
                    double bestSeconds = bestSegment.Value.TotalSeconds;
                    if (_cachedComparisonTime <= bestSeconds + 0.001)
                    {
                        isGoldSplit = true;
                    }
                }

                // Check if this is the worst segment
                bool isWorstSplit = false;
                if (_cachedHistory.Count > 0)
                {
                    var maxSegmentTime = _cachedHistory.Max();
                    // Use tolerance - consider worst if within 1ms or equal/worse
                    if (_cachedComparisonTime >= maxSegmentTime - 0.001)
                    {
                        isWorstSplit = true;
                    }
                }

                return GradeCalculator.CalculateGrade(_cachedZScore, Settings.GradingConfig, isGoldSplit, isWorstSplit);
            }

            return ("-", state.LayoutSettings.TextColor);
        }

        public void Dispose()
        {
            // Restore original split names before disposing
            RestoreOriginalSplitNames();
            
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
