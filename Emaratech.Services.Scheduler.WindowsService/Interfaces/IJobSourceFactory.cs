using Emaratech.Services.Scheduler.WindowsService.Models;

namespace Emaratech.Services.Scheduler.WindowsService.Interfaces
{
    public interface IJobSourceFactory
    {
        IJobSource Create(JobInfo job);
    }
}