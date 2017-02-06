using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ReservationProcessInfo data using the System.Data namespace.
    /// </summary>
    public static class ReservationProcessInfoData
    {
        public static IDataReader SelectAllDataReader(int reservationId)
        {
            var dba = DA.Current.GetAdapter();

            return dba
                .ApplyParameters(new { ReservationID = reservationId })
                .ExecuteReader("sselScheduler.dbo.procReservationProcessInfoSelect");
        }

        public static DataTable SelectAllDataTable(int reservationId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .MapSchema()
                    .ApplyParameters(new { ReservationID = reservationId })
                    .FillDataTable("sselScheduler.dbo.procReservationProcessInfoSelect");
        }

        public static void Update(DataTable dt, int reservationId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.InsertCommand
                    .AddParameter("@ReservationID", reservationId)
                    .AddParameter("@ProcessInfoLineID", SqlDbType.Int)
                    .AddParameter("@Value", SqlDbType.Float)
                    .AddParameter("@Special", SqlDbType.Bit);

                dba.UpdateCommand
                    .AddParameter("@ReservationProcessInfoID", SqlDbType.Int)
                    .AddParameter("@ProcessInfoLineID", SqlDbType.Int)
                    .AddParameter("@Value", SqlDbType.Float)
                    .AddParameter("@Special", SqlDbType.Bit);

                dba.DeleteCommand.AddParameter("@ReservationProcessInfoID", SqlDbType.Int);

                dba.UpdateDataTable(dt,
                    "sselScheduler.dbo.procReservationProcessInfoInsert",
                    "sselScheduler.dbo.procReservationProcessInfoUpdate",
                    "sselScheduler.dbo.procReservationProcessInfoDelete");
            }
        }
    }
}
