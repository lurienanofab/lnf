using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Tests.Data
{
    [TestClass]
    public class ClientTests : TestBase
    {
        [TestMethod]
        public void ClientTests_CanGetSingle()
        {
            Client c = DA.Current.Single<Client>(1301);
            Assert.AreEqual("jgett", c.UserName);
            Assert.AreEqual(true, c.Active);
        }

        [TestMethod]
        public void ClientTests_FormatStringTest()
        {
            DateTime d = DateTime.Now;
            string test0 = string.Format("The date is {0:yyyy-MM-dd}", d);

            double totalRoomCharge;
            string test1, test2;

            string format1 = "Total room usage fees: {0:$,#,##0.00}";
            string format2 = "Total room usage fees: {0:$#,##0.00}";

            totalRoomCharge = 0;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);

            totalRoomCharge = 1;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);

            totalRoomCharge = 123456;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);

            totalRoomCharge = 123456.123456;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);

            totalRoomCharge = 123456789.123456;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);

            totalRoomCharge = -123456789.123456;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);

            totalRoomCharge = 9999999999999999999;
            test1 = string.Format(format1, totalRoomCharge);
            test2 = string.Format(format2, totalRoomCharge);
            Assert.AreEqual(test1, test2);
            

            //Total room usage fees: $123,456,789.12
        }
    }
}
