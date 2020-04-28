using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Reporting;
using LNF.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Reporting
{
    public class ClientEmailPreferenceRepository : RepositoryBase, IClientEmailPreferenceRepository
    {
        public ClientEmailPreferenceRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IClientEmailPreference> GetClientEmailPreferences(int emailPreferenceId)
        {
            return Session.Query<ClientEmailPreference>().Where(x => x.EmailPreferenceID == emailPreferenceId).ToList();
        }

        public IClientEmailPreference AddClientEmailPreference(int emailPreferenceId, int clientId)
        {
            var result = new ClientEmailPreference
            {
                EmailPreferenceID = emailPreferenceId,
                ClientID = clientId,
                EnableDate = DateTime.Now,
                DisableDate = null
            };

            Session.Save(result);

            return result;
        }
    }
}
