using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class GlobalSettingRepository : RepositoryBase, IGlobalSettingRepository
    {
        public GlobalSettingRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IGlobalSetting> GetGlobalSettings()
        {
            return Session.Query<GlobalSettings>().ToList();
        }

        public IGlobalSetting GetGlobalSetting(string name)
        {
            return Session.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == name);
        }
    }
}
