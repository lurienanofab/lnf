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

            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "Select")
                    .AddParameter("@ReservationID", reservationId)
                    .AddParameter("@ClientID", clientId);

                using (var reader = dba.ExecuteReader("sselScheduler.dbo.procReservationInviteeSelect"))
                {
                    result = reader.Read();
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all invitees for the specified reservation
        /// </summary>
        public static IDataReader SelectReservationInviteesDataReader(int reservationId)
        {
            var dba = DA.Current.GetAdapter();

            return dba
                .ApplyParameters(new { Action = "SelectByReservation", ReservationID = reservationId })
                .ExecuteReader("sselScheduler.dbo.procReservationInviteeSelect");
        }


        /// <summary>
        /// Returns all invitees for the specified reservation
        /// </summary>
        public static DataTable SelectReservationInviteesDataTable(int reservationId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .MapSchema()
                    .ApplyParameters(new { Action = "SelectByReservation", ReservationID = reservationId })
                    .FillDataTable("sselScheduler.dbo.procReservationInviteeSelect");
        }

        /// <summary>
        /// Returns all available clients
        /// </summary>
        public static DataTable SelectAvailableInvitees(int reservationId, int resourceId, int activityId, int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "SelectAvailInvitees")
                    .AddParameter("@ReservationID", reservationId)
                    .AddParameter("@ResourceID", resourceId)
                    .AddParameter("@ActivityID", activityId)
                    .AddParameter("@ClientID", clientId);

                var dt = dba.FillDataTable("sselScheduler.dbo.procReservationInviteeSelect");

                dt.PrimaryKey = new[] { dt.Columns["ClientID"] };

                return dt;
            }
        }

        /// <summary>
        /// Inserts/Deletes reservation invitees for the specified reservation
        /// </summary>
        public static void Update(DataTable dt, int reservationId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.InsertCommand
                    .AddParameter("@ReservationID", reservationId)
                    .AddParameter("@InviteeID", SqlDbType.Int);

                dba.DeleteCommand
                    .AddParameter("@ReservationID", reservationId)
                    .AddParameter("@InviteeID", SqlDbType.Int);

                dba.UpdateDataTable(dt,
                    insertSql: "sselScheduler.dbo.procReservationInviteeInsert",
                    deleteSql: "sselScheduler.dbo.procReservationInviteeDelete");
            }
        }
    }
}
