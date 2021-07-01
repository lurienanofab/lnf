using FluentNHibernate.Mapping;
using LNF.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationItemMap : ClassMap<ReservationItem>
    {
        internal ReservationItemMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationItem");
            ReadOnly();
            Id(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.ReservFence);
            Map(x => x.Granularity);
            Map(x => x.Offset);
            Map(x => x.MinReservTime);
            Map(x => x.MaxReservTime);
            Map(x => x.MinCancelTime);
            Map(x => x.GracePeriod);
            Map(x => x.ResourceAutoEnd);
            Map(x => x.AuthDuration);
            Map(x => x.AuthState);
            Map(x => x.ProcessTechID);
            Map(x => x.ProcessTechName);
            Map(x => x.LabID);
            Map(x => x.BuildingID);
            Map(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.Editable);
            Map(x => x.ActivityAccountType).CustomType<ActivityAccountType>();
            Map(x => x.StartEndAuth).CustomType<ClientAuthLevel>();
            Map(x => x.IsFacilityDownTime);
            Map(x => x.ClientID);
            Map(x => x.UserName);
            Map(x => x.Privs).CustomType<ClientPrivilege>();
            Map(x => x.LName);
            Map(x => x.MName);
            Map(x => x.FName);
            Map(x => x.Email);
            Map(x => x.Phone);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.ChargeTypeID);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.ClientIDBegin);
            Map(x => x.ClientIDEnd);
            Map(x => x.CreatedOn);
            Map(x => x.LastModifiedOn);
            Map(x => x.Duration);
            Map(x => x.Notes);
            Map(x => x.ChargeMultiplier);
            Map(x => x.ApplyLateChargePenalty);
            Map(x => x.ReservationAutoEnd);
            Map(x => x.HasProcessInfo);
            Map(x => x.HasInvitees);
            Map(x => x.IsActive);
            Map(x => x.IsStarted);
            Map(x => x.RecurrenceID);
            Map(x => x.GroupID);
            Map(x => x.CancelledDateTime);
            Map(x => x.KeepAlive);
        }
    }
}
