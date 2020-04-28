using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class AccountInfoMap : AccountInfoBaseMap<AccountInfo>
    {
        protected override void MapAccountID()
        {
            Id(x => x.AccountID);
        }

        protected override void SetTable()
        {
            Table("v_AccountInfo");
            ReadOnly();
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
