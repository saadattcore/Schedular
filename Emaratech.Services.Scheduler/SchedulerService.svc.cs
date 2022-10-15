using System;
using System.Linq;
using System.ServiceModel.Activation;
using AutoMapper;
using Emaratech.Services.Common.Models;
using Emaratech.Services.Scheduler.Contracts.DataAccess;
using Emaratech.Services.Scheduler.Contracts.DataAccess.Models;
using Emaratech.Services.Scheduler.Contracts.Rest.Models;
using Emaratech.Services.WcfCommons.Client;
using Emaratech.Utilities;
using SwaggerWcf.Attributes;
using Emaratech.Services.Workflows.Contracts.Rest.Models;
using System.IO;
using System.Collections.Generic;
using System.ServiceModel;
using log4net;
using Emaratech.Services.Scheduler.Contracts.Export;
using System.ServiceModel.Web;

namespace Emaratech.Services.Scheduler
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [SwaggerWcf("/")]
    public class SchedulerService : ISchedulerService
    {
        private IMapper _mapper;
        private readonly WcfClient<ISchedulerDataAccessService> _serDAccMain;
        private static ILog Log = LogManager.GetLogger(typeof(SchedulerService));

        public SchedulerService(IMapper mapper,
            WcfClient<ISchedulerDataAccessService> servicesDataAccessServiceClient)
        {
            _mapper = mapper;
            _serDAccMain = servicesDataAccessServiceClient;
        }

        public void OptionsHandler() { }

        public PagingResponse<RestJob> GetJobsBySystemId(string systemId, string pageNumber, string itemsCount)
        {
            Func<ISchedulerDataAccessService, PagingResponse<RestJob>> func = (client) =>
            {
                PagingRequest request = PagingRequest.CreatePageRequest(pageNumber, itemsCount);
                var response = client.GetJobs(systemId, request);

                var result = response.Data
                    .Select(w => _mapper.Map<RestJob>(w))
                    .ToPagingResponse(response.Paging.PageNumber, response.Paging.TotalCount);

                return result;
            };

            return _serDAccMain.Process(func);
        }

        public PagingResponse<RestJob> GetJobsBySystemIds(RestSystems data, string pageNumber, string itemsCount)
        {
            Func<ISchedulerDataAccessService, PagingResponse<RestJob>> func = (client) =>
            {
                PagingRequest request = PagingRequest.CreatePageRequest(pageNumber, itemsCount);
                var response = client.GetJobs(data.SystemIds, request);

                var result = response.Data
                    .Select(w => _mapper.Map<RestJob>(w))
                    .ToPagingResponse(response.Paging.PageNumber, response.Paging.TotalCount);

                return result;
            };

            return _serDAccMain.Process(func);
        }

        public PagingResponse<RestJob> SearchJobs(string search, string systemId, string pageNumber, string itemsCount)
        {
            Func<ISchedulerDataAccessService, PagingResponse<RestJob>> func = (client) =>
            {
                PagingRequest request = PagingRequest.CreatePageRequest(pageNumber, itemsCount);
                var response = client.SearchJobs(systemId, new PagingRequest<string>(search, request));

                var result = response.Data
                    .Select(w => _mapper.Map<RestJob>(w))
                    .ToPagingResponse(response.Paging.PageNumber, response.Paging.TotalCount);

                return result;
            };

            return _serDAccMain.Process(func);
        }

        public RestNextJobInfo FetchNext(RestJobFilter filter)
        {
            Func<ISchedulerDataAccessService, RestNextJobInfo> func = (client) =>
            {
                client.UnlockHangingJobs();
                var job = client.FetchNext(filter?.JobIds, filter?.Tags);
                if (job != null)
                {
                    // Lock the job after fetching it
                    if (LockJob(job.JobId))
                    {
                        Log.Info($"Job {job.Name} with id '{job.JobId}' and version '{job.Version}' picked for execution");
                        return new RestNextJobInfo
                        {
                            JobId = job.JobId,
                            Version = job.Version,
                            JobSource = _mapper.Map<RestJobSource>(job.JobSource)
                        };
                    }
                }
                return null;
            };
            return _serDAccMain.Process(func);
        }

        public string AddJob(RestJobAdd rjob)
        {
            Func<ISchedulerDataAccessService, string> func = (client) =>
            {
                var jobDTO = _mapper.Map<Job>(rjob);
                var idOfNewJob = client.AddJob(jobDTO);
                return idOfNewJob;
            };
            return _serDAccMain.Process(func);
        }

        public long EnableJob(string jobId)
        {
            Func<ISchedulerDataAccessService, long> func = (client) =>
            {
                return client.EnableJob(jobId);
            };
            return _serDAccMain.Process(func);
        }

        public long DisableJob(string jobId)
        {
            Func<ISchedulerDataAccessService, long> func = (client) =>
            {
                return client.DisableJob(jobId);
            };
            return _serDAccMain.Process(func);
        }

        public bool LockJob(string jobId)
        {
            Func<ISchedulerDataAccessService, bool> func = (client) =>
            {
                return client.LockJob(jobId);
            };
            return _serDAccMain.Process(func);
        }

        public long UnlockJob(string jobId)
        {
            Func<ISchedulerDataAccessService, long> func = (client) =>
            {
                return client.UnlockJob(jobId);
            };
            return _serDAccMain.Process(func);
        }

        public long UpdateJob(string id, RestJobUpdate rjob)
        {
            Func<ISchedulerDataAccessService, long> func = (client) =>
            {
                var jobToUpdate = _mapper.Map<Job>(rjob);
                jobToUpdate.IsEnabled = true;
                return client.UpdateJob(id, jobToUpdate);
            };
            return _serDAccMain.Process(func);
        }

        public void DeleteJob(string id)
        {
            Action<ISchedulerDataAccessService> func = (client) =>
            {
                client.DeleteJob(id);
            };
            _serDAccMain.Process(func);
        }

        public RestJobEx GetJobByIdAndVersion(string id, string version)
        {
            Func<ISchedulerDataAccessService, RestJobEx> func = (client) =>
            {
                var v = version == null ? null : (int?)int.Parse(version);
                var job = client.GetJob(id, v);
                var res = _mapper.Map<RestJobEx>(job);
                return res;
            };
            return _serDAccMain.Process(func);
        }

        public RestJobEx GetJob(string id)
        {
            return GetJobByIdAndVersion(id, null);
        }

        public void AssignJobToSystem(string jobId, string systemId)
        {
            Action<ISchedulerDataAccessService> func = (client) =>
            {
                client.AssignJobToSystem(jobId, systemId);
            };
            _serDAccMain.Process(func);
        }

        public void RemoveJobFromSystem(string jobId, string systemId)
        {
            Action<ISchedulerDataAccessService> func = (client) =>
            {
                client.RemoveJobFromSystem(jobId, systemId);
            };
            _serDAccMain.Process(func);
        }

        public IEnumerable<RestScheduleType> GetScheduleTypes()
        {
            Func<ISchedulerDataAccessService, IEnumerable<RestScheduleType>> func = (client) =>
            {
                var types = client.GetScheduleTypes();
                return _mapper.Map<IEnumerable<RestScheduleType>>(types);
            };
            return _serDAccMain.Process(func);
        }

        public void AddJobSchedule(RestJobSchedule schedule)
        {
            Action<ISchedulerDataAccessService> func = (client) =>
            {
                client.AddJobSchedule(_mapper.Map<JobSchedule>(schedule));
            };
            _serDAccMain.Process(func);
        }

        public void DeleteJobSchedule(string id)
        {
            Action<ISchedulerDataAccessService> func = (client) =>
            {
                client.DeleteJobSchedule(id);
            };
            _serDAccMain.Process(func);
        }

        public Stream GetJobContent(string jobId, string version)
        {
            Func<ISchedulerDataAccessService, Stream> func = (client) =>
            {
                var stream = client.GetJobContent(jobId, int.Parse(version));
                return stream;
            };
            return _serDAccMain.Process(func);
        }

        public string AddJobInstance(RestJobInstance jobInstance)
        {
            Func<ISchedulerDataAccessService, string> func = (client) =>
            {
                return client.AddJobInstance(jobInstance?.JobId);
            };
            return _serDAccMain.Process(func);
        }

        public void UpdateJobInstance(string id, RestJobInstanceUpdate rjobInstance)
        {
            Action<ISchedulerDataAccessService> func = (client) =>
            {
                client.UpdateJobInstance(id, _mapper.Map<JobInstance>(rjobInstance));
            };
            _serDAccMain.Process(func);
        }

        public ExportJob Export(string entityId)
        {
            return ExportVersion(entityId, null);
        }

        public ExportJob ExportVersion(string entityId, string version)
        {
            Func<ISchedulerDataAccessService, ExportJob> runner = client =>
            {
                var job = client.GetJob(entityId, version == null ? null : (int?)int.Parse(version));

                var responseMessage = WebOperationContext.Current?.OutgoingResponse;
                if (responseMessage != null)
                {
                    responseMessage.Headers.Add("Content-Disposition",
                        $"attachment; filename={job.Name}.json");
                }

                var exportJob = _mapper.Map<ExportJob>(job);

                exportJob.JobParameters = _mapper.Map<List<ExportJobParameter>>(job.JobParameters);
                exportJob.JobSchedules = _mapper.Map<List<ExportJobSchedule>>(job.JobSchedules);
                exportJob.JobSource = _mapper.Map<ExportJobSource>(job.JobSource);
                return exportJob;
            };

            return _serDAccMain.Process(runner);
        }

        public long Import(ImportJob importModel)
        {
            Func<ISchedulerDataAccessService, long> runner = client => client.Import(importModel);

            return _serDAccMain.Process(runner);
        }

    }
}