using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Util.Store;

namespace LNF.GoogleApi
{
    public class AppFlowMetadata : FlowMetadata
    {
        private const string CLIENT_ID = "495387045568.project.googleusercontent.com";
        private const string CLIENT_SECRET = "U2Zrt9kWozkChFP25URWWgUU";

        private static readonly IAuthorizationCodeFlow flow;

        static AppFlowMetadata()
        {
            flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = CLIENT_ID,
                    ClientSecret = CLIENT_SECRET
                },
                Scopes = new[] { DriveService.Scope.Drive }
                //,DataStore = new FileDataStore("C:\\secure\\Drive.Api.Auth.Store", true)
            });
        }

        public override string GetUserId(Controller controller)
        {
            // In this sample we use the session to store the user identifiers.
            // That's not the best practice, because you should have a logic to identify
            // a user. You might want to use "OpenID Connect".
            // You can read more about the protocol in the following link:
            // https://developers.google.com/accounts/docs/OAuth2Login.

            var user = controller.Session["user"];

            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }

            return user.ToString();

        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}
