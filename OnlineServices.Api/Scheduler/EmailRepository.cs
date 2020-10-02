using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class EmailRepository : ApiClient, IEmailRepository
    {
        public void EmailOnCanceledByRepair(IReservation rsv, bool isRemoved, string state, string notes, DateTime repairEndDateTime, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnCanceledByResource(IReservation rsv, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnForgiveCharge(IReservation rsv, double forgiveAmount, bool sendToUser, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnInvited(IReservation rsv, IEnumerable<Invitee> invitees, int clientId, ReservationModificationType modificationType = ReservationModificationType.Created)
        {
            throw new NotImplementedException();
        }

        public EmailOnOpenReservationsProcessResult EmailOnOpenReservations(IReservation rsv, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public void EmailOnOpenSlot(IReservation rsv, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnPracticeRes(IReservation rsv, string inviteeName, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnSaveHistory(IReservation rsv, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnToolEngDelete(IReservation rsv, IClient toolEng, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUninvited(IReservation rsv, IEnumerable<Invitee> invitees, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUserCreate(IReservation rsv, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUserDelete(IReservation rsv, int clientId)
        {
            throw new NotImplementedException();
        }

        public void EmailOnUserUpdate(IReservation rsv, int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
