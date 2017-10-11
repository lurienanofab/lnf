using LNF.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LNF.Tests.Web
{
    [TestClass]
    public class ServerJScriptTests
    {
        [TestMethod]
        public void CanJSEncode()
        {
            Assert.AreEqual("hello\\nworld", ServerJScript.JSEncode("hello\nworld"));
            Assert.AreEqual("hello\\nworld", ServerJScript.JSEncode("hello\r\nworld"));
            Assert.AreEqual("hello\\nworld", ServerJScript.JSEncode("hello" + Environment.NewLine + "world"));
            Assert.AreEqual("hello\\nworld\\tworks!", ServerJScript.JSEncode("hello\nworld\tworks!"));

            // note: when "hello\\nworld" is rendered in the browser and executed by javascript
            // each word will appear on a separate line (which is what we want)
        }
    }
}
