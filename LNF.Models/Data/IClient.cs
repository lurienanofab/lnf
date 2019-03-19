namespace LNF.Models.Data
{
    public interface IClient : IPrivileged
    {
        string FName { get; set; }
        string MName { get; set; }
        string LName { get; set; }
        int DemCitizenID { get; set; }
        int DemGenderID { get; set; }
        int DemRaceID { get; set; }
        int DemEthnicID { get; set; }
        int DemDisabilityID { get; set; }
        int Communities { get; set; }
        int TechnicalInterestID { get; set; }
        string TechnicalInterestName { get; set; }
        bool? IsChecked { get; set; }
        bool? IsSafetyTest { get; set; }
        bool ClientActive { get; set; }
    }
}
