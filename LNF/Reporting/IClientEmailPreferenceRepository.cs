using System.Collections.Generic;

namespace LNF.Reporting
{
    public interface IClientEmailPreferenceRepository
    {
        IEnumerable<IClientEmailPreference> GetClientEmailPreferences(int emailPreferenceId);
        IClientEmailPreference AddClientEmailPreference(int emailPreferenceId, int clientId);
    }
}
