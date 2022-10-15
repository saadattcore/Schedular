using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess.Models
{
    [DataContract]
    public class Job
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "jobId")]
        public string JobId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "isDeleted")]
        public bool IsDeleted { get; set; }

        [DataMember(Name = "isActive")]
        public bool IsActive { get; set; }

        [DataMember(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember]
        public long Version { get; set; }

        [DataMember]
        public double? MaxLockSeconds { get; set; }

        [DataMember]
        public bool IsLocked { get; set; }

        [DataMember]
        public string LockedBy { get; set; }

        [DataMember]
        public DateTime? LockedDate { get; set; }

        [DataMember]
        public bool IsExecuted { get; set; }

        [DataMember]
        public DateTime? ExecutedDate { get; set; }

        [DataMember]
        public string ExecutedBy { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? DeletedDate { get; set; }

        [DataMember]
        public string DeletedBy { get; set; }

        [DataMember]
        public string ClonedSourceId { get; set; }

        [DataMember]
        public long ClonedSourceVersion { get; set; }

        [DataMember]
        public long RevertedFromVersion { get; set; }

        [DataMember]
        public List<JobSchedule> JobSchedules { get; set; }

        [DataMember]
        public List<JobParameter> JobParameters { get; set; }

        [DataMember]
        public JobSource JobSource { get; set; }

        [DataMember]
        public string[] Systems { get; set; }
    }
}
