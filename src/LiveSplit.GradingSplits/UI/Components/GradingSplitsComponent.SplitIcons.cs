using LiveSplit.Model;
using LiveSplit.GradingSplits.Model;
using System.Drawing;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Partial class containing split icon management functionality.
    /// Handles storing, restoring, and updating split icons with grade badges.
    /// </summary>
    public partial class GradingSplitsComponent
    {
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
    }
}
