using LNF.Impl.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LNF.Tests
{
    [TestClass]
    public class ToolBillingManagerTests : TestBase
    {
        [TestMethod]
        public void CanSelectReservations()
        {
            DateTime sd = DateTime.Parse("2020-01-01");
            DateTime ed = DateTime.Parse("2020-02-01");
            int resourceId = 123456;

            var mgr = new ToolBillingRepository(SessionManager);
            mgr.SelectReservations(sd, ed, resourceId);
        }
    }
}
