using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IEmailManager : IManager
    {
        void EmailOnCanceledByRepair(ReservationItem rsv, bool isRemoved, string state, string notes, DateTime repairEndDateTime, int clientId);
        void EmailOnCanceledByResource(ReservationItem rsv, int clientId);
        void EmailOnForgiveCharge(ReservationItem rsv, double forgiveAmount, bool sendToUser, int clientId);
        void EmailOnInvited(ReservationItem rsv, IEnumerable<ReservationInviteeItem> invitees, int clientId, ReservationModificationType modificationType = ReservationModificationType.Created);
        EmailOnOpenReservationsProcessResult EmailOnOpenReservations(ReservationItem rsv, DateTime startDate, DateTime endDate);
        void EmailOnOpenSlot(ReservationItem rsv, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int clientId);
        void EmailOnPracticeRes(ReservationItem rsv, string inviteeName, int clientId);
        void EmailOnSaveHistory(ReservationItem rsv, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser, int clientId);
        void EmailOnToolEngDelete(ReservationItem rsv, int toolEngClientId, int clientId);
        void EmailOnUninvited(ReservationItem rsv, IEnumerable<ReservationInviteeItem> invitees, int clientId);
        void EmailOnUserCreate(ReservationItem rsv, int clientId);
        void EmailOnUserDelete(ReservationItem rsv, int clientId);
        void EmailOnUserUpdate(ReservationItem rsv, int clientId);
    }
}