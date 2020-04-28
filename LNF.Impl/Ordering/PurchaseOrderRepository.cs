using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class PurchaseOrderRepository : RepositoryBase, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(ISessionManager mgr) : base(mgr) { }

        public IPurchaseOrder GetPurchaseOrder(int poid)
        {
            return Session.Get<PurchaseOrder>(poid).CreateModel<IPurchaseOrder>();
        }

        public IEnumerable<IPurchaseOrderDetail> GetDetails(int poid)
        {
            return Session.Query<PurchaseOrderDetail>().Where(x => x.PurchaseOrder.POID == poid).CreateModels<IPurchaseOrderDetail>();
        }

        public IEnumerable<IPurchaseOrderAccount> GetAccounts(int clientId)
        {
            return Session.Query<PurchaseOrderAccount>().Where(x => x.ClientID == clientId).CreateModels<IPurchaseOrderAccount>();
        }

        public IEnumerable<IPurchaseOrderAccount> GetActiveAccounts(int clientId)
        {
            return Session.Query<PurchaseOrderAccount>().Where(x => x.ClientID == clientId && x.Active).CreateModels<IPurchaseOrderAccount>();
        }

        public IPurchaseOrderAccount AddAccount(int clientId, int accountId)
        {
            PurchaseOrderAccount acct = new PurchaseOrderAccount { AccountID = accountId, ClientID = clientId };
            PurchaseOrderAccount existing = Session.Get<PurchaseOrderAccount>(acct);

            IPurchaseOrderAccount result;

            if (existing == null)
            {
                //insert new
                acct.Active = true;
                Session.Save(acct);
                result = acct.CreateModel<IPurchaseOrderAccount>();
            }
            else
            {
                //update existing
                existing.Active = true;
                Session.Update(existing);
                result = existing.CreateModel<IPurchaseOrderAccount>();
            }

            return result;
        }

        public bool DeleteAccount(int clientId, int accountId)
        {
            var acct = Session.Get<PurchaseOrderAccount>(new PurchaseOrderAccount { AccountID = accountId, ClientID = clientId });

            if (acct == null) return false;

            acct.Active = false;

            Session.Update(acct);

            return true;
        }

        public IPurchaseOrderDetail AddDetail(int poid, int itemId, int catId, double qty, string unit, double unitPrice, bool isInventoryControlled)
        {
            PurchaseOrderDetail pod = new PurchaseOrderDetail()
            {
                Category = Require<PurchaseOrderCategory>(catId),
                IsInventoryControlled = isInventoryControlled,
                Item = Require<Repository.Ordering.PurchaseOrderItem>(itemId),
                PurchaseOrder = Require<PurchaseOrder>(poid),
                Quantity = qty,
                ToInventoryDate = null,
                Unit = unit,
                UnitPrice = unitPrice
            };

            Session.Save(pod);

            return pod.CreateModel<IPurchaseOrderDetail>();
        }

        public IPurchaseOrderDetail UpdateDetail(int podid, int catId, double qty, string unit, double unitPrice, bool isInventoryControlled)
        {
            var pod = Require<PurchaseOrderDetail>(podid);

            if (pod != null)
            {
                pod.Category = Require<PurchaseOrderCategory>(catId);
                pod.Quantity = qty;
                pod.Unit = unit;
                pod.UnitPrice = unitPrice;
                pod.IsInventoryControlled = isInventoryControlled;
            }

            return pod.CreateModel<IPurchaseOrderDetail>();
        }
    }
}
