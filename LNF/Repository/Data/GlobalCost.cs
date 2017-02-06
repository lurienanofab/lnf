using System;

namespace LNF.Repository.Data
{
    public class GlobalCost : IDataItem
    {
        public virtual int GlobalID { get; set; }
        public virtual int BusinessDay { get; set; }
        public virtual Account LabAccount { get; set; }
        public virtual Account LabCreditAccount { get; set; }
        public virtual Account SubsidyCreditAccount { get; set; }
        public virtual Client Admin { get; set; }
        public virtual int AccessToOld { get; set; }
        public virtual DateTime EffDate { get; set; }
    }
}
