using Emaratech.Services.Scheduler.Api;
using Emaratech.Services.Scheduler.WindowsService.Interfaces;
using System.Configuration;

namespace Emaratech.Services.Scheduler.WindowsService
{
    public class ApiFactory : IApiFactory
    {
        private static ISchedulerApi schedulerApi;

        public static ApiFactory Default => new ApiFactory();

        public ISchedulerApi SchedulerApi
        {
            get
            {
                if (schedulerApi == null)
                {
                    schedulerApi = new SchedulerApi(ConfigurationManager.AppSettings["SchedulerApi"]);
                    //schedulerApi = new SchedulerApi("http://localhost/Emaratech.Services.Scheduler/SchedulerService.svc/json");
                }
                return schedulerApi;
            }
        }
    }
}