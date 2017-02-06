using System;

namespace LNF.Models.Data
{
    public class GlobalCostModel
    {
        public int GlobalID { get; set; }
        public int BusinessDay { get; set; }
        public int LabAccountID { get; set; }
        public string LabAccountName { get; set; }
        public string LabAccountShortCode { get; set; }
        public int LabCreditAccountID { get; set; }
        public string LabCreditAccountName { get; set; }
        public string LabCreditAccountShortCode { get; set; }
        public int SubsidyCreditAccountID { get; set; }
        public string SubsidyCreditAccountName { get; set; }
        public string SubsidyCreditAccountShortCode { get; set; }
        public int AdminID { get; set; }
        public string AdminDisplayName { get; set; }
        public int AccessToOld { get; set; }
        public DateTime EffDate { get; set; }
    }
}
