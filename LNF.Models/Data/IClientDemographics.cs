namespace LNF.Models.Data
{
    public interface IClientDemographics
    {
        int ClientID { get; set; }
        int DemCitizenID { get; set; }
        string DemCitizenName { get; set; }
        int DemDisabilityID { get; set; }
        string DemDisabilityName { get; set; }
        int DemEthnicID { get; set; }
        string DemEthnicName { get; set; }
        int DemGenderID { get; set; }
        string DemGenderName { get; set; }
        int DemRaceID { get; set; }
        string DemRaceName { get; set; }
        string FName { get; set; }
        string LName { get; set; }
        string UserName { get; set; }
    }
}