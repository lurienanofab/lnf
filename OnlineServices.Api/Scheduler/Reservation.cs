using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class Reservation : ReservationItem, IReservation
    {
        public bool IsUnloaded { get; set; }
        public double MaxReservedDuration { get; set; }
        public DateTime? OriginalBeginDateTime { get; set; }
        public DateTime? OriginalEndDateTime { get; set; }
        public DateTime? OriginalModifiedOn { get; set; }
        public bool ResourceIsActive { get; set; }
        public bool IsSchedulable { get; set; }
        public string ResourceDescription { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }
        public ResourceState State { get; set; }
        public string StateNotes { get; set; }
        public int MaxAlloc { get; set; }
        public int? UnloadTime { get; set; }
        public int? OTFSchedTime { get; set; }
        public bool IsReady { get; set; }
        public int ProcessTechGroupID { get; set; }
        public string ProcessTechGroupName { get; set; }
        public string ProcessTechDescription { get; set; }
        public bool ProcessTechIsActive { get; set; }
        public string ProcessTechName { get; set; }
        public string LabName { get; set; }
        public string LabDescription { get; set; }
        public string LabDisplayName { get; set; }
        public bool LabIsActive { get; set; }
        public string BuildingDescription { get; set; }
        public bool BuildingIsActive { get; set; }
        public string BuildingName { get; set; }
        public int ClientOrgID { get; set; }
        public int ClientAddressID { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public DateTime? SubsidyStartDate { get; set; }
        public DateTime? NewFacultyStartDate { get; set; }
        public bool ClientOrgActive { get; set; }
        public string ClientBeginLName { get; set; }
        public string ClientBeginFName { get; set; }
        public string ClientEndLName { get; set; }
        public string ClientEndFName { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string RoomDisplayName { get; set; }
        public string GetResourceName(ResourceNamePartial part) => Resources.GetResourceName(this, part);
        public bool HasState(ResourceState state) => Resources.HasState(State, state);
    }

    public class ReservationWithInvitees : Reservation, IReservationWithInvitees
    {
        public IEnumerable<IReservationInviteeItem> Invitees { get; set; }
    }
}
