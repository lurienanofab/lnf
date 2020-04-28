using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class Cost : IDataItem
    {
        public virtual int CostID { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual string TableNameOrDescription { get; set; }
        public virtual int? RecordID { get; set; }
        public virtual string AcctPer { get; set; }
        public virtual decimal AddVal { get; set; }
        public virtual decimal MulVal { get; set; }
        public virtual DateTime EffDate { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}
