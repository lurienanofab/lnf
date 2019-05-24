using System;

namespace LNF.Models.Data
{
    public interface IDryBoxAssignment
    {
        DateTime? ApprovedDate { get; set; }
        int ClientAccountID { get; set; }
        int DryBoxAssignmentID { get; set; }
        int DryBoxID { get; set; }
        bool PendingApproval { get; set; }
        bool PendingRemoval { get; set; }
        bool Rejected { get; set; }
        DateTime? RemovedDate { get; set; }
        DateTime ReservedDate { get; set; }
    }
}