using LNF;
using LNF.Mail;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class AttachmentUtility : ApiClient, IAttachmentUtility
    {
        internal AttachmentUtility(IRestClient rc) : base(rc) { }

        public Guid Attach(IEnumerable<Attachment> attachments)
        {
            var files = new FileCollection("attachments", attachments);
            var result = Post<Guid>("webapi/mail/attachment", files);
            return result;
        }
    }
}
