namespace LNF.Scheduler
{
    public interface IProcessTech : IProcessTechItem, ILab
    {
        int ProcessTechGroupID { get; set; }
        string ProcessTechGroupName { get; set; }
        string ProcessTechDescription { get; set; }
        bool ProcessTechIsActive { get; set; }
        string ProcessTechName { get; set; }
    }
}