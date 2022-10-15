using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.Contracts.Export
{
    [DataContract]
    public class ImportJob
    {
        [DataMember(Name = "systemIds")]
        public IList<string> SystemIds { get; set; }

        [DataMember(Name = "data")]
        public ExportJob Data { get; set; }
    }
}
