using System;

namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public class JournalUnitReportItem
    {
        public string ReportType { get; set; }   
        public string ChargeType { get; set; }
        public string JournalUnitType { get; set; }
        public DateTime Period { get; set; }
        public string Account { get; set; }
        public string FundCode { get; set; }
        public string DeptID { get; set; }
        public string ProgramCode { get; set; }
        public string Class { get; set; }
        public string ProjectGrant { get; set; }
        public string DepartmentalReferenceNumber { get; set; }
        public string ItemDescription { get; set; }
        public string MerchandiseAmount { get; set; }
        public string CreditAccount { get; set; }
        public string AccountID { get; set; }
    }
}
