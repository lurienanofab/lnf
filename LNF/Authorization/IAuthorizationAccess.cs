using System;

namespace LNF.Authorization
{
    public interface IAuthorizationAccess
    {
        string AccessToken { get; set; }
        string TokenType { get; set; }
        int ExpiresIn { get; set; }
        string RefreshToken { get; set; }
        DateTime Created { get; }
        DateTime ExpirationDate { get; }
        bool Expired { get; }
    }
}
