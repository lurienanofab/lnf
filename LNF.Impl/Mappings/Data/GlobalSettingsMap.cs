using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class GlobalSettingsMap : ClassMap<GlobalSettings>
    {
        public GlobalSettingsMap()
        {
            Schema("sselData.dbo");
            Id(x => x.SettingID);
            Map(x => x.SettingName);
            Map(x => x.SettingValue);
        }
    }
}
