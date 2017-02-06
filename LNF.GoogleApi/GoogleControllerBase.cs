using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Services;

namespace LNF.GoogleApi
{
    public abstract class GoogleControllerBase : Controller
    {
        public async Task<ActionResult> Authorize(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                GoogleAuthorization ga = new GoogleAuthorization()
                {
                    AccessToken = result.Credential.Token.AccessToken,
                    ExpiresIn = result.Credential.Token.ExpiresInSeconds.GetValueOrDefault(),
                    RefreshToken = result.Credential.Token.RefreshToken,
                    TokenType = result.Credential.Token.TokenType
                };

                return AuthorizedAction(ga);

                //var service = new DriveService(new BaseClientService.Initializer
                //{
                //    HttpClientInitializer = result.Credential,
                //    ApplicationName = "ASP.NET MVC Sample"
                //});

                //// YOUR CODE SHOULD BE HERE..
                //// SAMPLE CODE:
                //var list = await service.Files.List().ExecuteAsync();
                //ViewBag.Message = "FILE COUNT IS: " + list.Items.Count();
                //return View();
            }
            else
            {
                return Redirect(result.RedirectUri);
            }
        }

        protected abstract ActionResult AuthorizedAction(GoogleAuthorization auth);
    }
}
