using LNF.Scheduler;
using System;

namespace OnlineServices.Api.Scheduler
{
    public class ClientSettingRepository : ApiClient, IClientSettingRepository
    {
        public IClientSetting GetClientSettingOrDefault(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClientSetting SetAccountOrdering(int clientID, string value)
        {
            throw new NotImplementedException();
        }
    }
}
