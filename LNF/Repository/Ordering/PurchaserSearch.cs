using System;

namespace LNF.Repository.Ordering
{
    public class PurchaserSearch : IDataItem
    {
        public virtual int POID { get; set; }
        public virtual int StatusID { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int? PurchaserID { get; set; }
        public virtual string PurchaserDisplayName { get; set; }
        public virtual double Total { get; set; }
        public virtual string RealPO { get; set; }
    }
}
