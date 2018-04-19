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
        public static AccountModel GetAccount(this CacheManager cm, int accountId)
        {
            IList<AccountModel> list = cm.GetContextItem<IList<AccountModel>>("Accounts");

            if (list == null)
            {
                list = new List<AccountModel>();
                cm.SetContextItem("Accounts", list);
            }

            AccountModel result = list.FirstOrDefault(x => x.AccountID == accountId);

            if (result == null)
            {
                result = DA.Current.Query<AccountInfo>().FirstOrDefault(x => x.AccountID == accountId).Model<AccountModel>();
                list.Add(result);
            }

            return result;
        }

        public static IList<ClientAccountModel> ClientAccounts(this CacheManager cm, int clientId)
        {
            string key = "ClientAccounts#" + clientId.ToString();

            IList<ClientAccountModel> result = cm.GetContextItem<IList<ClientAccountModel>>(key);

            if (result == null || result.Count == 0)
            {
                result = DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId).Model<ClientAccountModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ClientAccountModel> ActiveClientAccounts(this CacheManager cm, int clientId)
        {
            return cm.ClientAccounts(clientId).Where(x => x.ClientAccountActive && x.ClientOrgActive).ToList();
        }

        public static ClientAccountModel GetClientAccount(this CacheManager cm, int clientId, int accountId)
        {
            return cm.ClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);
        }

        public static ClientAccountModel GetActiveClientAccount(this CacheManager cm, int clientId, int accountId)
        {
            return cm.ActiveClientAccounts(clientId).FirstOrDefault(x => x.AccountID == accountId);
        }

        public static IList<ClientAccountModel> CurrentUserClientAccounts(this CacheManager cm)
        {
            return cm.ClientAccounts(cm.ClientID);
        }

        public static IList<ClientAccountModel> CurrentUserActiveClientAccounts(this CacheManager cm)
        {
            return cm.ActiveClientAccounts(cm.ClientID);
        }

        public static IList<ClientItem> ClientOrgs(this CacheManager cm, int clientId)
        {
            string key = "ClientOrgs#" + clientId.ToString();

            IList<ClientItem> result = cm.GetContextItem<IList<ClientItem>>(key);

            if (result == null || result.Count == 0)
            {
                result = DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId).Model<ClientItem>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ClientItem> ActiveClientOrgs(this CacheManager cm, int clientId)
        {
            return cm.ClientOrgs(clientId).Where(x => x.ClientOrgActive).ToList();
        }

        public static ClientItem GetClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.ClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        public static ClientItem GetActiveClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.ActiveClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        public static IList<ClientItem> CurrentUserClientOrgs(this CacheManager cm)
        {
            return cm.ClientOrgs(cm.ClientID);
        }

        public static IList<ClientItem> CurrentUserActiveClientOrgs(this CacheManager cm)
        {
            return cm.ActiveClientOrgs(cm.ClientID);
        }

        public static GlobalCostModel GetGlobalCost(this CacheManager cm)
        {
            GlobalCostModel result = cm.GetContextItem<GlobalCostModel>("GlobalCost");

            if (result == null)
            {
                result = DA.Current.Query<GlobalCost>().FirstOrDefault().Model<GlobalCostModel>();
                cm.SetContextItem("GlobalCost", result);
            }

            return result;
        }

        public static int GetBusinessDay(this CacheManager cm)
        {
            return cm.GetGlobalCost().BusinessDay;
        }

        /// <summary>
        /// Gets all rooms from cache. Cached for one request.
        /// </summary>
        public static IList<RoomModel> Rooms(this CacheManager cm)
        {
            IList<RoomModel> result = cm.GetContextItem<IList<RoomModel>>("Rooms");

            if (result == null || result.Count == 0)
            {
                result = DA.Current.Query<Room>().Model<RoomModel>();
                cm.SetContextItem("Rooms", result);
            }

            return result;
        }

        /// <summary>
        /// Gets the rooms specifed by filter from cache. Cached for one request.
        /// </summary>
        public static IList<RoomModel> Rooms(this CacheManager cm, Func<RoomModel, bool> filter)
        {
            var result = cm.Rooms().Where(filter).ToList();
            return result;
        }

        public static RoomModel GetRoom(this CacheManager cm, int roomId)
        {
            return cm.Rooms(x => x.RoomID == roomId).FirstOrDefault();
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
