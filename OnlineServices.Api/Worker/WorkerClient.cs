using LNF.Models;
using LNF.Models.Worker;

namespace OnlineServices.Api.Worker
{
    public class WorkerClient : ApiClient, IWorkerService
    {
        public WorkerClient() : base(GetApiBaseUrl()) { }

        public string Execute(WorkerRequest req)
        {
            return Post("worker/api/execute", req);
        }
    }
}
