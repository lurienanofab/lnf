using System;

namespace LNF.Data
{
    public interface IClient : IPrivileged, IClientOrg, IOrg
    {
        int Communities { get; set; }
        bool IsChecked { get; set; }
        bool IsSafetyTest { get; set; }
        bool RequirePasswordReset { get; set; }
        bool ClientActive { get; set; }

        /// <summary>
        /// For Clients suse TechnicalInterest, for Account use TechnicalField. In both cases the ID referes to the TechnicalField table. 
        /// </summary>
        int TechnicalInterestID { get; set; }

        /// <summary>
        /// For Clients use TechnicalInterest, for Accounts use TechnicalField. In both cases the ID referes to the TechnicalField table. 
        /// </summary>
        string TechnicalInterestName { get; set; }

        int DepartmentID { get; set; }
        string DepartmentName { get; set; }
        int RoleID { get; set; }
        string RoleName { get; set; }
        int MaxChargeTypeID { get; set; }
        string MaxChargeTypeName { get; set; }
        long EmailRank { get; set; }
        bool IsStaff();
    }
}
