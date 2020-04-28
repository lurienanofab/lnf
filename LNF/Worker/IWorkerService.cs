namespace LNF.Worker
{
    public interface IWorkerService
    {
        string Execute(WorkerRequest req);
    }
}
