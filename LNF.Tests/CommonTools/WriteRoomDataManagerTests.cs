using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.CommonTools;

namespace LNF.Tests.CommonTools
{
    [TestClass]
    public class WriteRoomDataManagerTests
    {
        [TestMethod]
        public void CanWriteRoomData()
        {
            WriteRoomDataManager.Create(DateTime.Parse("2015-08-01"), DateTime.Parse("2015-09-01"), 53).WriteRoomData();
        }
    }
}
