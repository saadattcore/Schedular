using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobInstanceUpdate")]
    [DataContract]
    public class RestJobInstanceUpdate
    {
        [DataMember(Name = "isExecuted")]
        public bool IsExecuted { get; set; }
    }
}