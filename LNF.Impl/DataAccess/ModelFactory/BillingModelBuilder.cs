using LNF.Billing;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public class BillingModelBuilder : ModelBuilder
    {
        public BillingModelBuilder(ISessionManager mgr) : base(mgr) { }

        public IToolData MapToolData(ToolData source)
        {
            var acct = Session.Get<AccountInfo>(source.AccountID);
            var result = MapFrom<ToolDataItem>(source);
            result.OrgID = acct.OrgID;
            return result;
        }

        public override void AddMaps()
        {
            Map<ToolData, IToolData>(x => MapToolData(x));
            Map<RoomDataImportLog, RoomDataImportLogItem, IRoomDataImportLog>();
            Map<RoomBilling, RoomBillingItem, IRoomBilling>();
            Map<RoomBillingTemp, RoomBillingItem, IRoomBilling>();
            Map<ToolBilling, ToolBillingItem, IToolBilling>();
            Map<ToolBillingTemp, ToolBillingItem, IToolBilling>();
        }
    }
}
