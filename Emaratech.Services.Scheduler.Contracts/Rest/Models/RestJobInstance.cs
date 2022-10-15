using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobInstance")]
    [DataContract]
    public class RestJobInstance
    {
        [DataMember(Name = "jobId")]
        public string JobId { get; set; }
    }
}