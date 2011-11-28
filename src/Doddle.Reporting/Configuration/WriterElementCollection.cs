using System.Configuration;
using System.ComponentModel;
using System;
using System.IO;

namespace Doddle.Reporting.Configuration
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
                 TypeName = "Doddle.Reporting.Writers.HtmlReportWriter, Doddle.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=183ffec7490b24a9",
                 ContentType = "text/html",
                 FileExtension = ".htm"
             };

            var excelElement = new WriterElement
            {
                Format = "Excel",
                TypeName = "Doddle.Reporting.Writers.ExcelReportWriter, Doddle.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=183ffec7490b24a9",
                ContentType = "application/vnd.ms-excel",
                FileExtension = ".xls"
            };

            var txtElement = new WriterElement
            {
                Format = "Delimited",
                TypeName = "Doddle.Reporting.Writers.DelimitedTextReportWriter, Doddle.Reporting",
                ContentType = "text/plain",
                FileExtension = ".txt",
                OfferDownload = true
            };


            var pdfElement = new WriterElement
            {
                Format = "PDF",
                TypeName = "Doddle.Reporting.AbcPdf.PdfReportWriter, Doddle.Reporting.AbcPdf",
                ContentType = "application/pdf",
                FileExtension = ".pdf"
            };

   

            BaseAdd(htmlElement);
            BaseAdd(txtElement);
            BaseAdd(pdfElement);
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