using System.Data;
using System.Data.SqlClient;

namespace LNF
{
    public static class ParamterExtentions
    {
        public static void AddWithValue(this SqlParameterCollection parameters, string parameterName, object value, SqlDbType dbType)
        {
            var p = new SqlParameter(parameterName, value) { SqlDbType = dbType };
            parameters.Add(p);
        }

        public static void AddWithValue(this SqlParameterCollection parameters, string parameterName, object value, SqlDbType dbType, int size)
        {
            var p = new SqlParameter(parameterName, value) { SqlDbType = dbType, Size = size };
            parameters.Add(p);
        }

        public static void AddOutputParameter(this SqlParameterCollection parameters, string parameterName, SqlDbType dbType)
        {
            var p = new SqlParameter(parameterName, dbType) { Direction = ParameterDirection.Output };
            parameters.Add(p);
        }

        public static void AddOutputParameter(this SqlParameterCollection parameters, string parameterName, SqlDbType dbType, int size)
        {
            var p = new SqlParameter(parameterName, dbType, size) { Direction = ParameterDirection.Output };
            parameters.Add(p);
        }
    }
}
