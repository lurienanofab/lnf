using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler
{
    public class ReservationInviteeManager : ManagerBase, IReservationInviteeManager
    {
        public ReservationInviteeManager(ISession session) : base(session) { }

        public IList<ReservationInvitee> ToReservationInviteeList(DataTable dt, int reservationId)
        {
            if (dt == null) return null;

            if (dt.Columns.Contains("ReservationID") && dt.Columns.Contains("InviteeID"))
            {
                var result = new List<ReservationInvitee>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        int rsvId = Utility.ConvertTo(dr["ReservationID"], 0);

                        if (rsvId <= 0) rsvId = reservationId;

                        int inviteeId = Utility.ConvertTo(dr["InviteeID"], 0);

                        if (rsvId != 0 && inviteeId != 0)
                        {
                            var reservation = Session.Single<Reservation>(rsvId);
                            var invitee = Session.Single<Client>(inviteeId);

                            var ri = ReservationInviteeUtility.Select(reservation, invitee);

                            if (ri != null)
                                result.Add(ri);
                        }
                    }
                }
                return result;
            }
            else
                throw new Exception("ReservationID and InviteeID columns are required.");
        }

        public ReservationInvitee Find(int reservationId, int inviteeId)
        {
            ReservationInvitee key = new ReservationInvitee()
            {
                Reservation = Session.Single<Reservation>(reservationId),
                Invitee = Session.Single<Client>(inviteeId)
            };

            if (key.Reservation == null || key.Invitee == null)
                return null;

            return Find(key);
        }

        public ReservationInvitee Find(ReservationInvitee key)
        {
            return Session.Single<ReservationInvitee>(key);
        }

        public bool Exists(int reservationId, int inviteeId)
        {
            return Find(reservationId, inviteeId) != null;
        }

        public bool Exists(ReservationInvitee key)
        {
            return Find(key) != null;
        }

        /// <summary>
        /// Adds a ReservationInvitee record if it does not already exist.
        /// </summary>
        public void Insert(int reservationId, int inviteeId)
        {
            ReservationInvitee ri = new ReservationInvitee()
            {
                Reservation = Session.Single<Reservation>(reservationId),
                Invitee = Session.Single<Client>(inviteeId)
            };

            if (!Exists(ri))
                Session.Insert(ri);
        }


        public void Delete(int reservationId, int inviteeId)
        {
            var ri = Find(reservationId, inviteeId);

            if (ri != null)
                Session.Delete(ri);
        }

        /// <summary>
        /// Deletes an existing ReservationInvitee record.
        /// </summary>
        public void Delete(ReservationInvitee key)
        {
            var ri = Find(key);

            if (ri != null)
                Session.Delete(ri);
        }
    }
}
