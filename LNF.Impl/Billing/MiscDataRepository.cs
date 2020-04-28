using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public class MiscDataRepository : RepositoryBase, IMiscDataRepository
    {
        public MiscDataRepository(ISessionManager mgr) : base(mgr) { }

        public DataTable ReadMiscData(DateTime period)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            {
                conn.Open();
                var result = GetReader(conn).ReadMiscData(period);
                conn.Close();
                return result;
            }
        }

        private MiscDataReader GetReader(SqlConnection conn)
        {
            return new MiscDataReader(conn);
        }
    }
}
