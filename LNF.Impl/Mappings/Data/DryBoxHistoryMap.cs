using FluentNHibernate.Mapping;
using LNF.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DryBoxHistoryMap : ClassMap<DryBoxHistory>
    {
        internal DryBoxHistoryMap()
        {
            Schema("sselData.dbo");
            Table("v_DryBoxHistory");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.DryBoxAssignmentID)
                .KeyProperty(x => x.DryBoxAssignmentLogID);
            Map(x => x.ClientAccountID);
            Map(x => x.DryBoxID);
            Map(x => x.DryBoxName);
            Map(x => x.ClientID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.OrgName);
            Map(x => x.ReservedDate);
            Map(x => x.ApprovedDate);
            Map(x => x.RemovedDate);
            Map(x => x.PendingApproval);
            Map(x => x.PendingRemoval);
            Map(x => x.Rejected);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
            Map(x => x.StatusChangedDate);
            Map(x => x.ClientActive);
            Map(x => x.ClientOrgActive);
            Map(x => x.ClientAccountActive);
        }
    }
}
