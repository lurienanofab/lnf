using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.CommonTools;

namespace LNF.Tests.Billing
{
    [TestClass]
    public class CommonTools : TestBase
    {
        [TestMethod]
        public void CommonTools_CanReadStoreDataClean()
        {
            ReadStoreDataManager mgr = ReadStoreDataManager.Create(DateTime.Parse("2014-05-01"), DateTime.Parse("2014-06-01"));
            var dt = mgr.ReadStoreDataClean(ReadStoreDataManager.StoreDataCleanOption.RechargeItems);
            Assert.IsTrue(dt.Rows.Count > 0);
        }
    }
}
