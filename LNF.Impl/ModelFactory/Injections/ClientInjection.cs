using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;

namespace LNF.Impl.ModelFactory.Injections
{
    public class ClientInjection : ExtendedKnownSourceInjection<Client>
    {
        protected override void SetTarget(object target, Client obj)
        {
            ClientOrg primary = null;

            var mgr = DA.Current.ClientOrgManager();

            Func<ClientOrg> getPrimary = () =>
            {
                if (primary == null)
                    primary = mgr.GetPrimary(obj.ClientID);
                return primary;
            };

            SetTargetProperty(target, "TechnicalFieldName", obj, x => DA.Current.ClientManager().TechnicalFieldName(x));
            SetTargetProperty(target, "ClientOrgID", obj, x => getPrimary().ClientOrgID);
            SetTargetProperty(target, "Phone", obj, x => getPrimary().Phone);
            SetTargetProperty(target, "Email", obj, x => getPrimary().Email);
            SetTargetProperty(target, "OrgID", obj, x => getPrimary().Org.OrgID);
            SetTargetProperty(target, "OrgName", obj, x => getPrimary().Org.OrgName);
            SetTargetProperty(target, "ClientOrgActive", obj, x => getPrimary().Active);
            SetTargetProperty(target, "MaxChargeTypeID", obj, x => mgr.GetMaxChargeTypeID(x.ClientID));
        }
    }
}
