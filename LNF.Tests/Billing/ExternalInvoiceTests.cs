using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Tests.Billing
{
    [TestClass]
    public class ExternalInvoiceTests : TestBase
    {
        [TestMethod]
        public void ExternalInvoiceTests_CanGetDeptRef()
        {
            Org org;
            string actual;

            org = DA.Current.Single<Org>(134);
            Assert.AreEqual("Abbott Point of Care", org.OrgName);
            actual = ExternalInvoiceUtility.GetDeptRef(org, DateTime.Parse("2014-02-01"), DateTime.Parse("2014-03-01"));
            Assert.AreEqual("N003407", actual);

            org = DA.Current.Single<Org>(200);
            Assert.AreEqual("Jet Propulsion Lab", org.OrgName);
            actual = ExternalInvoiceUtility.GetDeptRef(org, DateTime.Parse("2014-02-01"), DateTime.Parse("2014-03-01"));
            Assert.AreEqual("F035380", actual);
        }
    }
}
