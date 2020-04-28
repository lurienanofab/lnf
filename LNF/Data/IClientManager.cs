namespace LNF.Data
{
    public interface IClientManager
    {
        int ClientManagerID { get; set; }
        int ClientOrgID { get; set; }
        int ManagerOrgID { get; set; }
        bool Active { get; set; }
        string DisplayName { get; set; }
        string ManagerDisplayName { get; set; }
    }
}
