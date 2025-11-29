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
        private Dictionary<int, (string Grade, Color Color)> _gradeCache = new Dictionary<int, (string Grade, Color Color)>();

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Math.Max(Label.ActualWidth + GradeLabel.ActualWidth + 5, MinimumWidth);
        public float MinimumHeight => 25;
        public float VerticalHeight
        {
            get
            {
                if (Settings.Mode == GradingMode.List)
                {
                    // Estimate height based on font or standard split height
                    // Since we don't have easy access to the actual split height from LayoutSettings here without state context in getter (though we have _state now)
                    // We'll use a standard height or try to measure.
                    // Standard split height is often around 25-30px depending on font.
                    // Let's use a safe default or try to read from LayoutSettings if possible.
                    return (_state?.Run?.Count ?? 0) * 25f + PaddingTop + PaddingBottom;
                }
                return 25f;
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
                DrawListVertical(g, state, width, clipRegion);
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

        private void DrawListVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            var oldMatrix = g.Transform;
            
            // Setup common label properties
            GradeLabel.HasShadow = state.LayoutSettings.DropShadows;
            GradeLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            GradeLabel.Font = state.LayoutSettings.TextFont;

            float rowHeight = 25f;
            float y = PaddingTop;

            for (int i = 0; i < state.Run.Count; i++)
            {
                if (_gradeCache.TryGetValue(i, out var gradeData))
                {
                    GradeLabel.Text = gradeData.Grade;
                    GradeLabel.ForeColor = gradeData.Color;
                    
                    GradeLabel.SetActualWidth(g);
                    
                    GradeLabel.X = width - GradeLabel.ActualWidth - 5;
                    GradeLabel.Y = y;
                    GradeLabel.Width = GradeLabel.ActualWidth;
                    GradeLabel.Height = rowHeight;
                    
                    // Only draw if visible in clip region (optimization)
                    if (clipRegion.IsVisible(GradeLabel.X, GradeLabel.Y, GradeLabel.Width, GradeLabel.Height))
                    {
                        GradeLabel.Draw(g);
                    }
                }
                y += rowHeight;
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
            else
            {
                UpdateList(state);
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

            var gradeResult = CalculateGradeForSplit(state, index);
            GradeLabel.Text = gradeResult.Grade;
            GradeLabel.ForeColor = gradeResult.Color;
        }

        private void UpdateList(LiveSplitState state)
        {
            // Check if we need to recalculate
            // We recalculate if the run changes, comparison changes, or timing method changes.
            // For simplicity, we can check if the cache is empty or if key parameters changed.
            // But since this is called every frame, we must be efficient.
            
            // We can track _lastRun and _lastComparison.
            // Also need to track TimingMethod.
            
            bool shouldRecalculate = false;
            if (state.Run != _lastRun || state.CurrentComparison != _lastComparison || _gradeCache.Count != state.Run.Count)
            {
                shouldRecalculate = true;
            }

            if (shouldRecalculate)
            {
                _gradeCache.Clear();
                _lastRun = state.Run;
                _lastComparison = state.CurrentComparison;

                for (int i = 0; i < state.Run.Count; i++)
                {
                    _gradeCache[i] = CalculateGradeForSplit(state, i);
                }
            }
        }

        private (string Grade, Color Color) CalculateGradeForSplit(LiveSplitState state, int index)
        {
            if (index < 0 || index >= state.Run.Count)
                return ("-", Color.White);

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
                return ("?", Color.White);
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
                return GradeCalculator.CalculateGrade(zScore);
            }

            return ("-", Color.White);
        }

        public void Dispose()
        {
            Settings.Dispose();
        }
    }
}
