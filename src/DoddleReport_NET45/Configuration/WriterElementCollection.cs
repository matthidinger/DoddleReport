using System.Configuration;
using System;
using System.Linq;

namespace DoddleReport.Configuration
{
    [ConfigurationCollection(typeof(WriterElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class WriterElementCollection : ConfigurationElementCollection
    {
        public WriterElementCollection()
        {
            AddDefaults();
        }

        public void AddDefaults()
        {
            var htmlElement = new WriterElement
             {
                 Format = "Html",
                 TypeName = "DoddleReport.Writers.HtmlReportWriter, DoddleReport",
                 ContentType = "text/html",
                 FileExtension = ".htm"
             };

            var excelElement = new WriterElement
            {
                Format = "Excel",
                TypeName = "DoddleReport.Writers.ExcelReportWriter, DoddleReport",
                ContentType = "application/vnd.ms-excel",
                FileExtension = ".xls"
            };

            var txtElement = new WriterElement
            {
                Format = "Delimited",
                TypeName = "DoddleReport.Writers.DelimitedTextReportWriter, DoddleReport",
                ContentType = "text/plain",
                FileExtension = ".txt",
                OfferDownload = true
            };
   

            BaseAdd(htmlElement);
            BaseAdd(txtElement);
            BaseAdd(excelElement);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WriterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WriterElement)element).Format;
        }

        public WriterElement GetWriterConfigurationByFormat(string format)
        {
            var key = 
                BaseGetAllKeys().OfType<string>().FirstOrDefault(
                    k => k.Equals(format, StringComparison.OrdinalIgnoreCase));

            if(key == null)
                throw new ArgumentException(string.Format("Unable to locate a ReportWriter Configuration with the format '{0}'. Has this format been registered in web.config?", format));

            return ((WriterElement)BaseGet(key));
        }

        public WriterElement GetWriterConfigurationForFileExtension(string extension)
        {
            return
                BaseGetAllKeys()
                    .Cast<string>()
                    .Where(key =>
                           GetWriterConfigurationByFormat(key).FileExtension.Equals(extension,
                                                                                    StringComparison.InvariantCultureIgnoreCase))
                    .Select(GetWriterConfigurationByFormat)
                    .FirstOrDefault();
        }


        public IReportWriter GetWriterForFileExtension(string extension)
        {
            try
            {
                var writer = Activator.CreateInstance(GetWriterConfigurationForFileExtension(extension).Type) as IReportWriter;
                return writer;
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Unable to locate report writer by the name of '{0}'"));
            }
        }

        public IReportWriter GetWriterByName(string name)
        {
            try
            {
                var writer = Activator.CreateInstance(GetWriterConfigurationByFormat(name).Type) as IReportWriter;
                return writer;
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Unable to locate report writer by the name of '{0}'")); 
            }
        }


    }
}