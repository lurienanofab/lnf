using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ClientRemoteManager : ManagerBase, IClientRemoteManager
    {
        public ClientRemoteManager(ISession session) : base(session) { }

        public void Enable(ClientRemote item, DateTime period)
        {
            ActiveLog alog = null;

            IList<ActiveLog> alogs = Session.Query<ActiveLog>().Where(x => x.TableName == item.TableName() && x.Record == item.Record()).ToList();

            if (alogs.Count > 0)
            {
                alog = alogs[0];

                // clean up invalid records, if any
                if (alogs.Count > 1)
                    Session.Delete(alogs.Skip(1).ToArray());
            }

            if (alog != null)
            {
                alog.EnableDate = period;
                alog.DisableDate = period.AddMonths(1);
            }
            else
            {
                alog = new ActiveLog()
                {
                    TableName = item.TableName(),
                    Record = item.Record(),
                    EnableDate = period,
                    DisableDate = period.AddMonths(1)
                };
            }

            Session.SaveOrUpdate(alog);
        }

        public IEnumerable<ClientRemote> SelectByPeriod(DateTime period)
        {
            DateTime sd = period;
            DateTime ed = period.AddMonths(1);
            var query = Session.Query<ClientRemote>();
            return Session.ActiveDataItemManager().FindActive(query, x => x.ClientRemoteID, sd, ed);
        }

        public ClientRemote Create(int clientId, int remoteClientId, int accountId, DateTime period, out bool success)
        {
            DateTime sd = period;
            DateTime ed = period.AddMonths(1);

            Client client = Session.Single<Client>(clientId);
            Client remoteClient = Session.Single<Client>(remoteClientId);
            Account acct = Session.Single<Account>(accountId);

            //check for an existing active ClientRemote record
            var query = Session.Query<ClientRemote>().Where(x => x.Client == client && x.RemoteClient == remoteClient && x.Account == acct);
            var existing = Session.ActiveDataItemManager().FindActive(query, x => x.ClientRemoteID, sd, ed).FirstOrDefault();

            if (existing != null)
            {
                success = false;
                return existing;
            }

            ClientRemote cr = new ClientRemote()
            {
                Client = client,
                RemoteClient = remoteClient,
                Account = acct,
            };

            Session.ActiveDataItemManager().Enable(cr);

            BillingType bt = Session.BillingTypeManager().GetBillingType(cr.Client, cr.Account, period);
            Session.ToolBillingManager().UpdateBillingType(cr.Client.ClientID, cr.Account.AccountID, bt.BillingTypeID, period);
            RoomBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);

            success = true;

            return cr;
        }

        public void Delete(int clientRemoteId, DateTime period)
        {
            ClientRemote cr = Session.Single<ClientRemote>(clientRemoteId);
            var alogs = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientRemote" && x.Record == clientRemoteId);
            Session.Delete(new[] { cr });
            Session.Delete(alogs);
            BillingType bt = Session.BillingTypeManager().GetBillingType(cr.Client, cr.Account, period);
            Session.ToolBillingManager().UpdateBillingType(cr.Client.ClientID, cr.Account.AccountID, bt.BillingTypeID, period);
            RoomBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);
        }
    }
}
