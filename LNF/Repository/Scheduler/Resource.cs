using LNF.Models.Scheduler;
using LNF.Scheduler;
using System.Collections.Generic;

namespace LNF.Repository.Scheduler
{
    /// <summary>
    /// A tool or other item in the Scheduler system that can be reserved and interlocked
    /// </summary>
    public class Resource : IDataItem
    {
        /// <summary>
        /// The unique id of a resource.
        /// </summary>
        public virtual int ResourceID { get; set; }

        /// <summary>
        /// The process technology to which the resource belongs.
        /// </summary>
        public virtual ProcessTech ProcessTech { get; set; }

        /// <summary>
        /// The lab where the tool is located.
        /// </summary>
        public virtual Lab Lab { get; set; }

        /// <summary>
        /// The name of the resource.
        /// </summary>
        public virtual string ResourceName { get; set; }

        /// <summary>
        /// The per use cost of the resource.
        /// </summary>
        public virtual decimal UseCost { get; set; }

        /// <summary>
        /// The hourly cost of the resource.
        /// </summary>
        public virtual decimal HourlyCost { get; set; }

        /// <summary>
        /// The number of minutes from the current time during which the resource can be reserved.
        /// </summary>
        public virtual int ReservFence { get; set; }

        /// <summary>
        /// The time increment in minutes a resource can be reserved.
        /// </summary>
        public virtual int Granularity { get; set; }

        /// <summary>
        /// The offset hours that specify the beginning of the day for the resource.
        /// </summary>
        public virtual int Offset { get; set; }

        /// <summary>
        /// The minimum number of minutes the resource can be reserved.
        /// </summary>
        public virtual int MinReservTime { get; set; }

        /// <summary>
        /// The maximum number of minutes the resource can be reserved.
        /// </summary>
        public virtual int MaxReservTime { get; set; }

        /// <summary>
        /// The maximum number of reserved minutes allowed per user.
        /// </summary>
        public virtual int MaxAlloc { get; set; }

        /// <summary>
        /// The number of minutes before the start of a reservation during which a user can still cancel the reservation.
        /// </summary>
        public virtual int MinCancelTime { get; set; }

        /// <summary>
        /// The number of minutes after the start of a reservation during which a user can still start the reservation.
        /// </summary>
        public virtual int GracePeriod { get; set; }

        /// <summary>
        /// The number of minutes after the end of a reservation after which the reservation will be automatically ended.
        /// </summary>
        public virtual int AutoEnd { get; set; }

        /// <summary>
        /// The number of months a user authorization lasts before expiring.
        /// </summary>
        public virtual int AuthDuration { get; set; }

        /// <summary>
        /// Indicates whether or not the authorization duration is a rolling period (authorization is extended whenever the user activates a reservation).
        /// </summary>
        public virtual bool AuthState { get; set; }

        /// <summary>
        /// Indicates if the resource is Online or Offline.
        /// </summary>
        public virtual ResourceState State { get; set; }

        /// <summary>
        /// Indicates if the resource is currently usable.
        /// </summary>
        public virtual bool IsReady { get; set; }

        /// <summary>
        /// Indicates if the resource can be scheduled.
        /// </summary>
        public virtual bool IsSchedulable { get; set; }

        /// <summary>
        /// A description of the resource.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Indicates if the resource is active (displayed on the Scheduler).
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// The time in minutes on-the-fly reservations are scheduled.
        /// </summary>
        public virtual int? OTFSchedTime { get; set; }

        /// <summary>
        /// The IP Address of the on-the-fly device.
        /// </summary>
        public virtual string IPAddress { get; set; }

        /// <summary>
        /// The time in minutes taken to unload.
        /// </summary>
        public virtual int? UnloadTime { get; set; }

        /// <summary>
        /// A note describing the current state of the resource.
        /// </summary>
        public virtual string StateNotes { get; set; }

        /// <summary>
        /// The email address of the helpdesk queue.
        /// </summary>
        public virtual string HelpdeskEmail { get; set; }

        /// <summary>
        /// The wiki page url for the resource.
        /// </summary>
        public virtual string WikiPageUrl { get; set; }

        public override string ToString()
        {
            return ResourceItem.GetResourceDisplayName(ResourceName, ResourceID);
        }
    }
}
