using System;

namespace LNF.Models.Data
{
    public interface IClient : IPrivileged, IOrg
    {
        string FName { get; set; }
        string MName { get; set; }
        string LName { get; set; }
        string DisplayName { get; }
        int Communities { get; set; }
        bool IsChecked { get; set; }
        bool IsSafetyTest { get; set; }
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
        int TechnicalInterestID { get; set; }
        string TechnicalInterestName { get; set; }
        int ClientOrgID { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
        bool IsManager { get; set; }
        bool IsFinManager { get; set; }
        DateTime? SubsidyStartDate { get; set; }
        DateTime? NewFacultyStartDate { get; set; }
        int ClientAddressID { get; set; }
        bool ClientOrgActive { get; set; }
        int DepartmentID { get; set; }
        string DepartmentName { get; set; }
        int RoleID { get; set; }
        string RoleName { get; set; }
        int MaxChargeTypeID { get; set; }
        string MaxChargeTypeName { get; set; }
        int EmailRank { get; set; }
        bool IsStaff();
    }
}
