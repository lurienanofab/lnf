namespace LNF.Scheduler
{
    public interface IProcessTech : ILab
    {
        int ProcessTechID { get; set; }
        int ProcessTechGroupID { get; set; }
        string ProcessTechGroupName { get; set; }
        string ProcessTechDescription { get; set; }
        bool ProcessTechIsActive { get; set; }
        string ProcessTechName { get; set; }
    }
}