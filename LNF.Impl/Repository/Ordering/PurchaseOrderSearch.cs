using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Ordering
{
    public class PurchaseOrderSearch : IDataItem
    {
        public virtual int POID { get; set; }
        public virtual int PODID { get; set; }
        public virtual int ItemID { get; set; }
        public virtual int StatusID { get; set; }
        public virtual string StatusName { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string CreatedDateText { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int ApproverID { get; set; }
        public virtual string ApproverDisplayName { get; set; }
        public virtual int VendorID { get; set; }
        public virtual string VendorName { get; set; }
        public virtual string CleanVendorName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual string CategoryNumber { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual int ParentCategoryID { get; set; }
        public virtual string ParentCategoryName { get; set; }
        public virtual int ItemCount { get; set; }
        public virtual double TotalPrice { get; set; }
        public virtual string TotalPriceText { get; set; }
        public virtual string PartNum { get; set; }
        public virtual string Description { get; set; }
    }
}
