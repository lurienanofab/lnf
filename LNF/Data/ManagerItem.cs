using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Data
{
    public class ManagerItem
    {
        private ClientManager clientManager;

        public int ClientOrgID { get; set; }
        public string DisplayName { get; set; }
        public bool Deleted { get; set; }

        public ManagerItem()
        {
            Deleted = false;
        }

        public static ManagerItem Create(ClientManager cm)
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
