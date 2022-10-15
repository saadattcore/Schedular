using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess.Models
{
    [DataContract]
    public class JobSource
    {
        [DataMember]
        public string JobId { get; set; }

        [DataMember]
        public string Class { get; set; }

        [DataMember]
        public string Process { get; set; }

        [DataMember]
        public string API { get; set; }

        [DataMember]
        public byte[] Content { get; set; }
    }
}