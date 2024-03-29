﻿using Autofac;
using Autofac.Integration.Mvc;
using BusinessHandler.MessageHandler;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MIMap
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


    }
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<SqlServerUserRepository>().As<IUserRepository>();
            builder.RegisterType<SqlServerSearchQueryRepository>().As<ISearchQueryRepository>();
            builder.RegisterType<SqlServerMeetingNote>().As<IMeetingNote>();
            builder.RegisterType<SqlServerMapDataRepository>().As<IMapDataRepository>();
            builder.RegisterType<KeyWordRepository>().As<IKeyWord>();
            builder.RegisterType<DocumentRepository>().As<IDocumentRepository>();
            builder.RegisterType<DynamicPriceRepository>().As<IDynamicPriceRepository>();
            builder.RegisterFilterProvider();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
