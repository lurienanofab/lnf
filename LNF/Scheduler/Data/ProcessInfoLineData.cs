using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ProcessInfoLine data using the System.Data namespace.
    /// </summary>
    public class ProcessInfoLineData
    {
        public IProvider Provider { get; }

        private ProcessInfoLineData(IProvider provider)
        {
            Provider = provider;
        }

        public static ProcessInfoLineData Create(IProvider provider)
        {
            return new ProcessInfoLineData(provider);
        }

        /// <summary>
        /// Returns all ProcessInfo belonging to the specified Resource
        /// </summary>
        public DataTable SelectByResource(int resourceId)
        {
            var items = Provider.Scheduler.Resource.GetProcessInfoLines(resourceId);
            var dt = CreateDataTable();
            FillDataTable(dt, items);
            return dt;
        }

        public DataTable SelectByProcessInfo(int resourceId)
        {
            var items = Provider.Scheduler.ProcessInfo.GetProcessInfoLines(resourceId);
            var dt = CreateDataTable();
            FillDataTable(dt, items);
            return dt;
        }

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public void Update(IEnumerable<IProcessInfoLine> insert, IEnumerable<IProcessInfoLine> update, IEnumerable<IProcessInfoLine> delete)
        {
            Provider.Scheduler.ProcessInfo.Update(insert, update, delete);
        }

        private static DataTable CreateDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("ProcessInfoLineID", typeof(int));
            dt.Columns.Add("ProcessInfoID", typeof(int));
            dt.Columns.Add("Param", typeof(string));
            dt.Columns.Add("MinValue", typeof(double));
            dt.Columns.Add("MaxValue", typeof(double));
            dt.Columns.Add("ProcessInfoLineParamID", typeof(int));

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
                ndr.SetField("Param", i.Param);
                ndr.SetField("MinValue", i.MinValue);
                ndr.SetField("MaxValue", i.MaxValue);
                dt.Rows.Add(ndr);
            }
        }
    }
}