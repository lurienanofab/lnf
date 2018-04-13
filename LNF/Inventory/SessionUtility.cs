using LNF.Repository;
using LNF.Repository.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Inventory
{
    public class SessionUtility
    {
        public static IList<Item> MasterList
        {
            get
            {
                if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.MasterList") == null)
                {
                    IList<Item> query = ItemUtility.MasterList();
                    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.MasterList", query);
                }
                return (IList<Item>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.MasterList");
            }
        }

        public static IList<Item> Items
        {
            get
            {
                if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.Items") == null)
                {
                    IList<Item> query = DA.Current.Query<Item>().ToList();
                    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.Items", query);
                }
                return (IList<Item>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.Items");
            }
        }

        public static IList<InventoryType> InventoryTypes
        {
            get
            {
                if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes") == null)
                {
                    IList<InventoryType> query = DA.Current.Query<InventoryType>().ToList();
                    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes", query);
                }
                return (IList<InventoryType>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes");
            }
        }

        public static IList<ItemInventoryType> ItemInventoryTypes
        {
            get
            {
                if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes") == null)
                {
                    IList<ItemInventoryType> query = DA.Current.Query<ItemInventoryType>().ToList();
                    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes", query);
                }
                return (IList<ItemInventoryType>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes");
            }
        }

        public static void ClearAll()
        {
            ClearItems();
            ClearInventoryTypes();
            ClearItemInventoryTypes();
        }

        public static void ClearItems()
        {
            ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.Items", null);
        }

        public static void ClearInventoryTypes()
        {
            ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes", null);
        }

        public static void ClearItemInventoryTypes()
        {
            ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes", null);
        }
    }
}
