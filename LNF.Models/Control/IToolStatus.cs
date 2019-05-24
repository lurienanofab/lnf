namespace LNF.Models.Control
{
    public interface IToolStatus
    {
        int BuildingID { get; set; }
        string BuildingName { get; set; }
        int LabID { get; set; }
        string LabName { get; set; }
        string LabDisplayName { get; set; }
        int ProcessTechID { get; set; }
        string ProcessTechName { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        bool IsActive { get; set; }
        int PointID { get; set; }
        string InterlockStatus { get; set; }
        bool InterlockState { get; set; }
        bool InterlockError { get; set; }
        bool IsInterlocked { get; set; }
    }
}
