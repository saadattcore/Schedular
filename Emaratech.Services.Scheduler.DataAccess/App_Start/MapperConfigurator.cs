using AutoMapper;
using Emaratech.Services.Scheduler.Contracts.Export;
using Emaratech.Services.WcfCommons.Dynamic;
using DAM = Emaratech.Services.Scheduler.Contracts.DataAccess.Models;

namespace Emaratech.Services.Scheduler.DataAccess
{
    [MapperConfigurationClass]
    public static class MapperConfigurator
    {
        [MapperConfigurationMethod]
        public static void Configure(this IMapperConfiguration configuration)
        {
            configuration.CreateMap<DAM.Job, Entities.Job>()
                .ReverseMap();
            configuration.CreateMap<DAM.JobSource, Entities.JobSource>()
                .ReverseMap();
            configuration.CreateMap<DAM.JobParameter, Entities.JobParameter>()
                .ReverseMap();
            configuration.CreateMap<DAM.ScheduleType, Entities.ScheduleType>()
                .ReverseMap();
            configuration.CreateMap<DAM.JobSchedule, Entities.JobSchedule>()
                .ReverseMap();
            configuration.CreateMap<DAM.JobInstance, Entities.JobInstance>()
                .ReverseMap();
            configuration.CreateMap<ExportJob, Entities.Job>()
                .ReverseMap();
            configuration.CreateMap<ExportJobParameter, Entities.JobParameter>()
                .ReverseMap();
            configuration.CreateMap<ExportJobSchedule, Entities.JobSchedule>()
                .ReverseMap();
            configuration.CreateMap<ExportJobSource, Entities.JobSource>()
                .ReverseMap();
        }
    }
}