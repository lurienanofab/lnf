using LNF.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace LNF.Impl.Mail
{
    public class AttachmentManager : IAttachmentUtility
    {
        public static string GetSecurePath()
        {
            var setting = ConfigurationManager.AppSettings["SecurePath"];

            if (string.IsNullOrEmpty(setting))
                throw new Exception("Missing required appSetting: SecurePath");

            if (!Directory.Exists(setting))
                throw new Exception($"SecurePath directory not found: {setting}");

            return setting;
        }

        public static string GetAttachmentsPath(Guid guid)
        {
            var p1 = Path.Combine(GetSecurePath(), "mail");

            if (!Directory.Exists(p1))
                Directory.CreateDirectory(p1);

            var p2 = Path.Combine(p1, "attachments");

            if (!Directory.Exists(p2))
                Directory.CreateDirectory(p2);

            return Path.Combine(p2, guid.ToString("n"));
        }

        public static bool DeleteAttachments(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                var dir = GetAttachmentsPath(guid);

                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    return true;
                }
            }
            
            return false;
        }

        public Guid Attach(IEnumerable<Attachment> attachments)
        {
            Guid result = Guid.NewGuid();

            var dir = GetAttachmentsPath(result);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (var a in attachments)
                File.WriteAllBytes(Path.Combine(dir, a.FileName), a.Data);

            return result;
        }
    }
}
