using System;

namespace LNF.Data
{
    public class ActiveLogItem : IActiveLog
    {
        public int LogID { get; set; }
        public string TableName { get; set; }
        public int Record { get; set; }
        public DateTime EnableDate { get; set; }
        public DateTime? DisableDate { get; set; }
    }
}
