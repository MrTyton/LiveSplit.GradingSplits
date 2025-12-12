using LiveSplit.Model;
using LiveSplit.GradingSplits.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Partial class containing all drawing and rendering functionality.
    /// Handles rendering the grade display, graph, statistics, and previous split comparison.
    /// </summary>
    public partial class GradingSplitsComponent
    {
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
    }
}
