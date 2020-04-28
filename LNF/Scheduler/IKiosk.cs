namespace LNF.Scheduler
{
    public interface IKiosk
    {
        int KioskID { get; set; }
        string KioskIP { get; set; }
        string KioskName { get; set; }
        int LabID { get; set; }
    }
}