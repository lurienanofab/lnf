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
            return DA.Command()
                .Param("ReservationID", reservationId)
                .ExecuteReader("sselScheduler.dbo.procReservationProcessInfoSelect");
        }

        public static DataTable SelectAllDataTable(int reservationId)
        {
            return DA.Command()
                .MapSchema()
                .Param("ReservationID", reservationId)
                .FillDataTable("sselScheduler.dbo.procReservationProcessInfoSelect");
        }

        public static void Update(DataTable dt, int reservationId)
        {
            DA.Command().Update(dt, x =>
            {
                x.Insert.SetCommandText("sselScheduler.dbo.procReservationProcessInfoInsert");
                x.Insert.AddParameter("ReservationID", reservationId);
                x.Insert.AddParameter("ProcessInfoLineID", SqlDbType.Int);
                x.Insert.AddParameter("Value", SqlDbType.Float);
                x.Insert.AddParameter("Special", SqlDbType.Bit);

                x.Update.SetCommandText("sselScheduler.dbo.procReservationProcessInfoUpdate");
                x.Update.AddParameter("ReservationProcessInfoID", SqlDbType.Int);
                x.Update.AddParameter("ProcessInfoLineID", SqlDbType.Int);
                x.Update.AddParameter("Value", SqlDbType.Float);
                x.Update.AddParameter("Special", SqlDbType.Bit);

                x.Delete.SetCommandText("sselScheduler.dbo.procReservationProcessInfoDelete");
                x.Delete.AddParameter("ReservationProcessInfoID", SqlDbType.Int);
            });
        }
    }
}
