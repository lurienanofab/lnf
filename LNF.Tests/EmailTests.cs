using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Email;
using LNF.CommonTools;

namespace LNF.Tests
{
    [TestClass]
    public class EmailTests
    {
        [TestMethod]
        public void EmailTests_CanSendMessage()
        {
            using (Providers.DataAccess.StartUnitOfWork())
            {
                SendMessageResult result = Providers.Email.SendMessage(1301, "LNF.Tests.EmailTests.EmailTests_CanSendMessage()", "Test Message", "This is a test.", SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                result.Assert();
                Assert.IsTrue(result.Success);
            }
        }
    }
}
