using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class ApproverRepository : RepositoryBase, IApproverRepository
    {
        public ApproverRepository(ISessionManager mgr) : base(mgr) { }

        public IApprover GetApprover(int clientId, int approverId)
        {
            return Session.Get<Approver>(new Approver() { ClientID = clientId, ApproverID = approverId }).CreateModel<IApprover>();
        }

        public IEnumerable<IApprover> GetApprovers(int clientId)
        {
            return Session.Query<Approver>().Where(x => x.ClientID == clientId).CreateModels<IApprover>();
        }

        public IEnumerable<IApprover> GetActiveApprovers(int clientId)
        {
            return Session.Query<Approver>().Where(x => x.ClientID == clientId && x.Active).CreateModels<IApprover>();
        }

        public bool IsApprover(int clientId)
        {
            return Session.Query<Approver>().Any(x => x.ApproverID == clientId && x.Active);
        }

        public IApprover AddApprover(int clientId, int approverId, bool isPrimary)
        {
            Approver appr = new Approver { ClientID = clientId, ApproverID = approverId };
            Approver existing = Session.Get<Approver>(appr);

            IApprover result;

            if (existing == null)
            {
                appr.Active = true;
                Session.Save(appr);
                SetPrimary(appr, isPrimary);
                result = appr.CreateModel<IApprover>();
            }
            else
            {
                existing.Active = true;
                SetPrimary(existing, isPrimary);
                result = existing.CreateModel<IApprover>();
            }

            return result;
        }

        public IApprover UpdateApprover(int clientId, int approverId, bool isPrimary)
        {
            var appr = Session.Get<Approver>(new Approver() { ClientID = clientId, ApproverID = approverId });

            if (appr == null)
                throw new ItemNotFoundException("Approver", $"Cannot find Approver with ClientID = {clientId} and ApproverID = {approverId}");

            SetPrimary(appr, isPrimary);

            return appr.CreateModel<IApprover>();
        }

        public bool DeleteApprover(int clientId, int approverId)
        {
            Approver key = new Approver { ClientID = clientId, ApproverID = approverId };
            Approver appr = Session.Get<Approver>(key);
            if (appr == null) return false;
            appr.Active = false;
            SetPrimary(appr, false);
            return true;
        }

        private void SetPrimary(Approver appr, bool isPrimary)
        {
            var query = Session.Query<Approver>().Where(x => x.ClientID == appr.ClientID);

            if (isPrimary)
            {
                foreach (Approver item in query)
                {
                    if (item.ApproverID == appr.ApproverID)
                        item.IsPrimary = true;
                    else
                        item.IsPrimary = false;

                    Session.Update(item);
                }
            }
            else
            {
                appr.IsPrimary = false;
                Session.Update(appr);
            }
        }
    }
}
