using LNF.Models.Worker;

namespace LNF.Models
{
    public interface IWorkerService
    {
        string Execute(WorkerRequest req);
    }
}
