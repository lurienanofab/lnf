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
        private IList<IReservation> _items;

        protected IProvider Provider { get; }

        public ReservationCollection(IProvider provider)
        {
            Provider = provider;
        }

        public  IList<IReservation> this[DateTime date]
        {
            get { return Find(date, true); }
        }

        public void Add(IReservation item)
        {
            _items.Add(item);
        }

        public IList<IReservation> Find(DateTime date, bool includeDeleted)
        {
            DateTime d = date.Date;

            DateTime sd = d;
            DateTime ed = d.AddDays(1);

            IEnumerable<IReservation> result = _items.Where(x => (x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd));

            if (includeDeleted)
                return result.ToList();
            else
                return result.Where(x => x.IsActive).ToList();
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

        private IList<IReservation> GetItems()
        {
            if (_items == null)
                _items = new List<IReservation>();
            return _items;
        }

        public IEnumerator<IReservation> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
