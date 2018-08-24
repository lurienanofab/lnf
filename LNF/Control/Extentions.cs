using LNF.Repository;
using LNF.Repository.Control;
using System;
using System.Linq;

namespace LNF.Control
{
    public static class Extentions
    {
        public static Point GetPoint(this ActionInstance inst)
        {
            return ActionInstanceUtility.GetPoint(inst);
        }

        public static ActionInstance GetInstance(this Point point, ActionType action)
        {
            var inst = DA.Current.Query<ActionInstance>().FirstOrDefault(x => x.Point == point.PointID && x.ActionName == Enum.GetName(typeof(ActionType), action));
            return inst;
        }

        public static T EnsureSuccess<T>(this T resp) where T : ControlResponse
        {
            if (resp.Error)
                throw new Exception(resp.Message);
            return resp;
        }

        public static BlockResponse CreateBlockResponse(this Block block)
        {
            BlockResponse result = new BlockResponse
            {
                Error = false,
                Message = string.Empty,
                BlockState = null,
                StartTime = DateTime.Now,
                BlockID = 0
            };

            if (block != null)
            { 
                result.BlockID = block.BlockID;
                result.BlockState = block.CreateBlockState();
            }

            return result;
        }

        public static BlockResponse CreateBlockResponse(this Block block, Exception ex)
        {
            BlockResponse result = block.CreateBlockResponse();
            result.Error = true;
            result.Message = ex.Message;
            return result;
        }

        public static PointResponse CreatePointResponse(this Point point)
        {
            PointResponse result = new PointResponse
            {
                Error = false,
                Message = string.Empty,
                PointID = 0,
                BlockID = 0,
                StartTime = DateTime.Now
            };

            if (point != null)
            {
                result.BlockID = point.Block.BlockID;
                result.PointID = point.PointID;
            }

            return result;
        }

        public static PointResponse CreatePointResponse(this Point point, Exception ex)
        {
            PointResponse result = point.CreatePointResponse();
            result.Error = true;
            result.Message = ex.Message;
            return result;
        }

        public static BlockState CreateBlockState(this Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            BlockState result = new BlockState
            {
                BlockID = block.BlockID,
                BlockName = block.BlockName,
                IPAddress = block.IPAddress,
                Points = null
            };

            return result;
        }

        public static PointState CreatePointState(this Point point, bool state)
        {
            PointState result = new PointState
            {
                PointID = point.PointID,
                BlockID = point.Block.BlockID,
                PointName = point.Name,
                State = state
            };

            return result;
        }
    }
}
