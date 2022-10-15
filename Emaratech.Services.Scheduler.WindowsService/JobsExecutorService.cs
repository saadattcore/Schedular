using Emaratech.Services.Scheduler.WindowsService.Interfaces;
using Emaratech.Services.Scheduler.WindowsService.Models;
using log4net;
using log4net.Config;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace Emaratech.Services.Scheduler.WindowsService
{
    public partial class JobsExecutorService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JobsExecutorService));
        private static readonly IJobExecutor JobExecutor = new JobExecutor(new JobSourceFactory());
        private static Timer timer;

        public JobsExecutorService()
        {
            InitializeComponent();
            timer = new Timer {AutoReset = false, Interval = 1000};
            timer.Elapsed += async delegate
             {

                 await FetchAndExecuteJob();
                 timer.Start();
             };
            timer.Start();
            
        }

        protected override void OnStart(string[] args)
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
        }

        protected override void OnStop()
        {
        }

        private static async Task FetchAndExecuteJob()
        {
            try
            {
                var nextJob =  await ApiFactory.Default.SchedulerApi.FetchNextAsync(new Model.JobFilter());
                if (nextJob != null)
                {

                    // Create a job instance
                    var instanceId = await ApiFactory.Default.SchedulerApi.AddJobInstanceAsync(new Model.JobInstance { JobId = nextJob.JobId });
                    
                    Log.Info($"Executing job {nextJob.JobId}");
                    JobExecutor.Execute(new JobInfo(nextJob, instanceId));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}