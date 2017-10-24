using System;

namespace LNF.Billing
{
    public class ExternalInvoiceHeader
    {
        public int OrgAcctID { get; set; }
        public int OrgID { get; set; }
        public int AccountID { get; set; }
        public string OrgName { get; set; }
        public string AccountName { get; set; }
        public DateTime? PoEndDate { get; set; }
        public decimal PoRemainingFunds { get; set; }
        public string InvoiceNumber { get; set; }
        public string DeptRef { get; set; }
        public bool HasActivity { get; set; }
    }
}
