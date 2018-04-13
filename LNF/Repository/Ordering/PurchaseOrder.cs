using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Ordering
{
    public class PurchaseOrder : IDataItem
    {
        public PurchaseOrder()
        {
            Details = new List<PurchaseOrderDetail>();
        }

        public virtual int POID { get; set; }
        public virtual Client Client { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual int? AccountID { get; set; }
        public virtual Client Approver { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime NeededDate { get; set; }
        public virtual bool Oversized { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }
        public virtual string Notes { get; set; }
        public virtual Status Status { get; set; }
        public virtual DateTime? CompletedDate { get; set; }
        public virtual int? RealApproverID { get; set; }
        public virtual DateTime? ApprovalDate { get; set; }
        public virtual bool Attention { get; set; }
        public virtual int? PurchaserID { get; set; }
        public virtual string RealPO { get; set; }
        public virtual string ReqNum { get; set; }
        public virtual string PurchaserNotes { get; set; }
        public virtual IList<PurchaseOrderDetail> Details { get; set; }
    }
}
