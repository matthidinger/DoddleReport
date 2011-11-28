using System;
using System.Web.Mvc;
using Doddle.Reporting.Configuration;
using System.IO;

namespace Doddle.Reporting.Web
{
    public class ReportResult : ActionResult
    {
        private readonly Report _report;

        public ReportResult(Report report)
        {
            _report = report;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string defaultExtension = Config.Report.Writers[Config.Report.DefaultWriter].FileExtension;

            string extension = Path.GetExtension(context.HttpContext.Request.RawUrl);
            if(string.IsNullOrEmpty(extension))
            {
                extension = defaultExtension;
            }

            var writerConfig = Config.Report.Writers.GetWriterConfigurationForFileExtension(extension);
            if(writerConfig == null)
                throw new InvalidOperationException(string.Format("Unable to locate a report writer for the extension '{0}'. Did you add this fileExtension to the web.config for DoddleReport?", extension));

            var writer = writerConfig.LoadWriter();
            var response = context.HttpContext.Response;
            response.ContentType = writerConfig.ContentType;
            writer.WriteReport(_report, response.OutputStream);
        }
    }
}
