﻿using LNF;
using LNF.Mail;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class AttachmentManager : ApiClient, IAttachmentUtility
    {
        public Guid Attach(IEnumerable<Attachment> attachments)
        {
            var files = new FileCollection("attachments", attachments);
            var result = Post<Guid>("webapi/mail/attachment", files);
            return result;
        }
    }
}
