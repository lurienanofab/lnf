using LNF.Data;
using LNF.Impl.Repository.Data;
using LNF.Mail;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public class DataModelBuilder : ModelBuilder
    {
        public DataModelBuilder(ISessionManager mgr) : base(mgr) { }

        private IAccount MapAccount(Account source)
        {
            var result = Session.Get<AccountInfo>(source.AccountID);
            return result;
        }

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
            result.NewsCreatedByClientID = source.NewsCreatedByClient.ClientID;
            return result;
        }

        public override void AddMaps()
        {
            Map<Account, IAccount>(MapAccount);
            Map<Client, ClientInfo, ClientItem, IClient>(x => x.ClientID);
            Map<ClientInfo, ClientItem, IClient>();
            Map<ClientOrg, ClientOrgInfo, ClientItem, IClient>(x => x.ClientOrgID);
            Map<ClientOrgInfo, ClientItem, IClient>();
            Map<ClientAccount, ClientAccountInfo, IClientAccount>(x => x.ClientAccountID);
            Map<ClientAccountInfo, ClientAccountItem, IClientAccount>();
            Map<Org, OrgInfo, OrgItem, IOrg>(x => x.OrgID);
            Map<OrgInfo, OrgItem, IOrg>();
            Map<Room, IRoom>(MapRoom);
            Map<GlobalCost, IGlobalCost>(MapGlobalCost);
            Map<ClientOrgInfoBase, ITechnicalField>(MapTechnicalField);
            Map<TechnicalField, TechnicalFieldItem, ITechnicalField>();
            Map<Menu, MenuItem, IMenu>();
            Map<ClientRemote, ClientRemoteInfo, ClientRemoteItem, IClientRemote>(x => x.ClientRemoteID);
            Map<ClientRemoteInfo, ClientRemoteItem, IClientRemote>();
            Map<Priv, PrivItem, IPriv>();
            Map<Community, CommunityItem, ICommunity>();
            Map<News, INews>(MapNews);
            Map<Holiday, HolidayItem, IHoliday>();
            Map<InvalidEmailList, InvalidEmailItem, IInvalidEmail>();
        }
    }
}
