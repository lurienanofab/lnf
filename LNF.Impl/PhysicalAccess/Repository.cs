using LNF.PhysicalAccess;
using System;
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
            if (ConfigurationManager.ConnectionStrings["cnSselData"] == null)
                throw new Exception("Missing connection string: cnSselData");

            if (ConfigurationManager.ConnectionStrings["cnProwatch"] == null)
                throw new Exception("Missing connection string: cnProwatch");

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
            using (var cmd = conn.CreateCommand(sql, type))
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
            using (var cmd = conn.CreateCommand(sql, type))
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
            using (var cmd = conn.CreateCommand(sql, type))
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
            {
                cmd.Parameters.Clear();

                foreach (var kvp in parameters)
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }
        }

        public IEnumerable<Badge> GetCurrentlyInArea(string alias)
        {
            var dt = SelectAreaOccupants(alias);
            var result = Utility.CreateBadges(dt);
            return result;
        }

        public IEnumerable<BadgeInArea> GetBadgeInAreas(string alias)
        {
            var dt = SelectAreaOccupants(alias);
            var result = Utility.CreateBadgeInAreas(dt);
            return result;
        }

        private DataTable SelectAreaOccupants(string alias)
        {
            string sql = "SELECT ID, BADGE_CLIENTID" +
                ", BADGE_SSEL_UNAME, LNAME" +
                ", FNAME, ISSUE_DATE" +
                ", EXPIRE_DATE, EVENT_TIME" +
                ", CARD_NO, EVENT_DESCRP" +
                ", AREA_NAME, ALT_DESCRP " +
                "FROM LNF.dbo.UsersInLab " +
                "WHERE AREA_NAME = ISNULL(@AreaName, AREA_NAME) " +
                "ORDER BY AREA_NAME, EVENT_TIME";

            var area = Utility.GetAreaName(alias);

            var dt = FillDataTable(sql, new Dictionary<string, object>
            {
                ["AreaName"] = Utility.DBNullIf(area, string.IsNullOrEmpty(area))
            });

            return dt;
        }
    }
}