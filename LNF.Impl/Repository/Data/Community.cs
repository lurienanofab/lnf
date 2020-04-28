using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class Community : IDataItem
    {
        public virtual int CommunityID { get; set; }
        public virtual int CommunityFlag { get; set; }
        public virtual string CommunityName { get; set; }
    }
}
