using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public class OrderingModelBuilder : ModelBuilder
    {
        public OrderingModelBuilder(ISessionManager mgr) : base(mgr) { }

        private IApprover CreateApproverModel(Approver source)
        {
            var approver = Session.Get<ClientInfo>(source.ApproverID);
            var client = Session.Get<ClientInfo>(source.ClientID);

            var result = MapFrom<ApproverItem>(source);
            result.ApproverDisplayName = approver != null ? approver.DisplayName : string.Empty;
            result.DisplayName = client != null ? client.DisplayName : string.Empty;
            return result;
        }

        public override void AddMaps()
        {
            Map<Approver, IApprover>(x => CreateApproverModel(x));
        }
    }
}
