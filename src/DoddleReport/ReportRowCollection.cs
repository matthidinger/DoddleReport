using System;
using System.Collections.ObjectModel;

namespace DoddleReport
{
    public class ReportRowCollection : Collection<ReportRow>
    {
        public Report Report { get; private set; }

        public event EventHandler<ReportRowEventArgs> RowAdding;

        public ReportRowCollection(Report report)
        {
            Report = report;
        }

        protected override void InsertItem(int index, ReportRow item)
        {
            var handler = RowAdding;
            if (handler != null)
            {
                handler(this, new ReportRowEventArgs(item));
            }
            base.InsertItem(index, item);
        }
    }
}
