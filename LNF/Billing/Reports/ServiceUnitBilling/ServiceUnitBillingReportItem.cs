using System;

namespace LNF.Billing.Reports.ServiceUnitBilling
{
    public class ServiceUnitBillingReportItem
    {
        public string ReportType { get; set; }
        public string ChargeType { get; set; }
        public DateTime Period { get; set; }
        public string CardType { get; set; }
        public string ShortCode { get; set; }
        public string Account { get; set; }
        public string FundCode { get; set; }
        public string DeptID { get; set; }
        public string ProgramCode { get; set; }
        public string Class { get; set; }
        public string ProjectGrant { get; set; }
        public string VendorID { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceID { get; set; }
        public string Uniqname { get; set; }
        public string LocationCode { get; set; }
        public string DeliverTo { get; set; }
        public string VendorOrderNum { get; set; }
        public string DepartmentalReferenceNumber { get; set; }
        public string TripOrEventNumber { get; set; }
        public string ItemID { get; set; }
        public string ItemDescription { get; set; }
        public string VendorItemID { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelNum { get; set; }
        public string SerialNum { get; set; }
        public string UMTagNum { get; set; }
        public string QuantityVouchered { get; set; }
        public string UnitOfMeasure { get; set; }
        public string UnitPrice { get; set; }
        public string MerchandiseAmount { get; set; }
        public string VoucherComment { get; set; }
        public string SubsidyDiscount { get; set; }
        public string BilledCharge { get; set; }
        public string UsageCharge { get; set; }
        public string CreditAccount { get; set; }
        public string AccountID { get; set; }
    }
}
