using Emaratech.Services.Scheduler.Model;

namespace Emaratech.Services.Scheduler.WindowsService.Models
{
    public class JobInfo
    {
        public string JobId { get; set; }
        public JobSource JobSource { get; set; }
        public long? JobVersion { get; set; }
        public string JobInstanceId { get; set; }

        public JobInfo(NextJob job, string jobInstanceId)
        {
            JobId = job.JobId;
            JobVersion = job.Version;
            JobSource = job.JobSource;
            JobInstanceId = jobInstanceId;
        }
    }
}