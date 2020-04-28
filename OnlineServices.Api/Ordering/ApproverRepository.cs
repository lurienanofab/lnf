using LNF.Ordering;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Ordering
{
    public class ApproverRepository : ApiClient, IApproverRepository
    {
        public IApprover AddApprover(int clientId, int approverId, bool isPrimary)
        {
            throw new NotImplementedException();
        }

        public bool DeleteApprover(int clientId, int approverId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApprover> GetActiveApprovers(int clientId)
        {
            throw new NotImplementedException();
        }

        public IApprover GetApprover(int clientId, int approverId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApprover> GetApprovers(int clientId)
        {
            throw new NotImplementedException();
        }

        public bool IsApprover(int clientId)
        {
            throw new NotImplementedException();
        }

        public IApprover UpdateApprover(int clientId, int approverId, bool isPrimary)
        {
            throw new NotImplementedException();
        }
    }
}
