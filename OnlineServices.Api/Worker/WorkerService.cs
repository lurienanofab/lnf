using LNF.Models;
using LNF.Models.Worker;

namespace OnlineServices.Api.Worker
{
    public class WorkerService : ApiClient, IWorkerService
    {
        public WorkerService() : base(GetApiBaseUrl()) { }

        public string Execute(WorkerRequest req)
        {
            return Post("worker/api/execute", req);
        }
    }
}
