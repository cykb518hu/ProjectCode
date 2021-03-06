﻿
using Autofac;
using Autofac.Integration.Mvc;
using BusinessHandler.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SingleApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.ConfigureContainer();
        }
        public class AutofacConfig
        {
            public static void ConfigureContainer()
            {
                var builder = new ContainerBuilder();
                builder.RegisterControllers(typeof(MvcApplication).Assembly);
                builder.RegisterType<DocQueryCSVRepository>().As<IDocQueryRepository>();
                builder.RegisterType<AspNetCacheRepository>().As<ICacheRepository>();
                builder.RegisterType<UserRepository>().As<IUserRepository>();
                builder.RegisterType<SearchQueryRepository>().As<ISearchQueryRepository>();
                builder.RegisterType<XmlHelper>().As<IDataFileHelper>();
                builder.RegisterType<KeyWordRepository>().As<IKeyWord>();
                builder.RegisterFilterProvider();

                var container = builder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            }
        }
    }
}
