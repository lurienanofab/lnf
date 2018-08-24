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
        /// <summary>
        /// Gets all active Orgs. Orgs are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<OrgItem> Orgs(this CacheManager cm)
        {
            IList<OrgItem> result;

            var value = cm.GetMemoryCacheValue("Orgs");

            if (value == null)
            {
                result = DA.Current.Query<OrgInfo>().Where(x => x.OrgActive && x.OrgActive).Model<OrgItem>();
                cm.SetMemoryCacheValue("Orgs", result, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
            }
            else
            {
                result = (IList<OrgItem>)value;
            }

            return result;
        }

        /// <summary>
        /// Gets one active Org by OrgID. Orgs are cached for 30 minutes.
        /// </summary>
        public static OrgItem GetOrg(this CacheManager cm, int orgId)
        {
            return cm.Orgs().FirstOrDefault(x => x.OrgID == orgId);
        }

        /// <summary>
        /// Gets all active Accounts. Accounts are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<AccountItem> Accounts(this CacheManager cm)
        {
            IList<AccountItem> result;

            var value = cm.GetMemoryCacheValue("Accounts");

            if (value == null)
            {
                result = DA.Current.Query<AccountInfo>().Where(x => x.AccountActive && x.OrgActive).Model<AccountItem>();
                cm.SetMemoryCacheValue("Accounts", result, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
            }
            else
            {
                result = (IList<AccountItem>)value;
            }

            return result;
        }

        /// <summary>
        /// Gets one active Account by AccountID. Accounts are cached for 30 minutes.
        /// </summary>
        public static AccountItem GetAccount(this CacheManager cm, int accountId)
        {
            return cm.Accounts().FirstOrDefault(x => x.AccountID == accountId);
        }

        /// <summary>
        /// Gets all active ClientAccounts. ClientAccounts are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<ClientAccountItem> ClientAccounts(this CacheManager cm)
        {
            IList<ClientAccountItem> result;

            var value = cm.GetMemoryCacheValue("ClientAccounts");

            if (value == null)
            {
                result = DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientAccountActive && x.ClientOrgActive).Model<ClientAccountItem>();
                cm.SetMemoryCacheValue("ClientAccounts", result, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
            }
            else
            {
                result = (IList<ClientAccountItem>)value;
            }

            return result;
        }

        /// <summary>
        /// Gets the active ClientAccounts for a particular client. ClientAccounts are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<ClientAccountItem> GetClientAccounts(this CacheManager cm, int clientId)
        {
            return cm.ClientAccounts().Where(x => x.ClientID == clientId).ToList();
        }

        /// <summary>
        /// Gets one active ClientAccount for a particular client and account. ClientAccounts are cached for 30 minutes.
        /// </summary>
        public static ClientAccountItem GetClientAccount(this CacheManager cm, int clientId, int accountId)
        {
            return cm.GetClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);
        }

        /// <summary>
        /// Gets the active ClientAccounts for the current user. ClientAccounts are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<ClientAccountItem> GetCurrentUserClientAccounts(this CacheManager cm)
        {
            return cm.GetClientAccounts(cm.CurrentUser.ClientID);
        }

        /// <summary>
        /// Gets all active ClientOrgs. ClientOrgs are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<ClientItem> ClientOrgs(this CacheManager cm)
        {
            IList<ClientItem> result;

            var value = cm.GetMemoryCacheValue("ClientOrgs");

            if (value == null)
            {
                result = DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientOrgActive).Model<ClientItem>();
                cm.SetMemoryCacheValue("ClientOrgs", result, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
            }
            else
            {
                result = (IList<ClientItem>)value;
            }

            return result;
        }

        /// <summary>
        /// Gets all active ClientOrgs for a particular client. ClientOrgs are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<ClientItem> GetClientOrgs(this CacheManager cm, int clientId)
        {
            return cm.ClientOrgs().Where(x => x.ClientID == clientId);
        }

        /// <summary>
        /// Gets one active ClientOrg for a particular client and org. ClientOrgs are cached for 30 minutes.
        /// </summary>
        public static ClientItem GetClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.GetClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        /// <summary>
        /// Gets the active ClientOrgs for the current user. ClientOrgs are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<ClientItem> GetCurrentUserClientOrgs(this CacheManager cm)
        {
            return cm.GetClientOrgs(cm.CurrentUser.ClientID);
        }

        // Gets the global cost config from cache. Using MemoryCache because this data rarely changes.
        public static GlobalCostItem GetGlobalCost(this CacheManager cm)
        {
            GlobalCostItem result;

            var value = cm.GetMemoryCacheValue("GlobalCost");

            if (value == null)
            {
                result = DA.Current.Query<GlobalCost>().FirstOrDefault().Model<GlobalCostItem>();
                cm.SetMemoryCacheValue("GlobalCost", result, DateTimeOffset.Now.Add(TimeSpan.FromDays(7)));
            }
            else
            {
                result = (GlobalCostItem)value;
            }
            
            return result;
        }

        public static int GetBusinessDay(this CacheManager cm)
        {
            return cm.GetGlobalCost().BusinessDay;
        }

        /// <summary>
        /// Gets all rooms from cache. Using MemoryCache because this data rarely changes.
        /// </summary>
        public static IEnumerable<RoomItem> Rooms(this CacheManager cm)
        {
            IList<RoomItem> result;

            var value = cm.GetMemoryCacheValue("Rooms");

            if (value == null)
            {
                result = DA.Current.Query<Room>().Model<RoomItem>();
                cm.SetMemoryCacheValue("Rooms", result, DateTimeOffset.Now.AddDays(1));
            }
            else
            {
                result = (IList<RoomItem>)value;
            }
            
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
