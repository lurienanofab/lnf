using System;

namespace LNF.Repository.Ordering
{
    public class PurchaseOrderInfo : IDataItem
    {
        public virtual int POID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int VendorID { get; set; }
        public virtual string VendorName { get; set; }
        public virtual int? AccountID { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual int ApproverID { get; set; }
        public virtual string ApproverName { get; set; }
        public virtual DateTime? ApprovalDate { get; set; }
        public virtual bool Attention { get; set; }
        public virtual DateTime? CompletedDate { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime NeededDate { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Oversized { get; set; }
        public virtual int? PurchaserID { get; set; }
        public virtual string PurchaserNotes { get; set; }
        public virtual int? RealApproverID { get; set; }
        public virtual string RealPO { get; set; }
        public virtual string ReqNum { get; set; }
        public virtual int ShippingMethodID { get; set; }
        public virtual string ShippingMethodName { get; set; }
        public virtual int StatusID { get; set; }
        public virtual string StatusName { get; set; }
        public virtual double TotalPrice { get; set; }
    }
}
