using System;

namespace LNF.Data
{
    public class GlobalCostItem : IGlobalCost
    {
        public int GlobalID { get; set; }
        public int BusinessDay { get; set; }
        public int LabAccountID { get; set; }
        public int LabCreditAccountID { get; set; }
        public string LabCreditAccountNumber { get; set; }
        public string LabCreditAccountShortCode { get; set; }
        public int SubsidyCreditAccountID { get; set; }
        public string SubsidyCreditAccountNumber { get; set; }
        public string SubsidyCreditAccountShortCode { get; set; }
        public int AdminID { get; set; }
        public int AccessToOld { get; set; }
        public DateTime EffDate { get; set; }
    }
}
