using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ProcessInfoLine data using the System.Data namespace.
    /// </summary>
    public static class ProcessInfoLineData
    {
        /// <summary>
        /// Returns all ProcessInfo belonging to the specified Resource
        /// </summary>
        public static DataTable SelectByResource(int resourceId)
        {
            var items = ServiceProvider.Current.Scheduler.Resource.GetProcessInfoLines(resourceId);
            var dt = CreateDataTable();
            FillDataTable(dt, items);
            return dt;
        }

        public static DataTable SelectByProcessInfo(int processInfoId)
        {
            var items = ServiceProvider.Current.Scheduler.ProcessInfo.GetProcessInfoLines(processInfoId);
            var dt = CreateDataTable();
            FillDataTable(dt, items);
            return dt;
        }

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public static void Update(IEnumerable<IProcessInfoLine> insert, IEnumerable<IProcessInfoLine> update, IEnumerable<IProcessInfoLine> delete)
        {
            ServiceProvider.Current.Scheduler.ProcessInfo.Update(insert, update, delete);
        }

        private static DataTable CreateDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("ProcessInfoLineID", typeof(int));
            dt.Columns.Add("ProcessInfoID", typeof(int));
            dt.Columns.Add("ProcessInfoLineParamID", typeof(int));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("ResourceName", typeof(string));
            dt.Columns.Add("Param", typeof(string));
            dt.Columns.Add("ParameterName", typeof(string));
            dt.Columns.Add("ParameterType", typeof(int));
            dt.Columns.Add("MinValue", typeof(double));
            dt.Columns.Add("MaxValue", typeof(double));

            dt.Columns["ProcessInfoLineID"].AutoIncrement = true;
            dt.Columns["ProcessInfoLineID"].AutoIncrementSeed = 1;
            dt.Columns["ProcessInfoLineID"].AutoIncrementStep = 1;
            dt.PrimaryKey = new[] { dt.Columns["ProcessInfoLineID"] };

            return dt;
        }

        private static void FillDataTable(DataTable dt, IEnumerable<IProcessInfoLine> items)
        {
            foreach (var i in items)
            {
                var ndr = dt.NewRow();
                ndr.SetField("ProcessInfoLineID", i.ProcessInfoLineID);
                ndr.SetField("ProcessInfoID", i.ProcessInfoID);
                ndr.SetField("ProcessInfoLineParamID", i.ProcessInfoLineParamID);
                ndr.SetField("ResourceID", i.ResourceID);
                ndr.SetField("ResourceName", i.ResourceName);
                ndr.SetField("Param", i.Param);
                ndr.SetField("ParameterName", i.ParameterName);
                ndr.SetField("ParameterType", i.ParameterType);
                ndr.SetField("MinValue", i.MinValue);
                ndr.SetField("MaxValue", i.MaxValue);
                dt.Rows.Add(ndr);
            }
        }
    }
}