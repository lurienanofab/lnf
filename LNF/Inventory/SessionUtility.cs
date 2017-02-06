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
                if (Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.MasterList") == null)
                {
                    IList<Item> query = ItemUtility.MasterList();
                    Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.MasterList", query);
                }
                return (IList<Item>)Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.MasterList");
            }
        }

        public static IList<Item> Items
        {
            get
            {
                if (Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.Items") == null)
                {
                    IList<Item> query = DA.Current.Query<Item>().ToList();
                    Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.Items", query);
                }
                return (IList<Item>)Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.Items");
            }
        }

        public static IList<InventoryType> InventoryTypes
        {
            get
            {
                if (Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes") == null)
                {
                    IList<InventoryType> query = DA.Current.Query<InventoryType>().ToList();
                    Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes", query);
                }
                return (IList<InventoryType>)Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes");
            }
        }

        public static IList<ItemInventoryType> ItemInventoryTypes
        {
            get
            {
                if (Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes") == null)
                {
                    IList<ItemInventoryType> query = DA.Current.Query<ItemInventoryType>().ToList();
                    Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes", query);
                }
                return (IList<ItemInventoryType>)Providers.Context.Current.GetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes");
            }
        }

        public static void ClearAll()
        {
            SessionUtility.ClearItems();
            SessionUtility.ClearInventoryTypes();
            SessionUtility.ClearItemInventoryTypes();
        }

        public static void ClearItems()
        {
            Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.Items", null);
        }

        public static void ClearInventoryTypes()
        {
            Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes", null);
        }

        public static void ClearItemInventoryTypes()
        {
            Providers.Context.Current.SetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes", null);
        }
    }
}
