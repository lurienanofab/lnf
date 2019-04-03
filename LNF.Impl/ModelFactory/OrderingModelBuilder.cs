using LNF.Models.Ordering;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;

namespace LNF.Impl.ModelFactory
{
    public class OrderingModelBuilder : ModelBuilder
    {
        public OrderingModelBuilder(ISession session) : base(session) { }

        private IApprover CreateApproverModel(Approver source)
        {
            var approver = Session.Single<ClientInfo>(source.ApproverID);
            var client = Session.Single<ClientInfo>(source.ClientID);

            var result = CreateModelFrom<ApproverItem>(source);
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
