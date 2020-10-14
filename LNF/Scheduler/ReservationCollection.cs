using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationCollection : IEnumerable<IReservation>
    {
        private IList<IReservation> _items = new List<IReservation>();
        private IEnumerable<IReservationInviteeItem> _invitees = null;
        private IEnumerable<IReservationProcessInfo> _reservationProcessInfos = null;
        private int[] _invited = null;
        
        public IProvider Provider { get; }
        public int ClientID { get; }

        public ReservationCollection(IProvider provider, int clientId)
        {
            Provider = provider;
            ClientID = clientId;
        }

        public void Add(IReservation item)
        {
            _items.Add(item);
        }

        public int[] GetInvited()
        {
            if (_items == null)
                throw new Exception("No reservations have been selected yet.");

            if (_invited == null)
            {
                var ids = _items.Select(x => x.ReservationID).ToArray();
                _invited = Provider.Scheduler.Reservation.FilterInvitedReservations(ids, ClientID);
            }

            return _invited;
        }

        public IEnumerable<IReservationInviteeItem> GetInvitees()
        {
            if (_items == null)
                throw new Exception("No reservations have been selected yet.");

            if (_invitees == null)
                return new IReservationInviteeItem[0];
            
            return _invitees;
        }

        public IEnumerable<IReservationProcessInfo> GetReservationProcessInfos()
        {
            if (_items == null)
                throw new Exception("No reservations have been selected yet.");

            if (_reservationProcessInfos == null)
            {
                var ids = _items.Select(x => x.ReservationID).ToArray();
                _reservationProcessInfos = Provider.Scheduler.ProcessInfo.GetReservationProcessInfos(ids);
            }

            return _reservationProcessInfos;
        }

        public IEnumerable<IReservation> Find(DateTime sd, DateTime ed, bool includeAllClients, bool includeCancelled)
        {
            IList<IReservation> step1;
            IList<IReservation> step2;
            IList<IReservation> step3;

            step1 = _items.Where(x => (x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)).ToList();

            if (includeAllClients)
                step2 = step1.ToList();
            else
                step2 = step1.Where(x => x.ClientID == ClientID || GetInvited().Contains(x.ReservationID)).ToList();

            if (includeCancelled)
                step3 = step2.ToList();
            else
                step3 = step2.Where(x => x.IsActive).ToList();

            return step3;
        }

        public IEnumerable<IReservation> Find(DateTime d, bool includeAllClients, bool includeCancelled)
        {
            DateTime sd = d.Date;
            DateTime ed = sd.AddDays(1);

            return Find(sd, ed, includeAllClients, includeCancelled);
        }

        public void SelectByResource(int resourceId, DateTime sd, DateTime ed)
        {
            AssertDatesAreValid(sd, ed);
            _items = Provider.Scheduler.Reservation.SelectByResource(resourceId, sd, ed, true).ToList();
            _invitees = Provider.Scheduler.Reservation.SelectInviteesByResource(resourceId, sd, ed, true).ToList();
        }

        public void SelectByProcessTech(int processTechId, DateTime sd, DateTime ed)
        {
            AssertDatesAreValid(sd, ed);
            _items = Provider.Scheduler.Reservation.SelectByProcessTech(processTechId, sd, ed, true).ToList();
            _invitees = Provider.Scheduler.Reservation.SelectInviteesByProcessTech(processTechId, sd, ed, true).ToList();
        }

        public void SelectByLabLocation(int labLocationId, DateTime sd, DateTime ed)
        {
            AssertDatesAreValid(sd, ed);
            _items = Provider.Scheduler.Reservation.SelectByLabLocation(labLocationId, sd, ed, true).ToList();
            _invitees = Provider.Scheduler.Reservation.SelectInviteesByLabLocation(labLocationId, sd, ed, true).ToList();
        }

        public void SelectByClient(DateTime sd, DateTime ed)
        {
            AssertDatesAreValid(sd, ed);
            _items = Provider.Scheduler.Reservation.SelectByClient(ClientID, sd, ed, true).ToList();
            _invitees = Provider.Scheduler.Reservation.SelectInviteesByClient(ClientID, sd, ed, true).ToList();
        }

        public void SelectByDateRange(DateTime sd, DateTime ed)
        {
            AssertDatesAreValid(sd, ed);
            _items = Provider.Scheduler.Reservation.SelectByDateRange(sd, ed, true).ToList();
            _invitees = Provider.Scheduler.Reservation.SelectInviteesByDateRange(sd, ed, true).ToList();
        }

        public IEnumerator<IReservation> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AssertDatesAreValid(DateTime sd, DateTime ed)
        {
            if (sd < Reservations.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservations.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");
        }
    }
}
