using LiveSplit.Model;
using LiveSplit.GradingSplits.Model;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
            if (!_splitIconsModified || _lastIconRun == null) return;

            foreach (var kvp in _originalSplitIcons)
            {
                if (kvp.Key < _lastIconRun.Count)
                {
                    _lastIconRun[kvp.Key].Icon = kvp.Value;
                }
            }

            _originalSplitIcons.Clear();
            _splitIconsModified = false;
        }

        /// <summary>
        /// Throttle for checking if Run Editor is open (avoid checking every frame).
        /// </summary>
        private int _runEditorCheckCounter = 0;
        private bool _lastRunEditorOpenState = false;
        private const int RunEditorCheckInterval = 30; // Check every ~30 frames

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

            // Throttled check for Run Editor dialog being open
            _runEditorCheckCounter++;
            if (_runEditorCheckCounter >= RunEditorCheckInterval)
            {
                _runEditorCheckCounter = 0;
                _lastRunEditorOpenState = IsRunEditorOpen();
            }

            // Don't modify icons while the Run Editor dialog is open
            // This prevents interference with the Run Editor's DataGridView which binds to the same Run object
            if (_lastRunEditorOpenState)
            {
                // Still track the setting state so we update when dialog closes
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
            else if (_lastIconRun != state.Run)
            {
                needsFullUpdate = true;
                _splitIconCache.Clear();
            }
            // Timer phase changed (e.g., NotRunning -> Running, Running -> Ended)
            // This ensures icons are refreshed when state changes
            else if (_lastIconPhase != state.CurrentPhase)
            {
                needsFullUpdate = true;
            }
            // Icons were cleared (e.g., after reset) but setting is still enabled
            // This ensures icons are reapplied after a reset
            else if (!_splitIconsModified && settingEnabled)
            {
                needsFullUpdate = true;
            }
            // Split index changed (a split was completed) - only matters when actually running
            else if (state.CurrentPhase != TimerPhase.NotRunning && _lastIconUpdateSplitIndex != state.CurrentSplitIndex)
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
            _lastIconFolderPath = Settings.GradingConfig.IconFolderPath;
            _lastIconPhase = state.CurrentPhase;

            if (!needsFullUpdate)
            {
                return; // Icons are up to date
            }

            _lastIconUpdateSplitIndex = state.CurrentSplitIndex;

            // Make sure we have original icons stored
            if (_originalSplitIcons.Count == 0 || _lastIconRun != state.Run)
            {
                StoreOriginalSplitIcons();
                _lastIconRun = state.Run;
            }

            for (int i = 0; i < state.Run.Count; i++)
            {
                string grade;
                Color color;
                bool isGold = false;
                bool isWorst = false;

                // For upcoming splits (not yet completed), use cached values if available
                // since their comparison grades don't change during a run
                bool useActualTime = i < state.CurrentSplitIndex;
                
                // Check if we have a cached value we can reuse for grade calculation
                bool hadCachedValue = _splitIconCache.TryGetValue(i, out var cached);
                bool canSkipCalculation = hadCachedValue && (!useActualTime || (i < state.CurrentSplitIndex - 1));
                
                if (canSkipCalculation)
                {
                    // Use cached grade/color
                    grade = cached.Grade;
                    color = cached.Color;
                }
                else
                {
                    // Calculate grade for this split
                    var result = CalculateGradeForSplit(state, i, useActualTime);
                    grade = result.Grade;
                    color = result.Color;
                    
                    if (useActualTime)
                    {
                        // Check if this was a gold or worst split
                        isGold = Settings.GradingConfig.UseGoldGrade && grade == Settings.GradingConfig.GoldLabel;
                        isWorst = Settings.GradingConfig.UseWorstGrade && grade == Settings.GradingConfig.WorstLabel;
                    }
                    
                    // Update cache
                    _splitIconCache[i] = (grade, color);
                }

                // Always set icons when we reach here (needsFullUpdate was true)
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

        /// <summary>
        /// Checks if the Run Editor dialog is currently open.
        /// </summary>
        /// <returns>True if the Run Editor dialog is open, false otherwise.</returns>
        private bool IsRunEditorOpen()
        {
            try
            {
                // Check if any open form is the RunEditorDialog
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType().Name == "RunEditorDialog")
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // If we can't check forms, assume it's safe to update
            }
            return false;
        }
    }
}
