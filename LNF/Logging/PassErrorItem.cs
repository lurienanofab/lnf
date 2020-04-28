using System;

namespace LNF.Logging
{
    public class PassErrorItem : IPassError
    {
        public Guid ErrorID { get; set; }
        public string ErrorMsg { get; set; }
        public DateTime ErrorTime { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string FilePath { get; set; }
    }
}
