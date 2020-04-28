namespace LNF.Scheduler
{
    public class KioskItem : IKiosk
    {
        public int KioskID { get; set; }
        public string KioskName { get; set; }
        public string KioskIP { get; set; }
        public int LabID { get; set; }
    }
}
