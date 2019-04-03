using System;

namespace LNF.Models.Data
{
    public class GlobalCostItem : IGlobalCost
    {
        // [2018-05-18 jg] I took out account name/shortcode and client displayname properties because these caused a lot of extra
        // database calls even when they aren't used. It's better to retrieve them when needed.

        public int GlobalID { get; set; }
        public int BusinessDay { get; set; }
        public int LabAccountID { get; set; }
        public int LabCreditAccountID { get; set; }
        public int SubsidyCreditAccountID { get; set; }
        public int AdminID { get; set; }
        public int AccessToOld { get; set; }
        public DateTime EffDate { get; set; }
    }
}
