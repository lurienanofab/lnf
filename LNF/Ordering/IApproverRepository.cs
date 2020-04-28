using System.Collections.Generic;

namespace LNF.Ordering
{
    public interface IApproverRepository
    {
        IEnumerable<IApprover> GetApprovers(int clientId);
        IEnumerable<IApprover> GetActiveApprovers(int clientId);
        bool IsApprover(int clientId);
        IApprover AddApprover(int clientId, int approverId, bool isPrimary);
        IApprover UpdateApprover(int clientId, int approverId, bool isPrimary);
        bool DeleteApprover(int clientId, int approverId);
        IApprover GetApprover(int clientId, int approverId);
    }
}
