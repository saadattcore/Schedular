using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobUpdate")]
    [DataContract]
    public class RestJobUpdate
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "maxLockSeconds")]
        public double? MaxLockSeconds { get; set; }
        
        [DataMember(Name = "jobSource")]
        public RestJobSourceAdd JobSource { get; set; }
    }
}