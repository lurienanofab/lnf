using LNF.Impl.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace LNF.Tests
{
    [TestClass]
    public class ToolDataReaderTests : TestBase
    {
        [TestMethod]
        public void CanReadToolData()
        {
            DateTime period = DateTime.Parse("2017-07-01");
            int clientId = 1296;
            int reservationId = 757744;

            using (var conn = NewConnection())
            {
                conn.Open();
                var reader = new ToolDataReader(conn);
                var dtSource = reader.ReadToolData(period, clientId, reservationId);
                var rows = dtSource.Select($"ReservationID = 757744");
                Assert.AreEqual(1, rows.Length);
                var ot = rows[0].Field<decimal>("OverTime");
                Assert.AreEqual(0, ot);
                conn.Close();
            }
        }
    }
}
