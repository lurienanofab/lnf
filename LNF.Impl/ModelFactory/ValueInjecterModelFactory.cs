using LNF.Impl.ModelFactory.Injections;
using LNF.Models.Data;
using LNF.Models.Ordering;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using LNF.Repository.Scheduler;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;

namespace LNF.Impl.ModelFactory
{
    //<lnf>
    //  <providers>
    //      <modelFactory type="LNF.Impl.ModelFactory.ValueInjecterModelFactory, LNF.Impl" />
    //  </providers>
    //</lnf>
    public class ValueInjecterModelFactory : IModelFactory
    {
        public ValueInjecterModelFactory()
        {
            //// xxxxx Data Maps
            Mapper.AddMap<ClientAccount, ClientAccountItem>(ModelBuilder.Data.CreateClientAccountModel);
            Mapper.AddMap<GlobalCost, GlobalCostItem>(ModelBuilder.Data.CreateGlobalCostModel);
            Mapper.AddMap<Room, RoomItem>(ModelBuilder.Data.CreateRoomModel);

            //// xxxxx Scheduler Maps
            Mapper.AddMap<Building, BuildingItem>(ModelBuilder.Scheduler.CreateBuildingModel);
            Mapper.AddMap<Lab, LabItem>(ModelBuilder.Scheduler.CreateLabModel);
            Mapper.AddMap<ProcessTech, ProcessTechItem>(ModelBuilder.Scheduler.CreateProcessTechModel);
            Mapper.AddMap<Resource, ResourceItem>(ModelBuilder.Scheduler.CreateResourceModel);
            Mapper.AddMap<ResourceInfo, ResourceItem>(ModelBuilder.Scheduler.CreateResourceModel);
            Mapper.AddMap<ProcessInfo, ProcessInfoItem>(ModelBuilder.Scheduler.CreateProcessInfoModel);
            Mapper.AddMap<ProcessInfoLine, ProcessInfoLineItem>(ModelBuilder.Scheduler.CreateProcessInfoLineModel);
            Mapper.AddMap<ResourceClient, ResourceClientItem>(ModelBuilder.Scheduler.CreateResourceClientModel);

            //// xxxxx Ordering Maps
            Mapper.AddMap<Approver, ApproverItem>(ModelBuilder.Ordering.CreateApproverModel);
        }

        private static readonly IValueInjection[] _injections =
        {
            new ExtendedFlatLoopInjection(),
            new NullableInjection(),
            new ClientInjection(),
            new ClientInfoInjection(),
            new AccountInjection(),
            new ReservationInjection()
        };

        public T Create<T>(object source)
        {
            if (source == null) return default(T);

            T result = Mapper.Map<T>(source);

            foreach (var injection in _injections)
            {
                result.InjectFrom(injection, source);
            }

            return result;
        }
    }
}
