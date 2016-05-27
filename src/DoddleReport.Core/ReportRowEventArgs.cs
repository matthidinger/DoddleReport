using System;

namespace DoddleReport
{
    public class ReportRowEventArgs : EventArgs
    {
        /// <summary>
        /// The row being rendered
        /// </summary>
        public ReportRow Row { get; private set; }

        public ReportRowEventArgs(ReportRow row)
        {
            Row = row;
        }
    }
}
