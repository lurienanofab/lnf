using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IOrgRepository
    {
        IOrg GetOrg(int orgId);
        IEnumerable<IOrg> GetOrgs();
        IEnumerable<IOrg> GetOrgs(int orgTypeId);
        IEnumerable<IOrg> GetActiveOrgs();
        IEnumerable<IOrg> GetActiveOrgs(int clientId);
        IEnumerable<IOrg> GetActiveOrgs(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClient> GetClientOrgs(int orgId);
        IEnumerable<IAccount> GetAccounts(int orgId);
        IEnumerable<IDepartment> GetDepartments(int orgId);
        IOrg GetPrimaryOrg();
        IEnumerable<IOrgType> OrgTypes(int chargeTypeId);
        IEnumerable<IChargeType> GetChargeTypes();
        IChargeType GetChargeType(int chargeTypeId);
    }
}