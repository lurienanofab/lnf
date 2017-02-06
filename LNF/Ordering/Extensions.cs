using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public static class PurchaseOrderItemExtensions
    {
        public static IQueryable<PurchaseOrderDetail> GetDetails(this PurchaseOrderItem item)
        {
            return DA.Current.Query<PurchaseOrderDetail>().Where(x => x.Item == item);
        }

        public static PurchaseOrderDetail GetMostRecentlyOrderedDetail(this PurchaseOrderItem item)
        {
            return item.GetDetails()
                    .Where(x => x.PurchaseOrder.Status != Status.Cancelled)
                    .OrderByDescending(x => x.PurchaseOrder.CreatedDate)
                    .FirstOrDefault();
        }
    }

    public static class PurchaseOrderExtensions
    {
        public static PurchaseOrderItem GetDuplicateItem(this PurchaseOrder po, string partNum, string description, int itemId)
        {
            string cleanPartNum = PurchaseOrderItem.CleanString(partNum);
            string cleanDescription = PurchaseOrderItem.CleanString(description);

            IQueryable<PurchaseOrderItem> items = po.Vendor.GetItems().Where(x => x.Active);

            // check if PartNum is empty
            if (!string.IsNullOrEmpty(cleanPartNum))
            {
                // is there a duplicate PartNum?
                PurchaseOrderItem match = items.FirstOrDefault(x => PurchaseOrderItem.CleanString(x.PartNum) == cleanPartNum && x.ItemID != itemId);
                if (match != null)
                    return match;
            }
            else
            {
                // is there a duplicate description?
                PurchaseOrderItem match = items.FirstOrDefault(x => PurchaseOrderItem.CleanString(x.Description) == cleanDescription && x.ItemID != itemId);
                if (match != null)
                    return match;
            }

            return null;
        }

        public static Client GetRealApprover(this PurchaseOrder item)
        {
            //may be null because RealApproverID is nullable
            if (!item.RealApproverID.HasValue) return null;
            return DA.Current.Single<Client>(item.RealApproverID.Value);
        }

        public static Client GetPurchaser(this PurchaseOrder item)
        {
            //may be null because PurchaserID is nullable
            if (!item.PurchaserID.HasValue) return null;
            return DA.Current.Single<Client>(item.PurchaserID.Value);
        }

        public static int GetPurchaserClientID(this PurchaseOrder item)
        {
            var c = item.GetPurchaser();

            if (c == null)
                return 0;
            else
                return c.ClientID;
        }

        public static Account GetAccount(this PurchaseOrder item)
        {
            //may be null if no account is specified yet
            if (!item.AccountID.HasValue) return null;
            return DA.Current.Single<Account>(item.AccountID.Value);
        }

        public static string GetShortCode(this PurchaseOrder item)
        {
            //may be null if no account is specified yet
            Account acct = item.GetAccount();
            return (acct == null) ? string.Empty : acct.ShortCode;
        }

        public static string GetDisplayPOID(this PurchaseOrder item)
        {
            return Providers.Email.CompanyName + item.POID.ToString();
        }

        public static IQueryable<PurchaseOrderDetail> GetDetails(this PurchaseOrder item)
        {
            if (item == null) return null;
            return DA.Current.Query<PurchaseOrderDetail>().Where(x => x.PurchaseOrder == item);
        }

        public static decimal GetTotalPrice(this PurchaseOrder item)
        {
            IQueryable<PurchaseOrderDetail> details = item.GetDetails();
            return Convert.ToDecimal(details.Sum(x => x.UnitPrice * x.Quantity));
        }

        public static bool IsInventoryControlled(this PurchaseOrder item)
        {
            IQueryable<PurchaseOrderDetail> details = item.GetDetails();
            return details.Any(x => x.Item.InventoryItemID.HasValue);
        }
    }

    public static class VendorExtensions
    {
        public static IQueryable<PurchaseOrderItem> GetItems(this Vendor item)
        {
            return DA.Current.Query<PurchaseOrderItem>().Where(x => x.Vendor == item);
        }

        public static IList<PurchaseOrderDetail> GetMostRecentlyOrderedDetails(this Vendor item)
        {
            return item.GetItems().ToList().Select(x => x.GetMostRecentlyOrderedDetail()).Where(x => x != null).ToList();
        }

        public static Client GetClient(this Vendor item)
        {
            //this may retrun null if ClientID = 0 (store manager)
            return DA.Current.Single<Client>(item.ClientID);
        }
    }

    public static class ApproverExtensions
    {
        public static ClientInfo GetApprover(this Approver item)
        {
            ClientInfo result = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.ClientID == item.ApproverID);
            return result;
        }

        public static ClientInfo GetClient(this Approver item)
        {
            ClientInfo result = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.ClientID == item.ClientID);
            return result;
        }

        public static string GetApproverDisplayName(this Approver item)
        {
            ClientInfo client = item.GetApprover();
            return (client == null) ? string.Empty : client.DisplayName;
        }
    }

    public static class ClientExtenstions
    {
        public static IQueryable<Approver> GetApprovers(this Client item)
        {
            return DA.Current.Query<Approver>().Where(x => x.ClientID == item.ClientID && x.Active);
        }

        public static IEnumerable<Client> GetAvailableApprovers(this Client item)
        {
            IList<Approver> approvers = item.GetApprovers().ToList();
            IList<Client> admins = DA.Current.Query<Client>().Where(x => x.Active && (x.Privs & ClientPrivilege.Administrator) > 0).ToList();
            IList<Client> result = admins.Where(acct => !approvers.Any(x => x.ApproverID == acct.ClientID)).ToList();
            return result;
        }

        public static IQueryable<PurchaseOrderAccount> GetAccounts(this Client item)
        {
            return DA.Current.Query<PurchaseOrderAccount>().Where(x => x.ClientID == item.ClientID && x.Active);
        }

        public static IList<Account> GetAvailabeAccounts(this Client item)
        {
            IQueryable<PurchaseOrderAccount> accounts = item.GetAccounts();
            IList<Account> activeAccounts = item.ActiveAccounts();
            IList<Account> result = activeAccounts.ToList().Where(acct => !accounts.Any(x => x.AccountID == acct.AccountID)).ToList();
            return result;
        }
    }

    public static class PurchaseOrderAccountExtensions
    {
        public static Account GetAccount(this PurchaseOrderAccount item)
        {
            return DA.Current.Single<Account>(item.AccountID);
        }

        public static string GetAccountName(this PurchaseOrderAccount item)
        {
            Account acct = item.GetAccount();

            if (acct == null)
                return string.Empty;
            else
                return acct.Name;
        }

        public static string GetShortCode(this PurchaseOrderAccount item)
        {
            Account acct = item.GetAccount();

            if (acct == null)
                return string.Empty;
            else
                return acct.ShortCode.Trim();
        }

        public static string GetFullAccountName(this PurchaseOrderAccount item)
        {
            Account acct = item.GetAccount();

            if (acct == null)
                return string.Empty;
            else
                return acct.GetFullAccountName();
        }

        public static string GetOrgName(this PurchaseOrderAccount item)
        {
            Account acct = item.GetAccount();

            if (acct == null)
                return string.Empty;
            else
                return acct.Org.OrgName;
        }
    }

    public static class PurchaseOrderCategoryExtensions
    {
        public static PurchaseOrderCategory GetParent(this PurchaseOrderCategory item)
        {
            //may return null because ParentID can be zero (for top level categories)
            return DA.Current.Single<PurchaseOrderCategory>(item.ParentID);
        }
    }

    public static class StatusExtensions
    {

    }
}
