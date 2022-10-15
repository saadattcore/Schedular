using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "Job")]
    [DataContract]
    public class RestJob
    {
        [DataMember(Name = "jobId")]
        public string JobId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "version")]
        public long Version { get; set; }

        [DataMember(Name = "maxLockSeconds")]
        public double? MaxLockSeconds { get; set; }
    }
}