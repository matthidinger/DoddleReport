using System.Web.Mvc;
using System.Web.Routing;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //DelimitedTextReportWriter.GetHeaderText = field => field.HeaderText;


            routes.MapRoute("LegacyUrl",
                    "home/{action}.{extension}",
                    new { controller = "Doddle" },
                    new { extension = new ReportRouteConstraint() }
                );

            routes.MapReportingRoute();

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, area = "" } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}