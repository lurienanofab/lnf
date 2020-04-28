using System;

namespace LNF.Logging
{
    public interface IPassError
    {
        Guid ErrorID { get; set; }
        string ErrorMsg { get; set; }
        DateTime ErrorTime { get; set; }
        int ClientID { get; set; }
        string ClientName { get; set; }
        string FilePath { get; set; }
    }
}
