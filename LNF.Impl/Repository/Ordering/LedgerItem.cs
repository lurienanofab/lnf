using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Ordering
{
    public class LedgerItem : IDataItem
    {
        public virtual int LedgerItemID { get; set; }
        public virtual DateTime ApprovalDate { get; set; }
        public virtual string VendorName { get; set; }
        public virtual string ItemDescription { get; set; }
        public virtual string RealPO { get; set; }
        public virtual double Encumbrance { get; set; }
        public virtual double? Actual { get; set; }
        public virtual DateTime? MonthPaid { get; set; }
        public virtual string Category { get; set; }
        public virtual string LabCodeNumber { get; set; }
        public virtual string LabCodeName { get; set; }
        public virtual string MainLabCodeNumber { get; set; }
        public virtual string MainLabCodeName { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual string CategorySupervisor { get; set; }
        public virtual string OrderedBy { get; set; }
        public virtual string Notes { get; set; }
    }
}
