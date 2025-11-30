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

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Label.ActualWidth + GradeLabel.ActualWidth + 5;
        public float MinimumHeight => 25;
        public float VerticalHeight => 25f;
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
                Label.Height = height;
                Label.Draw(g);

                GradeLabel.X = Label.ActualWidth + 5;
                GradeLabel.Y = 0;
                GradeLabel.Width = GradeLabel.ActualWidth;
                GradeLabel.Height = height;
                
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
                Label.Height = height;
                Label.Draw(g);

                GradeLabel.X = width - GradeLabel.ActualWidth - 5;
                GradeLabel.Y = 0;
                GradeLabel.Width = GradeLabel.ActualWidth;
                GradeLabel.Height = height;
                
                // Draw background for grade label only if enabled
                if (Settings.GradingConfig.UseBackgroundColor)
                {
                    var backgroundBrush = new SolidBrush(Settings.GradingConfig.BackgroundColor);
                    g.FillRectangle(backgroundBrush, GradeLabel.X, GradeLabel.Y, GradeLabel.Width, GradeLabel.Height);
                    backgroundBrush.Dispose();
                }
                
                GradeLabel.Draw(g);
            }

            g.Transform = oldMatrix;
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
                return ("-", state.LayoutSettings.TextColor);

            var segment = state.Run[index];
            var method = state.CurrentTimingMethod;

            // Calculate stats for this segment based on history
            var segmentHistory = new List<double>();
            foreach (var historyItem in segment.SegmentHistory)
            {
                var time = historyItem.Value[method];
                if (time.HasValue)
                {
                    segmentHistory.Add(time.Value.TotalSeconds);
                }
            }

            if (segmentHistory.Count < 2)
            {
                return ("-", state.LayoutSettings.TextColor);
            }

            var mean = Statistics.CalculateMean(segmentHistory);
            var stdDev = Statistics.CalculateStandardDeviation(segmentHistory);

            if (stdDev == 0) return ("-", state.LayoutSettings.TextColor);

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
                var zScore = Statistics.CalculateZScore(segmentTime.Value.TotalSeconds, mean, stdDev);
                
                // Check if this is a gold split (best segment)
                bool isGoldSplit = false;
                var bestSegment = segment.BestSegmentTime[method];
                if (bestSegment != null && segmentTime.Value == bestSegment.Value)
                {
                    isGoldSplit = true;
                }
                
                // Check if this is the worst segment
                bool isWorstSplit = false;
                if (segmentHistory.Count > 0)
                {
                    var maxSegmentTime = segmentHistory.Max();
                    if (Math.Abs(segmentTime.Value.TotalSeconds - maxSegmentTime) < 0.001) // Small epsilon for float comparison
                    {
                        isWorstSplit = true;
                    }
                }
                
                return GradeCalculator.CalculateGrade(zScore, Settings.GradingConfig, isGoldSplit, isWorstSplit);
            }

            return ("-", state.LayoutSettings.TextColor);
        }

        public void Dispose()
        {
            Settings.Dispose();
        }
    }
}
