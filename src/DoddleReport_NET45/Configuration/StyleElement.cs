using System.Configuration;
using System.Drawing;

namespace DoddleReport.Configuration
{
    public sealed class StyleElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("bold", DefaultValue = false)]
        public bool Bold
        {
            get { return (bool) this["bold"]; }
            set { this["bold"] = value; }
        }

        [ConfigurationProperty("underline", DefaultValue = false)]
        public bool Underline
        {
            get { return (bool)this["underline"]; }
            set { this["underline"] = value; }
        }

        [ConfigurationProperty("italic", DefaultValue = false)]
        public bool Italic
        {
            get { return (bool)this["italic"]; }
            set { this["italic"] = value; }
        }

        [IntegerValidator(MinValue=6, MaxValue=72)]
        [ConfigurationProperty("fontSize", DefaultValue = 9)]
        public int FontSize
        {
            get { return (int) this["fontSize"]; }
            set { this["fontSize"] = value; }
        }

        [IntegerValidator(MinValue = -90, MaxValue = 90)]
        [ConfigurationProperty("textRotation", DefaultValue = 0)]
        public int TextRotation
        {
            get { return (int)this["textRotation"]; }
            set { this["textRotation"] = value; }
        }

        [ConfigurationProperty("backColor", DefaultValue = "White")]
        public string BackColorString
        {
            get { return (string) this["backColor"]; }
            set { this["backColor"] = value; }
        }

        [ConfigurationProperty("foreColor", DefaultValue = "Black")]
        public string ForeColorString
        {
            get { return (string) this["foreColor"]; }
            set { this["foreColor"] = value; }
        }

        public Color BackColor
        {
            get
            {
                var c = new ColorConverter();
                return (Color)c.ConvertFrom(BackColorString);
            }
            set { this["backColor"] = value.ToString(); }
        }

        public Color ForeColor
        {
            get
            {
                var c = new ColorConverter();
                return (Color)c.ConvertFrom(ForeColorString);
            }
            set { this["foreColor"] = value.ToString(); }
        }

        // TODO: Add Alignment enums

        internal void ApplyStyle(ReportStyle reportStyle)
        {
            reportStyle.Bold = Bold;
            reportStyle.Underline = Underline;
            reportStyle.Italic = Italic;

            reportStyle.BackColor = BackColor;
            reportStyle.ForeColor = ForeColor;

            reportStyle.FontSize = FontSize;
            reportStyle.TextRotation = TextRotation;
        }
    }
}