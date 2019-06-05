using System.Collections.Generic;
using System.Configuration;

namespace LNF.Models.Authorization.Credentials
{
    public abstract class CredentialBase : ICredentials
    {
        protected readonly string _audienceId;
        protected readonly string _audienceSecret;

        public abstract string GrantType { get; }

        public CredentialBase()
        {
            _audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            _audienceSecret = ConfigurationManager.AppSettings["as:AudienceSecret"];
        }

        protected virtual void AddPostData(IDictionary<string, object> postData)
        {
            // when overridden allow postdata to be added
        }

        public void ApplyParameters(IRequest req)
        {
            req.AddParameter("client_id", _audienceId);
            req.AddParameter("client_secret", _audienceSecret);
            req.AddParameter("grant_type", GrantType);

            var dict = new Dictionary<string, object>();
            AddPostData(dict);

            if (dict.Count > 0)
            {
                foreach (var kvp in dict)
                    req.AddParameter(kvp.Key, kvp.Value);
            }
        }
    }
}
