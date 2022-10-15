using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "NextJob")]
    [DataContract]
    public class RestNextJobInfo
    {
        [DataMember(Name = "jobId")]
        public string JobId { get; set; }
        
        [DataMember(Name = "version")]
        public long Version { get; set; }

        [DataMember(Name = "jobSource")]
        public RestJobSource JobSource { get; set; }
    }
}