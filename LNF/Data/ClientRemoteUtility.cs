using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class ClientRemoteUtility
    {
        public static IEnumerable<ClientRemote> SelectByPeriod(DateTime period)
        {
            DateTime sd = period;
            DateTime ed = period.AddMonths(1);
            return ActiveLogUtility.FindActive<ClientRemote>(x => x.ClientRemoteID, sd, ed).ToArray();
        }

        public static ClientRemote Create(int clientId, int remoteClientId, int accountId, DateTime period, out bool success)
        {
            DateTime sd = period;
            DateTime ed = period.AddMonths(1);

            Client client = DA.Current.Single<Client>(clientId);
            Client remoteClient = DA.Current.Single<Client>(remoteClientId);
            Account acct = DA.Current.Single<Account>(accountId);

            //check for an existing active ClientRemote record
            var existing = ActiveLogUtility.FindActive<ClientRemote>(x => x.ClientRemoteID, sd, ed)
                .FirstOrDefault(x => x.Client == client && x.RemoteClient == remoteClient && x.Account == acct);

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

            cr.Enable();

            BillingType bt = BillingTypeUtility.GetBillingType(cr.Client, cr.Account, period);
            ToolBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);
            RoomBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);

            success = true;

            return cr;
        }

        public static void Delete(int clientRemoteId, DateTime period)
        {
            ClientRemote cr = DA.Current.Single<ClientRemote>(clientRemoteId);
            var alogs = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientRemote" && x.Record == clientRemoteId);
            DA.Current.Delete(new[] { cr });
            DA.Current.Delete(alogs);
            BillingType bt = BillingTypeUtility.GetBillingType(cr.Client, cr.Account, period);
            ToolBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);
            RoomBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);
        }
    }
}
