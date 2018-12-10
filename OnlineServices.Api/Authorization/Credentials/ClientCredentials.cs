namespace OnlineServices.Api.Authorization.Credentials
{
    public class ClientCredentials : CredentialBase
    {
        public override string GrantType
        {
            get { return "client_credentials"; }
        }
    }
}
