using System.Web.Mvc;
using System.Web.Routing;

namespace DoddleReport.Web
{
    public static class RouteExtensions
    {
        /// <summary>
        /// Register the ReportingRoute within an MVC Area. This Route allows ReportResult Actions to be accessed via a file extension in the URL, for example http://localhost/MyController/MyActionResult.html to get an HTML Report
        /// </summary>
        public static Route MapReportingRoute(this AreaRegistrationContext context)
        {
            return MapReportingRoute(context, null, null);
        }

        /// <summary>
        /// Register the ReportingRoute within an MVC Area. This Route allows ReportResult Actions to be accessed via a file extension in the URL, for example http://localhost/MyController/MyActionResult.html to get an HTML Report
        /// </summary>
        public static Route MapReportingRoute(this AreaRegistrationContext context, string defaultController, string defaultAction)
        {
            return context.Routes.MapRoute(string.Format("{0}_DoddleReport", context.AreaName),
                                           context.AreaName + "/{controller}/{action}.{extension}",
                                           new { controller = defaultController, action = defaultAction },
                                           new { extension = new ReportRouteConstraint() }
                );
        }

        /// <summary>
        /// Register the ReportingRoute. This Route allows ReportResult Actions to be accessed via a file extension in the URL, for example http://localhost/MyController/MyActionResult.html to get an HTML Report
        /// </summary>
        public static Route MapReportingRoute(this RouteCollection routes)
        {
            return routes.MapReportingRoute(null, null);
        }

        /// <summary>
        /// Register the ReportingRoute. This Route allows ReportResult Actions to be accessed via a file extension in the URL, for example http://localhost/MyController/MyActionResult.html to get an HTML Report
        /// </summary>
        public static Route MapReportingRoute(this RouteCollection routes, string defaultController, string defaultAction)
        {
            return routes.MapRoute("DoddleReport",
                                   "{controller}/{action}.{extension}",
                                   new { controller = defaultController, action = defaultAction },
                                   new { extension = new ReportRouteConstraint() }
                );
        }
    }
}