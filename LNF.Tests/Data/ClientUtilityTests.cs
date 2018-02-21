using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LNF.Tests.Data
{
    [TestClass]
    public class ClientUtilityTests
    {
        private static IUnitOfWork _uow;
        private static TestContext _context;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _context = context;
            _uow = Providers.DataAccess.StartUnitOfWork();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            // clean up;
            var c = Client.Find("newclient");
            if (c != null)
            {
                
                DA.Current.Delete(c);
                DA.Current.Delete(DA.Current.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.Record == c.ClientID));
            }

            _uow.Dispose();
        }

        [TestMethod]
        public void ClientUtility_GetActiveClientsTest()
        {
            DateTime sd = DateTime.Parse("2016-01-01");
            DateTime ed = DateTime.Parse("2017-01-01");
            ClientPrivilege priv = ClientPrivilege.LabUser;

            IEnumerable<Client> clients;

            clients = ClientUtility.GetActiveClients(sd, ed, priv);
            Assert.AreEqual(646, clients.Count());

            clients = ClientUtility.GetActiveClients(sd, ed);
            Assert.AreEqual(1558, clients.Count());

            clients = ClientUtility.GetActiveClients(priv);
            Assert.AreEqual(517, clients.Count());

            clients = ClientUtility.GetActiveClients();
            Assert.AreEqual(1314, clients.Count());
        }

        [TestMethod]
        public void ClientUtility_CheckPasswordTest()
        {
            bool result = ClientUtility.CheckPassword(1600, "test");
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ClientUtility_SetPasswordTest()
        {
            int result = ClientUtility.SetPassword(1600, "test");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClientUtility_GetActiveAccountCountTest()
        {
            int result = ClientUtility.GetActiveAccountCount(1600);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClientUtility_LoginTest()
        {
            Client client;

            client = ClientUtility.Login("test", "test");
            Assert.IsNotNull(client);
            Assert.AreEqual(1600, client.ClientID);

            try
            {
                client = ClientUtility.Login("ztest", "test");
            }
            catch (Exception ex)
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
        public void ClientUtility_CleanMiddleNameTest()
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
        public void ClientUtility_UpdatePhysicalAccessTest()
        {
            Client c;
            AccessCheck check;
            bool hasAccess;
            string alert;

            c = DA.Current.Single<Client>(1600);

            check = AccessCheck.Create(c);
            hasAccess = ClientUtility.UpdatePhysicalAccess(check, out alert);
            Assert.IsTrue(!check.AllowReenable());
            Assert.IsTrue(!check.CanEnableAccess());
            Assert.IsTrue(check.HasActiveAccounts);
            Assert.IsTrue(check.HasLabUserPriv);
            Assert.IsTrue(!check.HasPhysicalAccessPriv);
            Assert.IsTrue(check.HasStoreUserPriv);
            Assert.IsTrue(check.IsActive);
            Assert.IsTrue(!check.IsPhysicalAccessEnabled());
            _context.WriteLine("alert: {0}", alert);

            c = DA.Current.Single<Client>(1301);
            check = AccessCheck.Create(c);
            hasAccess = ClientUtility.UpdatePhysicalAccess(check, out alert);
            Assert.IsTrue(check.AllowReenable());
            Assert.IsTrue(check.CanEnableAccess());
            Assert.IsTrue(check.HasActiveAccounts);
            Assert.IsTrue(!check.HasLabUserPriv);
            Assert.IsTrue(check.HasPhysicalAccessPriv);
            Assert.IsTrue(check.HasStoreUserPriv);
            Assert.IsTrue(check.IsActive);
            Assert.IsTrue(check.IsPhysicalAccessEnabled());
            _context.WriteLine("alert: {0}", alert);
        }

        [TestMethod]
        public void ClientUtility_StoreClientInfoTest()
        {
            var c0 = DA.Current.Single<Client>(1600);

            c0.AddPriv(ClientPrivilege.PhysicalAccess);

            int clientId = c0.ClientID;
            var org = c0.PrimaryOrg();
            var co = c0.PrimaryClientOrg();
            var alert = string.Empty;
            var privs = PrivUtility.GetPrivs(c0.Privs).ToList();
            var comms = CommunityUtility.GetCommunities(c0.Communities).ToList();
            var clientManagers = co.ClientManagersWhereIsEmployee().Where(x => x.Active).ToList();
            var clientAccounts = co.ClientAccounts().Where(x => x.Active).ToList();
            var demographics = ClientDemographics.Create(c0);

            var c1 = ClientUtility.StoreClientInfo(
                clientId: ref clientId,
                lname: "User",
                fname: "Test",
                mname: null,
                username: "test",
                demographics: demographics,
                privs: privs,
                communities: comms,
                technicalFieldId: c0.TechnicalFieldID,
                org: org,
                role: co.Role,
                dept: co.Department,
                email: co.Email,
                phone: co.Phone,
                isManager: co.IsManager,
                isFinManager: co.IsFinManager,
                subsidyStart: co.SubsidyStartDate,
                newFacultyStart: co.NewFacultyStartDate,
                addedAddress: null,
                deletedAddress: null,
                clientManagers: clientManagers,
                clientAccounts: clientAccounts,
                alert: out alert);

            Assert.AreEqual(c0, c1);

            var possibleAlerts = new
            {
                noPhysicalAccessPrivAlreadyDisabled = "This client does not have the Physical Access privilege. Physical access was already disabled.",
                inactiveTooLongPhysicalAccessPrivRemoted = "Note that this client has been inactive for so long that access is not automatically reenabled. The Physical Access privilege has been removed."
            };

            //Assert.AreEqual(possibleAlerts.noPhysicalAccessPrivAlreadyDisabled, alert);
            Assert.AreEqual(possibleAlerts.inactiveTooLongPhysicalAccessPrivRemoted, alert);
            Assert.IsFalse(c0.HasPriv(ClientPrivilege.PhysicalAccess));
        }

        [TestMethod]
        public void ClientUtility_NewClientTest()
        {
            Client c = ClientUtility.NewClient("newclient", "newclient", "Client", "New", ClientPrivilege.LabUser, true);
            Assert.IsTrue(c.ClientID > 0);
            Assert.AreEqual("newclient", c.UserName);
        }

        [TestMethod]
        public void ClientUtility_FindByManagerTest()
        {
            IEnumerable<Client> clients;

            clients = Client.FindByManager(155);
            int activeCount = clients.Count();

            clients = Client.FindByManager(155, false);
            int inactiveCount = clients.Count();

            clients = Client.FindByManager(155, null);
            Assert.AreEqual(activeCount + inactiveCount, clients.Count());
        }

        [TestMethod]
        public void ClientUtility_FindByToolsTest()
        {
            IEnumerable<Client> clients;
            int[] tools = { 10020, 10030, 61081 };

            clients = Client.FindByTools(tools);
            int activeCount = clients.Count();

            clients = Client.FindByTools(tools, false);
            int inactiveCount = clients.Count();

            clients = Client.FindByTools(tools, null);
            Assert.AreEqual(activeCount + inactiveCount, clients.Count());
        }


        [TestMethod]
        public void ClientUtility_FindByCommunityTest()
        {
            IEnumerable<Client> clients;
            int flag = 1 | 2 | 8 | 16;

            clients = Client.FindByCommunity(flag);
            int activeCount = clients.Count();

            clients = Client.FindByCommunity(flag, false);
            int inactiveCount = clients.Count();

            clients = Client.FindByCommunity(flag, null);
            Assert.AreEqual(activeCount + inactiveCount, clients.Count());
        }

        [TestMethod]
        public void ClientUtility_FindByPrivilegeTest()
        {
            IEnumerable<Client> clients;
            ClientPrivilege privs = ClientPrivilege.LabUser;

            clients = Client.FindByPrivilege(privs);
            int activeCount = clients.Count();

            clients = Client.FindByPrivilege(privs, false);
            int inactiveCount = clients.Count();

            clients = Client.FindByPrivilege(privs, null);
            Assert.AreEqual(activeCount + inactiveCount, clients.Count());
        }

        [TestMethod]
        public void ClientUtility_FindByPeriodTest()
        {
            IEnumerable<Client> clients;
            DateTime period = DateTime.Parse("2017-06-01");
            Client test = DA.Current.Single<Client>(1600);

            // normal user, do not show all
            clients = Client.FindByPeriod(test, period);
            Assert.AreEqual(1, clients.Count()); //only itself

            clients = Client.FindByPeriod(test, period, true);
            Assert.AreEqual(1, clients.Count()); //only itself

            test.AddPriv(ClientPrivilege.Staff);

            // staff user, do not show all
            clients = Client.FindByPeriod(test, period);
            Assert.AreEqual(1, clients.Count()); //only itself

            // staff user, show all
            clients = Client.FindByPeriod(test, period, true);
            Assert.AreEqual(1059, clients.Count()); //everyone

            test.RemovePriv(ClientPrivilege.Staff);

            test.AddPriv(ClientPrivilege.Administrator);

            // admin user, do not show all
            clients = Client.FindByPeriod(test, period);
            Assert.AreEqual(1059, clients.Count()); //everyone

            // admin user, show all
            clients = Client.FindByPeriod(test, period, true);
            Assert.AreEqual(1059, clients.Count()); //everyone

            test.RemovePriv(ClientPrivilege.Administrator);
        }

        [TestMethod]
        public void ClientUtility_FindByDisplayNameTest()
        {
            var c = Client.FindByDisplayName("User, Test");
            Assert.IsNotNull(c);
            Assert.AreEqual("test", c.UserName);
        }

        [TestMethod]
        public void ClientUtility_FindTest()
        {
            Client c;

            c = DA.Current.Single<Client>(1600);
            Assert.IsNotNull(c);
            Assert.AreEqual("test", c.UserName);

            c = Client.Find("jgett");
            Assert.IsNotNull(c);
            Assert.AreEqual(1301, c.ClientID);
        }
    }
}
