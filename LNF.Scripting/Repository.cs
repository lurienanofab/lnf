using LNF.CommonTools;
using LNF.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scripting
{
    public static class Repository
    {
        public static IList<IDictionary> Query(string query, Parameters parameters)
        {
            var q = DA.Current.SqlQuery(query).SetParameters(parameters);
            var result = q.List();
            return result;
        }

        public static IEnumerable SqlQuery(string query, Parameters parameters)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselDataReadOnly"))
            {
                if (parameters != null)
                {
                    var cmd = dba.SelectCommand;
                    foreach (KeyValuePair<object, object> kvp in parameters)
                        if(query.Contains("@" + kvp.Key.ToString()))
                            cmd.AddParameter("@" + kvp.Key.ToString(), kvp.Value.ToString() == "null" ? DBNull.Value : kvp.Value);
                }

                DataTable dt = dba.CommandTypeText().FillDataTable(query);
                IList<IDictionary<object, object>> result = new List<IDictionary<object, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    IDictionary<object, object> dict = new Dictionary<object, object>();
                    foreach (DataColumn dc in dt.Columns)
                        dict.Add(dc.ColumnName, dr[dc.ColumnName]);
                    result.Add(dict);
                }

                return result;
            }
        }
    }
}
