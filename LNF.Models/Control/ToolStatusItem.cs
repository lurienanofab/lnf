namespace LNF.Models.Control
{
    public class ToolStatusItem : IToolStatus
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public int ProcessTechID { get; set; }
        public string ProcessTechName { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public bool IsActive { get; set; }
        public int PointID { get; set; }
        public string InterlockStatus { get; set; }
        public bool InterlockState { get; set; }
        public bool InterlockError { get; set; }
        public bool IsInterlocked { get; set; }
    }
}