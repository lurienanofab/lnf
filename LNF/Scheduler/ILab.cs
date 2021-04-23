namespace LNF.Scheduler
{
    public interface ILab : ILabItem, IBuilding
    {
        string LabName { get; set; }
        string LabDescription { get; set; }
        string LabDisplayName { get; set; }
        bool LabIsActive { get; set; }
    }
}