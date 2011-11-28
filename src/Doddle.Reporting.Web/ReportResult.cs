using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Doddle.Reporting;
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

            WriterElement e = Config.Report.Writers.GetWriterConfigurationForFileExtension(extension);
            if(e == null)
                throw new InvalidOperationException(string.Format("Unable to locate a report writer for the extension '{0}'. Did you add this fileExtension to the web.config for DoddleReport?", extension));

            var writer = e.LoadWriter();

            context.HttpContext.Response.ContentType = e.ContentType;
            
            writer.WriteReport(_report, context.HttpContext.Response.OutputStream);
        }
    }
}
