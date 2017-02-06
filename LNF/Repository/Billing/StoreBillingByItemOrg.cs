using LNF.Repository.Data;
using LNF.Repository.Inventory;
using System;

namespace LNF.Repository.Billing
{
    public class StoreBillingByItemOrg : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Org Org { get; set; }
        public virtual Item Item { get; set; }
        public virtual ChargeType ChargeType { get; set; }
        public virtual decimal TotalCharge { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var item = obj as StoreBillingByItemOrg;

            if (item == null) return false;

            return item.Period == Period
                && item.Client.ClientID == Client.ClientID
                && item.Org.OrgID == Org.OrgID
                && item.Item.ItemID == Item.ItemID;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}|{3}", Period.GetHashCode(), Client.ClientID, Org.OrgID, Item.ItemID).GetHashCode();
        }
    }
}
