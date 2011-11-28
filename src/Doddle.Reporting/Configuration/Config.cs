using System.Configuration;

namespace Doddle.Reporting.Configuration
{
    public static class Config
    {
        public static ReportSection Report
        {
            get
            {
                var section = ConfigurationManager.GetSection("doddle/reporting") as ReportSection;
                return section ?? new ReportSection();
            }
        }

    }
}