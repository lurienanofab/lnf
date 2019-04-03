using System;

namespace LNF.Models.Data
{
    public interface IGlobalCost
    {
        int AccessToOld { get; set; }
        int AdminID { get; set; }
        int BusinessDay { get; set; }
        DateTime EffDate { get; set; }
        int GlobalID { get; set; }
        int LabAccountID { get; set; }
        int LabCreditAccountID { get; set; }
        int SubsidyCreditAccountID { get; set; }
    }
}