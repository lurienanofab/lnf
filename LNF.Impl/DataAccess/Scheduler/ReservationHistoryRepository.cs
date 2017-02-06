using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Context;
using LNF.Repository.Scheduler;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class ReservationHistoryRepository<TContext> : Repository<TContext, ReservationHistory>, IReservationHistoryRepository
        where TContext : ICurrentSessionContext
    {
        public ReservationHistory Insert(string action, string actionSource, Reservation rsv, int? modifiedByClientId = null, int? linkedReservationId = null)
        {
            // procReservationHistoryInsert [note that @Reservations is a user-defined type (basically a temp table)]

            //INSERT dbo.ReservationHistory (ReservationID, LinkedReservationID, UserAction, ActionSource, ModifiedByClientID, ModifiedDateTime, AccountID, BeginDateTime, EndDateTime, ChargeMultiplier)
            //SELECT r.ReservationID, r.LinkedReservationID, @UserAction, @ActionSource, r.ClientID, GETDATE(), r.AccountID, r.BeginDateTime, r.EndDateTime, r.ChargeMultiplier
            //FROM @Reservations r

            ReservationHistory result = new ReservationHistory()
            {
                Reservation = rsv,
                LinkedReservationID = linkedReservationId,
                UserAction = action,
                ActionSource = actionSource,
                ModifiedByClientID = modifiedByClientId,
                ModifiedDateTime = DateTime.Now,
                Account = rsv.Account,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ChargeMultiplier = rsv.ChargeMultiplier
            };

            Session.SaveOrUpdate(result);

            return result;
        }
    }
}
