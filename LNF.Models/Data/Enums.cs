using System;
using System.Linq;

namespace LNF.Models.Data
{
    //The database is used to convert abitrary strings
    //to values so we can basically use aliases. Maybe
    //these names could change someday, who knows?

    [Flags]
    public enum ClientPrivilege
    {
        LabUser = 1,
        Staff = 2,
        StoreUser = 4,
        Executive = 8,
        FinancialAdmin = 16,
        Administrator = 32,
        WebSiteAdmin = 64,
        RemoteUser = 128,
        StoreManager = 256,
        PhysicalAccess = 512,
        OnlineAccess = 1024,
        Developer = 2048
    }

    public static class ClientPrivilegeUtility
    {
        public static string[] GetRoles(ClientPrivilege privs)
        {
            return privs.ToString().Split(',').Select(x => x.Trim()).ToArray();
        }
    }

    /// <summary>
    /// Represents the different components of a ChartField - used to parse an account number
    /// </summary>
    public enum ChartFieldName
    {
        /// <summary>
        /// The ChartField Account
        /// </summary>
        Account = 1,

        /// <summary>
        /// The ChartField Fund
        /// </summary>
        Fund = 2,

        /// <summary>
        /// The ChartField Department
        /// </summary>
        Department = 3,

        /// <summary>
        /// The ChartField Program
        /// </summary>
        Program = 4,

        /// <summary>
        /// The ChartField Account
        /// </summary>
        Class = 5,

        /// <summary>
        /// The ChartField Project
        /// </summary>
        Project = 6,

        /// <summary>
        /// The ChartField ShortCode (note: this is not part of the account number)
        /// </summary>
        ShortCode = 7
    }
}
