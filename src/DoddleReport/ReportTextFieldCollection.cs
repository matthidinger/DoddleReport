using System.Collections.Generic;

namespace DoddleReport
{
    public class ReportTextFieldCollection
    {
        private readonly Dictionary<string, string> _internal = new Dictionary<string, string>();

        public string Title
        {
            get { return this["Title"]; }
            set { this["Title"] = value; }
        }

        public string SubTitle
        {
            get { return this["SubTitle"]; }
            set { this["SubTitle"] = value; }
        }


        public string Header
        {
            get { return this["Header"]; }
            set { this["Header"] = value; }
        }

        public string Footer
        {
            get { return this["Footer"]; }
            set { this["Footer"] = value; }
        }

        public string this[string field]
        {
            get
            {
                if (!_internal.ContainsKey(field))
                    return string.Empty;

                return _internal[field];

            }
            set
            {
                _internal[field] = value;
            }
        }
    }
}