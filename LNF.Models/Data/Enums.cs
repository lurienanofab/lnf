using System;

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
}
