using LNF.Data;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class GlobalSettings : IGlobalSetting, IDataItem
    {
        public virtual int SettingID { get; set; }
        public virtual string SettingName { get; set; }
        public virtual string SettingValue { get; set; }
    }
}
