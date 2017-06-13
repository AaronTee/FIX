using FIX.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FIX.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Localization route - it will be used as a route of the first priority 
            routes.MapRoute(
                name: "DefaultLocalized",
                url: "{lang}/{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Account",
                    action = "Login",
                    id = UrlParameter.Optional,
                },
                constraints: new { lang = new CultureConstraint(defaultCulture: "en", pattern: "[a-z]{2}") }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { lang = "en", controller = "Account", action = "Login", id = UrlParameter.Optional }
            );

            
        }
    }
}
