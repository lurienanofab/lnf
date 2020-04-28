using System;
using System.Collections.Generic;

namespace LNF.Inventory
{
    [Obsolete("Use HttpContextBase instead.")]
    public class SessionUtility
    {
        public static IList<IInventoryItem> MasterList
        {
            get
            {
                throw new NotImplementedException();
                //if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.MasterList") == null)
                //{
                //    IList<Item> query = ItemUtility.MasterList();
                //    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.MasterList", query);
                //}
                //return (IList<Item>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.MasterList");
            }
        }

        public static IList<IInventoryItem> Items
        {
            get
            {
                throw new NotImplementedException();
                //if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.Items") == null)
                //{
                //    IList<Item> query = DA.Current.Query<Item>().ToList();
                //    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.Items", query);
                //}
                //return (IList<Item>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.Items");
            }
        }

        public static IList<IInventoryItem> InventoryTypes
        {
            get
            {
                throw new NotImplementedException();
                //if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes") == null)
                //{
                //    IList<InventoryType> query = DA.Current.Query<InventoryType>().ToList();
                //    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes", query);
                //}
                //return (IList<InventoryType>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes");
            }
        }

        public static IList<IInventoryItem> ItemInventoryTypes
        {
            get
            {
                throw new NotImplementedException();
                //if (ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes") == null)
                //{
                //    IList<ItemInventoryType> query = DA.Current.Query<ItemInventoryType>().ToList();
                //    ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes", query);
                //}
                //return (IList<ItemInventoryType>)ServiceProvider.Current.Context.GetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes");
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
            throw new NotImplementedException();
            //ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.Items", null);
        }

        public static void ClearInventoryTypes()
        {
            throw new NotImplementedException();
            //ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.InventoryTypes", null);
        }

        public static void ClearItemInventoryTypes()
        {
            throw new NotImplementedException();
            //ServiceProvider.Current.Context.SetSessionValue("LNF.Inventory.SessionUtility.ItemInventoryTypes", null);
        }
    }
}
