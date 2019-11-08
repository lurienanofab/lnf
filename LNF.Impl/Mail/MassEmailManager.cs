using LNF.Data;
using LNF.Mail;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

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

            var invalid = Session.Query<InvalidEmailList>().Where(x => x.IsActive).Select(x => x.EmailAddress).ToArray();

            var result = list.Distinct(comparer).Where(x => !invalid.Contains(x.Email)).ToList();

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
                var to = new List<string>();
                var cc = new List<string>();
                var bcc = new List<string>();

                int staffCount = 0;
                int totalCount = 0;

                foreach (var recip in recipients)
                {
                    if (recip.IsStaff)
                    {
                        // we have to expose staff member's emails, so adding to To (not Bcc)
                        AddRecipient(to, recip.Email);
                        staffCount++;
                    }
                    else
                    {
                        AddRecipient(bcc, recip.Email);
                    }

                    totalCount++;
                }

                if (!string.IsNullOrEmpty(args.CC))
                    AddRecipients(cc, args.CC.Split(','));

                IClient client = Provider.Data.Client.GetClient(args.ClientID);

                bool isStaff = client.HasPriv(ClientPrivilege.Staff);
                bool recipientsIncludeNonStaff = staffCount != totalCount;

                if (isStaff && recipientsIncludeNonStaff)
                    // messages only sent here if staff is sending email to non-staff
                    AddRecipient(bcc, "announcements@lnf.umich.edu");

                // all messages are sent here
                AddRecipient(bcc, "messages@lnf.umich.edu");

                // apparently we always send to the from address also
                AddRecipient(to, args.From);

                string footer = "\n\n--------------------------------------------------\nThis email has been sent to the following group(s) : " + GetGroup(args) + ".";
                footer += "\nYou are receiving this email message because you are associated with the LNF.\nTo unsubscribe, please go to:\nhttp://ssel-sched.eecs.umich.edu/sselonline/Unsubscribe.aspx";

                // an array of file paths
                var attachments = GetAttachments(args.Attachments);

                var sma = new SendMessageArgs
                {
                    ClientID = args.ClientID,
                    Caller = args.Caller,
                    Subject = args.Subject,
                    Body = args.Body + footer,
                    From = args.From,
                    DisplayName = args.DisplayName,
                    To = to.Count == 0 ? null : to,
                    Bcc = bcc.Count == 0 ? null : bcc,
                    Cc = cc.Count == 0 ? null : cc,
                    Attachments = attachments,
                    IsHtml = false
                };

                Provider.Mail.SendMessage(sma);

                AttachmentManager.DeleteAttachments(args.Attachments);
            }

            return recipients.Count;
        }

        public IEnumerable<IInvalidEmail> GetInvalidEmails(bool? active = null)
        {
            var query = Session.Query<InvalidEmailList>();

            if (active.HasValue)
                return query.Where(x => x.IsActive == active.Value).CreateModels<IInvalidEmail>();
            else
                return query.CreateModels<IInvalidEmail>();
        }

        public IInvalidEmail GetInvalidEmail(int emailId)
        {
            var item = Session.Single<InvalidEmailList>(emailId);
            return item.CreateModel<IInvalidEmail>();
        }

        public int AddInvalidEmail(IInvalidEmail model)
        {
            var item = new InvalidEmailList
            {
                DisplayName = model.DisplayName,
                EmailAddress = model.EmailAddress,
                IsActive = model.IsActive
            };

            Session.Insert(item);

            return item.EmailID;
        }

        public bool ModifyInvalidEmail(IInvalidEmail model)
        {
            var item = Session.Single<InvalidEmailList>(model.EmailID);

            if (item != null)
            {
                item.DisplayName = model.DisplayName;
                item.EmailAddress = model.EmailAddress;
                item.IsActive = model.IsActive;
                Session.SaveOrUpdate(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetInvalidEmailActive(int emailId, bool value)
        {
            var item = Session.Single<InvalidEmailList>(emailId);

            if (item != null)
            {
                item.IsActive = value;
                Session.SaveOrUpdate(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetGroup(MassEmailSendArgs args)
        {
            string result = string.Empty;

            switch (args.Group)
            {
                case "community":
                    result = string.Join(", ", CommunityUtility.GetCommunityNames(Combine(args.Values)));
                    break;
                case "manager":
                    result = Provider.Data.Client.GetClient(args.Values.First()).DisplayName;
                    break;
                case "tools":
                    result = string.Join(", ", Provider.Scheduler.Resource.GetResources(args.Values).Select(x => x.ResourceName));
                    break;
                case "lab":
                    result = string.Join(", ", Provider.Data.Room.GetPassbackRooms().Where(x => args.Values.Contains(x.AreaID)).Select(x => x.AreaName));
                    break;
                default: //privilege
                    result = string.Join(", ", PrivUtility.GetPrivTypes((ClientPrivilege)Combine(args.Values)));
                    break;
            }

            return result;
        }

        private void AddRecipient(IList<string> list, string email)
        {
            try
            {
                // make sure email is valid
                var addr = new MailAddress(email);
                if (!list.Contains(addr.Address))
                    list.Add(addr.Address);
            }
            catch { }
        }

        private void AddRecipients(IList<string> list, IEnumerable<string> emails)
        {
            foreach (var e in emails)
            {
                AddRecipient(list, e);
            }
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
