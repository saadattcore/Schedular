using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "JobSourceContent")]
    [DataContract]
    public class RestJobSourceContent
    {
        [DataMember(Name = "content")]
        public byte[] Content { get; set; }
    }
}
