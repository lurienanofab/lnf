using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Apis.Services;

namespace LNF.GoogleApi
{
    internal static class GoogleApiExtensions
    {
        internal static BaseClientService.Initializer GetInitializer(this GoogleAuthorization auth, string appName, Controller controller)
        {
            var result = new BaseClientService.Initializer()
            {
                HttpClientInitializer = auth.GetCredential(controller),
                ApplicationName = "ASP.NET MVC Sample"
            };

            return result;
        }

        internal static UserCredential GetCredential(this GoogleAuthorization auth, Controller controller)
        {
            var meta = new AppFlowMetadata();
            var app = new AuthorizationCodeMvcApp(controller, meta);
            var result = new UserCredential(app.Flow, meta.GetUserId(controller), new TokenResponse()
            {
                AccessToken = auth.AccessToken,
                ExpiresInSeconds = auth.ExpiresIn,
                RefreshToken = auth.RefreshToken,
                TokenType = auth.TokenType
            });
            return result;
        }
    }
}
