using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Tests
{
    [TestClass]
    public class DataAccessTests : TestBase
    {
        [TestMethod]
        public void DataAccessTests_CanGetClient()
        {
            using (Providers.DataAccess.StartUnitOfWork())
            {
                var c = DA.Current.Single<Client>(1301);
                Assert.AreEqual("jgett", c.UserName);
            }

            using (Providers.DataAccess.StartUnitOfWork())
            {
                var c = DA.Current.Single<Client>(1301);
                Assert.AreEqual("jgett", c.UserName);
            }
        }

        [TestMethod]
        public void DataAccessTests_CanDeleteWithQuery()
        {
            DateTime period = DateTime.Parse("2016-02-01");
            int? clientId = 162;
            int? resourceId = null;

            int result = DA.Current.SqlResult<int>(
                "DELETE ToolData WHERE Period = :period AND ClientID = ISNULL(:clientId, ClientID) AND ResourceID = ISNULL(:resourceId, ResourceID);SELECT @@ROWCOUNT;",
                new { period, clientId, resourceId });

            Assert.AreEqual(27, result);
        }

        [TestMethod]
        public void DataAccessTests_CanListWithQuery()
        {
            IList<ToolData> list = DA.Current.SqlQuery<ToolData>("SELECT * FROM ToolData WHERE Period = :period", new { period = DateTime.Parse("2016-02-01") });
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void DataAccessTests_CanUseUniversalPassword()
        {
            bool condition = ClientUtility.CheckPassword(1301, Providers.DataAccess.UniversalPassword);
            Assert.IsTrue(condition);
        }
    }
}
