using System.Web.Mvc;
using System.Web.Routing;
using DoddleReport.Web;
using DoddleReport.Writers;

namespace DoddleReport.Sample.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

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
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}