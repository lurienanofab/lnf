using System;

namespace LNF.Mail
{
    public interface IMassEmail
    {
        int MassEmailID { get; set; }
        int ClientID { get; set; }
        Guid EmailId { get; set; }
        string EmailFolder { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime ModifiedOn { get; set; }
        string RecipientGroup { get; set; }
        string RecipientCriteria { get; set; }
        string FromAddress { get; set; }
        string CCAddress { get; set; }
        string DisplayName { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        string[] GetCC();
    }
}
