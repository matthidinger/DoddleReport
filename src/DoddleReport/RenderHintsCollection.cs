using System.Collections.Generic;
using System.Drawing;

namespace DoddleReport
{
    public class RenderHintsCollection
    {
        private readonly Dictionary<string, object> _internal = new Dictionary<string, object>();

        public static SizeF DefaultMargins = new SizeF(20f, 20f);

        public RenderHintsCollection()
        {
            BooleanCheckboxes = false;
            BooleansAsYesNo = false;
            Margins = DefaultMargins;
        }

        public bool ContainsKey(string hint)
        {
            return _internal.ContainsKey(hint);
        }

        /// <summary>
        /// Rendering Margins. Specified in Pixels, but may be interpreted different based on the IReportWriter
        /// </summary>
        public SizeF Margins
        {
            get { return (SizeF)this["Margins"]; }
            set { this["Margins"] = value; }
        }

        public bool BooleansAsYesNo
        {
            get { return (bool)this["BooleansAsYesNo"]; }
            set { this["BooleansAsYesNo"] = value; }
        }

        public bool BooleanCheckboxes
        {
            get { return (bool) this["BooleanCheckboxes"]; }
            set { this["BooleanCheckboxes"] = value; }
        }

        public bool IncludePageNumbers
        {
            get { return this["IncludePageNumbers"] as bool? ?? true; }
            set { this["IncludePageNumbers"] = value; }
        }

        public ReportOrientation Orientation
        {
            get
            {
                return this["Orientation"] as ReportOrientation? ?? ReportOrientation.Landscape;
            }
            set
            {
                this["Orientation"] = value;
            }
        }


        public object this[string hint]
        {
            get
            {
                if (!_internal.ContainsKey(hint))
                    return null;

                return _internal[hint];
            }
            set
            {
                _internal[hint] = value;
            }
        }
    }
}