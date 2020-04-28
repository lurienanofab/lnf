using LNF.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public static class PurchaseOrderItemExtensions
    {
        public static IPurchaseOrderDetail GetMostRecentlyOrderedDetail(this IPurchaseOrderItem item)
        {
            return ServiceProvider.Current.Ordering.Item.GetDetails(item.ItemID)
                .Where(x => x.StatusID != OrderStatus.Cancelled)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();
        }
    }

    public static class PurchaseOrderExtensions
    {
        public static IPurchaseOrderItem GetDuplicateItem(this IPurchaseOrder po, string partNum, string description, int itemId)
        {
            string cleanPartNum = PurchaseOrderItems.CleanString(partNum);
            string cleanDescription = PurchaseOrderItems.CleanString(description);

            var items = ServiceProvider.Current.Ordering.Vendor.GetItems(po.VendorID).Where(x => x.Active).ToList();

            // check if PartNum is empty
            if (!string.IsNullOrEmpty(cleanPartNum))
            {
                // is there a duplicate PartNum?
                var match = items.FirstOrDefault(x => PurchaseOrderItems.CleanString(x.PartNum) == cleanPartNum && x.ItemID != itemId);
                if (match != null)
                    return match;
            }
            else
            {
                // is there a duplicate description?
                var match = items.FirstOrDefault(x => PurchaseOrderItems.CleanString(x.Description) == cleanDescription && x.ItemID != itemId);
                if (match != null)
                    return match;
            }

            return null;
        }

        public static IClient GetRealApprover(this IPurchaseOrder po)
        {
            //may be null because RealApproverID is nullable
            if (!po.RealApproverID.HasValue) return null;
            return ServiceProvider.Current.Data.Client.GetClient(po.RealApproverID.Value);
        }

        public static IClient GetPurchaser(this IPurchaseOrder po)
        {
            //may be null because PurchaserID is nullable
            if (!po.PurchaserID.HasValue) return null;
            return ServiceProvider.Current.Data.Client.GetClient(po.PurchaserID.Value);
        }

        public static int GetPurchaserClientID(this IPurchaseOrder po)
        {
            var c = po.GetPurchaser();

            if (c == null)
                return 0;
            else
                return c.ClientID;
        }

        public static IAccount GetAccount(this IPurchaseOrder po)
        {
            //may be null if no account is specified yet
            if (!po.AccountID.HasValue) return null;
            return ServiceProvider.Current.Data.Account.GetAccount(po.AccountID.Value);
        }

        public static string GetShortCode(this IPurchaseOrder po)
        {
            //may be null if no account is specified yet
            var acct = po.GetAccount();
            return (acct == null) ? string.Empty : acct.ShortCode;
        }

        public static bool IsInventoryControlled(this IPurchaseOrder po)
        {
            var details = ServiceProvider.Current.Ordering.PurchaseOrder.GetDetails(po.POID);
            return details.Any(x => x.InventoryItemID.HasValue);
        }
    }

    public static class VendorExtensions
    {
        public static IList<IPurchaseOrderDetail> GetMostRecentlyOrderedDetails(this IVendor vendor)
        {
            var items = ServiceProvider.Current.Ordering.Vendor.GetItems(vendor.VendorID);
            return items.Select(x => x.GetMostRecentlyOrderedDetail()).Where(x => x != null).ToList();
        }

        public static IClient GetClient(this IVendor vendor)
        {
            //this may retrun null if ClientID = 0 (store manager)
            return ServiceProvider.Current.Data.Client.GetClient(vendor.ClientID);
        }
    }

    public static class ApproverExtensions
    {
        public static IClient GetApprover(this IApprover approver)
        {
            return ServiceProvider.Current.Data.Client.GetClient(approver.ApproverID);
        }

        public static IClient GetClient(this IApprover approver)
        {
            return ServiceProvider.Current.Data.Client.GetClient(approver.ClientID);
        }

        public static string GetApproverDisplayName(this IApprover approver)
        {
            var client = approver.GetApprover();
            return (client == null) ? string.Empty : client.DisplayName;
        }
    }

    public static class ClientExtenstions
    {
        public static IEnumerable<IApprover> GetApprovers(this IClient client)
        {
            return ServiceProvider.Current.Ordering.Approver.GetActiveApprovers(client.ClientID).ToList();
        }

        public static IEnumerable<IClient> GetAvailableApprovers(this IClient client)
        {
            var approvers = client.GetApprovers().ToList();
            var admins = ServiceProvider.Current.Data.Client.GetActiveClients(ClientPrivilege.Administrator).ToList();
            var result = admins.Where(acct => !approvers.Any(x => x.ApproverID == acct.ClientID)).ToList();
            return result;
        }
    }

    public static class PurchaseOrderAccountExtensions
    {
        public static IAccount GetAccount(this IPurchaseOrderAccount acct)
        {
            return ServiceProvider.Current.Data.Account.GetAccount(acct.AccountID);
        }

        public static string GetAccountName(this IPurchaseOrderAccount acct)
        {
            var a = acct.GetAccount();

            if (a == null)
                return string.Empty;
            else
                return a.AccountName;
        }

        public static string GetShortCode(this IPurchaseOrderAccount acct)
        {
            var a = acct.GetAccount();

            if (a == null)
                return string.Empty;
            else
                return a.ShortCode.Trim();
        }

        public static string GetFullAccountName(this IPurchaseOrderAccount acct)
        {
            var a = acct.GetAccount();

            if (a == null)
                return string.Empty;
            else
                return Accounts.GetFullAccountName(a);
        }

        public static string GetOrgName(this IPurchaseOrderAccount acct)
        {
            var a = acct.GetAccount();

            if (a == null)
                return string.Empty;
            else
                return a.OrgName;
        }
    }

    public static class PurchaseOrderCategoryExtensions
    {
        public static IPurchaseOrderCategory GetParent(this IPurchaseOrderCategory category)
        {
            //may return null because ParentID can be zero (for top level categories)
            return ServiceProvider.Current.Ordering.Category.GetParent(category.ParentID);
        }
    }
}
