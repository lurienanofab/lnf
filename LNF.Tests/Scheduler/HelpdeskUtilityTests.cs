using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests.Scheduler
{
    [TestClass]
    public class HelpdeskUtilityTests : TestBase
    {
        [TestMethod]
        public void CanGetSchedulerHelpdeskUrl()
        {
            string result = HelpdeskUtility.GetSchedulerHelpdeskUrl("ssel-sched.eecs.umich.edu", 62020);
            Assert.AreEqual("http://ssel-sched.eecs.umich.edu/scheduler/resource/62020", result);
        }
    }
}
