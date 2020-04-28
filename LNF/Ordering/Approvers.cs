using System.Collections.Generic;

namespace LNF.Ordering
{
    public static class Approvers
    {
        public static bool IsApprover(int clientId)
        {
            return ServiceProvider.Current.Ordering.Approver.IsApprover(clientId);    
        }

        public static IApprover InsertApprover(int clientId, int approverId, bool isPrimary)
        {
            return ServiceProvider.Current.Ordering.Approver.AddApprover(clientId, approverId, isPrimary);
        }

        public static IApprover UpdateApprover(int clientId, int approverId, bool isPrimary)
        {
            return ServiceProvider.Current.Ordering.Approver.UpdateApprover(clientId, approverId, isPrimary);
        }

        public static bool DeleteApprover(int clientId, int approverId)
        {
            return ServiceProvider.Current.Ordering.Approver.DeleteApprover(clientId, approverId);
        }

        public static IApprover Select(int clientId, int approverId)
        {
            return ServiceProvider.Current.Ordering.Approver.GetApprover(clientId, approverId);
        }

        public static IEnumerable<IApprover> SelectApprovers(int clientId)
        {
            return ServiceProvider.Current.Ordering.Approver.GetActiveApprovers(clientId);
        }
    }
}
