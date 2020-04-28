using LNF.Impl.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

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

            using (var conn = NewConnection())
            {
                conn.Open();
                var proc = new WriteToolDataCleanProcess(conn, sd, ed, clientId);
                //var dtExtract = proc.Extract();
                //var dtTransform = proc.Transform(dtExtract);
                proc.Start();
                conn.Close();
            }
        }

        [TestMethod]
        public void CanWriteToolData()
        {
            DateTime period = DateTime.Parse("2017-07-01");
            int clientId = 1296;

            using (var conn = NewConnection())
            {
                conn.Open();
                var proc = new WriteToolDataProcess(conn, period, clientId);
                
                //var dtExtract = proc.Extract();
                //var dtTransform = proc.Transform(dtExtract);

                //var rows = dtTransform.Select($"ReservationID = {757744}");
                //Assert.AreEqual(1, rows.Length);
                //var ot = rows[0].Field<double>("OverTime");
                //Assert.AreEqual(0.083333, ot);

                proc.Start();

                conn.Close();
            }
        }
    }
}
