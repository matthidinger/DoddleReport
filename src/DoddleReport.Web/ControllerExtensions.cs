using System.Web.Mvc;
using System.Web.Routing;

namespace DoddleReport.Web
{
    public static class ControllerExtensions
    {
        public static ReportResult ReportResult(this Controller controller, Report report)
        {
            return new ReportResult(report);
        }
    }

    public static class RouteExtensions
    {
        public static Route MapReportingRoute(this RouteCollection routes)
        {
            return routes.MapReportingRoute(null, null);
        }

        public static Route MapReportingRoute(this RouteCollection routes, string defaultController, string defaultAction)
        {
            return routes.MapRoute("DoddleReport",
                    "{controller}/{action}.{extension}",
                    new { controller = defaultAction, action = defaultAction },
                    new { extension = new ReportRouteConstraint() }
                );
        }
    }
}