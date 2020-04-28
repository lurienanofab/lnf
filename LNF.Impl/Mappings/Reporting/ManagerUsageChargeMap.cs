using FluentNHibernate.Mapping;
using LNF.Billing;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ManagerUsageChargeMap : ClassMap<ManagerUsageCharge>
    {
        internal ManagerUsageChargeMap()
        {
            Schema("Reporting.dbo");
            Table("v_ManagerUsageCharges");
            ReadOnly();

            Id(x => x.ManagerUsageChargeID);
            Map(x => x.UsageChargeID);
            Map(x => x.Period);
            Map(x => x.BillingCategory).CustomType<GenericEnumMapper<BillingCategory>>();
            Map(x => x.UserClientID);
            Map(x => x.UserUserName);
            Map(x => x.UserLName);
            Map(x => x.UserFName);
            Map(x => x.UserEmail);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.AccountID);
            Map(x => x.ShortCode);
            Map(x => x.AccountNumber);
            Map(x => x.AccountName);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.ChargeTypeID);
            Map(x => x.TotalCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.IsRemote);
            Map(x => x.IsSubsidyOrg);
            Map(x => x.IsMiscCharge);
            Map(x => x.ManagerClientID);
            Map(x => x.ManagerUserName);
            Map(x => x.ManagerLName);
            Map(x => x.ManagerFName);
            Map(x => x.ManagerEnableDate);
            Map(x => x.ManagerDisableDate);
            Map(x => x.IsTechnicalManager);
            Map(x => x.IsFinancialManager);
        }
    }
}
