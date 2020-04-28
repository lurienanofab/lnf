using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.Scheduler
{
    public class ClientSettingRepository : RepositoryBase, IClientSettingRepository
    {
        public ClientSettingRepository(ISessionManager mgr) : base(mgr) { }

        public IClientSetting GetClientSettingOrDefault(int clientId)
        {
            var result = Session.Get<ClientSetting>(clientId);

            if (result == null)
            {
                result = ClientSetting.CreateWithDefaultValues(clientId);
                Session.Save(result); //creates a new record in the db
            }

            return result;
        }

        public IClientSetting SetAccountOrdering(int clientId, string value)
        {
            var cs = Require<ClientSetting>(clientId);
            cs.AccountOrder = value;
            Session.Update(cs);
            return cs;
        }
    }
}
