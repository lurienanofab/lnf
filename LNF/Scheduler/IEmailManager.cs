using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IEmailManager : IManager
    {
        void EmailOnCanceledByRepair(IReservation rsv, bool isRemoved, string state, string notes, DateTime repairEndDateTime, int clientId);
        void EmailOnCanceledByResource(IReservation rsv, int clientId);
        void EmailOnForgiveCharge(IReservation rsv, double forgiveAmount, bool sendToUser, int clientId);
        void EmailOnInvited(IReservation rsv, IEnumerable<IReservationInvitee> invitees, int clientId, ReservationModificationType modificationType = ReservationModificationType.Created);
        EmailOnOpenReservationsProcessResult EmailOnOpenReservations(IReservation rsv, DateTime startDate, DateTime endDate);
        void EmailOnOpenSlot(IReservation rsv, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int clientId);
        void EmailOnPracticeRes(IReservation rsv, string inviteeName, int clientId);
        void EmailOnSaveHistory(IReservation rsv, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser, int clientId);
        void EmailOnToolEngDelete(IReservation rsv, IClient toolEng, int clientId);
        void EmailOnUninvited(IReservation rsv, IEnumerable<IReservationInvitee> invitees, int clientId);
        void EmailOnUserCreate(IReservation rsv, int clientId);
        void EmailOnUserDelete(IReservation rsv, int clientId);
        void EmailOnUserUpdate(IReservation rsv, int clientId);
    }
}