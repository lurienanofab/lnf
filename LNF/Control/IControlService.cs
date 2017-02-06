using LNF.Repository.Control;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;

namespace LNF.Control
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypeProvider))]
    public interface IControlService
    {
        [OperationContract]
        PointResponse SetPointState(Point point, bool state, uint duration);

        [OperationContract]
        PointResponse Cancel(Point point);

        [OperationContract]
        BlockResponse GetBlockState(Block block);
    }

    public static class KnownTypeProvider
    {
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return new Type[]
            {
                typeof(LNF.Control.ControlResponse),
                typeof(LNF.Control.PointResponse),
                typeof(LNF.Control.BlockResponse),
                typeof(LNF.Control.PointState),
                typeof(LNF.Control.BlockState),
                typeof(LNF.Control.ActionType),
                typeof(LNF.Repository.Control.Point),
                typeof(LNF.Repository.Control.Block),
                typeof(LNF.Repository.Control.ActionInstance)
            };
        }
    }
}
