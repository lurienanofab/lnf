using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Tests.Data
{
    [TestClass]
    public class RoomDataCleanTests : TestBase
    {
        [TestMethod]
        public void RoomDataCleanGenerator()
        {
            var sw = Stopwatch.StartNew();

            var g = new RoomDataCleanGenerator(
                DateTime.Parse("2015-07-01"),
                DateTime.Parse("2015-08-01")
                //DA.Current.Single<Client>(1759),
                //DA.Current.Single<Room>(6)
            );

            var result = g.Generate();
            

            //LNF.CommonTools.WriteRoomDataManager.Create(DateTime.Parse("2015-08-01"), DateTime.Parse("2015-09-01")).WriteRoomDataClean();

            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString());
        }
    }
}
