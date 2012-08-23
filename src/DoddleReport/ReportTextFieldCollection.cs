using System.Collections.Generic;

namespace DoddleReport
{
    /// <summary>
    /// Text fields are passed to the report writers to render the data as they see fit
    /// </summary>
    public class ReportTextFieldCollection
    {
        private readonly Dictionary<string, string> _internal = new Dictionary<string, string>();

        /// <summary>
        /// The primary title of the report. Typically rendered in a large centered font at the top of every page
        /// </summary>
        public string Title
        {
            get { return this["Title"]; }
            set { this["Title"] = value; }
        }

        /// <summary>
        /// A subtitle to further clarity the report. Typically rendered just below the Title
        /// </summary>
        public string SubTitle
        {
            get { return this["SubTitle"]; }
            set { this["SubTitle"] = value; }
        }

        /// <summary>
        /// Commonly used as a multi-line block of text to describe what this report is showing. 
        /// </summary>
        public string Header
        {
            get { return this["Header"]; }
            set { this["Header"] = value; }
        }

        /// <summary>
        /// The footer that may be displayed on every page, depending on the report writer
        /// </summary>
        public string Footer
        {
            get { return this["Footer"]; }
            set { this["Footer"] = value; }
        }

        /// <summary>
        /// Use this to pass arbitrary text fields to a specific report writer
        /// </summary>
        /// <param name="field">The name of the text field. The Report Writer must be looking for this text field by name to have any affect</param>
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