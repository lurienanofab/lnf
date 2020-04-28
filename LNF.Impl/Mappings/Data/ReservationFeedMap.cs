using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ReservationFeedMap : ClassMap<ReservationFeed>
    {
        internal ReservationFeedMap()
        {
            Schema("sselData.dbo");
            Table("v_ReservationFeed");
            ReadOnly();
            Id(x => x.ReservationID);
            Map(x => x.ClientID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.UserName);
            Map(x => x.Email);
            Map(x => x.Invitees);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.CreatedOn);
            Map(x => x.LastModifiedOn);
            Map(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.IsStarted);
            Map(x => x.IsActive);
            Map(x => x.AccountID);
            Map(x => x.ShortCode);
            Map(x => x.AccountName);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
        }
    }
}
