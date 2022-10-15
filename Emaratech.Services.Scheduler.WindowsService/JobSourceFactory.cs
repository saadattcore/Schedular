using Emaratech.Services.Scheduler.WindowsService.Interfaces;
using Emaratech.Services.Scheduler.WindowsService.JobSources;
using Emaratech.Services.Scheduler.WindowsService.Models;

namespace Emaratech.Services.Scheduler.WindowsService
{
    public class JobSourceFactory : IJobSourceFactory
    {
        public IJobSource Create(JobInfo job)
        {
            IJobSource jobSource = null;
            if (job != null && job.JobSource != null)
            {
                if (!string.IsNullOrEmpty(job.JobSource._Class))
                {
                    jobSource = new JobClass { Source = job.JobSource._Class, JobId = job.JobId, JobVersion = (int)job.JobVersion, JobInstanceId = job.JobInstanceId };
                }
                else if (!string.IsNullOrEmpty(job.JobSource.API))
                {
                    jobSource = new JobApi { Source = job.JobSource.API, JobId = job.JobId, JobVersion = (int)job.JobVersion, JobInstanceId = job.JobInstanceId };
                }
                else if (!string.IsNullOrEmpty(job.JobSource.Process))
                {
                    jobSource = new JobProcess { Source = job.JobSource.Process, JobId = job.JobId, JobVersion = (int)job.JobVersion, JobInstanceId = job.JobInstanceId };
                }
            }
            return jobSource;
        }
    }
}