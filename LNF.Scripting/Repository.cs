using LNF.Data;
using LNF.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Scripting
{
    public static class Repository
    {
        public static DataCommandBase ReadOnlyCommand(CommandType type = CommandType.StoredProcedure) => ReadOnlyDataCommand.Create(type);

        public static IList<IDictionary> Query(string query, ScriptParameters parameters)
        {
            IList<IDictionary> result = null;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            using (var adap = new SqlDataAdapter(cmd))
            {
                ApplyParameters(cmd, parameters);
                var dt = new DataTable();
                adap.Fill(dt);
                result = DataTableToList(dt);
                conn.Close();
            }

            return result;
        }

        public static IEnumerable SqlQuery(string query, ScriptParameters parameters)
        {
            var command = ReadOnlyCommand(CommandType.Text);

            if (parameters != null)
            {
                foreach (KeyValuePair<object, object> kvp in parameters)
                    if (query.Contains("@" + kvp.Key.ToString()))
                        command.Param(kvp.Key.ToString(), kvp.Value.ToString() == "null", DBNull.Value, kvp.Value);
            }

            DataTable dt = command.FillDataTable(query);
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

        public static void ApplyParameters(SqlCommand cmd, ScriptParameters parameters)
        {
            foreach (var kvp in parameters)
            {
                var p = cmd.CreateParameter();

                p.ParameterName = kvp.Key.ToString();

                if (kvp.Value == null)
                    p.Value = DBNull.Value;
                else
                    p.Value = kvp.Value;

                cmd.Parameters.Add(p);
            }
        }

        public static IList<IDictionary> DataTableToList(DataTable dt)
        {
            var result = new List<IDictionary>();

            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();
                
                foreach(DataColumn dc in dt.Columns)
                {
                    row.Add(dc.ColumnName, dr[dc.ColumnName]);
                }

                result.Add(row);
            }

            return result;
        }
    }
}
