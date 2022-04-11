using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class Extensions
    {
        public static DateTime? OpenResSlot(this IEnumerable<FutureReservation> source, int resourceId, DateTime sd)
        {
            var query = source.Where(x => x.ResourceID == resourceId).OrderBy(x => x.BeginDateTime).ToList();

            for (int j = 1; j < query.Count - 1; j++)
            {
                // If there are other open reservation slots, then don't email reserver
                var curBeginDateTime = query[j].BeginDateTime;
                var lastEndDateTime = query[j - 1].EndDateTime;
                var minReservTime = TimeSpan.FromMinutes(query[j].MinReservTime);
                if (curBeginDateTime.Subtract(lastEndDateTime) >= minReservTime)
                    return null;
            }

            var followingReservations = query.Where(x => x.BeginDateTime >= sd).OrderBy(x => x.BeginDateTime).ToList();

            if (followingReservations.Count() == 0)
                // There are no other reservations behind it
                return null;
            else
                return followingReservations.First().BeginDateTime;
        }
    }
}
