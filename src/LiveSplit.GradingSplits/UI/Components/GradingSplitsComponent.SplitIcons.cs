using LiveSplit.Model;
using LiveSplit.GradingSplits.Model;
using System.Drawing;
using System.Linq;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Partial class containing split icon management functionality.
    /// Handles storing, restoring, and updating split icons with grade badges.
    /// </summary>
    public partial class GradingSplitsComponent
    {
        /// <summary>
        /// Tracks the last icon folder path to detect changes.
        /// </summary>
        private string _lastIconFolderPath;

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

            // Don't modify icons while timer is not running (e.g., when Edit Splits dialog may be open)
            // This prevents interference with the Run Editor's DataGridView which binds to the same Run object
            // Icons will be updated when the timer starts or a split is completed
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                // Still track the setting state so we update on first run
                _lastShowGradeIconsSetting = settingEnabled;
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
            // Icon folder path changed
            else if (_lastIconFolderPath != Settings.GradingConfig.IconFolderPath)
            {
                needsFullUpdate = true;
                _splitIconCache.Clear();
                CustomIconLoader.ClearCacheIfFolderChanged(Settings.GradingConfig.IconFolderPath);
            }

            _lastShowGradeIconsSetting = settingEnabled;
            _lastIconUpdateSplitIndex = state.CurrentSplitIndex;
            _lastIconFolderPath = Settings.GradingConfig.IconFolderPath;

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
                bool isGold = false;
                bool isWorst = false;

                if (i < state.CurrentSplitIndex)
                {
                    // Completed split - show achieved grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: true);
                    grade = result.Grade;
                    color = result.Color;
                    
                    // Check if this was a gold or worst split
                    isGold = Settings.GradingConfig.UseGoldGrade && grade == Settings.GradingConfig.GoldLabel;
                    isWorst = Settings.GradingConfig.UseWorstGrade && grade == Settings.GradingConfig.WorstLabel;
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
                        // Try to get a custom icon first
                        var threshold = Settings.GradingConfig.Thresholds.FirstOrDefault(t => t.Label == grade);
                        var customIcon = CustomIconLoader.GetCustomIcon(grade, Settings.GradingConfig, threshold, isGold, isWorst);
                        
                        if (customIcon != null)
                        {
                            state.Run[i].Icon = customIcon;
                        }
                        else
                        {
                            // Fall back to generated icon
                            state.Run[i].Icon = GradeIconGenerator.GenerateIcon(grade, color);
                        }
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
    }
}
