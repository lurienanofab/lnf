using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public static class ClientOrgBillingTypeLogUtility
    {
        public static void UpdateClientOrgBillingType(ClientOrg co, BillingType billingType)
        {
            IList<ClientOrgBillingTypeLog> query = DA.Current.Query<ClientOrgBillingTypeLog>().Where(x => x.ClientOrg == co && x.DisableDate == null).ToList();

            ClientOrgBillingTypeLog ts;

            if (query.Count > 0)
            {
                //disabled the existing entry
                ts = query[0];
                ts.DisableDate = DateTime.Now.Date;
            }

            //create an entry for the new billing type
            ts = new ClientOrgBillingTypeLog();
            ts.ClientOrg = co;
            ts.BillingType = billingType;
            ts.EffDate = DateTime.Now;
            ts.DisableDate = null;
            DA.Current.Insert(ts);
        }

        //public static bool InRange(IEnumerable<ClientOrgBillingTypeLog> logs, DateTime startDate, DateTime endDate)
        //{
        //    IList<ClientOrgBillingTypeLog> query = logs.Where(x => x.EffDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate)).ToList();
        //    bool result = query.Count > 0;
        //    return result;
        //}

        public static IQueryable<ClientOrgBillingTypeLog> GetActive(DateTime startDate, DateTime endDate)
        {
            return DA.Current.Query<ClientOrgBillingTypeLog>().Where(x => x.EffDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate));
        }

        //public static IList<ClientOrgBillingTypeLog> GetActive(IEnumerable<ClientOrg> list, DateTime startDate, DateTime endDate)
        //{
        //    IList<ClientOrgBillingTypeLog> query = DA.Current.Query<ClientOrgBillingTypeLog>().Where(x => list.Contains(x.ClientOrg) && x.EffDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate)).ToList();
        //    return query;
        //}

        //public static ClientOrgBillingTypeLog GetActive(ClientOrg co, DateTime startDate, DateTime endDate)
        //{
        //    ClientOrgBillingTypeLog result = DA.Current.Query<ClientOrgBillingTypeLog>().FirstOrDefault(x => x.ClientOrg == co && x.EffDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate));
        //    return result;
        //}

        //public static ClientOrgBillingTypeLog GetActive(ClientOrg co, IEnumerable<ClientOrgBillingTypeLog> logs)
        //{
        //    ClientOrgBillingTypeLog result = logs.Where(x => x.ClientOrg == co).FirstOrDefault();
        //    return result;
        //}
    }
}
