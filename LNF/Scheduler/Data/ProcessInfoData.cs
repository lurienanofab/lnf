using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ProcessInfo data using the System.Data namespace.
    /// </summary>
    public static class ProcessInfoData
    {
        /// <summary>
        /// Returns all ProcessInfo belonging to the specified Resource
        /// </summary>
        public static DataTable SelectProcessInfo(int resourceId)
        {
            var items = ServiceProvider.Current.Scheduler.ProcessInfo.GetProcessInfos(resourceId);

            var dt = new DataTable();
            dt.Columns.Add("ProcessInfoID", typeof(int));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("ResourceName", typeof(string));
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

            foreach (var i in items)
            {
                var ndr = dt.NewRow();
                ndr.SetField("ProcessInfoID", i.ProcessInfoID);
                ndr.SetField("ResourceID", i.ResourceID);
                ndr.SetField("ResourceName", i.ResourceName);
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

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public static void Update(IEnumerable<IProcessInfo> insert, IEnumerable<IProcessInfo> update, IEnumerable<IProcessInfo> delete)
        {
            ServiceProvider.Current.Scheduler.ProcessInfo.Update(insert, update, delete);
        }

        public static IProcessInfo CreateProcessInfo(DataRow dr)
        {
            return new ProcessInfoItem
            {
                ProcessInfoID = dr.Field<int>("ProcessInfoID"),
                ResourceID = dr.Field<int>("ResourceID"),
                ResourceName = dr.Field<string>("ResourceName"),
                ProcessInfoName = dr.Field<string>("ProcessInfoName"),
                ParamName = dr.Field<string>("ParamName"),
                ValueName = dr.Field<string>("ValueName"),
                Special = dr.Field<string>("Special"),
                AllowNone = dr.Field<bool>("AllowNone"),
                Order = dr.Field<int>("Order"),
                RequireValue = dr.Field<bool>("RequireValue"),
                RequireSelection = dr.Field<bool>("RequireSelection"),
                MaxAllowed = dr.Field<int>("MaxAllowed")
            };
        }
    }
}