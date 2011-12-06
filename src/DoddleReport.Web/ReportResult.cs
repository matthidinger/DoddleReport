using System;
using System.Web;
using System.Web.Mvc;
using System.IO;

using DoddleReport.Configuration;

namespace DoddleReport.Web
{
    public class ReportResult : ActionResult
    {
        private readonly Report _report;
        private IReportWriter _writer;
        private readonly string _contentType;

        /// <summary>
        /// This property is optional. 
        /// If you don't specify a FileName then the name of the ActionResult being executed will be used. 
        /// If you do specify a FileName, you may omit the file extension. If the file extension is omitted then DoddleReport will attempt to get the extension from the URL being requested
        /// </summary>
        public string FileName { get; set; }


        public ReportResult(Report report) : this(report, report.Writer)
        { }

        public ReportResult(Report report, IReportWriter writer, string contentType = null)
        {
            _report = report;
            _writer = writer;
            _contentType = contentType;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            string defaultExtension = Config.Report.Writers.GetWriterConfigurationByFormat(Config.Report.DefaultWriter).FileExtension;

            var response = context.HttpContext.Response;

            if (_writer == null)
            {
                var writerConfig = GetWriterFromExtension(context, defaultExtension);
                response.ContentType = writerConfig.ContentType;
                _writer = writerConfig.LoadWriter();
            }
            else
            {
                response.ContentType = _contentType;
            }

            if (!string.IsNullOrEmpty(FileName))
            {
                var extension = GetDownloadFileExtension(context.HttpContext.Request, defaultExtension);
                context.HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}{1}", FileName, extension));
            }


            _writer.WriteReport(_report, response.OutputStream);
        }

        protected virtual string GetDownloadFileExtension(HttpRequestBase request, string defaultExtension)
        {
            // Manual filename, don't override
            if (Path.HasExtension(FileName)) return "";

            // Extension passed in via in URL
            if (Path.HasExtension(request.RawUrl))
                return Path.GetExtension(request.RawUrl);

            return defaultExtension;
        }


        private static WriterElement GetWriterFromExtension(ControllerContext context, string defaultExtension)
        {

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
