using LNF.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class OrgRepository : ApiClient, IOrgRepository
    {
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

        public IEnumerable<IOrgType> OrgTypes(int chargeTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IChargeType> GetChargeTypes()
        {
            throw new NotImplementedException();
        }

        public IOrg GetOrg(int orgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> GetOrgs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> GetOrgs(int orgTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> GetActiveOrgs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> GetActiveOrgs(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrg> GetActiveOrgs(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IChargeType GetChargeType(int chargeTypeID)
        {
            throw new NotImplementedException();
        }
    }
}
