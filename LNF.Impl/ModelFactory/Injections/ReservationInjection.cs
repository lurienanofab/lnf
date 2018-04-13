using LNF.Repository;
using LNF.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.ModelFactory.Injections
{
    public class ReservationInjection : ExtendedKnownSourceInjection<Reservation>
    {
        protected override void SetTarget(object target, Reservation obj)
        {
            SetTargetProperty(target, "ChargeBeginDateTime", obj, x => x.ChargeBeginDateTime());
            SetTargetProperty(target, "ChargeEndDateTime", obj, x => x.ChargeEndDateTime());
            SetTargetProperty(target, "Invitees", obj, x => DA.Current.ReservationManager().GetInvitees(x).Model<Models.Scheduler.ReservationInviteeItem>());
        }
    }
}
