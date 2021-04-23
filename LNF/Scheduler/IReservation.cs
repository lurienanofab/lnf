using LNF.Data;
using System;

namespace LNF.Scheduler
{
    // Based on v_ReservationInfo

    public interface IReservation : IReservationItem, IResource, IPrivileged, IClientOrg
    {
        bool IsUnloaded { get; set; }
        double MaxReservedDuration { get; set; }
        string ClientBeginLName { get; set; }
        string ClientBeginFName { get; set; }
        string ClientEndLName { get; set; }
        string ClientEndFName { get; set; }
        DateTime? OriginalBeginDateTime { get; set; }
        DateTime? OriginalEndDateTime { get; set; }
        DateTime? OriginalModifiedOn { get; set; }
    }

    public interface IReservationWithInvitees : IReservation, IReservationWithInviteesItem { }
}
