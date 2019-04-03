namespace LNF.Models.Scheduler
{
    public class BuildingItem : IBuilding
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDescription { get; set; }
        public bool BuildingIsActive { get; set; }
    }
}
