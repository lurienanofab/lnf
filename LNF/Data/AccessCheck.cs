using LNF.Models.Data;
using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class AccessCheck
    {
        public ISession Session { get; }
        public Client Client { get; private set; }
        public bool IsActive { get { return Client.Active; } }
        public bool HasPhysicalAccessPriv { get { return Client.HasPriv(ClientPrivilege.PhysicalAccess); } }
        public bool HasLabUserPriv { get { return Client.HasPriv(ClientPrivilege.LabUser); } }
        public bool HasStoreUserPriv { get { return Client.HasPriv(ClientPrivilege.StoreUser); } }
        public bool HasActiveAccounts { get { return DA.Use<IClientManager>().GetActiveAccountCount(Client.ClientID) > 0; } }

        /// <summary>
        /// The reason why access can or cannot be enabled. Set by calling CanEnableAccess()
        /// </summary>
        public string Reason { get; private set; }

        private AccessCheck() { }

        public static AccessCheck Create(Client c)
        {
            AccessCheck result = new AccessCheck()
            {
                Client = c,
                Reason = null
            };

            return result;
        }

        public bool CanEnableAccess()
        {
            if (!IsActive)
            {
                Reason = "This client is not active.";
                return false;
            }

            if (!HasPhysicalAccessPriv)
            {
                Reason = "This client does not have the Physical Access privilege.";
                return false;
            }

            if (!HasLabUserPriv)
            {
                Reason = "This client has physical access. Skipped active account check for non-lab-user (staff, plant, vendor, etc).";
                return true; // this is the staff, plant or vendor case: they have physical access but not lab user
            }

            // at this point we know they have physical access and also the lab user priv, so we need to check for active accounts
            if (HasActiveAccounts)
            {
                Reason = "This client has both Physical Access and Lab User privileges, and at least one active account.";
                return true;
            }

            // no active accounts so false
            Reason = "This client has no active accounts.";
            return false;
        }

        public IEnumerable<Badge> GetBadges()
        {
            return ServiceProvider.Current.PhysicalAccess.GetBadge(Client);
        }

        /// <summary>
        /// Checks if the client is currently enabled in the access system by looking for a badge that is not expired.
        /// </summary>
        public bool IsPhysicalAccessEnabled()
        {
            return GetBadges().Any(x => x.IsActive());
        }

        /// <summary>
        /// Checks if the client is eligible to be reenabled based on the age of their access system account.
        /// </summary>
        public bool AllowReenable()
        {
            GlobalCost gc = Session.Query<GlobalCost>().FirstOrDefault();
            bool result = ServiceProvider.Current.PhysicalAccess.AllowReenable(Client.ClientID, gc.AccessToOld);
            return result;
        }

        public void RemovePhysicalAccessPriv()
        {
            Client.RemovePriv(ClientPrivilege.PhysicalAccess);
        }

        public void RemoveStoreUserPriv()
        {
            Client.RemovePriv(ClientPrivilege.StoreUser);
        }

        public void EnablePhysicalAccess()
        {
            ServiceProvider.Current.PhysicalAccess.EnableAccess(Client);
        }

        public void DisablePhysicalAccess()
        {
            ServiceProvider.Current.PhysicalAccess.DisableAccess(Client, DateTime.Now);
        }
    }
}
