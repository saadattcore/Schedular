using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess.Models
{
    [DataContract]
    public class JobInstance
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string JobId { get; set; }

        [DataMember]
        public bool IsExecuted { get; set; }

        [DataMember]
        public DateTime ExecutedDate { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }
        
        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? DeletedDate { get; set; }

        [DataMember]
        public string DeletedBy { get; set; }
    }
}