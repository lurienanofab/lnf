namespace LNF.Data
{
    public interface IDataService
    {
        IClientRepository Client { get; }
        IOrgRepository Org { get; }
        IActiveLogRepository ActiveLog { get; }
        ICostRepository Cost { get; }
        IDryBoxRepository DryBox { get; }
        IAccountRepository Account { get; }
        IRoomRepository Room { get; }
        IServiceLogRepository ServiceLog { get; }
        IHolidayRepository Holiday { get; }
        IFeedRepository Feed { get; }
        IHelpRepository Help { get; }
        IMenuRepository Menu { get; }
        IGlobalSettingRepository GlobalSetting { get; }
        INewsRepository News {get;}
}
}
