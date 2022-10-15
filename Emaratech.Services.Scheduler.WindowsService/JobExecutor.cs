using Emaratech.Services.Scheduler.WindowsService.Interfaces;
using Emaratech.Services.Scheduler.WindowsService.Models;
using log4net;

namespace Emaratech.Services.Scheduler.WindowsService
{
    public class JobExecutor : IJobExecutor
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(JobExecutor));
        private readonly IJobSourceFactory jobSourceFactory;
        
        public JobExecutor(IJobSourceFactory jobSourceFactory)
        {
            this.jobSourceFactory = jobSourceFactory;
        }

        public void Execute(JobInfo job)
        {
            var jobSource = jobSourceFactory.Create(job);
            jobSource?.Execute();
        }
    }
}