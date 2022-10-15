using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobEx")]
    [DataContract]
    public class RestJobEx
    {
        [DataMember(Name = "jobId")]
        public string JobId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "version")]
        public long Version { get; set; }

        [DataMember(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "maxLockSeconds")]
        public double? MaxLockSeconds { get; set; }
        
        [DataMember(Name = "executedDate")]
        public DateTime? ExecutedDate { get; set; }

        [DataMember(Name = "isLocked")]
        public bool IsLocked { get; set; }

        [DataMember(Name = "lockedDate")]
        public DateTime? LockedDate { get; set; }

        [DataMember(Name = "jobSchedules")]
        public IEnumerable<RestJobSchedule> JobSchedules { get; set; }

        [DataMember(Name = "jobParameters")]
        public IEnumerable<RestJobParameter> JobParameters { get; set; }

        [DataMember(Name = "jobSource")]
        public RestJobSource JobSource { get; set; }
    }
}