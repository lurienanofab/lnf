using LNF.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    public static class Extensions
    {
        public static MenuItem GetMenuItem(this Menu item)
        {
            if (item == null) return null;
            return item.CreateModel<MenuItem>();
        }

        public static GlobalSettingsItem CreateGlobalSettingsItem(this GlobalSettings item)
        {
            if (item == null) return null;
            var list = new List<GlobalSettings> { item };
            return CreateGlobalSettingsItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<GlobalSettingsItem> CreateGlobalSettingsItems(this IQueryable<GlobalSettings> query)
        {
            if (query == null) return null;

            return query.Select(x => new GlobalSettingsItem
            {
                SettingID = x.SettingID,
                SettingName = x.SettingName,
                SettingValue = x.SettingValue
            }).ToList();
        }
    }
}
