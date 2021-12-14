using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class GlobalSettingRepository : ApiClient, IGlobalSettingRepository
    {
        internal GlobalSettingRepository(IRestClient rc) : base(rc) { }

        public IGlobalSetting GetGlobalSetting(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGlobalSetting> GetGlobalSettings()
        {
            throw new NotImplementedException();
        }
    }
}
