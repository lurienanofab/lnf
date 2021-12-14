using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class DryBoxRepository : RepositoryBase, IDryBoxRepository
    {
        public DryBoxRepository(ISessionManager mgr) : base(mgr) { }

        public DryBoxAssignmentInfo GetCurrentDryBoxAssignment(int dryBoxId)
        {
            var dba = Session.Query<DryBoxAssignment>().FirstOrDefault(x => x.DryBoxID == dryBoxId && x.RemovedDate == null);
            var db = Require<DryBox>(dryBoxId);
            var ca = Require<ClientAccount>(dba.ClientAccountID);
            return CreateDryBoxAssignmentInfo(dba, db, ca);
        }

        public bool? IsAccountActive(int dryBoxId)
        {
            var dba = GetCurrentDryBoxAssignment(dryBoxId);

            // this only applies to dry boxes where an account has been assigned
            // so return null if there is no assignment

            if (dba == null)
                return null;

            var ca = Require<ClientAccountInfo>(dba.ClientAccountID);

            return ca.ClientAccountActive && ca.ClientOrgActive;
        }

        public DryBoxAssignmentInfo Request(DryBoxRequest request)
        {
            DryBoxAssignment dba = new DryBoxAssignment
            {
                DryBoxID = request.DryBoxID,
                ClientAccountID = request.ClientAccountID,
                ReservedDate = DateTime.Now,
                ApprovedDate = null,
                RemovedDate = null,
                PendingApproval = true,
                PendingRemoval = false,
                Rejected = false
            };

            SaveOrUpdateAssignment(dba);

            return CreateDryBoxAssignmentInfo(dba, Require<DryBox>(request.DryBoxID), Require<ClientAccount>(request.ClientAccountID));
        }

        public DryBoxAssignmentInfo Reject(int dryBoxAssignmentId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);

            dba.PendingApproval = false;
            dba.PendingRemoval = false;
            dba.Rejected = true;
            dba.ApprovedDate = null;
            dba.RemovedDate = DateTime.Now;

            SaveOrUpdateAssignment(dba);

            return CreateDryBoxAssignmentInfo(dba, Require<DryBox>(dba.DryBoxID), Require<ClientAccount>(dba.ClientAccountID));
        }

        public DryBoxAssignmentInfo Approve(int dryBoxAssignmentId, int modifiedByClientId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);
            var modifiedBy = Require<Client>(modifiedByClientId);

            var db = Require<DryBox>(dba.DryBoxID);
            var ca = Require<ClientAccount>(dba.ClientAccountID);

            ApproveDryBoxAssignment(dba, ca, modifiedBy);

            return CreateDryBoxAssignmentInfo(dba, db, ca);
        }

        public DryBoxAssignmentInfo UpdateDryBoxAssignment(int dryBoxAssignmentId, DryBoxAssignmentUpdate update)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);
            var modifiedBy = Require<Client>(update.ModifiedByClientID);

            var db = Require<DryBox>(dba.DryBoxID);
            var ca = Require<ClientAccount>(update.ClientAccountID);

            //always save the new ClientAccount
            dba.ClientAccountID = update.ClientAccountID;

            //whenever staff updates, make PendingApproval false to clear any removal request
            dba.PendingRemoval = false;
            dba.Rejected = false;

            if (dba.PendingApproval)
            {
                // only an approver should be able to modify, the user can only cancel
                // pendingApproval = true means the approver changed the account and then clicked the Approve button
                // this handles the log and also does a SaveOrUpdate on the dba
                ApproveDryBoxAssignment(dba, ca, modifiedBy);
            }
            else
            {
                SaveOrUpdateAssignment(dba);
                UpdateLog(dba, ca, modifiedBy);
            }

            return CreateDryBoxAssignmentInfo(dba, db, ca);
        }

        public bool Remove(int dryBoxAssignmentId, int modifiedByClientId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);
            var modifiedBy = Require<Client>(modifiedByClientId);

            //always set the removed date
            dba.RemovedDate = DateTime.Now;
            dba.PendingApproval = false;
            dba.PendingRemoval = false;
            dba.Rejected = false;

            SaveOrUpdateAssignment(dba);

            UpdateLog(dba, null, modifiedBy);

            return true;
        }

        public DryBoxAssignment GetDryBoxAssignment(int dryBoxAssignmentId)
        {
            return Require<DryBoxAssignment>(dryBoxAssignmentId);
        }

        /// <summary>
        /// Get all the active dry box assignments during a date range.
        /// </summary>
        public IEnumerable<DryBoxAssignment> GetActiveDryBoxAssignments(DateTime sd, DateTime ed)
        {
            var result = Session.Query<DryBoxAssignment>()
                .Where(x => (x.ApprovedDate < ed) && (x.RemovedDate == null || x.RemovedDate.Value > sd)).ToList();

            return result;
        }

        public bool ClientAccountHasDryBox(int clientAccountId)
        {
            var result = Session.Query<DryBoxAssignment>()
                .Any(x => x.ClientAccountID == clientAccountId && !x.Rejected && !x.RemovedDate.HasValue);

            return result;
        }

        public bool ClientOrgHasDryBox(int clientOrgId)
        {
            return GetDryBoxClientAccount(clientOrgId) != null;
        }

        public IClientAccount GetDryBoxClientAccount(int clientOrgId)
        {
            var q = Session.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId);

            var join = q.Join(
                Session.Query<DryBoxAssignment>(),
                o => o.ClientAccountID,
                i => i.ClientAccountID,
                (o, i) => i);

            IClientAccount result = null;

            var dba = join.FirstOrDefault();

            if (dba != null)
            {
                var ca = Require<ClientAccount>(dba.ClientAccountID);
                result = ca.CreateModel<IClientAccount>();
            }

            return result;
        }

        public DataSet ReadDryBoxData(DateTime sd, DateTime ed, int clientId = 0)
        {
            var ds = Session.Command()
                .Param("Action", "ForStoreDataClean")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataSet("dbo.DryBoxAssignmentLog_Select");

            // Four tables are returned:
            //      0) DryBoxAssignment
            //      1) DryBoxItem
            //      2) Price
            //      3) ClientAccount

            ds.Tables[0].TableName = "DryBoxAssignment";
            ds.Tables[1].TableName = "DryBoxItem";
            ds.Tables[2].TableName = "Price";
            ds.Tables[3].TableName = "ClientAccount";

            return ds;
        }

        public IEnumerable<DryBox> GetDryBoxes()
        {
            return Session.Query<DryBox>().ToList();
        }

        public bool UpdateDryBox(DryBox model)
        {
            var dryBox = Session.Get<DryBox>(model.DryBoxID);
            if (dryBox == null) return false;
            dryBox.DryBoxName = model.DryBoxName;
            dryBox.Visible = model.Visible;
            dryBox.Deleted = model.Deleted;
            dryBox.Active = model.Active;
            Session.Update(dryBox);
            return true;
        }

        public IEnumerable<DryBoxAssignmentInfo> GetCurrentDryBoxAssignments()
        {
            return Session
                .CreateSQLQuery("SELECT * FROM sselData.dbo.v_DryBoxCurrentAssignments")
                .SetResultTransformer(Transformers.AliasToBean<DryBoxAssignmentInfo>())
                .List<DryBoxAssignmentInfo>();
        }

        public DryBoxAssignmentInfo CancelRequest(int dryBoxAssignmentId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);

            dba.ApprovedDate = null;
            dba.RemovedDate = DateTime.Now;
            dba.PendingApproval = false;
            dba.PendingRemoval = false;
            dba.Rejected = false;

            SaveOrUpdateAssignment(dba);

            return CreateDryBoxAssignmentInfo(dba, Require<DryBox>(dba.DryBoxID), Require<ClientAccount>(dba.ClientAccountID));
        }

        public DryBoxAssignmentInfo RequestRemove(int dryBoxAssignmentId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);

            dba.PendingRemoval = true;

            SaveOrUpdateAssignment(dba);

            return CreateDryBoxAssignmentInfo(dba, Require<DryBox>(dba.DryBoxID), Require<ClientAccount>(dba.ClientAccountID));
        }

        public IEnumerable<DryBoxHistory> GetDryBoxHistory(string dryBoxName)
        {
            return Session.Query<DryBoxHistory>()
                .Where(x => x.DryBoxName == dryBoxName)
                .OrderByDescending(x => x.DryBoxAssignmentID)
                .ThenBy(x => x.StatusChangedDate)
                .ThenBy(x => x.DryBoxAssignmentLogID)
                .ToList();
        }

        public IEnumerable<DryBoxHistory> GetDryBoxHistory(int clientId)
        {
            return Session.Query<DryBoxHistory>()
                .Where(x => x.ClientID == clientId)
                .OrderByDescending(x => x.DryBoxAssignmentID)
                .ThenBy(x => x.StatusChangedDate)
                .ThenBy(x => x.DryBoxAssignmentLogID)
                .ToList();
        }

        private void UpdateLog(DryBoxAssignment dba, ClientAccount ca, Client modifiedBy)
        {
            // Step 1:  Check for an existing log. There might not be one at this point if
            //          the reservation is requested and then updated before it's approved.
            var existing = Session.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

            if (existing != null)
            {
                // Step 2:  If a log is found set the DisableDate to today (otherwise nothing happens)

                existing.DisableDate = DateTime.Now.Date;

                // Step 3:  Check to see if this is a Remove (ca == null) or a Modify (ca != null)
                if (ca == null)
                {
                    // we are removing - set ModifiedBy (DisableDate set above)
                    existing.ModifiedBy = modifiedBy;
                }
                else
                {
                    // we are modifying - create a new log entry, using modifiedBy on the new entry
                    Session.Save(new DryBoxAssignmentLog
                    {
                        DryBoxAssignment = dba,
                        ClientAccount = ca,
                        EnableDate = DateTime.Now.Date,
                        DisableDate = null,
                        ModifiedBy = modifiedBy
                    });
                }

                // save the existing entry (DisableDate only when modifying, DisableDate and ModifiedBy when removing)
                Session.Update(existing);
            }
        }

        private void SaveOrUpdateAssignment(DryBoxAssignment dba)
        {
            // sanity checks

            if (dba.Rejected)
            {
                // The following should be true when Rejected = 1
                //      ApprovedDate IS NULL
                //      RemovedDate IS NOT NULL
                //      PendingApproval = 0
                //      PendingRemoval = 0

                if (dba.ApprovedDate.HasValue)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because Rejected = 1 and ApprovedDate IS NOT NULL");

                if (!dba.RemovedDate.HasValue)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because Rejected = 1 and RemovedDate IS NULL");

                if (dba.PendingApproval)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because Rejected = 1 and PendingApproval = 1");

                if (dba.PendingRemoval)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because Rejected = 1 and PendingApproval = 1");
            }

            if (dba.PendingRemoval)
            {
                // The following should be true when PendingRemoval = 1
                //      ApprovedDate IS NOT NULL
                //      RemovedDate IS NULL
                //      PendingApproval = 0
                //      Rejected = 0

                if (!dba.ApprovedDate.HasValue)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingRemoval = 1 and ApprovedDate IS NULL");

                if (dba.RemovedDate.HasValue)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingRemoval = 1 and RemovedDate IS NOT NULL");

                if (dba.PendingApproval)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingRemoval = 1 and PendingApproval = 1");

                if (dba.Rejected)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingRemoval = 1 and Rejected = 1");
            }

            if (dba.PendingApproval)
            {
                // The following should be true when PendingApproval = 1
                //      ApprovedDate IS NULL
                //      RemovedDate IS NULL
                //      PendingRemoval = 0
                //      Rejected = 0

                if (dba.ApprovedDate.HasValue)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingApproval = 1 and ApprovedDate IS NOT NULL");

                if (dba.RemovedDate.HasValue)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingApproval = 1 and RemovedDate IS NOT NULL");

                if (dba.PendingRemoval)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingApproval = 1 and PendingRemoval = 1");

                if (dba.Rejected)
                    throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingApproval = 1 and Rejected = 1");
            }

            if (!dba.PendingApproval && !dba.PendingRemoval && !dba.Rejected)
            {
                // The following should be true
                //      ApprovedDate IS NOT NULL (RemovedDate can be NULL or NOT NULL)
                //      OR
                //      ApprovedDate IS NULL and RemovedDate IS NOT NULL (this happens when a request is cancelled)

                if (!dba.ApprovedDate.HasValue)
                    if (!dba.RemovedDate.HasValue)
                        throw new Exception($"DryBoxAssignment#{dba.DryBoxAssignmentID} cannot be updated because PendingApproval = 0 and PendingRemoval = 0 and Rejected = 0 and ApprovedDate IS NULL and RemovedDate IS NULL");
            }

            // all checks passed
            Session.SaveOrUpdate(dba);
        }

        private void ApproveDryBoxAssignment(DryBoxAssignment dba, ClientAccount ca, Client modifiedBy)
        {
            //add new row to DryBoxAssignmentLog table
            var dbalog = new DryBoxAssignmentLog
            {
                DryBoxAssignment = dba,
                ClientAccount = ca,
                EnableDate = DateTime.Now.Date,
                DisableDate = null,
                ModifiedBy = modifiedBy
            };

            Session.Save(dbalog);

            if (dbalog.DryBoxAssignmentLogID != 0)
            {
                //add new row to DryBoxAssignment table
                dba.PendingApproval = false;
                dba.PendingRemoval = false;
                dba.Rejected = false;
                dba.ApprovedDate = DateTime.Now;
                dba.RemovedDate = null;

                SaveOrUpdateAssignment(dba);
            }
            else
                throw new Exception("Failed to save DryBoxAssignmentLog record.");
        }

        private DryBoxAssignmentInfo CreateDryBoxAssignmentInfo(DryBoxAssignment dba, DryBox db, ClientAccount ca)
        {
            //var db = Require<DryBox>(dba.DryBoxID);
            //var ca = Require<ClientAccount>(dba.ClientAccountID);
            var co = ca.ClientOrg;
            var c = co.Client;
            var a = ca.Account;
            var o = a.Org;

            return new DryBoxAssignmentInfo
            {
                ClientID = c.ClientID,
                DryBoxName = db.DryBoxName,
                Active = db.Active,
                Visible = db.Visible,
                Deleted = db.Deleted,
                UserName = c.UserName,
                LName = c.LName,
                FName = c.FName,
                ShortCode = a.ShortCode,
                AccountName = a.Name,
                OrgName = o.OrgName,
                Email = co.Email,
                ApprovedDate = dba.ApprovedDate,
                ClientAccountID = dba.ClientAccountID,
                DryBoxAssignmentID = dba.DryBoxAssignmentID,
                DryBoxID = dba.DryBoxID,
                PendingApproval = dba.PendingApproval,
                PendingRemoval = dba.PendingRemoval,
                Rejected = dba.Rejected,
                RemovedDate = dba.RemovedDate,
                ReservedDate = dba.ReservedDate
            };
        }
    }
}