using System;
using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IAttachmentManager
    {
        Guid Attach(IEnumerable<Attachment> attachments);
    }
}
