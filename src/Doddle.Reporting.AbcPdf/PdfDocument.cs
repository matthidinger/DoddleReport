using System.IO;
using WebSupergoo.ABCpdf7;

namespace Doddle.Reporting.AbcPdf
{
    public class PdfDocument : Doc
    {
        public PdfDocument()
            : this(ReportOrientation.Portrait)
        { }

        public PdfDocument(ReportOrientation orientation)
        {
            
            this.FontSize = 6;
            this.Rect.Inset(5, 5);
            this.Orientation = orientation;
        }

        private ReportOrientation _orientation;

        public ReportOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                if (value == ReportOrientation.Landscape)
                    SetLandscape();
            }
        }

        public void Write(Stream destination)
        {
            base.Save(destination);
        }

        private void SetLandscape()
        {
            // apply a rotation transform
            double w = this.MediaBox.Width;
            double h = this.MediaBox.Height;
            double l = this.MediaBox.Left;
            double b = this.MediaBox.Bottom;
            this.Transform.Rotate(90, l, b);
            this.Transform.Translate(w, 0);

            // rotate our rectangle
            this.Rect.Width = h;
            this.Rect.Height = w;

            int theID = this.GetInfoInt(this.Root, "Pages");
            this.SetInfo(theID, "/Rotate", "90");
        }
    }
}
