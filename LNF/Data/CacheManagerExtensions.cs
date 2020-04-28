using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// Gets all active Orgs. Orgs are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<IOrg> Orgs(this ICache c) => c.GetValue("Orgs", p => p.Data.Org.GetActiveOrgs(), DateTimeOffset.Now.AddMinutes(30));

        /// <summary>
        /// Gets one active Org by OrgID. Orgs are cached for 30 minutes.
        /// </summary>
        public static IOrg GetOrg(this ICache c, int orgId) => c.Orgs().FirstOrDefault(x => x.OrgID == orgId);

        /// <summary>
        /// Gets all active Accounts. Accounts are cached for 30 minutes.
        /// </summary>
        public static IEnumerable<IAccount> Accounts(this ICache c) => c.GetValue("Accounts", p => p.Data.Account.GetActiveAccounts(), DateTimeOffset.Now.AddMinutes(30));

        /// <summary>
        /// Gets one active Account by AccountID. Accounts are cached for 30 minutes.
        /// </summary>
        public static IAccount GetAccount(this ICache c, int accountId) => c.Accounts().FirstOrDefault(x => x.AccountID == accountId);

        /// <summary>
        /// Gets all active ClientAccounts. ClientAccounts are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClientAccount> ClientAccounts(this ICache c) => c.GetValue("ClientAccounts", p => p.Data.Client.GetActiveClientAccounts(), DateTimeOffset.Now.AddMinutes(10));

        /// <summary>
        /// Gets the active ClientAccounts for a particular client. ClientAccounts are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClientAccount> GetClientAccounts(this ICache c, int clientId) => c.ClientAccounts().Where(x => x.ClientID == clientId);

        /// <summary>
        /// Gets one active ClientAccount for a particular client and account. ClientAccounts are cached for 30 minutes.
        /// </summary>
        public static IClientAccount GetClientAccount(this ICache c, int clientId, int accountId) => c.GetClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);

        /// <summary>
        /// Gets all active ClientOrgs. ClientOrgs are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClient> ClientOrgs(this ICache c) => c.GetValue("ClientOrgs", p => p.Data.Client.GetActiveClientOrgs(), DateTimeOffset.Now.AddMinutes(10));

        /// <summary>
        /// Gets all active ClientOrgs for a particular client. ClientOrgs are cached for 10 minutes.
        /// </summary>
        public static IEnumerable<IClient> GetClientOrgs(this ICache c, int clientId) => c.ClientOrgs().Where(x => x.ClientID == clientId);

        /// <summary>
        /// Gets one active ClientOrg for a particular client and org. ClientOrgs are cached for 30 minutes.
        /// </summary>
        public static IClient GetClientOrg(this ICache c, int clientId, int orgId) => c.GetClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);

        /// <summary>
        /// Gets the global cost config from cache. Using MemoryCache because this data rarely changes. 
        /// </summary>
        public static IGlobalCost GetGlobalCost(this ICache c) => c.GetValue("GlobalCost", p => p.Data.Cost.GetActiveGlobalCost(), DateTimeOffset.Now.AddDays(7));

        public static int GetBusinessDay(this ICache c) => c.GetGlobalCost().BusinessDay;

        /// <summary>
        /// Gets all rooms from cache. Using MemoryCache because this data rarely changes.
        /// </summary>
        public static IEnumerable<IRoom> Rooms(this ICache c) => c.GetValue("Rooms", p => p.Data.Room.GetActiveRooms(), DateTimeOffset.Now.AddDays(1));

        public static IRoom GetRoom(this ICache c, int roomId) => c.Rooms().FirstOrDefault(x => x.RoomID == roomId);

        /// <summary>
        /// Gets the active ClientAccounts for the current user. ClientAccounts are cached for 30 minutes.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public static IEnumerable<IClientAccount> GetCurrentUserClientAccounts(this ICache c) => throw new NotImplementedException(); //c.GetClientAccounts(c.CurrentUser.ClientID);

        /// <summary>
        /// Gets the active ClientOrgs for the current user. ClientOrgs are cached for 30 minutes.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public static IEnumerable<IClient> GetCurrentUserClientOrgs(this ICache c) => throw new NotImplementedException(); //c.GetClientOrgs(c.CurrentUser.ClientID);

    }

}
