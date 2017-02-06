using LNF.Cache;
using LNF.Models.Scheduler;
using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace LNF.Tests
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void CacheTests_CanGetUserState()
        {
            using (Providers.DataAccess.StartUnitOfWork())
            {
                var userState = CacheManager.Current.GetUserState(1301);
                Assert.AreEqual(1301, userState.ClientID);
            }
        }

        [TestMethod]
        public void CacheTests_CanDeleteUserState()
        {
            using (Providers.DataAccess.StartUnitOfWork())
            {
                CacheManager.Current.DeleteUserState(1301);

                var count = MongoRepository.Default.GetClient().GetDatabase("cachemgr")
                    .GetCollection<CacheObject<UserState>>("userState")
                    .Count(x => x.Value.ClientID == 1301);

                Assert.AreEqual(0, count);
            }
        }
    }
}
