using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DoddleReport.Sample.AspnetCore
{
    public class ReportRouteConstraint : IRouteConstraint
    {

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration)
            {
                return true;
            }


            return false;
            // TODO: implement ReportRoute
            //var configuredExtensions = Config.Report.Writers.OfType<WriterElement>().Select(e => e.FileExtension).ToList();

            //var requestExtension = Path.GetExtension(httpContext.Request.Url.AbsolutePath);
            //return configuredExtensions.Contains(requestExtension, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}