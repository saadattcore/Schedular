using Emaratech.Services.Scheduler.Api;

namespace Emaratech.Services.Scheduler.WindowsService.Interfaces
{
    public interface IApiFactory
    {
        ISchedulerApi SchedulerApi { get; }
    }
}