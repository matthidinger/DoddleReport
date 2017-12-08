using System;

namespace DoddleReport.Builder
{
    public interface IExportColumn<in TModel> where TModel : class
    {
        string Header { get; }
        string Formula { get; }
        Func<TModel, object> Display { get; }
    }
}
