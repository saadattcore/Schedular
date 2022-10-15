using Emaratech.Services.Scheduler.WindowsService.Interfaces;
using System;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.WindowsService.JobSources
{
    public class JobClass : IJobSource
    {
        public string JobInstanceId { get; set; }
        public string JobId { get; set; }
        public int JobVersion { get; set; }
        public string Source { get; set; }

        public async Task<object> Execute()
        {
            throw new NotImplementedException();
        }
    }
}