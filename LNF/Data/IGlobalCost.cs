using System;

namespace LNF.Data
{
    public interface IGlobalCost
    {
        int GlobalID { get; set; }
        int BusinessDay { get; set; }
        int LabAccountID { get; set; }
        int LabCreditAccountID { get; set; }
        string LabCreditAccountNumber { get; set; }
        string LabCreditAccountShortCode { get; set; }
        int SubsidyCreditAccountID { get; set; }
        string SubsidyCreditAccountNumber { get; set; }
        string SubsidyCreditAccountShortCode { get; set; }
        int AdminID { get; set; }
        int AccessToOld { get; set; }
        DateTime EffDate { get; set; }
    }
}