using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    internal class GlobalSettingsMap : ClassMap<Repository.Data.GlobalSettings>
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
