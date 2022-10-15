using SwaggerWcf.Attributes;
using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobSourceAdd")]
    [DataContract]
    public class RestJobSourceAdd : RestJobSource
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}