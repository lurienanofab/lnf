using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory
{
    public class DataModelBuilder : ModelBuilder
    {
        public DataModelBuilder(ISession session) : base(session) { }

        private IRoom MapRoom(Room source)
        {
            var result = MapFrom<RoomItem>(source);
            result.RoomDisplayName = source.DisplayName;
            return result;
        }

        private IGlobalCost MapGlobalCost(GlobalCost source)
        {
            var result = MapFrom<GlobalCostItem>(source);
            result.AdminID = source.Admin.ClientID;
            result.LabAccountID = source.LabAccount.AccountID;
            result.LabCreditAccountID = source.LabCreditAccount.AccountID;
            result.SubsidyCreditAccountID = source.SubsidyCreditAccount.AccountID;
            return result;
        }

        private ITechnicalField MapTechnicalField(ClientOrgInfoBase source)
        {
            return new TechnicalFieldItem
            {
                TechnicalFieldID = source.TechnicalInterestID,
                TechnicalFieldName = source.TechnicalInterestName
            };
        }

        private INews MapNews(News source)
        {
            var result = MapFrom<NewsItem>(source);
            result.NewsCreatedByClient = MapFrom<ClientItem>(source.NewsCreatedByClient);
            return result;
        }

        public override void AddMaps()
        {
            Map<Client, ClientInfo, ClientItem, IClient>(x => x.ClientID);
            Map<ClientInfo, ClientItem, IClient>();
            Map<ClientOrg, ClientOrgInfo, ClientItem, IClient>(x => x.ClientOrgID);
            Map<ClientOrgInfo, ClientItem, IClient>();
            Map<ClientAccount, ClientAccountInfo, ClientAccountItem, IClientAccount>(x => x.ClientAccountID);
            Map<ClientAccountInfo, ClientAccountItem, IClientAccount>();
            Map<Org, OrgInfo, OrgItem, IOrg>(x => x.OrgID);
            Map<OrgInfo, OrgItem, IOrg>();
            Map<Account, AccountInfo, AccountItem, IAccount>(x => x.AccountID);
            Map<AccountInfo, AccountItem, IAccount>();
            Map<Room, IRoom>(x => MapRoom(x));
            Map<GlobalCost, IGlobalCost>(x => MapGlobalCost(x));
            Map<ClientOrgInfoBase, ITechnicalField>(x => MapTechnicalField(x));
            Map<TechnicalField, TechnicalFieldItem, ITechnicalField>();
            Map<Menu, MenuItem, IMenu>();
            Map<ClientRemote, ClientRemoteInfo, ClientRemoteItem, IClientRemote>(x => x.ClientRemoteID);
            Map<ClientRemoteInfo, ClientRemoteItem, IClientRemote>();
            Map<Priv, PrivItem, IPriv>();
            Map<Community, CommunityItem, ICommunity>();
            Map<News, INews>(x => MapNews(x));
        }
    }
}
