using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ReservationRecurrenceUtility
    {
        public static IList<ReservationRecurrence> SelectByProcessTech(int processTechId)
        {
            IList<Resource> resByProcTech = DA.Current.Single<ProcessTech>(processTechId).GetResources().ToList();
            int[] resIds = resByProcTech.Select(x => x.ResourceID).ToArray();

            IList<ReservationRecurrence> result = DA.Current.Query<ReservationRecurrence>().Where(x => x.IsActive && resIds.Contains(x.Resource.ResourceID)).ToList();

            return result;
        }

        public static IList<ReservationRecurrence> SelectByClient(int clientId)
        {
            //IList<ReservationRecurrence> result = DA.Current.Query<ReservationRecurrence>().Where(x => x.IsActive && x.Client == client && x.Resource.IsActive).ToList();
            IList<ReservationRecurrence> result = DA.Current.Query<ReservationRecurrence>().ToList();
            IList<ReservationRecurrence> result2 = result.Where(x => x.IsActive).ToList();
            IList<ReservationRecurrence> result3 = result2.Where(x => x.Client.ClientID == clientId && x.Resource.IsActive).ToList();
            return result3;
        }

        public static IList<ReservationRecurrence> SelectByResource(int resourceId)
        {
            IList<ReservationRecurrence> result = DA.Current.Query<ReservationRecurrence>().Where(x => x.IsActive && x.Resource.ResourceID == resourceId && x.Resource.IsActive).ToList();
            return result;
        }

        public static DateTime GetDate(DateTime period, int n, DayOfWeek dow)
        {
            DateTime edate = period.AddMonths(1);
            int days = (int)(edate - period).TotalDays;
            IList<DateTime> result = Enumerable.Range(0, days).Select(x => period.AddDays(x)).Where(d => d.DayOfWeek == dow).ToList();
            if (result.Count >= n)
                return result[n - 1];
            else
                return result.Last();
        }
    }
}
