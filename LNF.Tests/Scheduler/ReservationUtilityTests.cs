using LNF.Repository;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LNF.Tests.Scheduler
{
    [TestClass]
    public class ReservationUtilityTests
    {
        [TestInitialize]
        public void Setup()
        {
            var context = ((DefaultContext)Providers.Context);
            context.SetApplicationVariable("SchedulerEmail", "labscheduler@eecs.umich.edu");
        }

        [TestMethod]
        public void CanCancel_EmailOnOpenSlot()
        {
            ReservationUtility.DeleteReservation(566446);
        }

        [TestMethod]
        public void CanEndUnstartedReservations()
        {
            Reservation resv = DA.Current.Single<Reservation>(14021);
            List<Reservation> res = new List<Reservation>();
            res.Add(resv);
            EndUnstartedReservations(res);
        }


        public static void EndUnstartedReservations(IEnumerable<Reservation> reservations)
        {
            //End unstarted reservations
            //Providers.Log.Write(LogMessageLevel.Info, string.Format("Begin EndUnstartedReservations [UnstartedReservations Count: {0}]", reservations.Count()));
            foreach (Reservation rsv in reservations)
            {
                try
                {
                    DateTime OldEndDateTime = rsv.EndDateTime;
                    DateTime NewEndDateTime = rsv.BeginDateTime;

                    bool endReservation = false;
                    DateTime eDate;

                    if (rsv.KeepAlive)
                    {
                        //KeepAlive: we don't care about GracePeriod, only AutoEnd
                        if (rsv.Resource.AutoEnd <= 0 || rsv.AutoEnd)
                            eDate = rsv.EndDateTime;
                        else
                            eDate = rsv.EndDateTime.AddMinutes(rsv.Resource.AutoEnd);

                        if (eDate <= DateTime.Now)
                        {
                            endReservation = true;
                            NewEndDateTime = eDate;
                        }
                    }
                    else
                    {
                        //The end datetime will be the scheduled begin datetime plus the great period
                        eDate = NewEndDateTime.AddMinutes(rsv.Resource.GracePeriod);
                        endReservation = true;
                        NewEndDateTime = eDate;
                    }

                    if (endReservation)
                    {
                        //ReservationUtility.EndPastUnstarted(rsv, NewEndDateTime, -1);
                        //Providers.Log.Write(LogMessageLevel.Warning, string.Format("Unstarted reservation {0} was ended, KeepAlive = {1}, Reservation.AutoEnd = {2}, Resource.AutoEnd = {3}, eDate = #{4}#", rsv.ReservationID, rsv.KeepAlive, rsv.AutoEnd, rsv.Resource.AutoEnd, eDate));

                        DateTime? NextBeginDateTime = ReservationUtility.OpenResSlot(rsv.Resource.ResourceID, TimeSpan.FromMinutes(rsv.Resource.ReservFence), TimeSpan.FromMinutes(rsv.Resource.MinReservTime), DateTime.Now, OldEndDateTime);

                        DateTime dt = NewEndDateTime;
                        //Check if reservation slot becomes big enough
                        if (NextBeginDateTime.HasValue)
                        {
                            if (NextBeginDateTime.Value.Subtract(OldEndDateTime).TotalMinutes < rsv.Resource.MinReservTime
                                && NextBeginDateTime.Value.Subtract(NewEndDateTime).TotalMinutes >= rsv.Resource.MinReservTime)
                                EmailUtility.EmailOnOpenReservations(rsv.Resource.ResourceID, NewEndDateTime, NextBeginDateTime.Value);
                        }
                    }
                    //else
                    //Providers.Log.Write(LogMessageLevel.Warning, string.Format("Unstarted reservation {0} was not ended, KeepAlive = {1}, Reservation.AutoEnd = {2}, Resource.AutoEnd = {3}, eDate = #{4}#", rsv.ReservationID, rsv.KeepAlive, rsv.AutoEnd, rsv.Resource.AutoEnd, eDate));
                }
                catch (Exception ex)
                {
                    var thisGetsRidOfAnnoyingWarning = ex;
                    //Providers.Log.Write(LogMessageLevel.Error, string.Format("EndUnstartedReservations: Resource {0}: {1}", rsv.Resource.ResourceID, ex.Message));
                }
            }
            //Providers.Log.Write(LogMessageLevel.Info, "EndUnstartedReservations Complete");
        }
    }
}
