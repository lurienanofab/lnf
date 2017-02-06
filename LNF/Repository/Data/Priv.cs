using LNF.Models.Data;

namespace LNF.Repository.Data
{
    public class Priv : IDataItem
    {
        public virtual ClientPrivilege PrivFlag { get; set; }
        public virtual string PrivType { get; set; }
        public virtual string PrivDescription { get; set; }

        public static ClientPrivilege operator |(Priv p1, Priv p2)
        {
            return p1.PrivFlag | p2.PrivFlag;
        }

        public static ClientPrivilege operator |(Priv p1, ClientPrivilege p2)
        {
            return p1.PrivFlag | p2;
        }

        public static ClientPrivilege operator &(Priv p1, Priv p2)
        {
            return p1.PrivFlag & p2.PrivFlag;
        }

        public static ClientPrivilege operator &(Priv p1, ClientPrivilege p2)
        {
            return p1.PrivFlag & p2;
        }
    }
}
