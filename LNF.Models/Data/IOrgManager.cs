using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IOrgManager
    {
        IEnumerable<IAccount> GetAccounts(int orgId);
        IEnumerable<IClient> GetClientOrgs(int orgId);
        IEnumerable<IDepartment> GetDepartments(int orgId);
        IOrg GetPrimaryOrg();
        IEnumerable<IOrg> SelectActive();
        IEnumerable<IOrg> Orgs(int orgTypeId);
        IAccount GetAccount(IChargeType chargeType);
        IEnumerable<IOrgType> OrgTypes(int chargeTypeId);
        IEnumerable<IChargeType> GetChargeTypes();
    }
}