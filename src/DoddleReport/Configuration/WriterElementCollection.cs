using System.Configuration;
using System;

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

        public new WriterElement this[string format]
        {
            get
            {
                return ((WriterElement)BaseGet(format));
            }
        }

        public WriterElement GetWriterConfigurationForFileExtension(string extension)
        {
            foreach (string key in BaseGetAllKeys())
            {
                if(this[key].FileExtension.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return this[key];
                }
            }
            return null;
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
                var writer = Activator.CreateInstance(this[name].Type) as IReportWriter;
                return writer;
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Unable to locate report writer by the name of '{0}'")); 
            }
        }


    }
}