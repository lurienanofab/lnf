using LNF.Models.Data.Utility;

namespace LNF.Models.Data
{
    public interface IDataService
    {
        IClientManager Client { get; }
        IOrgManager Org { get; }
        IActiveLogManager ActiveLog { get; }
        ICostManager Cost { get; }
        IDryBoxManager DryBox { get; }
        IAccountManager Account { get; }
        IRoomManager Room { get; }
        IServiceLogManager ServiceLog { get; }
        IUtilityManager Utility { get; }
        IFeedManager Feed { get; }
    }
}
