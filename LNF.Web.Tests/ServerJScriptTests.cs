using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Web.Tests
{
    [TestClass]
    public class ServerJScriptTests
    {
        [TestMethod]
        public void CanJSEncode()
        {
            Assert.AreEqual("hello\\nworld", LNF.Web.ServerJScript.JSEncode("hello\nworld"));
            Assert.AreEqual("hello\\nworld", LNF.Web.ServerJScript.JSEncode("hello\r\nworld"));
            Assert.AreEqual("hello\\nworld", LNF.Web.ServerJScript.JSEncode("hello" + Environment.NewLine + "world"));
            Assert.AreEqual("hello\\nworld\\tworks!", LNF.Web.ServerJScript.JSEncode("hello\nworld\tworks!"));

            // note: when "hello\\nworld" is rendered in the browser and executed by javascript
            // each word will appear on a separate line (which is what we want)
        }
    }
}
