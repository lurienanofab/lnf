namespace LNF.Scheduler
{
    public interface IBuilding
    {
        string BuildingDescription { get; set; }
        int BuildingID { get; set; }
        bool BuildingIsActive { get; set; }
        string BuildingName { get; set; }
    }
}