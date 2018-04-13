using System.Collections.Generic;

namespace LNF.Repository.Ordering
{
    public class Vendor : IDataItem
    {
        public Vendor()
        {
            Items = new List<PurchaseOrderItem>();
        }

        public virtual int VendorID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string VendorName { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Contact { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string URL { get; set; }
        public virtual string Email { get; set; }
        public virtual bool Active { get; set; }
        public virtual IList<PurchaseOrderItem> Items { get; set; }

        public virtual bool IsStoreManager()
        {
            return ClientID == 0;
        }
    }
}
