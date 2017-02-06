using System.Collections.Generic;
using System.Text;
using System;


namespace LNF.Repository.Data
{
    public class GlobalSettings : IDataItem
    {
        public virtual int SettingID { get; set; }
        public virtual string SettingName { get; set; }
        public virtual string SettingValue { get; set; }
    }
}
