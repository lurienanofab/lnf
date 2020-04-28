namespace LNF.Scheduler
{
    public class SchedulerService : ISchedulerService
    {
        public IResourceRepository Resource { get; }
        public IReservationRepository Reservation { get; }
        public IProcessInfoRepository ProcessInfo { get; }
        public IEmailRepository Email { get; }
        public IClientSettingRepository ClientSetting { get; }
        public IActivityRepository Activity { get; }
        public IKioskRepository Kiosk { get; }
        public ISchedulerPropertyRepository Properties { get; }
        public ILabLocationRepository LabLocation { get; }

        public SchedulerService(
            IResourceRepository resource,
            IReservationRepository reservation,
            IProcessInfoRepository processInfo,
            IEmailRepository email,
            IClientSettingRepository clientSetting,
            IActivityRepository activity,
            IKioskRepository kiosk,
            ISchedulerPropertyRepository properties,
            ILabLocationRepository labLocation)
        {
            Resource = resource;
            Reservation = reservation;
            ProcessInfo = processInfo;
            Email = email;
            ClientSetting = clientSetting;
            Activity = activity;
            Kiosk = kiosk;
            Properties = properties;
            LabLocation = labLocation;
        }
    }
}
