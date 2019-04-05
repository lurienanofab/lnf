using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LNF.Data
{
    public static class PrivUtility
    {
        public static Priv LabUser { get { return FindByClientPrivilege(ClientPrivilege.LabUser); } }
        public static Priv Staff { get { return FindByClientPrivilege(ClientPrivilege.Staff); } }
        public static Priv StoreUser { get { return FindByClientPrivilege(ClientPrivilege.StoreUser); } }
        public static Priv Executive { get { return FindByClientPrivilege(ClientPrivilege.Executive); } }
        public static Priv FinancialAdmin { get { return FindByClientPrivilege(ClientPrivilege.FinancialAdmin); } }
        public static Priv Administrator { get { return FindByClientPrivilege(ClientPrivilege.Administrator); } }
        public static Priv WebSiteAdmin { get { return FindByClientPrivilege(ClientPrivilege.WebSiteAdmin); } }
        public static Priv RemoteUser { get { return FindByClientPrivilege(ClientPrivilege.RemoteUser); } }
        public static Priv StoreManager { get { return FindByClientPrivilege(ClientPrivilege.StoreManager); } }
        public static Priv PhysicalAccess { get { return FindByClientPrivilege(ClientPrivilege.PhysicalAccess); } }
        public static Priv OnlineAccess { get { return FindByClientPrivilege(ClientPrivilege.OnlineAccess); } }
        public static Priv Developer { get { return FindByClientPrivilege(ClientPrivilege.Developer); } }

        public static Priv FindByClientPrivilege(ClientPrivilege priv)
        {
            return DA.Current.Single<Priv>(priv);
        }

        public static Priv FindByInt32(int value)
        {
            return FindByClientPrivilege((ClientPrivilege)value);
        }

        public static Priv FindByPrivType(string privType)
        {
            Priv p = DA.Current.Query<Priv>().FirstOrDefault(x => x.PrivType == privType);
            return p;
        }

        public static int CalculatePriv(IEnumerable<ClientPrivilege> privs)
        {
            int result = 0;
            foreach (var p in privs)
                result |= (int)p;
            return result;
        }

        public static int CalculatePriv(ClientPrivilege privs)
        {
            return (int)privs;
        }

        public static ClientPrivilege CalculatePriv(IEnumerable<Priv> privs)
        {
            ClientPrivilege result = 0;
            if (privs != null)
            {
                privs.Select(x => { result |= x.PrivFlag; return x; }).ToList();
            }
            return result;
        }

        public static ClientPrivilege CalculatePriv(IEnumerable<IPriv> privs)
        {
            ClientPrivilege result = 0;
            if (privs != null)
            {
                privs.Select(x => { result |= x.PrivFlag; return x; }).ToList();
            }
            return result;
        }

        public static ClientPrivilege CalculatePriv(IEnumerable<string> privs)
        {
            ClientPrivilege result = 0;
            if (privs != null)
            {
                foreach (var p in DA.Current.Query<Priv>().Where(x => privs.Contains(x.PrivType)))
                    result |= p.PrivFlag;
            }
            return result;
        }

        public static ClientPrivilege CalculatePriv(IEnumerable<int> privs)
        {
            ClientPrivilege result = 0;
            if (privs != null)
            {
                foreach (var p in privs)
                    result |= (ClientPrivilege)p;
            }
            return result;
        }

        public static ClientPrivilege CalculatePriv(int privs)
        {
            return (ClientPrivilege)privs;
        }

        public static bool HasPriv(ClientPrivilege priv1, ClientPrivilege priv2)
        {
            return (priv1 & priv2) > 0;
        }

        public static bool HasPriv(string privType, ClientPrivilege privs)
        {
            Priv p = FindByPrivType(privType);
            return HasPriv(p.PrivFlag, privs);
        }

        public static string[] GetPrivTypes(ClientPrivilege privs)
        {
            return privs.ToString().Split(',').Select(x => x.Trim()).ToArray();
        }

        public static IEnumerable<Priv> GetPrivs(ClientPrivilege privs)
        {
            foreach(var p in DA.Current.Query<Priv>().ToList())
            {
                if ((privs & p.PrivFlag) > 0)
                    yield return p;
            }
        }
    }
}
