using LNF.Worker;
using RestSharp;

namespace OnlineServices.Api.Worker
{
    public class WorkerService : ApiClient, IWorkerService
    {
        internal WorkerService(IRestClient rc) : base(rc) { }

        public string Execute(WorkerRequest req)
        {
            return Post("worker/api/execute", req);
        }
    }
}
