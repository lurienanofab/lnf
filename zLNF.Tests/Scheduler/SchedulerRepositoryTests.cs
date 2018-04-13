using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests.Scheduler
{
    [TestClass]
    public class SchedulerRepositoryTests
    {
        [TestMethod]
        public void CanGetSchedulerRepository()
        {
            var repo = IOC.Container.GetInstance<ISchedulerRepository>();
            Assert.IsNotNull(repo);
        }
    }
}
