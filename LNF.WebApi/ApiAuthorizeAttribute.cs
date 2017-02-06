using LNF.Data;
using LNF.Repository;
using Microsoft.Owin.Security.DataHandler.Encoder;
using OnlineServices.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using System.Security.Principal;

namespace LNF.WebApi
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (CheckApiKeyAuthorization(actionContext))
                return true;

            if (CheckFormsAuthorization(actionContext))
                return true;

            if (CheckBearerAuthorization(actionContext))
                return true;

            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // This will prevent redirecting to the login page, which is bad for webapi.
            if (HttpContext.Current != null)
                HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(actionContext);
        }

        private bool CheckFormsAuthorization(HttpActionContext context)
        {
            string accessToken = string.Empty;

            bool isAuthorized = base.IsAuthorized(context);

            if (isAuthorized)
            {
                // will be true when already signed in (for example ajax call)

                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request != null)
                    {
                        if (HttpContext.Current.Request.Cookies != null)
                        {
                            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                            if (cookie != null)
                            {
                                if (HttpContext.Current.Request.IsAuthenticated)
                                    accessToken = cookie.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                // check the authorization header

                if (context.Request.Headers.Authorization != null)
                {
                    if (new[] { "Forms", "forms" }.Contains(context.Request.Headers.Authorization.Scheme))
                    {
                        var token = context.Request.Headers.Authorization.Parameter;
                        if (!string.IsNullOrEmpty(token))
                        {
                            FormsAuthenticationTicket ticket;

                            try
                            {
                                ticket = GetAuthenticationTicket(token);
                            }
                            catch
                            {
                                // the ticket is no good
                                return false;
                            }

                            if (!ticket.Expired)
                            {
                                string[] roles = ticket.UserData.Split('|');
                                context.RequestContext.Principal = new GenericPrincipal(new GenericIdentity(ticket.Name), roles);
                                accessToken = token;
                                isAuthorized = true;
                            }
                        }
                    }
                }
            }

            if (isAuthorized && !string.IsNullOrEmpty(accessToken))
            {
                context.SetClientOptions(new ApiClientOptions()
                {
                    AccessToken = accessToken,
                    TokenType = "Forms",
                    Host = WebApiUtility.GetApiHost()
                });

                return true;
            }

            return false;
        }

        private FormsAuthenticationTicket GetAuthenticationTicket(string token)
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(token);
            return ticket;
        }

        private bool CheckApiKeyAuthorization(HttpActionContext context)
        {
            var header = context.Request.Headers.FirstOrDefault(x => x.Key == "apikey");

            if (header.Key == "apikey")
            {
                var val = header.Value.FirstOrDefault();
                if (!string.IsNullOrEmpty(val))
                {
                    if (val == "lnf123")
                        return true;
                }
            }

            return false;
        }

        private bool CheckBearerAuthorization(HttpActionContext context)
        {
            string accessToken = null;

            Dictionary<string, string> qs = context.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            if (qs.ContainsKey("token"))
                accessToken = qs["token"];
            else
            {
                if (context.Request.Headers.Authorization != null)
                {
                    if (new[] { "Bearer", "bearer" }.Contains(context.Request.Headers.Authorization.Scheme))
                        accessToken = context.Request.Headers.Authorization.Parameter;
                }
            }

            string issuer = GetIssuer();

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(issuer))
            {
                JwtSecurityToken securityToken;

                try
                {
                    securityToken = new JwtSecurityToken(accessToken);
                }
                catch
                {
                    // the token is no good
                    return false;
                }

                string audienceId = securityToken.Audiences.First();

                OAuthClientAudience aud = DA.Current.Query<OAuthClientAudience>().FirstOrDefault(x => x.AudienceId == audienceId);

                if (aud != null)
                {
                    if (!aud.Deleted)
                    {
                        if (aud.Active)
                        {
                            byte[] secretBytes = TextEncodings.Base64Url.Decode(aud.AudienceSecret);
                            InMemorySymmetricSecurityKey signingKey = new InMemorySymmetricSecurityKey(secretBytes);

                            TokenValidationParameters validationParameters = new TokenValidationParameters()
                            {
                                IssuerSigningKey = signingKey,
                                ValidAudience = audienceId,
                                ValidIssuer = issuer
                            };

                            ClaimsPrincipal principal;

                            try
                            {
                                principal = GetClaimsPrincipal(accessToken, validationParameters);
                            }
                            catch
                            {
                                // failed to validate token
                                return false;
                            }

                            context.RequestContext.Principal = principal;

                            context.SetClientOptions(new ApiClientOptions()
                            {
                                AccessToken = accessToken,
                                TokenType = "Bearer",
                                Host = WebApiUtility.GetApiHost()
                            });

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private ClaimsPrincipal GetClaimsPrincipal(string token, TokenValidationParameters validationParameters)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            return principal;
        }

        private string GetIssuer()
        {
            string issuer = ConfigurationManager.AppSettings["as:Issuer"];

            if (string.IsNullOrEmpty(issuer))
                throw new InvalidOperationException("Missing appSetting: as:Issuer");

            return issuer;
        }
    }
}
