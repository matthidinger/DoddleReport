using System.Collections.Generic;
using System.Drawing;

namespace DoddleReport
{
    /// <summary>
    /// Render hints are passed to each report writer to alter their rendering behavior. Not all render hints are supported in every writer
    /// </summary>
    public class RenderHintsCollection
    {
        private readonly Dictionary<string, object> _internal = new Dictionary<string, object>();

        public static SizeF DefaultMargins = new SizeF(20f, 20f);
        public static SizeF DefaultPageSize = new SizeF(612f, 792f); //default to letter size in points 8.5*72, 11*72

        public RenderHintsCollection()
        {
            BooleanCheckboxes = false;
            BooleansAsYesNo = false;
            Margins = DefaultMargins;
            PageSize = DefaultPageSize;
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

        /// <summary>
        /// Page Size of the document. Use .Width and .Height to specify the Page Size.  
        /// For PDFs, the unit of measure is in points (72 points = 1 inch), but may be interpreted differently based on the IReportWriter.
        /// <example>Example: to set 8.5in x 11in PageSize: report.RenderHints.PageSize = new SizeF(8.5f * 72f, 11f * 72f);</example>
        /// </summary>
        public SizeF PageSize 
        { 
            get{ return (SizeF)this["PageSize"];}
            set { this["PageSize"] = value; }
        }

        /// <summary>
        /// Boolean fields will render as Yes/No instead of true/false on the reports
        /// </summary>
        public bool BooleansAsYesNo
        {
            get { return (bool)this["BooleansAsYesNo"]; }
            set { this["BooleansAsYesNo"] = value; }
        }

        /// <summary>
        /// Boolean fields will render as Checkboxes in certain report writers
        /// </summary>
        public bool BooleanCheckboxes
        {
            get { return (bool) this["BooleanCheckboxes"]; }
            set { this["BooleanCheckboxes"] = value; }
        }

        /// <summary>
        /// Page numbers will be rendered onto the footer in certain report writers
        /// </summary>
        public bool IncludePageNumbers
        {
            get { return this["IncludePageNumbers"] as bool? ?? true; }
            set { this["IncludePageNumbers"] = value; }
        }

        /// <summary>
        /// Toggle the orientation if the report writer supports it
        /// </summary>
        public ReportOrientation Orientation
        {
            get
            {
                return this["Orientation"] as ReportOrientation? ?? ReportOrientation.Portrait;
            }
            set
            {
                this["Orientation"] = value;
            }
        }

        /// <summary>
        /// Indicates if Freeze Panes is enabled based on current settings of FreezeRows and FreezeColumns
        /// </summary>
        public bool FreezePanes
        {
            get { return FreezeRows + FreezeColumns > 0; }
        }

        /// <summary>
        /// Freeze rows
        /// </summary>
        public int FreezeRows
        {
            get { return this["FreezeRows"] as int? ?? 0; }
            set { this["FreezeRows"] = value; }
        }

        /// <summary>
        /// Freeze columns
        /// </summary>
        public int FreezeColumns
        {
            get { return this["FreezeColumns"] as int? ?? 0; }
            set { this["FreezeColumns"] = value; }
        }

        /// <summary>
        /// Use this to pass arbitrary render hints to a specific report writer
        /// </summary>
        /// <param name="hint">The name of the render hint. The Report Writer must be looking for this hint by name to have any affect</param>
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