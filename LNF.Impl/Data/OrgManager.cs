using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class OrgManager : ManagerBase, IOrgManager
    {
        public OrgManager(IProvider provider) : base(provider) { }

        public IEnumerable<IAccount> GetAccounts(int orgId)
        {
            return Session.Query<AccountInfo>().Where(x => x.OrgID == orgId).CreateModels<IAccount>();
        }

        public IEnumerable<IClient> GetClientOrgs(int orgId)
        {
            return Session.Query<ClientOrgInfo>().Where(x => x.OrgID == orgId).CreateModels<IClient>();
        }

        public IEnumerable<IDepartment> GetDepartments(int orgId)
        {
            return Session.Query<Department>().Where(x => x.Org.OrgID == orgId).CreateModels<IDepartment>();
        }

        public IOrg GetPrimaryOrg()
        {
            return Session.Query<OrgInfo>().FirstOrDefault(x => x.PrimaryOrg).CreateModel<IOrg>();
        }

        public IEnumerable<IOrg> SelectActive()
        {
            return Session.Query<OrgInfo>().Where(x => x.OrgActive).OrderBy(x => x.OrgName).CreateModels<IOrg>();
        }

        public IEnumerable<IOrg> Orgs(int orgTypeId)
        {
            return Session.Query<OrgInfo>().Where(x => x.OrgTypeID == orgTypeId).CreateModels<IOrg>();
        }

        public IAccount GetAccount(IChargeType chargeType)
        {
            return Session.Single<AccountInfo>(chargeType.AccountID).CreateModel<IAccount>();
        }

        public IEnumerable<IOrgType> OrgTypes(int chargeTypeId)
        {
            return Session.Query<OrgType>().Where(x => x.ChargeType.ChargeTypeID == chargeTypeId).CreateModels<IOrgType>();
        }

        public IEnumerable<IChargeType> GetChargeTypes()
        {
            return Session.Query<ChargeType>().CreateModels<IChargeType>();
        }
    }
}