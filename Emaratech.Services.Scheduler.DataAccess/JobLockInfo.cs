using System;

namespace Emaratech.Services.Scheduler.DataAccess
{
    public class JobLockInfo
    {
        public int LockCount { get; set; }
        public DateTime? MaxLockedDate { get; set; }
    }
}