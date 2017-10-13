using LNF.Data;
using LNF.Models.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class Client : ActiveDataItem, IPrivileged
    {
        /// <summary>
        /// The unique id of a Client
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// The first name of a Client
        /// </summary>
        public virtual string FName { get; set; }

        /// <summary>
        /// The middle name of a Client
        /// </summary>
        public virtual string MName { get; set; }

        /// <summary>
        /// The last name of a Client
        /// </summary>
        public virtual string LName { get; set; }

        /// <summary>
        /// The unique username of a Client used to log in
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Id used to indicate citizenship demographic data
        /// </summary>
        public virtual int DemCitizenID { get; set; }

        /// <summary>
        /// Id used to indicate gender demographic data
        /// </summary>
        public virtual int DemGenderID { get; set; }

        /// <summary>
        /// Id used to indicate racial demographic data
        /// </summary>
        public virtual int DemRaceID { get; set; }

        /// <summary>
        /// Id used to indicate ethnicity demographic data
        /// </summary>
        public virtual int DemEthnicID { get; set; }

        /// <summary>
        /// Id used to indicate disability demographic data
        /// </summary>
        public virtual int DemDisabilityID { get; set; }

        /// <summary>
        /// The privilges granted to a Client
        /// </summary>
        public virtual ClientPrivilege Privs { get; set; }

        /// <summary>
        /// The communities assigned to a Client
        /// </summary>
        public virtual int Communities { get; set; }

        /// <summary>
        /// Id used to indicate a technical field to which a Client is associated
        /// </summary>
        public virtual int TechnicalFieldID { get; set; }

        /// <summary>
        /// Indicates if a Client has viewed the new user ethics video
        /// </summary>
        public virtual bool? IsChecked { get; set; }

        /// <summary>
        /// Indicates if a Client has passed the new user safety test
        /// </summary>
        public virtual bool? IsSafetyTest { get; set; }

        /// <summary>
        /// Indicates if a Client is currently active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// The display name of a Client
        /// </summary>
        public virtual string DisplayName
        {
            get { return ClientModel.GetDisplayName(LName, FName); }
        }

        /// <summary>
        /// Gets the record id used in the ActiveLog
        /// </summary>
        /// <returns>A ClientID integer value</returns>
        public override int Record() { return ClientID; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        public override string TableName() { return "Client"; }

        /// <summary>
        /// Gets the non null value of IsChecked
        /// </summary>
        /// <returns>True if a Client has watched the ethical video, otherwise false</returns>
        public virtual bool HasWatchedEthicalVideo()
        {
            return IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the non null value of IsSafetyTest
        /// </summary>
        /// <returns>True if a Client has completed the safety test, otherwise false</returns>
        public virtual bool HasTakenSafetyTest()
        {
            return IsSafetyTest.GetValueOrDefault();
        }

        public virtual ClientInfo GetClientInfo()
        {
            ClientInfo result = DA.Current.Single<ClientInfo>(ClientID);
            return result;
        }

        public virtual string PrimaryEmail()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return string.Empty;
            return c.Email;
        }

        public virtual string PrimaryPhone()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return string.Empty;
            return c.Phone;
        }

        public virtual Org PrimaryOrg()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return null;
            return DA.Current.Single<Org>(c.OrgID);
        }

        public virtual ClientOrg PrimaryClientOrg()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return null;
            return DA.Current.Single<ClientOrg>(c.ClientOrgID);
        }

        public virtual ChargeType MaxChargeType()
        {
            var result = ActiveClientOrgs()
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
        public virtual IQueryable<ClientOrg> ClientOrgs()
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == ClientID);
        }

        /// <summary>
        /// Gets ClientOrgs for this Client that are currently active
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientOrg items</returns>
        public virtual IQueryable<ClientOrg> ActiveClientOrgs()
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == ClientID && x.Active);
        }

        /// <summary>
        /// Gets ClientOrgs for this Client that were active during the specified date range
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>A list of ClientOrg items</returns>
        public virtual IQueryable<ClientOrg> ActiveClientOrgs(DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientOrg" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientOrg>(), o => o.Record, i => i.ClientOrgID, (outer, inner) => inner);
            return join.Where(x => x.Client.ClientID == ClientID);
        }

        /// <summary>
        /// Gets an unflitered list of ClientAccounts for this Client
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientAccount items</returns>
        public virtual IQueryable<ClientAccount> ClientAccounts()
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client.ClientID == ClientID);
        }

        /// <summary>
        /// Gets ClientAccounts for this Client that are currently active
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientAccount items</returns>
        public virtual IQueryable<ClientAccount> ActiveClientAccounts()
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client.ClientID == ClientID && x.Active && x.ClientOrg.Active);
        }

        /// <summary>
        /// Gets ClientAccounts for this Client that were active during the specified date range
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>A list of ClientAccount items</returns>
        public virtual IQueryable<ClientAccount> ActiveClientAccounts(DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientAccount>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            return join.Where(x => x.ClientOrg.Client.ClientID == ClientID);
        }

        public virtual IList<Account> ActiveAccounts()
        {
            var result = ClientAccounts().Where(x => x.Active && x.ClientOrg.Active && x.Account.Active).Select(x => x.Account).ToList();
            return result;
        }

        public virtual IQueryable<Account> ActiveAccounts(DateTime sd, DateTime ed)
        {
            var result = ActiveClientAccounts(sd, ed).Select(x => x.Account).Distinct();
            return result;
        }

        public virtual TechnicalField TechnicalField()
        {
            return DA.Current.Single<TechnicalField>(TechnicalFieldID);
        }

        public virtual string TechnicalFieldName()
        {
            var tf = TechnicalField();

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }

        public virtual DateTime? LastReservation()
        {
            return DA.Scheduler.Reservation.Query()
                .Where(x => x.Client == this && x.IsActive && x.IsStarted)
                .Max(x => x.ActualBeginDateTime);
        }

        public virtual string[] ActiveEmails()
        {
            //this function returns the same result as sselData.dbo.udf_ClientEmails()
            return DA.Current.Query<ClientOrg>().Where(x => x.Client == this && x.Active).Select(x => x.Email).Distinct().ToArray();
        }

        public virtual string AccountEmail(int accountId)
        {
            var co = ClientOrgUtility.SelectByClientAccount(this, DA.Current.Single<Account>(accountId)).FirstOrDefault();

            if (co != null)
                return co.Email;
            else
                return string.Empty;
        }

        public virtual string AccountPhone(int accountId)
        {
            var query = ClientOrgUtility.SelectByClientAccount(this, DA.Current.Single<Account>(accountId));

            if (query.Count() > 0)
                return query.First().Phone;
            else
                return string.Empty;
        }

        public virtual bool IsAdmin()
        {
            return this.HasPriv(ClientPrivilege.Administrator);
        }

        public virtual bool IsStaff()
        {
            return this.HasPriv(ClientPrivilege.Staff);
        }

        public virtual DateTime? LastRoomEntry()
        {
            DateTime? result = DA.Current.Query<RoomDataClean>().Where(x => x.Client == this).Max(x => x.EntryDT);
            return result;
        }

        public virtual ClientOrgInfo GetClientOrgInfo(int rank)
        {
            ClientOrgInfo result = DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientID == ClientID && x.EmailRank == rank).FirstOrDefault();
            return result;
        }

        public virtual IQueryable<ClientAccountInfo> ActiveClientAccountInfos()
        {
            var result = DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == ClientID && x.ClientAccountActive && x.ClientOrgActive);
            return result;
        }

        public virtual int TotalDaysInLab(Room r, DateTime period)
        {
            IList<RoomData> query = DA.Current.Query<RoomData>().Where(x => x.Client.ClientID == ClientID && x.Room == r && x.Period == period).ToList();
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
        public virtual bool CheckPassword(string password)
        {
            return ClientUtility.CheckPassword(ClientID, password);
        }

        /// <summary>
        /// Sets the client password to the given value.
        /// </summary>
        /// <param name="item">The client for which this action is performed.</param>
        /// <param name="password">An unencrypted password.</param>
        /// <returns>The number of rows updated.</returns>
        public virtual int SetPassword(string password)
        {
            return ClientUtility.SetPassword(ClientID, password);
        }

        /// <summary>
        /// Sets the client password to the UserName.
        /// </summary>
        public virtual void ResetPassword()
        {
            SetPassword(UserName);
        }

        /// <summary>
        /// Gets a Client display name in the standard format
        /// </summary>
        [Obsolete("Use LNF.Models.Data.ClientModel.GetDisplayName instead")]
        public static string GetDisplayName(string lname, string fname)
        {
            return string.Join(", ", new[] { lname, fname }.Where(x => !string.IsNullOrEmpty(x))).Trim();
        }

        public virtual void Update(ClientModel model)
        {
            FName = model.FName;
            MName = model.MName;
            LName = model.LName;
            UserName = model.UserName;
            DemCitizenID = model.DemCitizenID;
            DemGenderID = model.DemGenderID;
            DemRaceID = model.DemRaceID;
            DemEthnicID = model.DemEthnicID;
            DemDisabilityID = model.DemDisabilityID;
            Privs = model.Privs;
            Communities = model.Communities;
            TechnicalFieldID = model.TechnicalFieldID;
            IsChecked = model.IsChecked;
            IsSafetyTest = model.IsSafetyTest;

            if (model.ClientActive)
                this.Enable();
            else
                this.Disable();
        }
    }
}
