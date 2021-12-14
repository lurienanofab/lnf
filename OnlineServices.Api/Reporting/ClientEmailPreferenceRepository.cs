using LNF.Reporting;
using RestSharp;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class ClientEmailPreferenceRepository : ApiClient, IClientEmailPreferenceRepository
    {
        internal ClientEmailPreferenceRepository(IRestClient rc) : base(rc) { }

        public IClientEmailPreference AddClientEmailPreference(int emailPreferenceId, int clientId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IClientEmailPreference> GetClientEmailPreferences(int emailPreferenceId)
        {
            throw new System.NotImplementedException();
        }
    }
}