using LNF.Models.Scheduler;
using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests.Scheduler
{
    [TestClass]
    public class ResourceClientTests : TestBase
    {
        [TestMethod]
        public void CanSelectEmailsByAuth()
        {
            string[] actual = ResourceClientUtility.SelectEmailsByAuth(82010, ClientAuthLevel.AuthorizedUser);
            //Assert.IsTrue(actual.Length == 3);
        }
    }
}
