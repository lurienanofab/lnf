namespace LNF.Scheduler
{
    public class ProcessInfoLineItem : IProcessInfoLine
    {
        public int ProcessInfoLineID { get; set; }
        public int ProcessInfoID { get; set; }
        public string Param { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int ProcessInfoLineParamID { get; set; }
        public bool Deleted { get; set; }
    }
}
