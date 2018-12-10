using LNF.Repository;
using LNF.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.ModelFactory.Injections
{
    public class ReservationInjection : ExtendedKnownSourceInjection<Reservation>
    {
        protected IReservationManager ReservationManager => ServiceProvider.Current.Use<IReservationManager>();

        protected override void SetTarget(object target, Reservation obj)
        {
            SetTargetProperty(target, "ChargeBeginDateTime", obj, x => x.ChargeBeginDateTime());
            SetTargetProperty(target, "ChargeEndDateTime", obj, x => x.ChargeEndDateTime());
            SetTargetProperty(target, "ActivityAccountType", obj, x => x.Activity.AccountType);
            SetTargetProperty(target, "Invitees", obj, x => ReservationManager.GetInvitees(x.ReservationID).Model<ReservationInviteeItem>());
        }
    }
}
