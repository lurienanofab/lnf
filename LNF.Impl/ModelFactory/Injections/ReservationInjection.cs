using LNF.Models.Scheduler;
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
            SetTargetProperty(target, "Invitees", obj, x => x.GetInvitees().Model<ReservationInviteeModel>());
        }
    }
}
