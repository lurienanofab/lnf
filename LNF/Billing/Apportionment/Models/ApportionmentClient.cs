using LNF.Data;

namespace LNF.Billing.Apportionment.Models
{
    public class ApportionmentClient : IPrivileged
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public string LName { get; set; }
        public string MName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public bool Active { get; set; }
    }
}
