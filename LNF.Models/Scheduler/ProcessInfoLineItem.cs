namespace LNF.Models.Scheduler
{
    public class ProcessInfoLineItem
    {
        public int ProcessInfoLineID { get; set; }
        public int ProcessInfoID { get; set; }
        public string Param { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int ProcessInfoLineParamID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ParameterName { get; set; }
        public int ParameterType { get; set; }
    }
}
