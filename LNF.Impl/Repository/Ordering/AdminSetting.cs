using LNF.DataAccess;
using System.Xml.Linq;

namespace LNF.Impl.Repository.Ordering
{
    public class AdminSetting : IDataItem
    {
        public virtual int AdminSettingID { get; set; }
        public virtual string Name { get;set;}
        public virtual XElement Value { get; set; }
    }
}
