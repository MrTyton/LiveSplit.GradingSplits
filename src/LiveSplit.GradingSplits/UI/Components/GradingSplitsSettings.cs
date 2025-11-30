using System;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.UI;

namespace LiveSplit.GradingSplits.UI.Components
{
    public partial class GradingSplitsSettings : UserControl
    {
        public GradingSplitsSettings()
        {
            InitializeComponent();
        }

        private void GradingSplitsSettings_Load(object sender, EventArgs e)
        {
        }

        public void SetSettings(XmlNode node)
        {
            // No settings to load for now
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return 0;
        }
    }
}
