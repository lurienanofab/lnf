using LNF.Billing.Process;
using LNF.Billing.Reports;

namespace LNF.Billing
{
    public class BillingService : IBillingService
    {
        public IProcessRepository Process { get; }

        public IApportionmentRepository Apportionment { get; }

        public ISubsidyRepository AccountSubsidy { get; }

        public IReportRepository Report { get; }

        public IToolBillingRepository Tool { get; }

        public IRoomBillingRepository Room { get; }

        public IStoreBillingRepository Store { get; }

        public IMiscBillingRepository Misc { get; }

        public IBillingTypeRepository BillingType { get; }

        public IRoomDataRepository RoomData { get; }

        public IToolDataRepository ToolData { get; }

        public IStoreDataRepository StoreData { get; }

        public IMiscDataRepository MiscData { get; }

        public IOrgRechargeRepository OrgRecharge { get; }

        public IExternalInvoiceRepository ExternalInvoice { get; }

        public BillingService(
            IProcessRepository process,
            IApportionmentRepository apportionment,
            ISubsidyRepository accountSubsidy,
            IReportRepository report,
            IToolBillingRepository tool,
            IRoomBillingRepository room,
            IStoreBillingRepository store,
            IMiscBillingRepository misc,
            IBillingTypeRepository billingType,
            IRoomDataRepository readRoomData,
            IToolDataRepository readToolData,
            IStoreDataRepository readStoreData,
            IMiscDataRepository readMiscData,
            IOrgRechargeRepository orgRecharge,
            IExternalInvoiceRepository externalInvoice)
        {
            Process = process;
            Apportionment = apportionment;
            AccountSubsidy = accountSubsidy;
            Report = report;
            Tool = tool;
            Room = room;
            Store = store;
            Misc = misc;
            BillingType = billingType;
            RoomData = readRoomData;
            ToolData = readToolData;
            StoreData = readStoreData;
            MiscData = readMiscData;
            OrgRecharge = orgRecharge;
            ExternalInvoice = externalInvoice;
        }
    }
}
