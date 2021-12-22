using System;
using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IAttachmentUtility
    {
        Guid Attach(IEnumerable<Attachment> attachments);
        int Delete(Guid guid);
    }
}
