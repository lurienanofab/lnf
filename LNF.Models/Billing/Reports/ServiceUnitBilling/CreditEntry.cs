namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public class CreditEntry
    {
        public string Account { get; set; }
        public string FundCode { get; set; }
        public string DeptID { get; set; }
        public string ProgramCode { get; set; }
        public string ClassName { get; set; }
        public string ProjectGrant { get; set; }
        public string DepartmentalReferenceNumber { get; set; }    //manager's last name, first name
        public string ItemDescription { get; set; }                //user's last name, first name - billing type
        public double MerchandiseAmount { get; set; }
        public string CreditAccount { get; set; }
    }
}
