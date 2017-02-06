using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Scheduler;

namespace LNF.Tests.Scheduler
{
    [TestClass]
    public class OnTheFlyUtilityTests : TestBase
    {
        [TestMethod]
        public void CanGetStateDuration()
        {
            uint zero = 0;
            Assert.IsTrue(OnTheFlyUtility.GetStateDuration(80031) > 0);
            Assert.IsTrue(OnTheFlyUtility.GetStateDuration(80032) > 0);
            Assert.AreEqual(zero, OnTheFlyUtility.GetStateDuration(80030)); //is an OnTheFly resource but not a cabinet
            Assert.AreEqual(zero, OnTheFlyUtility.GetStateDuration(62020)); //not an OnTheFly resource
            Assert.AreEqual(zero, OnTheFlyUtility.GetStateDuration(-1)); //invalid ResourceID
        }
    }
}
