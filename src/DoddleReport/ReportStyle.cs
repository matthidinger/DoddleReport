using System.Drawing;
using DoddleReport.Configuration;

namespace DoddleReport
{
    public class ReportStyle
    {
        public void ApplyStyle(StyleElement configElement)
        {
            Bold = configElement.Bold;
            Underline = configElement.Underline;
            Italic = configElement.Italic;

            BackColor = configElement.BackColor;
            ForeColor = configElement.ForeColor;

            FontSize = configElement.FontSize;
        }

        public static ReportStyle Default = new ReportStyle();

        public ReportStyle()
        {
            Bold = false;
            Underline = false;
            Italic = false;
            BackColor = Color.White;
            ForeColor = Color.Black;
            FontSize = 8;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Middle;
        }

        public ReportStyle(string styleName)
        {
            ApplyStyle(Config.Report.Styles[styleName]);
        }

        public static ReportStyle DataRowStyle
        {
            get { return new ReportStyle(Config.Report.DataRowStyleName); }
        }

        public static ReportStyle HeaderRowStyle
        {
            get { return new ReportStyle(Config.Report.DataRowStyleName); }
        }

        public static ReportStyle FooterRowStyle
        {
            get { return new ReportStyle(Config.Report.DataRowStyleName); }
        }

        // TODO: Move this to central config or something
        public ReportStyle(ReportRowType rowType)
        {
            switch (rowType)
            {
                case ReportRowType.DataRow:
                    ApplyStyle(Config.Report.Styles[Config.Report.DataRowStyleName]);
                    break;

                case ReportRowType.FooterRow:
                    ApplyStyle(Config.Report.Styles[Config.Report.FooterRowStyleName]);
                    break;

                case ReportRowType.HeaderRow:
                    ApplyStyle(Config.Report.Styles[Config.Report.HeaderRowStyleName]);
                    break;
            }
        }

        // TODO: Add Border properties

        public bool Bold { get; set; }
        public bool Underline { get; set; }
        public bool Italic { get; set; }

        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }

        public int FontSize { get; set; }

        public int Width { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

        internal ReportStyle Copy()
        {
            var style = new ReportStyle
            {
                Bold = Bold,
                Underline = Underline,
                Italic = Italic,

                BackColor = BackColor,
                ForeColor = ForeColor,

                FontSize = FontSize,

                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment

            };

            return style;
        }
    }
}