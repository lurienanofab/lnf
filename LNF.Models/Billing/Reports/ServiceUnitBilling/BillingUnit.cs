namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public class BillingUnit
    {
        public int CardType { get; set; }
        public string ShortCode { get; set; }
        public string Account { get; set; }
        public string FundCode { get; set; }
        public string DeptID { get; set; }
        public string ProgramCode { get; set; }
        public string ClassName { get; set; }
        public string ProjectGrant { get; set; }
        public string InvoiceDate { get; set; }
        public string Uniqname { get; set; }
        public string DepartmentalReferenceNumber { get; set; }
        public string ItemDescription { get; set; }
        public double MerchandiseAmount { get; set; }
        public string CreditAccount { get; set; }
        public string QuantityVouchered { get; set; }
    }
}
