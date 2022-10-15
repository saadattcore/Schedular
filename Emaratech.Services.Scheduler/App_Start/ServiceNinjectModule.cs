using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Emaratech.Services.Scheduler.Contracts.DataAccess;
using Emaratech.Services.WcfCommons.Client;
using log4net.Config;
using Ninject.Modules;

namespace Emaratech.Services.Scheduler.App_Start
{
    public class ServiceNinjectModule : NinjectModule
    {
        public override void Load()
        {
        }
    }
}