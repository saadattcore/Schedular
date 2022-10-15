using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess.Models
{
    [DataContract]
    public class ScheduleType
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}