using System;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.UI;

namespace LiveSplit.GradingSplits.UI.Components
{
    public enum GradingMode
    {
        Single,
        List
    }

    public partial class GradingSplitsSettings : UserControl
    {
        public GradingMode Mode { get; set; }

        public GradingSplitsSettings()
        {
            InitializeComponent();
            Mode = GradingMode.Single;
            cmbMode.SelectedIndexChanged += CmbMode_SelectedIndexChanged;
        }

        private void CmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Mode = (GradingMode)cmbMode.SelectedIndex;
        }

        private void GradingSplitsSettings_Load(object sender, EventArgs e)
        {
            cmbMode.SelectedIndex = (int)Mode;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Mode = SettingsHelper.ParseEnum(element["Mode"], GradingMode.Single);
            cmbMode.SelectedIndex = (int)Mode;
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
            return SettingsHelper.CreateSetting(document, parent, "Mode", Mode);
        }
    }
}
