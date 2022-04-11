using System;
using System.Data;
using System.Linq;
using LNF.Data;
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
                var costs = Provider.Data.Cost.GetToolCosts(period, resourceId);
                WriteToolDataProcess proc = new WriteToolDataProcess(WriteToolDataConfig.Create(conn, "LNF.Tests.WriteToolDataProcessTests.CanComputeCorrectTransferDuration", period, clientId, resourceId, costs));

                var dtToolDataClean = proc.Extract();
                var transformer = proc.GetTransformer();
                transformer.ProcessCleanData(dtToolDataClean);
                transformer.CalculateTransferTime(dtToolDataClean);

                var rows = dtToolDataClean.Select($"ReservationID = {reservationId}");

                var actual = rows.Sum(dr => dr.Field<double>("TransferredDuration"));

                Assert.AreEqual(0, actual);
            }
        }
    }
}
