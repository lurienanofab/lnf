namespace LNF.Repository.Scheduler
{
    public class ProcessInfoLine : IDataItem
    {
        public virtual int ProcessInfoLineID { get; set; }
        public virtual int ProcessInfoID { get; set; }
        public virtual string Param { get; set; }
        public virtual double MinValue { get; set; }
        public virtual double MaxValue { get; set; }
        public virtual ProcessInfoLineParam ProcessInfoLineParam { get; set; }
    }
}
