using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Doddle.Reporting.Configuration;
using System.IO;

namespace Doddle.Reporting.Web
{
    public class ReportRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration)
            {
                return true;
            }


            var configuredExtensions = Config.Report.Writers.OfType<WriterElement>().Select(e => e.FileExtension).ToList();

            var requestExtension = Path.GetExtension(httpContext.Request.RawUrl);
            return configuredExtensions.Contains(requestExtension, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}