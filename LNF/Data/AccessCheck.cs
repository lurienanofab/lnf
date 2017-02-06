using LNF.Models.Data;
using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System.Linq;

namespace LNF.Data
{
    public class AccessCheck
    {
        private Client _Client;
        private bool _IsActive;
        private bool _HasPhysicalAccessPriv;
        private bool _HasLabUserPriv;
        private bool _HasStoreUserPriv;
        private bool _HasActiveAccounts;

        public Client Client { get { return _Client; } }
        public bool IsActive { get { return _IsActive; } }
        public bool HasPhysicalAccessPriv { get { return _HasPhysicalAccessPriv; } }
        public bool HasLabUserPriv { get { return _HasLabUserPriv; } }
        public bool HasStoreUserPriv { get { return _HasStoreUserPriv; } }
        public bool HasActiveAccounts { get { return _HasActiveAccounts; } }

        private AccessCheck() { }

        public static AccessCheck Create(Client c)
        {
            AccessCheck result = new AccessCheck();
            result._Client = c;
            result._IsActive = c.Active;
            result._HasPhysicalAccessPriv = c.HasPriv(ClientPrivilege.PhysicalAccess);
            result._HasLabUserPriv = c.HasPriv(ClientPrivilege.LabUser);
            result._HasStoreUserPriv = c.HasPriv(ClientPrivilege.StoreUser);
            result._HasActiveAccounts = c.ClientAccounts().Any(x => x.Active && x.ClientOrg.Active);
            return result;
        }

        public bool EnableAccess()
        {
            if (!IsActive)
                return false;

            if (!HasPhysicalAccessPriv)
                return false;

            if (!HasLabUserPriv)
                return true; // this is the plant or vendor case: they have physical access but not lab user
            
            // at this point we know they have physical access and also the lab user priv, so we need to check for active accounts
            if (HasActiveAccounts)
                return true;

            // no active accounts so false
            return false;
        }

        public Badge[] GetBadges()
        {
            return Providers.PhysicalAccess.GetBadge(_Client).ToArray();
        }

        /// <summary>
        /// Checks if the client is currently enabled in the access system.
        /// </summary>
        public bool IsPhysicalAccessEnabled()
        {
            return GetBadges().Any(x => x.IsActive());
        }

        /// <summary>
        /// Checks if the client is eligible to be reenabled.
        /// </summary>
        public bool AllowReenable()
        {
            GlobalCost gc = DA.Current.Query<GlobalCost>().FirstOrDefault();
            bool result = Providers.PhysicalAccess.AllowReenable(_Client.ClientID, gc.AccessToOld);
            return result;
        }

        //public AccessCheck CheckEnableAccess()
        //{
        //    //This method does not change the client other than setting the EnableAccess flag. If privs
        //    //need to change or access enabled/disabled in Prowatch that should be done elsewhere.

        //    //Note: we are not setting Client.EnableAccess here anymore. The Result object has a property for this
        //    //and can be used outside this method. This way the property in the Client class can be depricated some day.

        //    if (_Client == null)
        //        return this;

        //    ClientPrivilege p = _Client.Privs;

        //    //Start by checking for Physical Access priv, if not we can skip the next part.
        //    _Result.EnableAccess = HasPhysicalAccessPriv(p);

        //    if (_Result.EnableAccess)
        //    {
        //        _Result.HasPhysicalAccessPriv = true;

        //        //Now we know they have the minimum required priv, we now check if they are a lab user.
        //        //If so they must have at least one active acct. A lab user is a user that has Physical Access
        //        //and Lab User privs plus either of the following: Store User, SSEl-OnLine Access. If they have
        //        //any additional privs (e.g. Staff) they do no need to be checked.

