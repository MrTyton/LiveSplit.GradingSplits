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

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Label.ActualWidth + GradeLabel.ActualWidth + 5;
        public float MinimumHeight => 25;
        public float VerticalHeight => 25;
        public float MinimumWidth => 100;

        public float PaddingTop => 0;
        public float PaddingBottom => 0;
        public float PaddingLeft => 0;
        public float PaddingRight => 0;

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
                DrawSingle(g, state, height, clipRegion, true);
            }
            else
            {
                // List mode horizontal not fully supported yet, fallback to single
                DrawSingle(g, state, height, clipRegion, true);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (Settings.Mode == GradingMode.Single)
            {
                DrawSingle(g, state, width, clipRegion, false);
            }
            else
            {
                // List mode vertical
                // Placeholder: just draw single for now
                DrawSingle(g, state, width, clipRegion, false);
            }
        }

        private void DrawSingle(Graphics g, LiveSplitState state, float dimension, Region clipRegion, bool horizontal)
        {
            var oldMatrix = g.Transform;

            // Basic layout
            Label.ForeColor = state.LayoutSettings.TextColor;
            Label.HasShadow = state.LayoutSettings.DropShadows;
            Label.ShadowColor = state.LayoutSettings.ShadowsColor;
            Label.Font = state.LayoutSettings.TextFont;

            GradeLabel.HasShadow = state.LayoutSettings.DropShadows;
            GradeLabel.ShadowColor = state.LayoutSettings.ShadowsColor;

            // Calculate sizes
            Label.SetActualWidth(g);
            GradeLabel.SetActualWidth(g);

            if (horizontal)
            {
                Label.X = 0;
                Label.Y = 0;
                Label.Height = dimension;
                Label.Draw(g);

                GradeLabel.X = Label.ActualWidth + 5;
                GradeLabel.Y = 0;
                GradeLabel.Height = dimension;
                GradeLabel.Draw(g);
            }
            else
            {
                Label.X = 5;
                Label.Y = 0;
                Label.Height = dimension;
                Label.Draw(g);

                GradeLabel.X = dimension - GradeLabel.ActualWidth - 5;
                GradeLabel.Y = 0;
                GradeLabel.Height = dimension;
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
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                GradeLabel.Text = "-";
                _lastGradedIndex = -1;
                _lastRun = state.Run;
                return;
            }

            // Find the last completed split
            var index = state.CurrentSplitIndex - 1;

            // If we just finished the run, the index is Run.Count - 1
            if (state.CurrentPhase == TimerPhase.Ended)
                index = state.Run.Count - 1;

            // Check if we need to recalculate
            if (index == _lastGradedIndex && state.Run == _lastRun)
            {
                return;
            }

            _lastGradedIndex = index;
            _lastRun = state.Run;

            if (index < 0)
            {
                GradeLabel.Text = "-";
                return;
            }

            // Get the segment
            var segment = state.Run[index];

            // Calculate stats for this segment
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
                return;
            }

            var mean = Statistics.CalculateMean(segmentHistory);
            var stdDev = Statistics.CalculateStandardDeviation(segmentHistory);

            // Current segment time
            TimeSpan? currentSegmentTime = null;

            var currentSplitTime = segment.SplitTime[state.CurrentTimingMethod];
            var prevSplitTime = index > 0 ? state.Run[index - 1].SplitTime[state.CurrentTimingMethod] : TimeSpan.Zero;

            if (currentSplitTime != null && prevSplitTime != null)
            {
                currentSegmentTime = currentSplitTime - prevSplitTime;
            }

            if (currentSegmentTime != null)
            {
                var zScore = Statistics.CalculateZScore(currentSegmentTime.Value.TotalSeconds, mean, stdDev);
                var grade = GradeCalculator.CalculateGrade(zScore);
                GradeLabel.Text = grade.Grade;
                GradeLabel.ForeColor = grade.Color;
            }
            else
            {
                GradeLabel.Text = "-";
            }
        }

        public void Dispose()
        {
        }
    }
}
