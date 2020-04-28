using System;

namespace LNF.Ordering
{
    public class PurchaseOrderSearchModel
    {
        public int POID { get; set; }
        public int PODID { get; set; }
        public int ItemID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public int ApproverID { get; set; }
        public string ApproverDisplayName { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string CleanVendorName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ItemCount { get; set; }
        public double TotalPrice { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
    }
}
