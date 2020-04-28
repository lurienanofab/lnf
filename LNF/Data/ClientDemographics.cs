namespace LNF.Data
{
    public class ClientDemographics : IClientDemographics
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public int DemCitizenID { get; set; }
        public string DemCitizenValue { get; set; }
        public int DemGenderID { get; set; }
        public string DemGenderValue { get; set; }
        public int DemRaceID { get; set; }
        public string DemRaceValue { get; set; }
        public int DemEthnicID { get; set; }
        public string DemEthnicValue { get; set; }
        public int DemDisabilityID { get; set; }
        public string DemDisabilityValue { get; set; }
    }
}
