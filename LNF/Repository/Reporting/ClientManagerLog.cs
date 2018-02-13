using LNF.Models.Data;
using System;
using System.Linq;

namespace LNF.Repository.Reporting
{
    public class ClientManagerLog : IDataItem
    {
        public virtual int ManagerLogID { get; set; }
        public virtual string ManagerTableName { get; set; }
        public virtual int ManagerRecord { get; set; }
        public virtual DateTime ManagerEnableDate { get; set; }
        public virtual DateTime? ManagerDisableDate { get; set; }
        public virtual int UserLogID { get; set; }
        public virtual string UserTableName { get; set; }
        public virtual int UserRecord { get; set; }
        public virtual DateTime UserEnableDate { get; set; }
        public virtual DateTime? UserDisableDate { get; set; }
        public virtual int ManagerClientID { get; set; }
        public virtual string ManagerUserName { get; set; }
        public virtual string ManagerLName { get; set; }
        public virtual string ManagerFName { get; set; }
        public virtual string ManagerEmail { get; set; }
        public virtual int UserClientID { get; set; }
        public virtual string UserUserName { get; set; }
        public virtual string UserLName { get; set; }
        public virtual string UserFName { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual bool ManagerIsTechnicalManager { get; set; }
        public virtual bool ManagerIsFinancialManager { get; set; }
        public virtual bool UserIsTechnicalManager { get; set; }
        public virtual bool UserIsFinancialManager { get; set; }

        /// <summary>
        /// Indicates if the user is also an account manager.
        /// </summary>
        public virtual bool UserManager { get; set; }

        public virtual ClientPrivilege UserPrivs { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual bool IsSubsidyOrg { get; set; }
        public virtual bool IsRemote { get; set; }
        public virtual int RemoteClientClientID { get; set; }
        public virtual string RemoteClientUserName { get; set; }
        public virtual string RemoteClientLName { get; set; }
        public virtual string RemoteClientFName { get; set; }
        public virtual string RemoteClientEmail { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            ClientManagerLog item = obj as ClientManagerLog;

            if (item == null) return false;

            return item.ManagerLogID == ManagerLogID && item.UserLogID == UserLogID;
        }

        public override int GetHashCode()
        {
            return new { ManagerLogID, UserLogID }.GetHashCode();
        }

        /// <summary>
        /// Selects all manager/client account relationships that were active during the specified date range.
        /// </summary>
        public static IQueryable<ClientManagerLog> SelectByManager(int clientId, DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ClientManagerLog>().Where(x => x.ManagerClientID == clientId
                && (x.ManagerEnableDate < ed && (x.ManagerDisableDate == null || x.ManagerDisableDate.Value > sd))
                && (x.UserEnableDate < ed && (x.UserDisableDate == null || x.UserDisableDate.Value > sd)));

            return query;
        }

        public static IQueryable<ClientManagerLog> SelectByPeriod(DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ClientManagerLog>().Where(x =>
                (x.ManagerEnableDate < ed && (x.ManagerDisableDate == null || x.ManagerDisableDate.Value > sd))
                && (x.UserEnableDate < ed && (x.UserDisableDate == null || x.UserDisableDate.Value > sd)));

            return query;
        }
    }
}
