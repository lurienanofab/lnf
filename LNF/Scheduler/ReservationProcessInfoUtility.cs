using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ReservationProcessInfoUtility
    {
        public static IList<ReservationProcessInfo> SelectByReservation(int reservationId)
        {
            /*
             * sselScheduler.dbo.procReservationProcessInfoSelect
             * 
                SELECT PI.ProcessInfoID, PI.ProcessInfoName, PIL.Param, par.ParameterName,  RPI.*
	            FROM dbo.ReservationProcessInfo RPI, dbo.ProcessInfoLine PIL, ProcessInfoLineParam par,  dbo.ProcessInfo PI
	            WHERE RPI.ProcessInfoLineID = PIL.ProcessInfoLineID
	            AND PIL.ProcessInfoID = PI.ProcessInfoID
	            AND PIL.ProcessInfoLineParamID = par.ProcessInfoLineParamID 
	            AND ReservationID = @ReservationID 
            */

            return DA.Current.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == reservationId).ToList();
        }
    }
}
