using LNF.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.ModelFactory.Injections
{
    internal class ReservationInfoInjection : ExtendedKnownSourceInjection<ReservationInfo>
    {
        protected override void SetTarget(object target, ReservationInfo obj)
        {
            var mgr = ServiceProvider.Current.Use<IReservationManager>();
            SetTargetProperty(target, "Invitees", obj, x => mgr.GetInvitees(x.ReservationID));
        }
    }
}
