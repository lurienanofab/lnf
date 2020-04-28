using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class SubsidyRepository : RepositoryBase, ISubsidyRepository
    {
        public SubsidyRepository(ISessionManager mgr) : base(mgr) { }

        public int AddAccountSubsidy(IAccountSubsidy model)
        {
            var existing = Session.Query<AccountSubsidy>().Where(x => x.AccountID == model.AccountID && x.DisableDate == null);

            if (existing != null && existing.Count() > 0)
            {
                foreach (var item in existing)
                    item.DisableDate = model.EnableDate;
            }

            var entity = new AccountSubsidy()
            {
                AccountID = model.AccountID,
                UserPaymentPercentage = model.UserPaymentPercentage,
                CreatedDate = DateTime.Now,
                EnableDate = model.EnableDate,
            };

            Session.Save(entity);

            return entity.AccountSubsidyID;
        }

        public bool DisableAccountSubsidy(int accountSubsidyId)
        {
            var entity = Session.Get<AccountSubsidy>(accountSubsidyId);
            if (entity == null) return false;
            entity.DisableDate = DateTime.Now.Date.AddDays(1);
            Session.SaveOrUpdate(entity);
            return true;
        }

        public IEnumerable<IAccountSubsidy> GetAccountSubsidy(int? accountId = null)
        {
            return Session.Query<AccountSubsidy>()
                .Where(x => x.AccountID == accountId.GetValueOrDefault(x.AccountID)).CreateModels<IAccountSubsidy>();
        }

        public IEnumerable<IAccountSubsidy> GetActiveAccountSubsidy(DateTime sd, DateTime ed)
        {
            //base query
            var baseQuery = Session.Query<AccountSubsidy>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
                .ToList();

            // step1: it is possible to have duplicates because of disabling and re-enabling in the same
            //        date range, in this case get the last one by joining to self grouped by max AccountSubsidyID
            var step1 = baseQuery.Join(
                baseQuery.GroupBy(x => x.AccountID).Select(g => new { Account = g.Key, AccountSubsidyID = g.Max(n => n.AccountSubsidyID) }),
                o => o.AccountSubsidyID,
                i => i.AccountSubsidyID,
                (o, i) => o);

            return step1.OrderBy(x => x.AccountID).CreateModels<IAccountSubsidy>();
        }

        public IAccountSubsidy GetSingleAccountSubsidy(int accountSubsidyId)
        {
            return Session.Get<AccountSubsidy>(accountSubsidyId).CreateModel<IAccountSubsidy>();
        }

        public IEnumerable<ITieredSubsidyBilling> PopulateSubsidyBilling(DateTime period, int clientId = 0)
        {
            IList<RoomBillingByAccount> roomQuery;
            IList<ToolBillingByAccount> toolQuery;

            if (clientId > 0)
            {
                roomQuery = Session.Query<RoomBillingByAccount>().Where(x => x.Period == period).ToList();
                toolQuery = Session.Query<ToolBillingByAccount>().Where(x => x.Period == period).ToList();
            }
            else
            {
                roomQuery = Session.Query<RoomBillingByAccount>().Where(x => x.Period == period && x.Client.ClientID == clientId).ToList();
                toolQuery = Session.Query<ToolBillingByAccount>().Where(x => x.Period == period && x.Client.ClientID == clientId).ToList();
            }

            List<TieredSubsidyBilling> result = new List<TieredSubsidyBilling>();

            foreach (RoomBillingByAccount item in roomQuery)
            {
                TieredSubsidyBilling tsb = new TieredSubsidyBilling
                {
                    Period = period,
                    Client = item.Client,
                    Org = item.Account.Org,
                    RoomSum = item.TotalCharge,
                    RoomMiscSum = 0,
                    ToolSum = 0,
                    ToolMiscSum = 0
                };

                result.Add(tsb);
            }

            return result.CreateModels<ITieredSubsidyBilling>();
        }
    }
}
