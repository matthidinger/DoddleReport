using System.IO;
using System.Web;
using DoddleReport.Configuration;

namespace DoddleReport.Web
{
    public abstract class WebReport : IHttpHandler
    {
        private WriterElement _elementConfig;

        /// <summary>
        /// The FileName to use for the downloaded report. 
        /// You may omit the file extension. 
        /// If the file extension is omitted then DoddleReport will attempt to get the correct extension for the requested ReportWriter from web.config
        /// </summary>
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
            string format = GetReportType(context);
            _elementConfig = Config.Report.Writers.GetWriterConfigurationByFormat(format);

            context.Response.ContentType = _elementConfig.ContentType;

            if (OfferDownload)
            {
                var extension = Path.HasExtension(FileName) ? "" : _elementConfig.FileExtension;
                context.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}{1}", FileName, extension));
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