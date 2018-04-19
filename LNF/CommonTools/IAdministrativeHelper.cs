using LNF.Models.Data;
using LNF.Repository;
using System.Collections.Generic;

namespace LNF.CommonTools
{
    public interface IAdministrativeHelper : IManager
    {
        IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs);
        void SendEmailToDevelopers(string subject, string body);
    }
}