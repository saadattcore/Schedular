using System;

namespace Emaratech.Services.Scheduler.Entities
{
    public partial class Job
    {
        public DateTime? MaxLockedDate
        {
            get
            {
                if (LockedDate.HasValue && MaxLockSeconds.HasValue)
                {
                    return LockedDate.Value.AddSeconds(MaxLockSeconds.Value);
                }
                return null;
            }
        }
    }
}
