using LNF.Data;
using RestSharp;

namespace OnlineServices.Api.Data
{
    public class DataService : IDataService
    {
        public IClientRepository Client { get; }

        public IOrgRepository Org { get; }

        public IActiveLogRepository ActiveLog { get; }

        public ICostRepository Cost { get; }

        public IDryBoxRepository DryBox { get; }

        public IAccountRepository Account { get; }

        public IRoomRepository Room { get; }

        public IServiceLogRepository ServiceLog { get; }

        public IHolidayRepository Holiday { get; }

        public IFeedRepository Feed { get; }

        public IHelpRepository Help { get; }

        public IMenuRepository Menu { get; }

        public IGlobalSettingRepository GlobalSetting { get; }

        internal DataService(IRestClient rc)
        {
            Client = new ClientRepository(rc);
            Org = new OrgRepository(rc);
            ActiveLog = new ActiveLogRepository(rc);
            Cost = new CostRepository(rc);
            DryBox = new DryBoxRepository(rc);
            Account = new AccountRepository(rc);
            Room = new RoomRepository(rc);
            ServiceLog = new ServiceLogRepository(rc);
            Holiday = new HolidayRepository(rc);
            Feed = new FeedRepository(rc);
            Help = new HelpRepository(rc);
            Menu = new MenuRepository(rc);
            GlobalSetting = new GlobalSettingRepository(rc);
        }
    }
}
