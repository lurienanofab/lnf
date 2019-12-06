using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Linq;

namespace LNF.Impl.ModelFactory
{
    public class SchedulerModelBuilder : ModelBuilder
    {
        public SchedulerModelBuilder(ISession session) : base(session) { }

        private IProcessTech MapProcessTech(ProcessTech source)
        {
            var result = MapFrom<ProcessTechItem>(source);
            result.ProcessTechGroupName = source.Group.GroupName;
            result.BuildingID = source.Lab.Building.BuildingID;
            result.BuildingName = source.Lab.Building.BuildingName;
            result.BuildingDescription = source.Lab.Building.Description;
            result.BuildingIsActive = source.Lab.Building.IsActive;
            return result;
        }

        private IProcessInfoLine MapProcessInfoLine(ProcessInfoLine source)
        {
            var result = MapFrom<ProcessInfoLineItem>(source);
            result.ProcessInfoLineParamID = source.ProcessInfoLineParam.ProcessInfoLineParamID;
            result.ResourceID = source.ProcessInfoLineParam.Resource.ResourceID;
            result.ResourceName = source.ProcessInfoLineParam.Resource.ResourceName;
            return result;
        }

        private IReservationWithInvitees MapReservationWithInvitees(Reservation source)
        {
            var info = DA.Current.Single<ReservationInfo>(source.ReservationID);
            return MapReservationWithInvitees(info);
        }

        private IReservationWithInvitees MapReservationWithInvitees(ReservationInfo source)
        {
            var result = MapFrom<ReservationWithInviteesItem>(source);
            result.Invitees = DA.Current.Query<ReservationInviteeInfo>().Where(x => x.ReservationID == source.ReservationID).CreateModels<IReservationInvitee>();
            return result;
        }

        private IReservationRecurrence MapReservationRecurrence(ReservationRecurrence source)
        {
            var result = MapFrom<ReservationRecurrenceItem>(source);
            result.ResourceID = source.Resource.ResourceID;
            result.ResourceName = source.Resource.ResourceName;
            result.AccountID = source.Account.AccountID;
            result.AccountName = source.Account.Name;
            result.ActivityID = source.Activity.ActivityID;
            result.ActivityName = source.Activity.ActivityName;
            result.ClientID = source.Client.ClientID;
            result.LName = source.Client.LName;
            result.FName = source.Client.FName;
            result.PatternID = source.Pattern.PatternID;
            result.PatternName = source.Pattern.PatternName;
            return result;
        }

        public override void AddMaps()
        {
            Map<Building, BuildingItem, IBuilding>();
            Map<Lab, LabItem, ILab>();
            Map<ProcessTech, IProcessTech>(x => MapProcessTech(x));
            Map<Resource, ResourceInfo, ResourceItem, IResource>(x => x.ResourceID);
            Map<ResourceInfo, ResourceItem, IResource>();
            Map<ResourceTree, ResourceTreeItem, IResourceTree>();
            Map<Reservation, ReservationInfo, ReservationItem, IReservation>(x => x.ReservationID);
            Map<ReservationInfo, ReservationItem, IReservation>();
            Map<Reservation, IReservationWithInvitees>(x => MapReservationWithInvitees(x));
            Map<ReservationInfo, IReservationWithInvitees>(x => MapReservationWithInvitees(x));
            Map<ProcessInfo, ProcessInfoItem, IProcessInfo>();
            Map<ProcessInfoLine, IProcessInfoLine>(x => MapProcessInfoLine(x));
            Map<ResourceClient, ResourceClientInfo, ResourceClientItem, IResourceClient>(x => x.ResourceClientID);
            Map<ResourceClientInfo, ResourceClientItem, IResourceClient>();
            Map<ReservationInvitee, ReservationInviteeInfo, ReservationInviteeItem, IReservationInvitee>(x => new ReservationInviteeInfo { ReservationID = x.Reservation.ReservationID, InviteeID = x.Invitee.ClientID });
            Map<ReservationInviteeInfo, ReservationInviteeItem, IReservationInvitee>();
            Map<ReservationProcessInfo, ReservationProcessInfoItem, IReservationProcessInfo>();
            Map<ReservationRecurrenceInfo, ReservationRecurrenceItem, IReservationRecurrence>();
            Map<ReservationRecurrence, ReservationRecurrenceInfo, ReservationRecurrenceItem, IReservationRecurrence>(x => x.RecurrenceID);
            Map<Activity, ActivityItem, IActivity>();
        }
    }
}
