using SwaggerWcf.Attributes;
using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobSource")]
    [DataContract]
    public class RestJobSource
    {
        [DataMember(Name = "class")]
        public string Class { get; set; }

        [DataMember(Name = "process")]
        public string Process { get; set; }

        [DataMember(Name = "API")]
        public string API { get; set; }
    }
}