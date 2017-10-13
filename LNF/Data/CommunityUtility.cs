using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public static class CommunityUtility
    {
        public static bool HasCommunity(int flag1, int flag2)
        {
            return (flag1 & flag2) > 0;
        }

        public static int CalculateFlag(IEnumerable<Community> communites)
        {
            int result = 0;

            foreach (Community c in communites)
                result |= c.CommunityFlag;

            return result;
        }

        public static IList<string> GetCommunityNames(int flags, IList<Community> list = null)
        {
            List<string> result = new List<string>();

            if (list == null)
                list = DA.Current.Query<Community>().ToList();
            
            foreach (Community c in list)
            {
                if (HasCommunity(c.CommunityFlag, flags))
                    result.Add(c.CommunityName);
            }

            return result;
        }

        public static IEnumerable<Community> GetCommunities(int c)
        {
            foreach (var comm in DA.Current.Query<Community>().ToList())
            {
                if ((comm.CommunityFlag & c) > 0)
                    yield return comm;
            }
        }
    }
}
