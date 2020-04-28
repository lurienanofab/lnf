using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ActiveLogClientAccountMap : ClientAccountInfoBaseMap<ActiveLogClientAccount>
    {
        internal ActiveLogClientAccountMap()
        {
            Id(x => x.LogID);
            Map(x => x.Record);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }

        protected override void MapClientAccountID()
        {
            Map(x => x.ClientAccountID);
        }

        protected override void SetTable()
        {
            Table("v_ActiveLogClientAccount");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
