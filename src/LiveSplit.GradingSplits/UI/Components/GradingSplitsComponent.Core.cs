using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Main component class for Grading Splits.
    /// This partial class contains fields, properties, constructor, and IComponent interface implementation.
    /// Other functionality is split into separate partial class files:
    /// - GradingSplitsComponent.SplitNames.cs - Split name management
    /// - GradingSplitsComponent.SplitIcons.cs - Split icon management
    /// - GradingSplitsComponent.Drawing.cs - Rendering/drawing methods
    /// - GradingSplitsComponent.GradeCalculation.cs - Grade calculation logic
    /// </summary>
    public partial class GradingSplitsComponent : IComponent
    {
        #region Fields

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
        private int _lastPreviousSplitIndex = -1;

        // Split name grading
        private Dictionary<int, string> _originalSplitNames = new Dictionary<int, string>();
        private bool _splitNamesModified = false;
        private IRun _gradedRun = null;
        private Dictionary<int, string> _splitNameGradeCache = new Dictionary<int, string>();
        private int _lastNameUpdateSplitIndex = -1;

        // Split icon grading
        private Dictionary<int, Image> _originalSplitIcons = new Dictionary<int, Image>();
        private bool _splitIconsModified = false;
        private int _lastIconUpdateSplitIndex = -1;
        private bool _lastShowGradeIconsSetting = false;
        private Dictionary<int, (string Grade, Color Color)> _splitIconCache = new Dictionary<int, (string, Color)>();
        private IRun _lastIconRun = null;
        private TimerPhase _lastIconPhase = TimerPhase.NotRunning;

        // Grade calculation cache
        private Dictionary<(int Index, bool UseActualTime), (string Grade, Color Color)> _gradeCalculationCache = new Dictionary<(int, bool), (string, Color)>();
        private int _lastGradeCalcSplitIndex = -1;

        // Cached current grade icon for component display
        private Image _currentGradeIcon = null;
        private string _lastIconGrade = null;
        private Color _lastIconColor = Color.White;
        private string _lastPrevIconFolderPath = null;

        #endregion

        #region IComponent Properties

        public string ComponentName => "Grading Splits";

        public float HorizontalWidth => Label.ActualWidth + GradeLabel.ActualWidth + 5;

        public float MinimumHeight => Settings.GradingConfig.ShowGraph ? 25 + Settings.GradingConfig.GraphHeight + 30 : 25;

        public float VerticalHeight
        {
            get
            {
                float height = 0f;

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

                return Math.Max(1f, height);
            }
        }

        public float MinimumWidth => 100;

        public float PaddingTop => 7f;
        public float PaddingBottom => 7f;
        public float PaddingLeft => 7f;
        public float PaddingRight => 7f;

        public IDictionary<string, Action> ContextMenuControls => null;

        #endregion

        #region Constructor

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

            _state.OnReset += State_OnReset;
            _state.OnStart += State_OnStart;
        }

        #endregion

        #region Event Handlers

        private void State_OnStart(object sender, EventArgs e)
        {
            // Only store original names if we haven't already for this run
            // or if the run changed
            if (_originalSplitNames.Count == 0 || _gradedRun != _state.Run)
            {
                StoreOriginalSplitNames();
            }

            // Only store original icons if we haven't already for this run
            // We check _lastIconRun instead of _gradedRun since icons are tracked separately
            if (_originalSplitIcons.Count == 0 || _lastIconRun != _state.Run)
            {
                StoreOriginalSplitIcons();
            }
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
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
            _splitIconsModified = false;  // Reset so icons get reapplied on next run
            _lastIconPhase = TimerPhase.NotRunning;  // Reset phase tracking
        }

        #endregion

        #region IComponent Interface Methods

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

        public void Dispose()
        {
            RestoreOriginalSplitNames();
            RestoreOriginalSplitIcons();

            _currentGradeIcon?.Dispose();
            _currentGradeIcon = null;
            _previousAchievedIcon?.Dispose();
            _previousAchievedIcon = null;
            _previousComparisonIcon?.Dispose();
            _previousComparisonIcon = null;

            if (_state != null)
            {
                _state.OnReset -= State_OnReset;
                _state.OnStart -= State_OnStart;
            }

            Settings.Dispose();
        }

        #endregion
    }
}
