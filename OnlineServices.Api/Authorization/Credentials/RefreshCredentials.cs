using System.Collections.Generic;

namespace OnlineServices.Api.Authorization.Credentials
{
    public class RefreshCredentials : CredentialBase
    {
        public string RefreshToken { get; }

        public RefreshCredentials(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public override string GrantType
        {
            get { return "refresh_token"; }
        }

        protected override void AddPostData(IDictionary<string, object> postData)
        {
            postData.Add("refresh_token", RefreshToken);
        }
    }
}
