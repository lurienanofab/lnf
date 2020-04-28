namespace LNF.Reporting
{
    public interface IReportingService
    {
        IClientItemRepository ClientItem { get; }
        IClientManagerLogRepository ClientManagerLog { get; }
        IClientEmailPreferenceRepository ClientEmailPreference { get; }
        IManagerUsageChargeRepository ManagerUsageCharge { get; }
        IAfterHoursRepository AfterHours { get; }
    }
}
