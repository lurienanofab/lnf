using LNF.Data;
using LNF.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace OnlineServices.Api.Tests.Mail
{
    [TestClass]
    public class MailServiceTests : OnlineServicesApiTest
    {
        [TestMethod]
        public void CanSendMassEmail()
        {
            int sent = Provider.Mail.SendMassEmail(new MassEmailSendArgs
            {
                ClientID = 1301,
                Caller = "OnlineServices.Api.Tests.MailServiceTests.CanSendMassEmail",
                Subject = "Test Email",
                Body = "This is a test.",
                From = "jgett@umich.edu",
                DisplayName = "Getty, James",
                Group = "privilege",
                Values = new[] { 2 }
            });

            Assert.AreEqual(ActiveStaffCount, sent);
        }

        [TestMethod]
        public void CanSendMessage()
        {
            Provider.Mail.SendMessage(new SendMessageArgs
            {
                ClientID = 1301,
                Caller = "OnlineServices.Api.Tests.MailServiceTests.CanSendMessage",
                Subject = "Test message",
                Body = "Hello world!",
                From = "jgett@umich.edu",
                DisplayName = "Getty, James",
                To = new[] { "jgett@hotmail.com" },
                IsHtml = false
            });
        }

        [TestMethod]
        public void CanGetMessages()
        {
            var messages = Provider.Mail.GetMessages(DateTime.Parse("2021-01-01"), DateTime.Parse("2021-01-02"), 0);
            Assert.AreEqual(587, messages.Count());
        }

        [TestMethod]
        public void CanGetMessage()
        {
            var m = Provider.Mail.GetMessage(637310);
            Assert.AreEqual("2021-01-01 00:00:11", $"{m.SentOn:yyyy-MM-dd HH:mm:ss}");
        }

        [TestMethod]
        public void CanGetRecipients()
        {
            var r = Provider.Mail.GetRecipients(637310);
            Assert.AreEqual("jgett@umich.edu", r.First().AddressText);
        }

        [TestMethod]
        public void GetEmailListByPrivilege()
        {
            var emails = Provider.Mail.GetEmailListByPrivilege(ClientPrivilege.Staff);
            Assert.AreEqual(ActiveStaffCount, emails.Count());
        }
    }
}
