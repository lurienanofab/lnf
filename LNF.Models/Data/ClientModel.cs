using System.Linq;

namespace LNF.Models.Data
{
    public class ClientModel : IPrivileged
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string LName { get; set; }
        public string DisplayName { get; set; }
        public int DemCitizenID { get; set; }
        public int DemGenderID { get; set; }
        public int DemRaceID { get; set; }
        public int DemEthnicID { get; set; }
        public int DemDisabilityID { get; set; }
        public ClientPrivilege Privs { get; set; }
        public int Communities { get; set; }
        public int TechnicalFieldID { get; set; }
        public string TechnicalFieldName { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsSafetyTest { get; set; }
        public bool ClientActive { get; set; }
        public int ClientOrgID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public bool ClientOrgActive { get; set; }
        public int MaxChargeTypeID { get; set; }

        public bool HasWatchedEthicalVideo()
        {
            return IsChecked.GetValueOrDefault();
        }

        public bool HasTakenSafetyTest()
        {
            return IsSafetyTest.GetValueOrDefault();
        }
    }
}
