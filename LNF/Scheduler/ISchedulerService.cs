namespace LNF.Scheduler
{
    public interface ISchedulerService
    {
        IResourceRepository Resource { get; }
        IReservationRepository Reservation { get; }
        IProcessInfoRepository ProcessInfo { get; }
        IEmailRepository Email { get; }
        IClientSettingRepository ClientSetting { get; }
        IActivityRepository Activity { get; }
        IKioskRepository Kiosk { get; }
        ISchedulerPropertyRepository Properties { get; }
        ILabLocationRepository LabLocation { get; }
    }
}
