using LNF.Models.Data.Utility;

namespace LNF.Models.Data
{
    public class DataService : IDataService
    {
        public IClientManager Client { get; }
        public IOrgManager Org { get; }
        public ICostManager Cost { get; }
        public IDryBoxManager DryBox { get; }
        public IAccountManager Account { get; }
        public IRoomManager Room { get; }
        public IActiveLogManager ActiveLog { get; }
        public IServiceLogManager ServiceLog { get; }
        public IUtilityManager Utility { get; }
        public IFeedManager Feed { get; }

        public DataService(
            IClientManager client,
            IOrgManager org,
            ICostManager cost,
            IDryBoxManager dryBox,
            IAccountManager account,
            IRoomManager room,
            IActiveLogManager activeLog,
            IServiceLogManager serviceLog,
            IUtilityManager utility,
            IFeedManager feed)
        {
            Client = client;
            Org = org;
            Cost = cost;
            DryBox = dryBox;
            Account = account;
            Room = room;
            ActiveLog = activeLog;
            ServiceLog = serviceLog;
            Utility = utility;
            Feed = feed;
        }
    }
}
