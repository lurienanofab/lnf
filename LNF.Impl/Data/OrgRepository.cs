using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class OrgRepository : RepositoryBase, IOrgRepository
    {
        public OrgRepository(ISessionManager mgr) : base(mgr) { }

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

        public IEnumerable<IOrg> GetOrgs()
        {
            return Session.Query<Org>().OrderBy(x => x.OrgName).CreateModels<IOrg>();
        }

        public IEnumerable<IOrg> GetOrgs(int orgTypeId)
        {
            return Session.Query<Org>().Where(x => x.OrgType.OrgTypeID == orgTypeId).CreateModels<IOrg>();
        }

        public IEnumerable<IOrg> GetActiveOrgs()
        {
            return Session.Query<OrgInfo>().Where(x => x.OrgActive).OrderBy(x => x.OrgName).CreateModels<IOrg>();
        }

        public IEnumerable<IOrg> GetActiveOrgs(int clientId)
        {
            var join = Session.Query<Org>()
                .Join(Session.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId && x.ClientOrgActive && x.OrgActive),
                     o => o.OrgID, i => i.OrgID, (o, i) => o)
                .ToList();

            var result = join.CreateModels<IOrg>();

            return result;
        }

        public IEnumerable<IOrg> GetActiveOrgs(int clientId, DateTime sd, DateTime ed)
        {
            ActiveLog alog = null;
            ClientOrg co = null;
            Org org = null;

            var query = Session.QueryOver(() => org)
                .JoinAlias(() => org, () => co.Org)
                .JoinAlias(() => alog.Record, () => org.OrgID)
                .Where(() => alog.TableName == "ClientOrg" && alog.EnableDate < ed && (alog.DisableDate == null || alog.DisableDate > sd))
                    .And(() => co.Client.ClientID == clientId);

            var list = query.List();
            
            var result = list.CreateModels<IOrg>();

            return result;
        }

        public IEnumerable<IOrg> Orgs(int orgTypeId)
        {
            return Session.Query<OrgInfo>().Where(x => x.OrgTypeID == orgTypeId).CreateModels<IOrg>();
        }

        public IAccount GetAccount(IChargeType chargeType)
        {
            return Session.Get<AccountInfo>(chargeType.AccountID).CreateModel<IAccount>();
        }

        public IEnumerable<IOrgType> OrgTypes(int chargeTypeId)
        {
            return Session.Query<OrgType>().Where(x => x.ChargeType.ChargeTypeID == chargeTypeId).CreateModels<IOrgType>();
        }

        public IEnumerable<IChargeType> GetChargeTypes()
        {
            return Session.Query<ChargeType>().CreateModels<IChargeType>();
        }

        public IOrg GetOrg(int orgId)
        {
            return Session.Get<Org>(orgId).CreateModel<IOrg>();
        }

        public IChargeType GetChargeType(int chargeTypeId)
        {
            return Session.Get<ChargeType>(chargeTypeId).CreateModel<IChargeType>();
        }
    }
}