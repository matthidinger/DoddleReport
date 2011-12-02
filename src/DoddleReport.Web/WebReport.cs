using System.Web;
using DoddleReport.Configuration;

namespace DoddleReport.Web
{
    public abstract class WebReport : IHttpHandler
    {
        private WriterElement _elementConfig;

        public abstract string FileName { get; }
        public abstract void CustomizeReport(Report report);
        public abstract IReportSource ReportSource { get; }

        public virtual string DefaultReportType
        {
            get
            {
                return Config.Report.DefaultWriter;
            }
        }

        public HttpContext Context { get; set; }

        public string GetReportType(HttpContext context)
        {
            string reportType = context.Request["ReportType"];

            if (string.IsNullOrEmpty(reportType))
                return DefaultReportType;

            return reportType;
        }

        protected virtual bool OfferDownload
        {
            get
            {
                return _elementConfig.OfferDownload;
            }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            Context = context;
            string format = GetReportType(context);
            _elementConfig = Config.Report.Writers[format];

            context.Response.ContentType = _elementConfig.ContentType;

            if (OfferDownload)
            {
                context.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}{1}", FileName, _elementConfig.FileExtension));
            }

            var report = new Report { Source = ReportSource };

            CustomizeReport(report);
             
            var writer = _elementConfig.LoadWriter();
            writer.WriteReport(report, context.Response.OutputStream);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}