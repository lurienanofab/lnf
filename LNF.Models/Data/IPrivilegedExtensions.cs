using System;
using System.Linq;

namespace LNF.Models.Data
{
    public static class IPrivilegedExtensions
    {
        public static bool HasPriv(this IPrivileged item, string privType)
        {
            ClientPrivilege cp = (ClientPrivilege)Enum.Parse(typeof(ClientPrivilege), privType, true);
            return item.HasPriv(cp);
        }

        public static bool HasPriv(this IPrivileged item, string[] privType)
        {
            ClientPrivilege cp = 0;
            foreach(string p in privType)
                cp |= (ClientPrivilege)Enum.Parse(typeof(ClientPrivilege), p, true);
            return item.HasPriv(cp);
        }

        public static bool HasPriv(this IPrivileged item, int privs)
        {
            return item.HasPriv((ClientPrivilege)privs);
        }

        public static bool HasPriv(this IPrivileged item, ClientPrivilege privs)
        {
            return (item.Privs & privs) > 0;
        }

        public static string[] Roles(this IPrivileged item)
        {
            return item.Privs.ToString().Split(',').Select(x => x.Trim()).ToArray();
        }

        public static bool IsInRole(this IPrivileged item, string role)
        {
            return item.Roles().Contains(role);
        }
    }
}
