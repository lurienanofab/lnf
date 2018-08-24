using System;
using LNF.CommonTools;
using LNF.Impl.DependencyInjection.Default;
using LNF.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests
{
    [TestClass]
    public class WriteToolDataManagerTests : TestBase
    {
        [TestMethod]
        public void CanWriteToolDataClean()
        {
            DateTime startPeriod = DateTime.Parse("2017-07-01");
            DateTime endPeriod = DateTime.Parse("2017-08-01");
            int clientId = 1296;
            int record = 14021;

            var mgr = WriteToolDataManager.Create(startPeriod, endPeriod, clientId, record);
            mgr.WriteToolDataClean();
        }
    }
}
