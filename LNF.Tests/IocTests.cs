using LNF.Billing;
using LNF.DataAccess;
using LNF.DependencyInjection;
using LNF.Impl.DataAccess;
using LNF.Impl.DependencyInjection;
using LNF.Impl.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class IocTests
    {
        private IContainerContext _context;

        [TestMethod]
        public void CanGetClient()
        {
            ContainerContextFactory.Current.NewThreadScopedContext();
            _context = ContainerContextFactory.Current.GetContext();

            var cfg = new ThreadStaticContainerConfiguration(_context);
            cfg.RegisterAllTypes();

            using (_context.GetInstance<IUnitOfWork>())
            {
                var session = _context.GetInstance<ISessionManager>().Session;
                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            ServiceProvider.Setup(_context.GetInstance<IProvider>());

            using (_context.GetInstance<IUnitOfWork>())
            {
                var client = ServiceProvider.Current.Data.Client.GetClient(1301);
                Assert.AreEqual("jgett", client.UserName);
            }

            using (_context.GetInstance<IUnitOfWork>())
            {
                var session = _context.GetInstance<ISessionManager>().Session;

                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            using (_context.GetInstance<IUnitOfWork>())
            {
                var session = _context.GetInstance<ISessionManager>().Session;

                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }
        }

        [TestMethod]
        public void CanSelectToolBilling()
        {
            ContainerContextFactory.Current.NewThreadScopedContext();
            _context = ContainerContextFactory.Current.GetContext();

            var cfg = new ThreadStaticContainerConfiguration(_context);
            cfg.RegisterAllTypes();

            using (_context.GetInstance<IUnitOfWork>())
            {
                var repo = _context.GetInstance<IToolBillingRepository>();
                var results = repo.SelectToolBilling(DateTime.Parse("2017-02-01"));
                Assert.IsTrue(results.Count() > 0);
            }
        }
    }
}
