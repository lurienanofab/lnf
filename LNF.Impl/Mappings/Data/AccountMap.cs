using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class AccountMap : ClassMap<Account>
    {
        internal AccountMap()
        {
            Schema("sselData.dbo");
            Id(x => x.AccountID);
            References(x => x.Org);
            Map(x => x.Name);
            References(x => x.AccountType);
            Map(x => x.Number);
            Map(x => x.ShortCode);
            Map(x => x.FundingSourceID);
            Map(x => x.TechnicalFieldID);
            Map(x => x.SpecialTopicID);
            Map(x => x.BillAddressID);
            Map(x => x.ShipAddressID);
            Map(x => x.InvoiceNumber);
            Map(x => x.InvoiceLine1);
            Map(x => x.InvoiceLine2);
            Map(x => x.PoEndDate);
            Map(x => x.PoInitialFunds);
            Map(x => x.PoRemainingFunds);
            Map(x => x.Active);
        }
    }
}
