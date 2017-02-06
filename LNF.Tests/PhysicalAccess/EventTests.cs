using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF;
using LNF.PhysicalAccess;

namespace LNF.Tests.PhysicalAccess
{
    [TestClass]
    public class EventTests : TestBase
    {
        [TestMethod]
        public void CanGetEvents()
        {
            Providers.PhysicalAccess.GetEvents(DateTime.Parse("2015-08-01"), DateTime.Parse("2015-09-01"));

        }
    }
}
