using System.Collections.Generic;

namespace DoddleReport.Builder.Extentions
{
    public static class ExtensionsForIEnumerable
    {
        public static ExportBuilder<TModel> Export<TModel>(this IEnumerable<TModel> models) where TModel : class
        {
            return ExportBuilder<TModel>.Create(models);
        }
    }
}
