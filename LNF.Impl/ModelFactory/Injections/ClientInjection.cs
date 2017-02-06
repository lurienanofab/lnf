using LNF.Data;
using LNF.Repository.Data;
using System;

namespace LNF.Impl.ModelFactory.Injections
{
    public class ClientInjection : ExtendedKnownSourceInjection<Client>
    {
        protected override void SetTarget(object target, Client obj)
        {
            ClientOrg primary = null;

            Func<ClientOrg> getPrimary = () =>
            {
                if (primary == null)
                    primary = ClientOrgUtility.GetPrimary(obj.ClientID);
                return primary;
            };

            SetTargetProperty(target, "TechnicalFieldName", obj, x => x.TechnicalFieldName());
            SetTargetProperty(target, "ClientOrgID", obj, x => getPrimary().ClientOrgID);
            SetTargetProperty(target, "Phone", obj, x => getPrimary().Phone);
            SetTargetProperty(target, "Email", obj, x => getPrimary().Email);
            SetTargetProperty(target, "OrgID", obj, x => getPrimary().Org.OrgID);
            SetTargetProperty(target, "OrgName", obj, x => getPrimary().Org.OrgName);
            SetTargetProperty(target, "ClientOrgActive", obj, x => getPrimary().Active);
            SetTargetProperty(target, "MaxChargeTypeID", obj, x => ClientOrgUtility.GetMaxChargeTypeID(x.ClientID));
        }
    }
}
