﻿using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class MenuMap : ClassMap<Menu>
    {
        public MenuMap()
        {
            Schema("sselData.dbo");
            Id(x => x.MenuID);
            Map(x => x.MenuParentID).Not.Nullable();
            Map(x => x.MenuText).Not.Nullable();
            Map(x => x.MenuURL).Nullable();
            Map(x => x.MenuCssClass).Nullable();
            Map(x => x.MenuPriv).Not.Nullable();
            Map(x => x.NewWindow).Not.Nullable();
            Map(x => x.TopWindow).Not.Nullable();
            Map(x => x.IsLogout).Not.Nullable();
            Map(X => X.SortOrder).Not.Nullable();
            Map(x => x.Active).Not.Nullable();
            Map(x => x.Deleted).Not.Nullable();
        }
    }
}
