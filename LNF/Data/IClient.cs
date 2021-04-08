using System;

namespace LNF.Data
{
    public interface IClient : IPrivileged, IClientOrg, IOrg
    {
        string FName { get; set; }
        string MName { get; set; }
        string LName { get; set; }
        string DisplayName { get; }
        int Communities { get; set; }
        bool IsChecked { get; set; }
        bool IsSafetyTest { get; set; }
        bool RequirePasswordReset { get; set; }
        bool ClientActive { get; set; }
        int DemCitizenID { get; set; }
        string DemCitizenName { get; set; }
        int DemGenderID { get; set; }
        string DemGenderName { get; set; }
        int DemRaceID { get; set; }
        string DemRaceName { get; set; }
        int DemEthnicID { get; set; }
        string DemEthnicName { get; set; }
        int DemDisabilityID { get; set; }
        string DemDisabilityName { get; set; }

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
