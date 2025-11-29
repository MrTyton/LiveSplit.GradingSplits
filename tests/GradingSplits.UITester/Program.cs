using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.GradingSplits.UI.Components;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace GradingSplits.UITester
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.WriteLine("Starting UI Tester...");

            try
            {
                // 1. Test Factory
                Console.WriteLine("Testing Factory...");
                var factory = new GradingSplitsFactory();
                Console.WriteLine($"Factory Name: {factory.ComponentName}");

                // 2. Create State
                Console.WriteLine("Creating State...");
                var run = new Run(new StandardComparisonGeneratorsFactory());

                // Populate Run with Segments and History for Logic Testing
                var segment1 = new Segment("Segment 1");
                // Add History: 10s, 12s, 8s (Mean=10, StdDev=2)
                // Note: History keys are run IDs.
                segment1.SegmentHistory.Add(1, new Time(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
                segment1.SegmentHistory.Add(2, new Time(TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(12)));
                segment1.SegmentHistory.Add(3, new Time(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(8)));
                
                // Add Comparison (Personal Best): 9s
                // Z-Score = (9 - 10) / 2 = -0.5.
                var comparisons = new Time(TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(9));
                segment1.Comparisons["Personal Best"] = comparisons;

                run.Add(segment1);

                var form = new Form();
                var layoutSettings = new LiveSplit.Options.LayoutSettings();
                layoutSettings.TextFont = new Font("Arial", 10);
                layoutSettings.TimerFont = new Font("Arial", 20);
                layoutSettings.TimesFont = new Font("Arial", 10);
                layoutSettings.TextColor = Color.White;
                layoutSettings.ShadowsColor = Color.Black;
                layoutSettings.DropShadows = true;
                
                // Constructor: LiveSplitState(IRun run, Form form, ILayout layout, LayoutSettings layoutSettings, ISettings settings)
                var state = new LiveSplitState(run, form, null, layoutSettings, null);

                // 3. Create Component
                Console.WriteLine("Creating Component...");
                var component = factory.Create(state);
                Console.WriteLine($"Component Created: {component.ComponentName}");

                // 4. Test Settings Control
                Console.WriteLine("Getting Settings Control...");
                var settingsControl = component.GetSettingsControl(LayoutMode.Vertical);
                if (settingsControl == null)
                {
                    throw new Exception("Settings control is null!");
                }
                Console.WriteLine("Settings Control retrieved.");

                // Show settings in a dialog to verify it loads
                var settingsForm = new Form();
                settingsForm.Text = "Settings Test";
                settingsForm.AutoSize = true;
                settingsForm.Controls.Add(settingsControl);
                settingsControl.Dock = DockStyle.Fill;
                
                Console.WriteLine("Showing Settings Form (Close it to continue)...");
                settingsForm.ShowDialog();
                // settingsForm.Show();
                // settingsForm.Close();
                Console.WriteLine("Settings Form created and closed.");

                // 5. Test XML Settings
                Console.WriteLine("Testing XML Settings...");
                var doc = new XmlDocument();
                var settingsNode = component.GetSettings(doc);
                Console.WriteLine($"Settings XML: {settingsNode.OuterXml}");

                // Test SetSettings with the generated node
                component.SetSettings(settingsNode);
                Console.WriteLine("SetSettings (Generated) passed.");

                // Test SetSettings with empty/missing node (Simulate fresh add)
                var emptyNode = doc.CreateElement("Settings");
                component.SetSettings(emptyNode);
                Console.WriteLine("SetSettings (Empty) passed.");

                // 6. Test Drawing
                Console.WriteLine("Testing Draw...");
                var bitmap = new Bitmap(300, 100);
                using (var g = Graphics.FromImage(bitmap))
                {
                    // Draw Vertical
                    using (var region = new Region(new Rectangle(0, 0, 300, 100)))
                    {
                        component.DrawVertical(g, state, 300, region);
                        Console.WriteLine("DrawVertical passed.");

                        // Draw Horizontal
                        component.DrawHorizontal(g, state, 100, region);
                        Console.WriteLine("DrawHorizontal passed.");
                    }
                }

                // 7. Test Update Logic
                Console.WriteLine("Testing Update Logic...");
                state.CurrentPhase = TimerPhase.Running;
                state.CurrentSplitIndex = 0;
                state.CurrentComparison = "Personal Best";
                state.CurrentTimingMethod = TimingMethod.RealTime;

                component.Update(null, state, 100, 25, LayoutMode.Vertical);
                Console.WriteLine("Update passed (No Crash).");
                
                // Reflection to check GradeLabel text
                var gradeLabelProp = component.GetType().GetProperty("GradeLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (gradeLabelProp != null)
                {
                    var gradeLabel = gradeLabelProp.GetValue(component) as SimpleLabel;
                    Console.WriteLine($"Grade Label Text: {gradeLabel.Text}");
                }

                Console.WriteLine("All tests passed!");
                MessageBox.Show("All tests passed successfully!", "Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}", "Test Failed");
            }
        }
    }
}