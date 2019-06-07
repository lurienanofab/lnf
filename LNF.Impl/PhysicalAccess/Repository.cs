using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.PhysicalAccess
{
    public class Repository
    {
        public static Repository LNF { get; }
        public static Repository Prowatch { get; }

        static Repository()
        {
            LNF = new Repository(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
            Prowatch = new Repository(ConfigurationManager.ConnectionStrings["cnProwatch"].ConnectionString);
        }

        private readonly string _connstr;

        public Repository(string connstr)
        {
            _connstr = connstr;
        }

        public DataTable FillDataTable(string sql, IDictionary<string, object> parameters = null, CommandType type = CommandType.Text)
        {
            using (var conn = new SqlConnection(_connstr))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = type })
            using (var adap = new SqlDataAdapter(cmd))
            {
                ApplyParameters(cmd, parameters);
                var dt = new DataTable();
                adap.Fill(dt);
                return dt;
            }
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null, CommandType type = CommandType.Text)
        {
            using (var conn = new SqlConnection(_connstr))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = type })
            {
                conn.Open();
                ApplyParameters(cmd, parameters);
                var result = cmd.ExecuteScalar();
                conn.Close();
                return result;
            }
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null, CommandType type = CommandType.Text)
        {
            using (var conn = new SqlConnection(_connstr))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = type })
            {
                conn.Open();
                ApplyParameters(cmd, parameters);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }

        private void ApplyParameters(SqlCommand cmd, IDictionary<string, object> parameters)
        {
            if (parameters != null)
                foreach (var kvp in parameters)
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
        }
    }
}