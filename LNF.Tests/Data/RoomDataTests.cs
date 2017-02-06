using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Tests.Data
{
    [TestClass]
    public class RoomDataTests : TestBase
    {
        [TestMethod]
        public void RoomDataPerformanceTest()
        {
            var sw = Stopwatch.StartNew();

            DateTime period = DateTime.Parse("2016-01-01");
            int clientId = 0;
            int roomId = 0;

            IList<RoomData> query;

            if (clientId > 0 && roomId > 0)
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.DataSource != 1 && x.Client.ClientID == clientId && x.Room.RoomID == roomId).ToList();
            else if (clientId > 0)
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.DataSource != 1 && x.Client.ClientID == clientId).ToList();
            else if (roomId > 0)
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.DataSource != 1 && x.Room.RoomID == roomId).ToList();
            else
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.DataSource != 1).ToList();

            sw.Stop();

            Console.WriteLine("done in {0} seconds", sw.Elapsed.TotalSeconds);
        }
    }
}
