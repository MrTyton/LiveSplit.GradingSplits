using LiveSplit.Model;
using System.Collections.Generic;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Partial class containing split name management functionality.
    /// Handles storing, restoring, and updating split names with grade information.
    /// </summary>
    public partial class GradingSplitsComponent
    {
        private void StoreOriginalSplitNames()
        {
            if (_state.Run == null) return;

            // First restore any previously modified names before storing new originals
            if (_splitNamesModified && _gradedRun != null)
            {
                RestoreOriginalSplitNames();
            }

            _originalSplitNames.Clear();
            _gradedRun = _state.Run;

            for (int i = 0; i < _state.Run.Count; i++)
            {
                // Try to extract the original name in case it already has grade formatting
                string name = _state.Run[i].Name;
                string cleanedName = StripGradeFromName(name);
                _originalSplitNames[i] = cleanedName;

                // If we cleaned the name, also update the run to use the clean name
                if (cleanedName != name)
                {
                    _state.Run[i].Name = cleanedName;
                }
            }

            _splitNamesModified = false;
        }

        /// <summary>
        /// Attempts to strip grade formatting from a split name using the configured format.
        /// </summary>
        private string StripGradeFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            string format = Settings.GradingConfig.SplitNameFormat;
            if (string.IsNullOrEmpty(format) || !format.Contains("{Name}") || !format.Contains("{Grade}"))
                return name;

            // Get all possible grade labels
            var allGrades = new HashSet<string>();
            foreach (var threshold in Settings.GradingConfig.Thresholds)
            {
                if (!string.IsNullOrEmpty(threshold.Label))
                    allGrades.Add(threshold.Label);
            }
            if (Settings.GradingConfig.UseGoldGrade && !string.IsNullOrEmpty(Settings.GradingConfig.GoldLabel))
                allGrades.Add(Settings.GradingConfig.GoldLabel);
            if (Settings.GradingConfig.UseWorstGrade && !string.IsNullOrEmpty(Settings.GradingConfig.WorstLabel))
                allGrades.Add(Settings.GradingConfig.WorstLabel);

            if (allGrades.Count == 0) return name;

            // Try each grade to see if we can extract the original name
            foreach (var grade in allGrades)
            {
                string formatted = format.Replace("{Grade}", grade);
                // Now we have something like "{Name} [A]" or "[A] {Name}"

                // Find where {Name} is in the formatted string
                int nameIndex = formatted.IndexOf("{Name}");
                if (nameIndex < 0) continue;

                string prefix = formatted.Substring(0, nameIndex);
                string suffix = formatted.Substring(nameIndex + 6); // 6 = "{Name}".Length

                // Check if the current name matches this pattern
                if (name.StartsWith(prefix) && name.EndsWith(suffix) && name.Length > prefix.Length + suffix.Length)
                {
                    string extracted = name.Substring(prefix.Length, name.Length - prefix.Length - suffix.Length);
                    // Recursively strip in case multiple grades were appended
                    return StripGradeFromName(extracted);
                }
            }

            return name;
        }

        private void RestoreOriginalSplitNames()
        {
            if (!_splitNamesModified || _gradedRun == null) return;

            foreach (var kvp in _originalSplitNames)
            {
                if (kvp.Key < _gradedRun.Count)
                {
                    _gradedRun[kvp.Key].Name = kvp.Value;
                }
            }

            _originalSplitNames.Clear();
            _splitNamesModified = false;
            _gradedRun = null;
        }

        private void UpdateSplitNamesWithGrades(LiveSplitState state)
        {
            if (state.Run == null) return;

            // If the feature is disabled, restore original names if modified
            if (!Settings.GradingConfig.ShowGradeInSplitNames)
            {
                if (_splitNamesModified)
                {
                    RestoreOriginalSplitNames();
                    _splitNameGradeCache.Clear();
                }
                _lastNameUpdateSplitIndex = -1;
                return;
            }

            // Check if we need to update names
            bool needsUpdate = false;

            // Run changed
            if (_gradedRun != state.Run)
            {
                needsUpdate = true;
                _splitNameGradeCache.Clear();
            }
            // Split index changed (a split was completed)
            else if (_lastNameUpdateSplitIndex != state.CurrentSplitIndex)
            {
                needsUpdate = true;
            }

            _lastNameUpdateSplitIndex = state.CurrentSplitIndex;

            if (!needsUpdate)
            {
                return;  // Names are up to date
            }

            // Make sure we have original names stored
            if (_originalSplitNames.Count == 0 || _gradedRun != state.Run)
            {
                StoreOriginalSplitNames();
            }

            for (int i = 0; i < state.Run.Count; i++)
            {
                string originalName = _originalSplitNames.ContainsKey(i) ? _originalSplitNames[i] : state.Run[i].Name;
                string grade;

                if (i < state.CurrentSplitIndex)
                {
                    // Completed split - show achieved grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: true);
                    grade = result.Grade;
                }
                else
                {
                    // Upcoming split - show comparison grade
                    var result = CalculateGradeForSplit(state, i, useActualTime: false);
                    grade = result.Grade;
                }

                // Check if grade changed from cached value
                bool gradeChanged = true;
                if (_splitNameGradeCache.TryGetValue(i, out var cachedGrade))
                {
                    gradeChanged = cachedGrade != grade;
                }

                if (gradeChanged)
                {
                    _splitNameGradeCache[i] = grade;

                    if (grade != "-" && !string.IsNullOrEmpty(grade))
                    {
                        // Use the customizable format string
                        string format = Settings.GradingConfig.SplitNameFormat;
                        if (string.IsNullOrEmpty(format))
                            format = "{Name} [{Grade}]";

                        state.Run[i].Name = format
                            .Replace("{Name}", originalName)
                            .Replace("{Grade}", grade);
                        _splitNamesModified = true;
                    }
                    else
                    {
                        state.Run[i].Name = originalName;
                    }
                }
            }
        }
    }
}
