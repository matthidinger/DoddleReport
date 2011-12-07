using System.Collections.Generic;

namespace DoddleReport
{
    public class RenderHintsCollection
    {
        private readonly Dictionary<string, object> _internal = new Dictionary<string, object>();

        public RenderHintsCollection()
        {
            BooleanCheckboxes = false;
            BooleansAsYesNo = false;
        }

        public bool ContainsKey(string hint)
        {
            return _internal.ContainsKey(hint);
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