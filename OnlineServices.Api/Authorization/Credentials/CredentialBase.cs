using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace OnlineServices.Api.Authorization.Credentials
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

        protected virtual void AddPostData(IDictionary<string, string> postData)
        {
            // Override to add additional post data
        }

        public HttpContent CreateContent()
        {
            IDictionary<string, string> postData = new Dictionary<string, string>();

            postData.Add("client_id", _audienceId);
            postData.Add("client_secret", _audienceSecret);
            postData.Add("grant_type", GrantType);

            AddPostData(postData);

            HttpContent result = new FormUrlEncodedContent(postData);
            return result;
        }
    }
}
