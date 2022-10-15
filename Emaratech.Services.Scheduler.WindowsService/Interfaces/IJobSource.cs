using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.WindowsService.Interfaces
{
    public interface IJobSource
    {
        string JobInstanceId { get; set; }
        string JobId { get; set; }
        int JobVersion { get; set; }
        string Source { get; set; }

        Task<object> Execute();
    }
}