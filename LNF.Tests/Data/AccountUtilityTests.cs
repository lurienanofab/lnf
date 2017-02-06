using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Tests.Data
{
    [TestClass]
    public class AccountUtilityTests : TestBase
    {
        [TestMethod]
        public void SelectActiveInDateRange()
        {
            var query = AccountUtility.FindActiveInDateRange(2697, DateTime.Parse("2015-02-01"), DateTime.Parse("2015-03-01"));
        }
    }
}
