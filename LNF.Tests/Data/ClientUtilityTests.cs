using LNF.Data;
using LNF.Models.Data;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Tests.Data
{
    [TestClass]
    public class ClientUtilityTests
    {
        private LNF.Repository.IUnitOfWork uow;

        [TestInitialize]
        public void TestSetup()
        {
            uow = Providers.DataAccess.StartUnitOfWork();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            uow.Dispose();
        }

        [TestMethod]
        public void CanGetActiveClients()
        {
            DateTime sd = DateTime.Parse("2016-01-01");
            DateTime ed = DateTime.Parse("2017-01-01");
            ClientPrivilege priv = ClientPrivilege.LabUser;

            IList<Client> clients;

            clients = ClientUtility.GetActiveClients(sd, ed, priv);
            Assert.AreEqual(646, clients.Count);

            clients = ClientUtility.GetActiveClients(sd, ed);
            Assert.AreEqual(1558, clients.Count);

            clients = ClientUtility.GetActiveClients(priv);
            Assert.AreEqual(517, clients.Count);

            clients = ClientUtility.GetActiveClients();
            Assert.AreEqual(1314, clients.Count);
        }

        [TestMethod]
        public void CanCheckPassword()
        {
            bool result = ClientUtility.CheckPassword(1600, "test");
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CanGetActiveAccountCount()
        {
            int result = ClientUtility.GetActiveAccountCount(1600);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CanLogin()
        {
            Client client;
            
            client = ClientUtility.Login("test", "test");
            Assert.IsNotNull(client);
            Assert.AreEqual(1600, client.ClientID);

            try
            {
                client = ClientUtility.Login("ztest", "test");
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Invalid username.", ex.Message);
            }

            try
            {
                client = ClientUtility.Login("test", "ztest");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid password.", ex.Message);
            }
        }

        [TestMethod]
        public void CanCleanMiddleName()
        {
            string result;

            result = ClientUtility.CleanMiddleName(null);
            Assert.AreEqual(string.Empty, result);

            result = ClientUtility.CleanMiddleName("");
            Assert.AreEqual(string.Empty, result);

            result = ClientUtility.CleanMiddleName(" H. ");
            Assert.AreEqual("H", result);

            result = ClientUtility.CleanMiddleName("H");
            Assert.AreEqual("H", result);
        }

        [TestMethod]
        public void CanUpdatePhysicalAccess()
        {
            Client c;
            AccessCheck check;

            c = ClientUtility.Find(1600);
            check = ClientUtility.UpdatePhysicalAccess(c);
            string reason;

            Assert.IsTrue(!check.AllowReenable());
            Assert.IsTrue(!check.CanEnableAccess(out reason));
            Assert.IsTrue(check.HasActiveAccounts);
            Assert.IsTrue(check.HasLabUserPriv);
            Assert.IsTrue(!check.HasPhysicalAccessPriv);
            Assert.IsTrue(!check.HasStoreUserPriv);
            Assert.IsTrue(check.IsActive);
            Assert.IsTrue(!check.IsPhysicalAccessEnabled());

            c = ClientUtility.Find(1301);
            check = ClientUtility.UpdatePhysicalAccess(c);
            Assert.IsTrue(check.AllowReenable());
            Assert.IsTrue(check.CanEnableAccess(out reason));
            Assert.IsTrue(check.HasActiveAccounts);
            Assert.IsTrue(!check.HasLabUserPriv);
            Assert.IsTrue(check.HasPhysicalAccessPriv);
            Assert.IsTrue(check.HasStoreUserPriv);
            Assert.IsTrue(check.IsActive);
            Assert.IsTrue(check.IsPhysicalAccessEnabled());
        }

        [TestMethod]
        public void CanStoreClientInfo()
        {
            var c0 = ClientUtility.Find(1600);
            int clientId = c0.ClientID;
            var org = c0.PrimaryOrg();
            var co = c0.PrimaryClientOrg();
            var alert = string.Empty;
            var privs = PrivUtility.GetPrivs(c0.Privs).ToList();
            var comms = CommunityUtility.GetCommunities(c0.Communities).ToList();
            var clientManagers = co.ClientManagersWhereIsEmployee().Where(x => x.Active).ToList();
            var clientAccounts = co.ClientAccounts().Where(x => x.Active).ToList();

            var c1 = ClientUtility.StoreClientInfo(ref clientId, false, true, "User", "Test", null, "test", c0.DemCitizenID, c0.DemEthnicID, c0.DemRaceID, c0.DemGenderID, c0.DemDisabilityID, privs, comms, c0.TechnicalFieldID, org, co.Role, co.Department, co.Email, co.Phone, co.IsManager, co.IsFinManager, co.SubsidyStartDate, co.NewFacultyStartDate, null, null, clientManagers, clientAccounts, ref alert);
            Assert.AreEqual(c0, c1);

            var possibleAlerts = new
            {
                inactiveTooLong = "Note that this client has been inactive for so long that access is not automatically reenabled. Please see the Lab Manager.",
                noPhysicalAccess = "Store and physical access disabled. This client does not physical access.",
                noActiveAcounts = "Store and physical access disabled for this client - no active accounts."
            };

            Assert.AreEqual(possibleAlerts.noPhysicalAccess, alert);
        }
    }
}
