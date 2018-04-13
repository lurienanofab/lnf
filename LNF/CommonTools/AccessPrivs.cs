using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    [Obsolete("Use LNF.Repository.Data.ClientPrivilege instead.")]
    public static class AccessPrivs
    {
        public static IDictionary<string, int> CreatePrivDict()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                DataTable dt = dba.FillDataTable("Priv_Select");

                string[] names = Enum.GetNames(typeof(ClientPrivilege));
                
                var values = Enum.GetValues(typeof(ClientPrivilege));

                var result = dt.AsEnumerable().ToDictionary(k => Enum.GetName(typeof(ClientPrivilege), k.Field<int>("Priv")).ToString(), v => v.Field<int>("Priv"));

                return result;
            }
        }

        public static bool ComparePriv(string[] privTypes)
        {
            if (privTypes == null) return true;

            foreach (string priv in privTypes)
            {
                if (ServiceProvider.Current.Context.User.IsInRole(priv))
                    return true;
            }

            return false;
        }

        public static bool ComparePriv(ClientPrivilege privs)
        {
            if (privs == 0) return true;
            return CacheManager.Current.CurrentUser.HasPriv(privs);
        }

        public static bool HasPriv(string privType, int privs)
        {
            int val = GetPrivVal(privType);
            return (privs & val) == val;
        }

        public static int GetPrivVal(string privType)
        {
            IDictionary<string, int> ClientPrivs = CreatePrivDict();
            return ClientPrivs[privType];
        }

        public static string[] MakePrivStringArray(int privs)
        {
            List<string> result = new List<string>();
            IDictionary<string, int> clientPrivs = CreatePrivDict();

            foreach (KeyValuePair<string, int> kvp in clientPrivs)
            {
                if ((privs & kvp.Value) == kvp.Value)
                    result.Add(kvp.Key);
            }

            return result.ToArray();
        }
    }
}
