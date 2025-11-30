using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

namespace LiveSplit.GradingSplits.UI.Components
{
    public class GradingSplitsFactory : IComponentFactory
    {
        public string ComponentName => "Grading Splits";

        public string Description => "Displays a grade (S, A, B, C, D, F) for the last completed split based on its time relative to history.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new GradingSplitsComponent(state);

        public string UpdateName => ComponentName;

        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.GradingSplits.xml";

        public string UpdateURL => "http://livesplit.org/update/";

        public Version Version => Version.Parse("1.0.0");
    }
}
