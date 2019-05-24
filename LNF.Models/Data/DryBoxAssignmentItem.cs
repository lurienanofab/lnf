using System;

namespace LNF.Models.Data
{
    public class DryBoxAssignmentItem : IDryBoxAssignment
    {
        public int DryBoxAssignmentID { get; set; }
        public int DryBoxID { get; set; }
        public int ClientAccountID { get; set; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public bool PendingApproval { get; set; }
        public bool PendingRemoval { get; set; }
        public bool Rejected { get; set; }
    }
}
