namespace LNF.Scheduler
{
    public interface ILab : IBuilding
    {
        int LabID { get; set; }
        string LabName { get; set; }
        string LabDescription { get; set; }
        string LabDisplayName { get; set; }
        bool LabIsActive { get; set; }
    }
}