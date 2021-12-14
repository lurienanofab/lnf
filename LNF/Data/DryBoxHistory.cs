using LNF.DataAccess;
using System;

namespace LNF.Data
{
    public class DryBoxHistory : IDryBoxAssignment, IDataItem
    {
        public virtual int DryBoxAssignmentID { get; set; }
        public virtual int DryBoxID { get; set; }
        public virtual string DryBoxName { get; set; }
        public virtual int ClientAccountID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string OrgName { get; set; }
        public virtual DateTime ReservedDate { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }
        public virtual DateTime? RemovedDate { get; set; }
        public virtual bool PendingApproval { get; set; }
        public virtual bool PendingRemoval { get; set; }
        public virtual bool Rejected { get; set; }
        public virtual int DryBoxAssignmentLogID { get; set; }
        public virtual DateTime? EnableDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
        public virtual DateTime StatusChangedDate { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual bool ClientOrgActive { get; set; }
        public virtual bool ClientAccountActive { get; set; }

        public virtual bool Cancelled
        {
            get
            {
                var cancelled =
                    !ApprovedDate.HasValue    //NULL
                    && RemovedDate.HasValue   //NOT NULL
                    && !EnableDate.HasValue   //NULL
                    && !DisableDate.HasValue; //NULL

                return cancelled;
            }
        }

        public override bool Equals(object obj)
        {
            var x = obj as DryBoxHistory;
            if (x == null) return false;
            return x.DryBoxAssignmentID == DryBoxAssignmentID && x.DryBoxAssignmentLogID == DryBoxAssignmentLogID;
        }

        public override int GetHashCode()
        {
            return new { DryBoxAssignmentID, DryBoxAssignmentLogID }.GetHashCode();
        }
    }
}
