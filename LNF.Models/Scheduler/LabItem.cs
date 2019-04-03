namespace LNF.Models.Scheduler
{
    public class LabItem : BuildingItem, ILab
    {
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public string LabDescription { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string RoomDisplayName { get; set; }
        public bool LabIsActive { get; set; }
    }
}
