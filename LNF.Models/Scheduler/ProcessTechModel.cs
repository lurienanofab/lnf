namespace LNF.Models.Scheduler
{
    public class ProcessTechModel
    {
        public int ProcessTechID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public bool LabIsActive { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public bool BuildingIsActive { get; set; }
        public string ProcessTechName { get; set; }
        public string ProcessTechDescription { get; set; }
        public bool ProcessTechIsActive { get; set; }
    }
}
