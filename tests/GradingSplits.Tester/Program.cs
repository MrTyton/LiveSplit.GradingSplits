using System;
using System.IO;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.Model.RunFactories;
using LiveSplit.Model.Comparisons;
using LiveSplit.GradingSplits.Model;

namespace GradingSplits.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var splitsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\deps\splits\Final Fantasy X - PC (Any% English).lss"));

            if (!File.Exists(splitsPath))
            {
                Console.WriteLine($"File not found: {splitsPath}");
                return;
            }

            Console.WriteLine($"Loading splits from: {splitsPath}");

            try
            {
                IRun run;
                using (var stream = File.OpenRead(splitsPath))
                {
                    var factory = new StandardFormatsRunFactory(stream, splitsPath);
                    run = factory.Create(new StandardComparisonGeneratorsFactory());
                }

                Console.WriteLine($"Run Name: {run.GameName} - {run.CategoryName}");
                Console.WriteLine($"Attempt Count: {run.AttemptCount}");
                Console.WriteLine($"Segment Count: {run.Count}");
                Console.WriteLine("--------------------------------------------------");

                foreach (var segment in run)
                {
                    Console.WriteLine($"Segment: {segment.Name}");

                    // Extract history for GameTime or RealTime. Let's assume RealTime for now.
                    var method = TimingMethod.RealTime;
                    var historyTimes = segment.SegmentHistory
                        .Where(x => x.Value[method].HasValue)
                        .Select(x => x.Value[method].Value.TotalSeconds)
                        .ToList();

                    Console.WriteLine($"  History Count: {historyTimes.Count}");

                    if (historyTimes.Count < 2)
                    {
                        Console.WriteLine("  Not enough history to grade.");
                        continue;
                    }

                    var mean = Statistics.CalculateMean(historyTimes);
                    var stdDev = Statistics.CalculateStandardDeviation(historyTimes);

                    Console.WriteLine($"  Mean: {TimeSpan.FromSeconds(mean)}");
                    Console.WriteLine($"  StdDev: {TimeSpan.FromSeconds(stdDev)}");

                    // Let's test with the Personal Best time if it exists
                    if (segment.PersonalBestSplitTime[method].HasValue)
                    {
                        // Note: PB Split Time is cumulative. We need Segment Time.
                        // But for this test, let's just pick the *last* history item as a "test case"
                        var lastHistoryTime = historyTimes.Last();

                        var zScore = Statistics.CalculateZScore(lastHistoryTime, mean, stdDev);
                        var grade = GradeCalculator.CalculateGrade(zScore);

                        Console.WriteLine($"  Test Time: {TimeSpan.FromSeconds(lastHistoryTime)}");
                        Console.WriteLine($"  Z-Score: {zScore:F2}");
                        Console.WriteLine($"  Grade: {grade.Grade}");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
