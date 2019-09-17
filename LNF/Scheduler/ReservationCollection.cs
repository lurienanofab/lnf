using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationCollection : IEnumerable<IReservation>
    {
        private IList<IReservation> _items = new List<IReservation>();

        public IProvider Provider { get; }

        public ReservationCollection(IProvider provider)
        {
            Provider = provider;
        }

        public  IEnumerable<IReservation> this[DateTime date] => Find(date, true);

        public void Add(IReservation item)
        {
            _items.Add(item);
        }

        public IEnumerable<IReservation> Find(DateTime date, bool includeCancelled)
        {
            DateTime d = date.Date;

            DateTime sd = d;
            DateTime ed = d.AddDays(1);

            IEnumerable<IReservation> result = _items.Where(x => (x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd));

            if (includeCancelled)
                return result.ToList();
            else
                return result.Where(x => x.IsActive).ToList();
        }

        public IEnumerable<IReservation> Find(DateTime date, int clientId, bool includeCancelled)
        {
            var result = Find(date, includeCancelled);
            return result.Where(x => x.ClientID == clientId).ToList();
        }

        public void SelectByResource(int resourceId, DateTime sd, DateTime ed)
        {
            if (sd < Reservation.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservation.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");

            _items = Provider.Scheduler.Reservation.SelectByResource(resourceId, sd, ed, true).ToList();
        }

        public void SelectByProcessTech(int processTechId, DateTime sd, DateTime ed)
        {
            if (sd < Reservation.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservation.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");

            _items = Provider.Scheduler.Reservation.SelectByProcessTech(processTechId, sd, ed, true).ToList();
        }

        public void SelectByClient(int clientId, DateTime sd, DateTime ed)
        {
            if (sd < Reservation.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservation.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");

            _items = Provider.Scheduler.Reservation.SelectByClient(clientId, sd, ed, true).ToList();
        }

        public void SelectByDateRange(DateTime sd, DateTime ed)
        {
            if (sd < Reservation.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservation.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");

            _items = Provider.Scheduler.Reservation.SelectByDateRange(sd, ed, true).ToList();
        }

        public IEnumerator<IReservation> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
