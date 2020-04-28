using LNF.Billing;
using LNF.CommonTools;
using LNF.Impl.Data;
using LNF.Impl.Repository.Data;
using NHibernate;
using System;
using System.Linq;

namespace LNF.Impl.Repository.Billing
{
    public static class SessionExtensions
    {
        public static int UpdateRoomBillingType(this ISession session, int clientId, int accountId, int billingTypeId, DateTime period)
        {
            string queryName = "UpdateBillingTypeRoomBilling" + (Utility.IsCurrentPeriod(period) ? "Temp" : string.Empty);
            return session.GetNamedQuery(queryName)
                .SetParameter("ClientID", clientId)
                .SetParameter("AccountID", accountId)
                .SetParameter("BillingTypeID", billingTypeId)
                .SetParameter("Period", period)
                .ExecuteUpdate();
        }

        public static int UpdateToolBillingType(this ISession session, int clientId, int accountId, int billingTypeId, DateTime period)
        {
            string queryName = "UpdateBillingTypeToolBilling" + ((Utility.IsCurrentPeriod(period)) ? "Temp" : string.Empty);
            return session.GetNamedQuery(queryName)
                .SetParameter("ClientID", clientId)
                .SetParameter("AccountID", accountId)
                .SetParameter("BillingTypeID", billingTypeId)
                .SetParameter("Period", period)
                .ExecuteUpdate();
        }

        public static IBillingType GetBillingType(this ISession session, int clientId, int accountId, DateTime period)
        {
            // always add one more month for @Period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            // 2011-01-26 the above statement is not quite right.  We should not allow change after the Period.  if a change is made on 2011-01-04, it has nothing
            // to do with period = 2010-12-01
            //set @Period = dbo.udf_BusinessDate (DATEADD(MONTH, 1, @Period), null)

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            var client = session.Require<Client>(clientId);
            var acct = session.Require<Account>(accountId);

            int record = 0;
            IBillingType result = null;

            var clientOrgs = session.Query<ClientOrg>().Where(x => x.Client == client && x.Org == acct.Org).FindActive(x => x.ClientOrgID, sd, ed, session.Query<ActiveLog>().Where(x => x.TableName == "ClientOrg"));
            var clientAccounts = session.Query<ClientAccount>().Where(x => x.ClientOrg.Client == client && x.Account == acct).FindActive(x => x.ClientAccountID, sd, ed, session.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount"));
            var clientRemotes = session.Query<ClientRemote>().Where(x => x.Client == client && x.Account == acct).FindActive(x => x.ClientRemoteID, sd, ed, session.Query<ActiveLog>().Where(x => x.TableName == "ClientRemote"));
            var co = clientOrgs.FirstOrDefault(x => x.Client == client && x.Org == acct.Org);

            if (co != null)
            {
                //is null for remote runs
                ClientAccount ca = clientAccounts.FirstOrDefault(x => x.ClientOrg == co && x.Account == acct);
                if (ca != null)
                    record = ca.ClientAccountID;
            }

            if (record == 0)
            {
                ClientRemote cr = clientRemotes.FirstOrDefault(x => x.Client == client && x.Account == acct);
                if (cr != null)
                    record = cr.ClientRemoteID;
                if (record == 0)
                    result = BillingTypes.RegularException;
                else
                    result = BillingTypes.Remote;
            }
            else
            {
                ClientOrgBillingTypeLog cobtlog = null;

                if (co != null)
                {
                    var cobtLogs = session.ActiveClientOrgBillingTypeLogQuery(sd, ed).Where(x => x.ClientOrgID == co.ClientOrgID).ToArray();
                    cobtlog = cobtLogs.FirstOrDefault(x => x.ClientOrgID == co.ClientOrgID);
                }

                if (cobtlog != null)
                    result = session.Require<BillingType>(cobtlog.BillingTypeID);
                if (result == null)
                    result = BillingTypes.Regular;
            }

            return result;
        }

        public static IQueryable<ClientOrgBillingTypeLog> ActiveClientOrgBillingTypeLogQuery(this ISession session, DateTime sd, DateTime ed)
        {
            return session.Query<ClientOrgBillingTypeLog>().Where(x => x.EffDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd));
        }
    }
}
