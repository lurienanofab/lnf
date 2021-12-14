using LNF.DataAccess;
using System;

namespace LNF.Data
{
    public class DryBoxAssignment : IDryBoxAssignment, IDataItem
    {
        public virtual int DryBoxAssignmentID { get; set; }
        public virtual int DryBoxID { get; set; }
        public virtual int ClientAccountID { get; set; }
        public virtual DateTime ReservedDate { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }
        public virtual DateTime? RemovedDate { get; set; }
        public virtual bool PendingApproval { get; set; }
        public virtual bool PendingRemoval { get; set; }
        public virtual bool Rejected { get; set; }

        //public virtual DryBoxAssignmentStatus GetStatus()
        //{
        //    if (DryBoxAssignmentID == 0)
        //        return DryBoxAssignmentStatus.Available;

        //    if (RemovedDate == null)
        //    {
        //        if (ApprovedDate == null || PendingApproval)   // not approved
        //            return DryBoxAssignmentStatus.PendingApproval;
        //        else if (Rejected)
        //            return DryBoxAssignmentStatus.Rejected;

        //        return DryBoxAssignmentStatus.Active;
        //    }

        //    return DryBoxAssignmentStatus.Removed;
        //}
    }
}
