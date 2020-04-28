namespace LNF.Data
{
    public class ClientManagerItem : IClientManager
    {
        public int ClientManagerID { get; set; }
        public int ClientOrgID { get; set; }
        public int ManagerOrgID { get; set; }
        public bool Active { get; set; }
        public string DisplayName { get; set; }
        public string ManagerDisplayName { get; set; }
    }
}
