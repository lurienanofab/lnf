using LNF.Repository;
using LNF.Repository.Data;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Data
{
    public static class ClientInfoUtility
    {
        private static readonly MemoryCache _cache = new MemoryCache("username-cache");

        public static ClientInfo Current
        {
            get
            {
                ClientInfo client = null;

                var user = Providers.Context.Current.User;

                if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                {
                    //now ClientIDs are cached by username, so this is a fast lookup
                    client = FindByUserName(Providers.Context.Current.User.Identity.Name);
                }

                //no longer calling CheckSession() here, this should be called independently (there isn't always a session)

                //might be null if the current request does not require authentication

                return client;
            }
        }

        public static ClientInfo Find(int clientId)
        {
            return DA.Current.Single<ClientInfo>(clientId);
        }

        public static ClientInfo FindByUserName(string username)
        {
            ClientInfo result = null;

            int clientId = GetId(username);

            if (clientId == 0)
            {
                result = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.UserName == username);

                if (result != null)
                {
                    clientId = result.ClientID;
                    _cache.Add(username, clientId, ObjectCache.InfiniteAbsoluteExpiration);
                }
            }

            if (result == null && clientId > 0)
                result = DA.Current.Single<ClientInfo>(clientId);

            return result;
        }

        public static ClientInfo FindByClientOrgID(int clientOrgId)
        {
            return DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.ClientOrgID == clientOrgId);
        }

        public static int GetId(string username)
        {
            int result = 0;

            if (_cache.Contains(username))
                result = (int)_cache[username];

            return result;
        }
    }
}
