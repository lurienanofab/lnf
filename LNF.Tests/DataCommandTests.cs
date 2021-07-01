using LNF.DataAccess;
using LNF.Impl;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;

namespace LNF.Tests
{
    [TestClass]
    public class DataCommandTests
    {
        private Container _container;

        [TestMethod]
        public void DoesItWork()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            var cfg = new ThreadStaticContainerConfiguration(_container);
            cfg.RegisterAllTypes();

            using (var uow = _container.GetInstance<IUnitOfWork>())
            {
                var session = _container.GetInstance<ISessionManager>().Session;
                var dt = session.Command(CommandType.Text).Param("ClientID", 1301).FillDataTable("SELECT UserName FROM dbo.Client WHERE ClientID = @ClientID");
                Assert.AreEqual(1, dt.Rows.Count);
                Assert.AreEqual("jgett", dt.Rows[0]["UserName"]);
            }
        }
    }
}
