namespace LNF.Data
{
    public class DataService : IDataService
    {
        public IClientRepository Client { get; }
        public IOrgRepository Org { get; }
        public ICostRepository Cost { get; }
        public IDryBoxRepository DryBox { get; }
        public IAccountRepository Account { get; }
        public IRoomRepository Room { get; }
        public IActiveLogRepository ActiveLog { get; }
        public IServiceLogRepository ServiceLog { get; }
        public IHolidayRepository Holiday { get; }
        public IFeedRepository Feed { get; }
        public IHelpRepository Help { get; }
        public IGlobalSettingRepository GlobalSetting { get; }
        public IMenuRepository Menu { get; }
        public INewsRepository News { get; }

        public DataService(
            IClientRepository client,
            IOrgRepository org,
            ICostRepository cost,
            IDryBoxRepository dryBox,
            IAccountRepository account,
            IRoomRepository room,
            IActiveLogRepository activeLog,
            IServiceLogRepository serviceLog,
            IHolidayRepository holiday,
            IFeedRepository feed,
            IHelpRepository help,
            IGlobalSettingRepository globalSetting,
            IMenuRepository menu,
            INewsRepository news)
        {
            Client = client;
            Org = org;
            Cost = cost;
            DryBox = dryBox;
            Account = account;
            Room = room;
            ActiveLog = activeLog;
            ServiceLog = serviceLog;
            Holiday = holiday;
            Feed = feed;
            Help = help;
            GlobalSetting = globalSetting;
            Menu = menu;
            News = news;
        }
    }
}
