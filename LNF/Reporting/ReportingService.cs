namespace LNF.Reporting
{
    public class ReportingService : IReportingService
    {
        public IClientItemRepository ClientItem { get; }
        public IClientManagerLogRepository ClientManagerLog { get; }
        public IClientEmailPreferenceRepository ClientEmailPreference { get; }
        public IManagerUsageChargeRepository ManagerUsageCharge { get; }
        public IAfterHoursRepository AfterHours { get; }

        public ReportingService(
            IClientItemRepository clientItem,
            IClientManagerLogRepository clientManagerLog,
            IClientEmailPreferenceRepository clientEmailPreference,
            IManagerUsageChargeRepository managerUsageCharge,
            IAfterHoursRepository afterHours)
        {
            ClientItem = clientItem;
            ClientManagerLog = clientManagerLog;
            ClientEmailPreference = clientEmailPreference;
            ManagerUsageCharge = managerUsageCharge;
            AfterHours = afterHours;
        }
    }
}
