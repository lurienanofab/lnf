using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF.Mail;
using System.IO;
using System;

namespace OnlineServices.Api.Tests.Mail
{
    [TestClass]
    public class AttachmentUtilityTests : OnlineServicesApiTest
    {
        [TestMethod]
        public void AttachmentTests()
        {
            var guid = CanAttach();
            CanDelete(guid);
        }

        private Guid CanAttach()
        {
            var guid = Provider.Mail.Attachment.Attach(new Attachment[]
            {
                new Attachment{ FileName = "test.txt", Data = File.ReadAllBytes("test.txt") }
            });

            Assert.IsTrue(Guid.Empty != guid);
            Assert.IsTrue(File.Exists(Path.Combine(GetSecurePath(), "mail\\attachments", guid.ToString("n"), "test.txt")));

            return guid;
        }

        private void CanDelete(Guid guid)
        {
            var test = Provider.Mail.Attachment.Delete(guid);
            Assert.AreEqual(1, test);
        }

        private string GetSecurePath()
        {
            return LNF.CommonTools.Utility.GetRequiredAppSetting("SecurePath");
        }
    }
}
