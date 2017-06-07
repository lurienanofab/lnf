using FluentNHibernate.Mapping;
using LNF.Models.Billing;
using LNF.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ManagerUsageChargeMap : ClassMap<ManagerUsageCharge>
    {
        internal ManagerUsageChargeMap()
        {
            Schema("Reporting.dbo");
            Table("v_ManagerUsageCharges");
            ReadOnly();
            Id(x => x.UsageChargeID);
            Map(x => x.BillingCategory).CustomType<GenericEnumMapper<BillingCategory>>();
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.UserName);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.Email);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.AccountID);
            Map(x => x.ShortCode);
            Map(x => x.AccountNumber);
            Map(x => x.AccountName);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.TotalCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.IsRemote);
            Map(x => x.IsSubsidyOrg);
            Map(x => x.IsMiscCharge);
            Map(x => x.ManagerClientID);
            Map(x => x.ManagerLName);
            Map(x => x.ManagerFName);
            Map(x => x.ManagerEnableDate);
            Map(x => x.ManagerDisableDate);
            Map(x => x.IsManager);
            Map(x => x.IsFinManager);
        }
    }
}
