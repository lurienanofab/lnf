using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ReservationProcessInfo data using the System.Data namespace.
    /// </summary>
    public static class ReservationProcessInfoData
    {
        public static DataTable SelectAllDataTable(int reservationId)
        {
            var items = ServiceProvider.Current.Scheduler.ProcessInfo.GetReservationProcessInfos(reservationId);

            var dt = new DataTable();
            dt.Columns.Add("ReservationProcessInfoID", typeof(int));
            dt.Columns.Add("Active", typeof(bool));
            dt.Columns.Add("ChargeMultiplier", typeof(double));
            dt.Columns.Add("Param", typeof(string));
            dt.Columns.Add("ParameterName", typeof(string));
            dt.Columns.Add("ProcessInfoID", typeof(int));
            dt.Columns.Add("ProcessInfoLineID", typeof(int));
            dt.Columns.Add("ProcessInfoLineParamID", typeof(int));
            dt.Columns.Add("ProcessInfoName", typeof(string));
            dt.Columns.Add("ReservationID", typeof(int));
            dt.Columns.Add("RunNumber", typeof(int));
            dt.Columns.Add("Special", typeof(bool));
            dt.Columns.Add("Value", typeof(double));

            foreach(var i in items)
            {
                var ndr = dt.NewRow();
                ndr.SetField("ReservationProcessInfoID", i.ReservationProcessInfoID);
                ndr.SetField("Active", i.Active);
                ndr.SetField("ChargeMultiplier", i.ChargeMultiplier);
                ndr.SetField("Param", i.Param);
                ndr.SetField("ParameterName", i.ParameterName);
                ndr.SetField("ProcessInfoID", i.ProcessInfoID);
                ndr.SetField("ProcessInfoLineID", i.ProcessInfoLineID);
                ndr.SetField("ProcessInfoLineParamID", i.ProcessInfoLineParamID);
                ndr.SetField("ProcessInfoName", i.ProcessInfoName);
                ndr.SetField("ReservationID", i.ReservationID);
                ndr.SetField("RunNumber", i.RunNumber);
                ndr.SetField("Special", i.Special);
                ndr.SetField("Value", i.Value);
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        public static void Update(int reservationId, IEnumerable<IReservationProcessInfo> insert, IEnumerable<IReservationProcessInfo> update, IEnumerable<IReservationProcessInfo> delete)
        {
            foreach (var i in insert)
                i.ReservationID = reservationId;

            foreach (var u in update)
                u.ReservationID = reservationId;

            foreach (var d in delete)
                d.ReservationID = reservationId;

            ServiceProvider.Current.Scheduler.ProcessInfo.Update(insert, update, delete);
        }
    }
}
