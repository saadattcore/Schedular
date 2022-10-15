using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.Contracts.Export
{
    [DataContract]
    public class ExportJobSource
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
