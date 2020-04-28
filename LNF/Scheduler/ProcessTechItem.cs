namespace LNF.Scheduler
{
    public class ProcessTechItem : LabItem, IProcessTech
    {
        public int ProcessTechID { get; set; }
        public int ProcessTechGroupID { get; set; }
        public string ProcessTechGroupName { get; set; }
        public string ProcessTechName { get; set; }
        public string ProcessTechDescription { get; set; }
        public bool ProcessTechIsActive { get; set; }
    }
}
