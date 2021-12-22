using LNF.Data;
using LNF.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace OnlineServices.Api.Tests.Mail
{
    [TestClass]
    public class MassEmailRepositoryTests : OnlineServicesApiTest
    {
        [TestMethod]
        public void CanGetRecipients()
        {
            var recipients = Provider.Mail.MassEmail.GetRecipients(new MassEmailRecipientArgs
            {
                Privs = Convert.ToInt32(ClientPrivilege.Staff)
            });

            Assert.AreEqual(ActiveStaffCount, recipients.Count());
        }

        [TestMethod]
        public void CanCreateSendMessageArgs()
        {
            var args = new MassEmailSendArgs
            {
                ClientID = 1301,
                Caller = "OnlineServices.Api.Tests.MassEmailRepositoryTests.CreateSendMessageArgs",
                Subject = "Test Email",
                Body = "Test email.",
                From = "jgett@umich.edu",
                DisplayName = "Getty, James",
                Group = "privilege",
                Values = new[] { Convert.ToInt32(ClientPrivilege.Staff) }
            };

            var sma = Provider.Mail.MassEmail.CreateSendMessageArgs(args);

            Assert.AreEqual(ActiveStaffCount, sma.To.Count());
        }

        [TestMethod]
        public void CanCreateCriteria()
        {
            var criteria = Provider.Mail.MassEmail.CreateCriteria(new MassEmail
            {
                RecipientGroup = "privilege",
                RecipientCriteria = JsonConvert.SerializeObject(new { SelectedPrivileges = Convert.ToInt32(ClientPrivilege.Staff) })
            });

            Assert.AreEqual("Staff", criteria.GroupName);
            Assert.AreEqual(ActiveStaffCount, criteria.Recipients.Count());
        }

        [TestMethod]
        public void InvalidEmailTests()
        {
            CanGetInvalidEmails();
            CanGetInvalidEmail();
            var emailId = CanAddInvalidEmail();
            CanModifyInvalidEmail(emailId);
            CanSetInvalidEmailActive(emailId);
            CanDeleteInvalidEmail(emailId);
        }

        private void CanGetInvalidEmails()
        {
            var invalid = Provider.Mail.MassEmail.GetInvalidEmails(true);
            Assert.AreEqual("boyang@engin.umich.edu", invalid.First().EmailAddress);
        }

        private void CanGetInvalidEmail()
        {
            var invalid = Provider.Mail.MassEmail.GetInvalidEmail(1);
            Assert.AreEqual("boyang@engin.umich.edu", invalid.EmailAddress);
        }

        private int CanAddInvalidEmail()
        {
            var emailId = Provider.Mail.MassEmail.AddInvalidEmail(new InvalidEmailItem
            {
                DisplayName = "User, Test",
                EmailAddress = "test@user.com",
                IsActive = true
            });

            Assert.IsTrue(emailId > 0);

            return emailId;
        }

        private void CanModifyInvalidEmail(int emailId)
        {
            var test = Provider.Mail.MassEmail.ModifyInvalidEmail(new InvalidEmailItem
            {
                EmailID = emailId,
                DisplayName = "User, Test",
                EmailAddress = "test@tester.com",
                IsActive = true
            });

            Assert.IsTrue(test);
        }

        private void CanSetInvalidEmailActive(int emailId)
        {
            var test = Provider.Mail.MassEmail.SetInvalidEmailActive(emailId, false);
            Assert.IsTrue(test);
        }

        private void CanDeleteInvalidEmail(int emailId)
        {
            bool test = Provider.Mail.MassEmail.DeleteInvalidEmail(emailId);
            Assert.IsTrue(test);
        }
    }
}
