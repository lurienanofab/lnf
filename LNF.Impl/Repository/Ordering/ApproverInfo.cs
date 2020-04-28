using LNF.DataAccess;
using System.Linq;

namespace LNF.Impl.Repository.Ordering
{
    public class ApproverInfo : IDataItem
    {
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Email { get; set; }
        public virtual int ApproverID { get; set; }
        public virtual string ApproverDisplayName { get; set; }
        public virtual string ApproverEmail { get; set; }
        public virtual bool IsPrimary { get; set; }
        public virtual bool Active { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ApproverInfo item)) return false;
            return item.ApproverID == ApproverID && item.ClientID == ClientID;
        }

        public override int GetHashCode()
        {
            return new { ApproverID, ClientID }.GetHashCode();
        }

        public static IQueryable<ApproverInfo> SelectActiveByClient(NHibernate.ISession session, int clientId)
        {
            return session.Query<ApproverInfo>().Where(x => x.ClientID == clientId && x.Active);
        }
    }
}
