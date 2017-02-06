using LNF.Impl;
using LNF.Repository;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        public void ProviderTest_CanGetDataAccessProvider()
        {
            IDataAccessProvider prov = Providers.DataAccess;

            Assert.IsInstanceOfType(prov, typeof(NHibernateThreadStaticDataAccess));

            using (Providers.DataAccess.StartUnitOfWork())
            {
                var c = DA.Current.Query<Client>().FirstOrDefault(x => x.UserName == "jgett");
                Assert.AreEqual("Getty, James", c.DisplayName);
                Console.WriteLine(c.DisplayName);

                var a = DA.Current.Query<Account>().FirstOrDefault(x => x.ShortCode == "943777");
                Assert.AreEqual("LNF General Lab Operating Account", a.Name);
                Console.WriteLine(a.Name);
            }
        }
    }
}
