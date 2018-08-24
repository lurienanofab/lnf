namespace LNF.Models.Scheduler
{
    public interface IResource
    {
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        int BuildingID { get; set; }
        string BuildingName { get; set; }
        int LabID { get; set; }
        string LabName { get; set; }
        string LabDisplayName { get; set; }
        int ProcessTechID { get; set; }
        string ProcessTechName { get; set; }
        string ResourceDescription { get; set; }
        string HelpdeskEmail { get; set; }
        bool IsSchedulable { get; set; }
        bool ResourceIsActive { get; set; }
    }
}
