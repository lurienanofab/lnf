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
            DateTime sd = DateTime.Parse("2017-07-01");
            DateTime ed = DateTime.Parse("2017-08-01");
            int clientId = 1296;

            var proc = new WriteToolDataCleanProcess(sd, ed, clientId);
            var result = proc.Start();
        }
    }
}
