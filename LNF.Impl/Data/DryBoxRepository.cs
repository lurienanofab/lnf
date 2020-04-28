using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class DryBoxRepository : RepositoryBase, IDryBoxRepository
    {
        public DryBoxRepository(ISessionManager mgr) : base(mgr) { }

        public IDryBoxAssignment GetCurrentAssignment(int dryBoxId)
        {
            return Session.Query<DryBoxAssignment>()
                .FirstOrDefault(x => x.DryBox.DryBoxID == dryBoxId && x.RemovedDate == null)
                .CreateModel<IDryBoxAssignment>();
        }

        public bool? IsAccountActive(int dryBoxId)
        {
            var dba = GetCurrentAssignment(dryBoxId);

            // this only applies to dry boxes where an account has been assigned
            // so return null if there is no assignment

            if (dba == null)
                return null;

            var ca = Require<ClientAccountInfo>(dba.ClientAccountID);

            return ca.ClientAccountActive && ca.ClientOrgActive;
        }

        public IDryBoxAssignment Request(int dryBoxId, int clientAccountId)
        {
            var db = Require<DryBox>(dryBoxId);
            var ca = Require<ClientAccount>(clientAccountId);

            DryBoxAssignment dba = new DryBoxAssignment
            {
                DryBox = db,
                ClientAccount = ca,
                ReservedDate = DateTime.Now,
                ApprovedDate = null,
                RemovedDate = null,
                PendingApproval = true,
                PendingRemoval = false,
                Rejected = false
            };

            Session.Save(dba);

            return dba.CreateModel<IDryBoxAssignment>();
        }

        public void Reject(int dryBoxAssignmentId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);

            dba.PendingApproval = false;
            dba.PendingRemoval = false;
            dba.Rejected = true;
            dba.RemovedDate = DateTime.Now;

            Session.SaveOrUpdate(dba);
        }

        public void Approve(int dryBoxAssignmentId, int modifiedByClientId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);
            var modifiedBy = Require<Client>(modifiedByClientId);

            //add new row to DryBoxAssignmentLog table
            DryBoxAssignmentLog dbalog = new DryBoxAssignmentLog
            {
                DryBoxAssignment = dba,
                ClientAccount = dba.ClientAccount,
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

                Session.SaveOrUpdate(dba);
            }
            else
                throw new Exception("Failed to save DryBoxAssignmentLog record.");
        }

        public void UpdateDryBoxAssignment(IDryBoxAssignment dba, int clientAccountId, int modifiedByClientId)
        {
            var entity = Require<DryBoxAssignment>(dba.DryBoxAssignmentID);
            var ca = Require<ClientAccount>(clientAccountId);
            var modifiedBy = Require<Client>(modifiedByClientId);

            //always save the new ClientAccount
            entity.ClientAccount = ca;

            DryBoxAssignmentLog dbalog = Session.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

            //it's possible to not have a log at this point if the reservation is requested and then updated before it's approved
            if (dbalog != null)
            {
                dbalog.DisableDate = DateTime.Now.Date;

                dbalog = new DryBoxAssignmentLog
                {
                    DryBoxAssignment = entity,
                    ClientAccount = ca,
                    EnableDate = DateTime.Now.Date,
                    DisableDate = null,
                    ModifiedBy = modifiedBy
                };

                Session.Save(dbalog);
            }
        }

        public void Remove(int dryBoxAssignmentId, int modifiedByClientId)
        {
            var dba = Require<DryBoxAssignment>(dryBoxAssignmentId);
            var modifiedBy = Require<Client>(modifiedByClientId);

            //always set the removed date
            dba.RemovedDate = DateTime.Now;
            dba.PendingApproval = false;
            dba.PendingRemoval = false;

            Session.SaveOrUpdate(dba);

            DryBoxAssignmentLog dbalog = Session.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

            //it's possible to not have a log at this point if the reservation is requested and then a removal request is made before it's approved
            if (dbalog != null)
            {
                dbalog.DisableDate = DateTime.Now.Date;
                dbalog.ModifiedBy = modifiedBy;
                Session.SaveOrUpdate(dbalog);
            }
        }

        /// <summary>
        /// Get all the active dry box assignments during a date range.
        /// </summary>
        public IEnumerable<IDryBoxAssignment> GetActiveAssignments(DateTime sd, DateTime ed)
        {
            var result = Session.Query<DryBoxAssignment>()
                .Where(x => (x.ApprovedDate < ed) && (x.RemovedDate == null || x.RemovedDate.Value > sd))
                .CreateModels<IDryBoxAssignment>();

            return result;
        }

        public bool ClientAccountHasDryBox(int clientAccountId)
        {
            IList<DryBoxAssignment> query = Session.Query<DryBoxAssignment>().Where(x => x.ClientAccount.ClientAccountID == clientAccountId).ToList();
            return query.Any(x => x.GetStatus() == DryBoxAssignmentStatus.Active);
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
                i => i.ClientAccount.ClientAccountID,
                (o, i) => i);

            var result = join.FirstOrDefault().ClientAccount.CreateModel<IClientAccount>();

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

        public IEnumerable<IDryBox> GetDryBoxes()
        {
            return Session.Query<DryBox>().CreateModels<IDryBox>();
        }

        public bool UpdateDryBox(IDryBox model)
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
    }
}