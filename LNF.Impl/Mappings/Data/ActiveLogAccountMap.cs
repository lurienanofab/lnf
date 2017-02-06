using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ActiveLogAccountMap : AccountInfoBaseMap<ActiveLogAccount>
    {
        internal ActiveLogAccountMap()
        {
            Id(x => x.LogID);
            Map(x => x.Record);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }

        protected override void MapAccountID()
        {
            Map(x => x.AccountID);
        }

        protected override void SetTable()
        {
            Table("v_ActiveLogAccount");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
