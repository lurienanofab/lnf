namespace LNF.Models.Scheduler
{
    public class SchedulerService : ISchedulerService
    {
        public IResourceManager Resource { get; }
        public IReservationManager Reservation { get; }
        public IProcessInfoManager ProcessInfo { get; }

        public SchedulerService(IResourceManager resource, IReservationManager reservation, IProcessInfoManager processInfo)
        {
            Resource = resource;
            Reservation = reservation;
            ProcessInfo = processInfo;
        }
    }
}
