using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using System.Configuration;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public abstract class BillingRepository : RepositoryBase
    {
        public BillingRepository(ISessionManager mgr) : base(mgr) { }

        protected SqlConnection NewConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
        }
    }
}
