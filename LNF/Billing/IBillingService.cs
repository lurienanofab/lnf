using LNF.Billing.Process;
using LNF.Billing.Reports;

namespace LNF.Billing
{
    public interface IBillingService
    {
        IProcessRepository Process { get; }
        IApportionmentRepository Apportionment { get; }
        ISubsidyRepository AccountSubsidy { get; }
        IReportRepository Report { get; }
        IToolBillingRepository Tool { get; }
        IRoomBillingRepository Room { get; }
        IStoreBillingRepository Store { get; }
        IMiscBillingRepository Misc { get; }
        IBillingTypeRepository BillingType { get; }
        IRoomDataRepository RoomData { get; }
        IToolDataRepository ToolData { get; }
        IStoreDataRepository StoreData { get; }
        IMiscDataRepository MiscData { get; }
        IOrgRechargeRepository OrgRecharge { get; }
    }
}
