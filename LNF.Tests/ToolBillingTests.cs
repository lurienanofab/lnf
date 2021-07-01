using LNF.Impl.Repository.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class ToolBillingTests : TestBase
    {
        [TestMethod]
        public void CanGetActivatedUnused()
        {
            var items = Session.Query<ToolBilling>().Where(x => x.ReservationID == 833138).ToList();

            Assert.IsTrue(items.Count == 1);

            var tb = items.First();

            Assert.AreEqual(TimeSpan.Zero, tb.UnstartedUnused());
        }
    }
}
