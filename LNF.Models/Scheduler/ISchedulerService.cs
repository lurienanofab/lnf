namespace LNF.Models.Scheduler
{
    public interface ISchedulerService
    {
        IResourceManager Resource { get; }
        IReservationManager Reservation { get; }
        IProcessInfoManager ProcessInfo { get; }
    }
}
