namespace LNF.Data
{
    public interface IClientDemographics
    {
        int ClientID { get; set; }
        int DemCitizenID { get; set; }
        string DemCitizenValue { get; set; }
        int DemDisabilityID { get; set; }
        string DemDisabilityValue { get; set; }
        int DemEthnicID { get; set; }
        string DemEthnicValue { get; set; }
        int DemGenderID { get; set; }
        string DemGenderValue { get; set; }
        int DemRaceID { get; set; }
        string DemRaceValue { get; set; }
        string FName { get; set; }
        string LName { get; set; }
        string UserName { get; set; }
    }
}