using LNF.Repository;
using LNF.Repository.Scheduler;
using System;

namespace LNF.Scheduler
{
    public static class ReservationGroupUtility
    {
        public static void Update(int groupId, DateTime beginDateTime, DateTime endDateTime)
        {
            // This is all that happens in procReservationGroupUpdate @Action = 'ByGroupID'

            //UPDATE [sselScheduler].[dbo].[ReservationGroup]
            //SET [BeginDateTime] = @BeginDateTime
            //    ,[EndDateTime] = @EndDateTime
            //WHERE GroupID = @GroupID

            ReservationGroup rg = DA.Current.Single<ReservationGroup>(groupId);
            rg.BeginDateTime = beginDateTime;
            rg.EndDateTime = endDateTime;
        }
    }
}
