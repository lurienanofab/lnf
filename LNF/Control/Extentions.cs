using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Control;

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
            BlockResponse result = new BlockResponse();

            result.Error = false;
            result.Message = string.Empty;
            result.BlockState = block.CreateBlockState();
            result.StartTime = DateTime.Now;

            return result;
        }

        public static BlockResponse CreateBlockResponse(this Block block, Exception ex)
        {
            BlockResponse result = new BlockResponse();

            result.Error = true;
            result.Message = ex.Message;
            result.BlockState = block.CreateBlockState();
            result.StartTime = DateTime.Now;

            return result;
        }

        public static PointResponse CreatePointResponse(this Point point)
        {
            PointResponse result = new PointResponse();

            result.Error = false;
            result.Message = string.Empty;
            result.PointID = point.PointID;
            result.BlockID = point.Block.BlockID;
            result.PointName = point.Name;
            result.StartTime = DateTime.Now;

            return result;
        }

        public static PointResponse CreatePointResponse(this Point point, Exception ex)
        {
            PointResponse result = new PointResponse();

            result.Error = true;
            result.Message = ex.Message;
            result.PointID = point.PointID;
            result.BlockID = point.Block.BlockID;
            result.PointName = point.Name;
            result.StartTime = DateTime.Now;

            return result;
        }

        public static BlockState CreateBlockState(this Block block)
        {
            BlockState result = new BlockState();

            result.BlockID = block.BlockID;
            result.BlockName = block.BlockName;
            result.IPAddress = block.IPAddress;
            result.Points = null;

            return result;
        }

        public static PointState CreatePointState(this Point point, bool state)
        {
            PointState result = new PointState();

            result.PointID = point.PointID;
            result.BlockID = point.Block.BlockID;
            result.PointName = point.Name;
            result.State = state;

            return result;
        }
    }
}
