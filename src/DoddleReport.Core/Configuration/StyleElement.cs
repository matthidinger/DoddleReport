namespace DoddleReport.Configuration
{
    public sealed class StyleElement
    {
        public string Name { get; set; }

        public bool Bold { get; set; }

        public bool Underline { get; set; }

        public bool Italic { get; set; }


        public int FontSize { get; set; }

        public int TextRotation { get; set; }

        public string BackColorString { get; set; }


        public string ForeColorString { get; set; }

        public Color BackColor { get; set; }

        public Color ForeColor { get; set; }
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

    // TODO: wire up color
    public class Color
    {
        
    }
}