using LNF.Scheduler;
using RestSharp;
using System;

namespace OnlineServices.Api.Scheduler
{
    public class ClientSettingRepository : ApiClient, IClientSettingRepository
    {
        internal ClientSettingRepository(IRestClient rc) : base(rc) { }

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
