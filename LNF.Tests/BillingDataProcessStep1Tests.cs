using LNF.Impl.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class BillingDataProcessStep1Tests : TestBase
    {
        [TestMethod]
        public void CanCalculateToolBillingCharges1()
        {
            var source = Provider.Billing.Tool.GetToolBilling(761251).ToArray();
            Assert.AreEqual(1, source.Length);

            var tb = source[0];
            ToolBillingUtility.CalculateToolBillingCharges(tb);
            Assert.AreEqual(0, tb.UsageFeeCharged);
            Assert.AreEqual(6, tb.BookingFee);
        }

        [TestMethod]
        public void CanCalculateToolBillingCharges2()
        {
            var source = Provider.Billing.Tool.GetToolBilling(761462).ToArray();
            Assert.AreEqual(1, source.Length);

            var tb = source[0];
            ToolBillingUtility.CalculateToolBillingCharges(tb);
            Assert.AreEqual(0, tb.UsageFeeCharged);
            Assert.AreEqual(0, tb.BookingFee); //BookingFee sould be 0 because 100% was transferred to other reservations
        }

        [TestMethod]
        public void CanCalculateBookingFee()
        {
            var source = Provider.Billing.Tool.GetToolBilling(983455).ToArray();
            Assert.AreEqual(1, source.Length);

            var tb = source[0];
            Provider.Billing.Tool.CalculateBookingFee(tb);
            Assert.AreEqual(0.6320M, tb.BookingFee);
        }

        [TestMethod]
        public void CanGetToolBillingLineCost()
        {
            var tb = Provider.Billing.Tool.GetToolBilling(759305).First();

            ToolBillingUtility.CalculateToolBillingCharges(tb);
            Assert.AreEqual(4.32M, tb.UsageFeeCharged);

            var lineCost = ToolBillingUtility.GetLineCost(tb);
            Assert.AreEqual(12.7575M, lineCost);
        }
    }
}
