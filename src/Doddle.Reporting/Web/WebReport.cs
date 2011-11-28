using System;
using System.IO;
using System.Web;
using Doddle.Reporting;
using Doddle.Reporting.Configuration;

namespace Doddle.Reporting.Web
{
    public abstract class WebReport : IHttpHandler
    {
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
                return elementConfig.OfferDownload;
            }
        }

        private WriterElement elementConfig;
        public virtual void ProcessRequest(HttpContext context)
        {
            Context = context;
            string format = GetReportType(context);
            elementConfig = Config.Report.Writers[format];

            context.Response.ContentType = elementConfig.ContentType;

            if (OfferDownload)
            {
                context.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}{1}", FileName, elementConfig.FileExtension));
            }

            Report report = new Report();
            report.Source = ReportSource;
            
            CustomizeReport(report);

            IReportWriter writer = elementConfig.LoadWriter();
            writer.WriteReport(report, context.Response.OutputStream);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}