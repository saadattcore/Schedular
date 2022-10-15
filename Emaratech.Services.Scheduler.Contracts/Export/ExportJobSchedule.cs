using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.Contracts.Export
{
    [DataContract]
    public class ExportJobSchedule
    {

        [DataMember]
        public string ScheduleTypeId { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public double ScheduleFrequency { get; set; }
    }
}
