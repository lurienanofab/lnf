using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class BillingDataProcessStep1Tests : TestBase
    {
        [TestMethod]
        public void CanCalculateToolBillingCharges1()
        {
            var source = ServiceProvider.Current.Billing.Tool.GetToolBilling(761251).ToArray();
            Assert.AreEqual(1, source.Length);
            var tb = source[0];
            var step1 = new BillingDataProcessStep1(DateTime.Parse("2017-07-01"), ServiceProvider.Current);
            step1.CalculateToolBillingCharges(tb);
            Assert.AreEqual(0, tb.UsageFeeCharged);
            Assert.AreEqual(6, tb.BookingFee);
        }

        [TestMethod]
        public void CanCalculateToolBillingCharges2()
        {
            var source = ServiceProvider.Current.Billing.Tool.GetToolBilling(761462).ToArray();
            Assert.AreEqual(1, source.Length);
            var tb = source[0];
            var step1 = new BillingDataProcessStep1(DateTime.Parse("2017-07-01"), ServiceProvider.Current);
            step1.CalculateToolBillingCharges(tb);
            Assert.AreEqual(0, tb.UsageFeeCharged);
            Assert.AreEqual(5.25M, tb.BookingFee);
        }

        [TestMethod]
        public void CanCalculateToolBillingCharges3()
        {
            var source = ServiceProvider.Current.Billing.Tool.GetToolBilling(844646).ToArray();
            Assert.AreEqual(2, source.Length);
            var step1 = new BillingDataProcessStep1(DateTime.Parse("2017-07-01"), ServiceProvider.Current);
            step1.CalculateToolBillingCharges(source[0]);
            step1.CalculateToolBillingCharges(source[1]);
        }

        [TestMethod]
        public void CanCalculateBookingFee()
        {
            var source = ServiceProvider.Current.Billing.Tool.GetToolBilling(761767).ToArray();
            Assert.AreEqual(1, source.Length);
            var tb = source[0];
            ServiceProvider.Current.Billing.Tool.CalculateBookingFee(tb);
            Assert.AreEqual(0.68M, tb.BookingFee);
        }

        [TestMethod]
        public void CanGetToolBillingLineCost()
        {
            var tb = ServiceProvider.Current.Billing.Tool.GetToolBilling(759305).First();
            var step1 = new BillingDataProcessStep1(DateTime.Now, ServiceProvider.Current);
            step1.CalculateToolBillingCharges(tb);
            Assert.AreEqual(1M, tb.UsageFeeCharged);
            var lineCost = ServiceProvider.Current.Billing.BillingType.GetLineCost(tb);
            Assert.AreEqual(1M, lineCost);
        }
    }
}
