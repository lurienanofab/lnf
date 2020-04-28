using LNF.DataAccess;
using LNF.Impl;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace LNF.Tests
{
    [TestClass]
    public class DataCommandTests
    {
        private DependencyResolver _resolver;

        [TestMethod]
        public void DoesItWork()
        {
            _resolver = new ThreadStaticResolver();
            using (var uow = _resolver.GetInstance<IUnitOfWork>())
            {
                var session = _resolver.GetInstance<ISessionManager>().Session;
                var dt = session.Command(CommandType.Text).Param("ClientID", 1301).FillDataTable("SELECT UserName FROM dbo.Client WHERE ClientID = @ClientID");
                Assert.AreEqual(1, dt.Rows.Count);
                Assert.AreEqual("jgett", dt.Rows[0]["UserName"]);
            }
        }
    }
}
