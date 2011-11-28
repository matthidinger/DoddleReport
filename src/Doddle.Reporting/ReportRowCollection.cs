using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Doddle.Reporting
{
    public class ReportRowCollection : Collection<ReportRow>
    {
        public event EventHandler<ReportRowEventArgs> RowAdding;

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
