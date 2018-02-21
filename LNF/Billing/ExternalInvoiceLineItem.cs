using LNF.Models.Data;
using System;

namespace LNF.Billing
{
    public class ExternalInvoiceLineItem
    {
        public int AccountID { get; set; }
        public int OrgID { get; set; }
        public int ClientID { get; set; }
        public int ChargeTypeID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string OrgName { get; set; }
        public string AccountName { get; set; }
        public DateTime? PoEndDate { get; set; }
        public decimal PoRemainingFunds { get; set; }
        public string InvoiceNumber { get; set; }
        public string DeptRef { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }

        public string DisplayName
        {
            get { return ClientItem.GetDisplayName(LName, FName); }
        }

        public double LineTotal
        {
            get { return Quantity * Cost; }
        }
    }
}
