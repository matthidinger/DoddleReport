namespace DoddleReport.Configuration
{
    public static class Config
    {
        public static DoddleReportSection Report
        {
            get
            {
                // TODO: blows up on netstandard
                //var section = ConfigurationManager.GetSection("doddleReport") as DoddleReportSection;
                //return section ?? new DoddleReportSection();
                return new DoddleReportSection();
            }
        }
    }
}