using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Data.Tests
{
    [TestClass()]
    public class AccessCheckTests
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
            _uow.Dispose();
        }

        [TestMethod()]
        public void AccessCheck_CreateTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            Assert.IsNotNull(check);
            Assert.IsNotNull(check.Client);
            Assert.AreEqual("test", check.Client.UserName);
        }

        [TestMethod()]
        public void AccessCheck_CanEnableAccessTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            var value = check.CanEnableAccess();
            Assert.IsFalse(value);
        }

        [TestMethod()]
        public void AccessCheck_GetBadgesTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            var badges = check.GetBadges();
            Assert.AreEqual(1, badges.Count());

            c = DA.Current.Single<Client>(1301);
            check = AccessCheck.Create(c);
            badges = check.GetBadges();
            Assert.AreEqual(1, badges.Count());

            // always should be one badge (each badge may have multiple cards)
        }

        [TestMethod()]
        public void AccessCheck_IsPhysicalAccessEnabledTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            Assert.IsFalse(check.IsPhysicalAccessEnabled());

            c = DA.Current.Single<Client>(1301);
            check = AccessCheck.Create(c);
            Assert.IsTrue(check.IsPhysicalAccessEnabled());
        }

        [TestMethod()]
        public void AccessCheck_AllowReenableTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            var value = check.AllowReenable();
            Assert.IsFalse(value);

            c = DA.Current.Single<Client>(1301);
            check = AccessCheck.Create(c);
            value = check.AllowReenable();
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void AccessCheck_RemovePhysicalAccessPrivTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            check.RemovePhysicalAccessPriv();
            Assert.IsFalse(check.HasPhysicalAccessPriv);
        }

        [TestMethod()]
        public void AccessCheck_RemoveStoreUserPrivTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            check.RemoveStoreUserPriv();
            Assert.IsFalse(check.HasStoreUserPriv);
            check.Client.AddPriv(ClientPrivilege.StoreUser);
        }

        [TestMethod()]
        public void AccessCheck_EnablePhysicalAccessTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            check.EnablePhysicalAccess();

            var badge = check.GetBadges().First();
            Assert.IsFalse(badge.IsExpired());

            // reset
            ResetTestClient(check);
        }

        [TestMethod()]
        public void AccessCheck_DisablePhysicalAccessTest()
        {
            var c = DA.Current.Single<Client>(1600);
            var check = AccessCheck.Create(c);
            check.DisablePhysicalAccess();
            var now = RoundDateToSeconds(DateTime.Now);

            var badge = check.GetBadges().First();
            Assert.IsTrue(badge.IsExpired());
            Assert.AreEqual(RoundDateToSeconds(badge.ExpireDate), now);

            // reset
            ResetTestClient(check);
        }

        private DateTime RoundDateToSeconds(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
        }

        private void ResetTestClient(AccessCheck check)
        {
            DateTime expireOn = DateTime.Parse("2011-12-08 09:56:20");
            Providers.PhysicalAccess.DisableAccess(check.Client, expireOn);
            var badge = check.GetBadges().First();
            Assert.AreEqual(RoundDateToSeconds(badge.ExpireDate), expireOn);
        }
    }
}