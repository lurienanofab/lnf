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
        public static IEnumerable<OrgItem> Orgs(this CacheManager cm) => cm.GetValue("Orgs", () => DA.Current.Query<OrgInfo>().Where(x => x.OrgActive && x.OrgActive).CreateModels<OrgItem>(), DateTimeOffset.Now.AddMinutes(30));

        /// <summary>
        /// Gets one active Org by OrgID. Orgs are cached for 30 minutes.
        /// </summary>
        public static OrgItem GetOrg(this CacheManager cm, int orgId) => cm.Orgs().FirstOrDefault(x => x.OrgID == orgId);

        /// <summary>
        /// Gets all active Accounts. Accounts are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<IAccount> Accounts(this CacheManager cm) => cm.GetValue("Accounts", () => DA.Current.Query<AccountInfo>().Where(x => x.AccountActive && x.OrgActive).CreateModels<IAccount>(), DateTimeOffset.Now.AddMinutes(30));

        /// <summary>
        /// Gets one active Account by AccountID. Accounts are cached for 30 minutes.
        /// </summary>
        public static IAccount GetAccount(this CacheManager cm, int accountId) => cm.Accounts().FirstOrDefault(x => x.AccountID == accountId);

        /// <summary>
        /// Gets all active ClientAccounts. ClientAccounts are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClientAccount> ClientAccounts(this CacheManager cm) => cm.GetValue("ClientAccounts", () => DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientAccountActive && x.ClientOrgActive).CreateModels<ClientAccountItem>(), DateTimeOffset.Now.AddMinutes(10));

        /// <summary>
        /// Gets the active ClientAccounts for a particular client. ClientAccounts are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClientAccount> GetClientAccounts(this CacheManager cm, int clientId) => cm.ClientAccounts().Where(x => x.ClientID == clientId);

        /// <summary>
        /// Gets one active ClientAccount for a particular client and account. ClientAccounts are cached for 30 minutes.
        /// </summary>
        public static IClientAccount GetClientAccount(this CacheManager cm, int clientId, int accountId) => cm.GetClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);

        /// <summary>
        /// Gets the active ClientAccounts for the current user. ClientAccounts are cached for 30 minutes.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public static IEnumerable<IClientAccount> GetCurrentUserClientAccounts(this CacheManager cm) => cm.GetClientAccounts(cm.CurrentUser.ClientID);

        /// <summary>
        /// Gets all active ClientOrgs. ClientOrgs are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClient> ClientOrgs(this CacheManager cm) => cm.GetValue("ClientOrgs", () => DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientOrgActive).CreateModels<IClient>(), DateTimeOffset.Now.AddMinutes(10));

        /// <summary>
        /// Gets all active ClientOrgs for a particular client. ClientOrgs are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClient> GetClientOrgs(this CacheManager cm, int clientId) => cm.ClientOrgs().Where(x => x.ClientID == clientId);

        /// <summary>
        /// Gets one active ClientOrg for a particular client and org. ClientOrgs are cached for 30 minutes.
        /// </summary>
        public static IClient GetClientOrg(this CacheManager cm, int clientId, int orgId) => cm.GetClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);

        /// <summary>
        /// Gets the active ClientOrgs for the current user. ClientOrgs are cached for 30 minutes.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public static IEnumerable<IClient> GetCurrentUserClientOrgs(this CacheManager cm) => cm.GetClientOrgs(cm.CurrentUser.ClientID);

        /// <summary>
        /// Gets the global cost config from cache. Using MemoryCache because this data rarely changes. 
        /// </summary>
        /// <param name="cm"></param>
        /// <returns></returns>
        public static GlobalCostItem GetGlobalCost(this CacheManager cm) => cm.GetValue("GlobalCost", () => DA.Current.Query<GlobalCost>().FirstOrDefault().CreateModel<GlobalCostItem>(), DateTimeOffset.Now.AddDays(7));

        public static int GetBusinessDay(this CacheManager cm) => cm.GetGlobalCost().BusinessDay;

        /// <summary>
        /// Gets all rooms from cache. Using MemoryCache because this data rarely changes.
        /// </summary>
        public static IEnumerable<IRoom> Rooms(this CacheManager cm) => cm.GetValue("Rooms", () => DA.Current.Query<Room>().CreateModels<IRoom>(), DateTimeOffset.Now.AddDays(1));

        public static IRoom GetRoom(this CacheManager cm, int roomId) => cm.Rooms().FirstOrDefault(x => x.RoomID == roomId);
    }

    public static class ClientPrivilegeExtenstions
    {
        public static bool HasPriv(this ClientPrivilege priv1, ClientPrivilege priv2)
        {
            return (priv1 & priv2) > 0;
        }
    }
}
