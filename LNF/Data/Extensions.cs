using LNF.Billing;
using LNF.Cache;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public static class CacheManagerExtensions
    {
        internal static IMongoCollection<CacheObject<AccountModel>> GetAccountCollection(this CacheManager cm)
        {
            return cm.GetCollection<AccountModel>("accounts")
                .Expire(TimeSpan.FromHours(6), x => x.CreatedAt)
                .Unique(x => x.Value.AccountID);
        }

        internal static IMongoCollection<CacheObject<ClientAccountModel>> GetClientAccountCollection(this CacheManager cm)
        {
            return cm.GetCollection<ClientAccountModel>("clientAccounts")
                .Expire(TimeSpan.FromHours(1), x => x.CreatedAt)
                .Unique(x => x.Value.ClientAccountID);
        }

        internal static IMongoCollection<CacheObject<ClientModel>> GetClientOrgCollection(this CacheManager cm)
        {
            return cm.GetCollection<ClientModel>("clientOrgs")
                .Expire(TimeSpan.FromHours(1), x => x.CreatedAt)
                .Unique(x => x.Value.ClientOrgID);
        }

        internal static IMongoCollection<CacheObject<GlobalCostModel>> GetGlobalCostCollection(this CacheManager cm)
        {
            return cm.GetCollection<GlobalCostModel>("globalCosts")
                .Expire(TimeSpan.FromDays(365), x => x.CreatedAt)
                .Unique(x => x.Value.GlobalID);
        }

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
                var query = cm.GetAccountCollection().Query(x => x.Value.AccountID == accountId, () => CacheObjectFactory.CreateMany(DA.Current.Query<AccountInfo>().Where(x => x.AccountID == accountId).Model<AccountModel>()), false);
                result = query.FirstOrDefault().GetValue();
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
                var query = cm.GetClientAccountCollection().Query(x => x.Value.ClientID == clientId, () => CacheObjectFactory.CreateMany(DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId).Model<ClientAccountModel>()), false);
                result = query.GetValues();
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

        public static IList<ClientModel> ClientOrgs(this CacheManager cm, int clientId)
        {
            string key = "ClientOrgs#" + clientId.ToString();

            IList<ClientModel> result = cm.GetContextItem<IList<ClientModel>>(key);

            if (result == null || result.Count == 0)
            {
                var query = cm.GetClientOrgCollection().Query(x => x.Value.ClientID == clientId, () => CacheObjectFactory.CreateMany(DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId).Model<ClientModel>()), false);
                result = query.GetValues();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ClientModel> ActiveClientOrgs(this CacheManager cm, int clientId)
        {
            return cm.ClientOrgs(clientId).Where(x => x.ClientOrgActive).ToList();
        }

        public static ClientModel GetClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.ClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        public static ClientModel GetActiveClientOrg(this CacheManager cm, int clientId, int orgId)
        {
            return cm.ActiveClientOrgs(clientId).FirstOrDefault(x => x.OrgID == orgId);
        }

        public static IList<ClientModel> CurrentUserClientOrgs(this CacheManager cm)
        {
            return cm.ClientOrgs(cm.ClientID);
        }

        public static IList<ClientModel> CurrentUserActiveClientOrgs(this CacheManager cm)
        {
            return cm.ActiveClientOrgs(cm.ClientID);
        }

        public static GlobalCostModel GetGlobalCost(this CacheManager cm)
        {
            GlobalCostModel result = cm.GetContextItem<GlobalCostModel>("GlobalCost");

            if (result == null)
            {
                var query = cm.GetGlobalCostCollection().Query(x => true, () => CacheObjectFactory.CreateMany(DA.Current.Query<GlobalCost>().Model<GlobalCostModel>()), true);
                result = query.First().GetValue();
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

    public static class ActiveDataItemExtensions
    {
        /// <summary>
        /// Sets Active to false and updates ActiveLog
        /// </summary>
        public static void Disable(this IActiveDataItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int record = item.Record();
            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be saved first.", "item");

            item.Active = false;

            ActiveLog alog = DA.Current.Query<ActiveLog>().FirstOrDefault(x => x.Record == record && x.TableName == item.TableName() && x.DisableDate == null);

            // if an ActiveLog with null DisableDate does not already exist then there is no reason to do anything else
            if (alog != null)
            {
                alog.DisableDate = DateTime.Now.Date.AddDays(1);
            }
        }

        /// <summary>
        /// Sets Active to true and updates ActiveLog
        /// </summary>
        public static void Enable(this IActiveDataItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int record = item.Record();
            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be inserted first.", "item");

            item.Active = true;

            ActiveLog alog = DA.Current.Query<ActiveLog>().FirstOrDefault(x => x.Record == record && x.TableName == item.TableName() && x.DisableDate == null);

            // if an ActiveLog with null DisableDate already exists then there is no reason to create a new one or do anything else
            if (alog == null)
            {
                DA.Current.Insert(new ActiveLog()
                {
                    DisableDate = null,
                    EnableDate = DateTime.Now.Date,
                    Record = record,
                    TableName = item.TableName()
                });
            }
        }
    }

    public static class AccountExtensions
    {
        public static IQueryable<ClientAccount> ClientAccounts(this Account item)
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.Account == item);
        }

        public static FundingSource FundingSource(this Account item)
        {
            return DA.Current.Single<FundingSource>(item.FundingSourceID);
        }

        public static string FundingSourceName(this Account item)
        {
            var fs = item.FundingSource();

            if (fs != null)
                return fs.FundingSourceName;
            else
                return string.Empty;
        }

        public static TechnicalField TechnicalField(this Account item)
        {
            return DA.Current.Single<TechnicalField>(item.TechnicalFieldID);
        }

        public static string TechnicalFieldName(this Account item)
        {
            var tf = item.TechnicalField();

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }

        public static SpecialTopic SpecialTopic(this Account item)
        {
            return DA.Current.Single<SpecialTopic>(item.SpecialTopicID);
        }

        public static string SpecialTopicName(this Account item)
        {
            var st = item.SpecialTopic();

            if (st != null)
                return st.SpecialTopicName;
            else
                return string.Empty;
        }

        public static AccountChartFields GetChartFields(this Account item)
        {
            return new AccountChartFields(item);
        }

        public static string GetChartField(this Account item, ChartFieldName field)
        {
            AccountChartFields fields = item.GetChartFields();
            switch (field)
            {
                case ChartFieldName.Account:
                    return fields.Account;
                case ChartFieldName.Fund:
                    return fields.Fund;
                case ChartFieldName.Department:
                    return fields.Department;
                case ChartFieldName.Program:
                    return fields.Program;
                case ChartFieldName.Class:
                    return fields.Class;
                case ChartFieldName.Project:
                    return fields.Project;
                default:
                    return fields.ShortCode;
            }
        }

        public static string GetDeptRef(this Account item, DateTime period)
        {
            //this is the Project chart field based on OrgRecharge or Org.OrgType.Account

            var allOrgRecharge = DA.Current.Query<OrgRecharge>();

            var orgRecharge = allOrgRecharge.Where(x => x.Account.AccountID == item.AccountID && x.EnableDate < period.AddMonths(1) && (x.DisableDate == null || x.DisableDate > period)).OrderBy(x => x.OrgRechargeID).LastOrDefault();

            if (orgRecharge != null)
                return orgRecharge.Account.GetChartFields().Project;
            else
                return item.Org.OrgType.ChargeType.GetAccount().GetChartFields().Project;
        }
    }

    public static class OrgExtensions
    {
        public static IQueryable<Account> Accounts(this Org item)
        {
            return DA.Current.Query<Account>().Where(x => x.Org.OrgID == item.OrgID);
        }

        public static IQueryable<ClientOrg> ClientOrgs(this Org item)
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Org.OrgID == item.OrgID);
        }

        public static IQueryable<Department> Departments(this Org item)
        {
            return DA.Current.Query<Department>().Where(x => x.Org.OrgID == item.OrgID);
        }
    }

    public static class ClientExtensions
    {
        public static ClientInfo GetClientInfo(this Client item)
        {
            ClientInfo result = DA.Current.Single<ClientInfo>(item.ClientID);
            return result;
        }

        public static string PrimaryEmail(this Client item)
        {
            ClientInfo c = item.GetClientInfo();
            if (c == null) return string.Empty;
            return c.Email;
        }

        public static string PrimaryPhone(this Client item)
        {
            ClientInfo c = item.GetClientInfo();
            if (c == null) return string.Empty;
            return c.Phone;
        }

        public static Org PrimaryOrg(this Client item)
        {
            ClientInfo c = item.GetClientInfo();
            if (c == null) return null;
            return DA.Current.Single<Org>(c.OrgID);
        }

        public static ChargeType MaxChargeType(this Client item)
        {
            var result = item.ActiveClientOrgs()
                .Select(x => x.Org.OrgType.ChargeType)
                .OrderBy(x => x.ChargeTypeID)
                .LastOrDefault();

            return result;
        }

        /// <summary>
        /// Gets an unflitered list of ClientOrgs for this Client
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientOrg items</returns>
        public static IQueryable<ClientOrg> ClientOrgs(this Client item)
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == item.ClientID);
        }

        /// <summary>
        /// Gets ClientOrgs for this Client that are currently active
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientOrg items</returns>
        public static IQueryable<ClientOrg> ActiveClientOrgs(this Client item)
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == item.ClientID && x.Active);
        }

        /// <summary>
        /// Gets ClientOrgs for this Client that were active during the specified date range
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>A list of ClientOrg items</returns>
        public static IQueryable<ClientOrg> ActiveClientOrgs(this Client item, DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientOrg" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientOrg>(), o => o.Record, i => i.ClientOrgID, (outer, inner) => inner);
            return join.Where(x => x.Client.ClientID == item.ClientID);
        }

        /// <summary>
        /// Gets an unflitered list of ClientAccounts for this Client
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientAccount items</returns>
        public static IQueryable<ClientAccount> ClientAccounts(this Client item)
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client.ClientID == item.ClientID);
        }

        /// <summary>
        /// Gets ClientAccounts for this Client that are currently active
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientAccount items</returns>
        public static IQueryable<ClientAccount> ActiveClientAccounts(this Client item)
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client.ClientID == item.ClientID && x.Active && x.ClientOrg.Active);
        }

        /// <summary>
        /// Gets ClientAccounts for this Client that were active during the specified date range
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>A list of ClientAccount items</returns>
        public static IQueryable<ClientAccount> ActiveClientAccounts(this Client item, DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientAccount>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            return join.Where(x => x.ClientOrg.Client.ClientID == item.ClientID);
        }

        public static IList<Account> ActiveAccounts(this Client item)
        {
            var result = item.ClientAccounts().Where(x => x.Active && x.ClientOrg.Active && x.Account.Active).Select(x => x.Account).ToList();
            return result;
        }

        public static IQueryable<Account> ActiveAccounts(this Client item, DateTime sd, DateTime ed)
        {
            var result = item.ActiveClientAccounts(sd, ed).Select(x => x.Account).Distinct();
            return result;
        }

        public static TechnicalField TechnicalField(this Client item)
        {
            return DA.Current.Single<TechnicalField>(item.TechnicalFieldID);
        }

        public static string TechnicalFieldName(this Client item)
        {
            var tf = item.TechnicalField();

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }

        public static DateTime? LastReservation(this Client item)
        {
            return DA.Scheduler.Reservation.Query()
                .Where(x => x.Client == item && x.IsActive && x.IsStarted)
                .Max(x => x.ActualBeginDateTime);
        }

        public static string[] ActiveEmails(this Client item)
        {
            //this function returns the same result as sselData.dbo.udf_ClientEmails()
            return DA.Current.Query<ClientOrg>().Where(x => x.Client == item && x.Active).Select(x => x.Email).Distinct().ToArray();
        }

        public static string AccountEmail(this Client item, int accountId)
        {
            var query = ClientOrgUtility.SelectByClientAccount(item, DA.Current.Single<Account>(accountId));

            if (query.Count() > 0)
                return query.First().Email;
            else
                return string.Empty;
        }

        public static string AccountPhone(this Client item, int accountId)
        {
            var query = ClientOrgUtility.SelectByClientAccount(item, DA.Current.Single<Account>(accountId));

            if (query.Count() > 0)
                return query.First().Phone;
            else
                return string.Empty;
        }

        public static bool IsAdmin(this Client item)
        {
            return item.HasPriv(ClientPrivilege.Administrator);
        }

        public static bool IsStaff(this Client item)
        {
            return item.HasPriv(ClientPrivilege.Staff);
        }

        public static DateTime? LastRoomEntry(this Client item)
        {
            DateTime? result = DA.Current.Query<RoomDataClean>().Where(x => x.Client == item).Max(x => x.EntryDT);
            return result;
        }

        public static ClientOrgInfo GetClientOrgInfo(this Client item, int rank)
        {
            ClientOrgInfo result = DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientID == item.ClientID && x.EmailRank == rank).FirstOrDefault();
            return result;
        }

        public static IQueryable<ClientAccountInfo> ActiveClientAccountInfos(this Client item)
        {
            var result = DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == item.ClientID && x.ClientAccountActive && x.ClientOrgActive);
            return result;
        }

        public static int TotalDaysInLab(this Client item, Room r, DateTime period)
        {
            IList<RoomData> query = DA.Current.Query<RoomData>().Where(x => x.Client.ClientID == item.ClientID && x.Room == r && x.Period == period).ToList();
            IEnumerable<DateTime> days = query.Select(x => x.EvtDate);
            IEnumerable<int> distinctDays = days.Select(x => x.Day).Distinct();
            int result = distinctDays.Count();
            return result;
        }

        /// <summary>
        /// Checks to see if the given password is correct.
        /// </summary>
        /// <param name="item">The client for which this action is performed.</param>
        /// <param name="password">An unencrypted password.</param>
        /// <returns>True if the password is correct, otherwise false.</returns>
        public static bool CheckPassword(this Client item, string password)
        {
            return ClientUtility.CheckPassword(item.ClientID, password);
        }

        /// <summary>
        /// Sets the client password to the given value.
        /// </summary>
        /// <param name="item">The client for which this action is performed.</param>
        /// <param name="password">An unencrypted password.</param>
        /// <returns>The number of rows updated.</returns>
        public static int SetPassword(this Client item, string password)
        {
            var pw = Providers.Encryption.EncryptText(password);
            var hash = Providers.Encryption.Hash(password);

            int result = 0;

            using (var dba = DA.Current.GetAdapter())
            {
                result = dba
                    .ApplyParameters(new { Action = "SetPassword", ClientID = item.ClientID, Password = pw, PasswordHash = hash })
                    .ExecuteNonQuery("dbo.Client_Password");
            }

            return result;
        }

        /// <summary>
        /// Sets the client password to the UserName.
        /// </summary>
        public static void ResetPassword(this Client item)
        {
            item.SetPassword(item.UserName);
        }
    }

    public static class ClientInfoExtensions
    {
        public static IQueryable<ClientOrgInfo> ClientOrgs(this ClientInfo item)
        {
            return DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientID == item.ClientID);
        }

        public static IQueryable<ClientAccountInfo> ClientAccounts(this ClientInfo item)
        {
            return DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == item.ClientID);
        }

        public static Client GetClient(this ClientInfo item)
        {
            return ClientUtility.Find(item.ClientID);
        }
    }

    public static class ClientOrgExtensions
    {
        public static IQueryable<ClientAccount> ClientAccounts(this ClientOrg item)
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID);
        }

        public static IQueryable<ClientManager> Managers(this ClientOrg item)
        {
            return DA.Current.Query<ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == item.ClientOrgID);
        }

        public static IQueryable<ClientManager> Employees(this ClientOrg item)
        {
            return DA.Current.Query<ClientManager>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID);
        }

        public static ClientAccount GetDryBoxClientAccount(this ClientOrg item)
        {
            IList<ClientAccount> query = DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID).ToList();
            ClientAccount ca = query.FirstOrDefault(x => x.HasDryBox());
            return ca;
        }

        public static bool HasDryBox(this ClientOrg item)
        {
            return item.GetDryBoxClientAccount() != null;
        }

        public static BillingType GetBillingType(this ClientOrg item)
        {
            var logs = DA.Current.Query<ClientOrgBillingTypeLog>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID);

            ClientOrgBillingTypeLog active = logs.FirstOrDefault(x => x.DisableDate == null);

            if (active != null)
                return active.BillingType;

            return logs.OrderBy(x => x.DisableDate).Last().BillingType;
        }

        public static ClientOrgInfo GetClientOrgInfo(this ClientOrg item)
        {
            return DA.Current.Single<ClientOrgInfo>(item.ClientOrgID);
        }

        public static void Disable(this ClientOrg item)
        {
            // when we disable a ClientOrg we might have to also disable the Client and/or disable physical access

            ((IActiveDataItem)item).Disable(); // normal disable of ClientOrg

            // first check for other active ClientOrgs, this one won't be included because it was just disabled
            bool otherActive = item.Client.ClientOrgs().Any(x => x.Active);

            if (!otherActive)
                item.Client.Disable();

            // be sure to check physical access after this
        }
    }

    public static class ClientAccountExtensions
    {
        public static bool HasDryBox(this ClientAccount item)
        {
            IList<DryBoxAssignment> query = DA.Current.Query<DryBoxAssignment>().Where(x => x.ClientAccount.ClientAccountID == item.ClientAccountID).ToList();
            DryBoxAssignment dba = query.FirstOrDefault(x => x.GetStatus() == DryBoxAssignmentStatus.Active);
            return dba != null;
        }
    }

    public static class ClientRemoteExtensions
    {
        public static void Enable(this ClientRemote item, DateTime period)
        {
            ActiveLog alog = null;

            IList<ActiveLog> alogs = DA.Current.Query<ActiveLog>().Where(x => x.TableName == item.TableName() && x.Record == item.Record()).ToList();

            if (alogs.Count > 0)
            {
                alog = alogs[0];

                // clean up invalid records, if any
                if (alogs.Count > 1)
                    DA.Current.Delete(alogs.Skip(1).ToArray());
            }

            if (alog != null)
            {
                alog.EnableDate = period;
                alog.DisableDate = period.AddMonths(1);
            }
            else
            {
                alog = new ActiveLog()
                {
                    TableName = item.TableName(),
                    Record = item.Record(),
                    EnableDate = period,
                    DisableDate = period.AddMonths(1)
                };
            }

            DA.Current.SaveOrUpdate(alog);
        }
    }

    public static class AccountTypeExtensions
    {
        public static IQueryable<Account> Accounts(this AccountType item)
        {
            return DA.Current.Query<Account>().Where(x => x.AccountType.AccountTypeID == item.AccountTypeID);
        }
    }

    public static class OrgTypeExtensions
    {
        public static IQueryable<Org> Orgs(this OrgType item)
        {
            return DA.Current.Query<Org>().Where(x => x.OrgType.OrgTypeID == item.OrgTypeID);
        }
    }

    public static class ChargeTypeExtensions
    {
        public static Account GetAccount(this ChargeType item)
        {
            return DA.Current.Single<Account>(item.AccountID);
        }

        public static IQueryable<OrgType> OrgTypes(this ChargeType item)
        {
            return DA.Current.Query<OrgType>().Where(x => x.ChargeType.ChargeTypeID == item.ChargeTypeID);
        }
    }

    public static class ActiveLogExtensions
    {
        public static T Item<T>(this ActiveLog item) where T : IActiveDataItem
        {
            T entity = Activator.CreateInstance<T>();
            string tableName = entity.TableName();
            if (tableName == item.TableName)
                return DA.Current.Single<T>(item.Record);
            else
                return default(T);
        }
    }

    public static class DryBoxExtensions
    {
        public static DryBoxAssignment CurrentAssignment(this DryBox item)
        {
            return DA.Current.Query<DryBoxAssignment>()
                .FirstOrDefault(x => x.DryBox.DryBoxID == item.DryBoxID && x.RemovedDate == null);
        }

        public static bool? IsAccountActive(this DryBox item)
        {
            var dba = item.CurrentAssignment();

            // this only applies to dry boxes where an account has been assigned
            // so return null if there is no assignment

            if (dba == null)
                return null;

            return dba.ClientAccount.Active && dba.ClientAccount.ClientOrg.Active;
        }
    }

    public static class DryBoxAssignmentLogExtensions
    {
        public static ClientAccountInfo GetClientAccountInfo(this DryBoxAssignmentLog item)
        {
            ClientAccountInfo result = DA.Current.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientAccountID == item.ClientAccount.ClientAccountID);
            return result;
        }
    }

    public static class NewsExtensions
    {
        /// <summary>
        /// Deletes a News item
        /// </summary>
        public static void Delete(this News item)
        {
            item.NewsUpdatedByClientID = CacheManager.Current.CurrentUser.ClientID;
            item.NewsLastUpdate = DateTime.Now;
            item.NewsDeleted = true;
        }

        /// <summary>
        /// Restores a previously deleted News item
        /// </summary>
        public static void Undelete(this News item)
        {
            item.NewsUpdatedByClientID = CacheManager.Current.CurrentUser.ClientID;
            item.NewsLastUpdate = DateTime.Now;
            item.NewsDeleted = false;
        }

        public static void SetDefault(this News item)
        {
            News d = DA.Current.Query<News>().FirstOrDefault(x => x.NewsDefault);
            d.NewsDefault = false;

            item.NewsUpdatedByClientID = CacheManager.Current.CurrentUser.ClientID;
            item.NewsLastUpdate = DateTime.Now;
            item.NewsDefault = true;
        }
    }

    public static class RoomDataExtensions
    {
        public static string GetEmail(this RoomData item)
        {
            ClientAccountInfo ca = DA.Current
                .Query<ClientAccountInfo>()
                .FirstOrDefault(x => x.ClientID == item.Client.ClientID && x.AccountID == item.Account.AccountID);

            if (ca == null) return string.Empty;

            return ca.Email;
        }
    }

    public static class ToolDataExtensions
    {
        //RoomID might be null
        public static Room GetRoom(this ToolData item)
        {
            if (item.RoomID.HasValue)
                return DA.Current.Single<Room>(item.RoomID.Value);
            else
                return null;
        }

        //ReservationID might be null
        public static Reservation GetReservation(this ToolData item)
        {
            if (item.ReservationID.HasValue)
                return DA.Scheduler.Reservation.Single(item.ReservationID.Value);
            else
                return null;
        }

        public static BillingType GetBillingType(this ToolData item)
        {
            return BillingTypeUtility.GetBillingType(DA.Current.Single<Client>(item.ClientID), DA.Current.Single<Account>(item.AccountID), item.Period);
        }
    }

    public static class IActiveDataItemExtenstions
    {
        /// <summary>
        /// Gets all ActiveLog entities for the IActiveDataItem item.
        /// </summary>
        public static IQueryable<ActiveLog> ActiveLogs(this IActiveDataItem item)
        {
            return DA.Current.Query<ActiveLog>().Where(x => x.TableName == item.TableName() && x.Record == item.Record());
        }

        /// <summary>
        /// Gets a collection of ActiveLogItems that were active during the date range and have a matching record in the collection of IActiveDataItems.
        /// </summary>
        public static IEnumerable<ActiveLogItem<IActiveDataItem>> Range(this IEnumerable<IActiveDataItem> list, DateTime startDate, DateTime endDate)
        {
            DateRange range = new DateRange(startDate, endDate);
            IEnumerable<int> records = list.Select(x => x.Record());
            IEnumerable<ActiveLogItem<IActiveDataItem>> result = range.Items(list);
            return result;
        }
    }
}
