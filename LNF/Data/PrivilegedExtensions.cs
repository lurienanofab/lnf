using System;
using System.Linq;

namespace LNF.Data
{
    public static class PrivilegedExtensions
    {
        public static bool HasPriv(this IPrivileged item, string privType)
        {
            if (item == null) return false;
            ClientPrivilege cp = (ClientPrivilege)Enum.Parse(typeof(ClientPrivilege), privType, true);
            return item.HasPriv(cp);
        }

        public static bool HasPriv(this IPrivileged item, string[] privType)
        {
            ClientPrivilege cp = 0;
            foreach (string p in privType)
                cp |= (ClientPrivilege)Enum.Parse(typeof(ClientPrivilege), p, true);
            return item.HasPriv(cp);
        }

        public static bool HasPriv(this IPrivileged item, int privs)
        {
            return item.HasPriv((ClientPrivilege)privs);
        }

        public static bool HasPriv(this IPrivileged item, ClientPrivilege privs)
        {
            if (item == null) return false;
            return (item.Privs & privs) > 0;
        }

        public static bool IsStaff(this IPrivileged client)
        {
            return client.HasPriv(ClientPrivilege.Staff);
        }

        public static void AddPriv(this IPrivileged item, ClientPrivilege privs)
        {
            if (item == null) return;
            item.Privs |= privs;
        }

        public static void RemovePriv(this IPrivileged item, ClientPrivilege privs)
        {
            if (item == null) return;
            item.Privs &= ~privs;
        }

        public static string[] Roles(this IPrivileged item)
        {
            return ClientPrivilegeUtility.GetRoles(item.Privs);
        }

        public static bool IsInRole(this IPrivileged item, string role)
        {
            return item.Roles().Contains(role);
        }
    }
}
