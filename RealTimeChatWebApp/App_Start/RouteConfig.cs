using System.Web.Mvc;
using System.Web.Routing;

namespace RealTimeChatWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ignore dynamic image handlers e.g. Charts/Summary.ashx?year=2008&month=08
            routes.IgnoreRoute("Charts/{*path}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute("Chat",
            //                "{controller}/{action}/{id}",
            //                new { controller = "Chat", action = "Index", id = UrlParameter.Optional });

            // All other pages use the default route.
            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new { controller = "Chat", action = "Index", id = UrlParameter.Optional }
            );

            //routes.RouteExistingFiles = true;
        }
    }
}
