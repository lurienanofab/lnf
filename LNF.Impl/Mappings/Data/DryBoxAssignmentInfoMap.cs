using FluentNHibernate.Mapping;
using LNF.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DryBoxAssignmentInfoMap : ClassMap<DryBoxAssignmentInfo>
    {
        internal DryBoxAssignmentInfoMap()
        {
            Schema("sselData.dbo");
            Table("v_DryBoxAssignmentInfo");
            ReadOnly();
            Id(x => x.DryBoxAssignmentID);
            Map(x => x.ClientID);
            Map(x => x.DryBoxName);
            Map(x => x.Active);
            Map(x => x.Visible);
            Map(x => x.Deleted);
            Map(x => x.UserName);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.ShortCode);
            Map(x => x.AccountName);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.ClientOrgID);
            Map(x => x.Email);
            Map(x => x.ApprovedDate);
            Map(x => x.ClientAccountID);
            Map(x => x.DryBoxID);
            Map(x => x.PendingApproval);
            Map(x => x.PendingRemoval);
            Map(x => x.Rejected);
            Map(x => x.RemovedDate);
            Map(x => x.ReservedDate);
            Map(x => x.ClientActive);
            Map(x => x.ClientOrgActive);
            Map(x => x.ClientAccountActive);
        }
    }
}
