using LiveSplit.GradingSplits.Model;
using System;
using System.Windows.Forms;

namespace LiveSplit.GradingSplits.UI.Components
{
    /// <summary>
    /// Settings control for the Grading Splits component.
    /// This partial class contains the core fields, properties, and constructor.
    /// </summary>
    public partial class GradingSplitsSettings : UserControl
    {
        /// <summary>
        /// The grading configuration that backs this settings control.
        /// </summary>
        public GradingSettings GradingConfig { get; set; }

        /// <summary>
        /// Initializes a new instance of the GradingSplitsSettings control.
        /// </summary>
        public GradingSplitsSettings()
        {
            InitializeComponent();
            GradingConfig = new GradingSettings();
            Load += GradingSplitsSettings_Load;
            WireUpEventHandlers();
            LoadSettingsToUI();
        }
    }
}
