using Emaratech.Services.Scheduler.Contracts.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Scheduler.Contracts.Export
{
    [DataContract]
    public class ExportJob
    {

        [DataMember(Name = "jobId")]
        public string JobId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember]
        public string JobType { get; set; }


        [DataMember]
        public long Version { get; set; }

        [DataMember]
        public double? MaxLockSeconds { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public List<ExportJobSchedule> JobSchedules { get; set; }

        [DataMember]
        public List<ExportJobParameter> JobParameters { get; set; }

        [DataMember]
        public ExportJobSource JobSource { get; set; }

        [DataMember]
        public string[] Systems { get; set; }
    }
}
