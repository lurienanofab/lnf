using LNF.CommonTools;
using LNF.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class DryBoxAssignmentInfo : IDryBox, IDryBoxAssignment, IDataItem
    {
        public virtual string DryBoxName { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual string ShortCode { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual string Email { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual bool ClientOrgActive { get; set; }
        public virtual bool ClientAccountActive { get; set; }
        public virtual int DryBoxAssignmentID { get; set; }
        public virtual int DryBoxID { get; set; }
        public virtual int ClientAccountID { get; set; }
        public virtual DateTime ReservedDate { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }
        public virtual DateTime? RemovedDate { get; set; }
        public virtual bool PendingApproval { get; set; }
        public virtual bool PendingRemoval { get; set; }
        public virtual bool Rejected { get; set; }

        /// <summary>
        /// Checks to see if the ClientAccount, ClientOrg, and Client are currenlty active (for example the user may no longer be assocated with the account).
        /// </summary>
        public virtual bool IsActive()
        {
            // if there is no current assignment then return true
            if (DryBoxAssignmentID == 0)
                return true;
            else
                return ClientAccountActive && ClientOrgActive && ClientActive;
        }
    }

    /// <summary>
    /// Contains DryBoxAssignmentInfo items for current DryBox reservations only. Removed or Rejected assignments don't count. In these cases DryBox data is included but without any DryBoxAssignment data (DryBoxAssignmnetID = 0).
    /// </summary>
    public class CurrentDryBoxAssignmentCollection : IEnumerable<DryBoxAssignmentInfo>
    {
        // this list should only be populated with results from sselData.dbo.v_DryBoxCurrentAssignments
        private readonly List<DryBoxAssignmentInfo> _list;

        public CurrentDryBoxAssignmentCollection(IEnumerable<DryBoxAssignmentInfo> source)
        {
            if (source.Any(x => x.Rejected))
                throw new Exception("The source should not include any rejected assignments.");

            if (source.Any(x => x.RemovedDate.HasValue))
                throw new Exception("The source should not include any removed assignments.");

            _list = source.OrderBy(x => x.DryBoxName).ToList();
        }

        public DryBoxAssignmentInfo this[int index] => _list[index];

        /// <summary>
        /// DryBoxes assigned to the given Client
        /// </summary>
        public IEnumerable<DryBoxAssignmentInfo> Active(int clientId)
        {
            return _list.Where(x => x.ClientID == clientId).ToList();
        }

        /// <summary>
        /// Gets dry boxes based on GetFilter.
        /// </summary>
        public IEnumerable<DryBoxAssignmentInfo> Filter(string group, bool includeInactiveDryBoxes = false)
        {
            var filter = group.ToLower();

            switch (filter)
            {
                case "all":
                    return All(includeInactiveDryBoxes);
                case "pending":
                    return Pending(includeInactiveDryBoxes);
                case "available":
                    return Available(includeInactiveDryBoxes);
                case "inactive":
                    return Inactive(includeInactiveDryBoxes);
                default:
                    return Group(filter, includeInactiveDryBoxes);
            }
        }

        public IEnumerable<DryBoxAssignmentInfo> Search(string search, bool includeInactive = false)
        {
            var result = All(includeInactive).Where(x =>
                    Utility.StringContains(x.DryBoxName, search)
                    || Utility.StringContains(x.UserName, search)
                    || Utility.StringContains(x.LName, search)
                    || Utility.StringContains(x.FName, search)
                    || Utility.StringContains(x.ShortCode, search)
                    || Utility.StringContains(x.AccountName, search)
                    || Utility.StringContains(x.OrgName, search)
                    || x.ClientID.ToString() == search).ToList();

            return result;
        }

        public IEnumerable<DryBoxAssignmentInfo> Group(string group, bool includeInactiveDryBoxes = false) => All(includeInactiveDryBoxes).Where(x => x.DryBoxName.ToLower().StartsWith(group)).OrderBy(x => x.DryBoxName).ToList();

        public IEnumerable<DryBoxAssignmentInfo> Inactive(bool includeInactiveDryBoxes) => All(includeInactiveDryBoxes).Where(x => !x.IsActive()).OrderBy(x => x.DryBoxName).ToList();

        public IEnumerable<DryBoxAssignmentInfo> Available(bool includeInactiveDryBoxes = false) => All(includeInactiveDryBoxes).Where(x => x.DryBoxAssignmentID == 0).OrderBy(x => x.DryBoxName).ToList();

        public IEnumerable<DryBoxAssignmentInfo> Pending(bool includeInactiveDryBoxes = false) => All(includeInactiveDryBoxes).Where(x => x.PendingApproval).OrderBy(x => x.DryBoxName).ToList();

        public IEnumerable<DryBoxAssignmentInfo> All(bool includeInactiveDryBoxes)
        {
            // in this case inactive refers to the DryBox itself, not the ClientAccount
            if (includeInactiveDryBoxes)
                return _list.OrderBy(x => x.DryBoxName).ToList();
            else
                return _list.Where(x => x.Active && x.Visible && !x.Deleted).OrderBy(x => x.DryBoxName).ToList();
        }

        public IEnumerator<DryBoxAssignmentInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
