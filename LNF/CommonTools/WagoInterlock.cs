using LNF.Control;
using System;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public struct InterlockData
    {
        public int BlockID { get; }
        public BlockState BlockState { get; }
        public string Message { get; }

        public InterlockData(int blockId, BlockState state, string message)
        {
            BlockID = blockId;
            BlockState = state;
            Message = message;
        }
    }

    public static class WagoInterlock
    {
        // Turn On/Off the points that are associated with the resource
        public static bool ToggleInterlock(int resourceId, bool state, uint duration)
        {
            // Not every tool has an interlock setup, so only set point state if
            // an ActionInstance is found. It's not an error if one is not found.

            var inst = ActionInstances.Find(ActionType.Interlock, resourceId);

            if (inst != null)
            {
                ServiceProvider.Current.Control.SetPointState(inst.Point, state, duration);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ByPass(int actionId, uint duration)
        {
            var inst = ActionInstances.Find(ActionType.ByPass, actionId);

            if (inst != null)
                ServiceProvider.Current.Control.SetPointState(inst.Point, true, duration);
        }

        // Get Block data
        public static InterlockData GetInterlockData(int resourceId, ActionType action = ActionType.Interlock, string username = "", string password = "")
        {
            var inst = ActionInstances.Find(action, resourceId);

            BlockState blockState = null;

            if (inst == null)
                return new InterlockData(0, null, "No Interlock is associated with this resource.");

            IPoint point = ServiceProvider.Current.Control.GetPoint(inst.Point);

            string message = string.Empty;

            int blockId = 0;

            try
            {
                blockId = point.BlockID;
                BlockResponse resp = ServiceProvider.Current.Control.GetBlockState(blockId);
                blockState = resp.BlockState;

                if (blockState == null)
                    message = "Failed to connect with WAGO block.<br />Please try to reload the page and repeat your action again (click the Maintenance tab above).<br />If the problem persists, please contact administrator.";
            }
            catch (Exception ex)
            {
                message = string.Format("Wago block {0} fault detected. Please contact system administrator. [{0}]", blockId, ex.Message);
            }

            return new InterlockData(blockId, blockState, message);
        }

        public static void AllToolStatus(DataTable dtToolStatus)
        {
            if (!dtToolStatus.Columns.Contains("PointID"))
                dtToolStatus.Columns.Add("PointID", typeof(int));
            if (!dtToolStatus.Columns.Contains("InterlockStatus"))
                dtToolStatus.Columns.Add("InterlockStatus", typeof(string));
            if (!dtToolStatus.Columns.Contains("InterlockState"))
                dtToolStatus.Columns.Add("InterlockState", typeof(bool));
            if (!dtToolStatus.Columns.Contains("InterlockError"))
                dtToolStatus.Columns.Add("InterlockError", typeof(bool));
            if (!dtToolStatus.Columns.Contains("IsInterlocked"))
                dtToolStatus.Columns.Add("IsInterlocked", typeof(bool));

            var toolStatus = ServiceProvider.Current.Control.GetToolStatus();

            foreach (DataRow dr in dtToolStatus.Rows)
            {
                if (dr["ResourceID"] != DBNull.Value)
                {
                    dr.SetField("InterlockStatus", "Blank");
                    dr.SetField("InterlockState", false);
                    dr.SetField("InterlockError", false);
                    dr.SetField("IsInterlocked", true);

                    var ts = toolStatus.FirstOrDefault(x => x.ResourceID == dr.Field<int>("ResourceID"));

                    if (ts != null)
                    {
                        dr.SetField("PointID", ts.PointID);
                        dr.SetField("InterlockStatus", ts.InterlockStatus);
                        dr.SetField("InterlockState", ts.InterlockState);
                        dr.SetField("InterlockError", ts.InterlockError);
                        dr.SetField("IsInterlocked", ts.IsInterlocked);
                    }
                    else
                    {
                        //this is not an error
                        dr.SetField("PointID", 0);
                        dr.SetField("InterlockStatus", "No interlock for resource.");
                        dr.SetField("IsInterlocked", false);
                    }
                }
            }
        }

        public static BlockState GetBlockState(int blockId)
        {
            BlockResponse resp = ServiceProvider.Current.Control.GetBlockState(blockId);
            return resp.EnsureSuccess().BlockState;
        }

        // Parses the passed data buffer, and returns the state of a point(word, bit) on the block
        public static bool GetPointState(int pointId, BlockState blockState)
        {
            PointState ps = blockState.Points.First(x => x.PointID == pointId);
            return ps.State;
        }

        public static bool GetPointState(int resourceId)
        {
            var inst = ActionInstances.Find(ActionType.Interlock, resourceId);

            if (inst == null)
                throw new ArgumentException(string.Format("No record found for ResourceID {0}", resourceId));

            IPoint p = ServiceProvider.Current.Control.GetPoint(inst.Point);

            var bs = GetBlockState(p.BlockID);

            return GetPointState(p.PointID, bs);
        }

        public static int GetAnalogPointState(IActionInstance inst, BlockState blockState = null)
        {
            throw new NotImplementedException();
        }
    }
}
