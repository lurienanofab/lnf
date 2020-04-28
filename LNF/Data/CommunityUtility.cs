using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class CommunityUtility
    {
        private static readonly IEnumerable<ICommunity> _communities;

        static CommunityUtility()
        {
            _communities = ServiceProvider.Current.Data.Client.GetCommunities();
        }

        public static bool HasCommunity(int flag1, int flag2)
        {
            return (flag1 & flag2) > 0;
        }

        public static int CalculateFlag(IEnumerable<ICommunity> communites)
        {
            int result = 0;

            foreach (ICommunity c in communites)
                result |= c.CommunityFlag;

            return result;
        }

        public static IList<string> GetCommunityNames(int flags, IList<ICommunity> list = null)
        {
            List<string> result = new List<string>();

            if (list == null)
                list = _communities.ToList();
            
            foreach (ICommunity c in list)
            {
                if (HasCommunity(c.CommunityFlag, flags))
                    result.Add(c.CommunityName);
            }

            return result;
        }

        public static IEnumerable<ICommunity> GetCommunities(int c)
        {
            foreach (var comm in _communities)
            {
                if ((comm.CommunityFlag & c) > 0)
                    yield return comm;
            }
        }
    }
}
