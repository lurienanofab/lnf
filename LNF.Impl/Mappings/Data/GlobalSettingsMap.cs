using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class GlobalSettingsMap : ClassMap<GlobalSettings>
    {
        internal GlobalSettingsMap()
        {
            Schema("sselData.dbo");
            Id(x => x.SettingID);
            Map(x => x.SettingName);
            Map(x => x.SettingValue);
        }
    }
}
