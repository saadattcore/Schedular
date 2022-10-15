using AutoMapper;
using Emaratech.Services.Scheduler.Contracts.DataAccess.Models;
using Emaratech.Services.Scheduler.Contracts.Rest.Models;
using System;
using System.Text;
using Emaratech.Services.WcfCommons.Dynamic;
using Emaratech.Services.Scheduler.Contracts.Export;

namespace Emaratech.Services.Scheduler.App_Start
{
    [MapperConfigurationClass]
    public static class MapperConfigurator
    {
        [MapperConfigurationMethod]
        public static void Configure(this IMapperConfiguration cfg)
        {
            cfg.CreateMap<RestJob, Job>().ReverseMap();
            cfg.CreateMap<RestJobEx, Job>().ReverseMap();
            cfg.CreateMap<RestJobAdd, Job>().ReverseMap();
            cfg.CreateMap<RestJobUpdate, Job>().ReverseMap();
            cfg.CreateMap<RestJobParameter, JobParameter>().ReverseMap();
            cfg.CreateMap<RestJobInstanceUpdate, JobInstance>().ReverseMap();
            cfg.CreateMap<RestScheduleType, ScheduleType>().ReverseMap();
            cfg.CreateMap<RestJobSchedule, JobSchedule>().ReverseMap();
            cfg.CreateMap<RestJobSource, JobSource>().ReverseMap();

            cfg.CreateMap<ExportJob, Job>()
           .ReverseMap();
            cfg.CreateMap<ExportJobParameter, JobParameter>()
                .ReverseMap();
            cfg.CreateMap<ExportJobSchedule, JobSchedule>()
                .ReverseMap();
            cfg.CreateMap<ExportJobSource, JobSource>()
                .ReverseMap();


            cfg.CreateMap<RestJobSourceAdd, JobSource>()
                .ForMember(x => x.Content, x => x.MapFrom(y => Convert.FromBase64String(y.Content)))
                .ReverseMap()
                .ForMember(x => x.Content, x => x.MapFrom(y => Convert.ToBase64String(y.Content)));
        }
    }
}