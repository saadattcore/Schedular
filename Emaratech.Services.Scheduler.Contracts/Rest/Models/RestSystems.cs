using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Workflows.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "Systems")]
    [DataContract]
    public class RestSystems
    {
        [DataMember(Name = "systemIds")]
        public string[] SystemIds { get; set; }
    }
}