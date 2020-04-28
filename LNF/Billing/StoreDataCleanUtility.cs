using LNF.Data;
using LNF.Inventory;
using System;

namespace LNF.Billing
{
    public static class StoreDataCleanUtility
    {
        public static int LoadDryBoxBilling(DateTime sd, DateTime ed)
        {
            return ServiceProvider.Current.Billing.StoreData.LoadDryBoxBilling(sd, ed);
        }

        public static int DeleteStoreDataClean(DateTime sd, DateTime ed, IClient client = null, IInventoryItem item = null, ICategory category = null)
        {
            int clientId = client == null ? 0 : client.ClientID;
            int itemId = item == null ? 0 : item.ItemID;
            int catId = category == null ? 0 : category.CatID;
            return ServiceProvider.Current.Billing.StoreData.DeleteStoreDataClean(sd, ed, clientId, itemId, catId);
        }
    }
}
