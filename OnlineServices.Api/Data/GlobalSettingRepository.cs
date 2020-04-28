using LNF.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class GlobalSettingRepository : ApiClient, IGlobalSettingRepository
    {
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
