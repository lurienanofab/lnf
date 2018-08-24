using LNF.Billing;
using LNF.CommonTools;
using LNF.Impl.DependencyInjection.Default;
using LNF.Repository;
using LNF.Repository.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class BillingDataProcessStep1Tests : TestBase
    {
        [TestMethod]
        public void CanCalculateToolBillingCharges1()
        {
            IToolBilling[] source = BillingDataProcessStep1.GetToolData(DateTime.Parse("2017-07-01"), 0, 761251, false);
            Assert.AreEqual(1, source.Length);
            IToolBilling tb = source[0];
            BillingDataProcessStep1.CalculateToolBillingCharges(tb, false);
            Assert.AreEqual(0, tb.UsageFeeCharged);
            Assert.AreEqual(6, tb.BookingFee);
        }

        [TestMethod]
        public void CanCalculateToolBillingCharges2()
        {
            IToolBilling[] source = BillingDataProcessStep1.GetToolData(DateTime.Parse("2017-07-01"), 0, 761462, false);
            Assert.AreEqual(1, source.Length);
            IToolBilling tb = source[0];
            BillingDataProcessStep1.CalculateToolBillingCharges(tb, false);
            Assert.AreEqual(0, tb.UsageFeeCharged);
            Assert.AreEqual(5.25M, tb.BookingFee);
        }

        [TestMethod]
        public void CanCalculateToolBillingCharges3()
        {
            IToolBilling[] source = BillingDataProcessStep1.GetToolData(DateTime.Parse("2018-07-01"), 0, 844646, false);
            Assert.AreEqual(2, source.Length);
            BillingDataProcessStep1.CalculateToolBillingCharges(source[0], false);
            BillingDataProcessStep1.CalculateToolBillingCharges(source[1], false);
        }

        [TestMethod]
        public void CanCalculateBookingFee()
        {
            IToolBilling[] source = BillingDataProcessStep1.GetToolData(DateTime.Parse("2017-07-01"), 0, 761767, false);
            Assert.AreEqual(1, source.Length);
            IToolBilling tb = source[0];
            DA.Use<IToolBillingManager>().CalculateBookingFee(tb);
            Assert.AreEqual(0.68M, tb.BookingFee);
        }

        [TestMethod]
        public void CanGetToolBillingLineCost()
        {
            //IToolBilling tb = DA.Current.Query<ToolBilling>().First(x => x.ReservationID == 759293);
            IToolBilling tb = DA.Current.Query<ToolBilling>().First(x => x.ReservationID == 759305);
            BillingDataProcessStep1.CalculateToolBillingCharges(tb, false);
            Assert.AreEqual(1M, tb.UsageFeeCharged);
            var lineCost = DA.Use<IBillingTypeManager>().GetLineCost(tb);
            Assert.AreEqual(1M, lineCost);
        }
    }
}
