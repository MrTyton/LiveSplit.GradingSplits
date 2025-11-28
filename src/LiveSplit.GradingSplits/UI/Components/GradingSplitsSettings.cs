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
            cmbMode.DataBindings.Add("SelectedIndex", this, "Mode", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Mode = (GradingMode)Enum.Parse(typeof(GradingMode), element["Mode"].InnerText);
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
