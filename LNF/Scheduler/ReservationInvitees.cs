using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationInvitees
    {
        public IProvider Provider { get; }

        public ReservationInvitees(IProvider provider)
        {
            Provider = provider;
        }

        public static ReservationInvitees Create(IProvider provider)
        {
            return new ReservationInvitees(provider);
        }

        public List<Invitee> SelectInvitees(int reservationId)
        {
            return Provider.Scheduler.Reservation.GetInvitees(reservationId).Select(x => new Invitee
            {
                ReservationID = x.ReservationID,
                InviteeID = x.InviteeID,
                LName = x.InviteeLName,
                FName = x.InviteeFName,
            }).ToList();
        }

        /// <summary>
        /// Returns all available clients
        /// </summary>
        public List<AvailableInvitee> SelectAvailable(int reservationId, int resourceId, int activityId, int clientId)
        {
            return Provider.Scheduler.Reservation.GetAvailableInvitees(reservationId, resourceId, activityId, clientId).ToList();
        }

        /// <summary>
        /// Inserts/Deletes reservation invitees for the specified reservation
        /// </summary>
        /// <param name="reservationId">The reservation to which invitees will be added or deleted.</param>
        /// <param name="add">The list of InviteeID values for invitees to add (note: ReservationInvitee.InviteeID and Client.ClientID are the same).</param>
        /// <param name="delete">The list of InviteeID values for invitees to delete (note: ReservationInvitee.InviteeID and Client.ClientID are the same).</param>
        public void Update(int reservationId, IEnumerable<int> add, IEnumerable<int> delete)
        {
            foreach (var inviteeId in add)
                Provider.Scheduler.Reservation.AddInvitee(reservationId, inviteeId);
            
            foreach(var inviteeId in delete)
                Provider.Scheduler.Reservation.DeleteInvitee(reservationId, inviteeId);
        }

        /// <summary>
        /// Returns true if the client is an invitee for the specified reservation
        /// </summary>
        public bool IsInvited(int reservationId, int inviteeId)
        {
            return Provider.Scheduler.Reservation.InviteeExists(reservationId, inviteeId);
        }

        public static DataTable ToDataTable(IEnumerable<IReservationInvitee> invitees)
        {
            var dt = new DataTable();
            dt.Columns.Add("ReservationID", typeof(int));
            dt.Columns.Add("InviteeID", typeof(int));
            dt.Columns.Add("LName", typeof(string));
            dt.Columns.Add("FName", typeof(string));
            dt.Columns.Add("DisplayName", typeof(string));

            foreach (var i in invitees)
            {
                var ndr = dt.NewRow();
                ndr.SetField("ReservationID", i.ReservationID);
                ndr.SetField("InviteeID", i.InviteeID);
                ndr.SetField("LName", i.LName);
                ndr.SetField("LName", i.FName);
                ndr.SetField("DisplayName", i.DisplayName);
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        public static IEnumerable<IReservationInvitee> ToList(DataTable dt, int reservationId)
        {
            if (dt == null) return null;

            if (dt.Columns.Contains("ReservationID") && dt.Columns.Contains("InviteeID"))
            {
                var result = new List<IReservationInvitee>();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        int rid = Utility.ConvertTo(dr["ReservationID"], 0);

                        if (reservationId <= 0) rid = reservationId;

                        int inviteeId = Utility.ConvertTo(dr["InviteeID"], 0);

                        if (rid > 0 && inviteeId > 0)
                        {
                            var ri = ServiceProvider.Current.Scheduler.Reservation.GetInvitee(rid, inviteeId);

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
    }
}
