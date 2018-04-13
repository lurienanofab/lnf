namespace LNF.Data
{
    public class ManagerItem
    {
        private Repository.Data.ClientManager clientManager;

        public int ClientOrgID { get; set; }
        public string DisplayName { get; set; }
        public bool Deleted { get; set; }

        public ManagerItem()
        {
            Deleted = false;
        }

        public static ManagerItem Create(Repository.Data.ClientManager cm)
        {
            ManagerItem result = new ManagerItem();
            result.clientManager = cm;
            result.ClientOrgID = cm.ManagerOrg.ClientOrgID;
            result.DisplayName = cm.ManagerOrg.Client.DisplayName;
            result.Deleted = false;
            return result;
        }
    }
}
