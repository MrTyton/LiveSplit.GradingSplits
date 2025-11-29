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

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Math.Max(Label.ActualWidth + GradeLabel.ActualWidth + 5, MinimumWidth);
        public float MinimumHeight => 25;
        public float VerticalHeight => 25;
        public float MinimumWidth => 100;

        public float PaddingTop => 7f;
        public float PaddingBottom => 7f;
        public float PaddingLeft => 7f;
        public float PaddingRight => 7f;

        public IDictionary<string, Action> ContextMenuControls => null;

        public GradingSplitsComponent(LiveSplitState state)
        {
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
            if (Settings.Mode == GradingMode.Single)
            {
                DrawSingle(g, state, HorizontalWidth, height, clipRegion, true);
            }
            else
            {
                // List mode horizontal not fully supported yet, fallback to single
                DrawSingle(g, state, HorizontalWidth, height, clipRegion, true);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (Settings.Mode == GradingMode.Single)
            {
                DrawSingle(g, state, width, VerticalHeight, clipRegion, false);
            }
            else
            {
                // List mode vertical
                // Placeholder: just draw single for now
                DrawSingle(g, state, width, VerticalHeight, clipRegion, false);
            }
        }

        private void DrawSingle(Graphics g, LiveSplitState state, float width, float height, Region clipRegion, bool horizontal)
        {
            var oldMatrix = g.Transform;

            // Basic layout
            Label.ForeColor = state.LayoutSettings.TextColor;
            Label.HasShadow = state.LayoutSettings.DropShadows;
            Label.ShadowColor = state.LayoutSettings.ShadowsColor;
            Label.Font = state.LayoutSettings.TextFont;
            GradeLabel.Font = state.LayoutSettings.TextFont; // Set font for GradeLabel

            GradeLabel.HasShadow = state.LayoutSettings.DropShadows;
            GradeLabel.ShadowColor = state.LayoutSettings.ShadowsColor;

            // Calculate sizes
            Label.SetActualWidth(g);
            GradeLabel.SetActualWidth(g);

            if (horizontal)
            {
                Label.X = 0;
                Label.Y = 0;
                Label.Width = Label.ActualWidth; // Set Width
                Label.Height = height;
                Label.Draw(g);

                GradeLabel.X = Label.ActualWidth + 5;
                GradeLabel.Y = 0;
                GradeLabel.Width = GradeLabel.ActualWidth; // Set Width
                GradeLabel.Height = height;
                GradeLabel.Draw(g);
            }
            else
            {
                Label.X = 5;
                Label.Y = 0;
                Label.Width = width - GradeLabel.ActualWidth - 10; // Set Width
                Label.Height = height;
                Label.Draw(g);

                GradeLabel.X = width - GradeLabel.ActualWidth - 5;
                GradeLabel.Y = 0;
                GradeLabel.Width = GradeLabel.ActualWidth; // Set Width
                GradeLabel.Height = height;
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
            if (Settings.Mode == GradingMode.Single)
            {
                UpdateSingle(state);
            }

            if (invalidator != null)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        private void UpdateSingle(LiveSplitState state)
        {
            if (state.CurrentPhase == TimerPhase.NotRunning || state.CurrentPhase == TimerPhase.Ended)
            {
                GradeLabel.Text = "-";
                GradeLabel.ForeColor = Color.White;
                _lastGradedIndex = -1;
                _lastRun = state.Run;
                _lastComparison = null;
                return;
            }

            // Use the current split index (the one being run)
            var index = state.CurrentSplitIndex;

            if (index < 0 || index >= state.Run.Count)
            {
                GradeLabel.Text = "-";
                GradeLabel.ForeColor = Color.White;
                return;
            }

            // Check if we need to recalculate
            if (index == _lastGradedIndex && state.Run == _lastRun && state.CurrentComparison == _lastComparison)
            {
                return;
            }

            _lastGradedIndex = index;
            _lastRun = state.Run;
            _lastComparison = state.CurrentComparison;

            // Get the segment
            var segment = state.Run[index];

            // Calculate stats for this segment based on history
            var segmentHistory = new List<double>();
            foreach (var historyItem in segment.SegmentHistory)
            {
                var time = historyItem.Value[state.CurrentTimingMethod];
                if (time.HasValue)
                {
                    segmentHistory.Add(time.Value.TotalSeconds);
                }
            }

            if (segmentHistory.Count < 2)
            {
                GradeLabel.Text = "?";
                GradeLabel.ForeColor = Color.White;
                return;
            }

            var mean = Statistics.CalculateMean(segmentHistory);
            var stdDev = Statistics.CalculateStandardDeviation(segmentHistory);

            // Calculate Comparison Segment Time
            TimeSpan? comparisonSegmentTime = null;
            var comparisonName = state.CurrentComparison;
            var method = state.CurrentTimingMethod;

            var currentSplitComp = segment.Comparisons[comparisonName][method];
            var prevSplitComp = index > 0 ? state.Run[index - 1].Comparisons[comparisonName][method] : TimeSpan.Zero;

            if (currentSplitComp != null && prevSplitComp != null)
            {
                comparisonSegmentTime = currentSplitComp - prevSplitComp;
            }

            if (comparisonSegmentTime != null)
            {
                var zScore = Statistics.CalculateZScore(comparisonSegmentTime.Value.TotalSeconds, mean, stdDev);
                var grade = GradeCalculator.CalculateGrade(zScore);
                GradeLabel.Text = grade.Grade;
                GradeLabel.ForeColor = grade.Color;
            }
            else
            {
                GradeLabel.Text = "-";
                GradeLabel.ForeColor = Color.White;
            }
        }

        public void Dispose()
        {
            Settings.Dispose();
        }
    }
}
