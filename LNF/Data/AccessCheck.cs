using LNF.PhysicalAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class AccessCheck
    {
        public IPhysicalAccessService PhysicalAccess { get; }
        public IClient Client { get; }
        public IGlobalCost GlobalCost { get; }
        public bool IsActive => Client.ClientActive;
        public bool HasPhysicalAccessPriv => Client.HasPriv(ClientPrivilege.PhysicalAccess);
        public bool HasLabUserPriv => Client.HasPriv(ClientPrivilege.LabUser);
        public bool HasStoreUserPriv => Client.HasPriv(ClientPrivilege.StoreUser);
        public bool HasActiveAccounts { get; }

        /// <summary>
        /// The reason why access can or cannot be enabled. Set by calling CanEnableAccess()
        /// </summary>
        public string Reason { get; private set; }

        private AccessCheck(IPhysicalAccessService physicalAccess, IClient client, IGlobalCost globalCost, bool hasActiveAccounts)
        {
            PhysicalAccess = physicalAccess;
            Client = client;
            GlobalCost = globalCost;
            HasActiveAccounts = hasActiveAccounts;
        }

        public static AccessCheck Create(IPhysicalAccessService physicalAccess, IClient client, IGlobalCost globalCost, bool hasActiveAccounts)
        {
            return new AccessCheck(physicalAccess, client, globalCost, hasActiveAccounts);
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
            return PhysicalAccess.GetBadge(Client.ClientID);
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
            bool result = PhysicalAccess.GetAllowReenable(Client.ClientID, GlobalCost.AccessToOld);
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
            PhysicalAccess.EnableAccess(new UpdateClientRequest { ClientID = Client.ClientID, ExpireOn = null });
        }

        public void DisablePhysicalAccess()
        {
            PhysicalAccess.DisableAccess(new UpdateClientRequest { ClientID = Client.ClientID, ExpireOn = DateTime.Now });
        }
    }
}
