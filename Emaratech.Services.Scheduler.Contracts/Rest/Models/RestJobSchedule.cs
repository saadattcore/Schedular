using SwaggerWcf.Attributes;
using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobSchedule")]
    [DataContract]
    public class RestJobSchedule
    {
        [DataMember(Name = "jobId")]
        public string JobId { get; set; }

        [DataMember(Name = "scheduleTypeId")]
        public string ScheduleTypeId { get; set; }
        
        [DataMember(Name = "startDate")]
        public DateTime? StartDate { get; set; }

        [DataMember(Name = "scheduleFrequency")]
        public double ScheduleFrequency { get; set; }
    }
}