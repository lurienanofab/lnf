using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.CommonTools;

namespace LNF.Tests.CommonTools
{
    [TestClass]
    public class BillingDataProcessStep1Tests
    {
        [TestMethod]
        public void CanCalculateUsageFeeCharged()
        {
            ToolBilling tb = DA.Current.Single<ToolBilling>(1829457);
            tb.CalculateUsageFeeCharged();
            Assert.AreEqual(92.07M, tb.UsageFeeCharged + tb.OverTimePenaltyFee);
        }

        [TestMethod]
        public void CanPopulateToolBilling()
        {
            DateTime period = DateTime.Parse("2015-10-01");
            BillingDataProcessStep1.PopulateToolBilling(period, 1113, false);
            ToolBilling tb = DA.Current.Query<ToolBilling>().FirstOrDefault(x => x.Period == period
                && x.ClientID == 1113
                && x.ResourceID == 14041
                && x.AccountID == 1264);
            Assert.AreEqual(92.07M, tb.UsageFeeCharged + tb.OverTimePenaltyFee);
        }
    }
}
