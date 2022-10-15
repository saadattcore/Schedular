using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Emaratech.Services.HealthCheck.EF;
using Ninject;
using Ninject.Web.Common;

namespace Emaratech.Services.Scheduler.DataAccess
{
    public class Global : NinjectHttpApplication
    {
        /// <summary>
        /// Creates the kernel that will manage application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        protected override IKernel CreateKernel()
        {
            return new StandardKernel(new ServiceNinjectModule(), new EFHealthCheckNinjectModule());
        }
    }
}