using System;
using System.Data;
using System.Linq;
using LNF.CommonTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests
{
    [TestClass]
    public class ReadToolDataManagerTests : TestBase
    {
        [TestMethod]
        public void CanReadToolDataFiltered()
        {
            DateTime startDate = DateTime.Parse("2017-07-01");
            DateTime endDate = DateTime.Parse("2017-08-01");
            int clientId = 1296;
            int resourceId = 14021;

            ReadToolDataManager reader = new ReadToolDataManager();
            DataTable dtSource = reader.ReadToolDataFiltered(startDate, endDate, clientId, resourceId);
            var rows = dtSource.Select($"ReservationID = {757744}");
            Assert.AreEqual(1, rows.Length);
            var ot = rows[0].Field<double>("OverTime");
            Assert.AreEqual(0.083333, ot);
        }
    }
}
