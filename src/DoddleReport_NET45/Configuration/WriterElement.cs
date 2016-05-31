using System;
using System.Configuration;

namespace DoddleReport.Configuration
{
    public class WriterElement : ConfigurationElement
    {
        [ConfigurationProperty("format", IsRequired = true)]
        public string Format
        {
            get { return (string)this["format"]; }
            set { this["format"] = value; }
        }

        public Type Type
        {
            get
            {
                return Type.GetType(TypeName);
            }
            
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get
            {
                return (string)base["type"];
            }
            set
            {
                base["type"] = value;
            }
        }

        [ConfigurationProperty("contentType", DefaultValue = "text/html")]
        public string ContentType
        {
            get { return (string) this["contentType"]; }
            set { this["contentType"] = value; }
        }

        [ConfigurationProperty("offerDownload", DefaultValue = false)]
        public bool OfferDownload
        {
            get { return (bool) this["offerDownload"]; }
            set { this["offerDownload"] = value; }
        }

        [ConfigurationProperty("fileExtension", IsRequired = true)]
        public string FileExtension
        {
            get { return (string) this["fileExtension"]; }
            set { this["fileExtension"] = value; }
        }

        public IReportWriter LoadWriter()
        {
            try
            {
                var writer = Activator.CreateInstance(Type) as IReportWriter;
                return writer;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Unable to load the ReportWriter format '{0}' because the type '{1}' could not be created", Format, TypeName), ex);
            }
        }     
    }
}