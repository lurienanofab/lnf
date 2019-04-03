using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory
{
    public class DataModelBuilder : ModelBuilder
    {
        public DataModelBuilder(ISession session) : base(session) { }

        private IRoom CreateRoomModel(Room source)
        {
            var result = CreateModelFrom<RoomItem>(source);
            result.RoomDisplayName = source.DisplayName;
            return result;
        }

        private IGlobalCost CreateGlobalCostModel(GlobalCost source)
        {
            var result = CreateModelFrom<GlobalCostItem>(source);
            result.AdminID = source.Admin.ClientID;
            result.LabAccountID = source.LabAccount.AccountID;
            result.LabCreditAccountID = source.LabCreditAccount.AccountID;
            result.SubsidyCreditAccountID = source.SubsidyCreditAccount.AccountID;
            return result;
        }

        public override void AddMaps()
        {
            Map<Client, IClient>(x => CreateModelFrom<ClientInfo, ClientItem>(x.ClientID));
            Map<ClientInfo, IClient>(x => CreateModelFrom<ClientItem>(x));
            Map<ClientOrg, IClient>(x => CreateModelFrom<ClientOrgInfo, ClientItem>(x.ClientOrgID));
            Map<ClientOrgInfo, IClient>(x => CreateModelFrom<ClientItem>(x));
            Map<ClientAccount, IClientAccount>(x => CreateModelFrom<ClientAccountInfo, ClientAccountItem>(x.ClientAccountID));
            Map<ClientAccountInfo, IClientAccount>(x => CreateModelFrom<ClientAccountItem>(x));
            Map<Org, IOrg>(x => CreateModelFrom<OrgInfo, OrgItem>(x.OrgID));
            Map<OrgInfo, IOrg>(x => CreateModelFrom<OrgItem>(x));
            Map<Account, IAccount>(x => CreateModelFrom<AccountInfo, AccountItem>(x.AccountID));
            Map<AccountInfo, IAccount>(x => CreateModelFrom<AccountItem>(x));
            Map<Room, IRoom>(x => CreateRoomModel(x));
            Map<GlobalCost, IGlobalCost>(x => CreateGlobalCostModel(x));
        }
    }
}
