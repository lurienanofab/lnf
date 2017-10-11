using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Models.Data;

namespace LNF.Tests.Data
{
    [TestClass]
    public class ClientUtilityTests
    {
        [TestMethod]
        public void CanGetActiveClients()
        {
            using (Providers.DataAccess.StartUnitOfWork())
            {
                DateTime sd = DateTime.Parse("2016-01-01");
                DateTime ed = DateTime.Parse("2017-01-01");
                ClientPrivilege priv = ClientPrivilege.LabUser;

                var clients = ClientUtility.GetActiveClients(sd, ed, priv);

                Assert.AreEqual(646, clients.Count);
            }
        }
    }
}
