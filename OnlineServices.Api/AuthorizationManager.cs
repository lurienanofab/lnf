using OnlineServices.Api.Authorization;
using OnlineServices.Api.Authorization.Credentials;
using System;
using System.Configuration;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class AuthorizationManager
    {
        public static AuthorizationManager Current { get; }

        static AuthorizationManager()
        {
            Current = new AuthorizationManager();
        }

        private readonly MemoryCache _cache;

        internal AuthorizationManager()
        {
            _cache = new MemoryCache("AuthorizationManagerCache");
        }

        public async Task<ApiClientOptions> GetOptionsForClientCredentials()
        {
            string key = "ClientCredentialsAccess";
            AuthorizationAccess access = null;
            DateTimeOffset expiration = DateTimeOffset.Now.AddMonths(3);
            bool hasAccess;

            if (_cache.Contains(key))
            {
                access = (AuthorizationAccess)_cache[key];
            }

            if (access != null)
            {
                if (access.Expired)
                {
                    // try to refresh
                    try
                    {
                        access = await GetAccess(new RefreshCredentials(access.RefreshToken));
                        _cache.Set(key, access, expiration);

                        // refresh successful
                        hasAccess = true;
                    }
                    catch (ApiHttpRequestException ex)
                    {
                        var clr = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ForegroundColor = clr;

                        // the refresh token is expired
                        hasAccess = false;
                    }
                }
                else
                {
                    // everything is ok, access not null and not expired
                    hasAccess = true;
                }
            }
            else
            {
                // access is null (not in cache)
                hasAccess = false;
            }

            if (!hasAccess)
            {
                access = await GetAccess(new ClientCredentials());
                _cache.Set(key, access, expiration);
            }

            return new ApiClientOptions()
            {
                AccessToken = access.AccessToken,
                TokenType = access.TokenType,
                Host = GetApiHost()
            };
        }

        private Uri GetApiHost()
        {
            string setting = ConfigurationManager.AppSettings["ApiHost"];

            if (string.IsNullOrEmpty(setting))
                throw new InvalidOperationException("Missing appSetting: ApiHost");

            return new Uri(setting);
        }

        private async Task<AuthorizationAccess> GetAccess(ICredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            using (var authClient = new AuthorizationClient())
            {
                var result = await authClient.Authorize(credentials);
                return result;
            }
        }
    }
}
