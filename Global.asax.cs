using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CmsShoppingCart
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthenticateRequest()
        {
            //check if user is logged in
            if (User == null) { return; }
            //get usernames
            string username = Context.User.Identity.Name;
            //declare array of roles
            string[] roles;
            using(Contextdb db=new Contextdb())
            {
                //populate roles
                UsersDTO dto = db.users.FirstOrDefault(x => x.Username == username);
                roles = db.UserRoles.Where(x => x.Userid == dto.id).Select(x => x.Roles.Name).ToArray();
            }


            //build IPrinciple object
            IIdentity useridentity = new GenericIdentity(username);
            IPrincipal userobj = new GenericPrincipal(useridentity, roles);
            //update context user
            Context.User = userobj;
        }
    }
}
