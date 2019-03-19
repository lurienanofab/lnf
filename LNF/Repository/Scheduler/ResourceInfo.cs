using LNF.Models.Scheduler;
using System;

namespace LNF.Repository.Scheduler
{
    public class ResourceInfo : IDataItem
    {
        public virtual int ResourceID { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual int LabID { get; set; }
        public virtual int BuildingID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string LabName { get; set; }
        public virtual string LabDisplayName { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsSchedulable { get; set; }
        public virtual string Description { get; set; }
        public virtual string HelpdeskEmail { get; set; }
        public virtual string WikiPageUrl { get; set; }
        public virtual int State { get; set; }
        public virtual string StateNotes { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool AuthState { get; set; }
        public virtual int ReservFence { get; set; }
        public virtual int MaxAlloc { get; set; }
        public virtual int MinCancelTime { get; set; }
        public virtual int AutoEnd { get; set; }
        public virtual int? UnloadTime { get; set; }
        public virtual int Granularity { get; set; }
        public virtual int Offset { get; set; }
        public virtual bool IsReady { get; set; }
        public virtual int MinReservTime { get; set; }
        public virtual int MaxReservTime { get; set; }
        public virtual int GracePeriod { get; set; }
        public virtual int CurrentReservationID { get; set; }
        public virtual int CurrentClientID { get; set; }
        public virtual int CurrentActivityID { get; set; }
        public virtual string CurrentFirstName { get; set; }
        public virtual string CurrentLastName { get; set; }
        public virtual string CurrentActivityName { get; set; }
        public virtual bool CurrentActivityEditable { get; set; }
        public virtual DateTime? CurrentBeginDateTime { get; set; }
        public virtual DateTime? CurrentEndDateTime { get; set; }
        public virtual string CurrentNotes { get; set; }
        public virtual bool HasState(ResourceState state) => State == (int)state;

        public virtual DateTime GetNextGranularity(DateTime actual, int dir)
        {
            // get number of minutes between now and beginning of day (midnight + offset) of passed-in date
            DateTime dayBegin = new DateTime(actual.Year, actual.Month, actual.Day);
            dayBegin = dayBegin.AddHours(Offset);

            double repairBeginMinutes = actual.Subtract(dayBegin).TotalMinutes;

            if (repairBeginMinutes % Granularity == 0)
                return actual; // this is a granularity boundary
            else
            {
                int numOfGrans = Convert.ToInt32(repairBeginMinutes / Granularity);
                return dayBegin.AddMinutes((numOfGrans + dir) * Granularity);
            }
        }

        public virtual string GetImageUrl()
        {
            return string.Format("//ssel-sched.eecs.umich.edu/sselscheduler/images/Resource/Resource{0:000000}.png", ResourceID);
        }

        public override string ToString()
        {
            return ResourceItem.GetDisplayName(ResourceName, ResourceID);
        }
    }
}
