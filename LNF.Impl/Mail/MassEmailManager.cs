using LNF.Mail;
using LNF.Models.Mail;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LNF.Impl.Mail
{
    public static class Groups
    {
        public const string Privilege = "privilege";
        public const string Community = "community";
        public const string Manager = "manager";
        public const string Tools = "tools";
        public const string InLab = "inlab";
    }

    public class MassEmailManager : ManagerBase, IMassEmailManager
    {
        public MassEmailManager(IProvider provider) : base(provider) { }

        public IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args)
        {
            var list = new List<MassEmailRecipient>();

            if (args.Privs > 0)
                list.AddRange(GroupEmailManager.GetEmailListByPrivilege(args.Privs));

            if (args.Communities > 0)
                list.AddRange(GroupEmailManager.GetEmailListByCommunity(args.Communities));

            if (args.Manager > 0)
                list.AddRange(GroupEmailManager.GetEmailListByManagerID(args.Manager));

            if (args.Tools != null && args.Tools.Count() > 0)
                list.AddRange(GroupEmailManager.GetEmailListByTools(args.Tools.ToArray()));

            if (args.InLab != null && args.InLab.Count() > 0)
                list.AddRange(GroupEmailManager.GetEmailListByInLab(args.InLab.ToArray()));

            var comparer = new MassEmailRecipientComparer();

            var result = list.Distinct(comparer).ToList();

            return result;
        }

        public int Send(MassEmailSendArgs args)
        {
            if (string.IsNullOrEmpty(args.Group))
                throw new Exception("Group must not be null or empty.");

            if (args.Values == null || args.Values.Count() == 0)
                throw new Exception("Values must not be null or empty.");

            if (string.IsNullOrEmpty(args.Caller))
                throw new Exception("Caller is required.");

            if (string.IsNullOrEmpty(args.From))
                throw new Exception("From address is required.");

            if (string.IsNullOrEmpty(args.DisplayName))
                throw new Exception("Display name is required.");

            if (string.IsNullOrEmpty(args.Subject))
                throw new Exception("Subject is required.");

            if (string.IsNullOrEmpty(args.Body))
                throw new Exception("Body is required.");

            var recipients = new List<MassEmailRecipient>();

            switch (args.Group)
            {
                case Groups.Privilege:
                    recipients.AddRange(GroupEmailManager.GetEmailListByPrivilege(Combine(args.Values)));
                    break;
                case Groups.Community:
                    recipients.AddRange(GroupEmailManager.GetEmailListByCommunity(Combine(args.Values)));
                    break;
                case Groups.Manager:
                    recipients.AddRange(GroupEmailManager.GetEmailListByManagerID(args.Values.First()));
                    break;
                case Groups.Tools:
                    recipients.AddRange(GroupEmailManager.GetEmailListByTools(args.Values.ToArray()));
                    break;
                case Groups.InLab:
                    recipients.AddRange(GroupEmailManager.GetEmailListByInLab(args.Values.ToArray()));
                    break;
                default:
                    throw new NotImplementedException($"Unknown group: {args.Group}");
            }

            if (recipients.Count > 0)
            {
                string[] cc = null;

                if (!string.IsNullOrEmpty(args.CC))
                    cc = args.CC.Split(',');

                var sma = new SendMessageArgs
                {
                    ClientID = args.ClientID,
                    Caller = args.Caller,
                    Subject = args.Subject,
                    Body = args.Body,
                    From = args.From,
                    DisplayName = args.DisplayName,
                    To = recipients.Select(x => x.Email).ToArray(),
                    Cc = cc,
                    Attachments = GetAttachments(args.Attachments),
                    IsHtml = false
                };

                ServiceProvider.Current.Mail.SendMessage(sma);

                AttachmentManager.DeleteAttachments(args.Attachments);
            }

            return recipients.Count;
        }

        private int Combine(IEnumerable<int> values)
        {
            var result = 0;

            foreach (var v in values)
            {
                result |= v;
            }

            return result;
        }

        private string[] GetAttachments(Guid guid)
        {
            string dir = string.Empty;

            if (guid != Guid.Empty)
            {
                dir = AttachmentManager.GetAttachmentsPath(guid);

                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir);
                    if (files.Length > 0)
                    {
                        return files;
                    }
                }
            }

            return null;
        }
    }
}
