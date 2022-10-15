using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess.Models
{
    [DataContract]
    public class JobParameter
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember]
        public long BeginVersion { get; set; }

        [DataMember]
        public long EndVersion { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}