using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class EmailRepository : ApiClient, IEmailRepository
    {
        public void EmailOnCanceledByRepair(int reservationId, bool isRemoved, string state, string notes, DateTime repairEndDateTime, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnCanceledByResource(int reservationId, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnForgiveCharge(int reservationId, double forgiveAmount, bool sendToUser, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnInvited(int reservationId, IEnumerable<Invitee> invitees, int clientId, ReservationModificationType modificationType = ReservationModificationType.Created)
        {
            throw new NotImplementedException();
        }

        public EmailOnOpenReservationsProcessResult EmailOnOpenReservations(int reservationId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public void EmailOnOpenSlot(int reservationId, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnPracticeRes(int reservationId, string inviteeName, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnSaveHistory(int reservationId, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnToolEngDelete(int reservationId, IClient toolEng, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUninvited(int reservationId, IEnumerable<Invitee> invitees, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUserCreate(int reservationId, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUserDelete(int reservationId, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUserUpdate(int reservationId, int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
