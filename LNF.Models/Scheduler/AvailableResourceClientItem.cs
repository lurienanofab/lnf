using LNF.Models.Data;

namespace LNF.Models.Scheduler
{
    public class AvailableResourceClientItem
    {
        public int ClientID { get; set; }
        public ClientPrivilege Privs { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}
