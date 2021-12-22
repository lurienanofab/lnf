using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace OnlineServices.Api.Tests.Scheduler
{
    [TestClass]
    public class ResourceRepositoryTests : OnlineServicesApiTest
    {
        [TestMethod]
        public void CanGetResources()
        {
            IEnumerable<IResource> resources;

            resources = Provider.Scheduler.Resource.GetResources();
            Assert.AreEqual(ResourceCount, resources.Count());

            resources = Provider.Scheduler.Resource.GetResources(new[] { 10020, 62020 });
            Assert.AreEqual(2, resources.Count());
            Assert.AreEqual("LAM 9400", resources.First(x => x.ResourceID == 10020).ResourceName);
            Assert.AreEqual("EnerJet Evaporator", resources.First(x => x.ResourceID == 62020).ResourceName);
        }
    }
}
