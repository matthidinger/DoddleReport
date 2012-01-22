using System;

namespace DoddleReport
{
    public class ReportRowEventArgs : EventArgs
    {
        public ReportRow Row { get; private set; }

        public ReportRowEventArgs(ReportRow row)
        {
            Row = row;
        }
    }
}
