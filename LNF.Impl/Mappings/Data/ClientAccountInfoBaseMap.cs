using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal abstract class ClientAccountInfoBaseMap<T> : ClientOrgInfoBaseMap<T> where T : ClientAccountInfoBase
    {
        internal ClientAccountInfoBaseMap()
        {
            MapClientAccountID();
            Map(x => x.IsDefault);
            Map(x => x.Manager);
            Map(x => x.ClientAccountActive);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.AccountNumber);
            Map(x => x.ShortCode);
            Map(x => x.BillAddressID);
            Map(x => x.ShipAddressID);
            Map(x => x.InvoiceNumber);
            Map(x => x.InvoiceLine1);
            Map(x => x.InvoiceLine2);
            Map(x => x.PoEndDate);
            Map(x => x.PoInitialFunds);
            Map(x => x.PoRemainingFunds);
            Map(x => x.AccountActive);
            Map(x => x.FundingSourceID);
            Map(x => x.FundingSourceName);
            Map(x => x.TechnicalFieldID);
            Map(x => x.TechnicalFieldName);
            Map(x => x.SpecialTopicID);
            Map(x => x.SpecialTopicName);
            Map(x => x.AccountTypeID);
            Map(x => x.AccountTypeName);
        }

        protected override void MapClientID()
        {
            Map(x => x.ClientID);
        }

        protected override void MapClientOrgID()
        {
            Map(x => x.ClientOrgID);
        }

        protected abstract void MapClientAccountID();
    }
}
