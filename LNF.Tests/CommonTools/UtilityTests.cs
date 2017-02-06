using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Logging;

namespace LNF.Tests.CommonTools
{
    [TestClass]
    public class UtilityTests : TestBase
    {
        [TestMethod]
        public void CanLogToFile()
        {
            Logger.Write(TextLogMessage.Create("testing", "this is a test"));
            Logger.Write(TextLogMessage.Create("testing", "hello {0} {1}", "world", 123));
        }
    }
}
