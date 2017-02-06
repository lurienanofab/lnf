using LNF.Repository.Data;
using System;

namespace LNF.Impl.ModelFactory.Injections
{
    public class ClientInfoInjection : ExtendedKnownSourceInjection<ClientInfo>
    {
        protected override void SetTarget(object target, ClientInfo obj)
        {
            SetTargetProperty(target, "TechnicalFieldID", obj, x => x.TechnicalInterestID);
            SetTargetProperty(target, "TechnicalFieldName", obj, x => x.TechnicalInterestName);
        }
    }
}
