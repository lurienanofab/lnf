using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationCollection : IEnumerable<IReservationItem>
    {
        private IList<IReservationItem> _items = new List<IReservationItem>();
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

        public void Add(IReservationItem item)
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

        public IEnumerable<IReservationItem> Find(DateTime sd, DateTime ed, bool includeAllClients, bool includeCancelled)
        {
            var filter = CreateFilter(sd, ed, includeAllClients, includeCancelled);
            return _items.Where(filter).ToList();
        }

        public IEnumerable<IReservationItem> Find(DateTime d, bool includeAllClients, bool includeCancelled)
        {
            DateTime sd = d.Date;
            DateTime ed = sd.AddDays(1);
            return Find(sd, ed, includeAllClients, includeCancelled);
        }

        public int Count(DateTime sd, DateTime ed, bool includeAllClients, bool includeCancelled)
        {
            var filter = CreateFilter(sd, ed, includeAllClients, includeCancelled);
            return _items.Count(filter);
        }

        public int Count(DateTime d, bool includeAllClients, bool includeCancelled)
        {
            DateTime sd = d.Date;
            DateTime ed = sd.AddDays(1);
            return Count(sd, ed, includeAllClients, includeCancelled);
        }

        public Func<IReservationItem, bool> CreateFilter(DateTime sd, DateTime ed, bool includeAllClients, bool includeCancelled)
        {
            Func<IReservationItem, bool> filter1;
            Func<IReservationItem, bool> filter2;
            Func<IReservationItem, bool> filter3;

            filter1 = x => (x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd);

            if (includeAllClients)
                filter2 = x => true;
            else
                filter2 = x => x.ClientID == ClientID || GetInvited().Contains(x.ReservationID);

            if (includeCancelled)
                filter3 = x => true;
            else
                filter3 = x => x.IsActive;

            Func<IReservationItem, bool> result = x => filter1(x) && filter2(x) && filter3(x);

            return result;
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

        public IEnumerator<IReservationItem> GetEnumerator()
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
