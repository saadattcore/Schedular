using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Emaratech.Services.Common.Models;
using Emaratech.Services.Scheduler.Contracts.Rest.Models;
using Emaratech.Services.WcfCommons.Cors;
using SwaggerWcf.Attributes;
using Emaratech.Services.WcfCommons.Faults.Models;
using Emaratech.Services.Workflows.Contracts.Rest.Models;
using System.IO;
using System.Collections.Generic;
using ImpromptuInterface;
using Emaratech.Services.Scheduler.Contracts.Export;

namespace Emaratech.Services.Scheduler
{
    [UseNamedArgument]
    [ServiceContract]
    public interface ISchedulerService : ICorsAwareRestWcfService
    {
        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Get all jobs", "Get all jobs.", "GetJobsBySystemId")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract(Name = "GetJobsPagedBySystemId")]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/jobs?pageNumber={pageNumber}&itemsCount={itemsCount}&systemId={systemId}")]
        PagingResponse<RestJob> GetJobsBySystemId(string systemId, string pageNumber, string itemsCount);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Get all jobs", "Get all jobs.", "GetJobsBySystemIds")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract(Name = "GetJobsPagedBySystemIds")]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs?pageNumber={pageNumber}&itemsCount={itemsCount}")]
        PagingResponse<RestJob> GetJobsBySystemIds(RestSystems data, string pageNumber, string itemsCount);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Search jobs", "Search jobs.", "SearchJobs")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract(Name = "SearchJobsPagedBySystem")]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/search/jobs?pageNumber={pageNumber}&itemsCount={itemsCount}&systemId={systemId}&query={query}")]
        PagingResponse<RestJob> SearchJobs(string query, string systemId, string pageNumber, string itemsCount);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Search next job to execute", "Search next job to execute.", "FetchNext")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract(Name = "FetchNext")]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs/fetchNext")]
        RestNextJobInfo FetchNext(RestJobFilter filter);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Add job", "Add job.", "AddJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs")]
        string AddJob(RestJobAdd rjob);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Lock job", "Lock job.", "LockJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs/{jobId}/lock")]
        bool LockJob(string jobId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Enable job", "Enable job.", "EnableJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs/{jobId}/enable")]
        long EnableJob(string jobId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Disable job", "Disable job.", "DisableJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs/{jobId}/disable")]
        long DisableJob(string jobId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Unlock job", "Unlock job.", "UnlockJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/jobs/{jobId}/unlock")]
        long UnlockJob(string jobId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Update job", "Update job.", "UpdateJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "PUT", UriTemplate = "/jobs/{id}")]
        long UpdateJob(string id, RestJobUpdate rjob);
        
        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Remove job", "Remove job.", "DeleteJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/jobs/{id}")]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        void DeleteJob(string id);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Get job by id and version", "Get job by id and version.", "GetJobByIdAndVersion")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract(Name = "GetJobByIdAndVersion")]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/jobs/{id}/{version}")]
        RestJobEx GetJobByIdAndVersion(string id, string version);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Get job by id", "Get job by id.", "GetJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/jobs/{id}")]
        RestJobEx GetJob(string id);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Assign job to a system", "Assign job to a system.", "AssignJobToSystem")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/jobs/assign/{jobId}/{systemId}")]
        void AssignJobToSystem(string jobId, string systemId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Unassign job to a system", "Unassign job to a system.", "RemoveJobFromSystem")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/jobs/unassign/{jobId}/{systemId}")]
        void RemoveJobFromSystem(string jobId, string systemId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Get schedule types", "Get schedule types.", "GetScheduleTypes")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/schedule/types")]
        IEnumerable<RestScheduleType> GetScheduleTypes();

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Schedule a job", "Schedule a job.", "ScheduleJob")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/schedule")]
        void AddJobSchedule(RestJobSchedule schedule);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Remove job schedule", "Remove job schedule.", "DeleteJobSchedule")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "DELETE", UriTemplate = "/schedule/{id}")]
        void DeleteJobSchedule(string id);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Get job content", "Get job content.", "GetJobContent")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebGet(UriTemplate = "/jobs/{jobId}/{version}/content")]
        Stream GetJobContent(string jobId, string version);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Create a job instance", "Add job instance.", "AddJobInstance")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "POST", UriTemplate = "/instances")]
        string AddJobInstance(RestJobInstance jobInstance);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfPath("Update job instance", "Update job instance.", "UpdateJobInstance")]
        [SwaggerWcfResponse(HttpStatusCode.OK)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", true)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        [WebInvoke(Method = "PUT", UriTemplate = "/instances/{id}")]
        void UpdateJobInstance(string id, RestJobInstanceUpdate rjobInstance);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfTag("Export-Import")]
        [SwaggerWcfPath("Export job", "Export job by id", "Export")]
        [SwaggerWcfResponse(HttpStatusCode.OK, "Ok")]
        [SwaggerWcfResponse(HttpStatusCode.NotFound, "No data found", true)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Server-side error", true)]
        [SwaggerWcfResponse(HttpStatusCode.BadRequest, "Client-side error (invalid input data)", true)]
        [WebGet(UriTemplate = "/job/{entityId}/export")]
        [OperationContract(Name = "Export")]
        ExportJob Export(string entityId);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfTag("Export-Import")]
        [SwaggerWcfPath("Export job version", "Export specific version of job by id", "ExportVersion")]
        [SwaggerWcfResponse(HttpStatusCode.OK, "Ok")]
        [SwaggerWcfResponse(HttpStatusCode.NotFound, "No data found", true)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Server-side error", true)]
        [SwaggerWcfResponse(HttpStatusCode.BadRequest, "Client-side error (invalid input data)", true)]
        [WebGet(UriTemplate = "/job/{entityId}/{version}/export")]
        [OperationContract(Name = "ExportVersion")]
        ExportJob ExportVersion(string entityId, string version);

        [SwaggerWcfTag("Scheduler")]
        [SwaggerWcfTag("Export-Import")]
        [SwaggerWcfPath("Import job", "Import job", "Import")]
        [SwaggerWcfResponse(HttpStatusCode.OK, "Ok")]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Server-side error", true)]
        [SwaggerWcfResponse(HttpStatusCode.BadRequest, "Client-side error (invalid input data)", true)]
        [OperationContract(Name = "Import")]
        [WebInvoke(Method = "POST",
            UriTemplate = "/job/import",
            BodyStyle = WebMessageBodyStyle.Bare)]
        long Import(ImportJob importModel);
    }
}