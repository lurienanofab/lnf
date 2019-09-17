using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationRecurrenceInfoMap : ClassMap<ReservationRecurrenceInfo>
    {
        internal ReservationRecurrenceInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationRecurrenceInfo");
            ReadOnly();
            Id(x => x.RecurrenceID);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.AutoEnd);
            Map(x => x.KeepAlive);
            Map(x => x.BeginDate);
            Map(x => x.BeginTime);
            Map(x => x.EndDate);
            Map(x => x.EndTime);
            Map(x => x.CreatedOn);
            Map(x => x.Duration);
            Map(x => x.IsActive);
            Map(x => x.Notes);
            Map(x => x.ClientID);
            Map(x => x.FName);
            Map(x => x.LName);
            Map(x => x.PatternID);
            Map(x => x.PatternName);
            Map(x => x.PatternParam1);
            Map(x => x.PatternParam2);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.ProcessTechID);
            Map(x => x.LabID);
            Map(x => x.BuildingID);
        }
    }
}
