using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Linq;

namespace LNF.Data
{
    public static class DryBoxUtility
    {
        public static DryBoxAssignment Request(DryBox db, ClientAccount ca)
        {
            DryBoxAssignment dba = new DryBoxAssignment();
            dba.DryBox = db;
            dba.ClientAccount = ca;
            dba.ReservedDate = DateTime.Now;
            dba.ApprovedDate = null;
            dba.RemovedDate = null;
            dba.PendingApproval = true;
            dba.PendingRemoval = false;
            dba.Rejected = false;
            DA.Current.Insert(dba);
            return dba;
        }

        public static void Reject(DryBoxAssignment dba)
        {
            dba.PendingApproval = false;
            dba.PendingRemoval = false;
            dba.Rejected = true;
            dba.RemovedDate = DateTime.Now;
        }

        public static void Approve(DryBoxAssignment dba, Client modifiedBy)
        {
            //add new row to DryBoxAssignmentLog table
            DryBoxAssignmentLog dbalog = new DryBoxAssignmentLog();
            dbalog.DryBoxAssignment = dba;
            dbalog.ClientAccount = dba.ClientAccount;
            dbalog.EnableDate = DateTime.Now.Date;
            dbalog.DisableDate = null;
            dbalog.ModifiedBy = modifiedBy;
            DA.Current.Insert(dbalog);

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

        public static void Update(DryBoxAssignment dba, ClientAccount ca, Client modifiedBy)
        {
            //always save the new ClientAccount
            dba.ClientAccount = ca;

            DryBoxAssignmentLog dbalog = DA.Current.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

            //it's possible to not have a log at this point if the reservation is requested and then updated before it's approved
            if (dbalog != null)
            { 
                dbalog.DisableDate = DateTime.Now.Date;

                dbalog = new DryBoxAssignmentLog();
                dbalog.DryBoxAssignment = dba;
                dbalog.ClientAccount = ca;
                dbalog.EnableDate = DateTime.Now.Date;
                dbalog.DisableDate = null;
                dbalog.ModifiedBy = modifiedBy;
                DA.Current.Insert(dbalog);
            }
        }

        public static void Remove(DryBoxAssignment dba, Client modifiedBy)
        {
            //always set the removed date
            dba.RemovedDate = DateTime.Now;
            dba.PendingApproval = false;
            dba.PendingRemoval = false;

            DryBoxAssignmentLog dbalog = DA.Current.Query<DryBoxAssignmentLog>().FirstOrDefault(x => x.DryBoxAssignment == dba && x.DisableDate == null);

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
        public static DryBoxAssignment[] ActiveAssignments(DateTime startDate, DateTime endDate)
        {
            var result = DA.Current.Query<DryBoxAssignment>()
                .Where(x => (x.ApprovedDate < endDate) && (x.RemovedDate == null || x.RemovedDate.Value > startDate))
                .ToArray();

            return result;
        }
    }
}
