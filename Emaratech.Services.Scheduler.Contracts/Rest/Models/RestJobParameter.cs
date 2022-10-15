using SwaggerWcf.Attributes;
using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobParameter")]
    [DataContract]
    public class RestJobParameter
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}