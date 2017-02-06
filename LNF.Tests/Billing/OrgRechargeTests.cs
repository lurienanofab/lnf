using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Billing;

namespace LNF.Tests.Billing
{
    [TestClass]
    public class OrgRechargeTests : TestBase
    {
        [TestMethod]
        public void OrgRechargeTests_CanGetSingle()
        {
            OrgRecharge item = DA.Current.Single<OrgRecharge>(3);
            Assert.AreEqual("Jet Propulsion Lab", item.Org.OrgName);
            Assert.AreEqual("LNF Federal Recharge - JPL", item.Account.Name);
        }

        [TestMethod]
        public void OrgRechargeTests_CanGetRechargeAccount()
        {
            Org org = DA.Current.Single<Org>(200);

            Assert.AreEqual("Jet Propulsion Lab", org.OrgName);

            Account acct;
                
            acct = OrgRechargeUtility.GetRechargeAccount(org, DateTime.Parse("2014-01-01"), DateTime.Parse("2014-02-01"));
            //should be the default recharge acct for external non-academic, because the OrgRecharge record was not enabled yet
            Assert.AreEqual(org.OrgType.ChargeType.GetAccount(), acct);

            acct = OrgRechargeUtility.GetRechargeAccount(org, DateTime.Parse("2014-02-01"), DateTime.Parse("2014-03-01"));
            //should be the recharge acct for JPL because the record was enabled in Feb
            Assert.AreEqual("LNF Federal Recharge - JPL", acct.Name);
        }
    }
}
