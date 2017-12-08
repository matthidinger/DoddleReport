using System;
using System.Linq.Expressions;

namespace DoddleReport.Builder
{
    public class ExportColumn<TModel, TProperty> : IExportColumn<TModel> where TModel : class
    {
        private readonly string _header;
        private readonly Expression<Func<TModel, TProperty>> _display;
        private string _formula;

        public string Header => _header;
        public string Formula => _formula;
        public Func<TModel, object> Display { get { return model => _display.Compile().Invoke(model); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExportColumn(string header, Expression<Func<TModel, TProperty>> display)
        {
            _header = header;
            _display = display; ;
        }

        public ExportColumn(string header, Expression<Func<TModel, TProperty>> property, string formula)
        {
            _header = header;
            _display = property;
            _formula = formula;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Header: {_header}, Display: {_display}";
        }
    }
}
