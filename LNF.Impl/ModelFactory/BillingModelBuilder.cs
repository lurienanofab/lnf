using LNF.Models.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory
{
    public class BillingModelBuilder : ModelBuilder
    {
        public BillingModelBuilder(ISession session) : base(session) { }

        public IToolData MapToolData(ToolData source)
        {
            var acct = Session.Single<AccountInfo>(source.AccountID);
            var result = MapFrom<ToolDataItem>(source);
            result.OrgID = acct.OrgID;
            return result;
        }

        public override void AddMaps()
        {
            Map<ToolData, IToolData>(x => MapToolData(x));
            Map<ApportionmentClient, ApportionmentClientItem, IApportionmentClient>();
            Map<RoomDataImportLog, RoomDataImportLogItem, IRoomDataImportLog>();
        }
    }
}
