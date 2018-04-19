using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IEmailManager : IManager
    {
        void EmailFromUser(string toAddr, string subject, string body, bool ccself = false, bool isHtml = false);
        void EmailOnCanceledByRepair(Reservation rsv, bool isRemoved, string state, string notes, DateTime repairEndDateTime);
        void EmailOnCanceledByResource(Reservation rsv);
        void EmailOnForgiveCharge(Reservation rsv, double forgiveAmount, bool sendToUser, int clientId);
        void EmailOnInvited(Reservation rsv, IEnumerable<ReservationInviteeItem> invitees, ReservationModificationType modificationType = ReservationModificationType.Created);
        int EmailOnOpenReservations(int resourceId, DateTime startDate, DateTime endDate);
        void EmailOnOpenSlot(int resourceId, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int reservationId);
        void EmailOnPracticeRes(Reservation rsv, string inviteeName);
        void EmailOnSaveHistory(Reservation rsv, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser);
        void EmailOnToolEngDelete(Reservation rsv, int toolEngClientId);
        void EmailOnUninvited(Reservation rsv, IEnumerable<ReservationInviteeItem> invitees);
        void EmailOnUserCreate(Reservation rsv);
        void EmailOnUserDelete(Reservation rsv);
        void EmailOnUserUpdate(Reservation rsv);
    }
}