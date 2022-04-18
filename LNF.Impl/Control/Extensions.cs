using LNF.Control;
using LNF.Impl.Repository.Control;
using System;

namespace LNF.Impl.Control
{
    public static class Extensions
    {
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

        public static PointState CreatePointState(this Point point, bool state)
        {
            PointState result = new PointState
            {
                PointID = point.PointID,
                BlockID = point.Block.BlockID,
                State = state
            };

            return result;
        }
    }
}
