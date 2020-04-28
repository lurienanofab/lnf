using System.Collections.Generic;

namespace LNF.Data
{
    public interface IClientClient
    {
        IEnumerable<ClientItem> GetInternalUsers();
        IEnumerable<ClientItem> GetExternalUsers();
        IEnumerable<ClientItem> GetStaff();
        IEnumerable<ClientItem> GetAll();
        ClientItem GetCurrentUser();
    }
}