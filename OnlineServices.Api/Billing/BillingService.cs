using LNF.Billing;
using LNF.Billing.Process;
using LNF.Billing.Reports;
using RestSharp;

namespace OnlineServices.Api.Billing
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

        internal BillingService(IRestClient rc)
        {
            Process = new ProcessRepository(rc);
            Apportionment = new ApportionmentRepository(rc);
            AccountSubsidy = new AccountSubsidyRepository(rc);
            Report = new ReportRepository(rc);
            Tool = new ToolBillingRepository(rc);
            Room = new RoomBillingRepository(rc);
            Store = new StoreBillingRepository(rc);
            Misc = new MiscBillingRepository(rc);
            BillingType = new BillingTypeRepository(rc);
            RoomData = new RoomDataRepository(rc);
            ToolData = new ToolDataRepository(rc);
            StoreData = new StoreDataRepository(rc);
            MiscData = new MiscDataRepository(rc);
            OrgRecharge = new OrgRechargeRepository(rc);
            ExternalInvoice = new ExternalInvoiceRepository(rc);
        }
    }
}
