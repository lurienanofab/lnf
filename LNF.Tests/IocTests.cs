using LNF.Billing;
using LNF.DataAccess;
using LNF.Impl;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class IocTests
    {
        private DependencyResolver _resolver;

        [TestMethod]
        public void CanGetClient()
        {
            _resolver = new ThreadStaticResolver();

            using (_resolver.GetInstance<IUnitOfWork>())
            {
                var session = _resolver.GetInstance<ISessionManager>().Session;
                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            ServiceProvider.Setup(_resolver.GetInstance<IProvider>());

            using (_resolver.GetInstance<IUnitOfWork>())
            {
                var client = ServiceProvider.Current.Data.Client.GetClient(1301);
                Assert.AreEqual("jgett", client.UserName);
            }

            using (_resolver.GetInstance<IUnitOfWork>())
            {
                var session = _resolver.GetInstance<ISessionManager>().Session;

                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            using (_resolver.GetInstance<IUnitOfWork>())
            {
                var session = _resolver.GetInstance<ISessionManager>().Session;

                var client = session.Get<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }
        }

        [TestMethod]
        public void CanSelectToolBilling()
        {
            _resolver = new ThreadStaticResolver();

            using (_resolver.GetInstance<IUnitOfWork>())
            {
                var repo = _resolver.GetInstance<IToolBillingRepository>();
                var results = repo.SelectToolBilling(DateTime.Parse("2017-02-01"));
                Assert.IsTrue(results.Count() > 0);
            }
        }
    }
}
