using LNF.Reporting;
using RestSharp;

namespace OnlineServices.Api.Reporting
{
    public class ReportingService : ApiClient, IReportingService
    {
        public IClientItemRepository ClientItem { get; }

        public IClientManagerLogRepository ClientManagerLog { get; }

        public IClientEmailPreferenceRepository ClientEmailPreference { get; }

        public IManagerUsageChargeRepository ManagerUsageCharge { get; }

        public IAfterHoursRepository AfterHours { get; }

        internal ReportingService(IRestClient rc) : base(rc)
        {
            ClientItem = new ClientItemRepository(rc);
            ClientManagerLog = new ClientManagerLogRepository(rc);
            ClientEmailPreference = new ClientEmailPreferenceRepository(rc);
            ManagerUsageCharge = new ManagerUsageChargeRepository(rc);
            AfterHours = new AfterHoursRepository(rc);
        }
    }
}