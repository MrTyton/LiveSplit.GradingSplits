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

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Label.ActualWidth + GradeLabel.ActualWidth + 5;
        public float MinimumHeight => Settings.GradingConfig.ShowGraph ? 25 + Settings.GradingConfig.GraphHeight + 30 : 25;
        public float VerticalHeight => Settings.GradingConfig.ShowGraph ? 25 + Settings.GradingConfig.GraphHeight + 30 : 25f;
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
                
                // Draw statistics text below graph
                float statsY = graphY + Settings.GradingConfig.GraphHeight + 2;
                DrawStatistics(g, state, width, statsY);
            }

            g.Transform = oldMatrix;
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
                
                // Draw dots for each bin, stacked vertically
                float dotSpacing = dotSize + 1;
                
                for (int bin = 0; bin < numBins; bin++)
                {
                    if (bins[bin] > 0)
                    {
                        float binCenterX = graphRect.X + bin * binWidth + binWidth / 2;
                        
                        // Stack dots from bottom up
                        for (int dotIndex = 0; dotIndex < bins[bin]; dotIndex++)
                        {
                            float dotY = graphRect.Y + graphRect.Height - 8 - (dotIndex * dotSpacing);
                            
                            // Don't draw if we'd go above the graph area
                            if (dotY < graphRect.Y + 10)
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
            var font = new Font(state.LayoutSettings.TextFont.FontFamily, 8, FontStyle.Regular);
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
        }

        private (string Grade, Color Color) CalculateGradeForSplit(LiveSplitState state, int index)
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

            // Get the comparison segment time for the current split
            TimeSpan? segmentTime = null;
            var comparison = state.CurrentComparison;
            
            var splitTime = segment.Comparisons[comparison][method];
            var prevSplitTime = index > 0 ? state.Run[index - 1].Comparisons[comparison][method] : TimeSpan.Zero;

            if (splitTime != null && prevSplitTime != null)
            {
                segmentTime = splitTime - prevSplitTime;
            }

            if (segmentTime != null)
            {
                _cachedComparisonTime = segmentTime.Value.TotalSeconds;
                _cachedZScore = Statistics.CalculateZScore(_cachedComparisonTime, _cachedMean, _cachedStdDev);
                
                // Check if this is a gold split (best segment)
                bool isGoldSplit = false;
                var bestSegment = segment.BestSegmentTime[method];
                if (bestSegment != null && segmentTime.Value == bestSegment.Value)
                {
                    isGoldSplit = true;
                }
                
                // Check if this is the worst segment
                bool isWorstSplit = false;
                if (_cachedHistory.Count > 0)
                {
                    var maxSegmentTime = _cachedHistory.Max();
                    if (Math.Abs(_cachedComparisonTime - maxSegmentTime) < 0.001) // Small epsilon for float comparison
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
            Settings.Dispose();
        }
    }
}
