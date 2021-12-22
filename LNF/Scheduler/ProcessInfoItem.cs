using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class ProcessInfoItem : IProcessInfo
    {
        public int ProcessInfoID { get; set; }
        public int ResourceID { get; set; }
        public string ProcessInfoName { get; set; }
        public string ParamName { get; set; }
        public string ValueName { get; set; }
        public string Special { get; set; }
        public bool AllowNone { get; set; }
        public int Order { get; set; }
        public bool RequireValue { get; set; }
        public bool RequireSelection { get; set; }
        public int MaxAllowed { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<IProcessInfoLine> Lines { get; set; }
    }
}
