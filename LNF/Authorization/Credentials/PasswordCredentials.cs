﻿using System.Collections.Generic;

namespace LNF.Authorization.Credentials
{
    public class PasswordCredentials : CredentialBase
    {
        public override string GrantType
        {
            get { return "password"; }
        }

        public string Username { get; }

        public string Password { get; }

        public PasswordCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        protected override void AddPostData(IDictionary<string, object> postData)
        {
            postData.Add("username", Username);
            postData.Add("password", Password);
        }
    }
}
