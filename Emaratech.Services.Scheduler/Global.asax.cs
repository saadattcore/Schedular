using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Activation;
using System.Web.Routing;
using Emaratech.Services.HealthCheck.EF;
using Emaratech.Services.Scheduler.Contracts.DataAccess;
using Emaratech.Services.Scheduler.DataAccess;
using Emaratech.Services.WcfCommons.Dynamic;
using Emaratech.Services.WcfCommons.HealthCheck.Rest;
using Emaratech.Services.WcfCommons.Rest.Modules;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using SwaggerWcf;
using ServiceNinjectModule = Emaratech.Services.Scheduler.App_Start.ServiceNinjectModule;

namespace Emaratech.Services.Scheduler
{
    public class Global : NinjectHttpApplication
    {
        /// <summary>
        /// Creates the kernel that will manage application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        protected override IKernel CreateKernel()
        {
            var hostingModel = (DynamicHostingModel)Enum.Parse(typeof(DynamicHostingModel), ConfigurationManager.AppSettings["DataAccessHostingModel"] ?? DynamicHostingModel.InProcess.ToString());
            var modules = new List<INinjectModule>()
            {
                new ServiceNinjectModule(),
                new LoggingInitializationNinjectModule(),
                new SwaggerInitializationNinjectModule()
            };
            if (hostingModel == DynamicHostingModel.InProcess)
            {
                modules.Add(new AutoMapperInitializationNinjectModule());
                modules.Add(new InProcessWcfClientInitializationNinjectModule<ISchedulerDataAccessService,SchedulerDataAccessService>());
                modules.Add(new ExternalModuleInitializationNinjectModule());
                modules.Add(new InProcessHealthCheckInitializationModule<EFHealthcheckImpl>());
            }
            else
            {
                modules.Add(new AutoMapperInitializationNinjectModule(Assembly.GetExecutingAssembly()));
                modules.Add(
                    new RemoteWcfClientInitializationNinjectModule<ISchedulerDataAccessService>(
                        "BasicHttpBinding_ISchedulerDataAccessService"));
                modules.Add(new ServiceHealthCheckInitializationModule("BasicHttpBinding_IDataAccessHealthCheck"));
            }
            return new StandardKernel(modules.ToArray());
        }
        
    }
}