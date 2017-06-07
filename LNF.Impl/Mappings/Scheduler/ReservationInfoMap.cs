﻿using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInfoMap : ClassMap<ReservationInfo>
    {
        internal ReservationInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationInfo");
            ReadOnly();
            Id(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.GracePeriod);
            Map(x => x.ProcessTechID);
            Map(x => x.ProcessTechName);
            Map(x => x.ClientID);
            Map(x => x.UserName);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.AccountNumber);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.NNINOrg);
            Map(x => x.PrimaryOrg);
            Map(x => x.OrgTypeID);
            Map(x => x.OrgTypeName);
            Map(x => x.ChargeTypeID);
            Map(x => x.ChargeTypeName);
            Map(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.ChargeBeginDateTime);
            Map(x => x.ChargeEndDateTime);
            Map(x => x.ClientIDBegin);
            Map(x => x.ClientBeginLName);
            Map(x => x.ClientBeginFName);
            Map(x => x.ClientIDEnd);
            Map(x => x.ClientEndLName);
            Map(x => x.ClientEndFName);
            Map(x => x.CreatedOn);
            Map(x => x.LastModifiedOn);
            Map(x => x.Duration);
            Map(x => x.Notes);
            Map(x => x.ChargeMultiplier);
            Map(x => x.ApplyLateChargePenalty);
            Map(x => x.AutoEnd);
            Map(x => x.HasProcessInfo);
            Map(x => x.HasInvitees);
            Map(x => x.IsActive);
            Map(x => x.IsStarted);
            Map(x => x.IsUnloaded);
            Map(x => x.RecurrenceID);
            Map(x => x.GroupID);
            Map(x => x.MaxReservedDuration);
            Map(x => x.CancelledDateTime);
            Map(x => x.KeepAlive);
            Map(x => x.OriginalBeginDateTime);
            Map(x => x.OriginalEndDateTime);
            Map(x => x.OriginalModifiedOn);
        }
    }
}