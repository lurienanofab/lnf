namespace LNF.Repository.Scheduler
{
    public class Kiosk : IDataItem
    {
        public virtual int KioskID { get; set; }
        public virtual string KioskName { get; set; }
        public virtual string KioskIP { get; set; }
    }
}
