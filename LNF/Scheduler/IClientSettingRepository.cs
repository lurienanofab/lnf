namespace LNF.Scheduler
{
    public interface IClientSettingRepository
    {
        IClientSetting GetClientSettingOrDefault(int clientId);
        IClientSetting SetAccountOrdering(int clientID, string value);
    }
}
