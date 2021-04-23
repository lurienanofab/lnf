namespace LNF.Scheduler
{
    public interface IBuilding : IBuildingItem
    {
        string BuildingDescription { get; set; }
        bool BuildingIsActive { get; set; }
        string BuildingName { get; set; }
    }
}