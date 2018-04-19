using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler
{
    public interface IReservationInviteeManager : IManager
    {
        void Delete(int reservationId, int inviteeId);
        void Delete(ReservationInvitee key);
        bool Exists(int reservationId, int inviteeId);
        bool Exists(ReservationInvitee key);
        ReservationInvitee Find(int reservationId, int inviteeId);
        ReservationInvitee Find(ReservationInvitee key);
        void Insert(int reservationId, int inviteeId);
        IList<ReservationInvitee> ToReservationInviteeList(DataTable dt, int reservationId);
    }
}