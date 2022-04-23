using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CmsShoppingCart
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { Controller = "Cart", Action = "index", id = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("shop", "shop/{action}/{name}", new { Controller = "shop", Action = "index", name = UrlParameter.Optional }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("SidebarPartial", "page/SidebarPartial", new { Controller = "page", Action = "SidebarPartial" }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("PageMenuPartial", "page/PageMenuPartial", new { Controller = "page", Action = "PageMenuPartial" }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Page", "{page}", new { Controller = "page", Action = "index" }, new[] { "CmsShoppingCart.Controllers" });
            routes.MapRoute("Default", "", new { Controller = "page", Action = "index" }, new[] { "CmsShoppingCart.Controllers" });
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
