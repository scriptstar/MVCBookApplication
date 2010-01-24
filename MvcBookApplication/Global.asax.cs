using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Models;
using Ninject.Core;
using Ninject.Framework.Mvc;

namespace MvcBookApplication
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "BrowseContacts",
                "contact/browse/{page}",
                new
                    {
                        controller = "Contact",
                        action = "browse",
                        page = 1
                    }
                );
            routes.MapRoute(
                "BrowseMessages",
                "message/browse/{page}",
                new
                {
                    controller = "message",
                    action = "browse",
                    page = 1
                }
                );
            routes.MapRoute(
                "Default",                    // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = ""
                }                             // Parameter defaults
            );

        }

        private StandardKernel _kernel;

        protected override IKernel CreateKernel()
        {
            IModule[] modules = new IModule[]
                        {
                            new AutoControllerModule(
                                Assembly.GetExecutingAssembly()), 
                            new InMemoryModule()
                        };
            _kernel = new StandardKernel(modules);
            return _kernel;
        }
    }

    //public class MvcApplication : System.Web.HttpApplication
    //{
    //    protected void RegisterRoutes(RouteCollection routes)
    //    {
    //        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

    //        routes.MapRoute(
    //            "Default",                    // Route name
    //            "{controller}/{action}/{id}", // URL with parameters
    //            new
    //            {
    //                controller = "Home",
    //                action = "Index",
    //                id = ""
    //            }                             // Parameter defaults
    //        );

    //    }


    //    protected void Application_Start()
    //    {
    //        RegisterRoutes(RouteTable.Routes);
    //    }
    //}
}