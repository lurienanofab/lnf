using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class OrgRechargeRepository : RepositoryBase, IOrgRechargeRepository
    {
        public OrgRechargeRepository(ISessionManager mgr) : base(mgr) { }

        public IOrgRecharge GetOrgRecharge(int orgId)
        {
            return Session.Query<OrgRecharge>().FirstOrDefault(x => x.Org.OrgID == orgId && x.DisableDate == null).CreateModel<IOrgRecharge>();
        }

        public IOrgRecharge GetOrgRecharge(int orgId, DateTime sd, DateTime ed)
        {
            return Session.Query<OrgRecharge>().FirstOrDefault(x => x.Org.OrgID == orgId && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd)).CreateModel<IOrgRecharge>();
        }

        public IOrgRecharge AddOrgRecharge(int orgId, int accountId, DateTime createdDate, DateTime endabledDate)
        {
            OrgRecharge result = new OrgRecharge()
            {
                Org = Require<Org>(orgId),
                Account = Require<Account>(accountId),
                CreatedDate = createdDate,
                EnableDate = endabledDate,
                DisableDate = null
            };

            Session.Save(result);

            return result.CreateModel<IOrgRecharge>();
        }

        public IEnumerable<IOrgRecharge> GetActiveOrgRecharges()
        {
            return Session.Query<OrgRecharge>().Where(x => x.DisableDate == null).CreateModels<IOrgRecharge>();
        }

        public void Disable(int orgRechargeId, DateTime disableDate)
        {
            var or = Require<OrgRecharge>(orgRechargeId);
            or.DisableDate = disableDate;
            Session.Update(or);
        }
    }
}
