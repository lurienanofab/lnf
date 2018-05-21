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
            Mapper.AddMap<Building, BuildingModel>(ModelBuilder.Scheduler.CreateBuildingModel);
            Mapper.AddMap<Lab, LabModel>(ModelBuilder.Scheduler.CreateLabModel);
            Mapper.AddMap<ProcessTech, ProcessTechModel>(ModelBuilder.Scheduler.CreateProcessTechModel);
            Mapper.AddMap<Resource, ResourceModel>(ModelBuilder.Scheduler.CreateResourceModel);
            Mapper.AddMap<ResourceInfo, ResourceModel>(ModelBuilder.Scheduler.CreateResourceModel);
            Mapper.AddMap<ProcessInfo, ProcessInfoModel>(ModelBuilder.Scheduler.CreateProcessInfoModel);
            Mapper.AddMap<ProcessInfoLine, ProcessInfoLineModel>(ModelBuilder.Scheduler.CreateProcessInfoLineModel);
            Mapper.AddMap<ResourceClient, ResourceClientModel>(ModelBuilder.Scheduler.CreateResourceClientModel);
            Mapper.AddMap<Cost, ResourceCostModel>(ModelBuilder.Scheduler.CreateResourceCostModel);

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
            new AccountInfoInjection(),
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
