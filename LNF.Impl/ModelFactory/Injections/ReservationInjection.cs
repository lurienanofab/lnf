using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;

namespace LNF.Impl.ModelFactory.Injections
{
    public class ReservationInjection : ExtendedKnownSourceInjection<Reservation>
    {
        protected override void SetTarget(object target, Reservation obj)
        {
            SetTargetProperty(target, "ChargeBeginDateTime", obj, x => x.ChargeBeginDateTime());
            SetTargetProperty(target, "ChargeEndDateTime", obj, x => x.ChargeEndDateTime());
            SetTargetProperty(target, "Invitees", obj, x => x.GetInvitees().Model<ReservationInviteeItem>());
        }
    }
}
