using LNF.Models.Ordering;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LNF.Ordering
{
    public static class ApproverUtility
    {
        public static bool IsApprover(int clientId)
        {
            int count = DA.Current.Query<Approver>().Count(x => x.ApproverID == clientId && x.Active);
            return count > 0;
        }

        public static bool InsertApprover(int clientId, int approverId, bool isPrimary)
        {
            Approver appr = new Approver { ClientID = clientId, ApproverID = approverId };
            Approver existing = DA.Current.Single<Approver>(appr);
            if (existing == null)
                return InsertApprover(appr, isPrimary);
            else
                return UpdateApprover(existing, isPrimary);
        }

        public static bool InsertApprover(Approver appr, bool isPrimary)
        {
            appr.Active = true;
            DA.Current.Insert(appr);
            SetPrimary(appr, isPrimary);
            return true;
        }
        
        public static bool UpdateApprover(int clientId, int approverId, bool isPrimary)
        {
            var appr = DA.Current.Single<Approver>(new Approver() { ClientID = clientId, ApproverID = approverId });
            return UpdateApprover(appr, isPrimary);
        }

        public static bool UpdateApprover(Approver appr, bool isPrimary)
        {
            if (appr == null) return false;
            SetPrimary(appr, isPrimary);
            return true;
        }

        public static bool DeleteApprover(int clientId, int approverId)
        {
            Approver key = new Approver { ClientID = clientId, ApproverID = approverId };
            Approver appr = DA.Current.Single<Approver>(key);
            if (appr == null) return false;
            appr.Active = false;
            SetPrimary(appr, false);
            return true;
        }

        public static void SetPrimary(Approver appr, bool isPrimary)
        {
            IQueryable<Approver> query = DA.Current.Query<Approver>().Where(x => x.ClientID == appr.ClientID);

            if (isPrimary)
            {
                foreach (Approver item in query)
                {
                    if (item.ApproverID == appr.ApproverID)
                        item.IsPrimary = true;
                    else
                        item.IsPrimary = false;
                }
            }
            else
            {
                appr.IsPrimary = false;
            }
        }

        public static ApproverItem Select(int clientId, int approverId)
        {
            return DA.Current.Single<Approver>(new Approver() { ClientID = clientId, ApproverID = approverId }).Model<ApproverItem>();
        }

        public static IList<ApproverItem> SelectApprovers(int clientId)
        {
            return DA.Current.Query<Approver>().Where(x => x.ClientID == clientId && x.Active).Model<ApproverItem>();
        }
    }
}
