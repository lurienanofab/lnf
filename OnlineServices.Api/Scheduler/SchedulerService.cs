using LNF.Scheduler;
using RestSharp;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerService : ApiClient, ISchedulerService
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

        internal SchedulerService(IRestClient rc) : base(rc)
        {
            Resource = new ResourceRepository(rc);
            Reservation = new ReservationRepository(rc);
            ProcessInfo = new ProcessInfoRepository(rc);
            Email = new EmailRepository(rc);
            ClientSetting = new ClientSettingRepository(rc);
            Activity = new ActivityRepository(rc);
            Kiosk = new KioskRepository(rc);
        }
    }
}