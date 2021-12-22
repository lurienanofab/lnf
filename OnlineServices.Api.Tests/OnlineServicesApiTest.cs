using LNF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OnlineServices.Api.Tests
{
    [TestClass]
    public abstract class OnlineServicesApiTest
    {
        public int ResourceCount { get; private set; }
        public int ActiveStaffCount { get; private set; }
        
        protected Container Container { get; private set; }
        protected IProvider Provider => Container.GetInstance<IProvider>();

        [TestInitialize]
        public void Setup()
        {
            Container = new Container();
            Container.ConfigureDependencies();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            {
                conn.Open();
                ResourceCount = GetResourceCount(conn);
                ActiveStaffCount = GetActiveStaffCount(conn);
                conn.Close();
            }
        }

        private int GetResourceCount(SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) AS ResourceCount FROM sselScheduler.dbo.[Resource]", conn))
            {
                var obj = cmd.ExecuteScalar();
                var result = Convert.ToInt32(obj);
                return result;
            }
        }

        private int GetActiveStaffCount(SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) AS ActiveStaffCount FROM sselData.dbo.Client WHERE Active = 1 AND (Privs & 2) > 0", conn))
            {
                var obj = cmd.ExecuteScalar();
                var result = Convert.ToInt32(obj);
                return result;
            }
        }
    }
}
