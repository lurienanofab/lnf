using LNF.Data;
using LNF.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Mail
{
    public class MailService : IMailService
    {
        public IMassEmailRepository MassEmail { get; }
        public IAttachmentUtility Attachment { get; }
        public IClientRepository Client { get; }

        public MailRepo MailRepo { get; } = MailRepo.Create();

        public MailService(IMassEmailRepository massEmail, IAttachmentUtility attachment, IClientRepository client)
        {
            MassEmail = massEmail;
            Attachment = attachment;
            Client = client;
        }

        public Message GetMessage(int messageId)
        {
            return MailRepo.SelectMessage(messageId);
        }

        public IEnumerable<Message> GetMessages(DateTime sd, DateTime ed, int clientId)
        {
            return MailRepo.SelectMessages(sd, ed, clientId);
        }

        public IEnumerable<Recipient> GetRecipients(int messageId)
        {
            return MailRepo.SelectRecipients(messageId);
        }

        public int SendMassEmail(MassEmailSendArgs args)
        {
            var sma = MassEmail.CreateSendMessageArgs(args);
            SendMessage(sma);
            Attachment.Delete(args.Attachments);
            var result = sma.GetDistinctEmails().Length;
            return result;
        }

        public void SendMessage(SendMessageArgs args)
        {
            int messageId = MailRepo.InsertMessage(args.ClientID, args.Caller, args.From, args.Subject, args.Body);

            if (messageId == 0)
                throw new Exception("Failed to create message [messageId = 0]");

            MailRepo.InsertRecipients(messageId, AddressType.To, args.To);
            MailRepo.InsertRecipients(messageId, AddressType.Cc, args.Cc);
            MailRepo.InsertRecipients(messageId, AddressType.Bcc, args.Bcc);

            try
            {
                MailUtility.Send(args);
                MailRepo.SetMessageSent(messageId);
            }
            catch (Exception ex)
            {
                MailRepo.SetMessageError(messageId, ex.Message);
                throw ex;
            }
        }

        public IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs)
        {
            var clients = Client.FindByPrivilege(privs);
            IEnumerable<string> result = clients.Select(c => c.Email);
            return result;
        }
    }
}