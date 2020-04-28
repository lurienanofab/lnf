using System;

namespace LNF.Control
{
    public static class Extentions
    {
        public static IPoint GetPoint(this IActionInstance inst)
        {
            return ActionInstances.GetPoint(inst);
        }

        public static IActionInstance GetInstance(this IPoint point, ActionType action)
        {
            return ServiceProvider.Current.Control.GetActionInstance(action, point);
        }

        public static T EnsureSuccess<T>(this T resp) where T : ControlResponse
        {
            if (resp.Error)
                throw new Exception(resp.Message);
            return resp;
        }

        public static BlockResponse CreateBlockResponse(this IBlock block)
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

        public static BlockResponse CreateBlockResponse(this IBlock block, Exception ex)
        {
            BlockResponse result = block.CreateBlockResponse();
            result.Error = true;
            result.Message = ex.Message;
            return result;
        }

        public static PointResponse CreatePointResponse(this IPoint point)
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
                result.BlockID = point.BlockID;
                result.PointID = point.PointID;
            }

            return result;
        }

        public static PointResponse CreatePointResponse(this IPoint point, Exception ex)
        {
            PointResponse result = point.CreatePointResponse();
            result.Error = true;
            result.Message = ex.Message;
            return result;
        }

        public static BlockState CreateBlockState(this IBlock block)
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

        public static PointState CreatePointState(this IPoint point, bool state)
        {
            PointState result = new PointState
            {
                PointID = point.PointID,
                BlockID = point.BlockID,
                State = state
            };

            return result;
        }
    }
}
