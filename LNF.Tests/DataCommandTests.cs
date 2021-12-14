using LNF.DataAccess;
using LNF.Impl.DataAccess;
using LNF.Impl.DependencyInjection;
using LNF.Impl.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace LNF.Tests
{
    [TestClass]
    public class DataCommandTests
    {
        [TestMethod]
        public void DoesItWork()
        {

            var ctx = ContainerContextFactory.Current.NewThreadScopedContext();
            var cfg = new ThreadStaticContainerConfiguration(ctx);
            cfg.RegisterAllTypes();

            using (var uow = ctx.GetInstance<IUnitOfWork>())
            {
                var session = ctx.GetInstance<ISessionManager>().Session;
                var dt = session.Command(CommandType.Text).Param("ClientID", 1301).FillDataTable("SELECT UserName FROM dbo.Client WHERE ClientID = @ClientID");
                Assert.AreEqual(1, dt.Rows.Count);
                Assert.AreEqual("jgett", dt.Rows[0]["UserName"]);
            }
        }
    }
}
