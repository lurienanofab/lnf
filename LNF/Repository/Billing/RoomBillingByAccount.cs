using LNF.Repository.Data;
using System;

namespace LNF.Repository.Billing
{
    public class RoomBillingByAccount : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal RoomCharge { get; set; }
        public virtual decimal EntryCharge { get; set; }
        public virtual decimal SubsidyDiscount { get; set; }
        public virtual decimal TotalCharge { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var item = obj as RoomBillingByAccount;

            if (item == null) return false;

            return item.Period == Period
                && item.Client.ClientID == Client.ClientID
                && item.Account.AccountID == Account.AccountID;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}", Period.GetHashCode(), Client.ClientID, Account.AccountID).GetHashCode();
        }
    }
}
