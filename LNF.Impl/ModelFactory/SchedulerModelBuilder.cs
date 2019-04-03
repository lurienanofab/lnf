using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Linq;

namespace LNF.Impl.ModelFactory
{
    public class SchedulerModelBuilder : ModelBuilder
    {
        public SchedulerModelBuilder(ISession session) : base(session) { }

        private ILab CreateLabModel(Lab source)
        {
            int roomId = 0;
            string roomName = string.Empty;
            string roomDisplayName = string.Empty;

            if (source.Room != null)
            {
                roomId = source.Room.RoomID;
                roomName = source.Room.RoomName;
                roomDisplayName = source.Room.DisplayName;
            }

            var result = CreateModelFrom<LabItem>(source);
            result.RoomID = roomId;
            result.RoomName = roomName;
            result.RoomDisplayName = roomDisplayName;
            return result;
        }

        private IProcessTech CreateProcessTechModel(ProcessTech source)
        {
            var result = CreateModelFrom<ProcessTechItem>(source);
            result.ProcessTechGroupName = source.Group.GroupName;
            result.BuildingID = source.Lab.Building.BuildingID;
            result.BuildingName = source.Lab.Building.BuildingName;
            result.BuildingDescription = source.Lab.Building.Description;
            result.BuildingIsActive = source.Lab.Building.IsActive;
            result.RoomID = source.Lab.Room.RoomID;
            result.RoomName = source.Lab.Room.RoomName;
            result.RoomDisplayName = source.Lab.Room.DisplayName;
            return result;
        }

        private IProcessInfoLine CreateProcessInfoLineModel(ProcessInfoLine source)
        {
            var result = CreateModelFrom<ProcessInfoLineItem>(source);
            result.ProcessInfoLineParamID = source.ProcessInfoLineParam.ProcessInfoLineParamID;
            result.ResourceID = source.ProcessInfoLineParam.Resource.ResourceID;
            result.ResourceName = source.ProcessInfoLineParam.Resource.ResourceName;
            return result;
        }

        private IReservationWithInvitees CreateReservationWithInviteesModel(Reservation source)
        {
            var info = DA.Current.Single<ReservationInfo>(source.ReservationID);
            return CreateReservationWithInviteesModel(info);
        }

        private IReservationWithInvitees CreateReservationWithInviteesModel(ReservationInfo source)
        {
            var result = CreateModelFrom<ReservationItemWithInvitees>(source);
            result.Invitees = DA.Current.Query<ReservationInviteeInfo>().Where(x => x.ReservationID == source.ReservationID).CreateModels<IReservationInvitee>();
            return result;
        }

        public override void AddMaps()
        {
            Map<Building, IBuilding>(x => CreateModelFrom<BuildingItem>(x));
            Map<Lab, ILab>(x => CreateLabModel(x));
            Map<ProcessTech, IProcessTech>(x => CreateProcessTechModel(x));
            Map<Resource, IResource>(x => CreateModelFrom<ResourceInfo, ResourceItem>(x.ResourceID));
            Map<ResourceInfo, IResource>(x => CreateModelFrom<ResourceItem>(x));
            Map<Reservation, IReservation>(x => CreateModelFrom<ReservationInfo, ReservationItem>(x.ReservationID));
            Map<ReservationInfo, IReservation>(x => CreateModelFrom<ReservationItem>(x));
            Map<Reservation, IReservationWithInvitees>(x => CreateReservationWithInviteesModel(x));
            Map<ReservationInfo, IReservationWithInvitees>(x => CreateReservationWithInviteesModel(x));
            Map<ProcessInfo, IProcessInfo>(x => CreateModelFrom<ProcessInfoItem>(x));
            Map<ProcessInfoLine, IProcessInfoLine>(x => CreateProcessInfoLineModel(x));
            Map<ResourceClient, IResourceClient>(x => CreateModelFrom<ResourceClientInfo, ResourceClientItem>(x.ResourceClientID));
            Map<ResourceClientInfo, IResourceClient>(x => CreateModelFrom<ResourceClientItem>(x));
        }
    }
}
