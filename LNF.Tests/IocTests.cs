using LNF.Billing;
using LNF.DataAccess;
using LNF.Impl;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class IocTests
    {
        private Container _container;

        [TestMethod]
        public void CanGetClient()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            var cfg = new ThreadStaticContainerConfiguration(_container);
            cfg.Configure();

            using (_container.GetInstance<IUnitOfWork>())
            {
                var session = _container.GetInstance<ISessionManager>().Session;
                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            ServiceProvider.Setup(_container.GetInstance<IProvider>());

            using (_container.GetInstance<IUnitOfWork>())
            {
                var client = ServiceProvider.Current.Data.Client.GetClient(1301);
                Assert.AreEqual("jgett", client.UserName);
            }

            using (_container.GetInstance<IUnitOfWork>())
            {
                var session = _container.GetInstance<ISessionManager>().Session;

                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            using (_container.GetInstance<IUnitOfWork>())
            {
                var session = _container.GetInstance<ISessionManager>().Session;

                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }
        }

        [TestMethod]
        public void CanSelectToolBilling()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            var cfg = new ThreadStaticContainerConfiguration(container);
            cfg.Configure();

            using (container.GetInstance<IUnitOfWork>())
            {
                var repo = container.GetInstance<IToolBillingRepository>();
                var results = repo.SelectToolBilling(DateTime.Parse("2017-02-01"));
                Assert.IsTrue(results.Count() > 0);
            }
        }
    }
}
