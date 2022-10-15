using Emaratech.Services.Scheduler.WindowsService.Models;

namespace Emaratech.Services.Scheduler.WindowsService.Interfaces
{
    public interface IJobExecutor
    {
        void Execute(JobInfo job);
    }
}