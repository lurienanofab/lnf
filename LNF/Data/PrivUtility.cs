using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class PrivUtility
    {
        private readonly static IEnumerable<IPriv> _privs;

        public static IPriv LabUser { get { return FindByClientPrivilege(ClientPrivilege.LabUser); } }
        public static IPriv Staff { get { return FindByClientPrivilege(ClientPrivilege.Staff); } }
        public static IPriv StoreUser { get { return FindByClientPrivilege(ClientPrivilege.StoreUser); } }
        public static IPriv Executive { get { return FindByClientPrivilege(ClientPrivilege.Executive); } }
        public static IPriv FinancialAdmin { get { return FindByClientPrivilege(ClientPrivilege.FinancialAdmin); } }
        public static IPriv Administrator { get { return FindByClientPrivilege(ClientPrivilege.Administrator); } }
        public static IPriv WebSiteAdmin { get { return FindByClientPrivilege(ClientPrivilege.WebSiteAdmin); } }
        public static IPriv RemoteUser { get { return FindByClientPrivilege(ClientPrivilege.RemoteUser); } }
        public static IPriv StoreManager { get { return FindByClientPrivilege(ClientPrivilege.StoreManager); } }
        public static IPriv PhysicalAccess { get { return FindByClientPrivilege(ClientPrivilege.PhysicalAccess); } }
        public static IPriv OnlineAccess { get { return FindByClientPrivilege(ClientPrivilege.OnlineAccess); } }
        public static IPriv Developer { get { return FindByClientPrivilege(ClientPrivilege.Developer); } }

        static PrivUtility()
        {
            _privs = ServiceProvider.Current.Data.Client.GetPrivs();
        }

        public static IPriv FindByClientPrivilege(ClientPrivilege priv)
        {
            return _privs.FirstOrDefault(x => x.PrivFlag == priv);
        }

        public static IPriv FindByInt32(int value)
        {
            return FindByClientPrivilege((ClientPrivilege)value);
        }

        public static IPriv FindByPrivType(string privType)
        {
            return _privs.FirstOrDefault(x => x.PrivType == privType);
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
                foreach (var p in _privs.Where(x => privs.Contains(x.PrivType)))
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
            IPriv p = FindByPrivType(privType);
            return HasPriv(p.PrivFlag, privs);
        }

        public static string[] GetPrivTypes(ClientPrivilege privs)
        {
            return privs.ToString().Split(',').Select(x => x.Trim()).ToArray();
        }

        public static IEnumerable<IPriv> GetPrivs(ClientPrivilege privs)
        {
            foreach(var p in _privs)
            {
                if ((privs & p.PrivFlag) > 0)
                    yield return p;
            }
        }
    }
}
