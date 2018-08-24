namespace LNF.Models.Scheduler
{
    public class ProcessInfoItem
    {
        public int ProcessInfoID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ProcessInfoName { get; set; }
        public string ParamName { get; set; }
        public string ValueName { get; set; }
        public string Special { get; set; }
        public bool AllowNone { get; set; }
        public int Order { get; set; }
        public bool RequireValue { get; set; }
        public bool RequireSelection { get; set; }
    }
}
