using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ProcessInfo data using the System.Data namespace.
    /// </summary>
    public class ProcessInfoData
    {
        public IProvider Provider { get; }

        private ProcessInfoData(IProvider provider)
        {
            Provider = provider;
        }

        public static ProcessInfoData Create(IProvider provider)
        {
            return new ProcessInfoData(provider);
        }

        /// <summary>
        /// Returns all ProcessInfo belonging to the specified Resource
        /// </summary>
        public DataTable SelectProcessInfo(IResource res)
        {
            var items = Provider.Scheduler.ProcessInfo.GetProcessInfos(res.ResourceID);

            var dt = CreateTable();

            foreach (var i in items)
            {
                var ndr = dt.NewRow();
                ndr.SetField("ProcessInfoID", i.ProcessInfoID);
                ndr.SetField("ResourceID", i.ResourceID);
                ndr.SetField("ResourceName", res.ResourceName);
                ndr.SetField("ProcessInfoName", i.ProcessInfoName);
                ndr.SetField("ParamName", i.ParamName);
                ndr.SetField("ValueName", i.ValueName);
                ndr.SetField("Special", i.Special);
                ndr.SetField("AllowNone", i.AllowNone);
                ndr.SetField("Order", i.Order);
                ndr.SetField("RequireValue", i.RequireValue);
                ndr.SetField("RequireSelection", i.RequireSelection);
                ndr.SetField("MaxAllowed", i.MaxAllowed);
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        private DataTable CreateTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("ProcessInfoID", typeof(int));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("ProcessInfoName", typeof(string));
            dt.Columns.Add("ParamName", typeof(string));
            dt.Columns.Add("ValueName", typeof(string));
            dt.Columns.Add("Special", typeof(string));
            dt.Columns.Add("AllowNone", typeof(bool));
            dt.Columns.Add("Order", typeof(int));
            dt.Columns.Add("RequireValue", typeof(bool));
            dt.Columns.Add("RequireSelection", typeof(bool));
            dt.Columns.Add("MaxAllowed", typeof(int));

            dt.Columns["ProcessInfoID"].AutoIncrement = true;
            dt.Columns["ProcessInfoID"].AutoIncrementSeed = 1;
            dt.Columns["ProcessInfoID"].AutoIncrementStep = 1;
            dt.PrimaryKey = new[] { dt.Columns["ProcessInfoID"] };

            return dt;
        }

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public void Update(IEnumerable<IProcessInfo> insert, IEnumerable<IProcessInfo> update, IEnumerable<IProcessInfo> delete)
        {
            Provider.Scheduler.ProcessInfo.Update(insert, update, delete);
        }

        public IProcessInfo CreateProcessInfo(DataRow dr)
        {
            return Provider.Scheduler.ProcessInfo.Create(dr);
        }
    }
}