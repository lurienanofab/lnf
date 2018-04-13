using System.Linq;
using System;

namespace LNF.Models.Data
{
    public class ClientItem : IPrivileged
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string LName { get; set; }
        public int DemCitizenID { get; set; }
        public string DemCitizenName { get; set; }
        public int DemGenderID { get; set; }
        public string DemGenderName { get; set; }
        public int DemRaceID { get; set; }
        public string DemRaceName { get; set; }
        public int DemEthnicID { get; set; }
        public string DemEthnicName { get; set; }
        public int DemDisabilityID { get; set; }
        public string DemDisabilityName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public int Communities { get; set; }
        public int TechnicalInterestID { get; set; }
        public string TechnicalInterestName { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsSafetyTest { get; set; }
        public bool ClientActive { get; set; }

        public int ClientOrgID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public DateTime? SubsidyStartDate { get; set; }
        public DateTime? NewFacultyStartDate { get; set; }
        public int ClientAddressID { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool ClientOrgActive { get; set; }

        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public int DefClientAddressID { get; set; }
        public int DefBillAddressID { get; set; }
        public int DefShipAddressID { get; set; }
        public bool NNINOrg { get; set; }
        public bool PrimaryOrg { get; set; }
        public bool OrgActive { get; set; }
        public int OrgTypeID { get; set; }
        public string OrgTypeName { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public int ChargeTypeAccountID { get; set; }

        public int MaxChargeTypeID { get; set; }
        public string MaxChargeTypeName { get; set; }

        public int EmailRank { get; set; }

        public string DisplayName { get { return GetDisplayName(LName, FName); } }

        public static string GetDisplayName(string lname, string fname)
        {
            return string.Join(", ", new[] { lname, fname }.Where(x => !string.IsNullOrEmpty(x))).Trim();
        }

        public override string ToString()
        {
            return GetDisplayName(LName, FName);
        }
    }
}
