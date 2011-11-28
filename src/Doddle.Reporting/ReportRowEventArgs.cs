using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doddle.Reporting
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
