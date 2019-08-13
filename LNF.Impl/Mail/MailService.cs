using LNF.Models.Mail;
using System;
using System.Collections.Generic;

namespace LNF.Impl.Mail
{
    public class MailService : IMailService
    {
        public IMassEmailManager MassEmail { get; }
        public IAttachmentManager Attachment { get; }

        public MailService(IMassEmailManager massEmail, IAttachmentManager attachment)
        {
            MassEmail = massEmail;
            Attachment = attachment;
        }

        public IMessage GetMessage(int messageId)
        {
            return MailRepo.SelectMessage(messageId);
        }

        public IEnumerable<IMessage> GetMessages(DateTime sd, DateTime ed, int clientId)
        {
            return MailRepo.SelectMessages(sd, ed, clientId);
        }

        public IEnumerable<IRecipient> GetRecipients(int messageId)
        {
            return MailRepo.SelectRecipients(messageId);
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
    }
}