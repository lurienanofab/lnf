using System;
using System.Data;
using System.Linq;
using LNF.Impl.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests
{
    [TestClass]
    public class WriteToolDataProcessTests : TestBase
    {
        [TestMethod]
        public void CanComputeCorrectTransferDuration()
        {
            DateTime period = DateTime.Parse("2018-05-01");
            int clientId = 1759;
            int resourceId = 14021;
            int reservationId = 833138;

            using (var conn = NewConnection())
            {
                WriteToolDataProcess proc = new WriteToolDataProcess(new WriteToolDataConfig
                {
                    Connection = conn,
                    Context = "LNF.Tests.WriteToolDataProcessTests.CanComputeCorrectTransferDuration",
                    Period = period,
                    ClientID = clientId,
                    ResourceID = resourceId
                });

                var dtToolDataClean = proc.Extract();
                var durations = proc.GetReservationDurations(dtToolDataClean);
                proc.ProcessCleanData(dtToolDataClean);
                proc.CalculateTransferTime(dtToolDataClean, durations);

                var rows = dtToolDataClean.Select($"ReservationID = {reservationId}");

                var actual = rows.Sum(dr => dr.Field<double>("TransferredDuration"));

                Assert.AreEqual(0, actual);
            }
        }
    }
}
