using NHibernate;
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

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value);
        }
    }
}
