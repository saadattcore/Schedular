using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobAdd")]
    [DataContract]
    public class RestJobAdd : RestJobUpdate
    {
        [DataMember(Name = "systems")]
        public string[] Systems { get; set; }
    }
}