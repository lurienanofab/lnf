using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ReservationInvitee data using the System.Data namespace.
    /// </summary>
    public static class ReservationInviteeData
    {
        /// <summary>
        /// Returns true if the client is an invitee for the specified reservation
        /// </summary>
        public static bool IsInvited(int reservationId, int clientId)
        {
            bool result = false;

            var cmd = DA.Command()
                .Param("Action", "Select")
                .Param("ReservationID", reservationId)
                .Param("ClientID", clientId);

            using (var reader = cmd.ExecuteReader("sselScheduler.dbo.procReservationInviteeSelect"))
            {
                result = reader.Read();
                reader.Close();
            }

            return result;
        }

        /// <summary>
        /// Returns all invitees for the specified reservation
        /// </summary>
        public static ExecuteReaderResult SelectReservationInviteesDataReader(int reservationId)
        {
            return DA.Command()
                .Param("Action", "SelectByReservation")
                .Param("ReservationID", reservationId)
                .ExecuteReader("sselScheduler.dbo.procReservationInviteeSelect");
        }


        /// <summary>
        /// Returns all invitees for the specified reservation
        /// </summary>
        public static DataTable SelectReservationInviteesDataTable(int reservationId)
        {
            return DA.Command()
                .MapSchema()
                .Param("Action", "SelectByReservation")
                .Param("ReservationID", reservationId)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeSelect");
        }

        /// <summary>
        /// Returns all available clients
        /// </summary>
        public static DataTable SelectAvailableInvitees(int reservationId, int resourceId, int activityId, int clientId)
        {
            var dt = DA.Command()
                .Param("Action", "SelectAvailInvitees")
                .Param("ReservationID", reservationId)
                .Param("ResourceID", resourceId)
                .Param("ActivityID", activityId)
                .Param("ClientID", clientId)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeSelect");

            dt.PrimaryKey = new[] { dt.Columns["ClientID"] };

            return dt;
        }

        /// <summary>
        /// Inserts/Deletes reservation invitees for the specified reservation
        /// </summary>
        public static void Update(DataTable dt, int reservationId)
        {
            DA.Command().Update(dt, x =>
            {
                x.Insert.SetCommandText("sselScheduler.dbo.procReservationInviteeInsert");
                x.Insert.AddParameter("ReservationID", reservationId);
                x.Insert.AddParameter("InviteeID", SqlDbType.Int);

                x.Delete.SetCommandText("sselScheduler.dbo.procReservationInviteeDelete");
                x.Delete.AddParameter("ReservationID", reservationId);
                x.Delete.AddParameter("InviteeID", SqlDbType.Int);
            });
        }
    }
}
