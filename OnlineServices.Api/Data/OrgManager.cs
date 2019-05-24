using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class OrgManager : ApiClient, IOrgManager
    {
        public IAccount GetAccount(IChargeType chargeType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetAccounts(int orgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetClientOrgs(int orgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDepartment> GetDepartments(int orgId)
        {
            throw new NotImplementedException();
        }

        public IOrg GetPrimaryOrg()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> Orgs(int orgTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrgType> OrgTypes(int chargeTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> SelectActive()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IChargeType> GetChargeTypes()
        {
            throw new NotImplementedException();
        }
    }
}
