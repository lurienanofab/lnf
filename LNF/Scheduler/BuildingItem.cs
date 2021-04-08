namespace LNF.Scheduler
{
    public class BuildingItem : IBuilding
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDescription { get; set; }
        public bool BuildingIsActive { get; set; }
        public override string ToString() => $"{BuildingName} [{BuildingID}]";
    }
}
