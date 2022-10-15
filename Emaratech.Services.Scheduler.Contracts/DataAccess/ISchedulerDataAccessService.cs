using System.ServiceModel;
using Emaratech.Services.Common.Models;
using Emaratech.Services.Scheduler.Contracts.DataAccess.Models;
using Emaratech.Services.Scheduler.Contracts.Rest.Models;
using Emaratech.Services.WcfCommons.Faults.Models;
using System.Collections.Generic;
using System.IO;
using ImpromptuInterface;
using Emaratech.Services.Scheduler.Contracts.Export;

namespace Emaratech.Services.Scheduler.Contracts.DataAccess
{
    [UseNamedArgument]
    [ServiceContract]
    public interface ISchedulerDataAccessService
    {
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        string AddJob(Job job); //returns job id

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        long UpdateJob(string id, Job job); //returns job version

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void DeleteJob(string id);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        bool LockJob(string id);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        long UnlockJob(string id);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void UnlockHangingJobs();

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        long EnableJob(string id);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        long DisableJob(string id);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Job FetchNext(IEnumerable<string> jobIds, IEnumerable<string> tags);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Job GetJob(string id, int? version);
        
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void AssignJobToSystem(string jobId, string systemId);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void RemoveJobFromSystem(string jobId, string systemId);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        PagingResponse<Job> GetJobs(string systemId, PagingRequest pagingRequest);

        [OperationContract(Name = "GetJobByManySystems")]
        [FaultContract(typeof(ErrorModel))]
        PagingResponse<Job> GetJobs(string[] systemId, PagingRequest pagingRequest);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        PagingResponse<Job> SearchJobs(string systemId, PagingRequest<string> searchRequest);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        IEnumerable<ScheduleType> GetScheduleTypes();

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void AddJobSchedule(JobSchedule schedule);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void DeleteJobSchedule(string id);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Stream GetJobContent(string jobId, int version);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        string AddJobInstance(string jobId);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void UpdateJobInstance(string id, JobInstance job);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        ExportJob Export(string entityId , int? version);

        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        long Import(ImportJob importModel);

    }
}