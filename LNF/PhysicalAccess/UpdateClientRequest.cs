using System;

namespace LNF.PhysicalAccess
{
    public class UpdateClientRequest
    {
        public int ClientID { get; set; }
        public DateTime? ExpireOn { get; set; }
    }
}
