using System;

namespace LNF.Data
{
    public interface IPasswordResetRequest
    {
        int PasswordResetRequestID { get; set; }
        int ClientID { get; set; }
        DateTime RequestDateTime { get; set; }
        string ResetCode { get; set; }
        DateTime? ResetDateTime { get; set; }
    }
}
