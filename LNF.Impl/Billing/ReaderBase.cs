using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public abstract class ReaderBase
    {
        protected SqlConnection Connection { get; }

        public ReaderBase(SqlConnection conn)
        {
            Connection = conn;
        }

        protected void AddParameter(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value);
        }

        protected void AddParameter(SqlCommand cmd, string name, object value, SqlDbType dbType)
        {
            cmd.Parameters.AddWithValue(name, value, dbType);
        }

        protected void AddParameter(SqlCommand cmd, string name, object value, SqlDbType dbType, int size)
        {
            cmd.Parameters.AddWithValue(name, value, dbType, size);
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value);
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value, SqlDbType dbType)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value, dbType);
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value, SqlDbType dbType, int size)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value, dbType, size);
        }
    }
}
