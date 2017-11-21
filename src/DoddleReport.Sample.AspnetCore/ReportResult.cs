using DoddleReport.iTextSharp;
using Microsoft.AspNetCore.Mvc;

namespace DoddleReport.Sample.AspnetCore
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

        public override void ExecuteResult(ActionContext context)
        {
            var response = context.HttpContext.Response;

            if (_writer == null)
            {
                //var writerConfig = GetWriterFromExtension(context, defaultExtension);
                //response.ContentType = writerConfig.ContentType;
                //_writer = writerConfig.LoadWriter();
                _writer = new PdfReportWriter();
            }
            else
            {
                response.ContentType = _contentType;
            }

            if (!string.IsNullOrEmpty(FileName))
            {
                //var extension = GetDownloadFileExtension(context.HttpContext.Request, defaultExtension);
                var extension = ".pdf";
                response.Headers["content-disposition"] = string.Format("attachment; filename={0}{1}", FileName, extension);
            }

            //response.RegisterForDispose()
            _writer.WriteReport(_report, response.Body);
        }
    }
}