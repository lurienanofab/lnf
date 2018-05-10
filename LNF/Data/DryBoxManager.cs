using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class DryBoxManager : ManagerBase, IDryBoxManager
    {
        public DryBoxManager(ISession session) : base(session) { }

        public DryBoxAssignment CurrentAssignment(DryBox item)
        {
            return Session.Query<DryBoxAssignment>()
                .FirstOrDefault(x => x.DryBox.DryBoxID == item.DryBoxID && x.RemovedDate == null);
        }

        public bool? IsAccountActive(DryBox item)
        {
            var dba = CurrentAssignment(item);

            // this only applies to dry boxes where an account has been assigned
            // so return null if there is no assignment

            if (dba == null)
                return null;

            return dba.ClientAccount.Active && dba.ClientAccount.ClientOrg.Active;
        }

        public ClientAccountInfo GetClientAccountInfo(DryBoxAssignmentLog item)
        {
            ClientAccountInfo result = Session.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientAccountID == item.ClientAccount.ClientAccountID);
            return result;
        }

        public DryBoxAssignment Request(DryBox db, ClientAccount ca)
        {
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
            Session.Insert(dba);
            return dba;
        }

        public void Reject(DryBoxAssignment dba)
        {
            dba.PendingApproval = false;
            dba.PendingRemoval = false;
            dba.Rejected = true;
            dba.RemovedDate = DateTime.Now;
        }

        public void Approve(DryBoxAssignment dba, Client modifiedBy)
        {
            //add new row to DryBoxAssignmentLog table
            DryBoxAssignmentLog dbalog = new DryBoxAssignmentLog
            {
                DryBoxAssignment = dba,
                ClientAccount = dba.ClientAccount,
                EnableDate = DateTime.Now.Date,
                DisableDate = null,
                ModifiedBy = modifiedBy
            };
            Session.Insert(dbalog);

            if (dbalog.DryBoxAssignmentLogID != 0)
            {
                //add new row to DryBoxAssignment table
                dba.PendingApproval = false;
                dba.PendingRemoval = false;
                dba.Rejected = false;
                dba.ApprovedDate = DateTime.Now;
            }
            else
                throw new Exception("Failed to save DryBoxAssignmentLog record.");
        }

        public void Update(DryBoxAssignment dba, ClientAccount ca, Client modifiedBy)
        {
            //always save the new ClientAccount
            dba.ClientAccount = ca;

            DryBoxAssignmentLog dbalog = Session.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

            //it's possible to not have a log at this point if the reservation is requested and then updated before it's approved
            if (dbalog != null)
            {
                dbalog.DisableDate = DateTime.Now.Date;

                dbalog = new DryBoxAssignmentLog
                {
                    DryBoxAssignment = dba,
                    ClientAccount = ca,
                    EnableDate = DateTime.Now.Date,
                    DisableDate = null,
                    ModifiedBy = modifiedBy
                };

                Session.Insert(dbalog);
            }
        }

        public void Remove(DryBoxAssignment dba, Client modifiedBy)
        {
            //always set the removed date
            dba.RemovedDate = DateTime.Now;
            dba.PendingApproval = false;
            dba.PendingRemoval = false;

            DryBoxAssignmentLog dbalog = Session.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

            //it's possible to not have a log at this point if the reservation is requested and then a removal request is made before it's approved
            if (dbalog != null)
            {
                dbalog.DisableDate = DateTime.Now.Date;
                dbalog.ModifiedBy = modifiedBy;
            }
        }

        /// <summary>
        /// Get all the active dry box assignments during a date range.
        /// </summary>
        public DryBoxAssignment[] ActiveAssignments(DateTime startDate, DateTime endDate)
        {
            var result = Session.Query<DryBoxAssignment>()
                .Where(x => (x.ApprovedDate < endDate) && (x.RemovedDate == null || x.RemovedDate.Value > startDate))
                .ToArray();

            return result;
        }

        public bool HasDryBox(ClientAccount item)
        {
            IList<DryBoxAssignment> query = Session.Query<DryBoxAssignment>().Where(x => x.ClientAccount.ClientAccountID == item.ClientAccountID).ToList();
            DryBoxAssignment dba = query.FirstOrDefault(x => x.GetStatus() == DryBoxAssignmentStatus.Active);
            return dba != null;
        }

        public ClientAccount GetDryBoxClientAccount(ClientOrg item)
        {
            IList<ClientAccount> query = Session.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID).ToList();
            ClientAccount ca = query.FirstOrDefault(x => HasDryBox(x));
            return ca;
        }

        public bool HasDryBox(ClientOrg item)
        {
            return GetDryBoxClientAccount(item) != null;
        }
    }
}
