namespace LNF.Models.Data
{
    public class ClientDemographics
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
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
    }
}
