using System;
using System.Web.Mvc;
using Doddle.Reporting.Configuration;
using System.IO;

namespace Doddle.Reporting.Web
{
    public class ReportResult : ActionResult
    {
        private readonly Report _report;
        private IReportWriter _writer;
        private readonly string _contentType;

        public ReportResult(Report report)
        {
            _report = report;
        }

        public ReportResult(Report report, IReportWriter writer, string contentType = null)
        {
            _report = report;
            _writer = writer;
            _contentType = contentType;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            if(_writer == null)
            {
                var writerConfig = GetWriterFromExtension(context);
                response.ContentType = writerConfig.ContentType;
                _writer = writerConfig.LoadWriter();
            }
            else
            {
                response.ContentType = _contentType;               
            }

            _writer.WriteReport(_report, response.OutputStream);
        }

        private static WriterElement GetWriterFromExtension(ControllerContext context)
        {
            string defaultExtension = Config.Report.Writers[Config.Report.DefaultWriter].FileExtension;

            // attempt to get the report format from the extension on the URL (ex. "/action/controller.pdf" yields ".pdf")
            string extension = Path.GetExtension(context.HttpContext.Request.RawUrl);

            if (string.IsNullOrEmpty(extension))
            {
                extension = defaultExtension;
            }

            var writerConfig = Config.Report.Writers.GetWriterConfigurationForFileExtension(extension);
            if (writerConfig == null)
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to locate a report writer for the extension '{0}'. Did you add this fileExtension to the web.config for DoddleReport?",
                        extension));

            return writerConfig;
        }
    }
}
