using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Repository.Ordering
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
            ApproverInfo item = obj as ApproverInfo;
            if (item == null) return false;
            return item.ApproverID == ApproverID && item.ClientID == ClientID;
        }

        public override int GetHashCode()
        {
            return new { ApproverID, ClientID }.GetHashCode();
        }

        public static IQueryable<ApproverInfo> SelectActiveByClient(int clientId)
        {
            return DA.Current.Query<ApproverInfo>().Where(x => x.ClientID == clientId && x.Active);
        }
    }
}