        //        //Remove Physical Access, Store User and SSEL-OnLine Access so we can see if the only remaining priv is Lab User.
        //        p -= ClientPrivilege.PhysicalAccess;
        //        if (p.HasFlag(ClientPrivilege.StoreUser)) p -= ClientPrivilege.StoreUser;
        //        if (p.HasFlag(ClientPrivilege.OnlineAccess)) p -= ClientPrivilege.OnlineAccess;

        //        if (p == ClientPrivilege.LabUser)
        //        {
        //            _Result.ActiveAccountNotRequired = false;

        //            //The only priv left is Lab User so we need to check if they have at least one active account.
        //            if (HasActiveAccounts(_Client))
        //            {
        //                _Result.EnableAccess = true;
        //                _Result.ActiveAccountRequirement = true;
        //            }
        //            else
        //            {
        //                _Result.EnableAccess = false;
        //                _Result.ActiveAccountRequirement = false;
        //            }
        //        }
        //        else
        //            _Result.ActiveAccountNotRequired = true;

        //        if (_Result.EnableAccess)
        //        {
        //            GlobalCost gc = DA.Current.Query<GlobalCost>().FirstOrDefault();
        //            _Result.EnableAccess = Providers.PhysicalAccess.AllowReenable(_Client.ClientID, gc.AccessToOld);

        //            if (!_Result.EnableAccess)
        //                _Result.AllowReenable = false;
        //            else
        //                _Result.AllowReenable = true;
        //        }
        //    }
        //    else
        //        _Result.HasPhysicalAccessPriv = false;

        //    return this;
        //}

        //this is a separate method so it can be called after saving the client and/or adding/disabling/enabling the client in Prowatch
        //public AccessCheckResult CheckPhysicalAccess()
        //{
        //    _Result.HasPhysicalAccess = false;
        //    _Badge = Providers.PhysicalAccess.GetBadge(_Client).FirstOrDefault();
        //    if (_Badge != null)
        //    {
        //        _Result.HasBadge = true;
        //        _Result.BadgeExpire = _Badge.ExpireDate;
        //        _Result.ActiveCard = false;
        //        _Result.Cards = Cards.Select(x => new AccessCheckCard() { Number = x.Number, Expire = x.CardExpireDate, LastAccess = x.LastAccess, Status = x.Status.ToString() }).ToArray();

        //        if (_Badge.ExpireDate > DateTime.Now)
        //        {
        //            _Result.ActiveBadge = true;

        //            foreach (var card in Cards)
        //            {
        //                if (card.CardExpireDate > DateTime.Now && card.Status == Status.Active)
        //                {
        //                    _Result.ActiveCard = true;
        //                    break;
        //                }
        //            }

        //            _Result.HasPhysicalAccess = _Result.ActiveCard;
        //        }
        //        else
        //            _Result.ActiveBadge = false;
        //    }
        //    else
        //    { 
        //        _Result.HasBadge = false;
        //        _Result.BadgeExpire = null;
        //    }

        //    return this;
        //}
    }

    //public class AccessCheckResult
    //{
    //    //The differnece between EnableAccess and HasPhysicalAccess is EnableAccess means the client can have physical access based on
    //    //privs and accounts. HasPhysicalAccess means the necessary records exist in Prowatch and are not expired and are active.

    //    public bool EnableAccess { get; set; }
    //    public bool HasPhysicalAccess { get; set; }
    //    public bool HasPhysicalAccessPriv { get; set; }
    //    public bool ActiveAccountRequirement { get; set; }
    //    public bool ActiveAccountNotRequired { get; set; }
    //    public bool AllowReenable { get; set; }
    //    public bool ActiveCard { get; set; }
    //    public bool ActiveBadge { get; set; }
    //    public bool HasBadge { get; set; }
    //    public DateTime? BadgeExpire { get; set; }
    //    public AccessCheckCard[] Cards { get; set; }
    //}

    //public class AccessCheckCard
    //{
    //    public int Number { get; set; }
    //    public DateTime? Expire { get; set; }
    //    public DateTime? LastAccess { get; set; }
    //    public string Status { get; set; }
    //}
}
