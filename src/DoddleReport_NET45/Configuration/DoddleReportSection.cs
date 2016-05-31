using System.Configuration;

namespace DoddleReport.Configuration
{
    public sealed class DoddleReportSection : ConfigurationSection
    {
        [ConfigurationProperty("writers", IsRequired = true)]
        public WriterElementCollection Writers
        {
            get { return (WriterElementCollection)this["writers"]; }
            set { this["writers"] = value; }
        }

        [ConfigurationProperty("styles")]
        public StyleElementCollection Styles
        {
            get { return (StyleElementCollection)this["styles"]; }
            set { this["styles"] = value; }
        }

        [ConfigurationProperty("defaultWriter", DefaultValue = "Html")]
        public string DefaultWriter
        {
            get { return (string)base["defaultWriter"]; }
            set { base["defaultWriter"] = value; }
        }

        [ConfigurationProperty("dataRowStyle", DefaultValue = "DataRowStyle")]
        public string DataRowStyleName
        {
            get { return (string) this["dataRowStyle"]; }
            set { this["dataRowStyle"] = value; }
        }

        [ConfigurationProperty("headerRowStyle", DefaultValue = "HeaderRowStyle")]
        public string HeaderRowStyleName
        {
            get { return (string) this["headerRowStyle"]; }
            set { this["headerRowStyle"] = value; }
        }

        [ConfigurationProperty("footerRowStyle", DefaultValue = "FooterRowStyle")]
        public string FooterRowStyleName
        {
            get { return (string) this["footerRowStyle"]; }
            set { this["footerRowStyle"] = value; }
        }
    }
}