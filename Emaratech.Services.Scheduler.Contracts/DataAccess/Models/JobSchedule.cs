using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess.Models
{
    [DataContract]
    public class JobSchedule
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "jobId")]
        public string JobId { get; set; }

        [DataMember(Name = "scheduleTypeId")]
        public string ScheduleTypeId { get; set; }

        [DataMember(Name = "isDeleted")]
        public bool IsDeleted { get; set; }

        [DataMember]
        public long Version { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? DeletedDate { get; set; }

        [DataMember]
        public string DeletedBy { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public double ScheduleFrequency { get; set; }
    }
}