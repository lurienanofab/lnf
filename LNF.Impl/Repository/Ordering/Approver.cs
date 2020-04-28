using LNF.DataAccess;

namespace LNF.Impl.Repository.Ordering
{
    public class Approver : IDataItem
    {
        public virtual int ApproverID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual bool IsPrimary { get; set; }
        public virtual bool Active { get; set; }

        public override bool Equals(object obj)
        {
            Approver item = obj as Approver;
            if (item == null) return false;
            return item.ApproverID == ApproverID && item.ClientID == ClientID;
        }

        public override int GetHashCode()
        {
            return (ApproverID.ToString() + "|" + ClientID.ToString()).GetHashCode();
        }
    }
}
