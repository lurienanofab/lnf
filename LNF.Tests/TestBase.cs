using LNF.Impl.Testing;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Linq;
using System.Security.Principal;

namespace LNF.Tests
{
    [TestClass]
    public class TestBase : NHibernateTestBase
    {
        [TestInitialize]
        public override void TestSetup()
        {
            base.TestSetup();
            log4net.Config.XmlConfigurator.Configure();
        }

        [TestCleanup]
        public void TestComplete()
        {
            //DA.Current.Dispose();
        }

        protected override void Prepare()
        {
        }

        protected void LogIn(string username)
        {
            var context = Providers.Context.Current;
            Client c = DA.Current.Query<Client>().FirstOrDefault(x => x.UserName == username);
            if (c != null)
            {
                context.Stub(x => x.User).Return(new GenericPrincipal(new GenericIdentity(username), c.Roles()));
                context.Stub(x => x.GetItem<Client>("CurrentUser")).Return(c);
            }
            else
                throw new Exception("Log in failed because UserName \"" + username + "\" was not found.");
        }
    }
}
