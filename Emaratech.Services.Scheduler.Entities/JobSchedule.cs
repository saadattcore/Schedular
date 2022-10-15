//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Emaratech.Services.Scheduler.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class JobSchedule
    {
        public string Id { get; set; }
        public string JobId { get; set; }
        public string ScheduleTypeId { get; set; }
        public long Version { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public int ScheduleFrequency { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
    
        public virtual ScheduleType ScheduleType { get; set; }
    }
}
