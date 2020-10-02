using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using Omu.ValueInjecter;
using System.Linq;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public class SchedulerModelBuilder : ModelBuilder
    {
        public SchedulerModelBuilder(ISessionManager mgr) : base(mgr) { }

        private ILab MapLab(Lab source)
        {
            var result = MapFrom<LabItem>(source);
            result.BuildingID = source.Building.BuildingID;
            result.BuildingName = source.Building.BuildingName;
            result.BuildingDescription = source.Building.BuildingDescription;
            result.BuildingIsActive = source.Building.BuildingIsActive;
            return result;
        }

        private IProcessTech MapProcessTech(ProcessTech source)
        {
            var result = MapFrom<ProcessTechItem>(source);
            result.ProcessTechGroupID = source.Group.ProcessTechGroupID;
            result.ProcessTechGroupName = source.Group.GroupName;
            result.BuildingID = source.Lab.Building.BuildingID;
            result.BuildingName = source.Lab.Building.BuildingName;
            result.BuildingDescription = source.Lab.Building.BuildingDescription;
            result.BuildingIsActive = source.Lab.Building.BuildingIsActive;
            result.LabID = source.Lab.LabID;
            result.LabName = source.Lab.LabName;
            result.LabDisplayName = source.Lab.DisplayName;
            result.LabDescription = source.Lab.Description;
            result.LabIsActive = source.Lab.IsActive;
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
            var info = Session.Get<ReservationInfo>(source.ReservationID);
            return MapReservationWithInvitees(info);
        }

        private IReservationWithInvitees MapReservationWithInvitees(ReservationInfo source)
        {
            var result = MapFrom<ReservationWithInviteesItem>(source);
            result.Invitees = Session.Query<ReservationInviteeInfo>().Where(x => x.ReservationID == source.ReservationID).ToList();
            return result;
        }

        private IReservationInvitee MapReservationInvitee(ReservationInvitee source)
        {
            var reservationInfo = Session.Get<ReservationInfo>(source.Reservation.ReservationID);

            var result = new ReservationInviteeInfo();
            result.InjectFrom(reservationInfo);

            result.InviteeID = source.Invitee.ClientID;
            result.InviteeLName = source.Invitee.LName;
            result.InviteeLName = source.Invitee.FName;
            result.InviteePrivs = source.Invitee.Privs;
            result.InviteeActive = source.Invitee.Active;

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
            Map<Lab, ILab>(MapLab);
            Map<ProcessTech, IProcessTech>(MapProcessTech);
            Map<Resource, ResourceInfo, ResourceItem, IResource>(x => x.ResourceID);
            Map<ResourceInfo, ResourceItem, IResource>();
            Map<ResourceClient, ResourceClientInfo, IResourceClient>(x => x.ResourceClientID);
            Map<Reservation, ReservationInfo, ReservationItem, IReservation>(x => x.ReservationID);
            Map<ReservationInfo, ReservationItem, IReservation>();
            Map<Reservation, IReservationWithInvitees>(MapReservationWithInvitees);
            Map<ReservationInfo, IReservationWithInvitees>(MapReservationWithInvitees);
            Map<ReservationInvitee, IReservationInvitee>(MapReservationInvitee);
            Map<ProcessInfo, ProcessInfoItem, IProcessInfo>();
            Map<ProcessInfoLine, IProcessInfoLine>(MapProcessInfoLine);
            Map<ReservationProcessInfo, ReservationProcessInfoItem, IReservationProcessInfo>();
            Map<ReservationRecurrenceInfo, ReservationRecurrenceItem, IReservationRecurrence>();
            Map<ReservationRecurrence, ReservationRecurrenceInfo, ReservationRecurrenceItem, IReservationRecurrence>(x => x.RecurrenceID);
            Map<Activity, ActivityItem, IActivity>();
        }
    }
}
