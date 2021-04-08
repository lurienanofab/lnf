using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IEmailRepository
    {
        void EmailOnCanceledByRepair(int reservationId, bool isRemoved, string state, string notes, DateTime repairEndDateTime, int clientId);
        void EmailOnCanceledByResource(int reservationId, int clientId);
        void EmailOnForgiveCharge(int reservationId, double forgiveAmount, bool sendToUser, int clientId);
        void EmailOnInvited(int reservationId, IEnumerable<Invitee> invitees, int clientId, ReservationModificationType modificationType = ReservationModificationType.Created);
        EmailOnOpenReservationsProcessResult EmailOnOpenReservations(int reservationId, DateTime startDate, DateTime endDate);
        void EmailOnOpenSlot(int reservationId, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int clientId);
        void EmailOnPracticeRes(int reservationId, string inviteeName, int clientId);
        void EmailOnSaveHistory(int reservationId, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser, int clientId);
        void EmailOnToolEngDelete(int reservationId, IClient toolEng, int clientId);
        void EmailOnUninvited(int reservationId, IEnumerable<Invitee> invitees, int clientId);
        void EmailOnUserCreate(int reservationId, int clientId);
        void EmailOnUserDelete(int reservationId, int clientId);
        void EmailOnUserUpdate(int reservationId, int clientId);
    }
}