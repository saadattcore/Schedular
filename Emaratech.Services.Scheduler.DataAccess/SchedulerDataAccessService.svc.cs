using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AutoMapper;
using Emaratech.DatabaseLayer;
using Emaratech.Services.Common.Models;
using Emaratech.Services.Scheduler.Contracts.DataAccess;
using Emaratech.Services.Scheduler.Entities;
using Emaratech.Utilities;
using Models = Emaratech.Services.Scheduler.Contracts.DataAccess.Models;
using Emaratech.Services.WcfCommons.Faults.Models;
using System.IO;
using System.Collections.Concurrent;
using System.Data.Entity.Validation;
using Emaratech.Services.Scheduler.Contracts.Export;
using log4net;

namespace Emaratech.Services.Scheduler.DataAccess
{
    public class SchedulerDataAccessService : ISchedulerDataAccessService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SchedulerDataAccessService));

        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public SchedulerDataAccessService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public PagingResponse<Models.Job> GetJobs(string[] systemId, PagingRequest pagingRequest)
        {
                var query = unitOfWork.Repository<Job>()
                    .Get(j => !j.IsDeleted && j.IsActive);

                int totalCount = query.Count();
                int takeCount = pagingRequest.GetPageSize();
                int pageNumber = pagingRequest.GetPageNumber();

                var result = query
                    .OrderBy(x => x.CreatedDate)
                    .Skip(pagingRequest.SkipCount)
                    .Take(takeCount);


                var allAssociations = unitOfWork.Repository<SystemJob>()
                    .Get(s => !s.IsDeleted);

                var res = new List<Job>();

                foreach (var job in result)
                {
                    if (allAssociations.Any(ass => ass.JobId == job.JobId && systemId.Contains(ass.SystemKey)))
                    {
                        res.Add(job);
                    }
                }

                return res.AsEnumerable()
                    .Select(s => mapper.Map<Models.Job>(s))
                    .ToPagingResponse(pageNumber, totalCount);

        }

        public PagingResponse<Models.Job> GetJobs(string systemId, PagingRequest pagingRequest)
        {
                var query = unitOfWork.Repository<Job>()
                    .Get(j => !j.IsDeleted && j.IsActive);

                var allAssociations = unitOfWork.Repository<SystemJob>()
                    .Get(s => !s.IsDeleted);

                var results = (from j in query
                               from sj in allAssociations
                               where j.JobId == sj.JobId && sj.SystemKey == systemId
                               select j)
                               .ToList();

                int totalCount = results.Count();
                int takeCount = pagingRequest.GetPageSize();
                int pageNumber = pagingRequest.GetPageNumber();

                return results
                    .OrderBy(x => x.CreatedDate)
                    .Skip(pagingRequest.SkipCount)
                    .Take(takeCount)
                    .AsEnumerable()
                    .Select(s => mapper.Map<Models.Job>(s))
                    .ToPagingResponse(pageNumber, totalCount);
        }

        public string AddJob(Models.Job job)
        {
                using (var transaction = unitOfWork.BeginTransaction())
                {
                    var jobs = unitOfWork.Repository<Job>();

                    var jobToAdd = mapper.Map<Job>(job);

                    if (!string.IsNullOrEmpty(job.JobId))
                    {
                        var existingJob = jobs
                        .Get(j => j.IsActive && !j.IsDeleted)
                        .FirstOrDefault(j => j.JobId == job.JobId);

                        if (existingJob != null)
                        {
                            ErrorCodes.Conflict.ToServiceFault($"JobId '{job.JobId}' uniqueness violated. Job with such Id already present in the database and is not deleted.");
                        }
                    }
                    else
                    {
                        jobToAdd.JobId = Utils.GenerateGuidKey();
                        jobToAdd.CreatedDate = DateTime.Now;
                        jobToAdd.CreatedBy = "-1";
                        jobToAdd.Id = jobToAdd.JobId;
                    }

                    jobToAdd.IsActive = true;
                    jobToAdd.IsEnabled = true;
                    if (job.JobSource != null)
                    {
                        var jobSourceToAdd = mapper.Map<JobSource>(job.JobSource);
                        jobSourceToAdd.Id = Utils.GenerateGuidKey();
                        jobSourceToAdd.JobId = jobToAdd.JobId;
                        unitOfWork.Repository<JobSource>().Insert(jobSourceToAdd);
                    }

                    unitOfWork.Repository<Job>().Insert(jobToAdd);

                    // Add systems associations
                    if (job.Systems != null)
                    {
                        AssignJobToSystems(jobToAdd.JobId, job.Systems.Distinct());
                    }

                    unitOfWork.Save();
                    transaction.Commit();
                    return jobToAdd.Id;
                }
        }

        public long UpdateJob(string id, Models.Job job)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var jobToMarkAsDeleted = unitOfWork.Repository<Job>()
                    .Get()
                    .First(j => j.JobId == id && !j.IsDeleted && j.IsActive);

                if (jobToMarkAsDeleted == null)
                {
                    throw ErrorCodes.Conflict.ToServiceFault($"JobId '{id}' not found. Nothing to update.")
                    ;
                }

                jobToMarkAsDeleted.IsActive = false;
                unitOfWork.Save();

                var nextVer = unitOfWork.Repository<Job>().Get(j => j.JobId == id).Max(j => j.Version) + 1;

                var jobToAdd = mapper.Map<Job>(job);
                jobToAdd.JobId = id;
                jobToAdd.Id = Utils.GenerateGuidKey();
                jobToAdd.Version = nextVer;
                jobToAdd.CreatedDate = DateTime.Now;
                jobToAdd.CreatedBy = "-1";
                jobToAdd.IsActive = true;
                jobToAdd.Name = jobToAdd.Name ?? jobToMarkAsDeleted.Name;
                jobToAdd.MaxLockSeconds = jobToAdd.MaxLockSeconds ?? jobToMarkAsDeleted.MaxLockSeconds;

                unitOfWork.Repository<Job>().Insert(jobToAdd);

                if (job.JobSource != null)
                {
                    var jobSource = mapper.Map<JobSource>(job.JobSource);
                    var jobSourceToUpdate = unitOfWork.Repository<JobSource>()
                        .Get(j => j.JobId == id)
                        .FirstOrDefault();

                    if (jobSourceToUpdate != null)
                    {
                        jobSourceToUpdate.Process = jobSource.Process;
                        jobSourceToUpdate.Content = jobSource.Content;
                        jobSourceToUpdate.API = jobSource.API;
                        jobSourceToUpdate.Class = jobSource.Class;
                    }
                    else
                    {
                        jobSource.Id = Utils.GenerateGuidKey();
                        jobSource.JobId = jobToAdd.JobId;
                        unitOfWork.Repository<JobSource>().Insert(jobSource);
                    }
                }

                unitOfWork.Save();
                transaction.Commit();

                return jobToAdd.Version;
            }
        }

        public void DeleteJob(string id)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var jobToMarkAsDeleted = unitOfWork.Repository<Job>()
                    .Get()
                    .First(j => j.JobId == id && !j.IsDeleted);

                if (jobToMarkAsDeleted == null)
                {
                    throw ErrorCodes.Conflict.ToServiceFault($"JobId '{id}' not found. Nothing to delete.")
                    ;
                }

                jobToMarkAsDeleted.IsActive = false;
                unitOfWork.Save();

                var nextVer = unitOfWork.Repository<Job>().Get().Max(j => j.Version) + 1;

                var jobToAddAsDeleted = mapper.Map<Job>(jobToMarkAsDeleted);
                jobToAddAsDeleted.Version = nextVer;
                jobToAddAsDeleted.IsActive = true;
                jobToAddAsDeleted.IsDeleted = true;

                unitOfWork.Repository<Job>().Insert(jobToAddAsDeleted);
                unitOfWork.Save();

                transaction.Commit();
            }
        }

        public bool LockJob(string id)
        {
                var jobToLock = GetJobQuery(id).FirstOrDefault();
                if (jobToLock == null)
                {
                    throw ErrorCodes.Conflict.ToServiceFault($"JobId '{id}' not found. Nothing to lock.");
                }

                using (var transaction = unitOfWork.BeginTransaction())
                {
                    JobLock jl = new JobLock() { JobId = id, CreatedDate = DateTime.Now, LockedBy = "-1" };
                    unitOfWork.Repository<JobLock>().Insert(jl);

                    jobToLock.IsLocked = true;
                    jobToLock.LockedDate = DateTime.Now;

                    unitOfWork.Save();
                    transaction.Commit();
                    return true;
                }
        }

        public void UnlockHangingJobs()
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var lockedJobs =
                    unitOfWork.Repository<Job>()
                        .Get(
                            j =>
                                j.IsActive && j.IsEnabled && j.IsLocked).ToList();

                var jobsToUnLock = lockedJobs
                                .Where(j => j.MaxLockedDate == null ||
                                j.MaxLockedDate < DateTime.Now);

                foreach (var job in jobsToUnLock)
                {
                    job.IsLocked = false;
                    var jobLock = unitOfWork.Repository<JobLock>().Get(x => x.JobId == job.JobId).SingleOrDefault();
                    if (jobLock != null)
                    {
                        unitOfWork.Repository<JobLock>().Delete(jobLock);

                    }

                }
                unitOfWork.Save();
                transaction.Commit();
            }
        }

        public long UnlockJob(string id)
        {
                var jobToUnlock = GetJobQuery(id).FirstOrDefault();
                if (jobToUnlock == null)
                {
                    throw ErrorCodes.Conflict.ToServiceFault($"JobId '{id}' not found. Nothing to lock.");
                }

                using (var transaction = unitOfWork.BeginTransaction())
                {
                    jobToUnlock.IsLocked = false;

                    unitOfWork.Repository<JobLock>().Delete(jobToUnlock.JobId);

                    unitOfWork.Save();
                    transaction.Commit();
                    return 0;

                }
        }

        public long EnableJob(string id)
        {
            var job = GetJobQuery(id).FirstOrDefault();
            if (job == null)
            {
                throw ErrorCodes.Conflict.ToServiceFault($"JobId '{id}' not found. Nothing to lock.");
            }

            if (job.IsEnabled)
            {
                return job.Version;
            }
            return UpdateJob(id, new Models.Job { IsEnabled = true });
        }

        public long DisableJob(string id)
        {
            var job = GetJobQuery(id).FirstOrDefault();
            if (job == null)
            {
                throw ErrorCodes.Conflict.ToServiceFault($"JobId '{id}' not found. Nothing to lock.");
            }

            if (!job.IsEnabled)
            {
                return job.Version;
            }
            return UpdateJob(id, new Models.Job { IsEnabled = false });
        }

        public Models.Job FetchNext(IEnumerable<string> jobIds, IEnumerable<string> tags)
        {
            var query = jobIds != null && jobIds.Any()
                ? unitOfWork.Repository<Job>().Get(j => jobIds.Contains(j.JobId))
                : unitOfWork.Repository<Job>().Get();

            var jobScheduleQuery = unitOfWork.Repository<JobSchedule>().Get();

            // ExecutedDate + Frequency in seconds >= DateTime.Now
            // Zombie tasks are out of the way, let's proceed with next
            var jobsToFetch = query.Where(j => j.IsActive && j.IsEnabled && !j.IsLocked)
                                .OrderBy(j => j.LockedDate)
                                .Select(j => new
                                {
                                    Job = j,
                                    JobSchedules = jobScheduleQuery.Where(x => x.JobId == j.JobId && !x.IsDeleted)
                                })
                                .Where(j => j.JobSchedules.Any(s => s.StartDate == null || s.StartDate < DateTime.Now))
                                .AsEnumerable();

            Job jobToFetch = null;
            bool jobFound = false;
            foreach (var j in jobsToFetch)
            {
                foreach (var js in j.JobSchedules)
                {
                    var nextExecutionDate = js.GetNextExecutionDate(j.Job);
                    if (nextExecutionDate == null
                        || nextExecutionDate <= DateTime.Now)
                    {
                        jobToFetch = j.Job;
                        jobFound = true;
                        break;
                    }
                }

                if (jobFound)
                {
                    break;
                }
            }


            var result = mapper.Map<Models.Job>(jobToFetch);
            if (result != null)
            {
                var jobSource = unitOfWork.Repository<JobSource>()
                    .Get(x => x.JobId == jobToFetch.JobId)
                    .Select(x => new { x.API, x.Process, x.Class })
                    .Single();

                result.JobSource = new Models.JobSource()
                {
                    API = jobSource.API,
                    Class = jobSource.Class,
                    Process = jobSource.Process,
                    JobId = result.JobId
                };
            }
            return result;
        }

        public Models.Job GetJob(string id, int? version)
        {
                var query = GetJobQuery(id);

                query = version != null
                    ? query.Where(x => x.Version == version)
                    : query.Where(x => x.IsActive).OrderByDescending(x => x.Version);

                var job = query.FirstOrDefault();
                if (job == null)
                {
                    throw ErrorCodes.BadRequest.ToServiceFault($"Job with id '{id}' and version {version} was not found.");
                }

                var schedules = unitOfWork.Repository<JobSchedule>()
                    .Get(j => !j.IsDeleted && j.JobId == id).ToList();

                var parameters = unitOfWork.Repository<JobParameter>()
                    .Get(p => p.JobId == id && p.StartVersion <= job.Version &&
                           (p.EndVersion == null || p.EndVersion > job.Version)).ToList();

                var source = unitOfWork.Repository<JobSource>()
                    .Get(j => j.JobId == id).FirstOrDefault();

                var modelJob = mapper.Map<Models.Job>(job);
                modelJob.JobSchedules = mapper.Map<List<Models.JobSchedule>>(schedules);
                modelJob.JobParameters = mapper.Map<List<Models.JobParameter>>(parameters);
                modelJob.JobSource = mapper.Map<Models.JobSource>(source);
                return modelJob;
        }

        public void AssignJobToSystem(string jobId, string systemId)
        {
            if (string.IsNullOrWhiteSpace(jobId) ||
                string.IsNullOrWhiteSpace(systemId))
                throw ErrorCodes.BadRequest.ToServiceFault("Both keys are mandatory to create association");

            using (var transaction = unitOfWork.BeginTransaction())
            {
                var systemJobs = unitOfWork.Repository<SystemJob>();

                var systemServiceAssociationExists = systemJobs
                    .Get(ss => !ss.IsDeleted && ss.JobId == jobId && ss.SystemKey == systemId)
                    .Any();

                if (systemServiceAssociationExists)
                    throw ErrorCodes.Conflict.ToServiceFault($"Association of '{jobId}' and '{systemId}' already exists.")
                ;

                var now = DateTime.Now;

                var newSystemJobAssociation = new SystemJob
                {
                    Id = Utils.GenerateGuidKey(),
                    CreatedDate = now,
                    CreatedBy = "0000000000000",
                    IsDeleted = false,
                    SystemKey = systemId,
                    JobId = jobId
                };



                systemJobs.Insert(newSystemJobAssociation);
                unitOfWork.Save();
                transaction.Commit();
            }
        }

        public void RemoveJobFromSystem(string jobId, string systemId)
        {
                if (string.IsNullOrWhiteSpace(jobId) ||
                    string.IsNullOrWhiteSpace(systemId))
                    throw ErrorCodes.BadRequest.ToServiceFault("Both keys are mandatory to remove association");

                var systemJobs = unitOfWork.Repository<SystemJob>();

                var existingSystemJobAssociation =
                    systemJobs
                        .Get()
                        .FirstOrDefault(ss => !ss.IsDeleted && ss.SystemKey == systemId &&
                                ss.JobId == jobId);

                if (existingSystemJobAssociation == null)
                {
                    throw ErrorCodes.NotFound.ToServiceFault($"System job association with job id '{jobId}' and system business key '{systemId}' was not found");
                }

                existingSystemJobAssociation.IsDeleted = true;
                existingSystemJobAssociation.DeletedBy = "0000000000000";
                existingSystemJobAssociation.DeletedDate = DateTime.Now;

                systemJobs.Update(existingSystemJobAssociation);
                unitOfWork.Save();
        }

        public PagingResponse<Models.Job> SearchJobs(string systemId, PagingRequest<string> searchRequest)
        {
                string search = searchRequest.Request;

                if (string.IsNullOrWhiteSpace(search))
                {
                    throw ErrorCodes.BadRequest.ToServiceFault("Invalid search query.");
                }

                var jobsQuery = GetJobsQuery(systemId);
                jobsQuery = jobsQuery.Where(x => x.Name.Contains(search));

                return GetJobsCommon(jobsQuery, searchRequest);
        }

        public IEnumerable<Models.ScheduleType> GetScheduleTypes()
        {
            var types = unitOfWork.Repository<ScheduleType>().Get();
            return mapper.Map<IEnumerable<Models.ScheduleType>>(types);
        }

        public void AddJobSchedule(Models.JobSchedule schedule)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var job = GetJob(schedule.JobId, null);
                var jobSchedule = mapper.Map<JobSchedule>(schedule);
                jobSchedule.Version = job.Version;
                jobSchedule.CreatedBy = "-1";
                jobSchedule.CreatedDate = DateTime.Now;
                jobSchedule.Id = Utils.GenerateGuidKey();
                jobSchedule.JobId = job.Id;

                unitOfWork.Repository<JobSchedule>().Insert(jobSchedule);
                unitOfWork.Save();
                transaction.Commit();
            }
        }

        public void DeleteJobSchedule(string id)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var scheduleToDelete = unitOfWork.Repository<JobSchedule>()
                    .Get()
                    .First(x => x.Id == id && !x.IsDeleted);

                if (scheduleToDelete == null)
                {
                    throw ErrorCodes.Conflict.ToServiceFault($"Schedule '{id}' not found. Nothing to delete.");
                }

                scheduleToDelete.IsDeleted = true;
                scheduleToDelete.DeletedDate = DateTime.Now;
                scheduleToDelete.DeletedBy = "-1";
                unitOfWork.Save();
                transaction.Commit();
            }
        }

        public Stream GetJobContent(string jobId, int version)
        {
            var job = unitOfWork.Repository<Job>()
                    .Get(j => j.JobId == jobId && j.Version == version)
                    .FirstOrDefault();

            if (job == null)
            {
                throw ErrorCodes.BadRequest.ToServiceFault($"Job with id '{jobId}' and version {version} was not found.");
            }

            var jobSource = unitOfWork.Repository<JobSource>()
                .Get(x => x.JobId == job.JobId)
                .FirstOrDefault();
            return new MemoryStream(jobSource?.Content ?? new byte[0]);
        }

        public string AddJobInstance(string jobId)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var job = unitOfWork.Repository<Job>()
                    .Get(j => j.JobId == jobId && !j.IsDeleted && j.IsActive && j.IsEnabled)
                    .FirstOrDefault();
                if (job == null)
                {
                    throw ErrorCodes.BadRequest.ToServiceFault($"Job with id '{jobId}' was not found.");
                }

                // Update job executed date
                job.ExecutedDate = DateTime.Now;

                var now = DateTime.Now;
                var instance = new Entities.JobInstance
                {
                    Id = Utils.GenerateGuidKey(),
                    JobId = jobId,
                    CreatedBy = "-1",
                    CreatedDate = now,
                    ExecutedDate = now,
                    ExecutedBy = "-1"
                };

                unitOfWork.Repository<JobInstance>().Insert(instance);
                unitOfWork.Save();
                transaction.Commit();
                return instance.Id;
            }
        }

        public void UpdateJobInstance(string id, Models.JobInstance jobInstance)
        {
            using (var transaction = unitOfWork.BeginTransaction())
            {
                var entity = unitOfWork.Repository<JobInstance>()
                    .Get(j => j.Id == id && !j.IsDeleted)
                    .FirstOrDefault();

                if (entity == null)
                {
                    throw ErrorCodes.BadRequest.ToServiceFault($"JobInstance with id '{id}' was not found.");
                }

                entity.IsExecuted = jobInstance.IsExecuted;
                unitOfWork.Save();
                transaction.Commit();
            }
        }

        public ExportJob Export(string entityId, int? version)
        {
            var job = GetJob(entityId, version);

            var exportJob = mapper.Map<ExportJob>(job);
            exportJob.JobParameters = mapper.Map<List<ExportJobParameter>>(job.JobParameters);
            exportJob.JobSchedules = mapper.Map<List<ExportJobSchedule>>(job.JobSchedules);
            exportJob.JobSource = mapper.Map<ExportJobSource>(job.JobSource);

            return exportJob;
        }

        public long Import(ImportJob importModel)
        {

                var model = importModel.Data;

                if (importModel.SystemIds == null) throw ErrorCodes.BadRequest.ToServiceFault("No systems");

                if (importModel.SystemIds.Any() == false)
                    throw ErrorCodes.BadRequest.ToServiceFault("No systems");

                if (model == null)
                    throw ErrorCodes.BadRequest.ToServiceFault("Job is null");

                if (string.IsNullOrEmpty(model.JobId))
                    throw ErrorCodes.BadRequest.ToServiceFault("No job id");

                using (var transaction = unitOfWork.BeginTransaction())
                {
                    long version = -1;

                    var query = unitOfWork.Repository<Job>()
                        .Get(j => j.JobId == model.JobId && j.Version == model.Version);

                    var job = query.FirstOrDefault();

                    if (job != null)
                    {
                        if (!job.IsActive)
                        {
                            throw ErrorCodes.BadRequest.ToServiceFault($"Job is not active {model.Version}")
                            ;
                        }
                        else if (job.IsDeleted)
                        {
                            job.IsDeleted = false;
                        }

                        version = job.Version;
                    }

                    else
                    {
                        var existing = unitOfWork.Repository<Job>()
                            .Get()
                            .FirstOrDefault(x => x.JobId == model.JobId && x.Version > model.Version);

                        if (existing != null)
                        {
                            throw ErrorCodes.BadRequest.ToServiceFault($"Job with a higher version {existing.Version} already exists")
                            ;
                        }

                        existing = unitOfWork.Repository<Entities.Job>()
                            .Get()
                            .SingleOrDefault(x => x.IsActive && x.JobId == model.JobId);

                        if (existing != null)
                        {
                            existing.IsActive = false;
                        }

                        var timeStamp = DateTime.Now;

                        var newDbJob = mapper.Map<Entities.Job>(model);
                        newDbJob.Id = Utils.GenerateGuidKey();
                        newDbJob.Version = model.Version;
                        newDbJob.CreatedBy = "-1";
                        newDbJob.CreatedDate = DateTime.Now;
                        newDbJob.IsActive = true;
                        newDbJob.IsLocked = false;
                        newDbJob.IsEnabled = model.IsEnabled;
                        newDbJob.ExecutedDate = existing?.ExecutedDate;
                        newDbJob.ExecutedBy = existing?.ExecutedBy;
                        newDbJob.LockedBy = existing?.LockedBy;
                        unitOfWork.Repository<Job>().InsertGraph(newDbJob);

                        SyncJobParameters(importModel, newDbJob);
                        ImportJobSchedule(importModel, timeStamp);
                        ImportJobSource(importModel);



                        version = model.Version;
                    }
                    SyncSystems(importModel, importModel.SystemIds);
                    unitOfWork.Save();
                    transaction.Commit();

                    /* here we can write in xml file if import and export run for this particular job update its version in xml file and windows services should compare 
                       current and latest version if current < latest then make api call and fetch latest job source or schedule 
                    */

                    return version;
                }

        }

        #region Private methods
        private IQueryable<Job> GetJobsQuery(string systemId)
        {
            var res = new List<Job>();

            var allJobs = unitOfWork.Repository<Job>().Get(x => !x.IsDeleted && x.IsActive);

            var allAssociations = unitOfWork.Repository<SystemJob>().Get(ass => !ass.IsDeleted);

            foreach (var job in allJobs)
            {
                if (allAssociations.Any(ass => ass.JobId == job.JobId && ass.SystemKey == systemId))
                {
                    res.Add(job);
                }
            }

            return res.AsQueryable();
        }

        private PagingResponse<Models.Job> GetJobsCommon(IQueryable<Job> formsQuery, PagingRequest pagingInfo)
        {
            try
            {
                int totalCount = formsQuery.Count();

                var result = formsQuery.AsEnumerable()
                    .Select(f => mapper.Map<Models.Job>(f));

                return result.ToPagingResponse(pagingInfo.GetPageNumber(), totalCount);
            }
            catch (Exception ex)
            {
                throw ErrorCodes.InternalServerError.ToServiceFault(ex.Message);
            }
        }

        private void AssignJobToSystems(string jobId, IEnumerable<string> systemIds)
        {
            var timestamp = DateTime.Now;
            foreach (var systemId in systemIds)
            {
                var newSystemJob = new SystemJob
                {
                    Id = Utils.GenerateGuidKey(),
                    CreatedBy = "-1",
                    CreatedDate = timestamp,
                    JobId = jobId,
                    SystemKey = systemId
                };

                unitOfWork.Repository<SystemJob>().InsertGraph(newSystemJob);
            }
        }

        private IQueryable<Job> GetJobQuery(string jobId)
        {
            return unitOfWork.Repository<Job>()
                    .Get(j => !j.IsDeleted && j.JobId == jobId && j.IsActive);
        }

      
        private void SyncJobParameters(ImportJob importModel, Job jobEntity)
        {
            var model = importModel.Data;
            var sourceJobParameters = model.JobParameters;

            var jobParamsEntity = unitOfWork.Repository<JobParameter>()
                                .Get(p => p.JobId == model.JobId && p.StartVersion <= model.Version &&
                               (p.EndVersion == null || p.EndVersion > jobEntity.Version));

            foreach (var dbParam in jobParamsEntity)
            {
                var found = sourceJobParameters.SingleOrDefault(x => x.Name == dbParam.Name
                                                               && x.Value == dbParam.Value);
                if (found != null)
                {
                    sourceJobParameters.Remove(found);
                }
                else
                {
                    dbParam.EndVersion = jobEntity.Version;
                }
            }

            foreach (var item in sourceJobParameters)
            {
                var newDbJobParam = mapper.Map<JobParameter>(item);
                unitOfWork.Repository<JobParameter>().InsertGraph(newDbJobParam);
            }

        }

        private void ImportJobSchedule(ImportJob importModel, DateTime timeStamp)
        {
            foreach (var schedule in unitOfWork.Repository<JobSchedule>().Get(x => x.JobId == importModel.Data.JobId))
            {
                schedule.IsDeleted = true;
                schedule.DeletedBy = "-1";
                schedule.DeletedDate = DateTime.Now;
            }

            var schedules = importModel.Data.JobSchedules;
            foreach (var schedule in schedules)
            {
                var newSchedule = mapper.Map<JobSchedule>(schedule);
                newSchedule.JobId = importModel.Data.JobId;
                newSchedule.Version = importModel.Data.Version;
                newSchedule.Id = Utils.GenerateGuidKey();
                newSchedule.CreatedBy = "-1";
                newSchedule.CreatedDate = timeStamp;

                unitOfWork.Repository<JobSchedule>().InsertGraph(newSchedule);
            }
        }

        private void ImportJobSource(ImportJob importModel)
        {
            var source = importModel.Data.JobSource;

            if (source == null)
                throw ErrorCodes.BadRequest.ToServiceFault("Job source is null");

            var dbSource = unitOfWork.Repository<JobSource>()
                          .Get(s => s.JobId == importModel.Data.JobId)
                          .FirstOrDefault();


            if (dbSource != null)
            {
                dbSource.API = source.API;
                dbSource.Class = source.Class;
                dbSource.Content = source.Content;
                dbSource.Process = source.Process;
            }
            else
            {
                var newSource = mapper.Map<JobSource>(source);
                newSource.Id = Utils.GenerateGuidKey();
                newSource.JobId = importModel.Data.JobId;


                unitOfWork.Repository<JobSource>()
                    .InsertGraph(newSource);
            }
        }

        private void SyncSystems(ImportJob job, IEnumerable<string> systemIds)
        {
            var distinctSystems = systemIds.Distinct().ToArray();

            var tblJob = unitOfWork.Repository<Job>().Get();
            var tblSystemJob = unitOfWork.Repository<SystemJob>().Get();

            var existingSystemsKeys = tblSystemJob
                                     .Where(sj => sj.JobId == job.Data.JobId
                                         && distinctSystems.Contains(sj.SystemKey)
                                         && !sj.IsDeleted)
                                         .Select(x => x.SystemKey).ToArray();


            var systemsToAssociate = distinctSystems.Except(existingSystemsKeys);

            var systemWithJobs = (from j in tblJob
                                  join sj in tblSystemJob
                                  on j.JobId equals sj.JobId
                                  where systemsToAssociate.Contains(sj.SystemKey)
                                  && j.Name.ToLower() == job.Data.Name.ToLower()
                                  && (j.IsDeleted == false && j.IsActive)
                                  && sj.IsDeleted == false
                                  select sj.SystemKey).ToList();

            if (systemWithJobs.Count > 0)
            {
                throw ErrorCodes.BadRequest.ToServiceFault($"Job with name {job.Data.Name} already exists in {string.Join(",", systemWithJobs)} systems");
            }

            AssignJobToSystems(job.Data.JobId, systemsToAssociate);
        }
        

        #endregion Private methods
    }
}
