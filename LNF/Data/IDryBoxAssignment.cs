using System;

namespace LNF.Data
{
    public interface IDryBoxAssignment
    {
        int DryBoxAssignmentID { get; set; }
        int DryBoxID { get; set; }
        int ClientAccountID { get; set; }
        DateTime ReservedDate { get; set; }
        DateTime? ApprovedDate { get; set; }
        DateTime? RemovedDate { get; set; }
        bool PendingApproval { get; set; }
        bool PendingRemoval { get; set; }
        bool Rejected { get; set; }
    }
}
