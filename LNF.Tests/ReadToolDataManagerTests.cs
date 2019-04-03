using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace LNF.Tests
{
    [TestClass]
    public class ReadToolDataManagerTests : TestBase
    {
        [TestMethod]
        public void CanReadToolDataFiltered()
        {
            DateTime period = DateTime.Parse("2017-07-01");
            int clientId = 1296;
            int resourceId = 14021;

            var reader = ServiceProvider.Current.ReadToolDataManager;
            DataTable dtSource = reader.ReadToolData(period, clientId, resourceId);
            var rows = dtSource.Select($"ReservationID = {757744}");
            Assert.AreEqual(1, rows.Length);
            var ot = rows[0].Field<double>("OverTime");
            Assert.AreEqual(0.083333, ot);
        }
    }
}
