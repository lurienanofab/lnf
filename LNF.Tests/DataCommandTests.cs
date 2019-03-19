using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Repository;
using System.Data;

namespace LNF.Tests
{
    [TestClass]
    public class DataCommandTests
    {
        [TestMethod]
        public void DoesItWork()
        {
            var dt = DA.Command(CommandType.Text).Param("ClientID", 1301).FillDataTable("SELECT UserName FROM dbo.Client WHERE ClientID = @ClientID");
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("jgett", dt.Rows[0]["UserName"]);
        }
    }
}
