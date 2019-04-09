namespace LNF.Models.Worker
{
    public interface IWorkerService
    {
        string Execute(WorkerRequest req);
    }
}
