using LNF.Cache;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public static class CacheManagerExtensions
    {
        public static IEnumerable<AccountItem> Accounts(this CacheManager cm)
        {
            var result = cm.GetContextItem<IList<AccountItem>>("Accounts");

            if (result == null)
            {
                result = DA.Current.Query<AccountInfo>().Model<AccountItem>();
                cm.SetContextItem("Accounts", result);
            }

            return result;
        }

        public static AccountItem GetAccount(this CacheManager cm, int accountId)
        {
            return cm.Accounts().FirstOrDefault(x => x.AccountID == accountId);
        }

        public static IEnumerable<ClientAccountItem> ClientAccounts(this CacheManager cm)
        {
            var result = cm.GetContextItem<IList<ClientAccountItem>>("ClientAccounts");

            if (result == null)
            {
                result = DA.Current.Query<ClientAccountInfo>().Model<ClientAccountItem>();
                cm.SetContextItem("ClientAccounts", result);
            }

            return result;
        }

        public static IEnumerable<ClientAccountItem> GetClientAccounts(this CacheManager cm, int clientId)
        {
            return cm.ClientAccounts().Where(x => x.ClientID == clientId);
        }

        public static IEnumerable<ClientAccountItem> ActiveClientAccounts(this CacheManager cm, int clientId)
        {
            return cm.GetClientAccounts(clientId).Where(x => x.ClientAccountActive && x.ClientOrgActive).ToList();
        }

        public static ClientAccountItem GetClientAccount(this CacheManager cm, int clientId, int accountId)
        {
            return cm.GetClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);
        }

        public static ClientAccountItem GetActiveClientAccount(this CacheManager cm, int clientId, int accountId)
        {
            return cm.ActiveClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);
        }

        public static IEnumerable<ClientAccountItem> CurrentUserClientAccounts(this CacheManager cm)
        {
            return cm.GetClientAccounts(cm.ClientID);
        }

        public static IEnumerable<ClientAccountItem> CurrentUserActiveClientAccounts(this CacheManager cm)
        {
            return cm.ActiveClientAccounts(cm.ClientID);
        }

        public static IEnumerable<ClientItem> ClientOrgs(this CacheManager cm)
        {
            var result = cm.GetContextItem<IList<ClientItem>>("ClientOrgs");

            if (result == null)
            {
                result = DA.Current.Query<ClientOrgInfo>().Model<ClientItem>();
                cm.SetContextItem("ClientOrgs", result);
            }

            return result;
        }

        public static IEnumerable<ClientItem> GetClientOrgs(this CacheManager cm, int clientId)
        {
            return cm.ClientOrgs().Where(x => x.ClientID == clientId);
        }

        public static IEnumerable<ClientItem> ActiveClientOrgs(this CacheManager cm, int clientId)
        {
            return cm.GetClientOrgs(clientId).Where(x => x.ClientOrgActive).ToList();
        }

        public static ClientItem GetClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.GetClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        public static ClientItem GetActiveClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.ActiveClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        public static IEnumerable<ClientItem> CurrentUserClientOrgs(this CacheManager cm)
        {
            return cm.GetClientOrgs(cm.CurrentUser.ClientID);
        }

        public static IEnumerable<ClientItem> CurrentUserActiveClientOrgs(this CacheManager cm)
        {
            return cm.ActiveClientOrgs(cm.ClientID);
        }

        // Gets the global cost config from cache. Cached for the session because this data rarely changes.
        public static GlobalCostItem GetGlobalCost(this CacheManager cm)
        {
            GlobalCostItem result = cm.GetSessionValue("GlobalCost", () => DA.Current.Query<GlobalCost>().FirstOrDefault().Model<GlobalCostItem>());
            return result;
        }

        public static int GetBusinessDay(this CacheManager cm)
        {
            return cm.GetGlobalCost().BusinessDay;
        }

        /// <summary>
        /// Gets all rooms from cache. Cached for the session because this data rarely changes.
        /// </summary>
        public static IEnumerable<RoomItem> Rooms(this CacheManager cm)
        {
            IList<RoomItem> result = cm.GetSessionValue("Rooms", () => DA.Current.Query<Room>().Model<RoomItem>());
            return result;
        }

        public static RoomItem GetRoom(this CacheManager cm, int roomId)
        {
            return cm.Rooms().FirstOrDefault(x => x.RoomID == roomId);
        }
    }

    public static class ClientPrivilegeExtenstions
    {
        public static bool HasPriv(this ClientPrivilege priv1, ClientPrivilege priv2)
        {
            return (priv1 & priv2) > 0;
        }
    }
}
