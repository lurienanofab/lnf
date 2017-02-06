using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LNF.Repository;
using LNF.Repository.Scheduler;

namespace LNF.Scheduler
{
    public class ReservationCollection : IEnumerable<Reservation>
    {
        private IList<Reservation> _items;

        public  IList<Reservation> this[DateTime date]
        {
            get { return Find(date, true); }
        }

        public void Add(Reservation item)
        {
            _items.Add(item);
        }

        public IList<Reservation> Find(DateTime date, bool includeDeleted)
        {
            DateTime d = date.Date;

            DateTime sd = d;
            DateTime ed = d.AddDays(1);

            IEnumerable<Reservation> result = _items.Where(x => (x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd));

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

            _items = DA.Scheduler.Reservation.SelectByResource(resourceId, sd, ed, true);
        }

        public void SelectByProcessTech(int procTechId, DateTime sd, DateTime ed)
        {
            if (sd < Reservation.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservation.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");

            _items = DA.Scheduler.Reservation.SelectByProcessTech(procTechId, sd, ed, true);
        }

        public void SelectByClient(int clientId, DateTime sd, DateTime ed)
        {
            if (sd < Reservation.MinReservationBeginDate)
                throw new ArgumentOutOfRangeException("sd");

            if (ed > Reservation.MaxReservationEndDate)
                throw new ArgumentOutOfRangeException("ed");

            _items = DA.Scheduler.Reservation.SelectByClient(clientId, sd, ed, true);
        }

        private IList<Reservation> GetItems()
        {
            if (_items == null)
                _items = new List<Reservation>();
            return _items;
        }

        public IEnumerator<Reservation> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
