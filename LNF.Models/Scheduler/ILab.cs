namespace LNF.Models.Scheduler
{
    public interface ILab : IBuilding
    {
        string LabDescription { get; set; }
        string LabDisplayName { get; set; }
        int LabID { get; set; }
        bool LabIsActive { get; set; }
        string LabName { get; set; }
        string RoomDisplayName { get; set; }
        int RoomID { get; set; }
        string RoomName { get; set; }
    }
}