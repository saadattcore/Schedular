using SwaggerWcf.Attributes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobFilter")]
    [DataContract]
    public class RestJobFilter
    {
        [DataMember(Name = "jobIds")]
        public IEnumerable<string> JobIds { get; set; }

        [DataMember(Name = "tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}