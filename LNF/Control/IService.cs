using LNF.Repository.Control;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;

namespace LNF.Control
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypeProvider))]
    public interface IService
    {
        [OperationContract]
        BlockResponse GetBlockState(int blockId);

        [OperationContract]
        PointResponse SetPointState(int blockId, int pointId, bool state, uint duration = 0);

        [OperationContract]
        PointResponse Cancel(int blockId, int pointId);
    }

    public static class KnownTypeProvider
    {
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return new Type[]
            {
                typeof(ControlResponse),
                typeof(PointResponse),
                typeof(BlockResponse),
                typeof(PointState),
                typeof(BlockState),
                typeof(ActionType),
                typeof(Point),
                typeof(Block),
                typeof(ActionInstance)
            };
        }
    }
}
