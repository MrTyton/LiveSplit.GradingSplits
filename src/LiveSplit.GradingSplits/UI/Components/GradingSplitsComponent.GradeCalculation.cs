using LiveSplit.Model;
using LiveSplit.GradingSplits.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Partial class containing grade calculation functionality.
    /// Handles calculating grades for splits and updating grade state.
    /// </summary>
    public partial class GradingSplitsComponent
    {
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

            // Check if icon folder changed - if so, force icon regeneration
            bool folderChanged = _lastIconFolderPath != Settings.GradingConfig.IconFolderPath;
            if (folderChanged)
            {
                _lastIconFolderPath = Settings.GradingConfig.IconFolderPath;
            }

            // Update cached icon if grade, color, or folder changed
            if (_lastIconGrade != gradeResult.Grade || _lastIconColor != gradeResult.Color || folderChanged)
            {
                _lastIconGrade = gradeResult.Grade;
                _lastIconColor = gradeResult.Color;
                _currentGradeIcon?.Dispose();

                if (gradeResult.Grade != "-")
                {
                    // Check if this is a gold or worst grade
                    bool isGold = Settings.GradingConfig.UseGoldGrade && gradeResult.Grade == Settings.GradingConfig.GoldLabel;
                    bool isWorst = Settings.GradingConfig.UseWorstGrade && gradeResult.Grade == Settings.GradingConfig.WorstLabel;

                    // Try custom icon first, fall back to generated
                    _currentGradeIcon = CustomIconLoader.GetCustomIconSmall(
                        gradeResult.Grade,
                        Settings.GradingConfig,
                        GradeIconGenerator.SmallIconSize,
                        isGold,
                        isWorst);

                    if (_currentGradeIcon == null)
                    {
                        _currentGradeIcon = GradeIconGenerator.GenerateSmallIcon(gradeResult.Grade, gradeResult.Color);
                    }
                }
                else
                {
                    _currentGradeIcon = null;
                }
            }

            // Calculate previous split comparison data
            UpdatePreviousSplitData(state, index);
        }

        private void UpdatePreviousSplitData(LiveSplitState state, int currentIndex)
        {
            // Need at least one previous split
            if (currentIndex <= 0)
            {
                _hasPreviousSplitData = false;
                _lastPreviousSplitIndex = -1;
                return;
            }

            // Check if we already have cached data for this split
            // Also regenerate if icon folder changed
            bool folderChangedForPrev = _lastPrevIconFolderPath != Settings.GradingConfig.IconFolderPath;
            if (_lastPreviousSplitIndex == currentIndex - 1 && _hasPreviousSplitData && !folderChangedForPrev)
            {
                return;  // Use cached data
            }
            _lastPrevIconFolderPath = Settings.GradingConfig.IconFolderPath;

            _hasPreviousSplitData = false;

            int prevIndex = currentIndex - 1;
            _lastPreviousSplitIndex = prevIndex;
            var prevSegment = state.Run[prevIndex];
            var method = state.CurrentTimingMethod;

            // Get the actual achieved time for the previous split
            var prevSplitTime = prevSegment.SplitTime[method];
            var prevPrevSplitTime = prevIndex > 0 ? state.Run[prevIndex - 1].SplitTime[method] : TimeSpan.Zero;

            TimeSpan? achievedSegmentTime = null;
            if (prevSplitTime.HasValue)
            {
                achievedSegmentTime = prevSplitTime.Value - (prevPrevSplitTime ?? TimeSpan.Zero);
            }

            // Get the comparison time for the previous split
            var comparison = state.CurrentComparison;
            var comparisonSplitTime = prevSegment.Comparisons[comparison][method];
            var comparisonPrevSplitTime = prevIndex > 0 ? state.Run[prevIndex - 1].Comparisons[comparison][method] : TimeSpan.Zero;

            TimeSpan? comparisonSegmentTime = null;
            if (comparisonSplitTime.HasValue)
            {
                comparisonSegmentTime = comparisonSplitTime.Value - (comparisonPrevSplitTime ?? TimeSpan.Zero);
            }

            // Calculate statistics for the previous segment
            var history = new List<double>();
            foreach (var historyItem in prevSegment.SegmentHistory)
            {
                var time = historyItem.Value[method];
                if (time.HasValue)
                {
                    history.Add(time.Value.TotalSeconds);
                }
            }

            if (history.Count < 2)
                return;

            var mean = Statistics.CalculateMean(history);
            var stdDev = Statistics.CalculateStandardDeviation(history);

            if (stdDev == 0)
                return;

            // Calculate achieved grade if we have the time
            if (achievedSegmentTime.HasValue)
            {
                double achievedSeconds = achievedSegmentTime.Value.TotalSeconds;
                double achievedZScore = Statistics.CalculateZScore(achievedSeconds, mean, stdDev);

                // Check for gold/worst using helper methods
                var bestSegment = prevSegment.BestSegmentTime[method];
                bool isGold = GradeCalculator.IsGoldSplit(achievedSeconds, bestSegment?.TotalSeconds);
                bool isWorst = GradeCalculator.IsWorstSplit(achievedSeconds, history);

                var achievedResult = GradeCalculator.CalculateGrade(achievedZScore, Settings.GradingConfig, isGold, isWorst);
                _previousAchievedGrade = achievedResult.Grade;
                _previousAchievedColor = achievedResult.Color;

                // Generate small icon for the achieved grade - try custom icon first
                _previousAchievedIcon?.Dispose();
                _previousAchievedIcon = CustomIconLoader.GetCustomIconSmall(
                    achievedResult.Grade.Trim(),
                    Settings.GradingConfig,
                    GradeIconGenerator.SmallIconSize,
                    isGold,
                    isWorst);
                if (_previousAchievedIcon == null)
                {
                    _previousAchievedIcon = GradeIconGenerator.GenerateSmallIcon(achievedResult.Grade.Trim(), achievedResult.Color);
                }
            }
            else
            {
                _previousAchievedGrade = "-";
                _previousAchievedColor = state.LayoutSettings.TextColor;
                _previousAchievedIcon?.Dispose();
                _previousAchievedIcon = null;
            }

            // Calculate comparison grade if we have the time
            if (comparisonSegmentTime.HasValue)
            {
                double comparisonSeconds = comparisonSegmentTime.Value.TotalSeconds;
                double comparisonZScore = Statistics.CalculateZScore(comparisonSeconds, mean, stdDev);

                // Check for gold/worst using helper methods
                var bestSegment = prevSegment.BestSegmentTime[method];
                bool isGold = GradeCalculator.IsGoldSplit(comparisonSeconds, bestSegment?.TotalSeconds);
                bool isWorst = GradeCalculator.IsWorstSplit(comparisonSeconds, history);

                var comparisonResult = GradeCalculator.CalculateGrade(comparisonZScore, Settings.GradingConfig, isGold, isWorst);
                _previousComparisonGrade = comparisonResult.Grade;
                _previousComparisonColor = comparisonResult.Color;

                // Generate small icon for the comparison grade - try custom icon first
                _previousComparisonIcon?.Dispose();
                _previousComparisonIcon = CustomIconLoader.GetCustomIconSmall(
                    comparisonResult.Grade.Trim(),
                    Settings.GradingConfig,
                    GradeIconGenerator.SmallIconSize,
                    isGold,
                    isWorst);
                if (_previousComparisonIcon == null)
                {
                    _previousComparisonIcon = GradeIconGenerator.GenerateSmallIcon(comparisonResult.Grade.Trim(), comparisonResult.Color);
                }
            }
            else
            {
                _previousComparisonGrade = "-";
                _previousComparisonColor = state.LayoutSettings.TextColor;
                _previousComparisonIcon?.Dispose();
                _previousComparisonIcon = null;
            }

            _hasPreviousSplitData = true;
        }

        private (string Grade, Color Color) CalculateGradeForSplit(LiveSplitState state, int index)
        {
            return CalculateGradeForSplit(state, index, useActualTime: false);
        }

        private (string Grade, Color Color) CalculateGradeForSplit(LiveSplitState state, int index, bool useActualTime)
        {
            if (index < 0 || index >= state.Run.Count)
            {
                _cachedHistory.Clear();
                return ("-", state.LayoutSettings.TextColor);
            }

            // Check if cache needs to be invalidated (split completed or run changed)
            if (_lastGradeCalcSplitIndex != state.CurrentSplitIndex || _gradedRun != state.Run)
            {
                _gradeCalculationCache.Clear();
                _lastGradeCalcSplitIndex = state.CurrentSplitIndex;
            }

            // Check cache first
            var cacheKey = (index, useActualTime);
            if (_gradeCalculationCache.TryGetValue(cacheKey, out var cachedResult))
            {
                // For the current split being viewed (used by graph), we still need to populate the cached stats
                // Only return cached result if this isn't the primary display split
                if (index != _lastGradedIndex)
                {
                    return cachedResult;
                }
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
                var noDataResult = ("-", state.LayoutSettings.TextColor);
                _gradeCalculationCache[cacheKey] = noDataResult;
                return noDataResult;
            }

            _cachedMean = Statistics.CalculateMean(_cachedHistory);
            _cachedStdDev = Statistics.CalculateStandardDeviation(_cachedHistory);

            if (_cachedStdDev == 0)
            {
                _cachedHistory.Clear();
                var noStdDevResult = ("-", state.LayoutSettings.TextColor);
                _gradeCalculationCache[cacheKey] = noStdDevResult;
                return noStdDevResult;
            }

            // Get the segment time - either actual or comparison based on parameter
            TimeSpan? segmentTime = null;

            if (useActualTime)
            {
                // Use the actual segment time from the current run
                var splitTime = segment.SplitTime[method];
                var prevSplitTime = index > 0 ? state.Run[index - 1].SplitTime[method] : TimeSpan.Zero;

                if (splitTime != null && prevSplitTime != null)
                {
                    segmentTime = splitTime - prevSplitTime;
                }
            }
            else
            {
                // Use comparison segment time
                var comparison = state.CurrentComparison;
                var splitTime = segment.Comparisons[comparison][method];
                var prevSplitTime = index > 0 ? state.Run[index - 1].Comparisons[comparison][method] : TimeSpan.Zero;

                if (splitTime != null && prevSplitTime != null)
                {
                    segmentTime = splitTime - prevSplitTime;
                }
            }

            if (segmentTime != null)
            {
                _cachedComparisonTime = segmentTime.Value.TotalSeconds;
                _cachedZScore = Statistics.CalculateZScore(_cachedComparisonTime, _cachedMean, _cachedStdDev);

                // Check if this is a gold or worst split using helper methods
                var bestSegment = segment.BestSegmentTime[method];
                bool isGoldSplit = GradeCalculator.IsGoldSplit(_cachedComparisonTime, bestSegment?.TotalSeconds);
                bool isWorstSplit = GradeCalculator.IsWorstSplit(_cachedComparisonTime, _cachedHistory);

                var gradeResult = GradeCalculator.CalculateGrade(_cachedZScore, Settings.GradingConfig, isGoldSplit, isWorstSplit);
                _gradeCalculationCache[cacheKey] = gradeResult;
                return gradeResult;
            }

            var noTimeResult = ("-", state.LayoutSettings.TextColor);
            _gradeCalculationCache[cacheKey] = noTimeResult;
            return noTimeResult;
        }
    }
}
