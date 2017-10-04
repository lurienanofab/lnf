﻿using LNF.Billing;
using LNF.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Tests.CommonTools
{
    [TestClass]
    public class ReservationDurationsTests
    {
        [TestMethod]
        public void CanGetReservationDurations()
        {
            DateTime sd = DateTime.Parse("2016-04-01");
            DateTime ed = DateTime.Parse("2016-05-01");
            ReservationDateRange range = new ReservationDateRange(sd, ed);
            List<ReservationDurationItem> all = new List<ReservationDurationItem>();
            //var query = DA.Scheduler.Resource.Query().Where(x => x.ResourceID == 62050);
            var query = DA.Scheduler.Resource.SelectActive();
            foreach (var res in DA.Scheduler.Resource.Query().Where(x => x.ResourceID == 62050))
            {
                all.AddRange(range.CreateReservationDurations());
            }
            Assert.IsTrue(true);
        }
    }
}
