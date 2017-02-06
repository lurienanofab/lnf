using System;
using System.Collections.Generic;
using System.Text;

namespace LNF.Repository.Data
{
    public enum DryBoxAssignmentStatus
    {
        PendingApproval = 0,
        Rejected = 1,
        Active = 2,
        Removed = 3
    }

    public class DryBoxAssignment : IDataItem
    {
        public virtual int DryBoxAssignmentID { get; set; }
        public virtual DryBox DryBox { get; set; }
        public virtual ClientAccount ClientAccount { get; set; }
        public virtual DateTime ReservedDate { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }
        public virtual DateTime? RemovedDate { get; set; }
        public virtual bool PendingApproval { get; set; }
        public virtual bool PendingRemoval { get; set; }
        public virtual bool Rejected { get; set; }

        public virtual DryBoxAssignmentStatus GetStatus()
        {
            if (RemovedDate == null)
            {
                if (ApprovedDate == null || PendingApproval)   // not approved
                    return DryBoxAssignmentStatus.PendingApproval;
                else if (Rejected)
                    return DryBoxAssignmentStatus.Rejected;

                return DryBoxAssignmentStatus.Active;
            }

            return DryBoxAssignmentStatus.Removed;
        }
    }
}
