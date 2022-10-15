using AutoMapper;
using Emaratech.DatabaseLayer;
using Emaratech.Services.Scheduler.Entities;
using Emaratech.Services.WcfCommons.Dynamic;
using log4net.Config;
using Ninject.Modules;
using Ninject.Web.Common;

namespace Emaratech.Services.Scheduler.DataAccess
{

    [NinjectConfigurationClass]
    public class ServiceNinjectModule : NinjectModule
    {
        [NinjectConfigurationMethod]
        public static void Configure(NinjectModule module)
        {
            module.Bind<IDbContext>().ToMethod(context => SchedulerDbProviders.GetDbContext()).InRequestScope();
            module.Bind<IUnitOfWork>().ToMethod(context => SchedulerDbProviders.GetUnitOfWork()).InRequestScope();
        }

        public override void Load()
        {

            XmlConfigurator.Configure();

            Configure(this);

            var mapperConfig = new MapperConfiguration(_ => { });
            mapperConfig.Configure();
            var mapper = mapperConfig.CreateMapper();
            this.Bind<IMapper>().ToConstant(mapper).InSingletonScope();
        }
    }
}