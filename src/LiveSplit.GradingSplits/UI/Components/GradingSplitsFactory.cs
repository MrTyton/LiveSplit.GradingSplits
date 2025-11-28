using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

namespace LiveSplit.GradingSplits.UI.Components
{
    public class GradingSplitsFactory : IComponentFactory
    {
        public string ComponentName => "Grading Splits";

        public string Description => "Displays a grade (S, A, B, etc.) for your splits based on historical performance.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new GradingSplitsComponent(state);

        public string UpdateName => ComponentName;

        public string XMLURL => ""; // Update URL if hosted somewhere

        public string UpdateURL => ""; // Update URL if hosted somewhere

        public Version Version => Version.Parse("1.0.0");
    }
}
