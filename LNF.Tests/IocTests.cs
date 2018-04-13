﻿using LNF.Impl.DependencyInjection;
using LNF.Impl.DependencyInjection.Default;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scheduler;
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
            using (IOC.Resolver.GetInstance<IUnitOfWork>())
            {
                var session = IOC.Resolver.GetInstance<ISession>();
                var client = session.Single<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                session = IOC.Resolver.GetInstance<ISession>();
                var co = session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();

            using (ServiceProvider.Current.DataAccess.StartUnitOfWork())
            {
                var client = DA.Current.Single<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = DA.Current.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }

            var svc = IOC.Resolver.GetInstance<IDataAccessService>();
            using (svc.StartUnitOfWork())
            {
                var client = svc.Session.Single<Client>(1301);
                Assert.AreEqual("jgett", client.UserName);

                var co = svc.Session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == 1301);
                Assert.AreEqual("jgett", co.Client.UserName);
            }
        }

        [TestMethod]
        public void CanGetToolCosts()
        {
            using (IOC.Resolver.GetInstance<IUnitOfWork>())
            {
                var repo = IOC.Resolver.GetInstance<ISchedulerRepository>();
                var toolCosts = repo.GetToolCosts(DateTime.Now, 10020);
                Assert.AreEqual(40, toolCosts.First().MulVal);
            }
        }

        [TestMethod]
        public void CanSelectToolBilling()
        {
            using (IOC.Resolver.GetInstance<IUnitOfWork>())
            {
                var repo = IOC.Resolver.GetInstance<Billing.ToolBillingManager>();
                var results = repo.SelectToolBilling(DateTime.Parse("2017-02-01"));
                Assert.IsTrue(results.Count() > 0);
            }
        }
    }
}