using System.Configuration;

namespace DoddleReport.Configuration
{
    public static class Config
    {
        public static DoddleReportSection Report
        {
            get
            {
                var section = ConfigurationManager.GetSection("doddleReport") as DoddleReportSection;
                return section ?? new DoddleReportSection();
            }
        }

    }
}