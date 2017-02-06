namespace LNF.Models.Scheduler
{
    public class LabModel
    {
        public int LabID { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public bool BuildingIsActive { get; set; }
        public string LabName { get; set; }
        public string Description { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string LabDisplayName { get; set; }
        public bool LabIsActive { get; set; }
    }
}
