using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class SpecialTopic : IDataItem
    {
        public virtual int SpecialTopicID { get; set; }
        public virtual string SpecialTopicName { get; set; }
    }
}
