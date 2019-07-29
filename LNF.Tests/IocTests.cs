using LNF.Data;
using LNF.Impl.Context;
using LNF.Impl.DependencyInjection.Default;
using LNF.Models.Billing;
using LNF.Repository;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class IocTests
    {
        [TestMethod]
        public void CanGetClient()
        {
            var ioc = new IOC();
            using (ioc.Resolver.GetInstance<IUnitOfWork>())
            {
                var session = ioc.Resolver.GetInstance<ISession>();
                var client = session.Single<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                session = ioc.Resolver.GetInstance<ISession>();
                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            ServiceProvider.Configure(ioc.Resolver);

            using (DA.StartUnitOfWork())
            {
                var client = DA.Current.Single<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = DA.Current.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            using (DA.StartUnitOfWork())
            {
                var client = DA.Current.Single<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = DA.Current.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }
        }

        [TestMethod]
        public void CanSelectToolBilling()
        {
            var ioc = new IOC();
            using (ioc.Resolver.GetInstance<IUnitOfWork>())
            {
                var repo = ioc.Resolver.GetInstance<IToolBillingManager>();
                var results = repo.SelectToolBilling(DateTime.Parse("2017-02-01"));
                Assert.IsTrue(results.Count() > 0);
            }
        }
    }
}
