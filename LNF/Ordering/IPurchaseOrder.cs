using System;

namespace LNF.Ordering
{
    public interface IPurchaseOrder
    {
        int POID { get; set; }
        int ClientID { get; set; }
        int VendorID { get; set; }
        int? AccountID { get; set; }
        int ApproverID { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime NeededDate { get; set; }
        bool Oversized { get; set; }
        int ShippingMethodID { get; set; }
        string Notes { get; set; }
        int StatusID { get; set; }
        DateTime? CompletedDate { get; set; }
        int? RealApproverID { get; set; }
        DateTime? ApprovalDate { get; set; }
        bool Attention { get; set; }
        int? PurchaserID { get; set; }
        string RealPO { get; set; }
        string ReqNum { get; set; }
        string PurchaserNotes { get; set; }
    }
}
