using FluentNHibernate.Mapping;
using LNF.Models.Billing;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RegularExceptionMap : ClassMap<RegularException>
    {
        internal RegularExceptionMap()
        {
            Schema("Billing.dbo");
            Table("v_RegularExceptions");
            ReadOnly();

            CompositeId()
                .KeyProperty(x => x.BillingID)
                .KeyProperty(x => x.BillingCategory).CustomType<BillingCategory>()
                .KeyProperty(x => x.IsTemp);

            Map(x => x.Period).Not.Nullable();
            Map(x => x.ReservationID).Not.Nullable();
            Map(x => x.ClientID).Not.Nullable();
            Map(x => x.LName).Not.Nullable();
            Map(x => x.FName).Not.Nullable();
            Map(x => x.InviteeClientID).Not.Nullable();
            Map(x => x.InviteeLName).Not.Nullable();
            Map(x => x.InviteeFName).Not.Nullable();
            Map(x => x.ResourceID).Not.Nullable();
            Map(x => x.ResourceName).Not.Nullable();
            Map(x => x.AccountID).Not.Nullable();
            Map(x => x.AccountName).Not.Nullable();
            Map(x => x.ShortCode).Not.Nullable();
        }
    }
}
